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

        public void PrintVector(double[,] Signal)
        {
            for (int i = 0; i < Signal.GetLength(0); i++)
            {
                for (int j = 0; j < Signal.GetLength(1); j++)
                {
                    Console.Write(Signal[i, j]);
                    Console.Write(" ");
                }
                Console.WriteLine(";");
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

        public double[,] MakeTachogram(double[] VPC, double[] rrIntervals)
        {
            int back = 5;
            int foward = 15;
            int sum = back + foward + 1;
            int i = 0;
            double[,] VPCTachogram = new double[sum,VPC.GetLength(0)];
            foreach (int nrVPC in VPC)
            {
                if ((nrVPC - back) > 0 && ((nrVPC + foward) < rrIntervals.Length)) {
                    for (int k = (nrVPC - back); k < (nrVPC + foward + 1); k++)
                    {
                        VPCTachogram[k + back -nrVPC,i] = rrIntervals[k];
                    }
                }
                i++;
            }
            return VPCTachogram;
        }


//        back=5; foward=15; VPCtachogram = zeros(VPCcount, back+foward+1); k=0; TO=zeros(VPCcount,1); TS=zeros(VPCcount,2); after=zeros(VPCcount,1); before=zeros(VPCcount,1);
//        figure; hold on; xlabel('# of RR interval '); ylabel('RR interval [ms]');
//for i=1:VPCcount
//    if nrVPC(i)-back>0 && nrVPC(i)+foward<length(rrIntervals) 
//        VPCtachogram(i,:) = rrIntervals(nrVPC(i)-back:nrVPC(i)+foward);
//        plot(VPCtachogram(i,:),'Color',[1 102/255 0]);
//        k=k+1;
//    end
//    if nrVPC(i)-2>0 && nrVPC(i)+2<length(rrIntervals)
//        after(i) = rrIntervals(nrVPC(i)+2)+rrIntervals(nrVPC(i)+3);
//        before(i) = rrIntervals(nrVPC(i)-2)+rrIntervals(nrVPC(i)-3);
//        TO(i) = (after(i)-before(i))/before(i);
//        end
//    for j=1:foward-5
//        if nrVPC(i)+5<length(rrIntervals)
//            TSnew = polyfit(1:5, [rrIntervals(nrVPC(i) + j), rrIntervals(nrVPC(i) + 1 + j), rrIntervals(nrVPC(i) + 2 + j), rrIntervals(nrVPC(i) + 3 + j), rrIntervals(nrVPC(i) + 4 + j)],1);
//            if TSnew(1) > TS(i,1)
//                TS(i,:)=TSnew;
//                %k=j;
//            end
//        end
//    end
//    %TSfit = polyval(TS(k,1),[back+2+j back+j+5]);
//    %plot(TSfit);





    }

}
