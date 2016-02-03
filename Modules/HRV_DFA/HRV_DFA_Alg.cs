using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.IO;

namespace EKG_Project.Modules.HRV_DFA
{
    

    public class HRV_DFA_Alg
    {
        Vector<double> veclogn1;
        Vector<double> veclogn2;
        Vector<double> veclogFn1;
        Vector<double> veclogFn2;
        Vector<double> vecparam1;
        Vector<double> vecparam2;
        Vector<double> logN;
        Vector<double> logFn;

        
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
        }
        /*
         public void HRV_DFA_Analysis(Vector<double> sig)
         {
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

             veclogn1 = ln1;
             veclogn2 = ln2;
             veclogFn1 = lineShortRange;
             veclogFn2 = lineLongRange;
             vecparam1 = p1;
             vecparam2 = p2;
            logN = vectorLogn;
            logFn = vectorLogFn;

        }*/

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

        public double[] CumulativeSum(double[] a)
        {
            double sum = 0;
            double[] b = a.Select(x => (sum += x)).ToArray();
            return b;
        }
      
        public Vector<double> IntegrateDfa (Vector<double> intervals)
        {
            double average = intervals.Sum() / intervals.Count();
            Vector<double> a = intervals.Subtract(average);
            Vector<double> b = a.PointwiseMultiply(a);      
            double[] sum = CumulativeSum(b.ToArray());

            Vector<double> output = Vector<double>.Build.DenseOfArray(sum);
            return output;
        }

        public Vector<double> ConvertToLog(Vector<double> input)
        {
           Vector<double> output = Vector<double>.Build.Dense(input.Count());
            for(int i = 0; i < input.Count(); i++)
            {
                output[i] = Math.Log10(input[i]);
            }
            return output;
        }

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


    }
}
