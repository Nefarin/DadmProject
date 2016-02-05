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
                _qrsEndTresh = 1.5;
                _qrsOnsTresh = 3;
            }
            else
            {
                _qrsEndTresh = 3;
                _qrsOnsTresh = 1.5;
            }

            DetectQRS( currentQRSonsetsPart,  currentQRSendsPart, currentECG,  currentRpeaks,  offset, frequency);
            FindP(frequency,  offset,  currentQRSonsetsPart,
             currentECG,  currentPonsetsPart,  currentPendsPart);
            //_params.WaveType = Wavelet_Type.haar;
            FindT(frequency,  currentQRSendsPart,  offset,  currentECG, currentTendsPart);

        }

        private List<Vector<double>> ListHaarDWT(Vector<double> signal, int n)
        {
            //Work just like wavedec but use only haar wavelet
            // http://www.mathworks.com/help/wavelet/ref/wavedec.html
            // [0]  -> d1, [1]  -> d2, ...
            int decompSize = signal.Count();
            if( n <1 )
                throw new InvalidOperationException("Decomposition level is too low");
            if ( decompSize >> n < 2)
                throw new InvalidOperationException("Not long enough signal for such decomposition");
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
        private List<Vector<double>> ListDWT(Vector<double> signal, int n, Wavelet_Type waveType)
        {
            double[] Hfilter = { 0 };
            double[] Lfilter = { 0 };
            int filterSize = 0;
            if (n < 1)
                throw new InvalidOperationException("Decomposition level is too low");
            if (signal.Count >> n < 2)
                throw new InvalidOperationException("Not long enough signal for such decomposition");
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

                default: return ListHaarDWT(signal, n);
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
        private void DetectQRS(List<int> currentQRSonsetsPart, List<int> currentQRSendsPart, 
            Vector<double> currentECG, Vector<double> currentRpeaks, int offset, uint freq)
        {
            currentQRSonsetsPart.Clear();
            currentQRSendsPart.Clear();
            List<Vector<double>> dwt = new List<Vector<double>>();

            dwt = ListDWT(currentECG, _params.DecompositionLevel, _params.WaveType);

            int d2size = dwt[_params.DecompositionLevel - 1].Count();
            int rSize = _params.RpeaksStep;
            int decLev = _params.DecompositionLevel;


            currentQRSonsetsPart.Add(FindQRSOnset(0, currentRpeaks[0], dwt[decLev - 1], offset, currentECG, freq));
            int maxRInd = currentRpeaks.Count - 1;

            for (int middleR = 0; middleR < maxRInd; middleR++)
            {
                currentQRSonsetsPart.Add(FindQRSOnset(currentRpeaks[middleR], currentRpeaks[middleR + 1], dwt[decLev - 1], offset, currentECG, freq));
                currentQRSendsPart.Add(FindQRSEnd(currentRpeaks[middleR], currentRpeaks[middleR + 1], dwt[decLev - 1], offset, currentECG, freq));

            }

            currentQRSendsPart.Add(FindQRSEnd(currentRpeaks[maxRInd], currentECG.Count, dwt[decLev - 1], offset, currentECG, freq));
        }
        #region
        /// <summary>
        /// This method finds location of QRS-onset
        /// </summary>
        /// <returns> index of founded QRS-onset or -1 if not found</returns>
        #endregion
        private int FindQRSOnset(double drightEnd, double dmiddleR, Vector<double> dwt, int offset,
            Vector<double> currentECG , uint freq)
        {
            int decompLevel = _params.DecompositionLevel;
            int rightEnd = (int)drightEnd;
            int middleR = (int)dmiddleR;
            int sectionStart = (rightEnd >> decompLevel);

            int len = (middleR >> decompLevel) - (rightEnd >> decompLevel);
            len = (len >> 1);
            if (sectionStart < 0)
                return -1;

            if (sectionStart + len >= dwt.Count)
                len = dwt.Count - sectionStart;

            if (len < 1)
                return -1;

            int qrsOnsetInd = dwt.SubVector(sectionStart+len, len).MinimumIndex() + sectionStart+len;
            double treshold = Math.Abs(dwt[qrsOnsetInd]) * _qrsOnsTresh;

            if (dmiddleR >= currentECG.Count)
                dmiddleR = currentECG.Count - 1;

            while (Math.Abs(dwt[qrsOnsetInd]) > treshold && qrsOnsetInd > sectionStart)
                qrsOnsetInd--;

            if (qrsOnsetInd == sectionStart)
                return -1;
            else
            {
                qrsOnsetInd = (qrsOnsetInd << decompLevel);
                double Rval = currentECG[(int)dmiddleR];
                int samples2analyse = (int)(freq * 0.04);
                while (lastNderivSquares(samples2analyse, qrsOnsetInd, currentECG) > 0.004 * Rval && qrsOnsetInd < rightEnd)
                {
                    qrsOnsetInd--;
                }

                return qrsOnsetInd + offset;
            }
        }
        #region
        /// <summary>
        /// This method finds location of QRS-ends
        /// </summary>
        /// <returns> index of founded QRS-end or -1 if not found</returns>
        #endregion
        private int FindQRSEnd(double dmiddleR, double dleftEnd, Vector<double> dwt, int offset, Vector<double> currentECG, uint freq)
        {
            int decompLevel = _params.DecompositionLevel;
            int middleR = (int)dmiddleR;
            int leftEnd = (int)dleftEnd;
            leftEnd -= (2 << decompLevel);
            int sectionEnd = (leftEnd >> decompLevel);
            int qrsEndInd = (middleR >> decompLevel);
            //int length = 250 * (int)InputData.Frequency;

            int len = (leftEnd >> decompLevel) - qrsEndInd;

            if (len < 1)
                len = 1;



            if (qrsEndInd + len > dwt.Count || len < 1 || qrsEndInd < 0)
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
                double Rval = currentECG[(int)dmiddleR];
                int samples2analyse = (int)(freq * 0.02);
                qrsEndInd = (qrsEndInd << decompLevel);
                while (Math.Abs(currentECG[qrsEndInd] - Rval) / Rval < 0.95 && qrsEndInd < leftEnd)
                    qrsEndInd++;
                while (currentECG[qrsEndInd] > currentECG[qrsEndInd + 1] && qrsEndInd < leftEnd)
                    qrsEndInd++;
                while (lastNderivSquares(samples2analyse, qrsEndInd, currentECG) > 0.02 * Rval && qrsEndInd < leftEnd)
                    qrsEndInd++;
                while (nextNderivSquares(samples2analyse, qrsEndInd, currentECG) > 0.02 * Rval && qrsEndInd < leftEnd)
                    qrsEndInd++;
                return qrsEndInd + offset;
            }

        }
        private double lastNderivSquares(int n, int index, Vector<double> signal)
        {
            double res = 0;
            for (int i = 0; i < n; i++)
            {
                res += derivSquare(index - i, signal);
            }
            return res;
        }
        private double nextNderivSquares(int n, int index, Vector<double> signal)
        {
            double res = 0;
            for (int i = 0; i < n; i++)
            {
                res += derivSquare(index + i, signal);
            }
            return res;
        }
        private double derivSquare(int index, Vector<double> signal)
        {
            double res = 0;
            if (index - 2 > 0 && index + 2 < signal.Count)
                res = 0.125 * (-signal[index - 2] - 2 * signal[index - 1] + 2 * signal[index + 1] + signal[index + 2]);
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
        public void FindMaxValue(int begin_loc, int end_loc, out int max_loc, out double max_val, Vector<double> currentECG)
        {

            if (currentECG.Count == 0)
            {
                throw new InvalidOperationException("Empty vector"); // Exception if signal is empty
            }
            int loc_index; // variable for indexing location of samples in signal

            max_val = double.MinValue; // initialize maximum value
            max_loc = 0; // initialize maximum value location

            for (loc_index = begin_loc; loc_index <= end_loc; loc_index++)
            {
                if (max_val < currentECG[loc_index])
                {
                    max_val = currentECG[loc_index]; // if current sample is greater than previous maximum then current sample is maximum value
                    max_loc = loc_index; // if current sample is greater than previous maximum then current sample location is maximum value location
                }
            }

        }

        #region
        /// <summary>
        /// This method finds locations of P-onsets and P-ends
        /// </summary>
        /// <param name="frequency"> Sampling frequency of signal</param>
        /// <param name="offset"> Signal shift during division of signal</param>
        /// <param name="currentQRSonsetsPart"> QRSonsets location in current signal part</param>
        /// <param name="currentECG"> Current signal</param>
        /// <param name="currentPonsetsPart"> Ponsets location in current signal part</param>
        /// <param name="currentPendsPart"> Pends location in current signal part</param>
        /// <returns> List containing locations of P-onsets and P-ends</returns>
        #endregion
        public void FindP( uint frequency, int offset, List<int> currentQRSonsetsPart,
            Vector<double> currentECG, List<int> currentPonsetsPart, List<int> currentPendsPart)
        {
            double pmax_val, thr; // initialization of P-wave maximum and threshold variables
            int window, break_window, pmax_loc, ponset, pend; // initialization of P-wave maximum location, Ponsets location, Pends location and window variables

            window = Convert.ToInt32( frequency * 0.25); // window length
            break_window = Convert.ToInt32(frequency * 0.3); // length of window to break searching for maximum

            foreach (int onset_loc_abs in currentQRSonsetsPart)
            {
                int onset_loc = -1; // initialization of QRSonset location
                if (onset_loc_abs != -1)
                    onset_loc = onset_loc_abs - offset; // offset adjustment to current part of signal
                if ((onset_loc - (window)) >= 1 && onset_loc != -1)
                {
                    FindMaxValue(onset_loc - window, onset_loc, out pmax_loc, out pmax_val, currentECG); // find maximum of P-wave on length of window
                }
                else
                {
                    ponset = -1; // if Ponset is on recognized then write as -1
                    pend = -1; // if Pend is on recognized then write as -1
                    currentPonsetsPart.Add(ponset); // add -1 to current Ponsets list
                    currentPendsPart.Add(pend); // add -1 to current Pends list
                    continue;
                }

                ponset = pmax_loc; // initialize Ponset location with maximum location
                thr = (pmax_val - currentECG[onset_loc]) * 0.5; // set threshold equal to percentage of level from maximum value and QRSonset
                while (currentECG[ponset] > currentECG[ponset - 1] || Math.Abs(pmax_val - currentECG[ponset]) < thr) // find local minimum and check if threshold is exceeded
                {
                    ponset--; // move backwards in samples
                    if (ponset < onset_loc - break_window || ponset < 1) // check if current location exceeded length of break window
                    {
                        ponset = -1; // set location to -1
                        break;
                    }
                }

                if (ponset != -1)
                    currentPonsetsPart.Add(ponset + offset); // add new Ponset location to the list
                else
                    currentPonsetsPart.Add(ponset ); // add -1 to the list

                pend = pmax_loc; // initialize Pend location with maximum location
                thr = (pmax_val - currentECG[onset_loc]) * 0.5; // set threshold equal to percentage of level from maximum value and QRSonset
                while (currentECG[pend] > currentECG[pend + 1] || (pmax_val - currentECG[pend] < thr)) // find local minimum and check if threshold is exceeded
                {
                    pend++; // move forward in samples
                    if (pend > onset_loc) // check if Pend location exceeded QRSonset location
                    {
                        pend = -1; // set location to -1
                        break;
                    }
                }
                if( pend != -1)
                    currentPendsPart.Add(pend + offset); // add new Pend location to the list
                else
                    currentPendsPart.Add(pend); // add -1 to the list
            }
        }

        #region
        /// <summary>
        /// This method finds locations of T-ends
        /// </summary>
        /// <param name="frequency"> Sampling frequency of signal</param>
        /// <param name="offset"> Signal shift during division of signal</param>
        /// <param name="currentQRSendsPart"> QRSends location in current signal part</param>
        /// <param name="currentECG"> Current signal</param>
        /// <param name="currentTendsPart"> Tends location in current signal part</param>
        /// <returns> List containing locations of T-ends</returns>
        #endregion
        public void FindT( uint frequency, List<int> currentQRSendsPart, int offset, Vector<double> currentECG,
            List<int> currentTendsPart)
        {
            double tmax_val, thr; // initialization of T-wave maximum and threshold variables
            int window, break_window, tmax_loc, tend; // initialization of T-wave maximum location, Tends location and window variables
            int maxTendVal = currentECG.Count-3;

            window = Convert.ToInt32(frequency * 0.35); // window length
            break_window = Convert.ToInt32(frequency * 0.4); // length of window to break searching for maximum

            foreach (int ends_loc_abs in currentQRSendsPart)
            {
                int ends_loc = -1; // initialization of QRSend location
                if (ends_loc_abs != -1)
                    ends_loc = ends_loc_abs - offset; // offset adjustment to current part of signal
                if (((ends_loc + (window)) < currentECG.Count) && ends_loc != -1)
                {
                    FindMaxValue(ends_loc, ends_loc + window, out tmax_loc, out tmax_val, currentECG); // find maximum of T-wave on length of window
                }
                else
                {
                    tend = -1; // if Tend is on recognized then write as -1
                    currentTendsPart.Add(tend); // add -1 to current Tends list
                    continue;
                }

                tend = tmax_loc; // initialize Tend location with maximum location
                thr = (tmax_val - currentECG[ends_loc]) * 0.25; // set threshold equal to percentage of level from maximum value and QRSend
                while (currentECG[tend] > currentECG[tend + 1] || ((tmax_val - currentECG[tend] < thr) && (tmax_val - currentECG[tend] > -(tmax_val * 0.01)))) // find local minimum and check if threshold is exceeded
                {
                    tend++; // move forward in samples
                    if (tend > ends_loc + break_window || tend > maxTendVal) // check if current location exceeded length of break window
                    {
                        tend = -1; // set location to -1
                        break;
                    }
                }
                if (tend != -1)
                    currentTendsPart.Add(tend + offset); // add new Tend location to the list
                else
                    currentTendsPart.Add(tend); // add -1 to the list
            }
        }
    }


}
