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
            Vector<double> dwt = ListHaarDWT(_ecg, 3)[1];
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
            Vector<double> onsets = Vector<double>.Build.Dense(_QRSends.Count);
            for( int i=0; i< _QRSends.Count; i++)
            {
                onsets[i]=(double)_QRSends[i];
                
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
            // [0]  -> d1, [1]  -> d2, ...
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
            int decompLevel = 2;
            dwt = ListHaarDWT(_ecg, decompLevel);
            
            int d2size = dwt[ decompLevel - 1].Count();
            int rSize = _Rpeaks.Count();

            _QRSonsets.Add(FindQRSOnset(0, _Rpeaks[0], dwt[1], decompLevel));
            for (int middleR = 1; middleR < rSize - 1; middleR++ )
            {
                _QRSonsets.Add(FindQRSOnset(_Rpeaks[middleR-1], _Rpeaks[middleR], dwt[1], decompLevel));
                _QRSends.Add(FindQRSEnd(_Rpeaks[middleR], _Rpeaks[middleR + 1], dwt[1], decompLevel));
            }
            _QRSends.Add(FindQRSEnd(_Rpeaks[rSize - 1], _ecg.Count - 1, dwt[1], decompLevel));

        }

        static public int FindQRSOnset( int rightEnd, int middleR, Vector<double> dwt, int decompLevel)
        {
            int sectionStart = (rightEnd >> decompLevel);
            int qrsOnsetInd = dwt.SubVector(sectionStart, (middleR >> decompLevel) - (rightEnd>> decompLevel)+1).MinimumIndex() + sectionStart;
            double treshold = Math.Abs(dwt[qrsOnsetInd])*0.05; //TRZEBA POTESTOWAC TĄ METODE PROGOWANIA!!!!

            while (Math.Abs(dwt[qrsOnsetInd]) > treshold && qrsOnsetInd > sectionStart)
                qrsOnsetInd--;

            if (qrsOnsetInd == sectionStart)
                return -1;
            else
                return (qrsOnsetInd << decompLevel);
        }

        static public int FindQRSEnd( int middleR, int leftEnd, Vector<double> dwt, int decompLevel)
        {
            int sectionEnd = (leftEnd >> decompLevel) + 1;
            int qrsEndInd = dwt.SubVector( (middleR >> decompLevel), (leftEnd >> decompLevel)-( middleR >> decompLevel )+1 ).MaximumIndex() + (middleR >> decompLevel);
            double treshold = Math.Abs(dwt[qrsEndInd]) * 0.05; //TRZEBA POTESTOWAC TĄ METODE PROGOWANIA!!!!
            while (Math.Abs(dwt[qrsEndInd]) > treshold && qrsEndInd < sectionEnd)
                qrsEndInd++;

            if (qrsEndInd == sectionEnd)
                return -1;
            else
                return (qrsEndInd << decompLevel);
        }

    }


}
