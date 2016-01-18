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
    
    public partial class HRV_DFA : IModule
    {
        Vector<double> veclogn1;
        Vector<double> veclogn2;
        Vector<double> veclogFn1;
        Vector<double> veclogFn2;
        Vector<double> vecparam1;
        Vector<double> vecparam2;
        private int boxVal;
        private int startValue;
        private int stepVal;
        private bool longCorrelations;

        public void HRV_DFA_Analysis(Vector<double> rRRIntervals, int stepValue, int boxValue)
        {
            Vector<double> sig = rRRIntervals;

            //read data from file
            //TempInput.setInputFilePath(@"C:\Users\Paulina\Desktop\DADM\RR_100.txt");
            //uint fs = TempInput.getFrequency();
            //Vector<double> sig = TempInput.getSignal();

            // DFA box parameters
            int dfastep = stepValue;
            int start = startValue;
            int stop = boxValue; 

            // DFA - fluctuation funcion computation
            double[] boxRanged = Generate.LinearRange(start, dfastep, stop);        // set of box sizes
            Vector<double> box = Vector<double>.Build.DenseOfArray(boxRanged);      // n
            Vector<double> vectorFn = DfaFluctuationComputation(box, sig);          // F(n)

            // Remove all zeros from vector
            Vector<double> vFn = ZerosRemove(vectorFn);
            Vector<double> vn = box.SubVector(0, vFn.Count());

            // Convert to logarytmic scale
            Vector<double> logn = Logarithmize(vn);
            Vector<double> logFn = Logarithmize(vFn);

            // Arrays to Fit.Line
            double[] logna = logn.ToArray();
            double[] logFna = logFn.ToArray();

            // short-range:long-range fitting bending data proportion
            double proportion;
            if (longCorrelations)
            {
                proportion = 0.33;
            }
            else
            {
                proportion = 1;
            }
            int q = Convert.ToInt32(logn.Count() * proportion);

            // vectors init
            Vector<double> p1 = Vector<double>.Build.Dense(2);
            Vector<double> logn1 = logn.SubVector(0, q);
            Vector<double> logFn1 = logFn.SubVector(0, q);
            Vector<double> fittedFn1 = Vector<double>.Build.Dense(logn1.Count());

            Vector<double> p2 = Vector<double>.Build.Dense(p1.Count());
            Vector<double> logn2 = Vector<double>.Build.Dense(logn.Count());
            Vector<double> logFn2 = Vector<double>.Build.Dense(logFn.Count());
            Vector<double> fittedFn2 = Vector<double>.Build.Dense(logn.Count());

            if (longCorrelations)
            {
                // short - range correlations
                double[] logn1a = logn1.ToArray();
                double[] logFn1a = logFn1.ToArray();
                // coefficents to obtain a,b from y = a * x + b
                Tuple<double,double> p01 = Fit.Line(logn1a, logFn1a);
                p1[0] = p01.Item1;     // b
                p1[1] = p01.Item2;     // a
                Func<double, double> fitting1 = Fit.LineFunc(logn1a, logFn1a);
                //fitting curve obtaining for short-range correlations
                for (int k = 0; k < logn1.Count(); k++)
                {
                    fittedFn1[k] = fitting1(logn1a[k]);
                }
                //long - range correlations
                logn2 = logn.SubVector(q, logn.Count() - q);
                logFn2 = logFn.SubVector(q, logFn.Count() - q);
                double[] logn2a = logn2.ToArray();
                double[] logFn2a = logFn2.ToArray();
                // coefficents to obtain a,b from y = a * x + b
                Tuple<double,double> p02 = Fit.Line(logn2a, logFn2a);
                p2[0] = p02.Item1;     // b
                p2[1] = p02.Item2;     // a
                // Line function obtained with Least Square Method
                Func<double, double> fitting2 = Fit.LineFunc(logn2a, logFn2a);
                //fitting curve obtaining for long-range correlations
                for (int k = 0; k < logFn2.Count(); k++)
                {
                    fittedFn2[k] = fitting2(logn2a[k]);
                }
            }
            else
            {
                // coefficents to obtain a,b from y = a * x + b
                Tuple<double, double> p01 = Fit.Line(logna, logFna);
                p1[0] = p01.Item1;     // b
                p1[1] = p01.Item2;     // a
                // Line function obtained with Least Square Method
                Func<double, double> fitting1 = Fit.LineFunc(logna, logFna);    //=PolynomialFunc(logna, logFna, 1, MathNet.Numerics.LinearRegression.DirectRegressionMethod.QR);
                //fitted line obtaining for short-range correlations
                for (int k = 0; k < logn.Count(); k++)
                {
                    fittedFn1[k] = fitting1(logna[k]);
                }
            }
            // Outputs
            veclogn1 = logn1;
            veclogn2 = logn2;
            veclogFn1 = fittedFn1;
            veclogFn2 = fittedFn2;
            vecparam1 = p1;
            vecparam2 = p2;
        }

        // METHODS:
        // Method that returs vector F(n) of Fluctuation Analysis results 
        public Vector<double> DfaFluctuationComputation(Vector<double> dfabox, Vector<double> signal)
        {
            
            int box_length = dfabox.Count();    // number of all boxes
            int sig_length = signal.Count();    // signal length

            Vector<double> fn = Vector<double>.Build.Dense(box_length);

            for (int i = 0; i < box_length - 1; i++)
            {
                double boxVal = dfabox[i];
                int boxValint = Convert.ToInt32(boxVal);
                int nWin = sig_length / boxValint;
                int winCount = nWin * Convert.ToInt32(boxVal);

                if (boxValint > sig_length)
                   {
                        fn[i] = 0;
                   }
                else
                {
                    Vector<double> yk = Vector<double>.Build.Dense(winCount, 1);
                    Vector<double> yn = Vector<double>.Build.Dense(winCount, 1);

                    yk = Integrate(signal.SubVector(0, winCount));

                    for (int j = 0; j < nWin - 1; j++)
                    {
                        // Least-Square Fitting
                        int ykIndex = j * boxValint;

                        double[] x = Generate.LinearRange(1, 1, boxValint);
                        double[] y = yk.SubVector(ykIndex, boxValint).ToArray();

                        // Line function                                    
                        Func<double, double> fitting = Fit.LineFunc(x,y); 
  
                        //fitting line obtaining
                        for (int k = 0; k < y.Count() - 1; k++)
                        {
                            yn[k] = fitting(x[k]);
                        }
                        // dfa fluctuation function F(n)
                        fn[i] = InBoxFluctuations(yk, yn, winCount);
                    }
                }
            }
            return fn;
        }

        // Method that computates in-box fluctuations F in given box size 
        public double InBoxFluctuations(Vector<double> y_integrated, Vector<double> y_fitted, int box_quantity)
        {
            Vector<double> y_k = y_integrated.SubVector(0, box_quantity);
            Vector<double> y_n = y_fitted.SubVector(0, box_quantity);

            Vector<double> y_sub = Vector<double>.Build.Dense(y_n.Count());

            double vector_add = 0;
            vector_add = Math.Pow(y_k[0] - y_n[0], 2);
            y_sub[0] = vector_add;

            for (int i = 1; i < y_k.Count()-1; i++)
            {
                y_sub[i] = Math.Pow(y_k[i] - y_n[i],2);
                vector_add = y_sub[i];
            }

            double fn = Math.Sqrt(y_sub.Sum() / box_quantity);

            return fn;
        }

        //function that integrates signal
        public Vector<double> Integrate(Vector<double> signal_rr)
        {
            Vector<double> signal_integrated = Vector<double>.Build.Dense(signal_rr.Count(), 0);
            double signal_add = 0; 

            //Average
            double rr_avg = signal_rr.Sum() / signal_rr.Count;

            signal_add = Math.Abs(signal_rr[0] - rr_avg);
            signal_integrated[0] = signal_add;

            for (int i = 1; i < signal_rr.Count - 1 ; i++)
            {
                signal_integrated[i] = Math.Abs(signal_rr[i] - rr_avg)+ signal_add;
                signal_add = signal_integrated[i];
            }

            return signal_integrated;
        }

        // Least - square approximation 
        public double[] Polyfit(double[] x, double[] y, int degree)
        {
            // Vandermonde matrix
            var X = new DenseMatrix(x.Length, 1 + degree); ;
            var yy = new DenseVector(y);

            // solve
            var p = X.QR().Solve(yy);

            return p.ToArray();
        }

        // Method that logarithmizes signal
        public Vector<double> Logarithmize(Vector<double> signal)
        {
            Vector<double> logSig = Vector<double>.Build.Dense(signal.Count());
            for (int i = 0; i < signal.Count(); i++)
            {
                logSig[i] = Math.Log10(signal[i]);
            }
            return logSig;
        }

        // Method that removes zeros from vector
        public Vector<double> ZerosRemove(Vector<double> vectorIn)
        {
            int counter = 0;
            for (int i = 0; i < vectorIn.Count()-1; i++)
            {
                if (vectorIn[i] != 0)
                {
                    vectorIn[i] = vectorIn[i];
                    counter = i;
                }
                else
                {
                    counter = i;
                    break;
                }
            }
            Vector<double> vectorOut = vectorIn.SubVector(0,counter);
            return vectorOut;
        }
    }   
}
