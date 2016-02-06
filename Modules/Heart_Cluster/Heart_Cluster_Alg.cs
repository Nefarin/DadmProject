using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Ink;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.IO;
using System.Globalization;

namespace EKG_Project.Modules.Heart_Cluster
{
    public partial class Heart_Cluster : IModule
    {
        private List<Vector<double>> centroids = new List<Vector<double>>();
        private List<int> classCounts = new List<int>();

        public void Klasteryzacja(List<Vector<double>> coeffs, double threshold)
        {
            List<Vector<double>> centroids = new List<Vector<double>>(coeffs.Take(1));
            List<int> classCounts = new List<int>() { 1 };

            foreach (var current_vector in coeffs.Skip(1))
            {

                var x = centroids.Aggregate((a, b) => a.Subtract(current_vector).L2Norm() < b.Subtract(current_vector).L2Norm() ? a : b);


                if (x.Subtract(current_vector).L2Norm() < threshold)
                {
                    var i = centroids.FindIndex(v => v.Equals(x));
                    centroids[i] = (centroids[i] * classCounts[i] + current_vector) / (classCounts[i] + 1);
                    classCounts[i]++;
                }
                else
                {
                    centroids.Add(current_vector);
                    classCounts.Add(1);
                }
            }

            var howManyClassTypes = classCounts.Count;

        }




        #region Documentation

        /// <summary>
        /// TODO 
        /// </summary>
        /// <param name="loadedSignal"></param>
        /// <param name="fs"></param>
        /// <param name="R"></param>
        /// <param name="qrsOnset"></param>
        /// <param name="qrsEnd"></param>
        /// <returns></returns>

        #endregion
        Tuple<int, int, int, int> Classification(Vector<double> loadedSignal, int qrsOnset, int qrsEnd, int qrsR, uint fs)
        {
            var onsetAndEnd = RepairQrsComplex(qrsOnset, qrsEnd, qrsR, fs);

            qrsOnset = onsetAndEnd.Item1;
            qrsEnd = onsetAndEnd.Item2;

            var singleQrsComplex = loadedSignal.SubVector(qrsOnset, qrsEnd - qrsOnset + 1);
            var coefficientVector = CountCoefficients(singleQrsComplex, fs);

            var classification = ClassificateComplex(coefficientVector, 14);
            return new Tuple<int, int, int, int>(qrsOnset, qrsEnd, qrsR, classification);
        }


        private int ClassificateComplex(Vector<double> coefficientVector, double threshold)
        {

            if (centroids.Count != 0)
            {
                var closestCentroid = centroids.Aggregate((a, b) => a.Subtract(coefficientVector).L2Norm() < b.Subtract(coefficientVector).L2Norm() ? a : b);

                if (closestCentroid.Subtract(coefficientVector).L2Norm() < threshold)
                {
                    var i = centroids.FindIndex(v => v.Equals(closestCentroid));
                    centroids[i] = (centroids[i] * classCounts[i] + coefficientVector) / (classCounts[i] + 1);
                    classCounts[i]++;
                    return i;
                }
            }

            centroids.Add(coefficientVector);
            classCounts.Add(1);
            return classCounts.Count - 1;

        }

        #region Documentation
        /// <summary>
        /// This method uses data from WAVES module (Qrs_onset and Qrs_end) and extracts single QRS complex, creating tuple which contains int value - number of R peaks corresponding to the QRS complex, and vector - containing following signal samples. 
        /// </summary>
        /// <param name="qrsOnset"></param>
        /// <param name="qrsEnd"></param>
        /// <param name="qrsR"></param>
        #endregion
        private Tuple<int, int> RepairQrsComplex(int qrsOnset, int qrsEnd, int qrsR, uint fs)
        {
            var maxQRTime = 0.063;
            var maxRSTime = 0.094;
            var samplingInterval = 1.0 / fs;
            var numberOfSamplesQR = maxQRTime / samplingInterval;
            var numberOfSamplesRS = maxRSTime / samplingInterval;
            var QRSamples = (int)numberOfSamplesQR;
            var RSSamples = (int)numberOfSamplesRS;

            // naprawa onset i endset
            if (qrsR - qrsOnset > QRSamples || (qrsOnset == -1))
                qrsOnset = qrsR - QRSamples;

            if (qrsEnd - qrsR > RSSamples || (qrsEnd == -1))
                qrsEnd = qrsR + RSSamples;

            return new Tuple<int, int>(qrsOnset, qrsEnd);
        }


