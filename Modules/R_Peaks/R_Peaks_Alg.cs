using System;
using System.Collections.Generic;
using MathNet.Filtering.FIR;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;
using EKG_Project.IO;
using EKG_Project.Modules.ECG_Baseline;
using System.Linq;

namespace EKG_Project.Modules.R_Peaks
{
    #region R_Peaks Class doc
    /// <summary>
    /// Class that locates the R peaks in ECG 
    /// </summary>
    #endregion
    public partial class R_Peaks : IModule
    {
        //TO DO: parts of signal reading


        static void Main(string[] args)
        {
            #region readData
            //read data from ecg_baseline (TO DO!)
            //List<Tuple<string, Vector<double>>> R_Peaks = new List<Tuple<string, Vector<double>>>();
            // Vector<double> sigs = ECG_Baseline_Data.SignalFiltered ;
            //Tuple<string, Vector <double>> sig_data = sigs.Item[0];
            //  string channel = sig_data.Item1;
            // Vector<double> sig = sig_data.Item2;

            //read data from dat file
            TempInput.setInputFilePath(@"D:\100in.txt");
            uint fs = TempInput.getFrequency();
            Vector<double> sig = TempInput.getSignal();
            #endregion
            double samplingFreq = Convert.ToDouble(fs);
            R_Peaks pt = new R_Peaks();
            R_Peaks h = new R_Peaks();
            double[] arr_sig = sig.ToArray();
            //Vector<double> locsR = h.Hilbert(sig, fs);





            /*//RR in ms
            pt.RRms = pt.Diff(pt.LocsR);
            pt.RRms.Multiply(Math.Round(1000 / Convert.ToDouble(fs), 3), pt.RRms);*/

            #region writeData
            //write result to DATA
            //Tuple<string, Vector<double>> r_data = Tuple.Create<channel, Vector<int> rPeaks>;
            // R_Peaks.Add(r_data);

            //write result to dat file
            //TempInput.setOutputFilePath(@"D:\100out.txt");
            //TempInput.writeFile(fs, Vector<double>.Build.DenseOfArray(vector_f));
            /*TempInput.setOutputFilePath(@"D:\biomed\DADM\C#\100v5RR.txt");
            TempInput.writeFile(fs, pt.RRms);*/
            #endregion

            //TEST-Console
            Console.WriteLine(h.Delay);
            Console.WriteLine(1 / (samplingFreq / 2));
            Console.WriteLine();
            //foreach (double sth in locsR) { Console.WriteLine(sth); }
            Console.ReadKey();
        }

        //FIELDS
        #region 
        /// <summary>
        /// Store the value of delay in samples generates due to processing
        /// </summary>
        #endregion
        private uint _delay;
        #region
        /// <summary>
        /// Store the numbers of indexes of R peaks in signal ECG as vector
        /// </summary>
        #endregion
        private Vector<double> _locsR;
        #region
        /// <summary>
        /// Store the values of RR intervals in ms as vector
        /// </summary>
        #endregion
        private Vector<double> _RRms;

        public uint Delay
        {
            set { _delay = value; }
            get { return _delay; }
        }
        public Vector<double> LocsR
        {
            set { _locsR = value; }
            get { return _locsR; }
        }
        public Vector<double> RRms
        {
            set { _RRms = value; }
            get { return _RRms; }
        }

        //METHODS
        #region
        /// <summary>
        /// Function that filters the signal by FIR filter (bandpass, 21 order)
        /// </summary>
        /// <param name="samplingFreq"> frequency of sampling for signal</param>
        /// <param name="lowCutOff"> Low cut off ferquency</param>
        /// <param name="highCutOff"> High cut off frequency</param>
        /// <param name="rawSignal"> Signal which are going to be filtered </param>
        /// <returns> filtered signal of ECG as a double array</returns>
        #endregion
        public double[] Filtering(double samplingFreq, double lowCutOff, double highCutOff, double[] rawSignal)
        {
            //TO DO: add cutoffs as constants not param?
            Delay += 10;
            IList<double> coef = new List<double>();

            double[] hf = FirCoefficients.BandPass(samplingFreq, lowCutOff, highCutOff, 10);
            foreach (double number in hf)
            {
                coef.Add(number);
            }

            OnlineFirFilter filter = new OnlineFirFilter(coef);
            double[] vector_f = filter.ProcessSamples(rawSignal);
            return vector_f;
        }

