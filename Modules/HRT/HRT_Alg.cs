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

        public Vector<double> GetNrVPC(double[] rrTimes, double[] rrTimesVPC, int VPCcount)
        {
            double[] Sig = new double[VPCcount];
            //Vector<double> nrVPC = Vector<double>.Build.Dense(VPCcount);
            int rrTimesLength = rrTimes.Length;
            for (int j = 0; j < VPCcount; j++)
            {
                for (int i = 0; i < rrTimes.Length; i++)
                {
                    if ((int)rrTimes[i] == (int)rrTimesVPC[j])
                    {
                        Sig[j] = i;
                        //Console.WriteLine(Sig[j]);
                    }
                }
            }
            return Vector<double>.Build.DenseOfArray(Sig);
        }
    }
}