        //współczynniki kształtu, wektory cech.
        private Vector<double> CountCoefficients(Vector<double> qrsComplex, uint fs)
        {
            Vector<double> singleCoeffVect = Vector<double>.Build.Dense(4);

            singleCoeffVect[0] = Coefficients.MalinowskaFactor(qrsComplex, fs);
            singleCoeffVect[1] = Coefficients.PnRatio(qrsComplex);
            singleCoeffVect[2] = Coefficients.SpeedAmpRatio(qrsComplex);
            singleCoeffVect[3] = Coefficients.FastSampleCount(qrsComplex);

            return singleCoeffVect;
        }

        public static class Coefficients
        {
            #region Documentation

            /// <summary>
            /// This method calculates Malinowska's factor as one of shape coefficients using a single QRS complex (qrsSignal) and sampling frequency (fs).
            /// </summary>
            /// <param name="qrsSignal"></param>
            /// <param name="fs"></param>
            /// <returns></returns>

            #endregion
            public static double MalinowskaFactor(Vector<double> qrsSignal, uint fs)
            {
                double surface = Integrate(qrsSignal);
                double perimeter = Perimeter(qrsSignal, fs);

                return !perimeter.Equals(0.0) ? surface / perimeter : 0;
            }

            public static double PnRatio(Vector<double> qrsSignal)
            {
                double positiveAmplitude = 0;
                double negativeAmplitude = 0;

                foreach (double value in qrsSignal)
                {
                    if (value < 0)
                        negativeAmplitude += Math.Abs(value);
                    else
                        positiveAmplitude += value;
                }

                return negativeAmplitude.Equals(0.0) ? positiveAmplitude : positiveAmplitude / negativeAmplitude;
            }

            public static double SpeedAmpRatio(Vector<double> qrsSignal)
            {
                double maxSpeed = 0.0;

                for (int i = 0; i < (qrsSignal.Count - 2); i++)
                {
                    var currentSpeed = Math.Abs(qrsSignal[i + 2] + qrsSignal[i] - 2 * qrsSignal[i + 1]);
                    if (currentSpeed > maxSpeed)
                        maxSpeed = currentSpeed;
                }

                var maxAmp = Math.Abs(qrsSignal.Maximum() - qrsSignal.Minimum());
                return maxSpeed / maxAmp;
            }

            public static double FastSampleCount(Vector<double> qrsSignal)
            {
                double maxSpeed = 0.0;

                int qrsLength = qrsSignal.Count;
                double[] speed = new double[qrsLength - 1];
                double constant = 0.4;

                for (int i = 0; i < (qrsLength - 1); i++)
                {
                    var currentSpeed = Math.Abs(qrsSignal[i + 1] - qrsSignal[i]);
                    speed[i] = currentSpeed;

                    if (currentSpeed > maxSpeed)
                        maxSpeed = currentSpeed;
                }
                var threshold = constant * maxSpeed;
                double count = speed.Count(value => value >= threshold);

                return count / speed.Length;
            }

            public static double QrsDuration(Vector<double> qrsSignal, uint fs)
            {
                double samplingInterval = (double)1 / fs;
                return qrsSignal.Count * samplingInterval;
            }

            #region Documentation

            /// <summary>
            /// This method is a sub method used in CountMalinowskaFactor(). It integrates the signal.
            /// </summary>
            /// <param name="qrsSignal"></param>
            /// <returns></returns>

            #endregion
            public static double Integrate(Vector<double> qrsSignal)
            {
                return qrsSignal.Sum(value => Math.Abs(value));
            }

            #region Documentation

            /// <summary>
            /// This method is a sub method used in CountMalinowskaFactor(). It calculates the perimeter of the signal.
            /// </summary>
            /// <param name="qrsSignal"></param>
            /// <param name="fs"></param>
            /// <returns></returns>

            #endregion
            public static double Perimeter(Vector<double> qrsSignal, uint fs)
            {
                double timeBtw2Points = (double)1 / fs;
                double result = 0;

                for (int i = 0; i < (qrsSignal.Count - 1); i++)
                {
                    var a = qrsSignal[i];
                    var b = qrsSignal[i + 1];
                    result = result + Math.Sqrt(timeBtw2Points * timeBtw2Points + (b - a) * (b - a));
                }
                return result;
            }
        }
    }
}