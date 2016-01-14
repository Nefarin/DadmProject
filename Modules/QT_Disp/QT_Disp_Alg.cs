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
        List<int> QRS_onset;                            //to do in init
        List<int> T_End_Global;                         //to do in init
        List<int> QRS_End;                              //to do in init
        Vector<double> R_Peaks;                         //to do in init
        T_End_Method T_End_method;                      //to do in init
        QT_Calc_Method QT_Calc_method;                  //to do in init
        uint Fs;                                        //to do in init
        List<double> QT_intervals;                      //to do in processData
        List<int> T_End_local;                          //to do in processData
        List<List<double>> AllQT_Intervals;             //to do after change a channel
        public QT_Disp()
        {
            QRS_onset = new List<int>();
            T_End_Global = new List<int>();
            QRS_End = new List<int>();
            R_Peaks = Vector<double>.Build.Dense(1);
            T_End_method = new T_End_Method();
            QT_Calc_method = new QT_Calc_Method();
            Fs = new uint();
            QT_intervals = new List<double>(1);
            AllQT_Intervals = new List<List<double>>();
            T_End_local = new List<int>();
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
            this.QRS_onset = QRS_Onset;
            this.QRS_End = QRS_End;
            this.T_End_Global = T_End_Global;
            this.R_Peaks = R_Peaks;
            this.T_End_method = T_End_method;
            this.QT_Calc_method = QT_Calc_method;
            this.Fs = Fs;
        }
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
                    int onset = QRS_onset.ElementAt(1);
                    int end = QRS_End.ElementAt(0);                   
                    if (onset > end )
                    {
                        Test data = new Test(QRS_onset.ElementAt(index), QRS_End.ElementAt(index), T_End_Global.ElementAt(index), samples, QT_Calc_method, T_End_method, Fs, R_peaks_loc);
                        //here we add a QT interval to a list
                        this.QT_INTERVALS.Add(data.Calc_QT_Interval());
                        //here we add a T_End index to a list
                        this.T_End_local.Add(data.FindT_End());
                        // we return a T_End index to save it to the output
                        T_End = data.FindT_End();
                    }                   
                    else
                    {
                        if (index > 1)
                        {                           
                            Test data = new Test(QRS_onset.ElementAt(index), QRS_End.ElementAt(index-1), T_End_Global.ElementAt(index-1), samples, QT_Calc_method, T_End_method, Fs, R_peaks_loc);
                            //here we add a QT interval to a list
                            this.QT_INTERVALS.Add(data.Calc_QT_Interval());
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
            ALL_QT_INTERVALS.Add(temp_ar.ToList());
            QT_INTERVALS.Clear();

        }/// <summary>
        /// This method calculate a global QT dispersion in signal
        /// </summary>
        /// <param name="AmountOFDrains">Number of drains</param>
        /// <returns></returns>
        public double CalculateQT_Disp(int AmountOFDrains)
        {
            // now is not working
            // I am working on it
            // problem due to waves
            // we need the same number of samples in each list containt QRS Onset etc.
            double QT_Disp = 0;
            //here we calculate a global QT_disp froma all drains check it because all above works
            List<double> temp = new List<double>();
            List<double> temp2 = new List<double>();
            List<double> globalQT = new List<double>();
            List<List<double>> QTIntervalsAll = new List<List<double>>();
            QTIntervalsAll = this.ALL_QT_INTERVALS;
            /*for (int i = 0; i < QTIntervalsAll.ElementAt(0).Count; i++)
            {
                foreach (List<double> onedrain in QTIntervalsAll)
                {
                    temp.Add(onedrain.ElementAt(i));
                }
                temp2 = temp.GetRange(i * AmountOFDrains, AmountOFDrains);
                if (!temp2.Contains(0))
                {
                    globalQT.Add(temp2.Max() - temp2.Min());
                }

            }
            QT_Disp = globalQT.Mean();
            globalQT.Clear();
            temp.Clear();*/
            return QT_Disp;
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
        public List<List<double>> ALL_QT_INTERVALS
        {
            get
            {
                return AllQT_Intervals;
            }
            set
            {
                AllQT_Intervals = value;
            }
        }
    }
    //It was writen but not tested, also there is not any comments
    //I get to know about processData asumptions on Saturday 09.01.16
    /// <summary>
    /// This class was written to fulfill a processData assumptions    /// 
    /// </summary>
    class Test
    {
        private int QRS_onset;
        private int QRS_End;
        private int T_End_Global;
        private Vector<double> samples;
        private QT_Calc_Method QT_Calc_method;
        private T_End_Method T_End_method;
        private uint Fs;
        private double[] R_Peak = new double[2];
        public Test(int QRS_onset, int QRS_End, int T_End_Global, Vector<double> samples, QT_Calc_Method QT_Calc_method, T_End_Method T_End_method, uint Fs, double[] R_Peak)
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
        public Test()
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
        /// <returns>QT interval calculated according to chosen formula</returns>
        public double Calc_QT_Interval()
        {
            double QT_Interval =0;
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
                    QT_Interval = ((((double)T_End - (double)QRS_onset) / (double)Fs) * 1000) / Math.Pow(((R_Peak.ElementAt(1) - R_Peak.ElementAt(0)) / (double)Fs), 1 / 3);
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
            return QT_Interval;
        }
        private Vector<double> diff(Vector<double> In)
        {
            List<double> Output = new List<double>(In.Count - 1);
            Vector<double> output = Vector<double>.Build.Dense(1);
            if (Output.Count > 1)
            {
                for (int i = 0; i < In.Count - 1; i++)
                {
                    Output.Insert(i, In.ElementAt(i + 1) - In.ElementAt(i));
                    output = Vector<double>.Build.DenseOfArray(Output.ToArray());
                }
            }
            else
            {
                
            }

            return output;
        }

    }
    //It was written earlier it works but I don't know that we should cut a signal
    //This algorithms get hole signal List<Tuple<String,Vector<double>>> and return output
    //It was tested and it was working, there are also comments to documentation
    //#region Documentation
    ///// <summary>
    ///// This class is created to implements algorithms to find T_end in signal
    ///// </summary>
    //#endregion 
    //class TWave
    //{

    //    private T_End_Method method;                                    //get from Params
    //    private bool alldrains;                                         //get from Params                                      
    //    uint Fs;                                                        //get from Basic Data

    //    private List<int> QRS_End;                                      //get from Waves
    //    private List<int> T_EndGlobal;                                  //get from Waves
    //    private Vector<double> R_Peaks;                                 //get from R_Peaks

    //    private List<Tuple<String,Vector<double>>> AllDrainSignal;      //get from ECG_Baseline

    //    #region Documentation
    //    /// <summary>
    //    /// This constructor set fields of a class
    //    /// </summary>
    //    /// <param name="AllDrainSignal">This is a list of tuple which stores a drain name and a vector signal from ECG_baseline module</param>
    //    /// <param name="T_EndGlobal">This list contains T_End indexes</param>      
    //    /// <param name="QRS_End">This list contains QRS_End indexes</param>      
    //    /// <param name="R_Peaks">Vector that contains a R_Peak indexes</param> 
    //    /// <param name="Fs">A sampling frequency</param>
    //    /// <param name="alldrains">This argument check if the control alldrains in gui is set</param>
    //    /// <param name="method">This argument is a method to find T_End in a signal</param>
    //    #endregion
    //    public TWave(List<Tuple<String, Vector<double>>> AllDrainSignal, List<int> QRS_End, List<int> T_EndGlobal, Vector<double> R_Peaks, uint Fs,T_End_Method method, bool alldrains)
    //    {           

    //        this.method = method;
    //        this.alldrains = alldrains;
    //        this.Fs = Fs;

    //        this.AllDrainSignal = AllDrainSignal;          
    //        this.QRS_End = QRS_End;             
    //        this.T_EndGlobal = T_EndGlobal;
    //        this.R_Peaks = R_Peaks;                     //!!!!! index of R_peak NOT a difference beetwen R Peaks
    //    }
    //    /// <summary>
    //    /// Only for test
    //    /// </summary>
    //    /// <param name="input">Signal</param>
    //    public TWave(List<Tuple<String,Vector<double>>> input)
    //    {
    //        //for testes


    //        int[] qrsend = { 380,676,960,1244,1528 };
    //        int[] tend = { 265,553,835,1119,1410 };
    //        double[] RPeaks = { 80, 373, 666, 950, 1234, 1518 };      

    //        this.QRS_End = qrsend.ToList();
    //        this.AllDrainSignal = input;
    //        this.R_Peaks = Vector<double>.Build.DenseOfArray(RPeaks);


    //        this.T_EndGlobal = tend.ToList();
    //        this.method = T_End_Method.PARABOLA;
    //        this.alldrains = true;
    //        this.Fs = 360;
    //    }

    //    #region Documentation
    //    /// <summary>
    //    /// This function finds T-Max indexes in signal
    //    /// </summary>
    //    /// <param name="signal">A vector with a signal</param>
    //    /// <returns>List that contains T_Max indexes</returns>
    //    #endregion            
    //    private List<int> FindT_Max(Vector<double> signal)
    //    {
    //        //list that stores t max indexes
    //        List<int> T_MaxIndexList = new List<int>();
    //        if (QRS_End.ElementAt(0) < T_EndGlobal.ElementAt(0))
    //        {

    //        }
    //        else
    //        {
    //            this.T_EndGlobal = T_EndGlobal.GetRange(1, this.T_EndGlobal.Count - 1);

    //        }          
    //            //this for goes for every T_end in signal
    //            for (int i = 0; i < T_EndGlobal.Count-2; i++)
    //            {                           

    //                    //check if there is not any bad recognized points            
    //                    if (QRS_End.ElementAt(i) != -1 && T_EndGlobal.ElementAt(i) != -1)
    //                    {

    //                        //create vector from QRS_End to T_End and calulate maximum index
    //                        if (signal.At((int)R_Peaks.At(4)) > 0) //case: positive QRS - the T-end is also positive
    //                        {
    //                            T_MaxIndexList.Add((signal.SubVector(QRS_End.ElementAt(i), T_EndGlobal.ElementAt(i) - QRS_End.ElementAt(i)).MaximumIndex()) + QRS_End.ElementAt(i));
    //                        }
    //                        else //case: QRS negative - T-end would be probably negative as well
    //                        {

    //                            T_MaxIndexList.Add(((signal.SubVector(QRS_End.ElementAt(i), T_EndGlobal.ElementAt(i) - QRS_End.ElementAt(i))).Negate()).MaximumIndex() + QRS_End.ElementAt(i)); // here i wanted to have a opposite vector to the given subvector and then take it's maximum...
    //                        }
    //                    }
    //                    else
    //                    {
    //                        T_MaxIndexList.Add(-1);         //insert -1 for bad recognized points
    //                    }
    //            }           

    //        return T_MaxIndexList;
    //    }

    //    #region Documentation
    //    /// <summary>
    //    /// This function finds max slope indexes in signal after T_max index
    //    /// </summary>
    //    /// <param name="signal">A vector with a signal</param>
    //    /// <returns>List that contains Max Slope indexes</returns>
    //    #endregion
    //    private List<int> Find_MaxSlope(Vector<double> signal)
    //    {
    //        List<int> T_MaxIndex = FindT_Max(signal);
    //        //list that stores t_max slope need to fit a line or a parabola
    //        List<int> T_MaxSlope = new List<int>();

    //        for (int i = 0; i < T_MaxIndex.Count; i++)
    //        {
    //            if (T_EndGlobal.ElementAt(i) != -1 && T_MaxIndex.ElementAt(i) != -1)
    //            {
    //                if (signal.At((int)R_Peaks.At(4)) > 0)
    //                {
    //                    T_MaxSlope.Add(diff(signal.SubVector(T_MaxIndex.ElementAt(i), (T_EndGlobal.ElementAt(i) - T_MaxIndex.ElementAt(i)))).MinimumIndex() + T_MaxIndex.ElementAt(i));
    //                }
    //                else
    //                {
    //                    //case: negative signal (QRS peaks values "-")
    //                    T_MaxSlope.Add(diff((signal.SubVector(T_MaxIndex.ElementAt(i), (T_EndGlobal.ElementAt(i) - T_MaxIndex.ElementAt(i)))).Negate()).MinimumIndex() + T_MaxIndex.ElementAt(i));
    //                }
    //            }
    //            else
    //            {
    //                T_MaxSlope.Add(-1);
    //            }
    //        }
    //        return T_MaxSlope;
    //    }
    //    #region Documentation
    //    /// <summary>
    //    /// This method find T_End inexes according to tangent method
    //    /// </summary>
    //    /// <param name="signal">A vector with a signal</param>
    //    /// <returns>List that contain T_End indexes calculated according to Tangent method</returns>
    //    #endregion
    //    private List<int> TangentMethod(Vector<double> signal)
    //    {
    //        //lsit that stores t_end indexes 
    //        List<int> T_End_Index = new List<int>();
    //        Vector<double> vectorTofit = Vector<double>.Build.Dense(9);
    //        int tempindex = -1;
    //        int j = 0;
    //        double[] x = new double[9];         //variable to store x_data to fit a line
    //        double[] y = new double[9];         //variable to store y_data to fit a line
    //        Tuple<double, double> coefficiants; //variable to store output from fitting , Item1=b, Item2=a , y = ax + b

    //        foreach (int MaxSlope in Find_MaxSlope(signal))
    //        {
    //            if(MaxSlope != -1)
    //            {                    
    //                vectorTofit =  signal.SubVector(MaxSlope-4,9);
    //                y = vectorTofit.ToArray();
    //                for (int i = -4; i < 5; i++)
    //                {
    //                    x[i + 4] = MaxSlope + i;
    //                }
    //                coefficiants = MathNet.Numerics.Fit.Line(x, y);
    //                tempindex = (int)(-coefficiants.Item1 / coefficiants.Item2);
    //                if (tempindex < (T_EndGlobal.ElementAt(j) + (int)( 0.08 * Fs)) && tempindex > (T_EndGlobal.ElementAt(j) - (int)(0.14 * Fs))) //start checking from 40ms s after global T_end
    //                {
    //                    //here we should check if t_end index is correct
    //                    T_End_Index.Add(tempindex);   //finding T_End according to Tangent method
    //                }
    //                else
    //                {
    //                    T_End_Index.Add(T_EndGlobal.ElementAt(j));
    //                }

    //            }
    //            else
    //            {
    //                T_End_Index.Add(-1);
    //            }
    //            j++;
    //        }
    //        return T_End_Index;



    //    }
    //    #region Documentation
    //    /// <summary>
    //    /// This method find T_End indexes according to parabola method
    //    /// </summary>
    //    /// <param name="signal">A vector with a signal</param>
    //    /// <returns>List that contains T_End indexes calculated according to parabola method</returns>
    //    #endregion
    //    private List<int> ParabolaMethod(Vector<double> signal)
    //    {
    //        List<int> T_End_Index = new List<int>();
    //        List<int> T_MaxSlope = Find_MaxSlope(signal);
    //        Vector<double> vectorTofit = Vector<double>.Build.Dense(6);
    //        int[] step = new int[T_EndGlobal.Count];
    //        double delta = new double();
    //        int tempindex = -1;
    //        int j = 0;

    //        //Vector<double> vectorTofit = Vector<double>.Build.Dense(5);

    //        double[] x = new double[7];         //variable to store x_data to fit a polynomial (parabola)
    //        double[] y = new double[7];         //variable to store y_data to fit a polynomial (parabola)
    //        double[] coefficients;              //variable to store output from fitting , Item1=c, Item2=b, Item3=a , y = ax^2 + bx +c

    //        foreach (int MaxSlope in Find_MaxSlope(signal))
    //        {
    //            if (MaxSlope != -1)
    //            {
    //                vectorTofit = signal.SubVector(MaxSlope, 7);
    //                y = vectorTofit.ToArray();
    //                for (int i = 0; i < 7; i++)
    //                {
    //                    x[i] = MaxSlope + i;
    //                }

    //                coefficients = MathNet.Numerics.Fit.Polynomial(x, y, 2); //calculates the coeffs of parabola that fits best the slope
    //                delta = Math.Pow(coefficients[1], 2) - 4 * coefficients[2] * coefficients[0];
    //                tempindex = (int)(-coefficients[1] / (2 * coefficients[2]));
    //                if ((tempindex < (T_EndGlobal.ElementAt(j) + (int)(0.04 * Fs))) && (tempindex > (T_EndGlobal.ElementAt(j) - (int)(0.08 * Fs)))) //start checking from 40ms s after global T_end
    //                {
    //                    //here we should check if t_end index is correct
    //                    T_End_Index.Add(tempindex);   //finding T_End according to Tangent method
    //                }
    //                else
    //                {
    //                    T_End_Index.Add(T_EndGlobal.ElementAt(j));
    //                }
    //            }
    //            else
    //            {
    //                T_End_Index.Add(-1);
    //            }
    //            j++;
    //        }
    //        return T_End_Index;
    //    }
    //    #region Documentation
    //    /// <summary>
    //    /// This function calculate the difference between the next elements in vector
    //    /// </summary>
    //    /// <param name="In">Vector to differitive</param>
    //    /// <returns>Vector after differentiation</returns>
    //    #endregion
    //    private Vector<double> diff(Vector<double> In)
    //    {
    //        List<double> Output = new List<double>(In.Count - 1);

    //        for (int i = 0; i < In.Count - 1; i++)
    //        {
    //            Output.Insert(i, In.ElementAt(i + 1) - In.ElementAt(i));
    //        }

    //        return Vector<double>.Build.DenseOfArray(Output.ToArray());
    //    }
    //    #region Documentation
    //    /// <summary>
    //    /// This function execute all methods to find a T_End indexes in a signal
    //    /// </summary>      
    //    /// <returns>List of tuple that contains a drain name and a list with indexes of T_End according to chose method</returns>
    //    #endregion
    //    public List<Tuple<String,List<int>>> ExecuteTWave()
    //    {
    //        List<Tuple<String, List<int>>> output = new List<Tuple<string, List<int>>>();
    //        //if alldrains in gui i set we calculate t_end index in all drains according to chosen method
    //        if(alldrains == true)
    //        {
    //            for(int i = 0; i<this.AllDrainSignal.Count(); i++)
    //            {
    //                if (this.method == T_End_Method.PARABOLA)
    //                {
    //                     output.Add(Tuple.Create(this.AllDrainSignal.ElementAt(i).Item1,ParabolaMethod(this.AllDrainSignal.ElementAt(i).Item2)));
    //                }
    //                else
    //                {
    //                    output.Add(Tuple.Create(this.AllDrainSignal.ElementAt(i).Item1, TangentMethod(this.AllDrainSignal.ElementAt(i).Item2)));                        
    //                }
    //            }
    //        }
    //        // if alldrains is not set we return T_End indexes detected by Waves
    //        else
    //        {
    //            output.Add(Tuple.Create("Default", T_EndGlobal));                
    //        }
    //        return output;

    //    }

    //}







    ///// <summary>
    ///// This class is to calculate QT intervals statistic and QT disp from one drain and all drains
    ///// </summary>
    //class QTCalculation
    //{
    //    //iput
    //    private List<int> QRS_Onset = new List<int>();       
    //    private Vector<double> RR_interval;
    //    private QT_Calc_Method method;
    //    private double Fs;
    //    private List<Tuple<String, List<int>>> T_End = new List<Tuple<string, List<int>>>();
    //    //output
    //    public List<Tuple<string, double>> meanQTInterval = new List<Tuple<string, double>>();
    //    public List<Tuple<string, double>> stdQTInterval = new List<Tuple<string, double>>();
    //    public List<Tuple<string, double>> localQTdisp = new List<Tuple<string, double>>();

    //    private double globalQTDisp;

    //    int AmountOFDrains;


    //    /// <summary>
    //    /// This constructor creates list of QT_Data such as QRS_Onset, T_End, RR_interval, Fs, drain name, and QT_interval calculation method
    //    /// </summary>
    //    /// <param name="QRS_Onset">This list stores next QRS_Onset indexes</param>
    //    /// <param name="T_End">This list stores tuples which is a drain name and list with T-End inexes</param>
    //    /// <param name="RR_interval">This list stores next RR Intervals in signal in ms</param>
    //    /// <param name="Fs">This is frequency sampling ih Hz</param>
    //    /// <param name="method">This is a formula, which we want to calculate a QT_interval</param>
    //    public QTCalculation(List<int> QRS_Onset, Vector<double> RR_interval, QT_Calc_Method method,double Fs, List<Tuple<String,List<int>>> T_End)
    //    {
    //        this.Fs = Fs;
    //        this.method = method;
    //        this.QRS_Onset = QRS_Onset;
    //        this.T_End = T_End;
    //        this.RR_interval = RR_interval;
    //        this.AmountOFDrains = this.T_End.Count;

    //    }
    //    /// <summary>
    //    /// only for test
    //    /// </summary>
    //    public QTCalculation()
    //    {
    //        int[] qrs_onset = { 558, 920, 1407, 1855 };
    //        int[] qrs_end = { 779, 1176, 1604, 2032 };
    //        int[] t_end = { 750, 1300, 1654, 2067 };
    //        double[] rr = { 958.33, 1152.78, 1238.89, 1113.78 };


    //        this.QRS_Onset = qrs_onset.ToList();
    //        this.T_End = new List<Tuple<String, List<int>>>(); qrs_end.ToList();
    //        this.RR_interval = Vector<double>.Build.DenseOfArray(rr);
    //        this.Fs = 360;
    //        Tuple<String, List<int>> drain1 = Tuple.Create("V2", qrs_end.ToList());
    //        Tuple<String, List<int>> drain2 = Tuple.Create("V5", t_end.ToList());
    //        this.T_End.Add(drain1);
    //        this.T_End.Add(drain2);
    //        this.method = QT_Calc_Method.BAZETTA;
    //        this.AmountOFDrains = this.T_End.Count;
    //    }

    //    /// <summary>
    //    /// This method calulate QT_intervals and statistic and QT_Disp
    //    /// </summary>
    //    public void setOutput()
    //    {
    //        List<List<double>> QTIntervalsAll = new List<List<double>>();
    //        List<double> zero = new List<double>(1);
    //        zero.Add(0);
    //        List<double> QTIntervals = new List<double>();                  //stores qt intervals from one drain
    //        double[] mean = new double[this.AmountOFDrains];                //stores mean qt intervals from next drain
    //        double[] std = new double[this.AmountOFDrains];                 //stores standard deviation from next drain
    //        double[] local = new double[this.AmountOFDrains];               //stores locals qt_disp
    //        int j = 0;
    //        int start = 0;
    //        int koniec = 0;

    //        //adding qt intervals calculated according to chosen formula 
    //        foreach (Tuple<String,List<int>> T_EndInDrain in T_End)
    //        {
    //            if (method == QT_Calc_Method.BAZETTA)
    //            {
    //                if (QRS_Onset.ElementAt(0) < T_EndInDrain.Item2.ElementAt(0))
    //                {
    //                    for (int i = 0; i < QRS_Onset.Count; i++)
    //                    {
    //                        if (QRS_Onset.ElementAt(i) != -1 && (T_EndInDrain.Item2.ElementAt(i) != -1))
    //                        {                                
    //                            QTIntervals.Add((((T_EndInDrain.Item2.ElementAt(i) - QRS_Onset.ElementAt(i)) / Fs) * 1000) / Math.Sqrt(RR_interval.ElementAt(i) / 1000));
    //                        }
    //                        else
    //                        {
    //                            QTIntervals.Add(0);
    //                        }

    //                    }
    //                }
    //            }
    //            if (method == QT_Calc_Method.FRIDERICA)
    //            {
    //                if (QRS_Onset.ElementAt(0) < T_EndInDrain.Item2.ElementAt(0))
    //                {
    //                    for (int i = 0; i < QRS_Onset.Count; i++)
    //                    {
    //                        if (QRS_Onset.ElementAt(i) != -1 && (T_EndInDrain.Item2.ElementAt(i) != -1))
    //                        {
    //                            QTIntervals.Add((((T_EndInDrain.Item2.ElementAt(i) - QRS_Onset.ElementAt(i)) / Fs) * 1000) / Math.Sqrt(RR_interval.ElementAt(i) / 1000));
    //                        }
    //                        else
    //                        {
    //                            QTIntervals.Add(0);
    //                        }

    //                    }

    //                }
    //            }
    //            if (method == QT_Calc_Method.FRAMIGHAMA)
    //            {
    //                if (QRS_Onset.ElementAt(0) < T_EndInDrain.Item2.ElementAt(0))
    //                {
    //                    for (int i = 0; i < QRS_Onset.Count; i++)
    //                    {
    //                        if (QRS_Onset.ElementAt(i) != -1 && (T_EndInDrain.Item2.ElementAt(i) != -1))
    //                        {
    //                            QTIntervals.Add((((T_EndInDrain.Item2.ElementAt(i) - QRS_Onset.ElementAt(i)) / Fs) * 1000) / Math.Sqrt(RR_interval.ElementAt(i) / 1000));
    //                        }
    //                        else
    //                        {
    //                            QTIntervals.Add(0);
    //                        }

    //                    }
    //                }
    //            }
    //            //calculate mean we count all intervals expect 0 because 0 means that there was a bad recognition

    //            mean[j] = QTIntervals.Sum() / (QTIntervals.Except(zero)).Count();
    //            local[j] = QTIntervals.Max() - QTIntervals.Except(zero).Min();
    //            std[j] = (QTIntervals.Except(zero)).StandardDeviation();
    //            koniec = QTIntervals.Count;
    //            QTIntervalsAll.Add(QTIntervals.GetRange(start,koniec-start));        //this list strores all qt intervals from all drain
    //            j++;
    //            start = koniec;
    //            //QTIntervals.Clear();                    //can't do it because it's a reference
    //        }
    //        //here we create data to send them to the GUI to show them in table
    //        for (int i = 0; i < AmountOFDrains; i++)
    //        {

    //            this.localQTdisp.Add(Tuple.Create(T_End.ElementAt(i).Item1, local.ElementAt(i)));
    //            this.meanQTInterval.Add(Tuple.Create(T_End.ElementAt(i).Item1, mean.ToList().ElementAt(i)));
    //            this.stdQTInterval.Add(Tuple.Create(T_End.ElementAt(i).Item1, std.ToList().ElementAt(i)));

    //        }
    //        //here we calculate a global QT_disp froma all drains   check it because all above works
    //        List<double> temp = new List<double>();
    //        List<double> temp2 = new List<double>();
    //        List<double> globalQT = new List<double>();
    //        for (int i = 0; i < QTIntervalsAll.ElementAt(0).Count; i++)
    //        {
    //            foreach (List<double> onedrain in QTIntervalsAll)
    //            {
    //                temp.Add(onedrain.ElementAt(i));
    //            }
    //            temp2 = temp.GetRange(i * this.AmountOFDrains, AmountOFDrains);
    //            if (!temp2.Contains(0))
    //            {
    //                globalQT.Add(temp2.Max() - temp2.Min());
    //            }

    //        }
    //        this.globalQTDisp = globalQT.Mean();
    //        globalQT.Clear();
    //        temp.Clear();

    //    }

    //}

}
