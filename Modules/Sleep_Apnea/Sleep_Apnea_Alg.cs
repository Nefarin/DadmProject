using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using EKG_Project.Modules.ECG_Baseline;


namespace EKG_Project.Modules.Sleep_Apnea
{
    #region Sleep_Apnea Class doc
    /// <summary>
    /// Class that locates the periods of sleep apnea in ECG 
    /// </summary>
    #endregion

    public partial class Sleep_Apnea : IModule
    {

        #region
        /// <summary>
        /// Function that finds intervals between RR peaks
        /// </summary>
        /// <param name="freq"> frequency of sampling for signal</param>
        /// <param name="R_detected"> Signal - number of samples for R peaks </param>
        /// <returns> RR intervals</returns>
        #endregion
        
        List<List<double>> findIntervals(List<uint> R_detected, int freq)
        {

            List<List<double>> RR = new List<List<double>>(2);
            RR.Add(new List<double>(R_detected.Count));
            RR.Add(new List<double>(R_detected.Count));

            for (int i = 0; i < R_detected.Count(); i++)
            {
                RR[0].Add((double)(R_detected[i]));
            }
            for (int i = 0; i < R_detected.Count() - 1; i++)
            {
                RR[1].Add((((double)R_detected[i + 1] - R_detected[i]) / freq));
            }

            RR[1].Add(0.0);

            return RR;
        }

        #region
        /// <summary>
        /// Function that filters RR intervals using average filter
        /// </summary>
        /// <param name="RR"> Signal - RR intervals </param>
        /// <returns> Filtered RR intervals</returns>
        #endregion

        List<List<double>> averageFilter(List<List<double>> RR)
        {
            int okno = 41;
            int length = RR[0].Count;
            double sum = 0;
            int licznik = 0;
            bool[] correct = new bool[length];

            //filter fo samples from 1 to (okno-1)/2
            //exclude intervals which lie outside the range of 0.4 to 2.0 sec
            for (int i = 0; i <= okno; i++)
            {
                if (RR[1][i] > 0.4 && RR[1][i] < 2.0)
                {
                    sum += RR[1][i];
                    licznik += 1;
                }
            }

            double mean = sum / licznik;

            for (int i = 0; i < (okno - 1) / 2; i++)
            {
                if (RR[1][i] > 0.8 * mean && RR[1][i] < 1.2 * mean)
                    correct[i] = true;
                else
                    correct[i] = false;
            }

            //filter fo samples from (okno+1)/2 to length -(okno-1)/2
            sum = 0;
            licznik = 0;

            for (int i = (okno + 1) / 2; i <= length - (okno - 1) / 2; i++)
            {
                for (int z = i - (okno - 1) / 2; z <= i + (okno - 1) / 2; z++)
                {
                    if (RR[1][i] > 0.4 && RR[1][i] < 2.0)
                    {
                        sum += RR[1][i];
                        licznik += 1;
                    }
                }
                mean = sum / licznik;
                if (RR[1][i] > 0.8 * mean && RR[1][i] < 1.2 * mean)
                    correct[i] = true;
                else
                    correct[i] = false;

            }

            //filter fo samples from length-(okno-1)/2 to length
            sum = 0;
            licznik = 0;

            for (int i = length - (okno - 1) / 2; i < length; i++)
            {
                if (RR[1][i] > 0.4 && RR[1][i] < 2.0)
                {
                    sum += RR[1][i];
                    licznik += 1;
                }
            }

            mean = sum / licznik;

            for (int i = length - (okno - 1) / 2; i < length; i++)
            {
                if (RR[1][i] > 0.8 * mean && RR[1][i] < 1.2 * mean)
                    correct[i] = true;
                else
                    correct[i] = false;
            }

            //create new array and write filtered samples            
            List<List<double>> RR_average = new List<List<double>>(2);
            RR_average.Add(new List<double>());
            RR_average.Add(new List<double>());

            for (int i = 0; i < length; i++)
            {
                if (correct[i] == true)
                {
                    RR_average[0].Add(RR[0][i]);
                    RR_average[1].Add(RR[1][i]);
                }
            }

            for (int i = 1; i < length - 1; i++)
            {
                if (correct[i] == false)
                {
                    RR_average[0].Add(RR[0][i]);
                    RR_average[1].Add((RR[0][i - 1] + RR[1][i + 1]) / 2);
                }
            }

            return RR_average;

        }

