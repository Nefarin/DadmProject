using System;
using System.Collections.Generic;
using MathNet.Filtering.FIR;
using MathNet.Filtering.IIR;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;
using MathNet.Numerics.Interpolation;
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
    public class R_Peaks_Alg
    {
        /*static void Main(string[] args)
        {
            #region readData            
            //read data from dat file
            TempInput.setInputFilePath(@"D:\biomed\DADM\C#\100v5.txt");
            uint fs = TempInput.getFrequency();
            Vector<double> signal = TempInput.getSignal();
            #endregion
            double[] testArray = { -0.03, -0.015, -0.02, -0.005, -0.005, -0.01, -0.015, -0.025, -0.015, -0.015, -0.015, -0.025, -0.025, -0.025, -0.005, 0.005, -0.01, -0.01, -0.015, -0.035, -0.03, -0.02, -0.02, -0.03, -0.025, -0.03, -0.015, 0.005, 0.005, 0.01, 0, 0.005, 0.02, 0.03, 0.02, 0.025, 0.005, 0.005, 0.03, 0.025, 0.025, 0.02, 0.015, 0.015, 0.025, 0.025, 0.025, 0.01, -0.005, 0, -0.005, -0.005, -0.015, -0.015, -0.025, -0.015, -0.01, 0, -0.015, -0.025, -0.045, -0.045, -0.05, -0.025, -0.03, -0.035, -0.04, -0.035, -0.035, -0.02, -0.03, -0.04, -0.04, -0.045, -0.025, -0.025, -0.035, -0.05, -0.07, -0.08, -0.09, -0.1, -0.13, -0.16, -0.155, -0.105, -0.005, 0.12, 0.215, 0.265, 0.315, 0.39, 0.5, 0.61, 0.695, 0.685, 0.56, 0.305, 0.035, -0.15, -0.25, -0.27, -0.25, -0.195, -0.145, -0.11, -0.095, -0.085, -0.095, -0.095, -0.08, -0.085, -0.085, -0.085, -0.09, -0.075, -0.075, -0.075, -0.07, -0.085, -0.095, -0.09, -0.075, -0.065, -0.065, -0.075, -0.095, -0.09, -0.085, -0.07, -0.065, -0.075, -0.08, -0.08, -0.07, -0.065, -0.07, -0.07, -0.08, -0.07, -0.065, -0.065, -0.065, -0.07, -0.085, -0.085, -0.065, -0.065, -0.055, -0.075, -0.075, -0.08, -0.07, -0.065, -0.075, -0.085, -0.08, -0.08, -0.075, -0.075, -0.075, -0.085, -0.095, -0.095, -0.075, -0.075, -0.085, -0.09, -0.115, -0.115, -0.105, -0.09, -0.11, -0.12, -0.12, -0.135, -0.13, -0.125, -0.145, -0.15, -0.165, -0.17, -0.155, -0.16, -0.17, -0.19, -0.195, -0.2, -0.19, -0.185, -0.19, -0.2, -0.21, -0.22, -0.205, -0.21, -0.21, -0.2, -0.205, -0.19, -0.185, -0.165, -0.155, -0.145, -0.155, -0.13, -0.095, -0.075, -0.07, -0.06, -0.065, -0.065, -0.045, -0.025, -0.01, -0.015, -0.015, -0.005, 0.005, 0.015, 0.015, 0.005, 0.005, 0.005, 0.015, 0.03, 0.015, 0.005, 0.005, 0.005, 0.015, 0.025, 0.025, 0.015, 0.005, 0.005, 0.005, 0.02, 0.015, 0.01, 0.005, 0.005, 0.01, 0.015, 0.015, 0.01, 0, -0.015, 0.005, 0.015, 0.015, 0.015, 0, -0.005, 0.01, 0.01, 0, 0, 0.005, -0.02, -0.005, 0.01, 0, -0.02, -0.03, -0.01, -0.005, 0, 0.005, -0.02, -0.025, -0.025, -0.015, -0.005, -0.005, -0.015, -0.025, -0.02, -0.025, 0.005, -0.015, -0.02, -0.015, -0.025, -0.015, -0.01, -0.01, -0.025, -0.03, -0.02, -0.01, -0.005, 0, -0.005, -0.02, -0.03, -0.015, -0.005, 0, -0.03, -0.02, -0.015, -0.005, 0, -0.005, -0.02, -0.02, -0.025, -0.005, 0, 0, -0.01, -0.03, -0.015, 0, 0, -0.005, -0.01, -0.015, -0.02, -0.005, 0, 0.005, -0.005, -0.005, -0.005, 0.02, 0.03, 0.01, 0.01, 0, 0.015, 0.035, 0.055, 0.055, 0.03, 0.025, 0.03, 0.035, 0.05, 0.04, 0.025, 0.01, 0.01, 0.015, 0.015, 0.025, 0.005, -0.015, -0.005, 0, 0.005, -0.005, -0.01, -0.02, -0.015, -0.01, -0.005, 0, -0.01, -0.02, -0.02, -0.02, 0, -0.005, -0.015, -0.025, -0.02, -0.015, -0.005, -0.02, -0.025, -0.04, -0.06, -0.08, -0.1, -0.145, -0.175, -0.145, -0.06, 0.075, 0.225, 0.345, 0.46, 0.575, 0.705, 0.835, 0.905, 0.81, 0.52, 0.155, -0.05, -0.07, -0.04, -0.04, -0.05, -0.055, -0.045, -0.045, -0.045, -0.055, -0.06, -0.06, -0.07, -0.045, -0.045, -0.05, -0.065, -0.07, -0.07, -0.065, -0.06, -0.06, -0.07, -0.08, -0.08, -0.07, -0.055, -0.065, -0.08, -0.09, -0.075, -0.065, -0.055, -0.06, -0.06, -0.08, -0.08, -0.065, -0.06, -0.055, -0.06, -0.075, -0.07, -0.075, -0.065, -0.065, -0.07, -0.08, -0.085, -0.08, -0.055, -0.07, -0.08, -0.09, -0.09, -0.07, -0.07, -0.085, -0.1, -0.1, -0.1, -0.095, -0.075, -0.095, -0.1, -0.11, -0.11, -0.115, -0.11, -0.11, -0.135, -0.145, -0.155, -0.15, -0.145, -0.16, -0.17, -0.175, -0.185, -0.17, -0.17, -0.185, -0.2, -0.215, -0.205, -0.21, -0.19, -0.205, -0.21, -0.225, -0.22, -0.21, -0.205, -0.2, -0.2, -0.205, -0.185, -0.165, -0.145, -0.135, -0.125, -0.11, -0.095, -0.075, -0.055, -0.05, -0.04, -0.045, -0.03, -0.005, 0, 0, -0.005, -0.01, -0.02, 0.005, 0.01, 0, 0, -0.01, 0, 0.01, 0.025, 0, -0.005, -0.01, 0, 0, 0.02, 0.01, 0.01, -0.005, -0.005, 0.005, 0.015, 0.01, 0.01, -0.01, -0.01, -0.005, -0.005, 0, -0.015, -0.025, -0.01, -0.01, -0.01, -0.015, -0.025, -0.025, -0.03, -0.01, -0.01, -0.03, -0.05, -0.05, -0.045, -0.03, -0.015, -0.03, -0.04, -0.045, -0.045, -0.04, -0.015, -0.025, -0.04, -0.04, -0.045, -0.04, -0.03, -0.035, -0.04, -0.03, -0.05, -0.025, -0.015};
            Vector<double> sig = Vector<double>.Build.DenseOfArray(testArray);

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> l = test.EMD(sig, 360);
           #region writeData
            //write result to dat file
            //TempInput.setOutputFilePath(@"D:\biomed\DADM\C#\baserr.txt");
            //TempInput.writeFile(fs, RRms);
            
            #endregion

            //TEST-Console
            Console.WriteLine("done");
            foreach (double sth in l) { Console.WriteLine(sth ); }
            //foreach (double sth in testResult.Item2) { Console.WriteLine(sth + "xx"); }
            Console.ReadKey();
        }*/

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
            if (rawSignal == null) throw new ArgumentNullException();
            if (samplingFreq <= 0 || lowCutOff < 0 || highCutOff <=0) throw new ArgumentOutOfRangeException();
            if (rawSignal.Length == 0) throw new ArgumentOutOfRangeException("Array is empty.");
            //TO DO: add cutoffs as constants not param? NOPE!
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
            if (filteredSignal == null) throw new ArgumentNullException();
            if (filteredSignal.Length == 0) throw new ArgumentOutOfRangeException("Array is empty.");
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
            if (derivativeSignal == null) throw new ArgumentNullException();
            if (derivativeSignal.Length == 0) throw new ArgumentOutOfRangeException("Array is empty.");
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
            if (squaredSignal == null) throw new ArgumentNullException();
            if (fs == 0) throw new ArgumentException();
            if (squaredSignal.Length == 0) throw new ArgumentOutOfRangeException("Array is empty.");
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

            //enhancing by put zero below threshold (0.05 of maximum value)
            double minIS = integratedSignal.Min();
            double maxIS = integratedSignal.Max();
            for (int i = 0; i < integratedSignal.Length; i++)
            {
                double normIntSig = (integratedSignal[i] - minIS) / (maxIS - minIS);
                if (normIntSig < 0.05)
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
        {
            if (signal == null) throw new ArgumentNullException();
            if (fs == 0) throw new ArgumentException();
            if (signal.Length == 0) throw new ArgumentOutOfRangeException("Array is empty.");
            List<double> potRs = new List<double>();
            double distanceInSamples = fs * distanceInSec;
            for (int i = 1; i < signal.Length - 1; i++)
            {
                if ((signal[i] > signal[i - 1]) && (signal[i] > signal[i + 1]))
                {
                    potRs.Add(i);
                }
            }
            if (potRs.Count > 0)
            {
                int j = 1;                          //remove maximas which are closer to previous maximum than distance
                double prevR = potRs.First();
                while (j < potRs.Count)
                {
                    if (potRs[j] - prevR < distanceInSamples)
                    {
                        if (signal[Convert.ToInt32(potRs[j])] <= signal[Convert.ToInt32(prevR)])
                            potRs.RemoveAt(j);
                        else {
                            prevR = potRs[j];
                            potRs.RemoveAt(j - 1);
                        }
                    }
                    else
                    {
                        prevR = potRs[j];
                        j++;
                    }
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
            if (inputSignal == null) throw new ArgumentNullException();
            if (end <= begin) throw new ArgumentException("End is lower than begin");
            if (inputSignal.Count == 0) throw new ArgumentOutOfRangeException("Array is empty.");
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
            if (signal == null) throw new ArgumentNullException();
            if (signal.Count < 1) throw new ArgumentOutOfRangeException("Vector must be at least 2 long.");
            if (signal.Count == 0) throw new ArgumentOutOfRangeException("Array is empty.");
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
            if (integratedSignal == null || filteredSignal == null) throw new ArgumentNullException();
            if (integratedSignal.Length == 0 || filteredSignal.Count == 0) throw new ArgumentOutOfRangeException("Array is empty.");
            if (fs == 0) throw new ArgumentException();

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

            //init thresholds 
            int assumedLength = Convert.ToInt32(2 * fs); //2 seconds of signal
            if (integratedSignalV.Count < Convert.ToInt32(2 * fs))
            {
                assumedLength = integratedSignalV.Count - 1;
            }
            //init thresholds for integrated signal
            Vector<double> sig_ic = CutSignal(integratedSignalV, 0, assumedLength);
            double thrSigI = sig_ic.Maximum() / 3;
            double thrNoiseI = sig_ic.Average() / 2;
            double levSigI = thrSigI;
            double levNoiseI = thrNoiseI;
            //init thersholds for filtered signal
            Vector<double> sig_fc = CutSignal(filteredSignal, 0, assumedLength);
            double thrSig = sig_fc.Maximum() / 3;
            double thrNoise = sig_fc.Average() / 2;
            double levSig = thrSig;
            double levNoise = thrNoise;

            //detecting peaks in both signals(integrated and filtered)
            List<double> potRsI = FindPeaks(integratedSignal, fs, 0.2);
            foreach (int r in potRsI)
            {
                //detect peaks in filetred signal
                int window = Convert.ToInt32(0.15 * fs);
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
                    double lastRR = locsRi.Last() - locsRi[locsRi.Count - 2];
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
                        int buff = Convert.ToInt32(0.2 * fs);
                        int beg = Convert.ToInt32(locsRi.Last()) + buff;
                        int en = Convert.ToInt32(r - buff);
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
                                Vector<double> tempSig = CutSignal(filteredSignal, Convert.ToInt32(tempInd) - window, Convert.ToInt32(tempInd));
                                tempPotR = tempSig.MaximumIndex();
                                tempAmpR = tempSig.Maximum();
                            }
                            else
                            {
                                Vector<double> tempSig = CutSignal(filteredSignal, Convert.ToInt32(tempInd) - window, filteredSignal.Count);
                                tempPotR = tempSig.MaximumIndex();
                                tempAmpR = tempSig.Maximum();
                            }

                            if (tempAmpR > thrNoise)
                            {
                                locsR.Add(tempPotR + Convert.ToInt32(tempInd) - window);
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
                            Vector<double> tempSig1 = CutSignal(filteredSignal, r - Convert.ToInt32(0.075 * fs), r);
                            double slope1 = Math.Abs(Diff(tempSig1).Mean());
                            Vector<double> tempSig2 = CutSignal(filteredSignal, Convert.ToInt32(locsRi.Last() - Math.Round(0.075 * fs)), Convert.ToInt32(locsRi.Last()));
                            double slope2 = Math.Abs(Diff(tempSig2).Mean());
                            if (slope1 <= 0.5 * slope2)
                            {
                                skip = true;
                                levNoise = 0.125 * potAmp + 0.875 * levNoise;
                                levNoiseI = 0.125 * integratedSignal[r] + 0.875 * levNoiseI;
                            }
                            else { skip = false; }
                            if (r == locsRi.Last()) { skip = true; }
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
                if (levNoiseI != 0 || levSigI != 0)
                {
                    thrSigI = levNoiseI + 0.25 * Math.Abs(levSigI - levNoiseI);
                    thrNoiseI = 0.5 * thrSigI;
                }

                //reset param
                skip = false;
                serback = false;
            }

            if (locsR.Count == 0)
            {
                throw new Exception("No Rs found");
            }
            return Vector<double>.Build.DenseOfEnumerable(locsR);
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
            if (filteredSignal == null) throw new ArgumentNullException();
            if (filteredSignal.Length == 0) throw new ArgumentOutOfRangeException("Array is empty.");
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
            if (htSignal == null) throw new ArgumentNullException();
            if (fs == 0) throw new ArgumentException();
            if (htSignal.Length == 0) throw new ArgumentOutOfRangeException("Array is empty.");
            double[] int1Signal = new double[htSignal.Length];
            double window = Math.Round(0.36 * fs);
            Delay += Convert.ToUInt32(Math.Round(window / 2));
            double[] delayCompensation = new double[Delay];
            for (int i = 0; i < Delay; i++)
            {
                delayCompensation[i] = 0;
            }
            double[] temp0Signal = new double[htSignal.Length + Delay];
            htSignal.CopyTo(temp0Signal, 0);
            delayCompensation.CopyTo(temp0Signal, htSignal.Length);

            IList<double> hi_coeff = new List<double>();
            for (int i = 0; i < (window+1); i++)
            {
                hi_coeff.Add((1 / window) + 1);
            }
            OnlineFirFilter integrationFilter = new OnlineFirFilter(hi_coeff);
            int1Signal = integrationFilter.ProcessSamples(temp0Signal);
            Vector<double> tempSignal = Vector<double>.Build.DenseOfArray(int1Signal);
            
            // correcting signal length
            Vector<double> int2Signal = CutSignal(tempSignal, Convert.ToInt32(Delay), int1Signal.Length - 1);
            int sigLength = int2Signal.Count;

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
        /// <param name="signal"> ECG signal</param>
        /// <returns> Numbers of samples of R peaks in ECG signal as int array </returns>
        #endregion
        public double[] FindPeak(double[] integratedSignal, Vector<double> signal)
        {
            if (integratedSignal == null) throw new ArgumentNullException();
            if (integratedSignal.Length == 0 || signal.Count == 0) throw new ArgumentOutOfRangeException("Array is empty.");
            if (signal == null) throw new NullReferenceException();
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
                    tempV[j] = signal[Convert.ToInt32(tempRRange[j])];
                }
                Vector<double> tempI = Vector<double>.Build.DenseOfArray(tempV);
                double tempIndex = tempI.MaximumIndex();
                double tempMin = tempI.Minimum();
                tempMin = (tempMin<0) ? tempMin*=-1 : tempMin;
                if (signal[Convert.ToInt32(tempIndex + leftLimit[i])] < tempI.Minimum())
                {
                    tempIndex = tempI.MinimumIndex();
                }
                locsR.Add(tempIndex + leftLimit[i]);
            }

            return locsR.ToArray();
        }

        #region
        /// <summary>
        /// Function that returns indexes for values in signal which fulfil some conditions (defined by predicate)
        /// </summary>
        /// <param name="signal"> Vector of signal in which value is searched </param>
        /// <param name="searchValue"> Functions which defines searching parameters </param>
        /// <returns> Vector of searched indexes </returns>
        #endregion
        public Vector<double> FindIndexes(Vector<double> signal, Func<double, bool> predicate)
        {
            if (signal == null || predicate == null) throw new ArgumentNullException();
            Vector<double> indexes = Vector<double>.Build.Dense(signal.Count);
            int i = 0;
            int lastInd = 0;
            while (lastInd < signal.Count)
            {
                Vector<double> partSig = signal.SubVector(lastInd, signal.Count - lastInd);
                Tuple<int, double> findItem = partSig.Find(predicate);
                if (findItem != null)
                {
                    lastInd = findItem.Item1 + lastInd;
                    indexes[i] = Convert.ToDouble(lastInd);
                    i++;
                }
                else { break; }
                lastInd++;
            }

            return i != 0 ? indexes.SubVector(0, i) : null;
        }

        #region
        /// <summary>
        /// Function that finds extrema in signal
        /// </summary>
        /// <param name="signal"> Analyzed signal as vector </param>
        /// <returns>Extrema as Tuple of two lists: Item1 is Minima, Item2 is Maxima </returns>
        #endregion
        public Tuple<List<double>, List<double>> Extrema(Vector<double> signal)
        {
            if (signal == null) throw new ArgumentNullException();
            List<double> iMax = new List<double>();
            List<double> iMin = new List<double>();
            Vector<double> diffSig = Diff(signal);
            Vector<double> dSig1 = diffSig.SubVector(0, diffSig.Count - 1);
            Vector<double> dSig2 = diffSig.SubVector(1, diffSig.Count - 1);
            for (int i = 0; i < dSig1.Count; i++)
            {
                if (dSig1[i] * dSig2[i] >= 0)
                {
                    dSig1[i] = 0;
                }
            }
            Vector<double> indMin = FindIndexes(dSig1, item => item < 0);
            Vector<double> indMax = FindIndexes(dSig1, item => item > 0);
            if (indMin != null) { indMin.Add(1, indMin); }
            if (indMax != null) { indMax.Add(1, indMax); }

            if (diffSig.Exists(item => item == 0))
            {
                Vector<double> bad = Vector<double>.Build.Dense(diffSig.Count + 2);
                for (int i = 0; i < diffSig.Count; i++)
                {
                    if (diffSig[i] == 0)
                    {
                        bad[i + 1] = 1;
                    }
                }
                Vector<double> diffBad = Diff(bad);
                Vector<double> debS = FindIndexes(diffBad, item => item == 1);
                Vector<double> finS = FindIndexes(diffBad, item => item == -1);
                if (debS[0] == 0)
                {
                    if (debS.Count > 1)
                    {
                        debS = debS.SubVector(1, debS.Count - 1);
                        finS = finS.SubVector(1, finS.Count - 1);
                    }
                    else
                    {
                        debS = null;
                        finS = null;
                    }
                }
                if (debS != null)
                {
                    if (finS.Last() == signal.Count-1)
                    {
                        if (debS.Count > 1)
                        {
                            debS = debS.SubVector(0, debS.Count - 1);
                            finS = finS.SubVector(0, finS.Count - 1);
                        }
                        else
                        {
                            debS = null;
                            finS = null;
                        }
                    }
                }
                if (debS != null)
                {
                    for (int i = 0; i < debS.Count; i++)
                    {
                        if (diffSig[Convert.ToInt32(debS[i]) - 1] > 0)
                        {
                            if (diffSig[Convert.ToInt32(finS[i])] < 0)
                            {
                                iMax.Add(Math.Round((finS[i] + debS[i]) / 2));
                            }
                        }
                        else
                        {
                            if (diffSig[Convert.ToInt32(finS[i])] > 0)
                            {
                                iMin.Add(Math.Round((finS[i] + debS[i]) / 2));
                            }
                        }
                    }
                }
            }
            if (indMax != null)
            {
                iMax.AddRange(indMax);
                iMax.Sort();
            }
            if (indMin != null)
            {
                iMin.AddRange(indMin);
                iMin.Sort();
            }
            Tuple<List<double>, List<double>> extrema = new Tuple<List<double>, List<double>>(iMin, iMax);
            return extrema;
        }

        #region
        /// <summary>
        /// Function that interpolate the signal to the given (x, y) points 
        /// </summary>
        /// <param name="signalLength"> Legnth of the interpolated signal </param>
        /// <param name="x"> Values of x of points </param>
        /// <param name="y"> Values of y of points</param>
        /// <returns> Signal consists of interpolated values for values of x form 0 to signalLength step 1 </returns>
        #endregion
        public Vector<double> CubicSplineInterp(int signalLength, IEnumerable<double> x, IEnumerable<double> y)
        {
            //TO DO: ROUND???       
            if (x==null || y==null) throw new ArgumentNullException();
            //result vector
            Vector<double> interpSpl = Vector<double>.Build.Dense(signalLength);
            //cubic spline interpolation
            CubicSpline splineCoeff = CubicSpline.InterpolateNatural(x, y);
            for (double c = 0; c < signalLength; c++)
            {
                interpSpl[Convert.ToInt32(c)] = splineCoeff.Interpolate(c);
            }
            return interpSpl;
        }

        #region
        /// <summary>
        /// Function which extract from signal its first intrinsic mode function
        /// </summary>
        /// <param name="signal"> Signal for decomposition</param>
        /// <returns> First Intrinsic Mode Function of the given signal </returns>
#endregion
        public Vector<double> ExtractModeFun(Vector<double> signal)
        {
            if (signal==null) throw new ArgumentNullException();
            int numOfSift = 20;
            Vector<double> d = signal;
            for (int i = 0; i < numOfSift; i++)
            {
                //find extrema
                Tuple<List<double>, List<double>> extr = Extrema(d);
                List<double> iMin = extr.Item1;
                List<double> iMax = extr.Item2;
                if (iMin.Count > 0)
                {
                    Vector<double> ampMin = Vector<double>.Build.Dense(iMin.Count);
                    Vector<double> ampMax = Vector<double>.Build.Dense(iMax.Count);
                    for (int j = 0; j < iMin.Count; j++)
                    {
                        ampMin[j] = d[Convert.ToInt32(iMin[j])];
                    }
                    for (int j = 0; j < iMax.Count; j++)
                    {
                        ampMax[j] = d[Convert.ToInt32(iMax[j])];
                    }
                    //envelopes
                    try
                    {
                    Vector<double> envMin = CubicSplineInterp(signal.Count, iMin, ampMin);
                    Vector<double> envMax = CubicSplineInterp(signal.Count, iMax, ampMax);
                    Vector<double> envMean = envMin.Add(envMax).Divide(2);
                    //substract form signal
                    d = d - envMean;
                }
                    catch(ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                        break;
                    }
                }
                else break;
            }
            return d;
        }

        #region
        /// <summary>
        /// Function which decomposes the signal into 3 first Intrinsic Mode Functions (using EMD)
        /// </summary>
        /// <param name="signal"> Decomposed signal as vector of samples</param>
        /// <returns> An array of vectors in which each vector is next Intrinsic Mode Function of signal </returns>
#endregion
        public Vector<double>[] EmpiricalModeDecomposition(Vector<double> signal)
        {
            if (signal == null) throw new ArgumentNullException();
            int numOfImfs = 2;
            Vector<double>[] imfs = new Vector<double>[numOfImfs];
            Vector<double> res = Vector<double>.Build.DenseOfVector(signal);
            for (int i = 0; i < numOfImfs; i++)
            {
                imfs[i] = ExtractModeFun(res);
                res = res - imfs[i];
            }
            return imfs;
        }

        #region
        /// <summary>
        /// Function that makes operations on Intrinsic Mode Functions from Empirical Mode Decomposition. This function povides nonlinear transform and integration imfs and as result return sum of imfs. 
        /// </summary>
        /// <param name="imfs"> Intrinsic Mode Functions as array of vectors</param>
        /// <param name="fs"> sampling frequency of signal of imf</param>
        /// <returns> Vector consists sum of transformed imfs </returns>
        #endregion
        public Vector<double> TransformImf(Vector<double>[] imfs, uint fs)
        {
            if (imfs == null) throw new ArgumentNullException();
            if (fs == 0) throw new ArgumentException();
            foreach(var imf in imfs) { if (imf.Count != imfs[0].Count) throw new ArgumentOutOfRangeException("Lenghts of vectors in array must be tha same"); }
            //result Vector
            Vector<double> imfSum = Vector<double>.Build.Dense(imfs[0].Count - 2);
            //integrating window
            double window = Math.Round(0.1 * fs);
            Delay += Convert.ToUInt32(window / 2);

            foreach (Vector<double> imf in imfs)
            {
                //nonlinear transform
                Vector<double> imfTransformed = Vector<double>.Build.Dense(imf.Count);
                for (int i = 2; i < imfs[0].Count; i++)
                {
                    if ((imf[i] * imf[i - 1] > 0) && (imf[i - 2] * imf[i] > 0))
                    {
                        imfTransformed[i] = Math.Abs(imf[i] * imf[i - 1] * imf[i - 2]);
                    }
                    else
                    {
                        imfTransformed[i] = 0;
                    }
                }
                imfTransformed = imfTransformed.SubVector(2, imf.Count - 2);

                //integrating
                IList<double> hi_coeff = new List<double>();
                for (int i = 0; i < window; i++)
                {
                    hi_coeff.Add(1 / window);
                }
                OnlineFirFilter integrationFilter = new OnlineFirFilter(hi_coeff);
                double[] imfIntegrated = integrationFilter.ProcessSamples(imfTransformed.ToArray());

                //sum
                imfSum = imfSum.Add(Vector<double>.Build.DenseOfArray(imfIntegrated));
            }
            return imfSum;
        }


        #region
        /// <summary>
        /// Function that filters the signal by lowpass IIR filter (cutoff frequency equals 2Hz, 1st order)
        /// </summary>
        /// <param name="signal"> Raw signal which is going to be filtered as double array</param>
        /// <param name="samplingFrequency"> Sampling frequency of the signal </param>
        /// <returns> Filtered signal as double array </returns>
        #endregion
        public double[] LPFiltering(double[] signal, uint samplingFrequency)
        {
            if (signal == null) throw new ArgumentNullException();
            if (samplingFrequency == 0) throw new ArgumentException();
            if (signal.Length == 0) throw new ArgumentOutOfRangeException("Array is empty.");
            double[] hf = new double[] { 0.0172, 0.0172, 1, -0.9657 };
            OnlineIirFilter filter = new OnlineIirFilter(hf);
            double[] signal_f = filter.ProcessSamples(signal);
            Delay += 2;
            return signal_f;
        }

        #region
        /// <summary>
        /// Function that locates the peaks in signal which are higher than threshold (0.00005)
        /// </summary>
        /// <param name="signal"> Signal in which peaks should be located</param>
        /// <returns> List of indexes of localosations of peaks in signal</returns>
        #endregion
        public List<double> FindPeaksTh(double[] signal)
        {
            if (signal == null) throw new ArgumentNullException();
            List<double> potRs = new List<double>();
            double minS = signal.Min();
            double maxS = signal.Max();   
            double th = 0.0005;
            for (int i = 1; i < signal.Length - 1; i++)
            {
                double normSignal = (signal[i] - minS) / (maxS - minS);
                if ((signal[i] > signal[i - 1]) && (signal[i] > signal[i + 1]) && normSignal > th)
                {
                    potRs.Add(i);
                }
            }
            return potRs;
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
            if (signalECG == null) throw new ArgumentNullException();
            if (samplingFrequency == 0) throw new ArgumentException();
            if (signalECG.Count == 0) throw new ArgumentOutOfRangeException("Array is empty.");
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
            if (signalECG == null) throw new ArgumentNullException();
            if (samplingFrequency == 0) throw new ArgumentException();
            if (signalECG.Count == 0) throw new ArgumentOutOfRangeException("Array is empty.");
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
            double[] loc_R = FindPeak(h_arr_i, signalECG);
            Vector<double> locsR = Vector<double>.Build.DenseOfArray(loc_R);

            return locsR;
        }


        public Vector<double> EMD(Vector<double> signalECG, uint samplingFrequency)
        {
            Delay = 0;
            //emd
            Vector<double>[] imfs = EmpiricalModeDecomposition(signalECG);
            //non linear tranform imfs
            Vector<double> imfSum = TransformImf(imfs, samplingFrequency);
            //filtering
            double[] filtSum = LPFiltering(imfSum.ToArray(), samplingFrequency);
            //finding peaks
            List<double> potRs = FindPeaksTh(filtSum);
            if (potRs.Count == 0) return null;
            else
            {
                Vector<double> locsR = Vector<double>.Build.DenseOfEnumerable(potRs);
                //subtract delay
                locsR = locsR.Subtract(Delay);
                return locsR;
            }
        }

        #region
        /// <summary>
        /// Function that calculates RR intervals
        /// </summary>
        /// <param name="locsR"> Vector of double that contains detected R peaks indexes</param>
        /// <param name="samplingFrequency"> Sampling frequency of the signal</param>
        /// <returns></returns>
        #endregion
        public Vector<double> RRinMS(Vector<double> locsR, uint samplingFrequency)
        {
            if (locsR == null) throw new ArgumentNullException();
            if (samplingFrequency <= 0) throw new ArgumentException();
            if (locsR.Count == 0) throw new ArgumentOutOfRangeException("Array is empty.");
            Vector<double> RRms = Diff(locsR);
            RRms.Multiply(Math.Round(1000 / Convert.ToDouble(samplingFrequency), 3), RRms);
            return RRms;
        }
    }
}
