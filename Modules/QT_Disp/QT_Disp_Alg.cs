using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Differentiation;
using EKG_Project.IO;
using MathNet.Numerics.Statistics;
            

namespace EKG_Project.Modules.QT_Disp
{
    public partial class QT_Disp : IModule
    {
        static void Main()
        {

            //Console.WriteLine("Test");
            //TempInput.setInputFilePath("D:\\DADM_Project\\dane.txt");
            //TempInput.setOutputFilePath("D:\\DADM_Project\\Rn_100.txt");
            //uint sda = TempInput.getFrequency();
            //Console.WriteLine("Fs: "+ sda);
            //Vector <double> RR = TempInput.getSignal();
            //Console.WriteLine("Signal length: " + RR.Count);

            //TWave samp = new TWave(RR);
            //samp.DeleteQRSWave();
            //Vector<double> wyniki = samp.FindT_Max();
            //TempInput.writeFile(360,wyniki);
            double[] aa = new double[12];
            aa[0] = 1;
            aa[1] = 2;
          
            Vector<double> test = Vector<double>.Build.DenseOfArray(aa);
         

            foreach (double x in test)
            {
                Console.WriteLine(x);
            }
           
          


            Console.ReadKey();

        }
       
       
    }
    /// <summary>
    /// This class is created to implements algorithms to find T_end in signal
    /// </summary>
    class TWave
    {
        //private double Fs;
        String drainName;
        T_End_Method method;

        private Vector<double> QRS_Onset;
        private Vector<double> QRS_End;
        private Vector<double> P_Onset;
        private Vector<double> RR_Interval;
        public Vector<double> signal;
      


        public TWave(Vector<double> Signal, Vector<double> QRS_Onset, Vector<double> P_Onset, Vector<double> QRS_End, Vector<double> RR_Interval, String drainName, T_End_Method method)
        {
            //this.Fs = Fs;
            this.drainName = drainName;
            this.method = method;

            this.signal = Signal;                   
            this.QRS_Onset = QRS_Onset;
            this.QRS_End = QRS_End;
            this.RR_Interval = RR_Interval;
            this.P_Onset = P_Onset;
            
        }
        public TWave(Vector<double> signal)
        {
            //for testes
            this.signal = signal;
            double[] tab = { 167, 576 };
            double[] tab2 = { 203, 613 };
            double[] tab3 = { 185, 288 };
            Vector<double> temp1 = Vector<double>.Build.DenseOfArray(tab);
            Vector<double> temp2 = Vector<double>.Build.DenseOfArray(tab2);
            Vector<double> temp3 = Vector<double>.Build.DenseOfArray(tab3);
            this.QRS_Onset = temp1;
            this.QRS_End = temp2;
            this.RR_Interval = temp3;

            //this.QRS_Onset.Add(167.0);
            //this.QRS_Onset.Add(576.0);

            //this.QRS_End.Add(203.0);
            //this.QRS_End.Add(613.0);

        }

        /// <summary>
        /// This function is responsible for finding QRS-Waves in signal and delete them.
        /// </summary>
        public void DeleteQRSWave()
        {
            if (QRS_Onset.At(0) < QRS_End.At(0))
            {
                for (int i = 0; i < QRS_End.Count(); i++)
                {
                    int difference = (int)(QRS_End.At(i) - QRS_Onset.At(i));
                    Vector<double> Zeros = Vector<double>.Build.Dense(difference);
                    signal.SetSubVector((int)QRS_Onset.At(i), difference, Zeros);
                    Zeros.Clear();
                }
            }



        }
        /// <summary>
        /// This function finds T-Max points in signal
        /// </summary>
        public Vector<double> FindT_Max()
        {
            List<double> T_MaxIndexList = new List<double>();

            //double start = RR_Interval.At(0);
            for (int i = 0; i < RR_Interval.Count - 1; i++)
            {

                T_MaxIndexList.Add((double)(signal.SubVector((int)RR_Interval.ElementAt(i), (int)RR_Interval.ElementAt(i + 1)).MaximumIndex()) + RR_Interval.ElementAt(i));


            }
            Vector<double> T_MaxIndex = Vector<double>.Build.DenseOfArray(T_MaxIndexList.ToArray());
            T_MaxIndexList.Clear();
            return T_MaxIndex;
        }
        /// <summary>
        /// This function finds T-End indexes in signal
        /// </summary>
        private Vector<double> FindT_End()
        {
            Vector<double> T_MaxIndex = FindT_Max();
            List<double> T_EndIndexList = new List<double>();
            if (P_Onset.At(0) > T_MaxIndex.At(0))
            {
                for (int i = 0; i < T_MaxIndex.Count; i++)
                {

                    T_EndIndexList.Add(diff(signal.SubVector((int)P_Onset.At(i), (int)(P_Onset.At(i) - T_MaxIndex.At(i)))).MinimumIndex());

                }


            }
            return Vector<double>.Build.DenseOfArray(T_EndIndexList.ToArray());



        }
        //private Vector<double> TangentMethod()
        //{