        #region
        /// <summary>
        /// Function that differentiate the signal (derivative filter H(z)=1/8T(-z^-2-2z^-1 +2z+2z^2))
        /// </summary>
        /// <param name="filteredSignal"> Filtered ECG signal</param>
        /// <returns> derrivative signal of ECG as a double array</returns>
        #endregion
        public double[] Derivative(double[] filteredSignal)
        {
            Delay += 2;
            IList<double> hd_coef = new List<double>();
            double[] hd_array = { -1, -2, 0, 2, 1 };
            foreach (double num in hd_array)
            {
                hd_coef.Add(num);
            }
            OnlineFirFilter diffFilter = new OnlineFirFilter(hd_coef);
            double[] derivativeSignal = diffFilter.ProcessSamples(filteredSignal);
            return derivativeSignal;
        }

        #region
        /// <summary>
        /// Function that squares the signal 
        /// </summary>
        /// <param name="derivativeSignal"> derivative signal of ECG returned by Derivative method</param>
        /// <returns> Squared values of derivative of ECG as a double array </returns>
        #endregion
        public double[] Squaring(double[] derivativeSignal)
        {
            double[] squaredSignal = new double[derivativeSignal.Length];
            for (int i = 0; i < derivativeSignal.Length; i++)
            {
                squaredSignal[i] = derivativeSignal[i] * derivativeSignal[i];
            }
            return squaredSignal;
        }

        #region
        /// <summary>
        /// Function that integrates the signal in moving-window (width equals 150 ms)
        /// </summary>
        /// <param name="squaredSignal"> squared signal returnet by Squaring method</param>
        /// <param name="fs"> sampling frequency of anlysed signal</param>
        /// <returns> integration of signal as a double array </returns>
        #endregion
        public double[] Integrating(double[] squaredSignal, uint fs)
        {
            double[] integratedSignal = new double[squaredSignal.Length];
            double window = Math.Round(0.15 * fs);
            Delay += Convert.ToUInt32(Math.Round(window / 2));
            IList<double> hi_coeff = new List<double>();
            for (int i = 0; i < window; i++)
            {
                hi_coeff.Add(1 / window);
            }
            OnlineFirFilter integrationFilter = new OnlineFirFilter(hi_coeff);
            integratedSignal = integrationFilter.ProcessSamples(squaredSignal);

            //enhancing by put zero below threshold (0.002)
            for (int i = 0; i < integratedSignal.Length; i++)
            {
                if (integratedSignal[i] < 0.002)
                    integratedSignal[i] = 0;
            }
            return integratedSignal;
        }

        #region
        /// <summary>
        /// Function that detects peaks (local maxima) in signal which are located in determined distance from each other
        /// </summary>
        /// <param name="signal"> Analysed signal with local maxima</param>
        /// <param name="fs"> sampling  grequency of the signal</param>
        /// <param name="distanceInSec"> Distance in seconds - minimum distance between next peaks</param>
        /// <returns> list of double which contains teh localisation of peaks in signal </returns>
        #endregion
        public List<double> FindPeaks(double[] signal, uint fs, double distanceInSec)
        //TO DO: distance--> threshold???
        {
            List<double> potRs = new List<double>();
            double distanceInSamples = fs * distanceInSec;
            for (int i = 1; i < signal.Length - 1; i++)
            {
                if ((signal[i] > signal[i - 1]) && (signal[i] > signal[i + 1]))
                {
                    potRs.Add(i);
                }
            }
            int j = 1;                          //remove maximas which are closer to previous maximum than distance
            double prevR = potRs.First();
            while (j < potRs.Count)
            {
                if (potRs[j] - prevR < distanceInSamples)
                    potRs.RemoveAt(j);
                else
                {
                    prevR = potRs[j];
                    j++;
                }
            }
            return potRs;
        }

