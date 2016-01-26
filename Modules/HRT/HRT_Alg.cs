﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using System.IO;
using MathNet.Numerics;


namespace EKG_Project.Modules.HRT
{
    public class HRT_Alg
    {

        public Vector<double> SearchVentricularTurbulences(Vector<double> Tachogram, Vector<double> RRTimes, Vector<double> RRTimesVC)
        {

            return Tachogram;
        }

        public Vector<double> ChangeVectorIntoTimeDomain(Vector<double> SignalInSampleDomain, int samplingFreq)
        {
            if (SignalInSampleDomain == null) throw new ArgumentNullException();
            if (samplingFreq <= 0) throw new ArgumentOutOfRangeException();
            Vector<double> SignalInTimeDomain;
            SignalInTimeDomain = 1000 * SignalInSampleDomain / samplingFreq;
            return SignalInTimeDomain;
        }

        public Vector<double> ChangeVectorIntoTimeDomain(int[] SignalInSampleDomain, int samplingFreq)
        {
            double[] Sig = new double[SignalInSampleDomain.Length];
            Vector<double> SignalInTimeDomain = Vector<double>.Build.Dense(SignalInSampleDomain.Length);
            int i = 0;
            foreach (int counter in SignalInSampleDomain)
            {
                Sig[i++] = 1000 * counter / samplingFreq;
            }
            return Vector<double>.Build.DenseOfArray(Sig);
        }

        public void PrintVector (Vector<double> Signal)
        {
            foreach (double _licznik in Signal)
            {
                Console.WriteLine(_licznik);
            }
        }


        //public Vector<double> CutSignal(Vector<double> inputSignal, int begin, int end)
        //{
        //    int len = end - begin + 1;
        //    double[] cuttedSignal = new double[len];
        //    for (int i = 0; i < len; i++)
        //    {
        //        cuttedSignal[i] = inputSignal[i + begin];
        //    }
        //    return Vector<double>.Build.DenseOfArray(cuttedSignal);
        //}





        public Vector<int> GetNrVPC (double[] rrTimes, double[] rrTimesVPC, int VPCcount)
        {
            Vector<int> nrVPC = Vector<int>.Build.Dense(VPCcount);
            int rrTimesLength = rrTimes.Length;
            for (int i=0; i<VPCcount; i++)
            {
                for (int j=0; j<rrTimes.Length; j++)
                {
                    if (rrTimes[i] == rrTimesVPC[j])
                    {
                        nrVPC.Add(i);
                    }
                }
                
            }
            return nrVPC;
        }
//        %ustalenie które nr pików R to są VPC
//nrVPC=zeros(VPCcount,1);
//for j=1:VPCcount
//    for i=1:length(rrTimes)
//        if rrTimes(i)==rrTimesVPC(j)
//            nrVPC(j)=i;
//        end
//    end
//end





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
