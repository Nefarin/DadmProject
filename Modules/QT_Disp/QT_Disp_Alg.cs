﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Differentiation;
using EKG_Project.IO;
using MathNet.Numerics.Statistics;
using System.IO;


namespace EKG_Project.Modules.QT_Disp
{
    public class QT_Disp_Alg
    {
        //this data we get from interface
        //after testes we should change double to int 
        List<int> QRS_onset;                         //to do in init
        List<int> T_End_Global;                      //to do in init
        List<int> QRS_End;                           //to do in init
        Vector<double> R_Peaks;                         //to do in init
        List<double> R_Peaks_List;
        T_End_Method T_End_method;                      //to do in init
        QT_Calc_Method QT_Calc_method;                  //to do in init
        uint Fs;                                        //to do in init
        // this data we store and calculate stats
        List<double> QT_intervals;                      //to do in processData
        List<int> T_End_local;                          //to do in processData
        //List<List<double>> AllQT_Intervals;             //to do after change a channel
        List<int>.Enumerator Onset_Enumerator;
        List<int>.Enumerator End_Enumerator;
        List<int>.Enumerator T_End_Enumerator;
        List<double>.Enumerator R_Peaks_Enumerator;
        List<double>.Enumerator R_Peaks_Next_Enumerator;
        public QT_Disp_Alg(int size)
        {
            QRS_onset = new List<int>(size);
            T_End_Global = new List<int>(size);
            QRS_End = new List<int>(size);
            R_Peaks = Vector<double>.Build.Dense(1);
            R_Peaks_List = new List<double>(size);
            T_End_method = new T_End_Method();
            QT_Calc_method = new QT_Calc_Method();
            Fs = new uint();
            QT_intervals = new List<double>(size);
            //AllQT_Intervals = new List<List<double>>();
            T_End_local = new List<int>(size);
        }      
        
