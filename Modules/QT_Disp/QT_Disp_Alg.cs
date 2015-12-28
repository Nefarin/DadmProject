using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Differentiation;
using EKG_Project.IO;
using MathNet.Numerics.Statistics;


namespace EKG_Project.Modules.QT_Disp
{
    public partial class QT_Disp : IModule
    {
        static void Main()
        {

            /* Console.WriteLine("Test");
             TempInput.setInputFilePath("D:\\DADM_Project\\dane.txt");
             TempInput.setOutputFilePath("D:\\DADM_Project\\Rn_100.txt");
             uint sda = TempInput.getFrequency();
             Console.WriteLine("Fs: " + sda);
             //List<double> RR = TempInput.getSignal();
             //List<double> signal = (RR.ToList());
             Console.WriteLine("Signal length: " + signal.Count);

             TWave samp = new TWave(signal);
             samp.DeleteQRSWave();
             List<int> wyniki = samp.FindT_Max();
             foreach (int a in wyniki)
             {
                 Console.WriteLine(a);
             }
            // TempInput.writeFile(360, wyniki);

             Console.ReadKey();*/
            Vector<double> sig = Vector<double>.Build.Dense(10);
            
            Console.WriteLine("Hello");

        }


    }
    /// <summary>
    /// This class is created to implements algorithms to find T_end in signal
    /// </summary>
    class TWave
    {
        //private double Fs;
        String drainName;
        T_End_Method method;

        private List<int> QRS_Onset;
        private List<int> QRS_End;
        private List<int> P_Onset;
        private List<int> RR_Interval;
        public List<double> signal;


        /// <summary>
        /// THis constructor set fields of a class
        /// </summary>
        /// <param name="Signal">This is a signal from ECG_baseline module</param>
        /// <param name="QRS_Onset">Ths vector contains QRS_Onset indexes</param>
        /// <param name="P_Onset">Ths vector contains P_Onset indexes</param>
        /// <param name="QRS_End">Ths vector contains QRS_End indexes</param>
        /// <param name="RR_Interval">Ths vector contains RR ntervals between next R peaks</param>
        /// <param name="drainName">This argument is a name of a drain</param>
        /// <param name="method">This argument is a method to find T_End in a signal</param>
        public TWave(List<double> Signal, List<int> QRS_Onset, List<int> P_Onset, List<int> QRS_End, List<int> RR_Interval, String drainName, T_End_Method method)
        {
            //this.Fs = Fs;
            this.drainName = drainName;
            this.method = method;

            this.signal = Signal;
            this.QRS_Onset = QRS_Onset;
            this.QRS_End = QRS_End;
            this.RR_Interval = RR_Interval;
            this.P_Onset = P_Onset;

        }
        public TWave(List<double> signal)
        {
            //for testes
            this.signal = signal;
            int[] tab = { 167, 576 };
            int[] tab2 = { 203, 613 };
            int[] tab3 = { 185, 288 };
            List<int> temp1;
            List<int> temp2;
            List<int> temp3;
            temp1 = tab.ToList();
            temp2 = tab2.ToList();
            temp3 = tab3.ToList();
            this.QRS_Onset = temp1;
            this.QRS_End = temp2;
            this.RR_Interval = temp3;



        }

