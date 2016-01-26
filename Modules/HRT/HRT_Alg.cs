using System;
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

        public void PrintVector(Vector<double> Signal)
        {
            foreach (double _licznik in Signal)
            {
                Console.WriteLine(_licznik);
            }
        }
        public void PrintVector(int[] Signal)
        {
            for (int i =0; i<Signal.Length; i++)
            { 
                Console.WriteLine(Signal[i]);
            }
        }

        public Vector<double> rrTimesShift(Vector<double> rrPeaks)
        {
            for (int i=0; i<rrPeaks.Count;i++)
            {
                rrPeaks[i]++;
            }
            return rrPeaks;
        }

        public Vector<double> GetNrVPC(double[] rrTimes, int[] rrTimesVPC, int VPCcount)
        {
            if (rrTimes == null || rrTimesVPC == null) throw new ArgumentNullException();
            if (rrTimes.Length < rrTimesVPC.Length) throw new ArgumentOutOfRangeException();
            if (rrTimesVPC.Length < VPCcount || VPCcount < 0 ) throw new ArgumentOutOfRangeException();

            double[] nrVPCArray;
            nrVPCArray = new double[VPCcount];
            for (int i = 0; i < VPCcount; i++)
            {
                for (int j = 0; j < rrTimes.Length; j++)
                {
                    if (rrTimes[j] == rrTimesVPC[i])
                    {
                        nrVPCArray[i] = j;
                    }
                }
            }
            Vector<double> nrVPC = Vector<double>.Build.DenseOfArray(nrVPCArray);
            return nrVPC;
        }
    }

}
