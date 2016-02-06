using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using System.IO;
using MathNet.Numerics;
using EKG_Project.Modules.HRV_DFA;


namespace EKG_Project.Modules.HRT
{
    public class HRT_Alg
    {
        //Additional methods
        int front = 5;
        int back = 15;

        public void PrintVector(Vector<double> Signal)
        {
            foreach (double _licznik in Signal)
            {
                System.Diagnostics.Debug.WriteLine(_licznik);
            }
        }

        public void PrintVector(int[] Signal)
        {
            for (int i = 0; i < Signal.Length; i++)
            {
                System.Diagnostics.Debug.WriteLine(Signal[i]);
            }
        }

        public void PrintVector(double[] Signal)
        {
            for (int i = 0; i < Signal.Length; i++)
            {
                System.Diagnostics.Debug.WriteLine(Signal[i]);
            }
        }

        public void PrintVector(Tuple<double[], double[]> Signal)
        {
            foreach (double x in Signal.Item1)
            {
                System.Diagnostics.Debug.WriteLine(x);
            }
            foreach (double x in Signal.Item2)
            {
                System.Diagnostics.Debug.WriteLine(x);
            }
        }

        public void PrintVector(double[,] Signal)
        {
            for (int i = 0; i < Signal.GetLength(0); i++)
            {
                for (int j = 0; j < Signal.GetLength(1); j++)
                {
                    System.Diagnostics.Debug.WriteLine(Signal[i, j]);
                    System.Diagnostics.Debug.WriteLine(" ");
                }
                Console.WriteLine(";");
            }
        }

        public void PrintVector(List<int> Signal)
        {
            foreach (int _licznik in Signal)
            {
                System.Diagnostics.Debug.WriteLine(_licznik);
            }
        }

        public void PrintVector(List<double[]> Signal)
        {
            foreach (double[] _licznik in Signal)
            {
                foreach (double _licznik2 in _licznik)
                {
                    System.Diagnostics.Debug.WriteLine(_licznik2);
                    System.Diagnostics.Debug.WriteLine(" ");
                }
                Console.WriteLine();
            }
        }

        public void PrintVector(List<double> Signal)
        {

            foreach (double _licznik2 in Signal)
            {
                System.Diagnostics.Debug.WriteLine(_licznik2);
                System.Diagnostics.Debug.WriteLine(" ");
            }
            Console.WriteLine();

        }

        public void PrintVector(Tuple<int[], List<double[]>> Signal)
        {
            foreach (int _licznik in Signal.Item1)
            {
                System.Diagnostics.Debug.WriteLine(_licznik);
                System.Diagnostics.Debug.WriteLine(" ");
            }
            System.Diagnostics.Debug.WriteLine("");
            foreach (double[] _licznik in Signal.Item2)
            {
                PrintVector(_licznik);
            }
            System.Diagnostics.Debug.WriteLine("");
        }

        public void PrintVector(List<Tuple<string, int[], List<double[]>>> Signal)
        {
            foreach (Tuple<string, int[], List<double[]>> _licznik in Signal)
            {
                System.Diagnostics.Debug.WriteLine(_licznik.Item1);
                PrintVector(_licznik.Item2);
                PrintVector(_licznik.Item3);
            }
        }

        public void PrintVector(List<List<double>> Signal)
        {
            System.Diagnostics.Debug.WriteLine("");
            foreach (List<double> _licznik in Signal)
            {
                System.Diagnostics.Debug.WriteLine("");
                foreach (double _licznik2 in _licznik)
                {
                    System.Diagnostics.Debug.WriteLine(_licznik2);
                }

            }
        }

        public void PrintVector(Tuple<int[], double[]> Signal)
        {
            foreach (int _licznik in Signal.Item1)
            {
                System.Diagnostics.Debug.WriteLine(_licznik);
                System.Diagnostics.Debug.WriteLine(" ");
            }
            System.Diagnostics.Debug.WriteLine("");
            foreach (double _licznik in Signal.Item2)
            {
                System.Diagnostics.Debug.WriteLine(_licznik);
                System.Diagnostics.Debug.WriteLine(" ");
            }
            System.Diagnostics.Debug.WriteLine("");
        }

