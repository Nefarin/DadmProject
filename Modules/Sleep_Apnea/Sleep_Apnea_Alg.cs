using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
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
        static Vector<double> _ecg;
        static List<int> _Rpeaks;

        static void Main(string[] args)
        {
            //read data from file
            _Rpeaks = new List<int>();
            TempInput.setInputFilePath(@"D:\studia nowe\dadm\projekt\matlabfunkcje\R_det.txt");
            TempInput.getFrequency();
            Vector<double> rpeaks = TempInput.getSignal();
            foreach (double singlePeak in rpeaks)
            {
                _Rpeaks.Add((int)singlePeak);
            }
            TempInput.setInputFilePath(@"D:\studia nowe\dadm\projekt\matlabfunkcje\ECG.txt");
            Vector<double> _ecg = TempInput.getSignal();
            uint freq = 100;
            TempInput.setOutputFilePath(@"D:\studia nowe\dadm\projekt\matlabfunkcje\is_apnea.txt");

        }

        //function that finds interval between RR peaks [s]
        List<List<double>> findIntervals(List<uint> R_detected, int freq)
        {
            double inter;
            List<List<double>> RR = new List<List<double>>(2);
            RR.Add(new List<double>(R_detected.Count));
            RR.Add(new List<double>(R_detected.Count));

            for (int i = 0; i < R_detected.Count(); i++)
            {
                RR[0].Add((double)(R_detected[i]));
            }
            for (int i = 0; i < R_detected.Count(); i++)
            {
                RR[1].Add((double)((R_detected[i + 1] - R_detected[i]) / freq));
            }

            RR[1].Add(0.0);

            return RR;
        }

        //use of average filter 
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

            for (int i = length - (okno - 1) / 2; i <= length; i++)
            {
                if (RR[1][i] > 0.4 && RR[1][i] < 2.0)
                {
                    sum += RR[1][ i];
                    licznik += 1;
                }
            }

            mean = sum / licznik;

            for (int i = length - (okno - 1) / 2; i <= length; i++)
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

            for (int i = 0; i <= length; i++)
            {
                if (correct[i] == true)
                {
                    RR_average[0].Add(RR[0][i]);
                    RR_average[1].Add(RR[1][i]);
                }
            }

            for (int i = 1; i <= length; i++)
            {
                if (correct[i] == false)
                {
                    RR_average[0].Add(RR[0][i]);
                    RR_average[1].Add((RR[0][i - 1] + RR[1][i + 1]) / 2);
                }
            }

            return RR_average;

        }

        //resampling on 1Hz frequency        
        List<List<double>> resampling(List<List<double>> RR_average, int freq)
        {
            int n_start = (int)RR_average[0][1];
            int n_stop = (int)RR_average[0][RR_average.Count - 1];
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

        //Highpass, lowpass filter        
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


        //Żanety..................................................

                void hilbert(List<List<double>> RR_HPLP, ref List<List<double>> h_amp, ref List<List<double>> h_freq)
        {
            int i, l, npt, lfilt, LMAX = RR_HPLP[0].Count - 1;
            lfilt = 32;
            int LFILT=lfilt;
            //defining local arrays
            double[] x = new double[LMAX + 1];
            double[] xh = new double[LMAX + 1];
            double[] phase = new double[LMAX + 1];
            double[] ampl = new double[LMAX + 1];
            double[] time = new double[LMAX + 1];
            double[] freq = new double[LMAX + 1];
            double[] hilb = new double[LFILT + 1];
            double pi, pi2, xt, xht;

            pi = 3.1415; pi2 = 2 * pi;

            for (i = 1; i <= lfilt; i++)
            {
                hilb[i] = 1 / ((i - lfilt / 2.0) - 0.5) / pi;
            }

            for (i = 1; i <= LMAX; i++)
            {
                time[i] = RR_HPLP[0][i];
                x[i] = RR_HPLP[1][i];
                xh[i] = 0.0;
                ampl[i] = 0.0;
            }
            npt = LMAX + 1;

            //hilbert transform
            double yt;
            for (l = 1; l < npt - lfilt + 1; l++)
            {
                yt = 0.0;
                for (i = 1; i <= lfilt; i++)
                    yt = yt + x[l + i - 1] * hilb[lfilt + 1 - i];
                xh[l] = yt;
            }
            /* shifting lfilt/1+1/2 points */
            for (i = 1; i <= npt - lfilt; i++)
            {
                xh[i] = 0.5 * (xh[i] + xh[i + 1]);
            }
            for (i = npt - lfilt; i >= 1; i--)
            {
                xh[i + lfilt / 2] = xh[i];
            }
            /* writing zeros */
            for (i = 1; i <= lfilt / 2; i++)
            {
                xh[i] = 0.0;
                xh[npt - i] = 0.0;
            }

            // Ampl and phase
            for (i = lfilt / 2 + 1; i <= npt - lfilt / 2; i++)
            {
                xt = x[i];
                xht = xh[i];
                ampl[i] = Math.Sqrt(xt * xt + xht * xht);
                phase[i] = Math.Atan2(xht, xt);
                if (phase[i] < phase[i - 1])
                {
                    freq[i] = phase[i] - phase[i - 1] + pi2;
                }
                else
                {
                    freq[i] = phase[i] - phase[i - 1];
                }
            }


            //writing output arrays
            int id_start = (LFILT / 2) + 1;
            int id_stop = LMAX - (LFILT / 2);
            int size1 = id_stop - id_start + 1;
            //resizing

            h_amp.Add(new List<double>(size1));
            h_freq.Add(new List<double>(size1));
            h_amp.Add(new List<double>(size1));
            h_freq.Add(new List<double>(size1));

            //writing time and values
            int j = 0;
            for (int k = id_start; k <= id_stop; k++)
            {
                if (j < size1)
                {
                    h_amp[0].Add(time[k]);
                    h_amp[1].Add(ampl[k]);

                    h_freq[0].Add(time[k]);
                    h_freq[1].Add(freq[k]);
                    j++;
                }
            }
        }

        void freq_amp_filter(List<List<double>> h_freq, List<List<double>> h_amp)
        {
            double diff, t1, t2, f1, f2, a, b; int i;

            double maxi = 0;
            //calculating treshold value for filter
            for (i = 0; i < h_freq[0].Count - 1; i++)
            {
                diff = Math.Abs(h_freq[1][i] - h_freq[1][i + 1]);
                if (diff > maxi)
                    maxi = diff;
            }
            double limit = maxi * 0.2;


            for (i = 1; i < h_freq[0].Count - 1; i++)
            {
                if (Math.Abs(h_freq[1][i]) > limit)
                {
                    t1 = h_freq[0][i - 1]; f1 = h_freq[1][i - 1];
                    t2 = h_freq[0][i + 1]; f2 = h_freq[1][i + 1];
                    a = (f1 - f2) / (t1 - t2);
                    b = f1 - a * t1;
                    h_freq[1][i] = a * h_freq[0][i] + b;
                }
            }

            //normalization of amplitude signal
            double sum = 0;
            for (i = 0; i < h_amp[0].Count; i++)
                sum += h_amp[1][i];
            double mean = sum / h_amp[0].Count;
            //writing values to output array
            for (i = 0; i < h_amp[0].Count; i++)
            {
                h_amp[1][i] = h_amp[1][i] * (1 / mean);
            }
        }

        void median_filter(List<List<double>> h_freq, List<List<double>> h_amp)
        {
            int window_median = 60;
            double[] amp = new double[window_median];
            double[] freq = new double[window_median];
            int i, j, k, l;
            double median_amp = 0, median_freq = 0;

            for (i = 0; i < h_freq[0].Count; i++)
            {
                if (i % window_median == 0 && i > 0)
                {
                    k = (i / window_median) - 1;
                    //filling arrays
                    l = 0;
                    for (j = 0 + window_median * k; j < window_median + window_median * k; j++)
                    {
                        freq[l] = h_freq[1][j];
                        amp[l] = h_amp[1][j];
                        l++;
                    }
                    //sorting arrays                    
                    Array.Sort(freq);
                    Array.Sort(amp);
                    //finding median_elements
                    if (freq.Length % 2 != 0)
                    {
                        median_freq = freq[(int)((freq.Length - 1) / 2)];
                        median_amp = amp[(int)((amp.Length - 1) / 2)];
                    }
                    else
                    {
                        median_freq = (freq[(int)(Math.Floor(((double)freq.Length - 1) / 2))] + freq[(int)(Math.Floor(((double)freq.Length - 1) / 2)) + 1]) * 0.5;
                        median_amp = (amp[(int)(Math.Floor(((double)amp.Length - 1) / 2))] + amp[(int)(Math.Floor(((double)amp.Length - 1) / 2)) + 1]) * 0.5;
                    }
                    //writing output arrays
                    for (j = 0 + window_median * k; j < window_median + window_median * k; j++)
                    {
                        h_freq[1][j] = median_freq;
                        h_amp[1][j] = median_amp;
                    }
                }
            }

            //loop for last elements
            if (h_freq[0].Count() % window_median != 0)
            {
                int start_id = (int)(Math.Floor((double)h_freq[0].Count / window_median) * window_median);
                int stop_id = h_freq[0].Count - 1;
                double[] amp1 = new double[stop_id - start_id + 1];
                double[] freq1 = new double[stop_id - start_id + 1];
                //filling arrays
                j = start_id;
                for (i = 0; i < freq1.Length; i++)
                {
                    freq1[i] = h_freq[1][j];
                    amp1[i] = h_amp[1][j];
                    j++;
                }
                //sorting arrays
                Array.Sort(freq);
                Array.Sort(amp);
                //finding median_elements
                if (freq1.Length % 2 != 0)
                {
                    median_freq = freq1[(int)((freq1.Length - 1) / 2)];
                    median_amp = amp1[(int)((amp1.Length - 1) / 2)];
                }
                else
                {
                    median_freq = (freq1[(int)(Math.Floor(((double)freq1.Length - 1) / 2))] + freq1[(int)(Math.Floor(((double)freq1.Length - 1) / 2)) + 1]) * 0.5;
                    median_amp = (amp1[(int)(Math.Floor(((double)amp1.Length - 1) / 2))] + amp1[(int)(Math.Floor(((double)amp1.Length - 1) / 2)) + 1]) * 0.5;
                }
                //writing output arrays
                for (i = start_id; i <= stop_id; i++)
                {
                    h_freq[1][i] = median_freq;
                    h_amp[1][i] = median_amp;
                }
            }
        }

        List<Tuple<ulong, ulong>> apnea_detection(List<List<double>> tab_amp, List<List<double>> tab_freq)
        {
            List<Tuple<ulong, ulong>> apnea_out = new List<Tuple<ulong, ulong>>();

            //treshold value for amplitude
            int i; double a, b, mini_amp, mini_freq, maxi_amp, maxi_freq, mid, y_amp, y_freq;
            mini_amp = 99999; mini_freq = 99999;
            maxi_amp = 0; maxi_freq = 0;
            for (i = 0; i < tab_amp[0].Count(); i++)
            {
                if (tab_amp[1][i] > maxi_amp) maxi_amp = tab_amp[1][i];
                if (tab_amp[1][i] < mini_amp) mini_amp = tab_amp[1][i];
                if (tab_freq[1][i] > maxi_freq) maxi_freq = tab_freq[1][i];
                if (tab_freq[1][i] < mini_freq) mini_freq = tab_freq[1][i];
            }
            a = -0.18; b = 1; mid = (maxi_amp + mini_amp) * 0.5;
            y_amp = a + b * (mid + 1) * 0.5;

            //treshold value for frequency
            y_freq = (maxi_freq + mini_freq) * 0.4;

            //apnea detection
            bool[] detect = new bool[tab_amp[0].Count];
            for (i = 0; i < tab_amp[0].Count; i++)
            {
                if (tab_amp[1][i] >= y_amp) detect[i] = true;
                else detect[i] = false;
            }

            bool added = false;
            ulong item1 = 0;
            for (i = 0; i < tab_amp[0].Count; i++)
            {
                if (detect[i] == true && added == false)
                {
                    item1 = (ulong)tab_amp[0][i];
                    added = true;
                }
                if (((detect[i] == false) && (added == true)) || (i == tab_amp[0].Count - 1 && item1 != 0))
                {
                    ulong item2 = (ulong)tab_amp[0][i];
                    apnea_out.Add(new Tuple<ulong, ulong>(item1, item2));
                    item1 = 0;
                    added = false;
                }
            }

            if (apnea_out.Count == 0)
            {
                //apnea_out.append(BeginEndPair(0, 0));
                apnea_out.Add(new Tuple<ulong, ulong>(0, 0));
            }

            return apnea_out;

        }
    }

}