        #region
        /// <summary>
        /// Function that choose some fragment of the input signal(between begin and end)
        /// </summary>
        /// <param name="inputSignal"> The signal form which we cut its fragment </param>
        /// <param name="begin"> the begining sample  of signal (implicit)</param>
        /// <param name="end"> the last sample of signal (implicit)</param>
        /// <returns> cutted signal from teh whole signal as Vector of double </returns>
        #endregion
        //SUBVECTOR from MATHNET?!
        public Vector<double> CutSignal(Vector<double> inputSignal, int begin, int end)
        {
            int len = end - begin + 1;
            double[] cuttedSignal = new double[len];
            for (int i = 0; i < len; i++)
            {
                cuttedSignal[i] = inputSignal[i + begin];
            }
            return Vector<double>.Build.DenseOfArray(cuttedSignal);
        }

        #region
        /// <summary>
        /// Function that calculates differential vector of  input signal
        /// </summary>
        /// <param name="vect"></param>
        /// <returns></returns>
        #endregion
        public Vector<double> Diff(Vector<double> signal)
        {
            Vector<double> diffSignal = Vector<double>.Build.Dense(signal.Count - 1);
            signal.SubVector(1, signal.Count - 1).Subtract(signal.SubVector(0, signal.Count - 1), diffSignal);
            return diffSignal;
        }

