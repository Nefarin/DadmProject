using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;

namespace EKG_Project.Modules.HRV_DFA
{


    public partial class HRV_DFA : IModule
    {

        public static void Main(string[] args)
        {
            HRV_DFA dfa = new HRV_DFA();

            //read data from file
            TempInput.setInputFilePath(@"C:\Users\Paulina\Desktop\DADM\RR_100.txt");
            uint fs = TempInput.getFrequency();
            Vector<double> sig = TempInput.getSignal();

            // DFA box parameters
            int step = 50;
            int start = 50;
            int stop = 50000; 

            // DFA - fluctuation funcion computation
            double[] boxRanged = Generate.LinearRange(start, step, stop);       // set of box sizes
            Vector<double> box = Vector<double>.Build.DenseOfArray(boxRanged);  // n
            Vector<double> vectorFn = dfa.DfaFluctuationComputation(box, sig);  // F(n)

            // Convert to logarytmic scale
            Vector<double> logn = box.PointwiseLog();
            Vector<double> logFn = vectorFn.PointwiseLog();

            // short-range:long-range fitting bending data proportion
            double proportion = 0.33;
            int q = Convert.ToInt32(logn.Count() * proportion);
            // short - range correlations
            Vector<double> logn1 = logn.SubVector(0, q);
            Vector<double> logFn1 = logFn.SubVector(0, q);
            double[] logn1a = logn1.ToArray();
            double[] logFn1a = logFn1.ToArray();
            double[] p1 = Fit.Polynomial(logn1a, logFn1a, 1);

            Vector<double> fittedFn1 = Vector<double>.Build.Dense(logn1.Count());
            Func<double, double> fitting1 = Fit.PolynomialFunc(logn1a, logFn1a, 1, MathNet.Numerics.LinearRegression.DirectRegressionMethod.NormalEquations);
            //fitting curve obtaining for short-range correlations
            for (int k = 0; k < logn1.Count(); k++)
            {
                fittedFn1[k] = fitting1(logn1a[k]);
            }
            //long - range correlations
            Vector<double> logn2 = logn.SubVector( q, logn.Count() - q );
            Vector<double> logFn2 = logFn.SubVector(q, logn.Count() - q );
            double[] logn2a = logn2.ToArray();
            double[] logFn2a = logFn2.ToArray();
            double[] p2 = Fit.Polynomial(logn2a, logFn2a, 1);

            Vector<double> fittedFn2 = Vector<double>.Build.Dense(logn2.Count());
            Func<double, double> fitting2 = Fit.PolynomialFunc(logn2a, logFn2a, 1, MathNet.Numerics.LinearRegression.DirectRegressionMethod.NormalEquations);
            //fitting curve obtaining for short-range correlations
            for (int k = 0; k < logn1.Count(); k++)
            {
                fittedFn2[k] = fitting2(logn2a[k]);
            }


            Console.WriteLine(fittedFn1.ToString());
            Console.WriteLine(fittedFn2.ToString());
            Console.ReadKey();

        }
     
        // METHODS
        //function that integrates signal
        public Vector<double> Integrate(Vector<double> signal_rr)
        {
            Vector<double> signal_integrated = Vector<double>.Build.Dense(signal_rr.Count(), 0);

            //Average
            double rr_avg = signal_rr.Sum() / signal_rr.Count;

            for (int i = 0; i < signal_rr.Count - 1 ; i++)
            {
                signal_integrated[0] = 0;
                signal_integrated[i + 1] = signal_rr[i] - rr_avg;
                signal_integrated[i + 1] += signal_integrated[i];
                signal_integrated[i + 1] = Math.Abs(signal_integrated[i + 1]);
            }

            return signal_integrated;
        }
        
        // Method that computates in-box fluctuations F in given box size 
        public double InBoxFluctuations(Vector<double> y_integrated, Vector<double> y_fitted, double box_quantity)
        {
            Vector<double> y_subtracted = y_integrated.Subtract(y_fitted);

            Vector<double> y_sub_pow = Vector<double>.Build.Dense(y_subtracted.Count());
            for (int i = 0; i < y_subtracted.Count(); i++)
            {
                y_sub_pow[i] = y_subtracted[i] * y_subtracted[i];
            }
            double fn = Math.Sqrt(y_sub_pow.Sum()/box_quantity);

            return fn;
        }

        // Method that returs vector F(n) of Fluctuation Analysis results 
        public Vector<double> DfaFluctuationComputation(Vector<double> dfabox, Vector<double> signal )
        {
            HRV_DFA dfaFn = new HRV_DFA();
            int box_length = dfabox.Count();    // number of all boxes
            int sig_length = signal.Count();    // signal length
            Vector<double> fn = Vector<double>.Build.Dense(box_length);

            for (int i = 0; i < box_length - 1 ; i++)
            {
                double boxVal = dfabox[i];
                double box_number = sig_length / boxVal;   // number of boxes
                double box_qtyD = box_number * boxVal;      // quantity of samples in boxes
                int boxValint = Convert.ToInt32(boxVal);
                int box_qty = Convert.ToInt32(box_qtyD);
                int boxNumber = Convert.ToInt32(box_number);

                // Signal integration
                Vector<double> yk = dfaFn.Integrate(signal);
                

                for (int j = 0; j < boxNumber-1; j++)
                {
                    // Least-Square Fitting
                    int ykIndex = j * boxValint;
                    if (ykIndex+boxValint > sig_length)
                    {
                        break;
                    }
                    int ykCount = boxValint;
                    double[] x = Generate.LinearRange(1, boxValint);            //ok
                    Vector<double> y = yk.SubVector(ykIndex, boxValint);         //ok
                    double[] y1 = y.ToArray();
                    double[] p = Fit.Polynomial(x, y1, 1);     //fitting coefficients
                    // Fitting method: NormalEquations                                         
                    Func<double, double> fitting = Fit.PolynomialFunc(x, y1, 1, MathNet.Numerics.LinearRegression.DirectRegressionMethod.NormalEquations);
                    Vector<double> yn = Vector<double>.Build.Dense(yk.Count());
                    
                    //fitting curve obtaining
                    for (int k = 0; k < y.Count(); k++)
                    {
                        yn[k] = fitting(x[k]);
                    }
                    // dfa fluctuation function F(n)
                    fn[i] = dfaFn.InBoxFluctuations(yk, yn, box_qtyD);
                }
            }
            return fn;
        }


    }
}
