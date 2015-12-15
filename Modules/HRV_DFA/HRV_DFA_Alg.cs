using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.HRV_DFA
{
    public partial class HRV_DFA : IModule
    {

        public static void Main(string[] args)             
        {
            //NEEDED: PREP: numberOfSamples, average, 
            //NEEDED: MAIN: integrate, lengthN_min, lengthN_max, localTrend, fluctFn, alpha, get_n, get_Fn, get_alphas

            //read data from file
            TempInput.setInputFilePath(@"C:\Users\Paulina\Desktop\DADM\R_100.txt");
            uint fs = TempInput.getFrequency();
            Vector<double> sig = TempInput.getSignal();

            HRV_DFA hd = new HRV_DFA();

            //samples to time convertion [ms]
            Vector<double> tacho_rr = hd.TimeConvert(fs, sig.ToArray());  
           
           // numberOfSamples obtaining

            Console.WriteLine(sig);
            Console.WriteLine(fs);
            Console.WriteLine(tacho_rr);
            Console.ReadKey();

        }

        // METHODS
        // function that converts samples to time [ms]
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

    }
}
