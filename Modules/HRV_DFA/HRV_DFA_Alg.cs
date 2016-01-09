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

           // Box_StepSize stepSize = new Box_StepSize;

            int step = 50;
            int start = 50;
            int stop = 50000; 

            double[] boxRanged = Generate.LinearRange(start, step, stop);
            Vector<double> box = Vector<double>.Build.DenseOfArray(boxRanged);

            // samplesOrder obtaining
            Vector<double> samplesOrder = dfa.Ordering(sig);

            Console.WriteLine(fs);
            Console.WriteLine(box);
            //Console.WriteLine(sig_integrated);
            Console.ReadKey();

        }

        // METHODS

        //function for samples ordering
        public Vector<double> Ordering(Vector<double> signal_rr)
        {
            // samplesOrder obtaining
            Vector<double> samplesOrder = Vector<double>.Build.Dense(signal_rr.Count(), 0);
            for (int i = 0; i < samplesOrder.Count; i++)
            {
                samplesOrder[i] = i;
            }
            return samplesOrder;
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


        public void DfaFluctuationComputation(Vector<double> dfabox, Vector<double> signal )
        {
            int box_length = dfabox.Count();    // number of all boxes
            int sig_length = signal.Count();    // signal length

            for(int i = 0; i < box_length; i++)
            {
                double boxVal = dfabox[i];
                double box_number = sig_length / boxVal ;   // number of boxes
                double box_qtyD = box_number * boxVal;      // quantity of samples in boxes
                int boxValint = Convert.ToInt32(boxVal);
                int box_qty = Convert.ToInt32(box_qtyD);

               // Vector<double> yk = Vector<double>.Build.Dense(box_qty);
                //Vector<double> yn = Vector<double>.Build.Dense(box_qty);

                // Signal integration
                Vector<double> yk = Integrate(signal);
                Vector<double> fn = Vector<double>.Build.Dense(box_length);

                for (int j = 0; j < box_number; j++)
                {
                    // Least-Square Fitting
                    int ykIndex = ((j - 1) * boxValint + 1);
                    int ykCount = j * boxValint - ykIndex;
                    double[] x = Generate.LinearRange(1, boxValint);
                    double[] y = yk.SubVector(ykIndex, ykCount).ToArray();
                    double[] p = Fit.Polynomial(x, y, 1);     //fitting coefficients
                    //Fitting curve
                    Func<double, double> yn = Fit.PolynomialFunc(x, y, 1, MathNet.Numerics.LinearRegression.DirectRegressionMethod.NormalEquations);
                    // dfa fluctuation F(n)
                    //fn[i] = 


                }



            }
            

        }
    

    }
}
