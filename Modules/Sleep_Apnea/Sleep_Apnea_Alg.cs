using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using EKG_Project.Modules.ECG_Baseline;


namespace EKG_Project.Modules.Sleep_Apnea
{
    #region Sleep_Apnea Class doc
    /// <summary>
    /// Class that locates the periods of sleep apnea in ECG 
    /// </summary>
    #endregion

    public partial class Sleep_Apnea : IModule
    {
        static void Main(string[] args)
        {
            //read data from file
            TempInput.setInputFilePath(@"D:\studia nowe\dadm\projekt\matlabfunkcje\R_det.txt");
            Vector<double> R_detected = TempInput.getSignal();
            uint freq = 100;

            //process
            //finding RR intervals
            //double[,] RR=findIntervals(Vector < double > R_detected, uint freq);

            //average filtering
            //double[,] RR_average=averageFilter(RR);

            //resampling on frequency 1Hz
            //double[,] RR_res=resampling(RR_average, freq);

            //high,low-pass filtering
            //double[,] RR_HPLP=HPLP(RR_res);
        }
        private double [,] findIntervals(Vector <double> R_detected, uint freq)
        {
            double inter;
            double[,] RR = new double[2, R_detected.Count()];
            for (int i = 0; i < R_detected.Count(); i++)
            {
                inter = (R_detected[i + 1] - R_detected[i]) / freq;
                RR[0, i] = R_detected[i];
                RR[1, i] = inter;
            }
            return RR;
        }

        private double [,] averageFilter(Matrix <double> RR)
        {
            int okno = 41;
            int dlugosc = RR.ColumnCount;
            double sum = 0;
            int licznik = 0;

            for (int i = 0; i <= okno; i++)
            {
                if (RR[1, i] > 0.4 && RR[1, i] < 2.0)
                {
                    sum += RR[1, i];
                    licznik += 1;
                }
            }

            double mean = sum / licznik;
            bool[] correct = new bool[RR.ColumnCount];
            for (int i=0; i<(okno-1)/2; i++)
            {
                if (RR[1, i] > 0.8 * mean && RR[1, i] < 1.2 * mean)
                    correct[i] = true;
                else
                    correct[i] = false;
            }

            sum = 0;
            licznik = 0;

            for (int i=(okno+1)/2; i<=dlugosc-(okno-1)/2; i++)
            {
                for (int z=i-(okno-1)/2; z<=i+(okno-1)/2; z++)
                {
                    sum += RR[1, z];
                    licznik += 1;
                }
                mean = sum / licznik;
                if (RR[1, i] > 0.8 * mean && RR[1, i] < 1.2 * mean)
                    correct[i] = true;
                else
                    correct[i] = false;

            }

            sum = 0;
            licznik = 0;

            for (int i=dlugosc-(okno-1)/2; i<=dlugosc; i++)
            {
                if (RR[1,i]>0.4 && RR[1,i]<2.0)
                {
                    sum += RR[1, i];
                    licznik += 1;
                }
            }

            mean = sum / licznik;

            for (int i=dlugosc-(okno-1)/2; i<=dlugosc; i++)
            {
                if (RR[1, i] > 0.8 * mean && RR[1, i] < 1.2 * mean)
                    correct[i] = true;
                else
                    correct[i] = false;
            }

            double[,] RR_average = new double[2,RR.ColumnCount];

            for (int i = 0; i<= dlugosc; i++)
            {
                if (correct[i] == true)
                {
                    RR_average[0, i] = RR[0, i];
                    RR_average[1, i] = RR[1, i];
                }
            }

            for (int i = 0; i<= dlugosc; i++)
            {
                if (correct[i] == false)
                {
                    RR_average[0, i] = RR[0, i];
                    RR_average[1, i] = (RR[0, i-1]+RR[1,i+1])/2;
                }
            }

            return RR_average;
            
        }

        private double[,] resampling(Matrix<double> RR_average, int freq)
        {
            int n_start = Convert.ToInt32(RR_average[0, 1]);
            int n_stop = Convert.ToInt32(RR_average[0, RR_average.ColumnCount - 1]);
            int size = Math.Abs((n_stop - n_start) / freq) + 1;

            double[,] RR_res = new double[2, RR_average.ColumnCount];
            int j = n_start;
            for (int i=0; i< size;i++)
            {
                RR_res[0, i] = j;
                j += freq;
            }

            double tm1 = RR_average[0, 0];
            double tm2 = RR_average[0, 1];
            double rr1 = RR_average[1, 0];
            double rr2 = RR_average[1, 1];
            double a = (rr1 - rr2) / (tm1 - tm2);
            double b = rr1 - a * tm1;
            RR_res[1, 0] = a * RR_res[0, 0] + b;

            if (RR_average[1, RR_average.ColumnCount - 1] == 0)
                RR_average[1, RR_average.ColumnCount - 1] = RR_average[1, RR_average.ColumnCount - 2];

            tm1 = RR_average[0, RR_average.ColumnCount - 2];
            tm2 = RR_average[0, RR_average.ColumnCount - 1];
            rr1 = RR_average[1, RR_average.ColumnCount - 2];
            rr2 = RR_average[1, RR_average.ColumnCount - 1];
            a = (rr1 - rr2) / (tm1 - tm2);
            b = rr1 - a * tm1;
            RR_res[1, size - 1] = a * RR_res[0, size - 1] + b;

            for (int k=1;k<size-1;k++)
            {
                int i = 0;
                while (i<RR_average.ColumnCount-2 && RR_average[0,i]<RR_res[0,k])
                {
                    if (RR_average[0, i + 1] > RR_res[0, k])
                        break;
                    else
                        i = i + 1;
                }

                tm1 = RR_average[0, i];
                tm2 = RR_average[0, i + 1];
                rr1 = RR_average[1, i];
                rr2 = RR_average[1, i + 1];
                a = (rr1 - rr2) / (tm1 - tm2);
                b = rr1 - a * tm2;
                RR_res[1, k] = a * RR_res[0, k] + b;
            }

            return RR_res;
        }

        private double[,] HPLP(Matrix<double> RR_res)
        {
            double[] Z1 = new double[RR_res.ColumnCount];
            for (int i = 0; i < Z1.Length; i++)
                Z1[i] = 0;

            double CUTOFF = 0.01;
            double RC = 1 / (CUTOFF * 2 * 3.14);
            double dt = 1;
            double alpha = RC / (RC + dt);

            for (int j = 1; j < RR_res.ColumnCount; j++)
                Z1[j] = alpha * (Z1[j - 1] + RR_res[1, j] - RR_res[1, j - 1]);

            double[] Z2 = new double[RR_res.ColumnCount];
            for (int i = 0; i < Z2.Length; i++)
                Z2[i] = 0;

            CUTOFF = 0.09;
            RC = 1 / (CUTOFF * 2 * 3.14);
            dt = 1;
            alpha = dt / (RC + dt);

            for (int j = 1; j < Z1.Length; j++)
                Z2[j] = Z2[j - 1] + (alpha * (Z1[j] - Z2[j - 1]));

            double[,] RR_HPLP = new double[2,RR_res.ColumnCount];
            for (int i=0; i<RR_res.ColumnCount;i++)
            {
                RR_HPLP[0, i] = RR_res[0, i];
                RR_HPLP[1, i] = RR_res[1, i];
            }

            return RR_HPLP;
        }

    }
}
