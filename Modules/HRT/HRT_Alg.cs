using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.IO;


namespace EKG_Project.Modules.HRT
{
    public partial class HRT : IModule
    {
        // Declaration of time parameters
        private const int _back = 5;
        private const int _foward = 15;

        ///<Summary>_RRTimes nr próbek wystąpienia załamków R</Summary>
        public Vector<double> _rInstants { get; set; }

        ///<Summary>_RRTimesVPC - nr próbek wystąpienia załamków Ventricular Premature Complex</Summary>
        public Vector<double> _rInstantsVentricularComplex { get; set; }

        ///<Summary>_Tachogram -Tachogram (od modułu HRV1 lub HRV2)</Summary>
        public Vector<double> _rrIntervals { get; set; }

        //KONSTUKTORY

        ///<Summary>konstruktor główny </Summary>
        public HRT(Vector<double> Tachogram, Vector<double> RRTimes, Vector<double> RRTimesVC)
        {
            _rrIntervals = Tachogram;
            _rInstants = RRTimes;
            _rInstantsVentricularComplex = RRTimesVC;
        }

        ///<Summary>konstruktor testowy</Summary>
        public HRT(Vector<double> Tachogram)
        {
            _rrIntervals = Tachogram;
        }


        // METHODS
        // function that converts samples numers to time [ms] (od Pauliny Sołtys)
        public Vector<double> TimeConvert(uint samplFreq, double[] rRawSamples)
        {
            int signal_size = rRawSamples.Count();
            Vector<double> tachos_r = Vector<double>.Build.Dense(signal_size);

            for (int i = 0; i < signal_size; i++)
            {
                tachos_r[i] = rRawSamples[i] * 1000 / samplFreq;     // [ms]
            }

            return tachos_r;
        }





        //MAIN
        public static void Main(string[] args)
        {
            //read data from file
            TempInput.setInputFilePath(@"C:\Users\mrevening\Desktop\R_100.txt");
            uint fs = TempInput.getFrequency();
            Vector<double> sig = TempInput.getSignal();

            HRT hrt = new HRT(sig);

            // Samples to time convertion [ms]
            Vector<double> tacho_rr = hrt.TimeConvert(fs, sig.ToArray());

            Console.WriteLine(fs);
            Console.WriteLine(sig);
            Console.ReadKey();
        }
    }
}
