using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.Waves
{

    #region Waves Class doc
    /// <summary>
    /// Class locates P-onsets, P-ends, QRS-onsets, QRS-end and T-ends in ECG 
    /// </summary>
    #endregion

    public partial class Waves : IModule
    {

        public void analyzeSignalPart()
        {
            if (InputData.Signals[_currentChannelIndex].Item2.Count == 0)
            {
                throw new InvalidOperationException("Empty vector");
            }

            if (InputDataRpeaks.RPeaks[_currentChannelIndex].Item2.Count == 0)
            {
                throw new InvalidOperationException("Empty vector");
            }


            _currentPendsPart.Clear();
            _currentPonsetsPart.Clear();
            _currentTendsPart.Clear();

            DetectQRS();
            FindP();
            FindT();

            _currentQRSonsets.AddRange(_currentQRSonsetsPart);
            _currentQRSends.AddRange(_currentQRSendsPart);
            _currentPonsets.AddRange(_currentPonsetsPart);
            _currentPends.AddRange(_currentPendsPart);
            _currentTends.AddRange(_currentTendsPart);
        }

        public Vector<double> HaarDWT(Vector<double> signal, int n)
        {
            //Work just like wavedec but use only haar wavelet
            // http://www.mathworks.com/help/wavelet/ref/wavedec.html
            //lol
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

        public List<Vector<double>> ListHaarDWT(Vector<double> signal, int n)
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

        public List<Vector<double>> ListDWT(Vector<double> signal, int n, Wavelet_Type waveType)
        {
            double[] Hfilter = { 0 };
            double[] Lfilter = { 0 };
            int filterSize = 0;
            //generated from wfilters Matlab function
            //byle zrobic commita
            switch (waveType)
            {
                case Wavelet_Type.haar:
                    return ListHaarDWT(signal, n);

                case Wavelet_Type.db2:
                    Hfilter = new double[] { -0.482962913144690, 0.836516303737469, -0.224143868041857, -0.129409522550921 };
                    Lfilter = new double[] { -0.129409522550921, 0.224143868041857, 0.836516303737469, 0.482962913144690 };
                    filterSize = 4;
                    break;

                case Wavelet_Type.db3:
                    Hfilter = new double[] { -0.332670552950957, 0.806891509313339, -0.459877502119331, -0.135011020010391, 0.0854412738822415, 0.0352262918821007 };
                    Lfilter = new double[] { 0.0352262918821007, -0.0854412738822415, -0.135011020010391, 0.459877502119331, 0.806891509313339, 0.332670552950957 };
                    filterSize = 6;
                    break;
            }
            int decompSize = signal.Count();
            Vector<double> outVec = Vector<double>.Build.Dense(decompSize);
            Vector<double> signalTemp = signal;
            List<Vector<double>> listOut = new List<Vector<double>>();
            for (int i = 0; i < n; i++)
            {
                decompSize /= 2;
                for (int dataInd = 0; dataInd < decompSize; dataInd++)
                {
                    outVec[dataInd] = 0;
                    outVec[decompSize + dataInd] = 0;
                    for (int filtIt = 0; filtIt < filterSize && (2 * dataInd + filtIt) < signalTemp.Count; filtIt++)
                    {
                        outVec[dataInd] += signalTemp[2 * dataInd + filtIt] * Lfilter[filterSize - filtIt - 1];
                        outVec[decompSize + dataInd] += signalTemp[2 * dataInd + filtIt] * Hfilter[filterSize - filtIt - 1];
                    }

                }
                signalTemp = outVec.SubVector(0, decompSize);
                listOut.Add(outVec.SubVector(decompSize, decompSize));
            }
            return listOut;

        }

        public void DetectQRS()
        {
            _currentQRSonsetsPart.Clear();
            _currentQRSendsPart.Clear();
            List<Vector<double>> dwt = new List<Vector<double>>();
            int startInd = 0;
            if (_rPeaksProcessed > 1)
            {
                startInd = (int)InputDataRpeaks.RPeaks[_currentChannelIndex].Item2[_rPeaksProcessed - 1];
                //Console.WriteLine("startujemy");
                //Console.WriteLine(startInd);
            }
            int endInd = (int)InputDataRpeaks.RPeaks[_currentChannelIndex].Item2.Count - 1;

            if (_rPeaksProcessed + _params.RpeaksStep + 1 < endInd)
            {
                endInd = (int)InputDataRpeaks.RPeaks[_currentChannelIndex].Item2[_rPeaksProcessed + _params.RpeaksStep + 1];
                //Console.WriteLine(endInd);
            }
            else
            {
                endInd = (int)InputDataRpeaks.RPeaks[_currentChannelIndex].Item2[endInd];
            }

            int dwtLen = 1;
            if (endInd != startInd)
                dwtLen = endInd - startInd;
            //Console.WriteLine(endInd);
            //Console.WriteLine(dwtLen);

            dwt = ListDWT(InputECGData.SignalsFiltered[_currentChannelIndex].Item2.SubVector(startInd, dwtLen), _params.DecompositionLevel, _params.WaveType);

            int d2size = dwt[_params.DecompositionLevel - 1].Count();
            int rSize = _params.RpeaksStep;
            int decLev = _params.DecompositionLevel;

            if (startInd == 0)
                _currentQRSonsetsPart.Add(FindQRSOnset(0, InputDataRpeaks.RPeaks[_currentChannelIndex].Item2[0], dwt[decLev - 1], _params.DecompositionLevel));

            int maxRInd = _rPeaksProcessed + rSize;
            if (maxRInd >= _currentRpeaksLength)
                maxRInd = _currentRpeaksLength - 1;

            Console.Write("Rpeaks Size ");
            Console.WriteLine(InputDataRpeaks.RPeaks[_currentChannelIndex].Item2.Count);

            Console.Write("Aktualny kanal: ");
            Console.WriteLine(_rPeaksProcessed);
            if( _rPeaksProcessed == 0)
                _currentQRSendsPart.Add(FindQRSEnd(InputDataRpeaks.RPeaks[_currentChannelIndex].Item2[_rPeaksProcessed] - startInd, InputDataRpeaks.RPeaks[_currentChannelIndex].Item2[_rPeaksProcessed + 1] - startInd, dwt[decLev - 1], _params.DecompositionLevel) + startInd);

            for (int middleR = _rPeaksProcessed +1 ; middleR < maxRInd; middleR++)
            {
                Console.Write("start ");
                Console.WriteLine(InputDataRpeaks.RPeaks[_currentChannelIndex].Item2[middleR - 1]);
                _currentQRSonsetsPart.Add(FindQRSOnset(InputDataRpeaks.RPeaks[_currentChannelIndex].Item2[middleR - 1] - startInd, InputDataRpeaks.RPeaks[_currentChannelIndex].Item2[middleR] - startInd, dwt[decLev - 1], _params.DecompositionLevel) + startInd);
                _currentQRSendsPart.Add(FindQRSEnd(InputDataRpeaks.RPeaks[_currentChannelIndex].Item2[middleR] - startInd, InputDataRpeaks.RPeaks[_currentChannelIndex].Item2[middleR + 1] - startInd, dwt[decLev - 1], _params.DecompositionLevel) + startInd);
            }

            int Rlast = InputDataRpeaks.RPeaks[_currentChannelIndex].Item2.Count- 1;

            if (_rPeaksProcessed + Params.RpeaksStep > Rlast)
                _currentQRSonsetsPart.Add(FindQRSOnset(InputDataRpeaks.RPeaks[_currentChannelIndex].Item2[Rlast - 1] - startInd, InputDataRpeaks.RPeaks[_currentChannelIndex].Item2[Rlast] - startInd, dwt[decLev - 1], _params.DecompositionLevel) + startInd);

            if (endInd >= (int)InputDataRpeaks.RPeaks[_currentChannelIndex].Item2.Count - 2)
            {
                //Console.WriteLine("uga buga");
                _currentQRSendsPart.Add(FindQRSEnd(endInd - startInd, endInd, dwt[decLev - 1], _params.DecompositionLevel) + startInd);
            }
        }

        public int FindQRSOnset(double drightEnd, double dmiddleR, Vector<double> dwt, int decompLevel)
        {
            int rightEnd = (int)drightEnd;
            int middleR = (int)dmiddleR;
            int sectionStart = (rightEnd >> decompLevel);

            int len = (middleR >> decompLevel) - (rightEnd >> decompLevel);

            if (len < 1)
                len = 1;

            //Console.WriteLine("nadupcamy!");
            //Console.WriteLine(sectionStart);
            //Console.WriteLine((middleR >> decompLevel) - (rightEnd >> decompLevel) + 1);
            //Console.WriteLine(dwt.Count);

            if (sectionStart + len >= dwt.Count || len<1 || sectionStart<0)
                return -1;

            int qrsOnsetInd = dwt.SubVector(sectionStart, len).MinimumIndex() + sectionStart;
            double treshold = Math.Abs(dwt[qrsOnsetInd]) * 0.05;

            while (Math.Abs(dwt[qrsOnsetInd]) > treshold && qrsOnsetInd > sectionStart)
                qrsOnsetInd--;

            if (qrsOnsetInd == sectionStart)
                return -1;
            else
                return (qrsOnsetInd << decompLevel);
        }

        public int FindQRSEnd(double dmiddleR, double dleftEnd, Vector<double> dwt, int decompLevel)
        {
            int middleR = (int)dmiddleR;
            int leftEnd = (int)dleftEnd;
            int sectionEnd = (leftEnd >> decompLevel);
            int qrsEndInd = (middleR >> decompLevel);
            int len = (leftEnd >> decompLevel) - qrsEndInd;

            if (len < 1)
                len = 1;

            //Console.WriteLine("qrsEndzik");
            //Console.WriteLine(len);
            //Console.WriteLine(qrsEndInd);
            //Console.WriteLine(dwt.Count);

            if (qrsEndInd + len >= dwt.Count || len < 1 || qrsEndInd<0)
            {
                return -1;
                //Console.WriteLine("Brak enda");
            }

            Console.WriteLine("QRS chuje muje");
            Console.Write("dwt. Count ");
            Console.WriteLine(dwt.Count);
            Console.Write("starcik ");
            Console.WriteLine(qrsEndInd);
            Console.Write("dlugosc subwektowa ");
            Console.WriteLine(len);

            Console.Write("middleR ");
            Console.WriteLine(middleR);
            double treshold = Math.Abs(dwt.SubVector(qrsEndInd, len).Minimum()) * 0.08;

            //Console.WriteLine("szczegoliki:");
            //Console.WriteLine(qrsEndInd);
            //Console.WriteLine(dwt.Count);
            if (!(qrsEndInd + 1 < dwt.Count))
            {
                return -1;
                //Console.WriteLine("brak enda");
            }
            //while (dwt[qrsEndInd] < dwt[qrsEndInd + 1])
            //    qrsEndInd++;
            Console.WriteLine(dwt.Count);
            Console.WriteLine(qrsEndInd);
            while (dwt[qrsEndInd] > dwt[qrsEndInd + 1] && qrsEndInd < sectionEnd)
                qrsEndInd++;
            while (Math.Abs(dwt[qrsEndInd]) > treshold && qrsEndInd < sectionEnd)
                qrsEndInd++;

            if (qrsEndInd >= sectionEnd)
                return -1;
            else
                return (qrsEndInd << decompLevel);
        }


        #region
        /// <summary>
        /// This method finds maximum value and its location through particular segment of ecg signal
        /// </summary>
        /// <param name="begin_loc"> First sample of specified signal segment</param>
        /// <param name="end_loc"> Last sample of specified signal segment</param>
        /// <param name="max_loc"> Localization of sample at maximum value</param>
        /// <param name="max_val"> Maximum value in specified signal segment</param>
        /// <returns> maximum value and its location in signal segment</returns>
        #endregion
        public void FindMaxValue(int begin_loc, int end_loc, out int max_loc, out double max_val)
        {

            if (InputECGData.SignalsFiltered[_currentChannelIndex].Item2.Count == 0)
            {
                throw new InvalidOperationException("Empty vector");
            }
            int loc_index;

            max_val = double.MinValue;
            max_loc = 0;

            for (loc_index = begin_loc; loc_index <= end_loc; loc_index++)
            {
                if (max_val < InputECGData.SignalsFiltered[_currentChannelIndex].Item2[loc_index])
                {
                    max_val = InputECGData.SignalsFiltered[_currentChannelIndex].Item2[loc_index];
                    max_loc = loc_index;
                }
            }

        }

        #region
        /// <summary>
        /// This method finds locations of P-onsets and P-ends
        /// </summary>
        /// <returns> List containing locations of P-onsets and P-ends</returns>
        #endregion
        public void FindP()
        {
            double pmax_val, thr;
            int window, break_window, pmax_loc, ponset, pend;

            window = Convert.ToInt32(InputData.Frequency * 0.5);
            break_window = Convert.ToInt32(InputData.Frequency * 0.6);

            foreach (int onset_loc in _currentQRSonsetsPart)
            {
                if ((onset_loc - (window)) >= 1 && onset_loc != -1)
                {
                    FindMaxValue(onset_loc - window, onset_loc, out pmax_loc, out pmax_val);
                }
                else
                {
                    continue;
                }

                ponset = pmax_loc;
                thr = (pmax_val - InputECGData.SignalsFiltered[_currentChannelIndex].Item2[onset_loc]) * 0.4;
                while (InputECGData.SignalsFiltered[_currentChannelIndex].Item2[ponset] > InputECGData.SignalsFiltered[_currentChannelIndex].Item2[ponset - 1] || Math.Abs(pmax_val - InputECGData.SignalsFiltered[_currentChannelIndex].Item2[ponset]) < thr) //dawniej 70
                {
                    ponset--;
                    if (ponset < onset_loc - break_window)
                    {
                        ponset = -1;
                        break;
                    }
                }
                _currentPonsetsPart.Add(ponset);

                pend = pmax_loc;
                thr = (pmax_val - InputECGData.SignalsFiltered[_currentChannelIndex].Item2[onset_loc]) * 0.4;
                while (InputECGData.SignalsFiltered[_currentChannelIndex].Item2[pend] > InputECGData.SignalsFiltered[_currentChannelIndex].Item2[pend + 1] || (pmax_val - InputECGData.SignalsFiltered[_currentChannelIndex].Item2[pend] < thr))
                {
                    pend++;
                    if (pend > onset_loc)
                    {
                        pend = -1;
                        break;
                    }
                }
                _currentPendsPart.Add(pend);
            }
        }

        #region
        /// <summary>
        /// This method finds locations of T-ends
        /// </summary>
        /// <returns> List containing locations of T-ends</returns>
        #endregion
        public void FindT()
        {
            double tmax_val, thr;
            int window, break_window, tmax_loc, tend;


            window = Convert.ToInt32(InputData.Frequency * 0.5);
            break_window = Convert.ToInt32(InputData.Frequency * 0.55);

            foreach (int ends_loc in _currentQRSendsPart)
            {
                if (((ends_loc + (window)) < InputECGData.SignalsFiltered[_currentChannelIndex].Item2.Count) && ends_loc != -1)
                {
                    FindMaxValue(ends_loc, ends_loc + window, out tmax_loc, out tmax_val);
                }
                else
                {
                    continue;
                }

                tend = tmax_loc;
                thr = (tmax_val - InputECGData.SignalsFiltered[_currentChannelIndex].Item2[ends_loc]) * 0.25;
                while (InputECGData.SignalsFiltered[_currentChannelIndex].Item2[tend] > InputECGData.SignalsFiltered[_currentChannelIndex].Item2[tend + 1] || ((tmax_val - InputECGData.SignalsFiltered[_currentChannelIndex].Item2[tend] < thr) && (tmax_val - InputECGData.SignalsFiltered[_currentChannelIndex].Item2[tend] > -(tmax_val * 0.01))))
                {
                    tend++;
                    if (tend > ends_loc + break_window)
                    {
                        tend = -1;
                        break;
                    }
                }
                _currentTendsPart.Add(tend);
            }
        }
    }


}
