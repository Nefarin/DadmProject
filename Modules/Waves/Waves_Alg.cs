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
        static List<int> _Ponsets;
        static List<int> _Pends;
        static List<int> _Tends;

        static uint fs;

        static void Main()
        {
            /*Vector<double> signal = Vector<double>.Build.Random(10);
            Vector<double> dwt = Vector<double>.Build.Random(10);
            dwt = HaarDWT(signal, 1);*/

            /*TempInput.setInputFilePath(@"C:\Users\Michał\Documents\biomed\II stopien\dadm\lab2\EKG.txt");
            TempInput.setOutputFilePath(@"C:\Users\Michał\Documents\biomed\II stopien\dadm\lab2\EKGQRSonsets3.txt");*/
            TempInput.setInputFilePath(@"C:\Users\Phantom\Desktop\DADM Project\Nowy folder\EKG.txt");
            TempInput.setOutputFilePath(@"C:\Users\Phantom\Desktop\DADM Project\Nowy folder\EKGQRSonsets.txt");
            fs = TempInput.getFrequency();
            _ecg = TempInput.getSignal();
            Vector<double> dwt = ListHaarDWT(_ecg, 3)[1];
            Vector<double> temp = Vector<double>.Build.Dense(2);
            _Rpeaks = new List<int>();
            _QRSends = new List<int>();
            _QRSonsets = new List<int>();
            _Ponsets = new List<int>();
            _Pends = new List<int>();
            _Tends = new List<int>();
            /*TempInput.setInputFilePath(@"C:\Users\Michał\Documents\biomed\II stopien\dadm\lab2\EKG3Rpeaks.txt");*/
            TempInput.setInputFilePath(@"C:\Users\Phantom\Desktop\DADM Project\Nowy folder\EKG3Rpeaks.txt");
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
            Vector<double> ends = Vector<double>.Build.Dense(_QRSends.Count);
            for (int i = 0; i < _QRSends.Count; i++)
            {
                ends[i] = (double)_QRSends[i];

            }
            FindP();
            Vector<double> ponset = Vector<double>.Build.Dense(_Ponsets.Count);
            for (int i = 0; i < _Ponsets.Count; i++)
            {
                ponset[i] = (double)_Ponsets[i];

            }
            Vector<double> pends = Vector<double>.Build.Dense(_Pends.Count);
            for (int i = 0; i < _Pends.Count; i++)
            {
                pends[i] = (double)_Pends[i];

            }
            FindT();
            Vector<double> tends = Vector<double>.Build.Dense(_Tends.Count);
            for (int i = 0; i < _Tends.Count; i++)
            {
                tends[i] = (double)_Tends[i];

            }

            TempInput.writeFile(360, onsets);
            TempInput.setOutputFilePath(@"C:\Users\Phantom\Desktop\DADM Project\Nowy folder\EKGQRSends.txt");
            TempInput.writeFile(360, ends);
            TempInput.setOutputFilePath(@"C:\Users\Phantom\Desktop\DADM Project\Nowy folder\EKGPonsets.txt");
            TempInput.writeFile(360, ponset);
            TempInput.setOutputFilePath(@"C:\Users\Phantom\Desktop\DADM Project\Nowy folder\EKGPends.txt");
            TempInput.writeFile(360, pends);
            TempInput.setOutputFilePath(@"C:\Users\Phantom\Desktop\DADM Project\Nowy folder\EKGTends.txt");
            TempInput.writeFile(360, tends);
            /*TempInput.setOutputFilePath(@"C:\Users\Michał\Documents\biomed\II stopien\dadm\lab2\d2ekg.txt");*/
            TempInput.setOutputFilePath(@"C:\Users\Phantom\Desktop\DADM Project\Nowy folder\d2ekg.txt");
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
            int qrsEndInd = (middleR >> decompLevel);

            double treshold = Math.Abs(dwt.SubVector(qrsEndInd, (leftEnd >> decompLevel) - qrsEndInd + 1).Minimum()) * 0.03; //TRZEBA POTESTOWAC TĄ METODE PROGOWANIA!!!!
            while (dwt[qrsEndInd] > dwt[qrsEndInd + 1] && qrsEndInd < sectionEnd)
                qrsEndInd++;
            while (Math.Abs(dwt[qrsEndInd]) > treshold && qrsEndInd < sectionEnd)
                qrsEndInd++;

            if (qrsEndInd == sectionEnd)
                return -1;
            else
                return (qrsEndInd << decompLevel);
        }

        static public void FindMaxValue(Vector<double> signal, double begin_loc, double end_loc, out int pmax_loc, out double pmax_val)
        {

            if (signal.Count == 0)
            {
                throw new InvalidOperationException("Empty vector");
            }
            int begin = Convert.ToInt32(begin_loc);
            int end = Convert.ToInt32(end_loc);
            int loc;
            
            pmax_val = double.MinValue;
            pmax_loc = 0;

            for (loc = begin; loc <= end ; loc++)
            {
                if (pmax_val < signal[loc])
                {
                    pmax_val = signal[loc];
                    pmax_loc = loc;
                }
            }
        }

        static public void FindP()
        {
            double window,pmax_val;
            int pmax_loc,ponset,pend;

            window = (Math.Round(0.25 * fs));

            foreach(int onset_loc in _QRSonsets)
            {
                if ((onset_loc - (window)) >= 1 && onset_loc != -1)
                {
                    FindMaxValue(_ecg, onset_loc - window, onset_loc, out pmax_loc, out pmax_val);
                }
                else
                {
                    continue;
                }

                ponset = pmax_loc;
                while(_ecg[ponset] > _ecg[ponset-1] || (pmax_val-_ecg[ponset] < 70))
                {
                    ponset--;
                }
                _Ponsets.Add(ponset);

                pend = pmax_loc;
                while (_ecg[pend] > _ecg[pend+1] || (pmax_val - _ecg[pend] < 110))
                {
                    pend++;
                }
                _Pends.Add(pend);
            }
        }

        static public void FindT()
        {
            double window, tmax_val;
            int tmax_loc, tend;


            window = (Math.Round(0.22 * fs));

            foreach (double ends_loc in _QRSends)
            {
                if (((ends_loc + (window)) < _ecg.Count) && ends_loc != -1)
                {
                    FindMaxValue(_ecg, ends_loc, ends_loc + window, out tmax_loc, out tmax_val);
                }
                else
                {
                    continue;
                }

                tend = tmax_loc;
                while (_ecg[tend] > _ecg[tend + 1] || (tmax_val - _ecg[tend] < 20))
                {
                    tend++;
                    if(tend == _ecg.Count-1)
                    {
                        break;
                    }
                }
                _Tends.Add(tend);
            }
        }
    }


}
