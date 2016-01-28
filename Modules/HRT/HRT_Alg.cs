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
            for (int i = 0; i < klasa.Length; i++)
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
            for (int i = 0; i < VPCcount; i++)
            {
                missClass[i] = true;
                for (int j = 0; j < rrTimes.Length; j++)
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
            if (length < 0 || length >= array.Length) throw new ArgumentOutOfRangeException();

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
            int[] nrprobe = new int[VPC.Length];

            double[] after = new double[VPC.GetLength(0)];
            double[] before = new double[VPC.GetLength(0)];
            double[] TO = new double[VPC.GetLength(0)];
            double[] TS = new double[VPC.GetLength(0)];
            Tuple<double[,], double[,]> SlopeVisualization;
            int i = 0;
            double[,] VPCTachogram = new double[sum, VPC.GetLength(0)];

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
                    Tuple<double[], Vector<double>> p;
                    for (int j = 0; j < foward - 5; j++)
                    {
                        double[] xdata = new double[5];
                        double[] ydata = new double[5];
                        for (int n = 0; n < 5; n++)
                        {
                            xdata[n] = xdata[n] + (double)n;
                            ydata[n] = rrIntervals[nrVPC + j + n];
                        }
                        Vector<double>  x = Vector<double>.Build.DenseOfArray(xdata);
                        Vector<double>  y = Vector<double>.Build.DenseOfArray(ydata);
                        p = LinearSquare(x, y);
                        
                        //SlopeVisualization = LinearSquarePrepareResults(p.Item2, j);
                        //PrintVector(p.Item2.);
                        
                        if (p.Item1[0] > TS[i])
                        {
                            TS[i] = p.Item1[0];
                            //nrprobe[i] = j;
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


        //od pauliny, jak będzie na głównym repo to zrobić odwołania do jej implementacji
        public Tuple<double[], Vector<double>> LinearSquare(Vector<double> x, Vector<double> y)
        {
            Vector<double> y_approx = Vector<double>.Build.Dense(y.Count());
            double[] P = new double[2];
            double n = y.Count();
            // Subsidiary variables
            double sumX = x.Sum();
            double sumX2 = x.PointwiseMultiply(x).Sum();
            double sumY = y.Sum();
            Vector<double> xy = x.PointwiseMultiply(y);
            double sumXY = (xy).Sum();
            // Coeffitients for y = pa * x + pb
            double pa = (n * sumXY - sumX * sumY) / (n * sumX2 - Math.Pow(sumX, 2));
            double pb = (sumY * sumX2 - sumX * sumXY) / (n * sumX2 - Math.Pow(sumX, 2));
            // Generate interpolated line 
            for (int i = 0; i < y_approx.Count(); i++)
            {
                y_approx[i] = pa * x[i] + pb;
            }

            P[0] = pa;
            P[1] = pb;

            Tuple<double[], Vector<double>> output = new Tuple<double[], Vector<double>>(P, y_approx);

            return output;
        }


        #region
        /// <summary>
        /// Function that prepare vector for IO module, to visualise VPC tachogram Slope
        /// </summary>
        /// <param name="wektor"> values of tachogramu (y values)</param>
        /// <param name="nrProbki"> nr of probes (x values) </param>
        /// <returns> tuple with two arrays: first with 5 values in row where highest slope was detected; second with values of tachogram, </returns>
        #endregion
        public Tuple<double[,], double[,]> LinearSquarePrepareResults(Vector<double>wektor, double nrProbki)
        {
            double[,] x = new double[5,wektor.Count / 5];
            double[,] y = new double[5, wektor.Count / 5];
            int k = 0;
            for (int j = 0; j < wektor.Count / 5; j++)
            {
                for (int i = 0; i < 5; i++)
                {
                    x[j, i] = i+nrProbki;
                    y[j, i] = wektor[k++];
                }
            }

            Tuple<double[,], double[,]> wynik =  Tuple.Create(x, y);
            return wynik;
        }

    }
}
