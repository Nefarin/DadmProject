using System;
using System.Collections.Generic;
using MathNet.Filtering.FIR;
using MathNet.Numerics.LinearAlgebra;
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
        //TO DO: documentation!

        static void Main(string[] args)
        {
            //Init
            double fd = 5;
            double fg = 15;
            List<Tuple<string, Vector<double>>> R_Peaks = new List<Tuple<string, Vector<double>>>();

            //read data from ecg_baseline (TO DO!)
            // Vector<double> sigs = ECG_Baseline_Data.SignalFiltered ;
            //Tuple<string, Vector <double>> sig_data = sigs.Item[0];
            //  string channel = sig_data.Item1;
            // Vector<double> sig = sig_data.Item2;

            //read data from dat file
            TempInput.setInputFilePath(@"D:\biomed\DADM\C#\100v5.txt");
            uint fs = TempInput.getFrequency();
            Vector<double> sig = TempInput.getSignal();

            R_Peaks pt = new R_Peaks();
            pt.delay = 0;

            //filtering
            double[] arr_f = pt.Filtering(Convert.ToDouble(fs), fd, fg, sig.ToArray());   //plus convert vector to array
            Vector<double> sig_f = Vector<double>.Build.DenseOfArray(arr_f);           //convert array to vector

            //differentiation
            double[] arr_d = pt.Derivative(arr_f);

            //squaring
            double[] arr_2 = pt.Squaring(arr_d);

            //moving-window integration
            double[] arr_i = pt.Integrating(arr_2, fs);
            Vector<double> sig_i = Vector<double>.Build.DenseOfArray(arr_i);

            //enhancing by put zero below threshold (0.002)
            for (int i = 0; i < arr_i.Length; i++)
            {
                if (arr_i[i] < 0.002)
                    arr_i[i] = 0;
            }

            //adaptive thresholding 
            //TO DO: substract delay!
            int[] potRs = pt.FindPeaks(arr_i, fs, 0.2);
            double[] Rs1d = Array.ConvertAll<int, double>(potRs, Convert.ToDouble);

            //init thresholds
            Vector<double> sig_ic = pt.CutSignal(sig_i, 0, Convert.ToInt16(2 * fs));
            double thrSigI = sig_ic.Maximum() / 3;
            double thrNoiseI = sig_ic.Average() / 2;
            double levSigI = thrSigI;
            double levNoiseI = thrNoiseI;

            Vector<double> sig_fc = pt.CutSignal(sig_f, 0, Convert.ToInt16(2 * fs));
            double thrSig = sig_fc.Maximum() / 3;
            double thrNoise = sig_fc.Average() / 2;
            double levSig = thrSig;
            double levNoise = thrNoise;

            List<int> locsR = new List<int>();

            //detecting peaks in both signals(integrated and filtered)
            foreach (int r in potRs)
            {
                //detect peaks in filetred signal (delay=10 from filtering!)
                int window = Convert.ToInt16(0.15 * fs);
                if ((r <= sig_f.Count) && (r - window >= 0))
                {
                    Vector<double> tempSig = pt.CutSignal(sig_f, r - window, r);
                    locsR.Add(tempSig.MaximumIndex() + r - window + 1-10);
                }
                else if (r > sig_f.Count)
                {
                    Vector<double> tempSig = pt.CutSignal(sig_f, r - window, sig_f.Count - 1);
                    locsR.Add(tempSig.MaximumIndex() + r - window + 1-10);
                }
                else
                {
                    Vector<double> tempSig = pt.CutSignal(sig_f, 0, r);
                    locsR.Add(tempSig.MaximumIndex()-10);
                }

            }



            //write result to DATA
            //Tuple<string, Vector<double>> r_data = Tuple.Create<channel, Vector<double> rPeaks>;
            // R_Peaks.Add(r_data);
            
            //write result to dat file
            //TempInput.setOutputFilePath(@"D:\biomed\DADM\C#\100v5R.txt");
            //TempInput.writeFile(fs, Vector<double>.Build.DenseOfArray(Rs1d));

            // foreach(double type in arr_ic) { Console.WriteLine(type); }
            Console.WriteLine(pt.delay);
            Console.WriteLine();
            foreach (int loc in locsR) { Console.WriteLine(loc); }
            Console.ReadKey();

        }

        //FIELDS
        #region 
        /// <summary>
        /// Store tha value of delay in samples generates due to processing
        /// </summary>
        #endregion
        private uint delay { set; get; }

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
            delay += 10;
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
            delay += 2;
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
            delay += Convert.ToUInt32(Math.Round(window / 2));
            IList<double> hi_coeff = new List<double>();
            for (int i = 0; i < window; i++)
            {
                hi_coeff.Add(1 / window);
            }
            OnlineFirFilter integrationFilter = new OnlineFirFilter(hi_coeff);
            integratedSignal = integrationFilter.ProcessSamples(squaredSignal);
            return integratedSignal;
        }

        #region
        /// <summary>
        /// Function that detects peaks (local maxima) in signal which are located in determined distance from each other
        /// </summary>
        /// <param name="signal"> Analysed signal with local maxima</param>
        /// <param name="fs"> sampling  grequency of the signal</param>
        /// <param name="distanceInSec"> Distance in seconds - minimum distance between next peaks</param>
        /// <returns> int array which contains teh localisation of peaks in signal </returns>
        #endregion
        int[] FindPeaks(double[] signal, uint fs, double distanceInSec)
        //TO DO: distance--> threshold???
        {
            List<int> potRs = new List<int>();
            double distanceInSamples = fs * distanceInSec;
            for (int i = 1; i < signal.Length - 1; i++)
            {
                if ((signal[i] > signal[i - 1]) && (signal[i] > signal[i + 1]))
                {
                    potRs.Add(i);
                }
            }
            int j = 1;                          //remove maximas which are closer to previous maximum than distance
            int prevR = potRs.First();
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
            return potRs.ToArray();
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
        Vector<double> CutSignal(Vector<double> inputSignal, int begin, int end)
        {
            int len = end - begin + 1;
            double[] cuttedSignal = new double[len];
            for (int i = 0; i < len; i++)
            {
                cuttedSignal[i] = inputSignal[i + begin];
            }
            return Vector<double>.Build.DenseOfArray(cuttedSignal);
        }
    }

}
