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
    public class QT_Disp_Alg
    {
        List<double> QRS_onset;                            //to do in init
        List<double> T_End_Global;                         //to do in init
        List<double> QRS_End;                              //to do in init
        Vector<double> R_Peaks;                         //to do in init
        T_End_Method T_End_method;                      //to do in init
        QT_Calc_Method QT_Calc_method;                  //to do in init
        uint Fs;                                        //to do in init
        List<double> QT_intervals;                      //to do in processData
        List<int> T_End_local;                          //to do in processData
        //List<List<double>> AllQT_Intervals;             //to do after change a channel
        /*public QT_Disp_Alg()
        {
            QRS_onset = new List<int>();
            T_End_Global = new List<int>();
            QRS_End = new List<int>();
            R_Peaks = Vector<double>.Build.Dense(1);
            T_End_method = new T_End_Method();
            QT_Calc_method = new QT_Calc_Method();
            Fs = new uint();
            QT_intervals = new List<double>(1);
            //AllQT_Intervals = new List<List<double>>();
            T_End_local = new List<int>(1);
        }*/
        public QT_Disp_Alg()
        {
            TempInput.setInputFilePath("C:\\Users\\Pawel\\Desktop\\QRS_Onset.txt");
            this.Fs = 360;
            this.QRS_onset = TempInput.getSignal().ToList();
            TempInput.setInputFilePath("C:\\Users\\Pawel\\Desktop\\QRS_End.txt");
            this.QRS_End = TempInput.getSignal().ToList();
            TempInput.setInputFilePath("C:\\Users\\Pawel\\Desktop\\R_Peak.txt");
            this.R_Peaks = TempInput.getSignal();
            TempInput.setInputFilePath("C:\\Users\\Pawel\\Desktop\\T_End.txt");
            this.T_End_Global = TempInput.getSignal().ToList();
            this.T_End_method = T_End_Method.TANGENT;
            this.QT_Calc_method = QT_Calc_Method.FRAMIGHAMA;
            this.QT_intervals = new List<double>();
            this.T_End_local = new List<int>();
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
        /*public void TODoInInit(List<int> QRS_Onset, List<int> T_End_Global, List<int> QRS_End, Vector<double> R_Peaks, T_End_Method T_End_method, QT_Calc_Method QT_Calc_method, uint Fs)
        {
            this.QRS_onset = QRS_Onset;
            this.QRS_End = QRS_End;
            this.T_End_Global = T_End_Global;
            this.R_Peaks = R_Peaks;
            this.T_End_method = T_End_method;
            this.QT_Calc_method = QT_Calc_method;
            this.Fs = Fs;
        }*/
        /// <summary>
        /// This function is called to get a current sampled signal and processed it 
        /// </summary>
        /// <param name="samples">Vector with a cut signal</param>
        /// <param name="index">An index of a current R peak</param>
        /// <returns>T End local index</returns>
        public int ToDoInProccessData(Vector<double> samples, int index)
        {
            int T_End = -1;
            //check if we exceed a size of R _ Peaks
            if (index < (R_Peaks.Count - 2))
            {
                // create a table with a R_peaks 
                //between this indexes we lookig for T_End 
                double[] R_peaks_loc = new double[2];
                R_peaks_loc[0] = R_Peaks[index];
                R_peaks_loc[1] = R_Peaks[index+1];                
                // create new object that stores a indexes of points from a Waves module
                // need this point to find T_end
                if(index<T_End_Global.Count)
                {                   
                    int onset = (int)QRS_onset.ElementAt(1);
                    int end =(int) QRS_End.ElementAt(0);                   
                    if (onset > end )
                    {
                        DataToCalculate data = new DataToCalculate((int)QRS_onset.ElementAt(index), (int)QRS_End.ElementAt(index), (int)T_End_Global.ElementAt(index), samples, QT_Calc_method, T_End_method, Fs, R_peaks_loc);
                        //here we add a QT interval to a list
                        this.QT_INTERVALS.Add(data.Calc_QT_Interval().Item2);
                        //here we add a T_End index to a list
                        this.T_End_local.Add(data.FindT_End());
                        // we return a T_End index to save it to the output
                        T_End = data.FindT_End();
                    }                   
                    else
                    {
                        if (index > 1)
                        {                           
                            DataToCalculate data = new DataToCalculate((int)QRS_onset.ElementAt(index),(int) QRS_End.ElementAt(index-1),(int)T_End_Global.ElementAt(index-1), samples, QT_Calc_method, T_End_method, Fs, R_peaks_loc);
                            //here we add a QT interval to a list
                            this.QT_INTERVALS.Add(data.Calc_QT_Interval().Item2);
                            //here we add a T_End index to a list
                            this.T_End_local.Add(data.FindT_End());
                            // we return a T_End index to save it to the output
                            T_End = data.FindT_End();
                        }
                        
                    }
                   
                }
                
            }
            // return a T_End index if bad recognition assign -1
            return T_End;
        }
        /// <summary>
        /// This method calculate a mean QT interval from one drain
        /// </summary>
        /// <returns>QT interval mean</returns>
        public double getMean()
        {
            //creat a list with zero element to compare
            List<double> zero = new List<double>(1);
            zero.Add(0);
            // here we store a mean value
            double mean;
            // chceck if a drain was calculate correct            
            if(QT_INTERVALS.Except(zero).Count() == 0)
            {
                // QT interval list include only element with zero values so something go wrong if we are here
                mean = 0;
            }
            else
            {
                // if there is some elements we calculate a mean
                // we count only good values so we count all QT interval expext 0
                mean = QT_intervals.Sum() / QT_intervals.Except(zero).Count();
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
            //creat a list with zero element to compare
            List<double> zero = new List<double>(1);
            zero.Add(0);
            // chceck if a drain was calculate correct      
            if (QT_INTERVALS.Except(zero).Count() == 0)
            {
                // QT interval list include only element with zero values so something go wrong if we are here
                std = 0;
            }
            else
            {
                // if there is some elements we calculate a standard deviation
                // we get only good values so we count all QT interval expext 0
                std = QT_intervals.Except(zero).StandardDeviation();
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
                local = QT_intervals.Max() - QT_intervals.Except(zero).Min();
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
            // we should copy a list because there is a reference
            List<double> temp = new List<double>(QT_INTERVALS.Count);
            double[] temp_ar = new double[QT_INTERVALS.Count]; 
            QT_INTERVALS.CopyTo(temp_ar);
            // this list stores all QT intervals from all drains
           
            QT_INTERVALS.Clear();

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
        public static void Main()
        {
            Console.WriteLine("Hello world");
            
            Vector<double> signal;
            Vector<double> samples;
            //TempInput input = new TempInput();
            TempInput.setInputFilePath("C:\\Users\\Pawel\\Documents\\MATLAB\\DADM\\projekt\\wyniki.txt");
            
            signal = TempInput.getSignal();
            QT_Disp_Alg data = new QT_Disp_Alg();
            List<Tuple<int, double>> output = new List<Tuple<int, double>>();
            double[] t_end = new double[22];
            double[] qt_interval = new double[22];
            for(int i=0; i<data.R_Peaks.Count-1; i++)
            {
                double[] r_peakss = new double[] { data.R_Peaks.ElementAt(i), data.R_Peaks.ElementAt(i + 1) }; 
                samples = signal.SubVector((int)data.R_Peaks.ElementAt(i), 
                    (int)(data.R_Peaks.ElementAt(i + 1) - data.R_Peaks.ElementAt(i)));
                DataToCalculate data_samples = new DataToCalculate((int)data.QRS_onset.ElementAt(i), 
                    (int)data.QRS_End.ElementAt(i), (int)data.T_End_Global.ElementAt(i),
                    samples, data.QT_Calc_method, data.T_End_method, data.Fs, r_peakss);
                output.Add(data_samples.Calc_QT_Interval());
                t_end[i] = data_samples.Calc_QT_Interval().Item1;
                qt_interval[i] = data_samples.Calc_QT_Interval().Item2;
            }
            TempInput.setOutputFilePath("C:\\Users\\Pawel\\Desktop\\output_end.txt");
            Vector<double> file = Vector<double>.Build.DenseOfArray(t_end);
            TempInput.writeFile(360, file);
            TempInput.setOutputFilePath("C:\\Users\\Pawel\\Desktop\\output_qt.txt");
            TempInput.writeFile(360, Vector<double>.Build.DenseOfArray(qt_interval));

            Console.ReadKey();
        }
        
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
            int T_End = 0;
            int T_Max = 0;
            int MaxSlope = 0;
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
                        T_Max = samples.SubVector(QRS_End - (int)R_Peak.ElementAt(0), (T_End_Global - QRS_End)).MaximumIndex() + QRS_End;

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
                        T_Max = samples.SubVector(QRS_End - (int)R_Peak.ElementAt(0), T_End_Global - QRS_End).Negate().MaximumIndex() + QRS_End;
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
                if (samples.ElementAt((int)(R_Peak.ElementAt(1) - R_Peak.ElementAt(0)) - 1) > 0)
                {
                    // if yes we calculate a differantion beetwen T_Max and T_End global
                    // then we get a minimum index to find a point with a max slope
                    MaxSlope = diff(samples.SubVector(T_Max - (int)R_Peak.ElementAt(0), T_End_Global - T_Max)).MinimumIndex() + T_Max;
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
            if (QT_Calc_method == QT_Calc_Method.BAZETTA)
            {
                if (QRS_onset != -1 && (T_End != -1))
                {
                    QT_Interval = ((((double)T_End - (double)QRS_onset) / (double)Fs) * 1000) / (Math.Sqrt((R_Peak.ElementAt(1) - R_Peak.ElementAt(0)) / (double)Fs));
                }
                else
                {
                    QT_Interval = 0;
                }
            }
            if (QT_Calc_method == QT_Calc_Method.FRIDERICA)
            {
                if (QRS_onset != -1 && (T_End != -1))
                {
                    QT_Interval = ((((double)T_End - (double)QRS_onset) / (double)Fs) * 1000.0) / Math.Pow(((R_Peak.ElementAt(1) - R_Peak.ElementAt(0)) / (double)Fs), 1 / 3);
                }
                else
                {
                    QT_Interval = 0;
                }
            }
            if (QT_Calc_method == QT_Calc_Method.FRAMIGHAMA)
            {
                if (QRS_onset != -1 && (T_End != -1))
                {
                    QT_Interval = ((((double)T_End - (double)QRS_onset )/ (double)Fs) * 1000.0) - 0.154 * (1 - ((R_Peak.ElementAt(1) - R_Peak.ElementAt(0)) / (double)Fs));
                }
                else
                {
                    QT_Interval = 0;
                }
            }
            outputData = Tuple.Create(T_End, QT_Interval);
            return outputData;
        }
        public Vector<double> diff(Vector<double> In)
        {
            List<double> Output = new List<double>(In.Count - 1);
            Vector<double> output = Vector<double>.Build.Dense(1);
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
            }

            return output;
        }

    }
    

}