        /// <summary>
        /// This constructor get all data to a current drain
        /// </summary>
        /// <param name="QRS_Onset">List with QRS_Onset indexes</param>
        /// <param name="T_End_Global">List with T_End indexes</param>
        /// <param name="QRS_End">List with QRS_End indexes</param>
        /// <param name="R_Peaks">Vector with R peaks indexes</param>
        /// <param name="T_End_method">method use to calculate T_End </param>
        /// <param name="QT_Calc_method">method use to calculate QT interval</param>
        /// <param name="Fs">Sampling Frequency</param>
        public void TODoInInit(List<int> QRS_Onset, List<int> T_End_Global, List<int> QRS_End, Vector<double> R_Peaks, T_End_Method T_End_method, QT_Calc_Method QT_Calc_method, uint Fs)
        {
            if (QRS_Onset.Count == 0) throw new ArgumentNullException("QRS_onset empty list");
            if(QRS_End.Count == 0) throw new ArgumentNullException("QRS_end empty list");
            if(R_Peaks.Count == 0) throw new ArgumentNullException("R_Peak empty list");
            if(T_End_Global.Count == 0) throw new ArgumentNullException("T_End empty list");
            if (Fs < 0) throw new InvalidDataException("Wrong frequency");
            if (QRS_Onset.Count != QRS_End.Count && QRS_End.Count != T_End_Global.Count) throw new InvalidOperationException("List not same capacity");

            this.QRS_onset = QRS_Onset;
            this.QRS_End = QRS_End;
            this.T_End_Global = T_End_Global;
            this.R_Peaks = R_Peaks;
            this.T_End_method = T_End_method;
            this.QT_Calc_method = QT_Calc_method;
            this.Fs = Fs;
            this.End_Enumerator = this.QRS_End.GetEnumerator();
            this.Onset_Enumerator = this.QRS_onset.GetEnumerator();
            this.T_End_Enumerator = this.T_End_Global.GetEnumerator();
            this.R_Peaks_List = this.R_Peaks.ToList();
            this.R_Peaks_Enumerator = this.R_Peaks_List.GetEnumerator();
            this.R_Peaks_Next_Enumerator = this.R_Peaks_List.GetEnumerator();
            this.R_Peaks_Next_Enumerator.MoveNext();
        }
        /// <summary>
        /// This function is called to get a current sampled signal and processed it 
        /// </summary>
        /// <param name="samples">Vector with a cut signal</param>
        /// <param name="index">An index of a current R peak</param>
        /// <returns>T End local index</returns>
        public Tuple<int,double> ToDoInProccessData(Vector<double> samples, int index)
        {
            Onset_Enumerator.MoveNext();
            End_Enumerator.MoveNext();
            T_End_Enumerator.MoveNext();
            R_Peaks_Enumerator.MoveNext();
            R_Peaks_Next_Enumerator.MoveNext();
            
            
            //check if we have a good arguments
            if (samples.Count == 0)
            {
                throw new InvalidOperationException("Empty samples");
            }
            if(index == null)
            {
                throw new InvalidOperationException("Null index");
            }
            if (samples.Count != (this.R_Peaks_Next_Enumerator.Current - this.R_Peaks_Enumerator.Current))
            {
                throw new InvalidDataException("Wrong size of samples");
            }
            //start algorithm
            int T_End = -1;
            Tuple<int, double> result = Tuple.Create(-1, 0.0);
            //check if we exceed a size of R _ Peaks
            if (index < (R_Peaks.Count - 2))
            {
                
                // create a table with a R_peaks 
                //between this indexes we lookig for T_End 
                double[] R_peaks_loc = new double[2];
                R_peaks_loc[0] = R_Peaks_Enumerator.Current;
                R_peaks_loc[1] = R_Peaks_Next_Enumerator.Current;              
                // create new object that stores a indexes of points from a Waves module
                // need this point to find T_end
                if(index<T_End_Global.Count)
                {                   
                    //int onset = QRS_onset.ElementAt(1);
                    //int end = QRS_End.ElementAt(0);                   
                    ///if (onset > end )
                    //{
                        //DataToCalculate data = new DataToCalculate(QRS_onset.ElementAt(index), QRS_End.ElementAt(index), T_End_Global.ElementAt(index), samples, QT_Calc_method, T_End_method, Fs, R_peaks_loc);
                        DataToCalculate data = new DataToCalculate(Onset_Enumerator.Current, End_Enumerator.Current, T_End_Enumerator.Current, samples, QT_Calc_method, T_End_method, Fs, R_peaks_loc);
                        result = data.Calc_QT_Interval();                    
                    ///}                   
                    //else
                    //{
                     //   if (index > 1)
                       // {                           
                         //   DataToCalculate data = new DataToCalculate(QRS_onset.ElementAt(index), QRS_End.ElementAt(index-1),T_End_Global.ElementAt(index-1), samples, QT_Calc_method, T_End_method, Fs, R_peaks_loc);
                         //   result = data.Calc_QT_Interval();                         
                      //  }
                        
                  //  }                   
                }
                //here we add a QT interval to a list
                QT_INTERVALS.Add(result.Item2);
                //here we add a T_End index to a list
                T_End_local.Add(result.Item1);
            }
            // return a T_End index if bad recognition assign -1
            return result;
        }
        /// <summary>
        /// This method calculate a mean QT interval from one drain
        /// </summary>
        /// <returns>QT interval mean</returns>
        public double getMean()
        {
           
           
            // here we store a mean value
            double mean;
            int zeroElements = QT_INTERVALS.FindAll(s => s.Equals(0)).Count;
           
            // chceck if a drain was calculate correct            
            if (QT_INTERVALS.Count() == zeroElements)
            {
                // QT interval list include only element with zero values so something go wrong if we are here
                mean = 0;
            }
            else
            {
                // if there is some elements we calculate a mean
                // we count only good values so we count all QT interval except 0
                mean = QT_intervals.Sum() / (QT_intervals.Count()-zeroElements);
            }           
            return mean;
        }
        /// <summary>
        /// This function calculate a standard deviation for all QT intervals in one drain
        /// </summary>
        /// <returns>QT interval standard deviation from one drain</returns>
        public double getStd()
        {
            //here we store a standard deviation value
            double std;
            //List<double> std_base = new List<double>;
            
            // chceck if a drain was calculate correctly      
            if (QT_INTERVALS.Exists(q => q > 0))
            {
                // if there are any elements, we calculate a standard deviation

                //subtract 0 elements before the std calculation
                //zeros indicate the lack of QT interval and not the value of interval - cannot be counted
            int std_base = QT_INTERVALS.RemoveAll(x => x.Equals(0));
                std = QT_INTERVALS.StandardDeviation();
            }
            else
            {
                // if QT interval list include only element with zero values (means something go wrong if we are here)
                std = 0;
                
            }           
            return std;
        }
        /// <summary>
        /// This method calculate local QT disperssion
        /// </summary>
        /// <returns>QT disperssion from one drain</returns>
        public double getLocal()
        {
            // here we store a local QT dispersion from one drain
            double local;
            List<double> zero = new List<double>(1);
            zero.Add(0);
            //here we check if there is any QT interval in this list
            try
            {
                //if yes,  we calculate a local QT disperssion
                int std_base = QT_INTERVALS.RemoveAll(x => x.Equals(0));
                int std_base2 = QT_INTERVALS.RemoveAll(x => x > 650);
                int std_base3 = QT_INTERVALS.RemoveAll(x => x < 250);
                local = QT_INTERVALS.Max() - QT_INTERVALS.Min();
               
            }
            catch(System.InvalidOperationException ex)
            {
                // if not we assign 0 to local QT disperssion
                local = 0;
            }            
            return local;
        }/// <summary>
        /// This method deletes a list with QT interval
        /// </summary>
        public void DeleteQT_Intervals()
        {           
            QT_INTERVALS.Clear();
            T_END_LOCAL.Clear();
        }
        //getters and setters
        public List<double> QT_INTERVALS
        {
            get
            {
                return QT_intervals;
            }
            set
            {
                QT_intervals = value;
            }

        }
        public List<int> T_END_LOCAL
        {
            get
            {
                return T_End_local;
            }
            set
            {
                T_End_local = value;
            }
        }
        /*public static void Main()
        {
            Console.WriteLine("Hello world");
            // Test if algorithm is correct
            //  read from file
            DebugECGPath path = new DebugECGPath();
            var path_data = path.getDataPath();
            string path_output = Path.Combine(path_data, "res_QT_Disp", "sig.txt");
            Vector<double> signal;
            Vector<double> samples;

            string path_output_onset = Path.Combine(path_data, "res_QT_Disp", "QRS_Onset.txt");
            string path_output_end = Path.Combine(path_data, "res_QT_Disp", "QRS_End.txt");
            string path_output_tend = Path.Combine(path_data, "res_QT_Disp", "T_End.txt");
            string path_output_rpeak = Path.Combine(path_data, "res_QT_Disp", "R_Peak.txt");
            string path_output_my_t_end = Path.Combine(path_data, "res_QT_Disp", "output_end.txt");
            string path_output_qt_intervals = Path.Combine(path_data, "res_QT_Disp", "output_qt.txt");


            List<int> onset = new List<int>();
            List<int> end = new List<int>();
            List<int> tend = new List<int>();
            Vector<double> R_Peaks;

            TempInput.setInputFilePath(path_output_onset);
            foreach (double element in TempInput.getSignal().ToList())
            {
                onset.Add((int)element);
            }
            
            TempInput.setInputFilePath(path_output_end);
            foreach (double element in TempInput.getSignal().ToList())
            {
                end.Add((int)element);
            }
            
            TempInput.setInputFilePath(path_output_rpeak);
            R_Peaks = TempInput.getSignal();
            TempInput.setInputFilePath(path_output_tend);
            foreach (double element in TempInput.getSignal().ToList())
            {
                tend.Add((int)element);
            }

       

            TempInput.setInputFilePath(path_output);            
            signal = TempInput.getSignal();

           

            Tuple<int, double> result;
            //create object like we do in interface
            QT_Disp_Alg data = new QT_Disp_Alg(24);
            // read list from waves, r_peaks  and params 
            data.TODoInInit(onset, tend, end, R_Peaks, T_End_Method.TANGENT, QT_Calc_Method.FRAMIGHAMA, 360);
            List<Tuple<int, double>> output = new List<Tuple<int, double>>();
            // variable to store output
            double[] t_end = new double[21];
            double[] qt_interval = new double[21];
            //signal cutting
           
            for(int i=0; i<data.R_Peaks.Count-1; i++)
            {
                samples = signal.SubVector((int)data.R_Peaks.ElementAt(i),
                   (int)(data.R_Peaks.ElementAt(i + 1) - data.R_Peaks.ElementAt(i)));
                // cutted signal between neighbour R_Peaks
                result = data.ToDoInProccessData(samples, i);
                t_end[i] = result.Item1;
                qt_interval[i] = result.Item2;             
               
            }
            // write t_ends and qt intervals to file
            TempInput.setOutputFilePath(path_output_my_t_end);
            TempInput.writeFile(360,Vector<double>.Build.DenseOfArray(t_end));
            TempInput.setOutputFilePath(path_output_qt_intervals);
            TempInput.writeFile(360, Vector<double>.Build.DenseOfArray(qt_interval));
            // Show output that will be saved in file 
            Console.WriteLine("QT local:\t" + data.getLocal() + "\nQT_mean:\t" + data.getMean() + "\nQT_std:\t\t" + data.getStd());



            Console.ReadKey();
        }*/
        
    }
    //It was writen but not tested, also there is not any comments
  