        public void PrintVector(Tuple<string, int[], double[]> Signal)
        {
            System.Diagnostics.Debug.WriteLine(Signal.Item1);
            System.Diagnostics.Debug.WriteLine("");
            foreach (int _licznik in Signal.Item2)
            {
                System.Diagnostics.Debug.WriteLine(_licznik);
                System.Diagnostics.Debug.WriteLine(" ");
            }
            System.Diagnostics.Debug.WriteLine("");
            foreach (double _licznik in Signal.Item3)
            {
                System.Diagnostics.Debug.WriteLine(_licznik);
                System.Diagnostics.Debug.WriteLine(" ");
            }
            System.Diagnostics.Debug.WriteLine("");


        }

        public void PrintVector(Tuple<List<double>, int[], double[]> Signal)
        {
            foreach (double _licznik2 in Signal.Item1)
            {
                System.Diagnostics.Debug.WriteLine(_licznik2);
                System.Diagnostics.Debug.WriteLine(" ");
            }
            System.Diagnostics.Debug.WriteLine("");
            foreach (int _licznik in Signal.Item2)
            {
                System.Diagnostics.Debug.WriteLine(_licznik);
                System.Diagnostics.Debug.WriteLine(" ");
            }
            System.Diagnostics.Debug.WriteLine("");
            foreach (double _licznik in Signal.Item3)
            {
                System.Diagnostics.Debug.WriteLine(_licznik);
                System.Diagnostics.Debug.WriteLine(" ");
            }
            System.Diagnostics.Debug.WriteLine("");

        }

        public void PrintVector(List<Tuple<int[], List<double[]>>> Signal)
        {
            foreach (Tuple<int[], List<double[]>> _licznik in Signal)
            {
                PrintVector(_licznik);
                System.Diagnostics.Debug.WriteLine("");
            }

        }

        public void PrintVector(List<Tuple<string, int[], double[]>> Signal)
        {

            foreach (Tuple<string, int[], double[]> _licznik in Signal)
            {
                System.Diagnostics.Debug.WriteLine(_licznik.Item1);
                PrintVector(_licznik.Item2);
                PrintVector(_licznik.Item3);
            }

        }

        public void PrintVector(List<Tuple<string, List<double>>> Signal)
        {
            foreach (Tuple<string, List<double>> _licznik in Signal)
            {
                System.Diagnostics.Debug.WriteLine(_licznik.Item1);
                PrintVector(_licznik.Item2);
            }
        }

        public void PrintVector(List<Tuple<string, double[], double[]>> Signal)
        {
            foreach (Tuple<string, double[], double[]> _licznik in Signal)
            {
                System.Diagnostics.Debug.WriteLine(_licznik.Item1);
                PrintVector(_licznik.Item2);
                PrintVector(_licznik.Item3);
            }
        }

