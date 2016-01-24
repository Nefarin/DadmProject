using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;

namespace EKG_Project.Modules.T_Wave_Alt
{
    public class T_Wave_Alt_Alg
    {
        private int[] findAlternans(List<int> t_end_List, Vector<double> ecg, uint fs)
        {
            int[] t_end = t_end_List.ToArray();

            // t-wave length calculation
            // assuming t-wave length of 150ms
            // 360Hz*0,15s=54
            double t_length1 = fs*0.15;
            int t_length = Convert.ToInt32(t_length1);

            // creating a matrix containing all detected t-waves
            var M = Matrix<double>.Build;

            var t_waves = M.Dense(t_end.Length, t_length);

            for (var i = 0; i < t_end.Length; i++)
            {
                var j = 0;
                for (int v = t_end[i] - t_length; v < t_end[i]; v++)
                {
                    t_waves[i, j] = ecg[v];
                    j += 1;
                }
            }

            // creating a vector containg the medians of corresponding
            // t-waves samples
            var V = Vector<double>.Build;

            var t_mdn = V.Dense(t_length);
            var temp = V.Dense(t_end.Length);

            for (var i = 0; i < t_length; i++)
            {
                t_waves.Column(i, 0, t_end.Length, temp);
                var mdn = temp.ToArray();
                Array.Sort(mdn);

                int mid = t_end.Length / 2;
                t_mdn[i] = mdn[mid];
            }

            // calculating ACI values for each t-wave vector
            var aci = V.Dense(t_end.Length);

            for (var m = 0; m < t_end.Length; m++)
            {
                var aci_aux_nom = V.Dense(t_length);
                var aci_aux_denom = V.Dense(t_length);

                for (var n = 0; n < t_length; n++)
                {
                    aci_aux_nom[n] = t_waves[m, n] * t_mdn[n];
                    aci_aux_denom[n] = t_mdn[n] * t_mdn[n];
                }

                aci[m] = aci_aux_nom.Sum() / aci_aux_denom.Sum();
            }

            // finding fluctuations around 1
            // 1 - value changed around 1
            // 0 - value hasn't changed
            var fluct = V.Dense(t_end.Length);

            for (int k = 0; k < t_end.Length; k++)
            {
                if (((aci[k] > 1) && (aci[k + 1] < 1)) || ((aci[k] < 1) && (aci[k + 1] > 1)))
                {
                    fluct[k] = 1;
                }
                else
                {
                    fluct[k] = 0;

                }
            }

            // determining whether the fluctuations have occured in 
            // 7 or more consecutive heartbeats
            var alt_tresh = 7;
            var V1 = Vector<int>.Build;
            var alternans = V1.Dense(t_end.Length);
            var counter = 0;
            var first_el = 0;
            var last_el = 0;

            for (var o = 0; o < t_end.Length; o++)
            {
                if (counter == 0) 
                {
                    if (fluct[o]==1)
                    {
                        first_el = o;
                        counter++;
                    }
                }

                else if ((counter > 0) && (counter < alt_tresh))
                {
                    if (fluct[o] == 1)
                    {
                        counter++;
                    }
                    else
                    {
                        counter = 0;
                    }
                }

                else if (counter >= alt_tresh)
                {
                    if (fluct[o] == 1)
                    {
                        counter++;
                    }
                    else
                    {
                        last_el = o - 1;
                        counter = 0;
                        for (var p = first_el; p <= last_el; p++)
                        {
                            alternans[p] = 1;
                        }
                        first_el = 0;
                        last_el = 0;
                    }
                }
            }

            var alternans_out = alternans.ToArray();
            return alternans_out;
        }
        
        /*
        public static void Main(string [] args)
        {
            TempInput.setInputFilePath(@"C:\Users\fouette\Desktop\Janek projekt\signal.txt");
            uint fs = TempInput.getFrequency();
            Vector<double> sig = TempInput.getSignal();

            TempInput.setInputFilePath(@"C:\Users\fouette\Desktop\Janek projekt\t_end.txt");
            Vector<int> t_ends = TempInput.getSignal();

            T_Wave_Alt instance1 = new T_Wave_Alt();
            int[] alt_found = instance1.findAlternans(t_ends, sig, fs);

            TempInput.setOutputFilePath(@"C:\Users\fouette\Desktop\Janek projekt\alternans.txt");
            TempInput.writeFile(fs, alt_found);
        }
        */
    }
}
