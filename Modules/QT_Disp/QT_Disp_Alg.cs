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

             Console.WriteLine("Test");
            /* TempInput.setInputFilePath("D:\\Dadm_nowy\\dane.txt");
             TempInput.setOutputFilePath("D:\\Dadm_nowy\\rn100.txt");
             uint sda = TempInput.getFrequency();
             Console.WriteLine("Fs: " + sda);
             Vector<double> RR = TempInput.getSignal();
             List<double> signal = (RR.ToList());
             Console.WriteLine("Signal length: " + RR.Count);

             TWave samp = new TWave(RR);
             samp.DeleteQRSWave();
             TempInput.writeFile(sda, samp.signal);
            List<int> wyniki = samp.ExecuteTWave();
            Console.WriteLine(wyniki.Count);
           
            
             foreach (int a in wyniki)
             {
                 Console.WriteLine(a);
             }
            // TempInput.writeFile(360, wyniki);

             
            //Vector<double> sig = Vector<double>.Build.Dense(10);*/
             QT_Data test = new QT_Data();
             List<QT_Data> test_list = new List<QT_Data>();
             test_list.Add(test);
             QTCalculation output = new QTCalculation(test_list);
             output.setOutput();

            /*
            TempInput.setInputFilePath("D:\\Dadm_nowy\\EKG.txt");
            Vector<double> signalfile = TempInput.getSignal();
            TWave signal = new TWave(signalfile);
            // double[] test = { 2, -1, 3.5, -4, 4, 5, 6, -7, -8.9, 500, 10, 0, 11 };
            // Vector<double> testvec = Vector<double>.Build.DenseOfArray(test);
            List<int> wynik = signal.ExecuteTWave();
            
            /*Console.WriteLine("Hello");
            double[] x= new double[10];
            double[] y = new double[10];
            for (int i = 0; i < 10; i++)
            {
                x[i] = i;
                y[i] = i * i + 2;
            }
            double[] wynik = MathNet.Numerics.Fit.Polynomial(x, y, 2);
            Console.WriteLine("Size" +wynik.Count());
            Console.WriteLine(wynik.ElementAt(0) +"   "+ wynik.ElementAt(1) +"      "+ wynik.ElementAt(2));*/
            Console.ReadKey();

        }


    }
    #region Documentation
    /// <summary>
    /// This class is created to implements algorithms to find T_end in signal
    /// </summary>
    #endregion 
    class TWave
    {
        //
        String drainName;
        T_End_Method method;

       
        private List<int> QRS_End;
        private List<int> P_Onset;
        private List<int> T_EndGlobal;
       
        public Vector<double> signal;

        #region Documentation
        /// <summary>
        /// This constructor set fields of a class
        /// </summary>
        /// <param name="Signal">This is a vector signal from ECG_baseline module</param>
        /// <param name="T_EndGlobal">This list contains T_End indexes</param>
        /// <param name="P_Onset">This list contains P_Onset indexes</param>
        /// <param name="QRS_End">This list contains QRS_End indexes</param>       
        /// <param name="drainName">This argument is a name of a drain</param>
        /// <param name="method">This argument is a method to find T_End in a signal</param>
        #endregion
        public TWave(Vector<double> Signal, List<int> P_Onset, List<int> QRS_End, List<int> T_EndGlobal,  String drainName, T_End_Method method)
        {           
            this.drainName = drainName;
            this.method = method;

            this.signal = Signal;          
            this.QRS_End = QRS_End;           
            this.P_Onset = P_Onset;
            this.T_EndGlobal = T_EndGlobal;
        }
        /// <summary>
        /// Only for test
        /// </summary>
        /// <param name="signal"></param>
        public TWave(Vector<double> signal)
        {
            //for testes
            this.signal = signal;
            int[] ponset = { 295, 588,863,1151,1434,1728 };
            int[] qrsend = { 96, 380,676,960,1244,1528 };
            int[] tend = { 265, 553,835,1119,1410,1692 };        
           
            this.QRS_End = qrsend.ToList();
            this.drainName = "V2";
            this.P_Onset = ponset.ToList();           
            this.T_EndGlobal = tend.ToList();
        }
       
        #region Documentation
        /// <summary>
        /// This function finds T-Max indexes in signal
        /// </summary>
        /// <returns>List that contains T_Max indexes</returns>
        #endregion
        private List<int> FindT_Max()
        {
            List<int> T_MaxIndexList = new List<int>();
            //check if we have a correct index 
            //try to do else statements!
            if (QRS_End.ElementAt(0) < T_EndGlobal.ElementAt(0))
            {
                //this for goes for every T_end in signal
                for (int i = 0; i < T_EndGlobal.Count; i++)
                {     
                        //check if there is not any bad recognized points            
                        if (QRS_End.ElementAt(i) != -1 && T_EndGlobal.ElementAt(i) != -1)
                        {
                            //create vector from QRS_End to T_End and calulate maximum index
                            T_MaxIndexList.Add((signal.SubVector(QRS_End.ElementAt(i), T_EndGlobal.ElementAt(i) - QRS_End.ElementAt(i)).MaximumIndex()) + QRS_End.ElementAt(i));
                        }
                        else
                        {
                            T_MaxIndexList.Add(-1);         //insert -1 for bad recognized points
                        }
                }               
            }
            return T_MaxIndexList;
        }
        #region Documentation
        /// <summary>
        /// This function finds max slope indexes in signal after T_max index
        /// </summary>
        /// <returns>Vector that contains Max Slope indexes</returns>
        #endregion
        private List<int> Find_MaxSlope()
        {
            List<int> T_MaxIndex = FindT_Max();
            List<int> T_MaxSlope = new List<int>();
            
            if (P_Onset.ElementAt(0) > T_MaxIndex.ElementAt(0))
            {

                for (int i = 0; i < T_MaxIndex.Count; i++)
                {
                    if (T_EndGlobal.ElementAt(i) != -1 && T_MaxIndex.ElementAt(i) != -1)
                    {                       
                        T_MaxSlope.Add(diff(signal.SubVector(T_MaxIndex.ElementAt(i), (T_EndGlobal.ElementAt(i) - T_MaxIndex.ElementAt(i)))).MinimumIndex()+T_MaxIndex.ElementAt(i));
                    }
                    else
                    {
                        T_MaxSlope.Add(-1);
                    }
                }
            }
            return T_MaxSlope;
        }
        #region Documentation
        /// <summary>
        /// This method find T_End inexes according to tangent method
        /// </summary>
        /// <returns>Vector that contain T_End indexes</returns>
        #endregion
        private List<int> TangentMethod()
        {
            List<int> T_End_Index = new List<int>();
            Vector<double> vectorTofit = Vector<double>.Build.Dense(9);
            double[] x = new double[9];         //variable to store x_data to fit a line
            double[] y = new double[9];         //variable to store y_data to fit a line
            Tuple<double, double> coefficiants; //variable to store output from fitting , Item1=b, Item2=a , y = ax + b

            foreach (int MaxSlope in Find_MaxSlope()){
                if(MaxSlope != -1)
                {                    
                    vectorTofit =  signal.SubVector(MaxSlope-4,9);
                    y = vectorTofit.ToArray();
                    for (int i = -4; i < 5; i++)
                    {
                        x[i + 4] = MaxSlope + i;
                    }
                    coefficiants = MathNet.Numerics.Fit.Line(x, y);
                    T_End_Index.Add((int)(-coefficiants.Item1 / coefficiants.Item2));   //finding T_End according to Tangent method

                }
                else
                {
                    T_End_Index.Add(-1);
                }
               
                
            }
            return T_End_Index;
            
            

        }
        #region Documentation
        /// <summary>
        /// This method find T_End indexes according to parabola method
        /// </summary>
        /// <returns>Vector that contains T_End indexes</returns>
        #endregion
        //private Vector<double> ParabolaMethod()
        //{

        //}
        #region Documentation
        /// <summary>
        /// This function calculate the difference between the next elements in vector
        /// </summary>
        /// <param name="In">Vector to differitive</param>
        /// <returns>Vector after differentiation</returns>
        #endregion
        private Vector<double> diff(Vector<double> In)
        {
            List<double> Output = new List<double>(In.Count - 1);

            for (int i = 0; i < In.Count - 1; i++)
            {
                Output.Insert(i, In.ElementAt(i + 1) - In.ElementAt(i));
            }

            return Vector<double>.Build.DenseOfArray(Output.ToArray());
        }
        #region Documentation
        /// <summary>
        /// This function execute all methods to find a T_End indexes in a signal
        /// </summary>      
        /// <returns>List that contains indexes of T_End according to chose method</returns>
        #endregion
        public List<int> ExecuteTWave()
        {
            
            return TangentMethod();
        }

    }
    #region Documentation
    /// <summary>
    /// This class keep all data to calculate QT interval - no methods
    /// </summary>
    #endregion
    public class QT_Data
    {

        public List<int> QRS_Onset;
        public List<int> T_End;
        public List<double> RR_Interval;
        public double Fs;
        public String drainName;
        public QT_Calc_Method method;

        #region Documentation
        /// <summary>
        /// This constructor creates object that consist of all vectors needed to calculate a QT interval stats and QT disp
        /// </summary>
        /// <param name="QRS_Onset"></param>
        /// <param name="T_End"></param>
        /// <param name="RR_Interval"></param>
        /// <param name="Fs"></param>
        /// <param name="drainName"></param>
        /// <param name="method"></param>
        #endregion
        public QT_Data(List<int> QRS_Onset, List<int> T_End, List<double> RR_Interval, double Fs, String drainName, QT_Calc_Method method)
        {
            this.QRS_Onset = QRS_Onset;
            this.T_End = T_End;
            this.RR_Interval = RR_Interval;
            this.Fs = Fs;
            this.drainName = drainName;
            this.method = method;

        }
        /// <summary>
        /// only for test
        /// </summary>
        public QT_Data()
        {
            int[] qrs_onset = { 558, -1, 1407, 1855 };
            int[] qrs_end = { 779, 1176, 1604, 2032 };
            double[] rr = { 958.33, 1152.78, 1238.89,1113.78};     


            this.QRS_Onset = qrs_onset.ToList();
            this.T_End = qrs_end.ToList();
            this.RR_Interval = rr.ToList();
            this.Fs = 360;
            this.drainName = "V2";
            this.method = QT_Calc_Method.BAZETTA;
        }

    }
    #region Documentation
    /// <summary>
    /// This class is to calculate a QT_disp(both local and global), mean and std of QT intervals
    /// </summary>
    #endregion
    class QTCalculation
    {
        private List<Tuple<string, double>> meanQTInterval = new List<Tuple<string, double>>();
        private List<Tuple<string, double>> stdQTInterval = new List<Tuple<string, double>>();
        private List<Tuple<string, double>> localQTdisp= new List<Tuple<string, double>>();
       
        private double globalQTDisp;
        private List<QT_Data> DataToOperate;            //size of List tells us the amount of drains.
        int AmountOFDrains;


        /// <summary>
        /// This constructor creates list of QT_Data such as QRS_Onset, T_End, RR_interval, Fs, drain name, and QT_interval calculation method
        /// </summary>
        /// <param name="DataToOperate">List with charecteristic points dranin name and method</param>
        public QTCalculation(List<QT_Data> DataToOperate)
        {
            this.DataToOperate = DataToOperate;
            this.AmountOFDrains = this.DataToOperate.Count;           

        }

        /// <summary>
        /// This method calulate QT_intervals and statistic and QT_Disp
        /// </summary>
        public void setOutput()
        {
            List<List<double>> QTIntervalsAll = new List<List<double>>();
            List<double> zero = new List<double>(1);
            zero.Add(0);
            List<double> QTIntervals = new List<double>();                  //stores qt intervals from one drain
            double[] mean = new double[this.AmountOFDrains];                //stores mean qt intervals from next drain
            double[] std = new double[this.AmountOFDrains];                 //stores standard deviation from next drain
            double[] local = new double[this.AmountOFDrains];               //stores locals qt_disp
            int j = 0;

            //adding qt intervals calculated according to chosen formula 
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
                                QTIntervals.Add((((drain.T_End.ElementAt(i) - drain.QRS_Onset.ElementAt(i)) / drain.Fs)*1000) / Math.Sqrt(drain.RR_Interval.ElementAt(i)/1000));
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
                                QTIntervals.Add((((drain.T_End.ElementAt(i) - drain.QRS_Onset.ElementAt(i)) / drain.Fs)*1000) / Math.Pow((drain.RR_Interval.ElementAt(i)/1000), 1 / 3));
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
                                QTIntervals.Add((((drain.T_End.ElementAt(i) - drain.QRS_Onset.ElementAt(i)) / drain.Fs)/1000) + 0.154 * (1 - (drain.RR_Interval.ElementAt(i)/1000)));
                            }
                            else
                            {
                                QTIntervals.Add(0);
                            }

                        }
                    }
                }
                //calculate mean we count all intervals expect 0 because 0 means that there was a bad recognition
                mean[j] = QTIntervals.Sum() / (QTIntervals.Except(zero)).Count();
                local[j] = QTIntervals.Max() - QTIntervals.Min();
                std[j] = (QTIntervals.Except(zero)).StandardDeviation();
                QTIntervalsAll.Add(QTIntervals);        //this list strores all qt intervals from all drain
                j++;
               // QTIntervals.Clear();
            }
            //here we create data to send them to the GUI to show them in table
            for (int i = 0; i < AmountOFDrains; i++)
            {
                
                    this.localQTdisp.Add(Tuple.Create(DataToOperate.ElementAt(i).drainName, local.ElementAt(i)));
                    this.meanQTInterval.Insert(i, Tuple.Create(DataToOperate.ElementAt(i).drainName, mean.ToList().ElementAt(i)));
                    this.stdQTInterval.Insert(i, Tuple.Create(DataToOperate.ElementAt(i).drainName, std.ToList().ElementAt(i)));
               
            }
            //here we calculate a global QT_disp froma all drains
            List<double> temp = new List<double>();
            List<double> globalQT = new List<double>();
            for(int i = 0; i < QTIntervalsAll.ElementAt(0).Count; i++)
            {
                foreach (List<double> onedrain in QTIntervalsAll)
                {
                   temp.Add(onedrain.ElementAt(i));
                }
                if (!temp.Contains(0))
                {
                    globalQT.Add(temp.Max() - temp.Min());
                }
              
            }
            this.globalQTDisp = globalQT.Mean();
            globalQT.Clear();
            temp.Clear();

        }

    }
}
