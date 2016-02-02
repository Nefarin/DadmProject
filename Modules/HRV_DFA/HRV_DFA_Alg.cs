using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.HRV_DFA
{
    #region HRV_DFA class documentation
    /// <summary>
    /// Class that performs Detrended Fluctuations Analysis on given RR Interval
    /// </summary>
    #endregion 
    public class HRV_DFA_Alg
    {
        #region
        /// <summary>
        /// Store input vector of RR Intervals
        /// </summary>
        #endregion
        private Vector<Double> _currentVector;
        #region
        /// <summary>
        /// Store result values of log(n), where n is alternate box size
        /// Item1 is for short range correlations and Item2 is for long range correlations
        /// </summary>
        #endregion
        private Tuple<Vector<double>, Vector<double>> _resultN;
        #region
        /// <summary>
        /// Store result values of log(F(n)), where F(n) is value of fluctuations in alternate box size
        /// Item1 is for short range correlations and Item2 is for long range correlations
        /// </summary>
        #endregion
        private Tuple<Vector<double>, Vector<double>> _resultFn;
        #region
        /// <summary>
        /// Store result values of parameters alpha - alpha is coefficient of fluctuation
        /// Item1 is for short range correlations and Item2 is for long range correlations
        /// </summary>
        #endregion
        private Tuple<Vector<double>, Vector<double>> _resultAlpha;
        #region
        /// <summary>
        /// Store result values of obtained fluctuations in whole range
        /// Item1 is for log(n) and Item2 is for log(F(n))
        /// </summary>
        #endregion
        private Tuple<Vector<double>, Vector<double>> _resultFluctuations;

        public Vector<double> CurrentVector
        {
            get
            {return _currentVector;}
            set
            {if (value == null) throw new ArgumentNullException();
                _currentVector = value;}
        }

        #region
        /// <summary>
        /// Main method of HRV_DFA_Alg that performs main computations and sets DFA parameters
        /// </summary>
        /// <param name="sig"> Input vector of RR Intervals </param>
        #endregion
        public HRV_DFA_Alg(Vector<double> sig)
         {
            CurrentVector = sig;

            bool longCorrelations;
            int dfastep;
            int start;
            int stop;
            if (sig.Count() > 60000)
            {
                longCorrelations = true;
                dfastep = 100;
                start = 500;
                stop = 50000; 
            }
            else if (sig.Count() > 2750 && sig.Count() < 60000)
            {
                longCorrelations = true;
                dfastep = 50;
                start = 500;
                stop = 5000;
            }
            else
            {
                longCorrelations = false;
                dfastep = 50;
                start = 500;
                stop = 5000;
            }

             double[] BoxRanged = Generate.LinearRange(start, dfastep, stop);        // set of box sizes
             Vector<double> boxSizeSet = Vector<double>.Build.DenseOfArray(BoxRanged);
             Vector<double> vectorLogN = ConvertToLog(boxSizeSet);

             Vector<double> vectorFn0 = ComputeDfaFluctuation(sig, BoxRanged);
             Vector<double> vFn = RemoveZeros(vectorFn0);
             Vector<double> vectorLogFn = ConvertToLog(vFn);
             Vector<double> vectorLogn = vectorLogN.SubVector(0, vFn.Count());
             
            // vectors init
            Vector<double> ln1 = Vector<double>.Build.Dense(vectorLogn.Count);
            Vector<double> lFn1 = Vector<double>.Build.Dense(vectorLogFn.Count);
            Vector<double> ln2 = Vector<double>.Build.Dense(vectorLogn.Count);
            Vector<double> lFn2 = Vector<double>.Build.Dense(vectorLogFn.Count);
            Vector<double> lineShortRange = Vector<double>.Build.Dense(lFn1.Count());
            Vector<double> lineLongRange = Vector<double>.Build.Dense(lFn2.Count());
            Vector<double> p1 = Vector<double>.Build.Dense(2);
            Vector<double> p2 = Vector<double>.Build.Dense(2);

            if (longCorrelations)
            {
                int q = 55;
                int qq = vectorLogFn.Count() - q;
                ln1 = vectorLogn.SubVector(0, q);
                lFn1 = vectorLogFn.SubVector(0, q);
                ln2 = vectorLogn.SubVector(q, qq);
                lFn2 = vectorLogFn.SubVector(q, qq);
                lineShortRange = Vector<double>.Build.Dense(lFn1.Count());
                lineLongRange = Vector<double>.Build.Dense(lFn2.Count());
                p1 = Vector<double>.Build.Dense(2);
                p2 = Vector<double>.Build.Dense(2);
                // short - range correlations
                Tuple<double[], Vector<double>> shortRange = LinearSquare(ln1, lFn1);
                p1 = Vector<double>.Build.DenseOfArray(shortRange.Item1);
                //fitting curve obtaining for short-range correlations
                lineShortRange = shortRange.Item2;

                //long - range correlations
                Tuple<double[], Vector<double>> longRange = LinearSquare(ln2, lFn2);
                p2 = Vector<double>.Build.DenseOfArray(longRange.Item1);
                //fitting curve obtaining for short-range correlations
                lineLongRange = longRange.Item2;
            }
            else
            {
                ln1 = vectorLogn;
                lFn1 = vectorLogFn;
                Tuple<double[], Vector<double>> shortRange = LinearSquare(ln1, lFn1);
                Vector<double> p = Vector<double>.Build.DenseOfArray(shortRange.Item1);
                //fitting curve obtaining for short-range correlations
                lineShortRange = shortRange.Item2;
            }

            Tuple<Vector<double>, Vector<double>> resultN = new Tuple<Vector<double>, Vector<double>>(ln1, ln2);
            Tuple<Vector<double>, Vector<double>> resultFn = new Tuple<Vector<double>, Vector<double>>(lineShortRange, lineLongRange);
            Tuple<Vector<double>, Vector<double>> resultAlpha = new Tuple<Vector<double>, Vector<double>>(p1, p2);
            Tuple<Vector<double>, Vector<double>> resultFluctuations = new Tuple<Vector<double>, Vector<double>>(vectorLogn, vectorLogFn);

        }
        #region
        /// <summary>
        /// Method that performs Detrended Fluctuation Analysis with given intervals and array of box sizes
        /// </summary>
        /// <param name="intervals">Vector of input RR Intervals</param>
        /// <param name="BoxValues">Array of increasing box length</param>
        /// <returns>Vector of computed fluctuations</returns>
        #endregion
        public Vector<double> ComputeDfaFluctuation(Vector<double> intervals, double[] BoxValues )
        {
            Vector<double> fluctuations = Vector<double>.Build.Dense(BoxValues.Count());

            int box_length = BoxValues.Count();    // number of all boxes
            int sig_length = intervals.Count();    // signal length
            
            for (int i = 0; i < box_length - 1; i++)
            {
                double boxVal = BoxValues[i];
                int boxValint = Convert.ToInt32(boxVal);
                int nWin = sig_length / boxValint;
                int winCount = nWin * Convert.ToInt32(boxVal);

                if (boxValint > sig_length)
                {
                    fluctuations[i] = 0;
                }
                else
                {
                    Vector<double> yk = IntegrateDfa(intervals.SubVector(0, winCount));
                    Vector<double> yn = Vector<double>.Build.Dense(winCount, 1);

                    for (int j = 0; j < nWin - 1; j++)
                    {
                        // Least-Square Fitting
                        int ykIndex = (j+1) * boxValint;
            
                        double[] xx = Generate.LinearRange(1, 1, boxValint);
                        Vector<double> x = Vector<double>.Build.DenseOfArray(xx);
                        Vector<double> y = yk.SubVector(ykIndex, boxValint);

                        // Line function                                    
                        Tuple<double[], Vector<double>> localTrend = LinearSquare(x, y);
                        yn = localTrend.Item2;

                        // dfa fluctuation function F(n)
                        fluctuations[i] = ComputeInBox(y, yn, sig_length);
                    }
                }
            }
            return fluctuations;
        }

        #region
        /// <summary>
        /// Method that returns fluctuations computed in single box
        /// </summary>
        /// <param name="y_integrated">Vector of integrated intervals</param>
        /// <param name="y_fitted">Vector of local trend inside single box</param>
        /// <param name="intervals_length">Intervals quantity</param>
        /// <returns>Single value of fluctuation</returns>
        #endregion
        public double ComputeInBox(Vector<double> y_integrated, Vector<double> y_fitted, int intervals_length)
        {
            Vector<double> y_k = y_integrated;
            Vector<double> y_n = y_fitted;

            Vector<double> subtraction = y_k.Subtract(y_n);
            double[] power = subtraction.PointwiseMultiply(subtraction).ToArray();
            double[] y_sub = CumulativeSum(power);

            double fn = Math.Sqrt(y_sub.Sum() / intervals_length);

            Vector<double> potega = Vector<double>.Build.DenseOfArray(power);
            Vector<double> suma = Vector<double>.Build.DenseOfArray(y_sub);

            return fn;
        }

        #region
        /// <summary>
        /// Method that computes cumulative sum
        /// </summary>
        /// <param name="a">Array of values to operate on</param>
        /// <returns>Array of result cumulative sum computations</returns>
        #endregion
        public double[] CumulativeSum(double[] a)
        {
            double sum = 0;
            double[] b = a.Select(x => (sum += x)).ToArray();
            return b;
        }

        #region
        /// <summary>
        /// Method that integrates signal
        /// </summary>
        /// <param name="intervals">Vector of RR intervals to integrate</param>
        /// <returns>Vector of integrated signal</returns>
        #endregion
        public Vector<double> IntegrateDfa(Vector<double> intervals)
        {
            double average = intervals.Sum() / intervals.Count();
            Vector<double> a = intervals.Subtract(average);
            Vector<double> b = a.PointwiseMultiply(a);      
            double[] sum = CumulativeSum(b.ToArray());

            Vector<double> output = Vector<double>.Build.DenseOfArray(sum);
            return output;
        }

        #region
        /// <summary>
        /// Method that logarythmizes values in vector
        /// </summary>
        /// <param name="input">Vector to logarythmize</param>
        /// <returns>Vector of values converted to logarythmic scale</returns>
        #endregion
        public Vector<double> ConvertToLog(Vector<double> input)
        {
           Vector<double> output = Vector<double>.Build.Dense(input.Count());
            for(int i = 0; i < input.Count(); i++)
            {
                output[i] = Math.Log10(input[i]);
            }
            return output;
        }

        #region
        /// <summary>
        /// Method that fits line function to given values and returns coefficients of line
        /// Fitting to line y = pa * x + pb
        /// </summary>
        /// <param name="x">Vector of x values </param>
        /// <param name="y">Vector of y values </param>
        /// <returns>Array of coefficients, and Vector of Y values for fitted line </returns>
        #endregion
        public Tuple<double[],Vector<double>> LinearSquare(Vector<double> x, Vector<double> y)
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
            double pb = (sumY*sumX2 - sumX * sumXY) / (n * sumX2 - Math.Pow(sumX, 2));
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
        /// Method that removes zeros from Vector with zeros at the end 
        /// </summary>
        /// <param name="input">Vector with zeros</param>
        /// <returns>Vector vith no zeros inside</returns>
        #endregion
        public Vector<double> RemoveZeros(Vector<double> input)
        {
            int counter = 0;
            for (int i = 0; i < input.Count(); i++)
            {
                if (input[i] != 0)
                {
                    input[i] = input[i];
                    counter = i+1;
                }
                else
                {
                    counter = i;
                    break;
                }
            }
            Vector<double> vectorOut = input.SubVector(0, counter);
            return vectorOut;
        }

        public Tuple<Vector<double>, Vector<double>> ResultN
        {
            get
            { return _resultN; }
            set
            { _resultN = value; }
        }

        public Tuple<Vector<double>, Vector<double>> ResultFn
        {
            get
            { return _resultFn; }

            set
            { _resultFn = value; }
        }

        public Tuple<Vector<double>, Vector<double>> ResultAlpha
        {
            get
            { return _resultAlpha; }
            set
            { _resultAlpha = value; }
        }

        public Tuple<Vector<double>, Vector<double>> ResultFluctuations
        {
            get
            { return _resultFluctuations; }
            set
            { _resultFluctuations = value; }
        }
        /*
       static void Main(string[] args)
       {
           //read data from file
           TempInput.setInputFilePath(@"C:\Users\Paulina\Desktop\inervals\wyniki.txt");
           uint fs = TempInput.getFrequency();
           Vector<double> sig = TempInput.getSignal();
           HRV_DFA_Alg dfa = new HRV_DFA_Alg();

           bool longCorrelations;
           int dfastep;
           int start;
           int stop;
           string state;
           if (sig.Count() > 60000)
           {
               longCorrelations = true;
               dfastep = 100;
               start = 500;
               stop = 50000;
               state = "bending";
           }
           else if (sig.Count() > 2750 && sig.Count() < 60000)
           {
               longCorrelations = true;
               dfastep = 50;
               start = 500;
               stop = 5000;
               state = "bending";
           }
           else
           {
               longCorrelations = false;
               dfastep = 50;
               start = 500;
               stop = 5000;
               state = "no_bending";
           }

           double[] BoxRanged = Generate.LinearRange(start, dfastep, stop);        // set of box sizes
           Vector<double> boxSizeSet = Vector<double>.Build.DenseOfArray(BoxRanged);
           Vector<double> vectorLogN = dfa.ConvertToLog(boxSizeSet);

           Vector<double> vectorFn0 = dfa.ComputeDfaFluctuation(sig, BoxRanged);
           Vector<double> vFn = dfa.RemoveZeros(vectorFn0);
           Vector<double> vectorLogFn = dfa.ConvertToLog(vFn);
           Vector<double> vectorLogn = vectorLogN.SubVector(0, vFn.Count());


           // vectors init
           Vector<double> ln1 = Vector<double>.Build.Dense(vectorLogn.Count);
           Vector<double> lFn1 = Vector<double>.Build.Dense(vectorLogFn.Count);
           Vector<double> ln2 = Vector<double>.Build.Dense(vectorLogn.Count);
           Vector<double> lFn2 = Vector<double>.Build.Dense(vectorLogFn.Count);
           Vector<double> lineShortRange = Vector<double>.Build.Dense(lFn1.Count());
           Vector<double> lineLongRange = Vector<double>.Build.Dense(lFn2.Count());
           Vector<double> p1 = Vector<double>.Build.Dense(2);
           Vector<double> p2 = Vector<double>.Build.Dense(2);

           if (longCorrelations)
           {
               int q = 55;
               int qq = vectorLogFn.Count() - q;
               ln1 = vectorLogn.SubVector(0, q);
               lFn1 = vectorLogFn.SubVector(0, q);
               ln2 = vectorLogn.SubVector(q, qq);
               lFn2 = vectorLogFn.SubVector(q, qq);
               lineShortRange = Vector<double>.Build.Dense(lFn1.Count());
               lineLongRange = Vector<double>.Build.Dense(lFn2.Count());
               p1 = Vector<double>.Build.Dense(2);
               p2 = Vector<double>.Build.Dense(2);
               // short - range correlations
               Tuple<double[], Vector<double>> shortRange = dfa.LinearSquare(ln1, lFn1);
               p1 = Vector<double>.Build.DenseOfArray(shortRange.Item1);
               //fitting curve obtaining for short-range correlations
               lineShortRange = shortRange.Item2;

               //long - range correlations
               Tuple<double[], Vector<double>> longRange = dfa.LinearSquare(ln2, lFn2);
               p2 = Vector<double>.Build.DenseOfArray(longRange.Item1);
               //fitting curve obtaining for short-range correlations
               lineLongRange = longRange.Item2;
           }
           else
           {
               ln1 = vectorLogn;
               lFn1 = vectorLogFn;
               Tuple<double[], Vector<double>> shortRange = dfa.LinearSquare(ln1, lFn1);
               Vector<double> p = Vector<double>.Build.DenseOfArray(shortRange.Item1);
               //fitting curve obtaining for short-range correlations
               lineShortRange = shortRange.Item2;
           }
           //
           //write result to dat file
           TempInput.setOutputFilePath(@"C:\Users\Paulina\Desktop\inervals\result.txt");
           TempInput.writeFile(fs, ln1, lineShortRange);
       }*/
    }
}
