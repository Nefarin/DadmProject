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

        public int[] removeRedundant(int[] array, int length)
        {
            if (array == null) throw new ArgumentNullException();
            if (length <= 0 || length >= array.Length) throw new ArgumentOutOfRangeException();

            int arraySize = array.Length;
            int newArraySize = arraySize - length;

            int[] newArray = new int[newArraySize];

            for (int i = 0; i < newArraySize; i++)
            {
                newArray[i] = array[i];
            }
            return newArray;
        }

        public Tuple<double[,], double[], double[]> MakeTachogram(int[] VPC, double[] rrIntervals)
        {
            int back = 5;
            int foward = 15;
            int sum = back + foward ;

            double[] after = new double[VPC.GetLength(0)-1];
            double[] before = new double[VPC.GetLength(0)-1];
            double[] TO = new double[VPC.GetLength(0)-1];
            double[] TS = new double[VPC.GetLength(0)-1];
            int i = 0;
            double[,] VPCTachogram = new double[sum, VPC.GetLength(0)-1];

            foreach (int nrVPC in VPC)
            {

                if ((nrVPC - back) > 0 && ((nrVPC + foward) < rrIntervals.Length))
                {
                    for (int k = (nrVPC - back); k < (nrVPC + foward); k++)
                    {
                        VPCTachogram[k + back - nrVPC, i] = rrIntervals[k];
                    }
                    after[i] = rrIntervals[nrVPC + 2] + rrIntervals[nrVPC + 3];
                    before[i] = rrIntervals[nrVPC - 2] + rrIntervals[nrVPC - 3];
                    TO[i] = (after[i] - before[i]) / before[i];
                    Tuple<double, double> p;
                    for (int j = 0; j < foward - 5; j++)
                    {
                        double[] xdata = new double[5];
                        double[] ydata = new double[5];
                        for (int n = 0; n < 5; n++)
                        {
                            xdata[n] = xdata[n] + (double)n;
                            ydata[n] = rrIntervals[nrVPC + j + n];
                        }
                        p = Fit.Line(xdata, ydata);
                        if (p.Item2 > TS[i])
                        {
                            TS[i] = p.Item2;
                        }
                    }
                }
                i++;
            }
            return Tuple.Create(VPCTachogram, TO, TS);
        }

        public double CountMean (double[] vec)
        {
            double suma = vec.Sum();
            double dlugosc = vec.Length;
            double mean = suma / dlugosc;
            return mean;
        }

        public double CountMax(double[] vec)
        {
            double max;
            return max = vec.Max();
        }

        public double CountMin(double[] vec)
        {
            double min;
            return min = vec.Min();
        }
    }
}
