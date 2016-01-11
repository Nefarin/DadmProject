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

        public void HRV_DFA_Analysis()
        {
            HRV_DFA dfa = new HRV_DFA();

            Vector<double> sig = InputData.Signals[_currentChannelIndex].Item2;

            //read data from file
            //TempInput.setInputFilePath(@"C:\Users\Paulina\Desktop\DADM\RR_100.txt");
            //uint fs = TempInput.getFrequency();
            //Vector<double> sig = TempInput.getSignal();

            // DFA box parameters
            int step = 10;
            int start = 100;
            int stop = 10000;

            // DFA - fluctuation funcion computation
            double[] boxRanged = Generate.LinearRange(start, step, stop);       // set of box sizes
            Vector<double> box = Vector<double>.Build.DenseOfArray(boxRanged);  // n
            Vector<double> vectorFn = dfa.DfaFluctuationComputation(box, sig);  // F(n)

            // Remove all zeros from vector
            Vector<double> vFn = dfa.ZerosRemove(vectorFn);
            Vector<double> vn = box.SubVector(0, vFn.Count());

            // Convert to logarytmic scale
            //Vector<double> vn = box.SubVector(0,vFn.Count());
            Vector<double> logn = dfa.Logarithmize(vn);
            Vector<double> logFn = dfa.Logarithmize(vFn);

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
            Func<double, double> fitting1 = Fit.PolynomialFunc(logn1a, logFn1a, 1, MathNet.Numerics.LinearRegression.DirectRegressionMethod.QR);
            //fitting curve obtaining for short-range correlations
            for (int k = 0; k < logn1.Count(); k++)
            {
                fittedFn1[k] = fitting1(logn1a[k]);
            }
            //long - range correlations
            Vector<double> logn2 = logn.SubVector(q, logn.Count() - q);
            Vector<double> logFn2 = logFn.SubVector(q, logFn.Count() - q);
            double[] logn2a = logn2.ToArray();
            double[] logFn2a = logFn2.ToArray();
            double[] p2 = Fit.Polynomial(logn2a, logFn2a, 1);

            Vector<double> fittedFn2 = Vector<double>.Build.Dense(logFn2.Count());
            Func<double, double> fitting2 = Fit.PolynomialFunc(logn2a, logFn2a, 1, MathNet.Numerics.LinearRegression.DirectRegressionMethod.QR);
            //fitting curve obtaining for short-range correlations
            for (int k = 0; k < logFn2.Count(); k++)
            {
                fittedFn2[k] = fitting2(logn2a[k]);
            }

            Console.WriteLine(sig);

        }

        // METHODS:

        // Method that returs vector F(n) of Fluctuation Analysis results 
        public Vector<double> DfaFluctuationComputation(Vector<double> dfabox, Vector<double> signal)
        {
            HRV_DFA dfaFn = new HRV_DFA();
            int box_length = dfabox.Count();    // number of all boxes
            int sig_length = signal.Count();    // signal length
            Vector<double> fn = Vector<double>.Build.Dense(box_length);

            for (int i = 0; i < box_length - 1; i++)
            {
                double boxVal = dfabox[i];
                double box_number = sig_length / boxVal;   // number of boxes
                double box_qtyD = box_number * boxVal;      // quantity of samples in boxes
                int boxValint = Convert.ToInt32(boxVal);
                int box_qty = Convert.ToInt32(box_qtyD);
                int boxNumber = Convert.ToInt32(box_number);

                Vector<double> yk = dfaFn.Integrate(signal);    // Signal integration

                for (int j = 0; j < boxNumber - 1; j++)
                {
                    // Least-Square Fitting
                    int ykIndex = j * boxValint;
                    if (ykIndex + boxValint > sig_length)
                    {
                        break;
                    }
                    int ykCount = boxValint;
                    double[] x = Generate.LinearRange(1, boxValint);
                    Vector<double> y = yk.SubVector(ykIndex, boxValint);
                    double[] y1 = y.ToArray();
                    double[] p = Fit.Polynomial(x, y1, 1);     //fitting coefficients
                    // Fitting method: NormalEquations                                         
                    Func<double, double> fitting = Fit.PolynomialFunc(x, y1, 1, MathNet.Numerics.LinearRegression.DirectRegressionMethod.QR);

                    //fitting curve obtaining
                    Vector<double> yn = Vector<double>.Build.Dense(yk.Count());
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

        // Method that computates in-box fluctuations F in given box size 
        public double InBoxFluctuations(Vector<double> y_integrated, Vector<double> y_fitted, double box_quantity)
        {

            Vector<double> y_subtracted = Vector<double>.Build.Dense(y_fitted.Count());
            for (int i = 0; i < y_integrated.Count(); i++)
            {
                y_subtracted[i] = y_integrated[i] - y_fitted[i];
            }

            Vector<double> y_sub_pow = Vector<double>.Build.Dense(y_subtracted.Count());
            for (int i = 0; i < y_subtracted.Count(); i++)
            {
                y_sub_pow[i] = y_subtracted[i] * y_subtracted[i];
            }
            double fn = Math.Sqrt(y_sub_pow.Sum() / box_quantity);

            return fn;
        }

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
            for (int i = 0; i < vectorIn.Count(); i++)
            {
                if (vectorIn[i] != 0)
                {
                    vectorIn[i] = vectorIn[i];
                }
                else
                {
                    counter = i;
                    break;
                }
            }
            Vector<double> vectorOut = vectorIn.SubVector(0, counter);
            return vectorOut;
        }

    }
}