        #region
        /// <summary>
        /// Function which finds in ECG signal R peaks by adaptive thersholding and reverse search
        /// </summary>
        /// <param name="integratedSignal"> integrated signal of ECG</param>
        /// <param name="filteredSignal"> filtereg signal of ECG</param>
        /// <param name="fs"> sampling frequency of signal</param>
        /// <returns> Numbers of samples of R peaks in ECG signal as list of double </returns>
        #endregion
        public Vector<double> findRs(double[] integratedSignal, Vector<double> filteredSignal, uint fs)
        {
            Vector<double> integratedSignalV = Vector<double>.Build.DenseOfArray(integratedSignal);

            //init temp values
            List<double> locsR = new List<double>();
            List<double> locsRi = new List<double>();
            double selectedRR = 0;
            double testRR = 0;
            double mRR = 0;
            bool serback = false;
            bool skip = false;
            double potR = 0;
            double potAmp = 0;

            //init thresholds for integrated signal
            Vector<double> sig_ic = CutSignal(integratedSignalV, 0, Convert.ToInt16(2 * fs));
            double thrSigI = sig_ic.Maximum() / 3;
            double thrNoiseI = sig_ic.Average() / 2;
            double levSigI = thrSigI;
            double levNoiseI = thrNoiseI;
            //init thersholds for filtered signal
            Vector<double> sig_fc = CutSignal(filteredSignal, 0, Convert.ToInt16(2 * fs));
            double thrSig = sig_fc.Maximum() / 3;
            double thrNoise = sig_fc.Average() / 2;
            double levSig = thrSig;
            double levNoise = thrNoise;

            //detecting peaks in both signals(integrated and filtered)
            List<double> potRsI = FindPeaks(integratedSignal, fs, 0.2);
            foreach (int r in potRsI)
            {
                //detect peaks in filetred signal
                int window = Convert.ToInt16(0.15 * fs);
                if ((r <= filteredSignal.Count) && (r - window >= 0))
                {
                    Vector<double> tempSig = CutSignal(filteredSignal, r - window, r);
                    potR = tempSig.MaximumIndex();
                    potAmp = tempSig.Maximum();
                }
                else if (r > filteredSignal.Count)
                {
                    Vector<double> tempSig = CutSignal(filteredSignal, r - window, filteredSignal.Count - 1);
                    potR = tempSig.MaximumIndex();
                    potAmp = tempSig.Maximum();
                }
                else
                {
                    Vector<double> tempSig = CutSignal(filteredSignal, 0, r);
                    potR = tempSig.MaximumIndex();
                    potAmp = tempSig.Maximum();
                    serback = true;
                }

                //updating HR and thersholds
                if (locsR.Count >= 9)
                {
                    List<double> lastRs = locsRi.GetRange(locsRi.Count - 8, 8);
                    Vector<double> tempRR = Diff(Vector<double>.Build.DenseOfArray(lastRs.ToArray()));
                    mRR = tempRR.Mean();
                    double lastRR = locsRi.Last() - locsRi[locsRi.Count - 1];
                    if (lastRR <= 0.92 * mRR || lastRR >= 1.66 * mRR) //lower thersholds if irregular beat
                    {
                        thrSig = 0.5 * thrSig;
                        thrSigI = 0.5 * thrSigI;
                    }
                    else        //regular beat
                    {
                        selectedRR = mRR;
                    }
                }

                // trigger search back if missing R
                if (selectedRR != 0) { testRR = selectedRR; }
                else if (mRR != 0 && selectedRR == 0) { testRR = mRR; }
                else { testRR = 0; }

                //searchback
                if (testRR != 0)
                {
                    if ((r - locsRi.Last()) >= 1.66 * testRR)
                    {
                        int buff = Convert.ToInt16(0.2 * fs);
                        int beg = Convert.ToInt16(locsRi.Last()) + buff;
                        int en = Convert.ToInt16(potRsI.Last()) - buff;
                        Vector<double> tempSigI = CutSignal(integratedSignalV, beg, en);
                        double tempPeak = tempSigI.Maximum();
                        double tempInd = tempSigI.MaximumIndex() + beg;
                        if (tempPeak > thrNoiseI)
                        {
                            locsRi.Add(tempInd);
                            double tempPotR = 0;
                            double tempAmpR = 0;
                            if (tempInd < filteredSignal.Count)
                            {
                                Vector<double> tempSig = CutSignal(filteredSignal, Convert.ToInt16(tempInd) - window, Convert.ToInt16(tempInd));
                                tempPotR = tempSig.MaximumIndex();
                                tempAmpR = tempSig.Maximum();
                            }
                            else
                            {
                                Vector<double> tempSig = CutSignal(filteredSignal, Convert.ToInt16(tempInd) - window, filteredSignal.Count);
                                tempPotR = tempSig.MaximumIndex();
                                tempAmpR = tempSig.Maximum();
                            }

                            if (tempAmpR > thrNoise)
                            {
                                locsR.Add(tempPotR + Convert.ToInt16(tempInd) - window);
                                levSig = 0.25 * tempAmpR + 0.75 * levSig;
                            }
                            levSigI = 0.25 * tempPeak + 0.75 * levSigI;
                        }
                    }
                }

                //find noise and peaks
                if (integratedSignal[r] >= thrSigI)
                {
                    if (locsRi.Count >= 3)
                    {
                        if (r - locsRi.Last() <= Math.Round(0.360 * fs))
                        {
                            Vector<double> tempSig1 = CutSignal(filteredSignal, r - Convert.ToInt16(0.075 * fs), r);
                            double slope1 = Math.Abs(Diff(tempSig1).Mean());
                            Vector<double> tempSig2 = CutSignal(filteredSignal, Convert.ToInt16(locsRi.Last() - Math.Round(0.075 * fs)), Convert.ToInt16(locsRi.Last()));
                            double slope2 = Math.Abs(Diff(tempSig2).Mean());
                            if (slope1 <= 0.5 * slope2)
                            {
                                skip = true;
                                levNoise = 0.125 * potAmp + 0.875 * levNoise;
                                levNoiseI = 0.125 * integratedSignal[r] + 0.875 * levNoiseI;
                            }
                            else { skip = false; }
                        }
                    }
                    if (!skip)
                    {
                        locsRi.Add(r);
                        if (potAmp >= thrSig)
                        {
                            if (serback) { locsR.Add(potR); }
                            else { locsR.Add(potR + r - window); }
                            levSig = 0.125 * potAmp + 0.875 * levSig;
                        }
                        levSigI = 0.125 * integratedSignal[r] + 0.875 * levSigI;
                    }
                }
                else if (thrNoiseI <= integratedSignal[r] && integratedSignal[r] < thrSigI)
                {
                    levNoise = 0.125 * potAmp + 0.875 * levNoise;
                    levNoiseI = 0.125 * integratedSignal[r] + 0.875 * levNoiseI;
                }
                else if (integratedSignal[r] < thrNoiseI)
                {
                    levNoise = 0.125 * potAmp + 0.875 * levNoise;
                    levNoiseI = 0.125 * integratedSignal[r] + 0.875 * levNoiseI;
                }

                //adjust thresholds with SNR
                if (levNoise != 0 || levSig != 0)
                {
                    thrSig = levNoise + 0.25 * Math.Abs(levSig - levNoise);
                    thrNoise = 0.5 * thrSig;
                }

                //reset param
                skip = false;
                serback = false;
            }
            return Vector<double>.Build.DenseOfArray(locsR.ToArray());
        }

