using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.IO;

namespace EKG_Project.Modules.Waves
{
    public partial class Waves : IModule
    {
        static Vector<double> _ecg;
        static List<int> _Rpeaks;

        static List<int> _QRSonsets;
        static List<int> _QRSends;
        static void Main()
        {
            /*Vector<double> signal = Vector<double>.Build.Random(10);
            Vector<double> dwt = Vector<double>.Build.Random(10);
            dwt = HaarDWT(signal, 1);*/
            
            TempInput.setInputFilePath(@"C:\Users\Michał\Documents\biomed\II stopien\dadm\lab2\EKG.txt");
            TempInput.setOutputFilePath(@"C:\Users\Michał\Documents\biomed\II stopien\dadm\lab2\EKGQRSonsets3.txt");
            TempInput.getFrequency();
            _ecg = TempInput.getSignal();
            Vector<double> dwt = HaarDWT(_ecg, 3);
            Vector<double> temp = Vector<double>.Build.Dense(2);
            _Rpeaks = new List<int>();
            _QRSends = new List<int>();
            _QRSonsets = new List<int>();
            TempInput.setInputFilePath(@"C:\Users\Michał\Documents\biomed\II stopien\dadm\lab2\EKG3Rpeaks.txt");
            Vector<double> rpeaks = TempInput.getSignal();
            foreach( double singlePeak in rpeaks)
            {
                _Rpeaks.Add((int)singlePeak);
            }
            DetectQRS();
            Vector<double> onsets = Vector<double>.Build.Dense(_QRSonsets.Count);
            for( int i=0; i< _QRSonsets.Count; i++)
            {
                onsets[i]=(double)_QRSonsets[i];
                
            }
            foreach(double chuj in onsets)
            {
               // Console.WriteLine(chuj);
            }
            TempInput.writeFile(360, onsets);
            TempInput.setOutputFilePath(@"C:\Users\Michał\Documents\biomed\II stopien\dadm\lab2\d2ekg.txt");
            TempInput.writeFile(360, dwt);
            Console.Read();
        }

        static public Vector<double> HaarDWT(Vector<double> signal, int n)
        {
            //Work just like wavedec but use only haar wavelet
            // http://www.mathworks.com/help/wavelet/ref/wavedec.html
            int decompSize = signal.Count();
            double sqrt2 = Math.Sqrt(2);
            Vector<double> outVec = Vector<double>.Build.Dense(decompSize);
            Vector<double> signalTemp = signal;

            for (int i = 0; i < n; i++)
            {
                decompSize /= 2;
                for (int dataInd = 0; dataInd < decompSize; dataInd++)
                {
                    outVec[dataInd] = (signalTemp[2 * dataInd] + signalTemp[2 * dataInd + 1]) / sqrt2;
                    outVec[decompSize + dataInd] = (signalTemp[2 * dataInd] - signalTemp[2 * dataInd + 1]) / sqrt2;
                }
                signalTemp = outVec.SubVector(0, decompSize);
            }
            return outVec;
        }

        static public List<Vector<double>> ListHaarDWT(Vector<double> signal, int n)
        {
            //Work just like wavedec but use only haar wavelet
            // http://www.mathworks.com/help/wavelet/ref/wavedec.html
            int decompSize = signal.Count();
            double sqrt2 = Math.Sqrt(2);
            Vector<double> outVec = Vector<double>.Build.Dense(decompSize);
            Vector<double> signalTemp = signal;
            List<Vector<double>> listOut = new List<Vector<double>>();
            for (int i = 0; i < n; i++)
            {
                decompSize /= 2;
                for (int dataInd = 0; dataInd < decompSize; dataInd++)
                {
                    outVec[dataInd] = (signalTemp[2 * dataInd] + signalTemp[2 * dataInd + 1]) / sqrt2;
                    outVec[decompSize + dataInd] = (signalTemp[2 * dataInd] - signalTemp[2 * dataInd + 1]) / sqrt2;
                }
                signalTemp = outVec.SubVector(0, decompSize);
                listOut.Add(outVec.SubVector(decompSize, decompSize));
            }
            return listOut;
            
        }

        static public void DetectQRS()
        {
            _QRSonsets.Clear();
            List<Vector<double>> dwt =new List< Vector<double>>();
            dwt = ListHaarDWT(_ecg, 3);
            int d2size = dwt[1].Count();
            Console.WriteLine(d2size);
            int rSize = _Rpeaks.Count();

            _QRSonsets.Add(FindQRSOnset(0, _Rpeaks[0], dwt[1]));
            Console.WriteLine(rSize);
            for (int middleR = 1; middleR < rSize - 1; middleR++ )
            {
                _QRSonsets.Add(FindQRSOnset(_Rpeaks[middleR-1], _Rpeaks[middleR], dwt[1]));
               // _QRSends.Add(FindQRSEnd(_Rpeaks[middleR], _Rpeaks[middleR + 1], dwt[1]));
            }
            //_QRSends.Add(FindQRSEnd(_Rpeaks[rSize - 1], rSize - 1, dwt[1]));

        }

        static public int FindQRSOnset( int rightEnd, int middleR, Vector<double> dwtD2)
        {
            int divider = 4;
            int qrsOnsetInd = dwtD2.SubVector( rightEnd/ divider, (middleR-rightEnd)/ divider + divider).AbsoluteMinimumIndex() + rightEnd/ divider;
            double treshold = 100;
            while (Math.Abs(dwtD2[qrsOnsetInd]) > treshold && qrsOnsetInd > 1)
                qrsOnsetInd--;
            return divider * qrsOnsetInd;
        }

        static public int FindQRSEnd(int leftEnd, int middleR, Vector<double> dwtD2)
        {
            int divider = 4;
            Console.WriteLine((leftEnd - middleR) / divider + divider);
            
            int qrsEndInd = dwtD2.SubVector( middleR / divider, (middleR-leftEnd)/ divider + divider).AbsoluteMaximumIndex() + middleR / divider;
            double treshold = 5;
            while (Math.Abs(dwtD2[qrsEndInd]) > treshold)
                qrsEndInd++;
            return divider*qrsEndInd;
        }
    }
}