        //}
        //private Vector<double> ParabolaMethod()
        //{

        //}

        /// <summary>
        /// This function calculate the difference between the next elements in vector
        /// </summary>
        /// <param name="In">Vector to differitive</param>
        /// <returns>Vector after</returns>
        private Vector<double> diff(Vector<double> In)
        {
            Vector<double> Output = Vector<double>.Build.Dense(In.Count - 1);
            double[] output = new double[In.Count - 1];

            for (int i = 0; i < In.Count - 1; i++)
            {
                output[i] = In.At(i + 1) - In.At(i);
            }
            Output.SetValues(output);
            return Output;
        }
        /// <summary>
        /// This function execute all methods to find a T_End indexes in a signal
        /// </summary>
        /// <returns>Vector that contains indexes of T_End</returns>
        public Vector<double> ExecuteTWave()
        {
            DeleteQRSWave();
            return FindT_End();
        }

    }


    public class QT_Data
    {

        public Vector<double> QRS_Onset;
        public Vector<double> T_End;
        public Vector<double> RR_Interval;
        public double Fs;
        public String drainName;
        public QT_Calc_Method method;
        

        public QT_Data(Vector<double> QRS_Onset, Vector<double> T_End, Vector<double> RR_Interval, double Fs, String drainName, QT_Calc_Method method)
        {
            this.QRS_Onset = QRS_Onset;
            this.T_End = T_End;
            this.RR_Interval = RR_Interval;
            this.Fs = Fs;
            this.drainName = drainName;
            this.method = method;
            
        }
        /// <summary>
        /// This function caluclate QT interval according to Bazetta's formula
        /// </summary>
        /// <returns>QT_Bazetta interval in seconds</returns>
        //public double QT_Bazetta()
        //{
        //    return ((T_End - QRS_Onset) / Fs) / Math.Sqrt(RR_Interval);
        //}
        ///// <summary>
        ///// This function calculate QT interval according to Friderica's formula
        ///// </summary>
        ///// <returns>QT_Friderica interval in seconds</returns>
        //public double QT_Friderica()
        //{
        //    return ((T_End - QRS_Onset) / Fs) / Math.Pow(RR_Interval, 1 / 3);

        //}
        ///// <summary>
        ///// This function calculate QT interval according to Framigham's formula
        ///// </summary>
        ///// <returns>QT_Framigham interval in seconds</returns>
        //public double QT_Framigham()
        //{
        //    return ((T_End - QRS_Onset) / Fs) + 0.154 * (1 - RR_Interval);
        //}
    }
    class QTCalculation
    {
        Vector<double> meanQTInterval;
        Vector<double> stdQTInterval;
        Vector<double> localQTdisp;
        List<String> drainName;
        double globalQTDisp;
        List<QT_Data> DataToOperate;            //size of List tells us the amount of drains.
        int AmountOFDrains;

        public QTCalculation(List<QT_Data> DataToOperate)
        {
            this.DataToOperate = DataToOperate;
            this.AmountOFDrains = this.DataToOperate.Count;

        }
        private List<double> parametrization(List<double> In, QT_Calc_Method method)
        {
            List<double> output = new List<double>();
            if(method == QT_Calc_Method.BAZETTA)
            {

            }
            if (method == QT_Calc_Method.FRAMIGHAMA)
            {

            }
            if (method == QT_Calc_Method.FRIDERICA)
            {

            }
            return output;

        }
        public void setOutput()
        {
            List<double> QTIntervals = new List<double>();
            double[] mean = new double[this.AmountOFDrains];
            double[] std = new double[this.AmountOFDrains];
            double[] local = new double[this.AmountOFDrains];
            int j = 0;

            foreach (QT_Data drain in DataToOperate){
                if (drain.QRS_Onset.ElementAt(0) < drain.T_End.ElementAt(0))
                {
                    for(int i = 0; i<drain.QRS_Onset.Count; i++)
                    {
                        QTIntervals.Add(drain.T_End.ElementAt(i) - drain.QRS_Onset.ElementAt(i));
                    }


                    
                        
                }

            }
        }



    }




}
