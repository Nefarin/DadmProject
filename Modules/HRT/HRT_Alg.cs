using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using System.IO;

namespace EKG_Project.Modules.HRT
{
    public class HRT_Alg
    {
        
        public Vector<double> SearchVentricularTurbulence(Vector<double> Tachogram, Vector<double> RRTimes, Vector<double> RRTimesVC)
        {

            return Tachogram;
        }

        public Vector<double> ChangeVectorIntoTimeDomain(Vector<double> SignalInSampleDomain,int samplingFreq)
        {
            Vector<double> SignalInTimeDomain;
            SignalInTimeDomain = SignalInSampleDomain / (samplingFreq*1000);
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

    }
}