        /// <summary>
        /// This function is responsible for finding QRS-Waves in signal and delete them.
        /// </summary>
        public void DeleteQRSWave()
        {
            if (QRS_Onset.ElementAt(0) < QRS_End.ElementAt(0))
            {
                for (int i = 0; i < QRS_End.Count(); i++)
                {

                    int difference = QRS_End.ElementAt(i) - QRS_Onset.ElementAt(i);
                    List<double> zeros = new List<double>(difference);
                    for (int j = 0; j < difference; j++)
                    {
                        zeros.Insert(j, 0);
                    }
                    signal.RemoveRange(QRS_Onset.ElementAt(i), difference);
                    signal.InsertRange(QRS_Onset.ElementAt(i), zeros);
                    //Vector<double> Zeros = Vector<double>.Build.Dense(difference);
                    //signal.SetSubVector((int)QRS_Onset.At(i), difference, Zeros);

                    zeros.Clear();
                }
            }



        }
        /// <summary>
        /// This function finds T-Max indexes in signal
        /// </summary>
        /// <returns>Vector that contains T_Max indexes</returns>
        public List<int> FindT_Max()
        {
            List<int> T_MaxIndexList = new List<int>();
            T_MaxIndexList.Clear();
            //double start = RR_Interval.At(0);
            for (int i = 0; i < QRS_Onset.Count - 1; i++)
            {
                List<double> temp = new List<double>();
                if (QRS_Onset.ElementAt(i) != -1 && QRS_Onset.ElementAt(i + 1) != -1)
                {
                    temp = signal.GetRange(QRS_Onset.ElementAt(i), QRS_Onset.ElementAt(i + 1) - QRS_Onset.ElementAt(i));
                    T_MaxIndexList.Add(QRS_Onset.ElementAt(i) + temp.IndexOf(temp.Min()));
                    // T_MaxIndexList.Add((double)(signal.SubVector((int)QRS_Onset.ElementAt(i), (int)QRS_Onset.ElementAt(i + 1)).MaximumIndex()) + QRS_Onset.ElementAt(i));

                }
                else
                {
                    T_MaxIndexList.Add(-1);
                }
                temp.Clear();
            }

            return T_MaxIndexList;
        }
        /// <summary>
        /// This function finds max slope indexes in signal after T_max index
        /// </summary>
        ///  /// <returns>Vector that contains Max Slope indexes</returns>
        private List<int> Find_MaxSlope()
        {
            List<int> T_MaxIndex = FindT_Max();
            List<int> T_MaxSlope = new List<int>();
            List<double> temp = new List<double>();
            if (P_Onset.ElementAt(0) > T_MaxIndex.ElementAt(0))
            {

                for (int i = 0; i < T_MaxIndex.Count; i++)
                {
                    if (P_Onset.ElementAt(i) != -1 && T_MaxIndex.ElementAt(i) != -1)
                    {
                        temp = signal.GetRange(T_MaxIndex.ElementAt(i), P_Onset.ElementAt(i) - T_MaxIndex.ElementAt(i));
                        T_MaxSlope.Add(T_MaxIndex.ElementAt(i) + diff(temp).IndexOf(diff(temp).Min()));
                        //T_EndIndexList.Add(diff(signal.SubVector((int)P_Onset.At(i), (int)(P_Onset.At(i) - T_MaxIndex.At(i)))).MinimumIndex());

                    }
                    else
                    {
                        T_MaxSlope.Insert(i, -1);
                    }


                }


            }
            return T_MaxSlope;



        }
        /// <summary>
        /// This method find T_End inexes according to tangent method
        /// </summary>
        /// <returns>Vector that contain T_End indexes</returns>
        //private Vector<double> TangentMethod()
        //{

        //}

        /// <summary>
        /// This method find T_End indexes according to parabola method
        /// </summary>
        /// <returns>Vector that contains T_End indexes</returns>
        //private Vector<double> ParabolaMethod()
        //{

        //}

        /// <summary>
        /// This function calculate the difference between the next elements in vector
        /// </summary>
        /// <param name="In">Vector to differitive</param>
        /// <returns>Vector after differentiation</returns>
        private List<double> diff(List<double> In)
        {
            List<double> Output = new List<double>(In.Count - 1);


            for (int i = 0; i < In.Count - 1; i++)
            {
                Output.Insert(i, In.ElementAt(i + 1) - In.ElementAt(i));
            }

            return Output;
        }
        /// <summary>
        /// This function execute all methods to find a T_End indexes in a signal
        /// </summary>
        /// <returns>Vector that contains indexes of T_End according to chose method</returns>
        public List<int> ExecuteTWave()
        {
            DeleteQRSWave();
            return FindT_Max();
        }

    }

    /// <summary>
    /// This class keep all data to calculate QT interval - no methods
    /// </summary>
    public class QT_Data
    {

        public List<int> QRS_Onset;
        public List<int> T_End;
        public List<double> RR_Interval;
        public double Fs;
        public String drainName;
        public QT_Calc_Method method;