    /// <summary>
    /// This class was written to fulfill a processData assumptions    /// 
    /// </summary>
    public class DataToCalculate
    {
        private int QRS_onset;
        private int QRS_End;
        private int T_End_Global;
        private Vector<double> samples;
        private QT_Calc_Method QT_Calc_method;
        private T_End_Method T_End_method;
        private uint Fs;
        private double[] R_Peak = new double[2];

        public DataToCalculate(int QRS_onset, int QRS_End, int T_End_Global, Vector<double> samples, QT_Calc_Method QT_Calc_method, T_End_Method T_End_method, uint Fs, double[] R_Peak)
        {
            //check if we have a good arguments if not system crash
            if(QRS_onset == null) throw new ArgumentNullException("QRS_Onset null");
            if(QRS_End == null) throw new ArgumentNullException("QRS_End null");
            if(T_End_Global == null) throw new ArgumentNullException("T_End_Global null");
            if (R_Peak.Count() != 2) throw new InvalidDataException("Wrong parameters for R_Peak");
            if(samples.Count != (R_Peak[1]- R_Peak[0])) throw new ArgumentNullException("Samples wrong length");
            if(QRS_onset > QRS_End && (QRS_End !=- 1)) throw new ArgumentNullException("Waves not working good, QRS onset > QRS End - recognition impossible");
            if(QRS_End > T_End_Global && (T_End_Global !=-1) ) throw new ArgumentNullException("Waves not working good, QRS End > T_End- recognition impossible");
            if(Fs < 0) throw new ArgumentNullException("Wrong Frequency parameter");
            if ((QRS_End < R_Peak.ElementAt(0) || QRS_End > R_Peak.ElementAt(1)) && QRS_End != -1)
            {
                QRS_End = -1;// throw new InvalidDataException("Wrong QRS End detection");
            }
            if ((T_End_Global < R_Peak.ElementAt(0) || T_End_Global > R_Peak.ElementAt(1)) && T_End_Global != -1)
            {
                T_End_Global = -1; // throw new InvalidDataException("Wrong T End detection");
            }
            if (QRS_onset > R_Peak.ElementAt(0) && QRS_onset != -1)
            {
                QRS_onset = -1; //  throw new InvalidDataException("Wrong QRS Onset detection");
            }

            this.QRS_onset = QRS_onset;
            this.QRS_End = QRS_End;
            this.T_End_Global = T_End_Global;
            this.samples = samples;
            this.QT_Calc_method = QT_Calc_method;
            this.T_End_method = T_End_method;
            this.Fs = Fs;
            this.R_Peak = R_Peak;
        }
        public DataToCalculate()
        {
            this.QRS_onset = -1;
            this.QRS_End = -1;
            this.QT_Calc_method = QT_Calc_Method.BAZETTA;
            this.R_Peak = new double[2];
            this.Fs = 360;
            this.T_End_Global = -1;
            this.samples = Vector<double>.Build.Dense(1);
            this.T_End_method = T_End_Method.TANGENT;
        }

