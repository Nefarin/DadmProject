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

            //read data from file
            TempInput.setInputFilePath(@"C:\Users\Paulina\Desktop\DADM\RR_100.txt");
            uint fs = TempInput.getFrequency();
            Vector<double> sig = TempInput.getSignal();
            
            HRV_DFA dfa = new HRV_DFA();

            int step = 50;
            int start = 50;
            int stop = 50000; 

            double[] boxRanged = Generate.LinearRange(start, step, stop);
            Vector<double> box = Vector<double>.Build.DenseOfArray(boxRanged);

            Console.WriteLine(fs);
            Console.WriteLine(box);
            Console.ReadKey();

        }

        // METHODS
        //function that integrates signal
        public Vector<double> Integrate(Vector<double> signal_rr)
        {
            Vector<double> signal_integrated = Vector<double>.Build.Dense(signal_rr.Count(), 0);

            //Average
            double rr_avg = signal_rr.Sum() / signal_rr.Count;

            for (int i = 0; i < signal_rr.Count - 1; i++)
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
            int box_length = dfabox.Count();    // number of all boxes
            int sig_length = signal.Count();    // signal length
            Vector<double> fn = Vector<double>.Build.Dense(box_length);

            for (int i = 0; i < box_length; i++)
            {
                HRV_DFA dfaFn = new HRV_DFA();
                double boxVal = dfabox[i];
                double box_number = sig_length / boxVal;   // number of boxes
                double box_qtyD = box_number * boxVal;      // quantity of samples in boxes
                int boxValint = Convert.ToInt32(boxVal);
                int box_qty = Convert.ToInt32(box_qtyD);

                // Signal integration
                Vector<double> yk = dfaFn.Integrate(signal);

                for (int j = 0; j < box_number; j++)
                {
                    // Least-Square Fitting
                    int ykIndex = j * boxValint;
                    int ykCount = (j + 1) * boxValint - ykIndex;
                    double[] x = Generate.LinearRange(1, boxValint);
                    Vector<double> y = yk.SubVector(ykIndex, ykCount);
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
