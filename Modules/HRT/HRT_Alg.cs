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
        public List<int> TakeNonAtrialComplexes(List<Tuple<int,int>> _class)
        {
            List<int> Klasy = new List<int>();
            foreach (Tuple<int, int> _licznik in _class)
            {
                if (_licznik.Item2 == 1)
                {
                    Klasy.Add(_licznik.Item1);
                }
            }
            return Klasy;
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

        public void PrintVector(List<int> Signal)
        {
            foreach (int _licznik in Signal)
            {
                Console.WriteLine(_licznik);
            }
        }

        public void PrintVector(List<double[]> Signal)
        {
            foreach (double[] _licznik in Signal)
            {
                foreach (double _licznik2 in _licznik)
                {
                    Console.Write(_licznik2);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }

        public void PrintVector(List<double> Signal)
        {
            
            foreach (double _licznik2 in Signal)
            {
                Console.Write(_licznik2);
                Console.Write(" ");
            }
            Console.WriteLine();
            
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

        public List<int> GetNrVPC(double[] rrTimes, int[] rrTimesVPC)
        {
            int VPCcount = rrTimesVPC.Length;
            if (rrTimes == null || rrTimesVPC == null) throw new ArgumentNullException();
            if (rrTimes.Length < rrTimesVPC.Length) throw new ArgumentOutOfRangeException();
            if (VPCcount < 0) throw new ArgumentOutOfRangeException();
            int remove = 0;
            int[] nrVPCArray;
            bool[] missClass = new bool[VPCcount];
            nrVPCArray = new int[VPCcount];
            for (int i = 0; i < VPCcount; i++)
            {
                missClass[i] = true;
                for (int j = 0; j < rrTimes.Length; j++)
                {
                    if (rrTimesVPC[i] > (rrTimes[j] - 50) && rrTimesVPC[i] < (rrTimes[j] + 50)) 
                    { 
                        nrVPCArray[i] = j;
                        missClass[i] = false;
                    }
                }
                if (missClass[i])
                {
                    missClass[i] = true;
                    remove++;
                    Console.Write(i);
                    Console.Write("      ");
                    Console.WriteLine("błąd detekcji - nie ma piku odpowiadającego przypisaniu do klasy");
                }
            }

            nrVPCArray = missClassificationCorrection(missClass, nrVPCArray);
           
            nrVPCArray = removeRedundant(nrVPCArray, remove);
            //Vector<double> nrVPC = Vector<double>.Build.DenseOfArray(nrVPCArray);
            return nrVPCArray.ToList();
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

        public List<double[]> MakeTachogram(List<int> VPC, Vector<double> rrIntervals)
        {
            int back = 5;
            int forward = 15;
            int sum = back + forward;
            List <double[]> newTacho = new List<double[]>();
            List<Tuple<double,double>> VPCTachogram = new List<Tuple<double,double>>();
            int i = 0;
            foreach (int nrpikuVPC in VPC)
            {
                if ((nrpikuVPC - back) > 0 && ((nrpikuVPC + forward) < rrIntervals.Count))
                {
                    double[] temparray = new double[forward+back];
                    for (int k = (nrpikuVPC - back); k < (nrpikuVPC + forward); k++)
                    {
                        temparray[i++] = rrIntervals[k];
                    }
                    newTacho.Add(temparray);
                    i = 0;
                }
            }
            return newTacho;
        }

        public List<int> SearchPrematureTurbulences(List<double[]> Tachogram, List<int> numerPikuVC)
        {
            if (Tachogram.Capacity == numerPikuVC.Capacity) throw new ArgumentOutOfRangeException();

            double sumbefore = 0;
            double Mean=0;
            List<int> nrpikuPVC = new List<int>();
            for (int i = 0; i < Tachogram.Count; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    sumbefore = sumbefore + Tachogram[i][j];
                }
                Mean = (sumbefore / 4);
                if (Tachogram[i][4] < Mean * 0.8 && Tachogram[i][5] > Mean*0.8 && Tachogram[i][4] > 300 && Tachogram[i][5] < 2000)
                {
                   nrpikuPVC.Add(numerPikuVC[i]);
                }
                Mean = 0;
                sumbefore = 0;
            }
            return nrpikuPVC;
        }

        public int PrepareDataForGUI(List<double[]>d)
        {

            return 0;
        }

        public Tuple<double[], double[], double[,], double[,]> StatisticsToPDF(int[] VPC, List<double> rrIntervals)
        {

            int back = 5;
            int forward = 15;
            int sum = back + forward ;
            int[] nrprobe = new int[VPC.Length];

            double[] after = new double[VPC.GetLength(0)];
            double[] before = new double[VPC.GetLength(0)];
            double[] TO = new double[VPC.GetLength(0)];
            double[] TS = new double[VPC.GetLength(0)];
            int i = 0;
       
            
            double[,] xx = new double[VPC.Length, 5];
            double[,] yy = new double[VPC.Length, 5];

            foreach (int nrVPC in VPC)
            {
                if ((nrVPC - back) > 0 && ((nrVPC + forward) < rrIntervals.Capacity))
                {
                    after[i] = rrIntervals[nrVPC + 2] + rrIntervals[nrVPC + 3];
                    before[i] = rrIntervals[nrVPC - 2] + rrIntervals[nrVPC - 3];
                    TO[i] = 100 * (after[i] - before[i]) / before[i];
                    Tuple<double[], Vector<double>> p;
                    for (int j = 0; j < forward - 5; j++)
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
                                               
                        if (p.Item1[0] > TS[i])
                        {
                            TS[i] = p.Item1[0];
                            for (int n = 0; n < 5; n++)
                            {
                                xx[i, n] = j + n;
                                yy[i, n] = p.Item2.At(n);
                            }
                        }
                    }
                }
                i++;
            }
            return Tuple.Create(TO, TS, xx, yy);
        }

        public List<double> PrepareStatisticTurbulenceOnset(List<int> VPC, Vector<double> rrIntervals)  
        {
            int back = 5;
            int forward = 15;
            int sum = back + forward;

            double[] after = new double[VPC.Capacity];
            double[] before = new double[VPC.Capacity];
            List<double> TO = new List<double>();
            int i = 0;

            foreach (int nrVPC in VPC)
            {
                if ((nrVPC - back) > 0 && ((nrVPC + forward) < rrIntervals.Count))
                {
                    after[i] = rrIntervals[nrVPC + 2] + rrIntervals[nrVPC + 3];
                    before[i] = rrIntervals[nrVPC - 2] + rrIntervals[nrVPC - 3];
                    TO.Add(100 * (after[i] - before[i]) / before[i]);
                }
                i++;
            }
            return TO; 
        }

        public Tuple<List<double>,double[,],double[,]> PrepareStatisticAndGUITurbulenceSlope(List<int> VPC, Vector<double> rrIntervals)
        {
            int back = 5;
            int forward = 15;
            int sum = back + forward;

            double[] TS = new double[VPC.Count];
            int i = 0;


            double[,] xx = new double[VPC.Count, 5];
            double[,] yy = new double[VPC.Count, 5];

            foreach (int nrVPC in VPC)
            {
                if ((nrVPC - back) > 0 && ((nrVPC + forward) < rrIntervals.Count))
                {
                    
                    Tuple<double[], Vector<double>> p;
                    for (int j = 0; j < forward - 5; j++)
                    {
                        double[] xdata = new double[5];
                        double[] ydata = new double[5];
                        for (int n = 0; n < 5; n++)
                        {
                            xdata[n] = xdata[n] + (double)n;
                            ydata[n] = rrIntervals[nrVPC + j + n];
                        }
                        Vector<double> x = Vector<double>.Build.DenseOfArray(xdata);
                        Vector<double> y = Vector<double>.Build.DenseOfArray(ydata);
                        p = LinearSquare(x, y);

                        if (p.Item1[0] > TS[i])
                        {
                            TS[i] = p.Item1[0];
                            for (int n = 0; n < 5; n++)
                            {
                                xx[i, n] = j + n;
                                yy[i, n] = p.Item2.At(n);
                            }
                        }
                    }
                }
                i++;
            }
           List<double>TSnew = TS.ToList();
            return Tuple.Create(TSnew,xx, yy);
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
        //nie pisać testów bo nie nasza funkcja :)
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

    }
}
