using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.IO;
using EKG_Project.Modules.R_Peaks;
using EKG_Project.Modules.Heart_Class;
using EKG_Project.IO;


namespace EKG_Project.Modules.HRT
{
    public class HRT_Alg
    {

        public Vector<double> _rInstants { get; set; }
        public Vector<double> _rInstantsVentricularComplex { get; set; }
        public Vector<double> _rrIntervals { get; set; }

        public HRT_Alg(Vector<double> Tachogram, Vector<double> RRTimes, Vector<double> RRTimesVC)
        {
            _rrIntervals = Tachogram;
            _rInstants = RRTimes;
            _rInstantsVentricularComplex = RRTimesVC;
        }


        //public static void Main(string[] args)
        //{
        //    //read data from file
        //    TempInput.setInputFilePath(@"C:\Users\mrevening\Desktop\R_100.txt");
        //    uint fs = TempInput.getFrequency();
        //    Vector<double> sig = TempInput.getSignal();

        //    HRT_Alg hrt = new HRT_Alg(sig);

        //    // Samples to time convertion [ms]
        //    Vector<double> tacho_rr = hrt.TimeConvert(fs, sig.ToArray());

        //    Console.WriteLine(fs);
        //    Console.WriteLine(sig);
        //    Console.ReadKey();
        //}
    }
}
