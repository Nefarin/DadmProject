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

    public class Waves_Alg
    {
        private double _qrsOnsTresh;
        private double _qrsEndTresh;
        private Waves_Params _params;
        public Waves_Alg(Waves_Params parameters)
        {
            _params = parameters;
        }
        public void analyzeSignalPart( Vector<double> currentECG, Vector<double> currentRpeaks, 
            List<int> currentQRSonsetsPart, List<int> currentQRSendsPart, 
            List<int> currentPonsetsPart, List<int> currentPendsPart, List<int> currentTendsPart, int offset, uint frequency)
        {
            //if (InputData.Signals[_currentChannelIndex].Item2.Count == 0)
            //{
            //    throw new InvalidOperationException("Empty vector");
            //}

            //if (InputDataRpeaks.RPeaks[_currentChannelIndex].Item2.Count == 0)
            //{
            //    throw new InvalidOperationException("Empty vector");
            //}


            currentPendsPart.Clear();
            currentPonsetsPart.Clear();
            currentTendsPart.Clear();

            if (_params.WaveType == Wavelet_Type.haar)
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
                _qrsEndTresh = 0.2;
                _qrsOnsTresh = 0.2;
            }

            DetectQRS( currentQRSonsetsPart,  currentQRSendsPart, currentECG,  currentRpeaks,  offset);
            FindP(frequency,  offset,  currentQRSonsetsPart,
             currentECG,  currentPonsetsPart,  currentPendsPart);
            FindT(frequency,  currentQRSendsPart,  offset,  currentECG, currentTendsPart);

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
                    //Hfilter = new double[] { -0.332670552950957, 0.806891509313339, -0.459877502119331, -0.135011020010391, 0.0854412738822415, 0.0352262918821007 };
                    //Lfilter = new double[] { 0.0352262918821007, -0.0854412738822415, -0.135011020010391, 0.459877502119331, 0.806891509313339, 0.332670552950957 };
                    //filterSize = 6;
                    Hfilter = new double[] { -0.482962913144690, 0.836516303737469, -0.224143868041857, -0.129409522550921 };
                    Lfilter = new double[] { -0.129409522550921, 0.224143868041857, 0.836516303737469, 0.482962913144690 };
                    filterSize = 4;
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
        public void DetectQRS(List<int> currentQRSonsetsPart, List<int> currentQRSendsPart, 
            Vector<double> currentECG, Vector<double> currentRpeaks, int offset)
        {
            currentQRSonsetsPart.Clear();
            currentQRSendsPart.Clear();
            List<Vector<double>> dwt = new List<Vector<double>>();

            dwt = ListDWT(currentECG, _params.DecompositionLevel, _params.WaveType);

            int d2size = dwt[_params.DecompositionLevel - 1].Count();
            int rSize = _params.RpeaksStep;
            int decLev = _params.DecompositionLevel;


            currentQRSonsetsPart.Add(FindQRSOnset(0, currentRpeaks[0], dwt[decLev - 1], offset));
            int maxRInd = currentRpeaks.Count - 1;

            for (int middleR = 0; middleR < maxRInd; middleR++)
            {
                currentQRSonsetsPart.Add(FindQRSOnset(currentRpeaks[middleR], currentRpeaks[middleR + 1], dwt[decLev - 1], offset));
                currentQRSendsPart.Add(FindQRSEnd(currentRpeaks[middleR], currentRpeaks[middleR + 1], dwt[decLev - 1], _params.DecompositionLevel));

            }

            currentQRSendsPart.Add(FindQRSEnd(currentRpeaks[maxRInd], currentECG.Count, dwt[decLev - 1], _params.DecompositionLevel));
        }
        #region
        /// <summary>
        /// This method finds location of QRS-onset
        /// </summary>
        /// <returns> index of founded QRS-onset or -1 if not found</returns>
        #endregion
        public int FindQRSOnset(double drightEnd, double dmiddleR, Vector<double> dwt, int offset )
        {
            int decompLevel = _params.DecompositionLevel;
            int rightEnd = (int)drightEnd;
            int middleR = (int)dmiddleR;
            int sectionStart = (rightEnd >> decompLevel);

            int len = (middleR >> decompLevel) - (rightEnd >> decompLevel);

            if (len < 1)
                len = 1;


            if (len < 1 || sectionStart < 0)
                return -1;

            if (sectionStart + len >= dwt.Count)
                len = dwt.Count - sectionStart;

            int qrsOnsetInd = dwt.SubVector(sectionStart, len).MinimumIndex() + sectionStart;
            double treshold = Math.Abs(dwt[qrsOnsetInd]) * _qrsOnsTresh;

            while (Math.Abs(dwt[qrsOnsetInd]) > treshold && qrsOnsetInd > sectionStart)
                qrsOnsetInd--;

            if (qrsOnsetInd == sectionStart)
                return -1;
            else
                return (qrsOnsetInd << decompLevel) + offset;
        }
        #region
        /// <summary>
        /// This method finds location of QRS-ends
        /// </summary>
        /// <returns> index of founded QRS-end or -1 if not found</returns>
        #endregion
        public int FindQRSEnd(double dmiddleR, double dleftEnd, Vector<double> dwt, int offset)
        {
            int decompLevel = _params.DecompositionLevel;
            int middleR = (int)dmiddleR;
            int leftEnd = (int)dleftEnd;
            int sectionEnd = (leftEnd >> decompLevel);
            int qrsEndInd = (middleR >> decompLevel);
            //int length = 250 * (int)InputData.Frequency;

            int len = (leftEnd >> decompLevel) - qrsEndInd;

            if (len < 1)
                len = 1;



            if (qrsEndInd + len >= dwt.Count || len < 1 || qrsEndInd < 0)
            {
                return -1;
            }


            double treshold = Math.Abs(dwt.SubVector(qrsEndInd, len).Minimum()) * _qrsEndTresh;


            if (!(qrsEndInd + 1 < dwt.Count))
            {
                return -1;

            }

            while (dwt[qrsEndInd] > dwt[qrsEndInd + 1] && qrsEndInd < sectionEnd)
                qrsEndInd++;
            while (Math.Abs(dwt[qrsEndInd]) > treshold && qrsEndInd < sectionEnd)
                qrsEndInd++;

            if (qrsEndInd >= sectionEnd)
                return -1;
            else
            {
                qrsEndInd = qrsEndInd << decompLevel;
                //double val = Math.Abs(InputECGData.SignalsFiltered[_currentChannelIndex].Item2[qrsEndInd]);
                ////val = 12;
                //while (Math.Abs(InputECGData.SignalsFiltered[_currentChannelIndex].Item2[qrsEndInd] - calcMean(qrsEndInd, sectionEnd)) > 0.4 * val)
                //    qrsEndInd++;
                return qrsEndInd + offset;
            }

        }
        #region
        /// <summary>
        /// This method calc means of a part of signal
        /// </summary>
        /// <returns> Means of 1 ms part of signal counted from qrsEndInd index </returns>
        #endregion
        //double calcMean(int qrsEndInd, int sectionEnd)
        //{
        //    int length = (int)InputData.Frequency * 1;
        //    double result = 0;
        //    int i = 0;
        //    for (i = 1; i < length && i < sectionEnd && qrsEndInd + i < InputECGData.SignalsFiltered[_currentChannelIndex].Item2.Count; i++)
        //    {
        //        result += InputECGData.SignalsFiltered[_currentChannelIndex].Item2[qrsEndInd + i];
        //    }
        //    return result / (double)i;
        //}
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
        public void FindMaxValue(int begin_loc, int end_loc, out int max_loc, out double max_val, Vector<double> currentECG)
        {

            if (currentECG.Count == 0)
            {
                throw new InvalidOperationException("Empty vector");
            }
            int loc_index;

            max_val = double.MinValue;
            max_loc = 0;

            for (loc_index = begin_loc; loc_index <= end_loc; loc_index++)
            {
                if (max_val < currentECG[loc_index])
                {
                    max_val = currentECG[loc_index];
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
        public void FindP( uint frequency, int offset, List<int> currentQRSonsetsPart,
            Vector<double> currentECG, List<int> currentPonsetsPart, List<int> currentPendsPart)
        {
            double pmax_val, thr;
            int window, break_window, pmax_loc, ponset, pend;

            window = Convert.ToInt32( frequency * 0.25);
            break_window = Convert.ToInt32(frequency * 0.3);

            foreach (int onset_loc_abs in currentQRSonsetsPart)
            {
                int onset_loc = -1;
                if (onset_loc_abs != -1)
                    onset_loc = onset_loc_abs - offset;
                if ((onset_loc - (window)) >= 1 && onset_loc != -1)
                {
                    FindMaxValue(onset_loc - window, onset_loc, out pmax_loc, out pmax_val, currentECG);
                }
                else
                {
                    ponset = -1;
                    pend = -1;
                    currentPonsetsPart.Add(ponset);
                    currentPendsPart.Add(pend);
                    continue;
                }

                ponset = pmax_loc;
                thr = (pmax_val - currentECG[onset_loc]) * 0.4;
                while (currentECG[ponset] > currentECG[ponset - 1] || Math.Abs(pmax_val - currentECG[ponset]) < thr) //dawniej 70
                {
                    ponset--;
                    if (ponset < onset_loc - break_window || ponset < 1)
                    {
                        ponset = -1;
                        break;
                    }
                }

                if (ponset != -1)
                    currentPonsetsPart.Add(ponset + offset);
                else
                    currentPonsetsPart.Add(ponset );

                pend = pmax_loc;
                thr = (pmax_val - currentECG[onset_loc]) * 0.4;
                while (currentECG[pend] > currentECG[pend + 1] || (pmax_val - currentECG[pend] < thr))
                {
                    pend++;
                    if (pend > onset_loc)
                    {
                        pend = -1;
                        break;
                    }
                }
                if( pend != -1)
                    currentPendsPart.Add(pend + offset);
                else
                    currentPendsPart.Add(pend);
            }
        }

        #region
        /// <summary>
        /// This method finds locations of T-ends
        /// </summary>
        /// <returns> List containing locations of T-ends</returns>
        #endregion
        public void FindT( uint frequency, List<int> currentQRSendsPart, int offset, Vector<double> currentECG,
            List<int> currentTendsPart)
        {
            double tmax_val, thr;
            int window, break_window, tmax_loc, tend;


            window = Convert.ToInt32(frequency * 0.3);
            break_window = Convert.ToInt32(frequency * 0.35);

            foreach (int ends_loc_abs in currentQRSendsPart)
            {
                int ends_loc = -1;
                if (ends_loc_abs != -1)
                    ends_loc = ends_loc_abs - offset;
                if (((ends_loc + (window)) < currentECG.Count) && ends_loc != -1)
                {
                    FindMaxValue(ends_loc, ends_loc + window, out tmax_loc, out tmax_val, currentECG);
                }
                else
                {
                    tend = -1;
                    currentTendsPart.Add(tend);
                    continue;
                }

                tend = tmax_loc;
                thr = (tmax_val - currentECG[ends_loc]) * 0.25;
                while (currentECG[tend] > currentECG[tend + 1] || ((tmax_val - currentECG[tend] < thr) && (tmax_val - currentECG[tend] > -(tmax_val * 0.01))))
                {
                    tend++;
                    if (tend > ends_loc + break_window)
                    {
                        tend = -1;
                        break;
                    }
                }
                currentTendsPart.Add(tend);
            }
        }
    }


}
