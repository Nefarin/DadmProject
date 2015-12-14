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
        public static void Main()
        {  
            TempInput.setInputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\signal.txt");
            uint fs = TempInput.getFrequency();
            Vector<double> sig = TempInput.getSignal();

            TempInput.setInputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\qrsOnset.txt");
            Vector<double> qrsOnset = TempInput.getSignal();
            TempInput.setInputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\qrsEnd.txt");
            Vector<double> qrsEnd = TempInput.getSignal();

            TempInput.setInputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\qrsEnd.txt"); // uwaga tu mam pozniej wrzucic plik qrsR.txt
            Vector<double> qrsR = TempInput.getSignal();


            QrsVectors newQrsVectors = new QrsVectors();

            newQrsVectors.Signal = sig;
            newQrsVectors.QrsOnset = qrsOnset;
            newQrsVectors.QrsEnd = qrsEnd;
            newQrsVectors.QrsR = qrsR;
            newQrsVectors.SetQrsComplex();
            

           


   
            TempInput.setOutputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\out_sig.txt");
            TempInput.writeFile(fs, sig);
            TempInput.setOutputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\out_on.txt");
            TempInput.writeFile(fs, qrsOnset);
            //TempInput.setOutputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\matrix.txt");
            //TempInput.writeFileM(fs, qrsMatrix);

        }


        public class QrsVectors
        {
            private Vector<double> _signal;
            private Vector<double> _qrsOnset; // docelowo int
            private Vector<double> _qrsEnd;   // docelowo int
            private int _qrsNumber;
            private Vector<double> _qrsR;   // docelowo int

            private Vector<double> _singleQrs = Vector<double>.Build.Dense(1);
            private List<Tuple<int, Vector<double>>> _QrsComplex = new List<Tuple<int, Vector<double>>>();

            public Vector<double> Signal
            {
                get { return _signal; }
                set { _signal = value; }
            }

            public Vector<double> QrsOnset
            {
                get { return _qrsOnset; }
                set
                {
                    //powinien byc typ int! ale to pozniej, bo klasa TempInut nie wczytuje int
                    _qrsOnset = value;
                    _qrsNumber = _qrsOnset.Count();
                }
            }

            public Vector<double> QrsEnd
            {
                get { return _qrsEnd; }
                set { _qrsEnd = value; }
            }

            public Vector<double> QrsR
            {
                get { return _qrsR; }
                set { _qrsR = value; }
            }

            public void SetQrsComplex()
            {
                
                for (int i = 0; i < _qrsNumber; i++)
                {
                    
                    double singleQrsOnset = _qrsOnset.At(i);
                    double signleQrsEnd = _qrsEnd.At(i);
                    int qrsLength = (int)(signleQrsEnd - singleQrsOnset+1);
                    _singleQrs = Vector<double>.Build.Dense(qrsLength);
                    int singleQrsR = (int) _qrsR.At(i);

                    _signal.CopySubVectorTo(_singleQrs, sourceIndex: (int)singleQrsOnset, targetIndex: 0, count: qrsLength);
                    Tuple<int, Vector<double>> a = new Tuple<int, Vector<double>>(singleQrsR, _singleQrs);
                    _QrsComplex.Add(a);

                }
            } 

            // TODO get QrsComplex
        }



        public class QrsComplex
        {
            static Vector<double> qrssignal;
            double malinowskaCoefficient;
            double pnRatio;
            double speedAmpltudeRatio;
            double fastSample;
            double qRSTime;
            int qrsLength = qrssignal.Count();

            void CountMalinowskaFactor(Vector<double> qrssignal, uint fs)
            {
                double surface = Integrate(qrssignal);
                double perimeter = Perimeter(qrssignal, fs);

                if (perimeter != 0)
                {
                    malinowskaCoefficient = surface / perimeter;
                }
                else malinowskaCoefficient = 0;
            }

            double Integrate(Vector<double> qrssignal)
            {

                double result = 0;
                foreach (double value in qrssignal)
                {
                    if (value < 0)
                        result = result - value;
                    else
                        result = result + value;

                }
                return result;
            }

            double Perimeter(Vector<double> qrssignal, uint fs)
            {
                double timeBtw2points = 1 / fs;
                double result = 0;
                double a, b;

                for (int i = 0; i < (qrsLength - 1); i++)
                {
                    a = qrssignal.At(i);
                    b = qrssignal.At(i + 1);
                    result = result + Math.Sqrt((timeBtw2points * timeBtw2points) + ((b - a) * (b - a)));
                }
                return result;
            }

            double PnRatio(Vector<double> qrssignal)
            {
                double result = 0;
                double positiveAmplitude = 0;
                double negativeAmplitude = 0;

                foreach (double value in qrssignal)
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

            double SpeedAmpRatio(Vector<double> qrssignal)
            {
                double[] speed;
                speed = new double[qrsLength - 2];
                double maxSpeed;
                double maxAmp;

                for (int i = 0; i < (qrsLength - 2); i++)
                {
                    speed[i] = Math.Abs(qrssignal.At(i + 2) + qrssignal.At(i) - 2 * qrssignal.At(i + 1));
                }
                maxSpeed = speed.Max();
                maxAmp = Math.Abs(qrssignal.Maximum() - qrssignal.Minimum());
                return maxSpeed / maxAmp;
            }

            double FastSampleCount(Vector<double> qrssignal)
            {
                double[] speed;
                speed = new double[qrsLength - 1];
                double threshold;
                int counter = 0;
                int speedLength;

                for (int i = 0; i < (qrsLength - 1); i++)
                {
                    speed[i] = qrssignal.At(i + 1) - qrssignal.At(i);
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

            double QrsDuration(Vector<double> qrssignal, uint fs)
            {
                double samplingInterval = 1 / fs;
                return qrsLength * samplingInterval;

            }

        }

    }
}
