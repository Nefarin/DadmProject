using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Automation.Peers;
using MathNet.Numerics.LinearAlgebra;
//using EKG_Project.Modules.Heart_Class;
using EKG_Project.IO;

namespace EKG_Project.Modules.Heart_Class
{
    public partial class Heart_Class : IModule
    {
        private Vector<double> _qrssignal;
        double malinowskaCoefficient;
        double pnRatio;
        double speedAmpltudeRatio;
        double fastSample;
        double qRSTime;
        uint fs;
        int qrsLength; // jak zrobie konstruktor klasy to musze to dodac: qrsLength = qrssignal.Count();
        private Heart_Class_Data HeartClassData;

        public Heart_Class()
        {
            _qrssignal = Vector<double>.Build.Dense(1);
            malinowskaCoefficient = new double();
            pnRatio = new double();
            speedAmpltudeRatio = new double();
            fastSample = new double();
            qRSTime = new double();
            fs = new uint();
            qrsLength = _qrssignal.Count();
            HeartClassData = new Heart_Class_Data();
        }


        #region Documentation
        /// <summary>
        /// Test method of Heart_Class module
        /// </summary>
        #endregion
        public static void Main()
        {
            Heart_Class HeartClass = new Heart_Class();
            TempInput.setInputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\signal.txt");
            uint fs = TempInput.getFrequency();
            HeartClass.HeartClassData.Signal = TempInput.getSignal();
            TempInput.setInputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\qrsOnset.txt");
            HeartClass.HeartClassData.QrsOnset = TempInput.getSignal();
            TempInput.setInputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\qrsEnd.txt");
            HeartClass.HeartClassData.QrsEnd = TempInput.getSignal();
            TempInput.setInputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\qrsEnd.txt");

            // uwaga tu mam pozniej wrzucic plik qrsR.txt !!!!
            HeartClass.HeartClassData.QrsR = TempInput.getSignal();
            HeartClass.SetQrsComplex(); // to nie powinna być osobna klasa ale metoda w klasie Heart_Class

            TempInput.setOutputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\out_sig.txt");
            TempInput.writeFile(fs, HeartClass.HeartClassData.Signal);
            TempInput.setOutputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\out_on.txt");
            TempInput.writeFile(fs, HeartClass.HeartClassData.QrsOnset);
            //TempInput.setOutputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\matrix.txt");
            //TempInput.writeFileM(fs, qrsMatrix);







        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        private void SetQrsComplex()
        {
            for (int i = 0; i < HeartClassData.QrsNumber; i++)
            {
                double singleQrsOnset = HeartClassData.QrsOnset.At(i);
                double signleQrsEnd = HeartClassData.QrsEnd.At(i);
                int qrsLength = (int)(signleQrsEnd - singleQrsOnset+1);
                HeartClassData.SingleQrs = Vector<double>.Build.Dense(qrsLength);
                int singleQrsR = (int)HeartClassData.QrsR.At(i);
                HeartClassData.Signal.CopySubVectorTo(HeartClassData.SingleQrs, sourceIndex: (int)singleQrsOnset, targetIndex: 0, count: qrsLength);
                Tuple<int, Vector<double>> a = new Tuple<int, Vector<double>>(singleQrsR, HeartClassData.SingleQrs);
                HeartClassData.QrsComplex.Add(a);
            }
        }
        // TODO get QrsComplex

        void CountMalinowskaFactor(Vector<double> _qrssignal, uint fs)
        {
            double surface = Integrate(_qrssignal);
            double perimeter = Perimeter(_qrssignal, fs);

            if (perimeter != 0)
            {
                malinowskaCoefficient = surface / perimeter;
            }
            else malinowskaCoefficient = 0;
        }

        double Integrate(Vector<double> _qrssignal)
        {

            double result = 0;
            foreach (double value in _qrssignal)
            {
                if (value < 0)
                    result = result - value;
                else
                    result = result + value;
            }
            return result;
        }

        double Perimeter(Vector<double> _qrssignal, uint fs)
        {
            double timeBtw2points = 1 / fs;
            double result = 0;
            double a, b;

            for (int i = 0; i < (qrsLength - 1); i++)
            {
                a = _qrssignal.At(i);
                b = _qrssignal.At(i + 1);
                result = result + Math.Sqrt((timeBtw2points * timeBtw2points) + ((b - a) * (b - a)));
            }
            return result;
        }

        double PnRatio(Vector<double> _qrssignal)
        {
            double result = 0;
            double positiveAmplitude = 0;
            double negativeAmplitude = 0;

            foreach (double value in _qrssignal)
            {
                if (value < 0)
                    negativeAmplitude = negativeAmplitude + (-value);
                else
                    positiveAmplitude = positiveAmplitude + value;

            }
            if (negativeAmplitude == 0)
                result = positiveAmplitude;
            else
                result = positiveAmplitude / negativeAmplitude;
            return result;
        }

        double SpeedAmpRatio(Vector<double> _qrssignal)
        {
            double[] speed;
            speed = new double[qrsLength - 2];
            double maxSpeed;
            double maxAmp;

            for (int i = 0; i < (qrsLength - 2); i++)
            {
                speed[i] = Math.Abs(_qrssignal.At(i + 2) + _qrssignal.At(i) - 2 * _qrssignal.At(i + 1));
            }
            maxSpeed = speed.Max();
            maxAmp = Math.Abs(_qrssignal.Maximum() - _qrssignal.Minimum());
            return maxSpeed / maxAmp;
        }

        double FastSampleCount(Vector<double> _qrssignal)
        {
            double[] speed;
            speed = new double[qrsLength - 1];
            double threshold;
            int counter = 0;
            int speedLength;

            for (int i = 0; i < (qrsLength - 1); i++)
            {
                speed[i] = _qrssignal.At(i + 1) - _qrssignal.At(i);
            }
            threshold = 0.4 * speed.Max();

            foreach (double value in speed)
            {
                if (value >= threshold)
                    counter = counter + 1;
            }

            speedLength = speed.GetLength(0);
            return counter / speedLength;
        }

        double QrsDuration(Vector<double> _qrssignal, uint fs)
        {
            double samplingInterval = 1 / fs;
            return qrsLength * samplingInterval;

        }

    }

}
