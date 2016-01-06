﻿using System;
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
        
        public void analyzeSignalPart( int channel, int step, int startIndex)
        {
            ;
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
            double[] Hfilter =  { 0 };
            double[] Lfilter = { 0 };
            int filterSize = 0;
                //generated from wfilters Matlab function
                switch (waveType)
                {
                case Wavelet_Type.haar:
                    return ListHaarDWT(signal, n);

                case Wavelet_Type.db2:
                    Hfilter = new double []{ -0.482962913144690 ,    0.836516303737469, -0.224143868041857 ,-0.129409522550921};
                    Lfilter = new double[] { -0.129409522550921, 0.224143868041857, 0.836516303737469, 0.482962913144690 };
                    filterSize = 4;
                    break;

                case Wavelet_Type.db3:
                    Hfilter = new double[] { -0.332670552950957,  0.806891509313339, - 0.459877502119331, - 0.135011020010391,  0.0854412738822415,  0.0352262918821007 };
                    Lfilter = new double[] { 0.0352262918821007, - 0.0854412738822415, - 0.135011020010391,  0.459877502119331,   0.806891509313339,   0.332670552950957 };
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
                        for( int filtIt = 0; filtIt < filterSize && (2*dataInd + filtIt) < signalTemp.Count ; filtIt++)
                        {
                            outVec[dataInd] += signalTemp[2 * dataInd + filtIt]*Lfilter[filterSize - filtIt - 1] ;
                            outVec[decompSize + dataInd] += signalTemp[2 * dataInd + filtIt]* Hfilter[filterSize - filtIt - 1];
                        }
                        
                    }
                    signalTemp = outVec.SubVector(0, decompSize);
                    listOut.Add(outVec.SubVector(decompSize, decompSize));
                }
                return listOut;

        }

        public void DetectQRS()
        {
            _currentQRSonsets.Clear();
            List<Vector<double>> dwt =new List< Vector<double>>();

            dwt = ListDWT(InputData.Signals[_currentChannelIndex].Item2, _params.DecompositionLevel , _params.WaveType);
            
            int d2size = dwt[_params.DecompositionLevel - 1].Count();
            int rSize = InputDataRpeaks.RPeaks[_currentChannelIndex].Item2.Count();

            _currentQRSonsets.Add(FindQRSOnset(0, _data.Rpeaks[0], dwt[1], _params.DecompositionLevel));
            for (int middleR = 1; middleR < rSize - 1; middleR++ )
            {
                _currentQRSonsets.Add(FindQRSOnset(_data.Rpeaks[middleR-1], _data.Rpeaks[middleR], dwt[1], _params.DecompositionLevel));
                _currentQRSends.Add(FindQRSEnd(_data.Rpeaks[middleR], _data.Rpeaks[middleR + 1], dwt[1], _params.DecompositionLevel));
            }
            _currentQRSends.Add(FindQRSEnd(_data.Rpeaks[rSize - 1], InputData.Signals[_currentChannelIndex].Item2.Count - 1, dwt[1], _params.DecompositionLevel));

        }

        public int FindQRSOnset( int rightEnd, int middleR, Vector<double> dwt, int decompLevel)
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

        public int FindQRSEnd( int middleR, int leftEnd, Vector<double> dwt, int decompLevel)
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

            if (InputData.Signals[_currentChannelIndex].Item2.Count == 0)
            {
                throw new InvalidOperationException("Empty vector");
            }
            int loc_index;

            max_val = double.MinValue;
            max_loc = 0;

            for (loc_index = begin_loc; loc_index <= end_loc; loc_index++)
            {
                if (max_val < InputData.Signals[_currentChannelIndex].Item2[loc_index])
                {
                    max_val = InputData.Signals[_currentChannelIndex].Item2[loc_index];
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
            double pmax_val;
            int window,break_window,pmax_loc,ponset,pend;

            window = Convert.ToInt32(InputData.Frequency*0.25);
            break_window = Convert.ToInt32(InputData.Frequency * 0.3);

            foreach (int onset_loc in _currentQRSonsets)
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
                while(InputData.Signals[_currentChannelIndex].Item2[ponset] > InputData.Signals[_currentChannelIndex].Item2[ponset-1] || (pmax_val- InputData.Signals[_currentChannelIndex].Item2[ponset] < 70))
                {
                    ponset--;
                    if (ponset < onset_loc - break_window)
                    {
                        ponset = -1;
                        break;
                    }
                }
                _currentPonsets.Add(ponset);

                pend = pmax_loc;
                while (InputData.Signals[_currentChannelIndex].Item2[pend] > InputData.Signals[_currentChannelIndex].Item2[pend+1] || (pmax_val - InputData.Signals[_currentChannelIndex].Item2[pend] < 110))
                {
                    pend++;
                    if (pend > onset_loc)
                    {
                        pend = -1;
                        break;
                    }
                }
                _currentPends.Add(pend);
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
            double tmax_val;
            int window,break_window,tmax_loc, tend;


            window = Convert.ToInt32(InputData.Frequency * 0.2);
            break_window = Convert.ToInt32(InputData.Frequency * 0.35);

            foreach (int ends_loc in _currentQRSends)
            {
                if (((ends_loc + (window)) < InputData.Signals[_currentChannelIndex].Item2.Count) && ends_loc != -1)
                {
                    FindMaxValue(ends_loc, ends_loc + window, out tmax_loc, out tmax_val);
                }
                else
                {
                    continue;
                }

                tend = tmax_loc;
                while (InputData.Signals[_currentChannelIndex].Item2[tend] > InputData.Signals[_currentChannelIndex].Item2[tend + 1] || ((tmax_val - InputData.Signals[_currentChannelIndex].Item2[tend] < 30) && (tmax_val - InputData.Signals[_currentChannelIndex].Item2[tend] > -10)))
                {
                    tend++;
                    if(tend > ends_loc+break_window)
                    {
                        tend = -1;
                        break;
                    }
                }
                _currentTends.Add(tend);
            }
        }
    }
    

}
