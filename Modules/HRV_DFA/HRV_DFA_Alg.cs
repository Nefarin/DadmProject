using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;

namespace EKG_Project.Modules.HRV_DFA
{
    public partial class HRV_DFA : IModule
    {

        public static void Main(string[] args)             
        {
            //NEEDED: MAIN: integrate, lengthN_min, lengthN_max, localTrend, fluctFn, alpha, get_n, get_Fn, get_alphas

            //read data from file
            TempInput.setInputFilePath(@"C:\Users\Paulina\Desktop\DADM\R_100.txt");
            uint fs = TempInput.getFrequency();
            Vector<double> sig = TempInput.getSignal();

            HRV_DFA dfa = new HRV_DFA();

            // Samples to time convertion [ms]
            Vector<double> tacho_rr = dfa.TimeConvert(fs, sig.ToArray());

            // samplesOrder obtaining
            Vector<double> samplesOrder = dfa.Ordering(tacho_rr);

            // Signal integration
            Vector<double> sig_integrated = dfa.Integrate(tacho_rr);

            Console.WriteLine(fs);
            Console.WriteLine(sig_integrated);
            Console.ReadKey();

        }

        // METHODS
        // function that converts samples numers to time [ms]
        public Vector<double> TimeConvert(uint samplFreq, double[] rRawSamples)
        {
            int signal_size = rRawSamples.Count();
            Vector < double > tachos_r = Vector<double>.Build.Dense(signal_size);

            for (int i = 0; i < signal_size; i++)
            {
                tachos_r[i] = rRawSamples[i] * 1000/samplFreq ;     // [ms]
            }

            return tachos_r;
        }

        //function for samples ordering
        public Vector<double> Ordering(Vector<double> signal_rr)
        {
            // samplesOrder obtaining
            Vector<double> samplesOrder = Vector<double>.Build.Dense(signal_rr.Count(), 0);
            for (int i = 0; i < samplesOrder.Count; i++)
            {
                samplesOrder[i] = i;
            }
            return samplesOrder;
        }

        //function that integrates signal
        public Vector<double> Integrate(Vector<double> signal_rr)
        {
            Vector<double> signal_integrated = Vector<double>.Build.Dense(signal_rr.Count(),0);

            //Average
            double rr_avg = signal_rr.Sum()/signal_rr.Count;

            for (int i = 0; i < signal_rr.Count - 1; i++)
            {
                signal_integrated[0] = 0;
                signal_integrated[i+1] = signal_rr[i] - rr_avg;  
                signal_integrated[i + 1] += signal_integrated[i];
                signal_integrated[i+1] = Math.Abs(signal_integrated[i+1]);
            }

            return signal_integrated;
        }
    }
}
