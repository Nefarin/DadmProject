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


            //TWave testes - begin
            TempInput.setInputFilePath("D:\\Dadm_nowy\\EKG.txt");
            Vector<double> signalfile = TempInput.getSignal();
            List<Tuple<String, Vector<double>>> input = new List<Tuple<string, Vector<double>>>();
            input.Add(Tuple.Create("My drain", signalfile));            
            TWave signal = new TWave(input);
            //here insert a breakpoint          
            List<Tuple<String,List<int>>> wynik = signal.ExecuteTWave();
            //  TWave testes - end
           
            //QTCalulation testes - begin
            QTCalculation wyniki = new QTCalculation();
            //here insert a breakpoint 
            wyniki.setOutput();
            Console.WriteLine(wyniki.meanQTInterval.ElementAt(0).Item2);
            Console.WriteLine(wyniki.meanQTInterval.ElementAt(1).Item2);

            Console.WriteLine(wyniki.stdQTInterval.ElementAt(0).Item2);
            Console.WriteLine(wyniki.stdQTInterval.ElementAt(1).Item2);

            Console.WriteLine(wyniki.localQTdisp.ElementAt(0).Item2);
            Console.WriteLine(wyniki.localQTdisp.ElementAt(1).Item2);
            //QTCalculation testes - end
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
       
        private T_End_Method method;                                    //get from Params
        private bool alldrains;                                         //get from Params                                      
        uint Fs;                                                        //get from Basic Data
       
        private List<int> QRS_End;                                      //get from Waves
        private List<int> T_EndGlobal;                                  //get from Waves
        private Vector<double> R_Peaks;                                 //get from R_Peaks
       
        private List<Tuple<String,Vector<double>>> AllDrainSignal;      //get from ECG_Baseline

        #region Documentation
        /// <summary>
        /// This constructor set fields of a class
        /// </summary>
        /// <param name="AllDrainSignal">This is a list of tuple which stores a drain name and a vector signal from ECG_baseline module</param>
        /// <param name="T_EndGlobal">This list contains T_End indexes</param>      
        /// <param name="QRS_End">This list contains QRS_End indexes</param>      
        /// <param name="R_Peaks">Vector that contains a R_Peak indexes</param> 
        /// <param name="Fs">A sampling frequency</param>
        /// <param name="alldrains">This argument check if the control alldrains in gui is set</param>
        /// <param name="method">This argument is a method to find T_End in a signal</param>
        #endregion
        public TWave(List<Tuple<String, Vector<double>>> AllDrainSignal, List<int> QRS_End, List<int> T_EndGlobal, Vector<double> R_Peaks, uint Fs,T_End_Method method, bool alldrains)
        {           
           
            this.method = method;
            this.alldrains = alldrains;
            this.Fs = Fs;

            this.AllDrainSignal = AllDrainSignal;          
            this.QRS_End = QRS_End;             
            this.T_EndGlobal = T_EndGlobal;
            this.R_Peaks = R_Peaks;
        }
        /// <summary>
        /// Only for test
        /// </summary>
        /// <param name="input">Signal</param>
        public TWave(List<Tuple<String,Vector<double>>> input)
        {
            //for testes
            
           
            int[] qrsend = { 380,676,960,1244,1528 };
            int[] tend = { 265,553,835,1119,1410 };        
           
            this.QRS_End = qrsend.ToList();
            this.AllDrainSignal = input;
           
                  
            this.T_EndGlobal = tend.ToList();
            this.method = T_End_Method.PARABOLA;
            this.alldrains = false;
        }
       
        #region Documentation
        /// <summary>
        /// This function finds T-Max indexes in signal
        /// </summary>
        /// <param name="signal">A vector with a signal</param>
        /// <returns>List that contains T_Max indexes</returns>
        #endregion
        private List<int> FindT_Max(Vector<double> signal)
        {
            //list that stores t max indexes
            List<int> T_MaxIndexList = new List<int>();            
           
            if (QRS_End.ElementAt(0) < T_EndGlobal.ElementAt(0))
            {
                //this for goes for every T_end in signal
                for (int i = 0; i < T_EndGlobal.Count-2; i++)
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
            else
            {
                //in case where signal started to be recorded after a QRS peak
                for (int i = 0; i < T_EndGlobal.Count-2; i++)
                {
                    //check if there is not any bad recognized points            
                    if (QRS_End.ElementAt(i) != -1 && T_EndGlobal.ElementAt(i+1) != -1)
                    {                       
                        T_MaxIndexList.Add((signal.SubVector(QRS_End.ElementAt(i), T_EndGlobal.ElementAt(i + 1) - QRS_End.ElementAt(i)).MaximumIndex()) + QRS_End.ElementAt(i));
                    }
                    else
                    {
                        T_MaxIndexList.Add(-1);         //insert -1 for bad-recognized points
                    }
                }
            }
            return T_MaxIndexList;
        }
        #region Documentation
        /// <summary>
        /// This function finds max slope indexes in signal after T_max index
        /// </summary>
        /// <param name="signal">A vector with a signal</param>
        /// <returns>List that contains Max Slope indexes</returns>
        #endregion
        private List<int> Find_MaxSlope(Vector<double> signal)
        {
            List<int> T_MaxIndex = FindT_Max(signal);
            //list that stores t_max slope need to fit a line or a parabola
            List<int> T_MaxSlope = new List<int>();          

                for (int i = 0; i < T_MaxIndex.Count; i++)
                if(T_MaxIndex.ElementAt(i)<T_EndGlobal.ElementAt(i))
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
                else
                {
                    if (T_EndGlobal.ElementAt(i) != -1 && T_MaxIndex.ElementAt(i) != -1)
                    {
                        T_MaxSlope.Add(diff(signal.SubVector(T_MaxIndex.ElementAt(i), (T_EndGlobal.ElementAt(i+1) - T_MaxIndex.ElementAt(i)))).MinimumIndex() + T_MaxIndex.ElementAt(i));
                    }
                    else
                    {
                        T_MaxSlope.Add(-1);
                    }

                }
           
            return T_MaxSlope;
        }
        #region Documentation
        /// <summary>
        /// This method find T_End inexes according to tangent method
        /// </summary>
        /// <param name="signal">A vector with a signal</param>
        /// <returns>List that contain T_End indexes calculated according to Tangent method</returns>
        #endregion
        private List<int> TangentMethod(Vector<double> signal)
        {
            //lsit that stores t_end indexes 
            List<int> T_End_Index = new List<int>();
            Vector<double> vectorTofit = Vector<double>.Build.Dense(9);
            int tempindex = 0;
            double[] x = new double[9];         //variable to store x_data to fit a line
            double[] y = new double[9];         //variable to store y_data to fit a line
            Tuple<double, double> coefficiants; //variable to store output from fitting , Item1=b, Item2=a , y = ax + b

            foreach (int MaxSlope in Find_MaxSlope(signal)){
                if(MaxSlope != -1)
                {                    
                    vectorTofit =  signal.SubVector(MaxSlope-4,9);
                    y = vectorTofit.ToArray();
                    for (int i = -4; i < 5; i++)
                    {
                        x[i + 4] = MaxSlope + i;
                    }
                    coefficiants = MathNet.Numerics.Fit.Line(x, y);
                    tempindex = (int)(-coefficiants.Item1 / coefficiants.Item2);
                    // here we should check if the index is correct
                    T_End_Index.Add(tempindex);   //finding T_End according to Tangent method

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
        /// <param name="signal">A vector with a signal</param>
        /// <returns>List that contains T_End indexes calculated according to parabola method</returns>
        #endregion
        private List<int> ParabolaMethod(Vector<double> signal)
        {
            List<int> T_End_Index = new List<int>();
            List<int> T_MaxSlope = Find_MaxSlope(signal);
            Vector<double> vectorTofit = Vector<double>.Build.Dense(6);
            int[] step = new int[T_EndGlobal.Count];
            double delta = new double();
            int tempindex = -1;

            //Vector<double> vectorTofit = Vector<double>.Build.Dense(5);

            double[] x = new double[7];         //variable to store x_data to fit a polynomial (parabola)
            double[] y = new double[7];         //variable to store y_data to fit a polynomial (parabola)
            double[] coefficients;              //variable to store output from fitting , Item1=c, Item2=b, Item3=a , y = ax^2 + bx +c

            foreach (int MaxSlope in Find_MaxSlope(signal))
            {
                if (MaxSlope != -1)
                {
                    vectorTofit = signal.SubVector(MaxSlope, 7);
                    y = vectorTofit.ToArray();
                    for (int i = 0; i < 7; i++)
                    {
                        x[i] = MaxSlope + i;
                    }

                    coefficients = MathNet.Numerics.Fit.Polynomial(x, y, 2); //calculates the coeffs of parabola that fits best the slope
                    delta = Math.Pow(coefficients[1], 2) - 4 * coefficients[2] * coefficients[0];
                    tempindex = (int)(-coefficients[1] / (2 * coefficients[2]));
                    //here we should check if t_end index is correct
                    T_End_Index.Add(tempindex);   //finding T_End according to Tangent method
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
        /// <returns>List of tuple that contains a drain name and a list with indexes of T_End according to chose method</returns>
        #endregion
        public List<Tuple<String,List<int>>> ExecuteTWave()
        {
            List<Tuple<String, List<int>>> output = new List<Tuple<string, List<int>>>();
            //if alldrains in gui i set we calculate t_end index in all drains according to chosen method
            if(alldrains == true)
            {
                for(int i = 0; i<this.AllDrainSignal.Count(); i++)
                {
                    if (this.method == T_End_Method.PARABOLA)
                    {
                         output.Add(Tuple.Create(this.AllDrainSignal.ElementAt(i).Item1,ParabolaMethod(this.AllDrainSignal.ElementAt(i).Item2)));
                    }
                    else
                    {
                        output.Add(Tuple.Create(this.AllDrainSignal.ElementAt(i).Item1, TangentMethod(this.AllDrainSignal.ElementAt(i).Item2)));                        
                    }
                }
            }
            // if alldrains is not set we return T_End indexes detected by Waves
            else
            {
                output.Add(Tuple.Create("Default", T_EndGlobal));                
            }
            return output;
            
        }

    }






    
    /// <summary>
    /// This class is to calculate QT intervals statistic and QT disp from one drain and all drains
    /// </summary>
    class QTCalculation
    {
        //iput
        private List<int> QRS_Onset = new List<int>();       
        private Vector<double> RR_interval;
        private QT_Calc_Method method;
        private double Fs;
        private List<Tuple<String, List<int>>> T_End = new List<Tuple<string, List<int>>>();
        //output
        public List<Tuple<string, double>> meanQTInterval = new List<Tuple<string, double>>();
        public List<Tuple<string, double>> stdQTInterval = new List<Tuple<string, double>>();
        public List<Tuple<string, double>> localQTdisp = new List<Tuple<string, double>>();

        private double globalQTDisp;
      
        int AmountOFDrains;


        /// <summary>
        /// This constructor creates list of QT_Data such as QRS_Onset, T_End, RR_interval, Fs, drain name, and QT_interval calculation method
        /// </summary>
        /// <param name="QRS_Onset">This list stores next QRS_Onset indexes</param>
        /// <param name="T_End">This list stores tuples which is a drain name and list with T-End inexes</param>
        /// <param name="RR_interval">This list stores next RR Intervals in signal in ms</param>
        /// <param name="Fs">This is frequency sampling ih Hz</param>
        /// <param name="method">This is a formula, which we want to calculate a QT_interval</param>
        public QTCalculation(List<int> QRS_Onset, Vector<double> RR_interval, QT_Calc_Method method,double Fs, List<Tuple<String,List<int>>> T_End)
        {
            this.Fs = Fs;
            this.method = method;
            this.QRS_Onset = QRS_Onset;
            this.T_End = T_End;
            this.RR_interval = RR_interval;
            this.AmountOFDrains = this.T_End.Count;

        }
        /// <summary>
        /// only for test
        /// </summary>
        public QTCalculation()
        {
            int[] qrs_onset = { 558, 920, 1407, 1855 };
            int[] qrs_end = { 779, 1176, 1604, 2032 };
            int[] t_end = { 750, 1300, 1654, 2067 };
            double[] rr = { 958.33, 1152.78, 1238.89, 1113.78 };


            this.QRS_Onset = qrs_onset.ToList();
            this.T_End = new List<Tuple<String, List<int>>>(); qrs_end.ToList();
            this.RR_interval = Vector<double>.Build.DenseOfArray(rr);
            this.Fs = 360;
            Tuple<String, List<int>> drain1 = Tuple.Create("V2", qrs_end.ToList());
            Tuple<String, List<int>> drain2 = Tuple.Create("V5", t_end.ToList());
            this.T_End.Add(drain1);
            this.T_End.Add(drain2);
            this.method = QT_Calc_Method.BAZETTA;
            this.AmountOFDrains = this.T_End.Count;
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
            int start = 0;
            int koniec = 0;

            //adding qt intervals calculated according to chosen formula 
            foreach (Tuple<String,List<int>> T_EndInDrain in T_End)
            {
                if (method == QT_Calc_Method.BAZETTA)
                {
                    if (QRS_Onset.ElementAt(0) < T_EndInDrain.Item2.ElementAt(0))
                    {
                        for (int i = 0; i < QRS_Onset.Count; i++)
                        {
                            if (QRS_Onset.ElementAt(i) != -1 && (T_EndInDrain.Item2.ElementAt(i) != -1))
                            {                                
                                QTIntervals.Add((((T_EndInDrain.Item2.ElementAt(i) - QRS_Onset.ElementAt(i)) / Fs) * 1000) / Math.Sqrt(RR_interval.ElementAt(i) / 1000));
                            }
                            else
                            {
                                QTIntervals.Add(0);
                            }

                        }
                    }
                }
                if (method == QT_Calc_Method.FRIDERICA)
                {
                    if (QRS_Onset.ElementAt(0) < T_EndInDrain.Item2.ElementAt(0))
                    {
                        for (int i = 0; i < QRS_Onset.Count; i++)
                        {
                            if (QRS_Onset.ElementAt(i) != -1 && (T_EndInDrain.Item2.ElementAt(i) != -1))
                            {
                                QTIntervals.Add((((T_EndInDrain.Item2.ElementAt(i) - QRS_Onset.ElementAt(i)) / Fs) * 1000) / Math.Sqrt(RR_interval.ElementAt(i) / 1000));
                            }
                            else
                            {
                                QTIntervals.Add(0);
                            }

                        }

                    }
                }
                if (method == QT_Calc_Method.FRAMIGHAMA)
                {
                    if (QRS_Onset.ElementAt(0) < T_EndInDrain.Item2.ElementAt(0))
                    {
                        for (int i = 0; i < QRS_Onset.Count; i++)
                        {
                            if (QRS_Onset.ElementAt(i) != -1 && (T_EndInDrain.Item2.ElementAt(i) != -1))
                            {
                                QTIntervals.Add((((T_EndInDrain.Item2.ElementAt(i) - QRS_Onset.ElementAt(i)) / Fs) * 1000) / Math.Sqrt(RR_interval.ElementAt(i) / 1000));
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
                local[j] = QTIntervals.Max() - QTIntervals.Except(zero).Min();
                std[j] = (QTIntervals.Except(zero)).StandardDeviation();
                koniec = QTIntervals.Count;
                QTIntervalsAll.Add(QTIntervals.GetRange(start,koniec-start));        //this list strores all qt intervals from all drain
                j++;
                start = koniec;
                //QTIntervals.Clear();                    //can't do it because it's a reference
            }
            //here we create data to send them to the GUI to show them in table
            for (int i = 0; i < AmountOFDrains; i++)
            {

                this.localQTdisp.Add(Tuple.Create(T_End.ElementAt(i).Item1, local.ElementAt(i)));
                this.meanQTInterval.Add(Tuple.Create(T_End.ElementAt(i).Item1, mean.ToList().ElementAt(i)));
                this.stdQTInterval.Add(Tuple.Create(T_End.ElementAt(i).Item1, std.ToList().ElementAt(i)));

            }
            //here we calculate a global QT_disp froma all drains   check it because all above works
            List<double> temp = new List<double>();
            List<double> temp2 = new List<double>();
            List<double> globalQT = new List<double>();
            for (int i = 0; i < QTIntervalsAll.ElementAt(0).Count; i++)
            {
                foreach (List<double> onedrain in QTIntervalsAll)
                {
                    temp.Add(onedrain.ElementAt(i));
                }
                temp2 = temp.GetRange(i * this.AmountOFDrains, AmountOFDrains);
                if (!temp2.Contains(0))
                {
                    globalQT.Add(temp2.Max() - temp2.Min());
                }

            }
            this.globalQTDisp = globalQT.Mean();
            globalQT.Clear();
            temp.Clear();

        }

    }
}
