using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.HRT
{
    public class HRT_Alg
    {
        public bool IsLengthOfTachogramOK(Vector<double> Tachogram)
        { 
            return false;
        }
        public Vector<double> SearchVentricularTurbulence(Vector<double> Tachogram, Vector<double> RRTimes, Vector<double> RRTimesVC)
        {

            return Tachogram;
        }

        public Vector<double> ChangeVectorIntoTimeDomain(Vector<double> SignalUnchanged,int samplingFreq)
        {
            Vector<double> SignalInTimeDomain;
            SignalInTimeDomain = SignalUnchanged / samplingFreq;
            return SignalInTimeDomain;
        }

        int CountNrOfVPC(Vector<double> rrTimesVPC) {

            return 0;
        }


        //ustalenie które nr pików R to są VPC
        //Vector<double> WhichPeaksAreVPC(Vector<double> rrTimes, Vector<double> rrTimesVPC)
        //{
        //    int VPCcount=rrTimesVPC
        //    Vector<double> nrVPC;
        //    for (int i = 0; i < VPCcount; i++)
        //    {
        //        for (int j = 0, j< rrTimes.Count; j++)
        //        {
        //            if rrTimes(i) == rrTimesVPC(j)
        //            {
        //                nrVPC(j) = i;
        //            }

        //        }
        //    }
        //}










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