        /// <summary>
        /// This constructor creates object that consist of all vectors needed to calculate a QT interval stats and QT disp
        /// </summary>
        /// <param name="QRS_Onset"></param>
        /// <param name="T_End"></param>
        /// <param name="RR_Interval"></param>
        /// <param name="Fs"></param>
        /// <param name="drainName"></param>
        /// <param name="method"></param>
        public QT_Data(List<int> QRS_Onset, List<int> T_End, List<double> RR_Interval, double Fs, String drainName, QT_Calc_Method method)
        {
            this.QRS_Onset = QRS_Onset;
            this.T_End = T_End;
            this.RR_Interval = RR_Interval;
            this.Fs = Fs;
            this.drainName = drainName;
            this.method = method;

        }

    }
    /// <summary>
    /// This class is to calculate a QT_disp(both local and global), mean and std of QT intervals
    /// </summary>
    class QTCalculation
    {
        List<Tuple<string, double>> meanQTInterval;
        List<Tuple<string, double>> stdQTInterval;
        List<Tuple<string, double>> localQTdisp;
        List<String> drainName;
        double globalQTDisp;
        List<QT_Data> DataToOperate;            //size of List tells us the amount of drains.
        int AmountOFDrains;



        public QTCalculation(List<QT_Data> DataToOperate)
        {
            this.DataToOperate = DataToOperate;
            this.AmountOFDrains = this.DataToOperate.Count;

        }

        public void setOutput()
        {
            List<double> zero = new List<double>(1);
            zero.Add(0);
            List<double> QTIntervals = new List<double>();
            double[] mean = new double[this.AmountOFDrains];
            double[] std = new double[this.AmountOFDrains];
            double[] local = new double[this.AmountOFDrains];
            int j = 0;


            foreach (QT_Data drain in DataToOperate)
            {
                if (drain.method == QT_Calc_Method.BAZETTA)
                {
                    if (drain.QRS_Onset.ElementAt(0) < drain.T_End.ElementAt(0))
                    {
                        for (int i = 0; i < drain.QRS_Onset.Count; i++)
                        {
                            if (drain.QRS_Onset.ElementAt(i) != -1 && (drain.T_End.ElementAt(i) != -1))
                            {
                                QTIntervals.Add(((drain.T_End.ElementAt(i) - drain.QRS_Onset.ElementAt(i)) / drain.Fs) / Math.Sqrt(drain.RR_Interval.ElementAt(i)));
                            }
                            else
                            {
                                QTIntervals.Add(0);
                            }

                        }
                    }
                }
                if (drain.method == QT_Calc_Method.FRIDERICA)
                {
                    if (drain.QRS_Onset.ElementAt(0) < drain.T_End.ElementAt(0))
                    {
                        for (int i = 0; i < drain.QRS_Onset.Count; i++)
                        {
                            if (drain.QRS_Onset.ElementAt(i) != -1 && (drain.T_End.ElementAt(i) != -1))
                            {
                                QTIntervals.Add(((drain.T_End.ElementAt(i) - drain.QRS_Onset.ElementAt(i)) / drain.Fs) / Math.Pow((drain.RR_Interval.ElementAt(i)), 1 / 3));
                            }
                            else
                            {
                                QTIntervals.Add(0);
                            }
                        }
                    }
                }
                if (drain.method == QT_Calc_Method.FRAMIGHAMA)
                {
                    if (drain.QRS_Onset.ElementAt(0) < drain.T_End.ElementAt(0))
                    {
                        for (int i = 0; i < drain.QRS_Onset.Count; i++)
                        {
                            if (drain.QRS_Onset.ElementAt(i) != -1 && (drain.T_End.ElementAt(i) != -1))
                            {
                                QTIntervals.Add(((drain.T_End.ElementAt(i) - drain.QRS_Onset.ElementAt(i)) / drain.Fs) + 0.154 * (1 - (drain.RR_Interval.ElementAt(i))));
                            }
                            else
                            {
                                QTIntervals.Add(0);
                            }

                        }
                    }
                }

                mean[j] = QTIntervals.Sum() / (QTIntervals.Except(zero)).Count();
                local[j] = QTIntervals.Max() - QTIntervals.Min();
                std[j] = (QTIntervals.Except(zero)).StandardDeviation();
                j++;
                QTIntervals.Clear();
            }
            for (int i = 0; i < AmountOFDrains; i++)
            {
                this.localQTdisp.Insert(i, Tuple.Create(drainName.ElementAt(i), local.ToList().ElementAt(i)));
                this.meanQTInterval.Insert(i, Tuple.Create(drainName.ElementAt(i), mean.ToList().ElementAt(i)));
                this.stdQTInterval.Insert(i, Tuple.Create(drainName.ElementAt(i), std.ToList().ElementAt(i)));

            }

        }

    }
}
