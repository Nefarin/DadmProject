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

            if( _params.WaveType == Wavelet_Type.haar)
            {
                _qrsEndTresh = 0.2;
                _qrsOnsTresh = 0.2;
            }
            else if (_params.WaveType == Wavelet_Type.db2)
            {
                _qrsEndTresh = 0.12;
                _qrsOnsTresh = 0.12;
            }
            else
            {
                _qrsEndTresh = 0.3;
                _qrsOnsTresh = 0.3;
            }

            DetectQRS();
            FindP();
            FindT();

            _currentQRSonsets.AddRange(_currentQRSonsetsPart);
            _currentQRSends.AddRange(_currentQRSendsPart);
            _currentPonsets.AddRange(_currentPonsetsPart);
            _currentPends.AddRange(_currentPendsPart);
            _currentTends.AddRange(_currentTendsPart);
            // _params.RpeaksStep = _currentStep;
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
        #region
        /// <summary>
        /// This method calc discrete wavelet transform
        /// </summary>
        /// <returns> list of details and approximation coefficients</returns>
        #endregion
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
        #region
        /// <summary>
        /// This method finds location of QRS-ends and QRS-onsets
        /// </summary>
        #endregion
        public void DetectQRS()
        {
            _currentQRSonsetsPart.Clear();
            _currentQRSendsPart.Clear();
            List<Vector<double>> dwt = new List<Vector<double>>();
            
            dwt = ListDWT(_currentECG , _params.DecompositionLevel, Wavelet_Type.haar);

            int d2size = dwt[_params.DecompositionLevel - 1].Count();
            int rSize = _params.RpeaksStep;
            int decLev = _params.DecompositionLevel;

            
            _currentQRSonsetsPart.Add(FindQRSOnset(0, _currentRpeaks[0] , dwt[decLev - 1], _params.DecompositionLevel));
            int maxRInd = _currentRpeaks.Count - 1;

            for (int middleR = 0; middleR < maxRInd; middleR++)
            {
                _currentQRSonsetsPart.Add(FindQRSOnset(_currentRpeaks[middleR ], _currentRpeaks[middleR+1] , dwt[decLev - 1], _params.DecompositionLevel) );
                _currentQRSendsPart.Add(FindQRSEnd(_currentRpeaks[middleR] , _currentRpeaks[middleR + 1] , dwt[decLev - 1], _params.DecompositionLevel));

            }

            _currentQRSendsPart.Add(FindQRSEnd(_currentRpeaks[maxRInd] , _currentECG.Count , dwt[decLev - 1], _params.DecompositionLevel) );
        }
        #region
        /// <summary>
        /// This method finds location of QRS-onset
        /// </summary>
        /// <returns> index of founded QRS-onset or -1 if not found</returns>
        #endregion
        public int FindQRSOnset(double drightEnd, double dmiddleR, Vector<double> dwt, int decompLevel)
        {
            int rightEnd = (int)drightEnd;
            int middleR = (int)dmiddleR;
            int sectionStart = (rightEnd >> decompLevel);

            int len = (middleR>>decompLevel) -(rightEnd >> decompLevel);



            if  ( sectionStart < 0)
                return -1;

            if (sectionStart + len >= dwt.Count)
                len = dwt.Count - sectionStart;

            if (len < 1)
                return -1;

            int qrsOnsetInd = dwt.SubVector(sectionStart, len).MinimumIndex() + sectionStart;
            double treshold = Math.Abs(dwt[qrsOnsetInd]) * _qrsOnsTresh;
            Console.WriteLine("kurwa " + middleR);
            Console.WriteLine("mac " + _currentECG.Count);
            if (dmiddleR >= _currentECG.Count)
                dmiddleR = _currentECG.Count - 1;
            double Rval = _currentECG[(int)dmiddleR];
            while (Math.Abs(dwt[qrsOnsetInd]) > treshold && qrsOnsetInd > sectionStart)
                qrsOnsetInd--;

            if (qrsOnsetInd == sectionStart)
                return -1;
            else
            {
                qrsOnsetInd = (qrsOnsetInd << decompLevel);
                int samples2analyse = (int)(InputData.Frequency * 0.04);
                while (lastNderivSquares(samples2analyse, qrsOnsetInd, _currentECG) > 0.004 * Rval && qrsOnsetInd < rightEnd)
                {
                    qrsOnsetInd--;
                }
                    
                return  qrsOnsetInd + _offset;
            }
                
        }
        #region
        /// <summary>
        /// This method finds location of QRS-ends
        /// </summary>
        /// <returns> index of founded QRS-end or -1 if not found</returns>
        #endregion
        public int FindQRSEnd(double dmiddleR, double dleftEnd, Vector<double> dwt, int decompLevel)
        {
            int middleR = (int)dmiddleR;
            int leftEnd = (int)dleftEnd;
            leftEnd -= (2 << decompLevel);
            int sectionEnd = (leftEnd >> decompLevel);
            int qrsEndInd = (middleR >> decompLevel);
            int length = 250 * (int)InputData.Frequency;

            int len = (leftEnd >> decompLevel) - qrsEndInd;

            if (len < 1)
                len = 1;



            if (qrsEndInd + len > dwt.Count || len < 1 || qrsEndInd < 0)
            {
                return -1;
            }


            double treshold = Math.Abs(dwt.SubVector(qrsEndInd, len).Maximum()) * _qrsEndTresh;


            if (!(qrsEndInd + 1 < dwt.Count))
            {
                return -1;

            }

            //qrsEndInd = qrsEndInd << decompLevel;
            //sectionEnd = leftEnd;
            double Rval = _currentECG[(int)dmiddleR];
            //while (Math.Abs(_currentECG[qrsEndInd]) > 0.12 * Rval && qrsEndInd < sectionEnd)
            //    qrsEndInd++;
            //while (_currentECG[qrsEndInd] > _currentECG[qrsEndInd+1] && qrsEndInd < sectionEnd)
            //    qrsEndInd++;
            //while (Math.Abs(_currentECG[qrsEndInd]) > 0.12 * Rval && qrsEndInd < sectionEnd)
            //    qrsEndInd++;
            //while (calcLastNSquares(3, qrsEndInd, _currentECG) > Rval * 0.05 && qrsEndInd < sectionEnd)
            //    qrsEndInd++;
            while (dwt[qrsEndInd] > dwt[qrsEndInd + 1] && qrsEndInd < sectionEnd)
                qrsEndInd++;
            while (Math.Abs(dwt[qrsEndInd]) > treshold && qrsEndInd < sectionEnd)
                qrsEndInd++;
            if (qrsEndInd >= sectionEnd)
                return -1;
            else
            {
                int samples2analyse = (int)(InputData.Frequency * 0.02);
                qrsEndInd = (qrsEndInd << decompLevel);
                while (Math.Abs(_currentECG[qrsEndInd] - Rval) / Rval < 0.95 && qrsEndInd < leftEnd)
                    qrsEndInd++;
                while (_currentECG[qrsEndInd] > _currentECG[qrsEndInd + 1] && qrsEndInd < leftEnd)
                    qrsEndInd++;
                while (lastNderivSquares(samples2analyse, qrsEndInd, _currentECG) > 0.02 * Rval && qrsEndInd < leftEnd)
                    qrsEndInd++;
                while (nextNderivSquares(samples2analyse, qrsEndInd, _currentECG) > 0.02 * Rval && qrsEndInd < leftEnd)
                    qrsEndInd++;
                return qrsEndInd + _offset;
            }
                
        }
        #region
        /// <summary>
        /// This method calc means of a part of signal
        /// </summary>
        /// <returns> Means of 1 ms part of signal counted from qrsEndInd index </returns>
        #endregion
        double lastNderivSquares(int n, int index, Vector<double> signal)
        {
            double res = 0;
            for(int i=0; i< n; i++)
            {
                res += derivSquare(index - i, signal);
            }
            return res;
        }
        double nextNderivSquares(int n, int index, Vector<double> signal)
        {
            double res = 0;
            for (int i = 0; i < n; i++)
            {
                res += derivSquare(index + i, signal);
            }
            return res;
        }
        double derivSquare( int index, Vector<double> signal)
        {
            double res = 0;
            if (index - 2 > 0 && index + 2 < signal.Count)
                res = 0.125 * (-signal[index -2] - 2 * signal[index-1] +2*signal[index+1] + signal[index+2]);
            return res * res;
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

            if (_currentECG.Count == 0)
            {
                throw new InvalidOperationException("Empty vector");
            }
            int loc_index;

            max_val = double.MinValue;
            max_loc = 0;

            for (loc_index = begin_loc; loc_index <= end_loc; loc_index++)
            {
                if (max_val < _currentECG[loc_index])
                {
                    max_val = _currentECG[loc_index];
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

            window = Convert.ToInt32(InputData.Frequency * 0.25);
            break_window = Convert.ToInt32(InputData.Frequency * 0.3);

            foreach (int onset_loc_abs in _currentQRSonsetsPart)
            {
                int onset_loc = -1;
                if( onset_loc_abs!=-1)
                    onset_loc = onset_loc_abs - _offset;
                if ((onset_loc - (window)) >= 1 && onset_loc != -1)
                {
                    FindMaxValue(onset_loc - window, onset_loc, out pmax_loc, out pmax_val);
                }
                else
                {
                    ponset = -1;
                    pend = -1;
                    _currentPonsetsPart.Add(ponset);
                    _currentPendsPart.Add(pend);
                    continue;
                }

                ponset = pmax_loc;
                thr = (pmax_val - InputECGData.SignalsFiltered[_currentChannelIndex].Item2[onset_loc]) * 0.4;
                while (_currentECG[ponset] > _currentECG[ponset - 1] || Math.Abs(pmax_val - _currentECG[ponset]) < thr) //dawniej 70
                {
                    ponset--;
                    if (ponset < onset_loc - break_window || ponset <1)
                    {
                        ponset = -1;
                        break;
                    }
                }
                _currentPonsetsPart.Add(ponset+_offset);

                pend = pmax_loc;
                thr = (pmax_val - _currentECG[onset_loc]) * 0.4;
                while (_currentECG[pend] > _currentECG[pend + 1] || (pmax_val - _currentECG[pend] < thr))
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


            window = Convert.ToInt32(InputData.Frequency * 0.3);
            break_window = Convert.ToInt32(InputData.Frequency * 0.35);

            foreach (int ends_loc_abs in _currentQRSendsPart)
            {
                int ends_loc = -1;
                if( ends_loc_abs != -1)
                    ends_loc = ends_loc_abs - _offset;
                if (((ends_loc + (window)) < _currentECG.Count) && ends_loc != -1)
                {
                    FindMaxValue(ends_loc, ends_loc + window, out tmax_loc, out tmax_val);
                }
                else
                {
                    tend = -1;
                    _currentTendsPart.Add(tend);
                    continue;
                }

                tend = tmax_loc;
                thr = (tmax_val - _currentECG[ends_loc]) * 0.25;
                while (_currentECG[tend] > _currentECG[tend + 1] || ((tmax_val - _currentECG[tend] < thr) && (tmax_val - _currentECG[tend] > -(tmax_val * 0.01))))
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