        #region
        /// <summary>
        /// Function that calculates Hilbert Transform of filtered ECG signal
        /// </summary>
        /// <param name="filteredSignal"> Filtered ECG signal</param>
        /// <returns> Hilbert Transform of ECG as a double array</returns>
        #endregion
        public double[] HilbertTransform(double[] filteredSignal)
        {
            double pi = Math.PI;
            IList<double> d = new List<double>();
            d.Add(1 / (pi * filteredSignal.Length));

            OnlineFirFilter transformFilter = new OnlineFirFilter(d);
            double[] hSignal = transformFilter.ProcessSamples(filteredSignal);
            double[] htSignal = new double[hSignal.Length];
            for (int i = 0; i < htSignal.Length; i++)
            {
                htSignal[i] = Math.Abs(filteredSignal[i]) + Math.Abs(hSignal[i]);
            }
            return htSignal;
        }

        #region
        /// <summary>
        /// Function that integrates the signal in moving-window (width equals ??? ms)
        /// </summary>
        /// <param name="htSignal"> signal returned by HilbertTransform method</param>
        /// <param name="fs"> sampling frequency of anlysed signal</param>
        /// <returns> integration of signal as a double array </returns>
        #endregion
        public double[] Integration(double[] htSignal, uint fs)
        {
            double[] int1Signal = new double[htSignal.Length];
            double window = Math.Round(0.37 * fs);
            Delay += Convert.ToUInt32(Math.Round(window / 2));
            IList<double> hi_coeff = new List<double>();
            for (int i = 0; i < window; i++)
            {
                hi_coeff.Add((1 / window) + 1);
            }
            OnlineFirFilter integrationFilter = new OnlineFirFilter(hi_coeff);
            int1Signal = integrationFilter.ProcessSamples(htSignal);
            Vector<double> tempSignal = Vector<double>.Build.DenseOfArray(int1Signal);

            // correcting signal length
            Vector<double> int2Signal = CutSignal(tempSignal, Convert.ToInt16(Math.Round(window / 2)), htSignal.Length - 1);
            int sigLength = htSignal.Length - Convert.ToInt16(Math.Round(window / 2));

            // normalization
            double tempMax = int2Signal.Maximum();
            double tempMin = int2Signal.Minimum();
            double normCoeff = (Math.Abs(tempMin) > tempMax) ? Math.Abs(tempMin) : tempMax;
            double[] integratedSignal = new double[sigLength];
            for (int i = 0; i < sigLength; i++)
            {
                integratedSignal[i] = int2Signal[i] / normCoeff;
            }
            return integratedSignal;
        }

        #region
        /// <summary>
        /// Function which locates R peaks in ECG signal by thersholding and finding local maxima
        /// </summary>
        /// <param name="integratedSignal"> integrated signal of ECG</param>
        /// <param name="filteredSignal"> filtereg signal of ECG</param>
        /// <returns> Numbers of samples of R peaks in ECG signal as int array </returns>
        #endregion
        double[] FindPeak(double[] integratedSignal, Vector<double> filteredSignal)
        {
            // finding threshold
            double tempMax = integratedSignal.Max();
            double threshold = integratedSignal.Average() * tempMax;

            // thresholding
            double[] thresResult = new double[integratedSignal.Length];
            for (int i = 0; i < integratedSignal.Length; i++)
            {
                thresResult[i] = (integratedSignal[i] >= threshold) ? 1 : 0;
            }

            // determining sections limits which are above the threshold
            IList<int> leftLimit = new List<int>();
            IList<int> rightLimit = new List<int>();
            for (int i = 1; i < thresResult.Length; i++)
            {
                if (thresResult[i] - thresResult[i - 1] == 1)
                {
                    leftLimit.Add(i);
                }
                else if (thresResult[i] - thresResult[i - 1] == -1)
                {
                    rightLimit.Add(i);
                }
            }

            // locating R peaks as local extremes in between sections limits
            List<double> locsR = new List<double>();
            int rCount;
            if (leftLimit.Count == rightLimit.Count)
            {
                rCount = leftLimit.Count;
            }
            else
            {
                rCount = (leftLimit.Count < rightLimit.Count) ? leftLimit.Count : rightLimit.Count;
            }
            for (int i = 0; i < rCount; i++)
            {
                int tempLength = rightLimit[i] - leftLimit[i] + 1;
                double[] tempRRange = new double[tempLength];
                for (int j = 0; j < tempLength; j++)
                {
                    tempRRange[j] = leftLimit[i] + j;
                }
                double[] tempV = new double[tempLength];
                for (int j = 0; j < tempLength; j++)
                {
                    tempV[j] = filteredSignal[Convert.ToInt32(tempRRange[j])];
                }
                Vector<double> tempI = Vector<double>.Build.DenseOfArray(tempV);
                double tempIndex = tempI.MaximumIndex();
                locsR.Add(tempIndex + leftLimit[i] - 10);
            }
            
            return locsR.ToArray();
        }