        public void PrintVector(List<Tuple<double[], double[]>> Signal)
        {
            foreach (Tuple<double[], double[]> _licznik in Signal)
            {

                PrintVector(_licznik);
                System.Diagnostics.Debug.WriteLine("");

            }

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

        public Tuple<double[], double[], double[,], double[,]> StatisticsToPDF(int[] VPC, List<double> rrIntervals)
        {
            HRV_DFA_Alg externalAlg = new HRV_DFA_Alg();
            int back = 5;
            int forward = 15;
            int sum = back + forward;
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
                        Vector<double> x = Vector<double>.Build.DenseOfArray(xdata);
                        Vector<double> y = Vector<double>.Build.DenseOfArray(ydata);
                        p = externalAlg.LinearSquare(x, y);

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

        public double CountMean(double[] vec)
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

        //METHODS
        #region
        /// <summary>
        /// Function search for ventricular complex
        /// </summary>
        /// <param name="_class"> all peaks detected, with marked class;
        /// <returns> Ventricular complexes</returns>
        #endregion
        public List<int> TakeNonAtrialComplexes(List<Tuple<int, int>> _class)
        {


            List<int> Klasy = new List<int>();
            foreach (Tuple<int, int> _licznik in _class)
            {
                if (_licznik.Item2 == 0)
                {
                    Klasy.Add(_licznik.Item1);
                }
            }
            return Klasy;
        }

        #region
        /// <summary>
        /// Which peak is VPC
        /// </summary>
        /// <param name="rrTimes"> numbers of probes of peaks 
        /// <param name="rrTimesVPC"> numbers of probes of peaks R classified as VPC
        /// <returns> Which R peak was classified as Ventricular complex</returns>
        #endregion
        public List<int> GetNrVPC(double[] rrTimes, int[] rrTimesVPC)
        {
            int VPCcount = rrTimesVPC.Length;
            if (rrTimes == null) throw new ArgumentNullException();
            if (rrTimesVPC.Length == 0) throw new ArgumentOutOfRangeException();
            if (rrTimes.Length < rrTimesVPC.Length) throw new ArgumentOutOfRangeException();
            if (VPCcount < 0) throw new ArgumentOutOfRangeException();

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

        /// <summary>
        /// Create list of tachograms
        /// </summary>
        /// <param name="VPC"> numbers of probes of peaks R classified as VPC
        /// <param name=" rrIntervals"> intevals between R peaks
        /// <returns> Tachograms</returns>
        public List<List<double>> MakeTachogram(List<int> VPC, Vector<double> rrIntervals)
        {
            int back = 5;
            int forward = 15;
            int sum = back + forward;
            List<List<double>> newTacho = new List<List<double>>();
            foreach (int nrpikuVPC in VPC)
            {
                if ((nrpikuVPC - back) > 0 && ((nrpikuVPC + forward) < rrIntervals.Count))
                {
                    //double[] temparray = new double[forward + back];
                    List<double> singleTacho = new List<double>();
                    for (int k = (nrpikuVPC - back); k < (nrpikuVPC + forward); k++)
                    {
                        singleTacho.Add(rrIntervals[k]);
                    }
                    newTacho.Add(singleTacho);
                }
            }
            return newTacho;
        }

        /// <summary>
        /// Which complexes are premature
        /// </summary>
        /// <param name="Tachogram"> list of tachograms
        /// <param name="  numerPikuVC"> Which R peak was classified as VPC
        /// <returns> Which R peak was classified as VPC</returns>
        public List<int> SearchPrematureTurbulences(List<List<double>> Tachogram, List<int> numerPikuVC)
        {
            if (Tachogram.Count != numerPikuVC.Count) throw new ArgumentOutOfRangeException();

            double sumbefore = 0;
            double Mean = 0;
            List<int> nrpikuPVC = new List<int>();
            for (int i = 0; i < Tachogram.Count; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    sumbefore = sumbefore + Tachogram[i][j];
                }
                Mean = (sumbefore / 4);
                if (Tachogram[i][4] < Mean * 0.8 && Tachogram[i][5] > Mean * 0.8 && Tachogram[i][4] > 300 && Tachogram[i][5] < 2000)
                {
                    nrpikuPVC.Add(numerPikuVC[i]);
                }
                Mean = 0;
                sumbefore = 0;
            }
            return nrpikuPVC;
        }

        /// <summary>
        /// Prepare List of Turbulence Onsets to print
        /// </summary>
        /// <param name="VPC"> numbers of probes of peaks R classified as VPC
        /// <param name="rrIntervals"> ntevals between R peaks
        /// <returns> List of Turbulence Onset</returns>
        public List<double> TurbulenceOnsetsPDF(List<int> VPC, Vector<double> rrIntervals)
        {

            int sum = back + front;

            double[] after = new double[VPC.Capacity];
            double[] before = new double[VPC.Capacity];
            List<double> TO = new List<double>();
            int i = 0;

            foreach (int nrVPC in VPC)
            {

                after[i] = rrIntervals[nrVPC + 2] + rrIntervals[nrVPC + 3];
                before[i] = rrIntervals[nrVPC - 2] + rrIntervals[nrVPC - 3];
                TO.Add(Math.Round((100 * (after[i] - before[i]) / before[i]), 2));

                i++;
            }
            return TO;
        }


        /// <summary>
        /// Prepare points to plot of Mean Turbulece Onset
        /// </summary>
        /// <param name="tacho"> tachograms
        /// <returns> x and y coordinates to plot</returns>
        public Tuple<int[], double[]> TurbulenceOnsetMeanGUI(List<List<double>> tacho)
        {
            List<double> listBefore3 = new List<double>();
            List<double> listBefore4 = new List<double>();
            List<double> listAfter7 = new List<double>();
            List<double> listAfter8 = new List<double>();

            double sumBefore3Suma = 0;
            double sumBefore4Suma = 0;
            double sumAfter7Suma = 0;
            double sumAfter8Suma = 0;

            foreach (List<double> tach in tacho)
            {
                listBefore3.Add(tach[2]);
                listBefore4.Add(tach[3]);
                listAfter7.Add(tach[6]);
                listAfter8.Add(tach[7]);
            }
            sumBefore3Suma = listBefore3.Sum() / tacho.Count;
            sumBefore4Suma = listBefore4.Sum() / tacho.Count;
            sumAfter7Suma = listAfter7.Sum() / tacho.Count;
            sumAfter8Suma = listAfter8.Sum() / tacho.Count;


            double sumBefore = Math.Round(((sumBefore3Suma + sumBefore4Suma) / 2), 2);
            double sumAfter = Math.Round(((sumAfter7Suma + sumAfter8Suma) / 2), 2);


            int[] x = { -2, -1, 2, 3 };
            double[] y = { sumBefore, sumBefore, sumAfter, sumAfter };
            return Tuple.Create(x, y);

        }


        /// <summary>
        /// Return values of turbulence slope (max linear regresion) from every channel and prepare coordinates to plot max TS
        /// </summary>
        /// <param name="VPC"> numbers of probes of peaks R classified as VPC
        /// <param name="rrIntervals"> intevals between R peaks
        /// <returns>  Return values of turbulence slope (max linear regresion) from every channel and prepare coordinates to plot max TS</returns>
        public Tuple<List<double>, int[], double[]> TurbulenceSlopeGUIandPDF(List<int> VPC, Vector<double> rrIntervals)
        {
            HRV_DFA_Alg externalAlg = new HRV_DFA_Alg();
            double[] TS = new double[VPC.Count];
            for (int j = 0; j < VPC.Count; j++)
            {
                TS[j] = -1000;
            }
            int i = 0;


            int[] xx = new int[5];
            double[] yy = new double[5];

            foreach (int nrVPC in VPC)
            {
                Tuple<double[], Vector<double>> p;
                for (int j = 0; j < front; j++)
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
                    p = externalAlg.LinearSquare(x, y);

                    if (p.Item1[0] > TS[i])
                    {
                        TS[i] = p.Item1[0];
                        for (int n = 0; n < 5; n++)
                        {
                            xx[n] = j + n;
                            yy[n] = Math.Round(p.Item2.At(n), 2);
                        }
                    }
                }
                i++;
            }
            List<double> TSnew = TS.ToList();
            return Tuple.Create(TSnew, xx, yy);
        }


        /// <summary>
        /// Create 
        /// </summary>
        /// <returns>  array of indexes for xaxis to plot  
        public int[] xPlot()
        {
            int[] xaxis = new int[back + front];
            for (int i = 0; i < 20; i++)
            {
                xaxis[i] = i - 4;
            }
            return xaxis;
        }

        /// <summary>
        /// Return average tachogram from all tachograms to plot 
        /// </summary>
        /// <param name="tacho">tachogram
        /// <returns>   Return average tachogram from all tachograms /returns>
        public double[] MeanTachogram(List<List<double>> tacho)
        {
            double sumTach = 0;
            double[] meanTach = new double[back + front];
            int k = 0;
            for (int i = 0; i < (back + front); i++)
            {
                foreach (List<double> tach in tacho)
                {
                    sumTach = sumTach + tach[k];
                }

                meanTach[k] = Math.Round((sumTach / tacho.Count), 2);
                k++;
                sumTach = 0;
            }

            return meanTach;
        }

    }
}