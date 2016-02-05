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
        /// Method that initialize DFA analysis by setting box sizes and obtaining vector of fluctuations
        /// </summary>
        /// <param name="dfaStep"> Interval of box size increase </param>
        /// <param name="start"> The smallest box size</param>
        /// <param name="stop"> The largest box size</param>
        /// <param name="intervals"> Vector of RR intervals</param>
        /// <returns> Tuple of vectors with log(n) and log(F(n))</returns>
        #endregion
        public Tuple<Vector<double>, Vector<double>> ObtainFluctuations(int dfaStep, int start, int stop, Vector<double> intervals)
        {
            double[] BoxRanged = Generate.LinearRange(start, dfaStep, stop);        // set of box sizes
            Vector<double> boxSizeSet = Vector<double>.Build.DenseOfArray(BoxRanged);
            Vector<double> vectorLogN = ConvertToLog(boxSizeSet);

            Vector<double> vectorFn0 = ComputeDfaFluctuation(intervals, BoxRanged);
            Vector<double> vFn = RemoveZeros(vectorFn0);
            Vector<double> vectorLogFn = ConvertToLog(vFn);
            Vector<double> vectorLogn = vectorLogN.SubVector(0, vFn.Count());

            Tuple<Vector<double>, Vector<double>> resultFluctuations = new Tuple<Vector<double>, Vector<double>>(vectorLogn, vectorLogFn);
            return resultFluctuations;
        }
      
        #region
        /// <summary>
        /// Method that performs Detrended Fluctuation Analysis with given intervals and array of box sizes
        /// </summary>
        /// <param name="intervals">Vector of input RR Intervals</param>
        /// <param name="BoxValues">Array of increasing box length</param>
        /// <returns>Vector of computed fluctuations F(n)</returns>
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
        /// Method that performs linear-least-square fitting with fiven fluctuations
        /// </summary>
        /// <param name="fluctuations">Tuple of result vectors from fluctuations obtaining</param>
        /// <param name="longCorrelations"> Boolean value that is connected with intervals length </param>
        /// <returns>Parameters of Analysis </returns>
        #endregion
        public List<Tuple<Vector<double>, Vector<double>>> HRV_DFA_Analysis(Tuple<Vector<double>, Vector<double>> fluctuations, bool longCorrelations)
        {

            Vector<double> vectorLogn = fluctuations.Item1;
            Vector<double> vectorLogFn = fluctuations.Item2;

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

            List<Tuple<Vector<double>, Vector<double>>> output = new List<Tuple<Vector<double>, Vector<double>>>();
            output.Add(resultN);
            output.Add(resultFn);
            output.Add(resultAlpha);
            return output;
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
        #region
        /// <summary>
        /// V.1
        /// Method that merges two vectos into one where vector 2 is putted behind vector1
        /// </summary>
        /// <param name="vector1"> First vector</param>
        /// <param name="vector2"> Secound vector</param>
        /// <returns> Merged vector consisted of values of vector1 and vector2 </returns>
        #endregion
        public Vector<double> CombineVectors(Vector<double> vector1, Vector<double> vector2)
        {
            Vector<double> output = Vector<double>.Build.Dense(vector1.Count + vector2.Count);
            for (int i = 0; i < vector1.Count; i++)
            {
                output[i] = vector1[i];
            }
            for (int i = vector1.Count; i < output.Count; i++)
            {
                output[i] = vector2[i - vector1.Count];
            }

            return output;
        }

        #region
        /// <summary>
        /// V.2
        /// Method that adds double value between two vectors and merge into one vector
        /// </summary>
        /// <param name="vector1"> First vector</param>
        /// <param name="a"> Double value</param>
        /// <param name="vector2"> Secound vector</param>
        /// <returns> Merged vector consisted of values from vector1, double and vector2</returns>
        #endregion
        public Vector<double> CombineVectors(Vector<double> vector1, double a, Vector<double> vector2)
        {
            Vector<double> output = Vector<double>.Build.Dense(vector1.Count + vector2.Count + 1);
            for (int i = 0; i < vector1.Count; i++)
            {
                output[i] = vector1[i];
            }
            output[vector1.Count] = a;
            for (int i = vector1.Count + 1; i < output.Count; i++)
            {
                output[i] = vector2[i - vector1.Count - 1];
            }

            return output;
        }

        #region
        /// <summary>
        /// Method that interpolates RR intervals value in case of failure R peaks detection
        /// </summary>
        /// <param name="input"> RR Intervals</param>
        /// <returns>Interpolated intervals</returns>
        #endregion
        public Vector<double> Interpolate(Vector<double> input)
        {
            Vector<double> output = Vector<double>.Build.Dense(input.Count);
            double avg = input.Average();
            for (int i = 0; i < input.Count; i++)
            {
                if (input[i] > 2 * avg)
                {
                    double newValue = (input[i - 1] + input[i + 1]) / 2;
                    Vector<double> v1 = input.SubVector(0, i - 1);
                    Vector<double> v2 = input.SubVector(i + 1, input.Count - i - 1);
                    output = CombineVectors(v1, newValue, v2);
                }
                else if (input[i] < 0.5 * avg)
                {
                    Vector<double> v1 = input.SubVector(0, i - 1);
                    Vector<double> v2 = input.SubVector(i, input.Count - i);
                    output = CombineVectors(v1, v2);
                }
            }
            return output;
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
