using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.Heart_Class
{
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
        private static double Integrate(Vector<double> qrsSignal)
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
        private static double Perimeter(Vector<double> qrsSignal, uint fs)
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