        #region
        /// <summary>
        /// Function that resamples RR intervals on 1Hz frequency
        /// </summary>
        /// <param name="freq"> frequency of sampling for signal</param>
        /// <param name="RR_average"> Signal - RR intervals filtered </param>
        /// <returns> Resampled RR intervals</returns>
        #endregion
        //resampling on 1Hz frequency        
        List<List<double>> resampling(List<List<double>> RR_average, int freq)
        {
            int n_start = (int)RR_average[0][1];
            int n_stop = (int)RR_average[0][RR_average[0].Count - 1];
            int size = (int)Math.Floor(((double)n_stop - n_start) / freq) + 1;

            //create new array and fill with equally distant samples            
            List<List<double>> RR_res = new List<List<double>>(2);
            RR_res.Add(new List<double>(size));
            RR_res.Add(new List<double>(size));

            int j = n_start;
            for (int i = 0; i < size; i++)
            {
                RR_res[0].Add(j);
                j += freq;
            }

            //calculations for first sample
            double tm1 = RR_average[0][0];
            double tm2 = RR_average[0][1];
            double rr1 = RR_average[1][0];
            double rr2 = RR_average[1][1];
            double a = (rr1 - rr2) / (tm1 - tm2);
            double b = rr1 - a * tm1;
            RR_res[1].Add(a * RR_res[0][0] + b);

            if (RR_average[1][RR_average[0].Count - 1] == 0)
                RR_average[1][RR_average[0].Count - 1] = RR_average[1][RR_average[0].Count - 2];

            //calculations for last sample
            tm1 = RR_average[0][RR_average[0].Count - 2];
            tm2 = RR_average[0][RR_average[0].Count - 1];
            rr1 = RR_average[1][RR_average[0].Count - 2];
            rr2 = RR_average[1][RR_average[0].Count - 1];
            a = (rr1 - rr2) / (tm1 - tm2);
            b = rr1 - a * tm1;
            RR_res[1].Add(a * RR_res[0][size - 1] + b);

            //calculations for 2 to last-1 samples
            for (int k = 1; k < size - 1; k++)
            {
                int i = 0;
                while (i < RR_average[0].Count - 2 && RR_average[0][i] < RR_res[0][k])
                {
                    if (RR_average[0][i + 1] > RR_res[0][k])
                        break;
                    else
                        i = i + 1;
                }
                tm1 = RR_average[0][i];
                tm2 = RR_average[0][i + 1];
                rr1 = RR_average[1][i];
                rr2 = RR_average[1][i + 1];
                a = (rr1 - rr2) / (tm1 - tm2);
                b = rr1 - a * tm2;
                RR_res[1].Add(a * RR_res[0][k] + b);
            }

            return RR_res;
        }

        #region
        /// <summary>
        /// Function that filters RR intervals with highpass and lowpass filters
        /// </summary>
        /// <param name="RR_res"> Signal - RR intervals resampled </param>
        /// <returns> Filtered RR intervals</returns>
        #endregion
              
        List<List<double>> HPLP(List<List<double>> RR_res)
        {
            //high-pass filter
            List<double> Z1 = new List<double>(RR_res[0].Count());
            Z1.Add(0.0);

            double CUTOFF = 0.01;
            double RC = 1 / (CUTOFF * 2 * 3.14);
            double dt = 1;
            double alpha = RC / (RC + dt);

            for (int j = 1; j < RR_res[0].Count(); j++)
            {
                Z1.Add(alpha * (Z1[j - 1] + RR_res[1][j] - RR_res[1][j - 1]));
            }

            //low-pass filter
            List<double> Z2 = new List<double>(RR_res[0].Count());
            Z2.Add(0.0);

            CUTOFF = 0.09;
            RC = 1 / (CUTOFF * 2 * 3.14);
            dt = 1;
            alpha = dt / (RC + dt);

            for (int j = 1; j < Z1.Count(); j++)
            {
                Z2.Add(Z2[j - 1] + (alpha * (Z1[j] - Z2[j - 1])));
            }

            List<List<double>> RR_HPLP = new List<List<double>>(2);
            RR_HPLP.Add(new List<double>());
            RR_HPLP.Add(new List<double>());

            for (int i = 0; i < RR_res[0].Count; i++)
            {
                RR_HPLP[0].Add(RR_res[0][i]);
                RR_HPLP[1].Add(RR_res[1][i]);
            }

            return RR_HPLP;
        }


