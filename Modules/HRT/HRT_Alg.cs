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

        public void PrintVector(Vector<double> Signal)
        {
            foreach (double _licznik in Signal)
            {
                Console.WriteLine(_licznik);
            }
        }

        public void PrintVector(int[] Signal)
        {
            for (int i = 0; i < Signal.Length; i++)
            {
                Console.WriteLine(Signal[i]);
            }
        }

        public void PrintVector(double[] Signal)
        {
            for (int i = 0; i < Signal.Length; i++)
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
            for (int i = 0; i < rrPeaks.Count; i++)
            {
                rrPeaks[i]++;
            }
            return rrPeaks;
        }

        public int[] checkVPCifnotNULL(int[] rrTimesVPC)
        {
            int numToRemove = 0;
            rrTimesVPC = rrTimesVPC.Where(val => val != numToRemove).ToArray();
            return rrTimesVPC;
        }

        public int[] missClassificationCorrection(bool[] miss, int[] klasa)
        {
            int k = 0;
            int[] newklasa = new int[klasa.Length];
            for (int i = 0; i < klasa.Length - 1; i++)
            {
                if (!miss[i])
                {
                    newklasa[k] = klasa[i];
                    k++;
                }
            }
            return newklasa;
        }

        public int[] GetNrVPC(double[] rrTimes, int[] rrTimesVPC, int VPCcount)
        {
            if (rrTimes == null || rrTimesVPC == null) throw new ArgumentNullException();
            if (rrTimes.Length < rrTimesVPC.Length) throw new ArgumentOutOfRangeException();
            if (rrTimesVPC.Length < VPCcount || VPCcount < 0) throw new ArgumentOutOfRangeException();
            int remove = 0;
            int[] nrVPCArray;
            bool[] missClass = new bool[VPCcount];
            nrVPCArray = new int[VPCcount];
            for (int i = 0; i < VPCcount - 1; i++)
            {
                missClass[i] = true;
                for (int j = 0; j < rrTimes.Length - 1; j++)
                {
                    if (rrTimes[j] == rrTimesVPC[i])
                    {
                        nrVPCArray[i] = j;
                        //Console.Write(i);
                        //Console.Write("      ");
                        //Console.Write(j);
                        //Console.Write("      ");
                        //Console.Write(rrTimes[j]);
                        //Console.Write("      ");
                        //Console.WriteLine(rrTimesVPC[i]);
                        missClass[i] = false;

                    }
                }
                if (missClass[i])
                {
                    missClass[i] = true;
                    remove++;
                }
                    //{
                    //Console.Write(i);
                    //Console.Write("      ");
                    //Console.WriteLine("błąd detekcji - nie ma piku odpowiadającego przypisaniu do klasy");


                    //}
                }
             nrVPCArray = missClassificationCorrection(missClass, nrVPCArray);
            nrVPCArray = removeRedundant(nrVPCArray, remove);
        //Vector<double> nrVPC = Vector<double>.Build.DenseOfArray(nrVPCArray);
        return nrVPCArray;
        }

        public int[] removeRedundant(int[] array, int ile)
        {
            int k = 0;
            int arraysize = array.Length;
            int size = arraysize - ile;

            int[] newarray = new int[size];
            {
                for (int i = 0; i < size -1; i++)
                {
                    if (array[i] != 0)
                    {
                        newarray[k] = array[i];
                        k++;
                    }
                }
                return newarray;
            }
        }

        public Tuple<double[,], double[]> MakeTachogram(int[] VPC, double[] rrIntervals)
        {
            int back = 5;
            int foward = 15;
            int sum = back + foward + 1;
            
            double[] after = new double[VPC.GetLength(0)];
            double[] before = new double[VPC.GetLength(0)];
            double[] TO = new double[VPC.GetLength(0)];
            int i = 0;
            double[,] VPCTachogram = new double[sum, VPC.GetLength(0)-1];
            foreach (int nrVPC in VPC)
            {
               if ((nrVPC - back) > 0 && ((nrVPC + foward) < rrIntervals.Length))
                    {
                        for (int k = (nrVPC - back); k < (nrVPC + foward + 1); k++)
                        { 
                                VPCTachogram[k + back - nrVPC, i] = rrIntervals[k];
                                
                            }
                    }
                    if ((nrVPC - 2) > 0 && ((nrVPC + 2) < rrIntervals.Length))
                    {
                        after[i] = rrIntervals[nrVPC + 2] + rrIntervals[nrVPC + 3];
                        before[i] = rrIntervals[nrVPC - 2] + rrIntervals[nrVPC - 3];
                        TO[i] = (after[i] - before[i]) / before[i];
                    }
                    i++;
                }
            //Console.WriteLine(i);
            return Tuple.Create(VPCTachogram, TO);
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