        /// <summary>
        /// This method find T_End index in a signal
        /// </summary>
        /// <returns>T End index</returns>
        public int FindT_End()
        {
            /*if(QRS_End == 74627)
            {
                Console.WriteLine("Something go wrong");
            }*/
            int T_End = -1;
            int T_Max = -1;
            int MaxSlope = -1;
            // check if QRS_End or  T_End globa was correct identify
            if (QRS_End != -1 && T_End_Global != -1)
            {
                // check if a R peak is normal or invert
                if (samples.ElementAt((int)(R_Peak.ElementAt(1) - R_Peak.ElementAt(0)) - 1) > 0)
                {
                    // positive QRS wave
                    // check if Waves made good recognition
                    try
                    {
                        //if yes calculate T_Max index
                        T_Max = samples.SubVector((QRS_End - (int)R_Peak.ElementAt(0)), (T_End_Global - QRS_End - (int)(0.015* Fs))).MaximumIndex() + QRS_End;
                    }
                    catch (Exception ex)
                    {
                        if (ex is IndexOutOfRangeException || ex is ArgumentOutOfRangeException)
                        {
                            // if not assign -1 to T_Max
                            T_Max = -1;
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                else
                {
                    //invert QRS wave
                    // check if waves make good recognition
                    try
                    {
                        // if yes invert a signal and get a T_Max index
                        T_Max = samples.SubVector((QRS_End - (int)R_Peak.ElementAt(0))+(int)(Fs*0.2), T_End_Global - QRS_End - (int)(0.035* Fs)).Negate().MaximumIndex() + QRS_End;
                    }
                    catch (Exception ex)
                    {
                        if (ex is IndexOutOfRangeException || ex is ArgumentOutOfRangeException)
                        {
                            // if not assign -1 to T_Max
                            T_Max = -1;
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
            //if QRS_Onset or T_End Global index is -1
            else
            {
                T_Max = -1;

            }
            // now we calculate a max slope for points after a T_Max
            // check if the QRS Onset or T_End global was correct recognize
            if (T_Max != -1)
            {
                // if yes check if we have a positive QRS wave
                if (samples.ElementAt(T_Max-(int)R_Peak.ElementAt(0)) > 0)
                {
                    // if yes we calculate a differantion beetwen T_Max and T_End global
                    // then we get a minimum index to find a point with a max slope
                    MaxSlope = diff(samples.SubVector(T_Max - (int)R_Peak.ElementAt(0), T_End_Global - T_Max )).MinimumIndex() + T_Max;
                }
                else
                {
                    // if we get a negative QRS we get a point with a max slope
                    MaxSlope = diff(samples.SubVector(T_Max-(int)R_Peak.ElementAt(0), T_End_Global - T_Max)).MaximumIndex() + T_Max;
                }
            }
            //bad recognition QRS End or T_End
            else
            {
                MaxSlope = -1;
            }
            // choose a method to calculate a T End
            if (T_End_method == T_End_Method.TANGENT)
            {
                // this tables stores a points around Max Slope
                // we use them to fit a line tangent to Max slope point
                double[] x = new double[9];
                double[] y = new double[9];
                Tuple<double, double> coefficiants;
                Vector<double> vectorToFit = Vector<double>.Build.Dense(9);
                int tempindex = 0;
                if (MaxSlope != -1)
                {                   
                    // creates array to store y points
                    try
                    {
                        vectorToFit = samples.SubVector(MaxSlope - (int)R_Peak.ElementAt(0) - 5, 9);
                    }
                    catch (ArgumentOutOfRangeException ex) {
                        vectorToFit.Add(R_Peak.ElementAt(0));
                        vectorToFit.Add(R_Peak.ElementAt(0)+1);
                        vectorToFit.Add(R_Peak.ElementAt(0)+2);
                        vectorToFit.Add(R_Peak.ElementAt(0)+3);
                        vectorToFit.Add(R_Peak.ElementAt(0)+4);
                        vectorToFit.Add(R_Peak.ElementAt(0)+5);
                        vectorToFit.Add(R_Peak.ElementAt(0)+6);
                        vectorToFit.Add(R_Peak.ElementAt(0)+7);
                        vectorToFit.Add(R_Peak.ElementAt(0)+8);                   
                    }            
                   
                    y = vectorToFit.ToArray();
                    // creates array to store x points
                    for (int i = -4; i < 5; i++)
                    {
                        x[i + 4] = MaxSlope + i;
                    }
                    // calculate a line coefficiants
                    coefficiants = MathNet.Numerics.Fit.Line(x, y);                 //y = ax + b
                    tempindex = (int)(-coefficiants.Item1 / coefficiants.Item2);    // x = (y - b)/a  if  y=0  so x = -b/a
                    // start checking from 80ms  after global T_end and 140 ms before T_End
                    if (tempindex < (T_End_Global + (int)(0.08 * Fs)) && tempindex > (T_End_Global - (int)(0.14 * Fs))) 
                    {
                        //here we should check if t_end index is correct
                        T_End = tempindex;   //finding T_End according to Tangent method
                    }
                    else
                    // if our local T_End  is bad recognised write global T_End index
                    {
                        T_End = T_End_Global;
                    }
                }
                else
                {
                    T_End = -1;
                }
            }
            // if we choose a Parabola method
            else
            {
                double[] x = new double[7];
                double[] y = new double[7];
                double[] coefficiants;
                Vector<double> vectorToFit = Vector<double>.Build.Dense(7);
                int tempindex = 0;

                if (MaxSlope != -1)
                {
                    vectorToFit = samples.SubVector(MaxSlope - (int)R_Peak.ElementAt(0), 7);
                    y = vectorToFit.ToArray();
                    for (int i = 0; i < 7; i++)
                    {
                        x[i] = MaxSlope + i;
                    }
                    coefficiants = MathNet.Numerics.Fit.Polynomial(x, y, 2);
                    tempindex = (int)(-coefficiants.ElementAt(1) / (2 * coefficiants.ElementAt(2)));
                    if (tempindex < (T_End_Global + (int)(0.08 * Fs)) && tempindex > (T_End_Global - (int)(0.14 * Fs))) //start checking from 40ms s after global T_end
                    {
                        //here we should check if t_end index is correct
                        T_End = tempindex;   //finding T_End according to Tangent method
                    }
                    else
                    {
                        T_End = T_End_Global;
                    }

                }
                else
                {
                    T_End = -1;
                }
            }
            return T_End;
        }
        /// <summary>
        /// This method calculate QT interval 
        /// </summary>
        /// <returns>QT interval calculated according to chosen formula and T_end local index</returns>
        public Tuple<int,double> Calc_QT_Interval()
        {
            Tuple<int, double> outputData;
            double QT_Interval = 0;
            int T_End = FindT_End();
            if (QRS_onset != -1 && (T_End != -1))
            {
                switch (QT_Calc_method)
                {
                    case QT_Calc_Method.BAZETTA:
                        QT_Interval = ((((double)T_End - (double)QRS_onset) / (double)Fs) * 1000) / (Math.Sqrt((R_Peak.ElementAt(1) - R_Peak.ElementAt(0)) / (double)Fs));
                        break;
                    case QT_Calc_Method.FRIDERICA:
                        QT_Interval = ((((double)T_End - (double)QRS_onset) / (double)Fs) * 1000.0) / Math.Pow(((R_Peak.ElementAt(1) - R_Peak.ElementAt(0)) / (double)Fs), 1 / 3);
                        break;
                    case QT_Calc_Method.FRAMIGHAMA:
                        QT_Interval = ((((double)T_End - (double)QRS_onset) / (double)Fs) * 1000.0) - 0.154 * (1 - ((R_Peak.ElementAt(1) - R_Peak.ElementAt(0)) / (double)Fs));
                        break;
                    default:
                        QT_Interval = ((((double)T_End - (double)QRS_onset) / (double)Fs) * 1000) / (Math.Sqrt((R_Peak.ElementAt(1) - R_Peak.ElementAt(0)) / (double)Fs));
                        break;
                }
            }
            else
            {
                QT_Interval = 0;
            }         
            outputData = Tuple.Create(T_End, QT_Interval);
            return outputData;
        }
        public Vector<double> diff(Vector<double> In)
        {
            List<double> Output = new List<double>(In.Count - 1);
            Vector<double> output = Vector<double>.Build.Dense(1);
            double[] ze = { 0 };
            if (In.Count > 1)
            {
                for (int i = 0; i < In.Count - 1; i++)
                {
                    Output.Insert(i, In.ElementAt(i + 1) - In.ElementAt(i));
                    output = Vector<double>.Build.DenseOfArray(Output.ToArray());
                }
            }
            else
            {
                throw new InvalidOperationException("Wrong input vector");
                //output = Vector<double>.Build.DenseOfArray(ze); 
            }
            return output;
        }
    }
    

}