        #region
        /// <summary>
        /// Function that creates Hilbert transform for signal
        /// </summary>
        /// <param name="RR_HPLP"> Signal - RR intervals filtered </param>
        /// <returns> Hilbert's amplitudes and frequencies </returns>
        #endregion

        void hilbert(List<List<double>> RR_HPLP, ref List<List<double>> h_amp, ref List<List<double>> h_freq)
        {
            Complex[] hilb = MatlabHilbert(RR_HPLP[1].ToArray());

            double Fs = 1.0 / (RR_HPLP[0][1] - RR_HPLP[0][0]);

            h_amp.Add(new List<double>(RR_HPLP[0].Count));
            h_freq.Add(new List<double>(RR_HPLP[0].Count));
            h_amp.Add(new List<double>(RR_HPLP[0].Count));
            h_freq.Add(new List<double>(RR_HPLP[0].Count));

            //Writing time and values
            for (int i = 0; i < hilb.Length; i++)
            {
                h_amp[0].Add(RR_HPLP[0][i]);
                h_freq[0].Add(RR_HPLP[0][i]);

                h_amp[1].Add(Complex.Abs(hilb[i]));

                if (i < hilb.Length - 1)
                {
                    double phase = hilb[i].Phase;
                    if (phase < 0) phase = Math.PI * 2 + phase;
                    double phase2 = hilb[i + 1].Phase;
                    if (phase2 < 0) phase2 = Math.PI * 2 + phase2;

                    double freq = Fs / (2 * Math.PI) * (phase2 - phase);
                    h_freq[1].Add(freq);
                }
                else
                {
                    h_freq[1].Add(0.0);
                }
            }
        }

        private static Complex[] MatlabHilbert(double[] xr)
        {
            var x = (from sample in xr select new Complex(sample, 0)).ToArray();
            Fourier.BluesteinForward(x, FourierOptions.Default);
            var h = new double[x.Length];
            var fftLengthIsOdd = (x.Length | 1) == 1;
            if (fftLengthIsOdd)
            {
                h[0] = 1;
                for (var i = 1; i < xr.Length / 2; i++) h[i] = 2;
            }
            else
            {
                h[0] = 1;
                h[(xr.Length / 2)] = 1;
                for (var i = 1; i < xr.Length / 2; i++) h[i] = 2;
            }
            for (var i = 0; i < x.Length; i++)
            {
                x[i] *= h[i];
            }
            Fourier.BluesteinInverse(x, FourierOptions.Default);


            return x;
        }

        #region
        /// <summary>
        /// Function that filters signal using a moving window of 60 points
        /// </summary>
        /// <param name="h_freq"> Signal - Hilbert's frequencies </param>
        /// <param name="h_amp"> Signal - Hilbert's amplitudes </param>
        /// <returns> Filtered amplitudes and frequencies </returns>
        #endregion

        void median_filter(List<List<double>> h_freq, List<List<double>> h_amp)
        {
            int window_median = 60;
            double[] amp = new double[window_median];
            double[] freq = new double[window_median];
            double[] amp_sorted = new double[window_median];
            double[] freq_sorted = new double[window_median];
            int i, j;

            //poczatkowe wypelnienie okna
            for (i = 0; i < window_median; i++)
            {
                amp[i] = h_amp[1][i];
                amp_sorted[i] = h_amp[1][i];
                freq[i] = h_freq[1][i];
                freq_sorted[i] = h_freq[1][i];
            }
            Array.Sort(amp_sorted);
            Array.Sort(freq_sorted);
            h_amp[1][window_median/2] = amp_sorted[window_median / 2]; // tu ma byc srednia
            h_freq[1][window_median/2] = freq_sorted[window_median / 2];

            int last_index = 0;
            for (i = window_median + 1, j = window_median/2+1; i < h_amp[1].Count; i++, j++)
            {
                amp[last_index] = h_amp[1][i];
                Array.Copy(amp, amp_sorted, amp.Length);
                Array.Sort(amp_sorted);
                h_amp[1][j] = amp_sorted[window_median / 2];

                freq[last_index] = h_freq[1][i];
                Array.Copy(freq, freq_sorted, freq.Length);
                Array.Sort(freq_sorted);
                h_freq[1][j] = freq_sorted[window_median / 2];

                last_index++;
                if(last_index % window_median == 0)
                {
                    last_index = 0;
                }
            }
        }

