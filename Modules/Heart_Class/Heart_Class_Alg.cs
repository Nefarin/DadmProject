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
using EKG_Project.Modules.Heart_Class;
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

            
            QrsVectors newQrsVectors = new QrsVectors();

            newQrsVectors.SetSignal(sig);
            newQrsVectors.SetQrsOnset(qrsOnset);
            newQrsVectors.SetQrsEnd(qrsEnd);
            newQrsVectors.SetQrsComplex();

            Matrix<double> qrsMatrix = newQrsVectors.GetQrsComplex();


   
            TempInput.setOutputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\out_sig.txt");
            TempInput.writeFile(fs, sig);
            TempInput.setOutputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\out_on.txt");
            TempInput.writeFile(fs, qrsOnset);
            TempInput.setOutputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\matrix.txt");
            //TempInput.writeFileM(fs, qrsMatrix);

        }


        public class QrsVectors
        {
            private Vector<double> signal;
            private static Vector<double> qrsOnset; // docelowo int
            private Vector<double> qrsEnd;   // docelowo int
            private int qrsNumber;
            private Matrix<double> QrsComplex; 
            private Vector<double> singleQrs;

            public QrsVectors() { }

            public void SetSignal(Vector<double> data)
            {
                signal = data;
            }

            public void SetQrsOnset(Vector<double> data)
            {
                //powinien byc typ int! ale to pozniej, bo klasa TempInut nie wczytuje int
                qrsOnset = data;
                qrsNumber = qrsOnset.Count();
            }

            public void SetQrsEnd(Vector<double> data)
            {
                qrsEnd = data;
            }

            public void SetQrsComplex()
            {
                
                for (int i = 0; i < qrsNumber; i++)
                {
                    
                    double singleQrsOnset = qrsOnset.At(i);
                    double signleQrsEnd = qrsEnd.At(i);
                    int qrsLength = (int)(signleQrsEnd - singleQrsOnset+1);


                    signal.CopySubVectorTo(singleQrs, (int)singleQrsOnset, (int)signleQrsEnd, qrsLength);

                    QrsComplex.SetColumn(i, singleQrs);
                }
            } 

            //Uwaga - kolejne zespoły QRS sa w kolumnach
            public Matrix<double> GetQrsComplex()
            {
                return QrsComplex;
            }
        }



        public class QrsComplex
        {
            static Vector<double> qrssignal;
            static double malinowskaCoefficient;
            static double pnRatio;
            static double speedAmpltudeRatio;
            static double fastSample;
            static double qRSTime;
            static int qrsLength = qrssignal.Count();

            static void CountMalinowskaFactor(Vector<double> qrssignal, uint fs)
            {
                double surface = Integrate(qrssignal);
                double perimeter = Perimeter(qrssignal, fs);

                if (perimeter != 0)
                {
                    malinowskaCoefficient = surface / perimeter;
                }
                else malinowskaCoefficient = 0;
            }

            static double Integrate(Vector<double> qrssignal)
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

            static double Perimeter(Vector<double> qrssignal, uint fs)
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

            static double PnRatio(Vector<double> qrssignal)
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

            static double SpeedAmpRatio(Vector<double> qrssignal)
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

            static double FastSampleCount(Vector<double> qrssignal)
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

            static double QrsDuration(Vector<double> qrssignal, uint fs)
            {
                double samplingInterval = 1 / fs;
                return qrsLength * samplingInterval;

            }

        }

    }
}
