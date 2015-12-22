using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Ink;
using MathNet.Numerics.LinearAlgebra;
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
        int qrsLength; 
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
            //_qrsCoefficients = new List<Tuple<int, Vector<double>>> ();
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
            HeartClass.HeartClassData.QrsCoefficients = HeartClass.CountCoeff(HeartClass.GetQrsComplex(), fs);

            TempInput.setOutputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\out_sig.txt");
            TempInput.writeFile(fs, HeartClass.HeartClassData.Signal);
            TempInput.setOutputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\out_on.txt");
            TempInput.writeFile(fs, HeartClass.HeartClassData.QrsOnset);








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

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        public List<Tuple<int, Vector<double>>> GetQrsComplex()
        {
            return HeartClassData.QrsComplex;
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        double CountMalinowskaFactor(Vector<double> _qrssignal, uint fs)
        {
            double surface = Integrate(_qrssignal);
            double perimeter = Perimeter(_qrssignal, fs);

            if (perimeter != 0)
            {
                malinowskaCoefficient = surface / perimeter;
            }
            else malinowskaCoefficient = 0;
            return malinowskaCoefficient;
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
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

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        double Perimeter(Vector<double> _qrssignal, uint fs)
        {
            qrsLength = _qrssignal.Count();
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

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
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

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        double SpeedAmpRatio(Vector<double> _qrssignal)
        {
            qrsLength = _qrssignal.Count();
            double[] speed = new double[qrsLength-2];
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
        
        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        double FastSampleCount(Vector<double> _qrssignal)
        {
            // qrsLength = _qrssignal.Count();
            // double[] speed = new double[qrsLength - 1];
            int qrsLength = _qrssignal.Count();
            double[] speed = new double[qrsLength - 1];
            double threshold;
            int counter = 0;
            int speedLength;
            double constant = 0.4;

            for (int i = 0; i < (qrsLength - 1); i++)
            {
                speed[i] = Math.Abs(_qrssignal.At(i + 1) - _qrssignal.At(i));
            }
            threshold = constant * speed.Max();

            foreach (double value in speed)
            {
                if (value >= threshold)
                    counter = counter + 1;
            }

            speedLength = speed.Length;
            return (double)counter / (double)speedLength;
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        double QrsDuration(Vector<double> _qrssignal, uint fs)
        {
            qrsLength = _qrssignal.Count();
            double samplingInterval = 1 / (double)fs;
            return (double)qrsLength * samplingInterval;
        }


        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        List<Tuple<int, Vector<double>>> CountCoeff(List<Tuple<int, Vector<double>>> _QrsComplex, uint fs)
        {
            Vector<double> _singleCoeffVect; //bedzie wektorem cech dla 1 zespołu
            _singleCoeffVect = Vector<double>.Build.Dense(5);
            int singleQrsR;
            Tuple<int, Vector<double>> coeffTuple;
            List<Tuple<int, Vector<double>>> result;
            result = new List<Tuple<int, Vector<double>>>();
            //coeffTuple = new Tuple<int, Vector<double>>(0, _singleCoeffVect);

            foreach (Tuple<int, Vector<double>> data in _QrsComplex)
            {
               // _singleCoeffVect[0] = 0;
              //  _singleCoeffVect[1] = 0;
               // _singleCoeffVect[2] = 0;
               // _singleCoeffVect[3] = 0;
                //_singleCoeffVect[4] = 0;
                singleQrsR = data.Item1;
                _singleCoeffVect[0] = CountMalinowskaFactor(data.Item2, fs);
                _singleCoeffVect[1] = PnRatio(data.Item2);
                _singleCoeffVect[2] = SpeedAmpRatio(data.Item2);
                _singleCoeffVect[3] = FastSampleCount(data.Item2);
                _singleCoeffVect[4] = QrsDuration(data.Item2, fs);


                coeffTuple = new Tuple<int, Vector<double>>(singleQrsR, _singleCoeffVect);
                result.Add(coeffTuple);
            }
            return result;
        }

    }

}