        #region
        /// <summary>
        /// Implemented algorithm Pan-Tompkins for detecting R Peaks in ECG signal 
        /// </summary>
        /// <param name="signalECG"> Vector of double that contain raw or filtered values of the ECG Signal </param>
        /// <param name="samplingFrequency"> sampling frequency of aquiring the signal </param>
        /// <returns> numbers of indexes where are located R peaks as vector </returns>
        #endregion
        public Vector<double> PanTompkins(Vector<double> signalECG, uint samplingFrequency)
        {
            //Init
            double fd = 5;
            double fg = 15;
            Delay = 0;

            //PROCESS
            //filtering
            double[] arr_f = Filtering(Convert.ToDouble(samplingFrequency), fd, fg, signalECG.ToArray());   //plus convert vector to array

            //differentiation
            double[] arr_d = Derivative(arr_f);

            //squaring
            double[] arr_2 = Squaring(arr_d);

            //moving-window integration
            double[] arr_i = Integrating(arr_2, samplingFrequency);

            //adaptive thresholding 
            Vector<double> locsR = findRs(arr_i, signalECG, samplingFrequency);

            return locsR;
        }

        #region
        /// <summary>
        /// Implemented algorithm using Hilbert Transform for detecting R Peaks in ECG signal 
        /// </summary>
        /// <param name="signalECG"> Vector of double that contain raw or filtered values of the ECG Signal </param>
        /// <param name="samplingFrequency"> sampling frequency of aquiring the signal </param>
        /// <returns> numbers of indexes where are located R peaks as vector </returns>
        #endregion
        public Vector<double> Hilbert(Vector<double> signalECG, uint samplingFrequency)
        {
            //Init
            double fd = 5;
            double fg = 15;
            Delay = 0;

            //PROCESS
            //filtering
            double[] h_arr_f = Filtering(Convert.ToDouble(samplingFrequency), fd, fg, signalECG.ToArray());   //plus convert vector to array
            Vector<double> h_sig_f = Vector<double>.Build.DenseOfArray(h_arr_f);

            //Hilbert Transform 
            double[] h_arr_ht = HilbertTransform(h_arr_f);

            //moving-window integration
            double[] h_arr_i = Integration(h_arr_ht, samplingFrequency);

            //adaptive thresholding 
            double[] loc_R = FindPeak(h_arr_i, h_sig_f);
            Vector<double> locsR = Vector<double>.Build.DenseOfArray(loc_R);

            return locsR;
        }

        #region
        /// <summary>
        /// Function that filters the signal by lowpass FIR filter (cutoff frequency equals 2Hz, 3rd order)
        /// </summary>
        /// <param name="signal"> Raw signal which is going to be filtered as double array</param>
        /// <param name="samplingFrequency"> Sampling frequency of the signal </param>
        /// <returns> Filtered signal as double array </returns>
        #endregion
        public double[] LPFiltering(double[] signal, uint samplingFrequency)
        {
            IList<double> coef = new List<double>();
            double samplingFreq = Convert.ToDouble(samplingFrequency);
            double[] hf = FirCoefficients.LowPass(samplingFreq, 2 / (samplingFreq / 2), 1);
            foreach (double number in hf)
            {
                coef.Add(number);
            }

            OnlineFirFilter filter = new OnlineFirFilter(coef);
            double[] signal_f = filter.ProcessSamples(signal);
            Delay += 2;
            return signal_f;
        }
    }
}