        //Normalization of amplitude signal
        void amp_filter(List<List<double>> h_amp)
        {
            double sum = 0;
            for (int i = 0; i < h_amp[0].Count; i++)
                sum += h_amp[1][i];
            double mean = sum / h_amp[0].Count;

            //Writing values to output array
            for (int i = 0; i < h_amp[0].Count; i++)
            {
                h_amp[1][i] = h_amp[1][i] * (1 / mean);
            }

            //Changing the field from samples numbers to time
            double[] time = new double[h_amp[0].Count];
            for (int i = 0; i < h_amp[0].Count(); i++)
            {
                time[i] = (h_amp[0][i] - h_amp[0][0]) / (h_amp[0][1] - h_amp[0][0]);
            }

            for (int i = 0; i < h_amp[0].Count(); i++)
            {
                h_amp[0][i] = time[i];
            }
        }

        #region
        /// <summary>
        /// Function that detects Apnea(if the frequency goes below 0,06 Hz and the amplitude goes above max amplitude the same time)
        /// </summary>
        /// <param name="h_freq"> Signal - filtered Hilbert's frequencies </param>
        /// <param name="h_amp"> Signal - filtered Hilbert's amplitudes </param>
        /// <returns> The percentage value of sleep apnea in signal (il_Apnea), Hilbert's amplitudes (h_amp) and time periods for which detected apnea (Detected_Apnea) </returns>
        #endregion


        List<Tuple<int, int>> apnea_detection(List<List<double>> h_amp, List<List<double>> h_freq, out double il_Apnea)
        {


            //Finding the minimum and maximum Hilbert amplitudes
            int i;
            double a, b, min_amp, max_amp, mid, y_amp, y_freq;
            min_amp = 9999;
            max_amp = 0;

            for (i = 0; i < h_amp[0].Count(); i++)
            {
                if (h_amp[1][i] > max_amp) max_amp = h_amp[1][i];
                if (h_amp[1][i] < min_amp) min_amp = h_amp[1][i];
            }
            //The minimum Hilbert amplitude threshold (a linear function of the minimum and maximum Hilbert amplitudes):
            a = -0.555; b = 1.3;
            //mid = the midpoint of the minimum and maximum amplitudes
            mid = (max_amp + min_amp) * 0.5;
            y_amp = a + b * (mid + 1) * 0.5;

            //The maximum Hilbert frequency threshold [Hz]:
            y_freq = 0.06;

            //Apnea detection
            bool[] detect = new bool[h_amp[0].Count];
            for (i = 0; i < h_amp[0].Count; i++)
            {
                if (h_amp[1][i] >= y_amp && h_freq[1][i] <= y_freq) detect[i] = true;
                else detect[i] = false;
            }

            //Checking if the duration of sleep apnea is longer than 60 seconds
            List<Tuple<int, int>> Detected_Apnea = new List<Tuple<int, int>>();
            int counter = 0;
            int counter2 = 0;
            for (i = 0; i < detect.Length; i++)
            {
                if (detect[i] == false)
                {
                    if (counter >= 60)
                    {
                        Detected_Apnea.Add(new Tuple<int, int>(i - 1 - counter, i - 1));
                        counter2 = counter2 + counter;
                    }
                    counter = 0;
                }
                else
                {
                    counter = counter + 1;
                }
            }

            //Calculating the percentage of sleep apnea         

            il_Apnea = (counter2 / detect.Length) * 100;



            if (Detected_Apnea.Count == 0)
            {
                Detected_Apnea.Add(new Tuple<int, int>(0, 0));
            }


            return Detected_Apnea;
        }


    }
}

