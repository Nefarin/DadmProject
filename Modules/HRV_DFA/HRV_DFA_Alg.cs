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

        static void Main(string[] args)
        {
            //read data from file
            TempInput.setInputFilePath(@"C:\Users\Paulina\Desktop\inervals\16265rr.txt");
            uint fs = TempInput.getFrequency();
            Vector<double> sig = TempInput.getSignal();

            int dfastep = 50;
            int start = 500;
            int stop = 5000;
            HRV_DFA_Alg dfa = new HRV_DFA_Alg();

            double[] BoxRanged = Generate.LinearRange(start, dfastep, stop);        // set of box sizes
            Vector<double> boxSizeSet = Vector<double>.Build.DenseOfArray(BoxRanged);

            
            Vector<double> vectorFn0 = dfa.ComputeDfaFluctuation(sig, BoxRanged);
            Vector<double> vectorLogN = dfa.ConvertToLog(boxSizeSet);

            Vector<double> vFn = dfa.RemoveZeros(vectorFn0);
            Vector<double> vectorLogFn = dfa.ConvertToLog(vFn);
            Vector<double> vectorLog_n = vectorLogN.SubVector(0, vFn.Count());

            // Convert to logarytmic scale

            // short-range:long-range fitting bending data proportion
            bool longCorrelations;
            if (sig.Count() > 1500)
            {longCorrelations = true;}
            else{longCorrelations = false;}

            double proportion;
            if (longCorrelations)
            { proportion = 0.33;}
            else{proportion = 1;}

            int q = Convert.ToInt32(vectorLog_n.Count() * proportion);
            // vectors init
            Vector<double> p1 = Vector<double>.Build.Dense(2);
            Vector<double> logn1 = vectorLog_n.SubVector(0, q);
            Vector<double> logFn1 = vectorLogFn.SubVector(0, q);
            Vector<double> lineShortRange = Vector<double>.Build.Dense(logFn1.Count());

            Vector<double> p2 = Vector<double>.Build.Dense(p1.Count());
            Vector<double> logn2 = Vector<double>.Build.Dense(vectorLog_n.Count());
            Vector<double> logFn2 = Vector<double>.Build.Dense(vectorLogFn.Count());
            Vector<double> lineLongRange = Vector<double>.Build.Dense(vectorLogFn.Count());

            if (longCorrelations)
            {
                // short - range correlations
                Tuple<double[], Vector<double>> shortRange = dfa.LinearSquare(logn1, logFn1, logn1.Count());
                p1 = Vector<double>.Build.DenseOfArray(shortRange.Item1);
                //fitting curve obtaining for short-range correlations
                lineShortRange = shortRange.Item2;

                //long - range correlations
                logn2 = vectorLog_n.SubVector(q, vectorLog_n.Count() - q);
                logFn2 = vectorLogFn.SubVector(q, vectorLogFn.Count() - q);
                Tuple<double[], Vector<double>> longRange = dfa.LinearSquare(logn2, logFn2, logn2.Count());
                p2 = Vector<double>.Build.DenseOfArray(longRange.Item1);
                //fitting curve obtaining for short-range correlations
                lineLongRange = longRange.Item2;
            }
            else
            {
                // short - range correlations
                Tuple<double[], Vector<double>> shortRange = dfa.LinearSquare(logn1, logFn1, logn1.Count());
                p1 = Vector<double>.Build.DenseOfArray(shortRange.Item1);
                //fitting curve obtaining for short-range correlations
                lineShortRange = shortRange.Item2;
            }

            // Outputs
            dfa.veclogn1 = logn1;
            dfa.veclogn2 = logn2;
            dfa.veclogFn1 =lineShortRange;
            dfa.veclogFn2 = lineLongRange;
            dfa.vecparam1 = p1;
            dfa.vecparam2 = p2;

            uint lth = Convert.ToUInt32(sig.Count());
            
            //
            //write result to dat file
            TempInput.setOutputFilePath(@"C:\Users\Paulina\Desktop\inervals\result.txt");
            TempInput.writeFile(lth, vectorLogFn);
        }
    
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
                    //Vector<double> yk = Vector<double>.Build.Dense(winCount, 1);
                    Vector<double> yk = IntegrateDfa(intervals.SubVector(0, winCount));
                    Vector<double> yn = Vector<double>.Build.Dense(winCount, 1);

                    //yk = integrated.SubVector(0, winCount);

                    for (int j = 0; j < nWin - 1; j++)
                    {
                        // Least-Square Fitting
                        int ykIndex = j * boxValint;

                        double[] xx = Generate.LinearRange(1, 1, winCount);
                        Vector<double> x = Vector<double>.Build.DenseOfArray(xx);
                        Vector<double> y = yk;

                        // Line function                                    
                        Tuple<double[], Vector<double>> localTrend = LinearSquare(x, y, boxVal);
                        yn = localTrend.Item2;
                        //yk = yk.SubVector(0, boxValint);
                        // dfa fluctuation function F(n)
                        fluctuations[i] = ComputeInBox(yk, yn, winCount, sig_length);
                    }
                }
            }
            return fluctuations;
        }

        public double ComputeInBox(Vector<double> y_integrated, Vector<double> y_fitted, int box_quantity, int intervals_length)
        {
            Vector<double> y_k = y_integrated;
            Vector<double> y_n = y_fitted;

            Vector<double> y_sub = Vector<double>.Build.Dense(y_n.Count());

            double vector_add = 0;
            vector_add = Math.Pow(y_k[0] - y_n[0], 2);
            y_sub[0] = vector_add;

            for (int i = 1; i < y_k.Count() - 1; i++)
            {
                y_sub[i] = Math.Pow(y_k[i] - y_n[i], 2);
                vector_add = y_sub[i];
            }

            double fn = Math.Sqrt(y_sub.Sum() / intervals_length);

            return fn;
        }


        public Vector<double> IntegrateDfa(Vector<double> intervals)
        {
            Vector<double> output = Vector<double>.Build.Dense(intervals.Count());
            double average = intervals.Sum() / intervals.Count();

            double signal_add = 0;
            signal_add = intervals[0] - average;
            output[0] = signal_add;

            for (int i = 1; i < intervals.Count - 1; i++)
            {
                output[i] = intervals[i] - average + signal_add;
                signal_add = output[i];
            }
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

        public Tuple<double[],Vector<double>> LinearSquare(Vector<double> x, Vector<double> y, double boxValue)
        {
            Vector<double> y_approx = Vector<double>.Build.Dense(y.Count());
            double boxValue2 = y.Count();
            double[] P = new double[2];
            double sumX = x.Sum();
            double sumX2 = x.PointwiseMultiply(x).Sum();
            double sumY = y.Sum();
            Vector<double> xy = x.PointwiseMultiply(y);
            double sumXY = (xy).Sum();
            double pa = (boxValue2 * sumXY - sumX * sumY) / (boxValue2 * sumX2 - Math.Pow(sumX, 2));
            double pb = (sumY*sumX2 - sumX * sumXY) / (boxValue2 * sumX2 - Math.Pow(sumX, 2));
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
            for (int i = 0; i < input.Count() - 1; i++)
            {
                if (input[i] != 0)
                {
                    input[i] = input[i];
                    counter = i;
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
