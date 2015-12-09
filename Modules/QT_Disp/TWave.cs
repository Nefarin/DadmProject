using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.QT_Disp
{
    /// <summary>
    /// This class is created to implements algorithms to find T_end in signal
    /// </summary>
    class TWave
    {
        private double Fs;
        private Vector<double> QRS_Onset;
        private Vector<double> QRS_End; 
        private Vector<double> P_Onset;
        private Vector<double> RR_Interval;
        private Matrix<double> signal;
            
        TWave(double Fs, Matrix<double> Signal, Vector<double> QRS_Onset, Vector<double> P_Onset, Vector<double> QRS_End, Vector<double> RR_Interval) 
        {
            this.Fs = Fs;
            this.signal = Signal;                   //at Row(index 1) we get amplitude
            this.QRS_Onset = QRS_Onset;
            this.QRS_End = QRS_End;         
            this.RR_Interval = RR_Interval;
            this.P_Onset = P_Onset;
            this.Fs = Fs;
        }
        
        /// <summary>
        /// This function is responsible for finding QRS-Waves in signal and delete them.
        /// </summary>
        private void DeleteQRSWave()
        {
            if (QRS_Onset.At(0) < QRS_End.At(0))
            {
                for (int i = 0; i < QRS_End.Count(); i++)
                {
                    int difference =(int)( QRS_End.At(i) - QRS_Onset.At(i));
                    Vector<double> Zeros = Vector<double>.Build.Dense(difference);                    
                    signal.Row(1).SetSubVector((int)QRS_Onset.At(i), difference, Zeros);
                    Zeros.Clear();
                }
            }
            
          
           
        }
        /// <summary>
        /// This function finds T-Max points in signal
        /// </summary>
        private Vector<double> FindT_Max()
        {
            List <double> T_MaxIndexList = new List<double>() ;
            
            double start = QRS_Onset.At(0);
            foreach(double RR in RR_Interval)
            {

                T_MaxIndexList.Add((double)(signal.Row(1).SubVector((int)start, (int)RR).MaximumIndex()));
                start += RR;

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
                for(int i = 0; i < T_MaxIndex.Count; i++)
                {
                   
                    T_EndIndexList.Add(diff(signal.Row(1).SubVector((int)P_Onset.At(i), (int)(P_Onset.At(i) - T_MaxIndex.At(i)))).MinimumIndex());

                }
                

            }
            return Vector<double>.Build.DenseOfArray(T_EndIndexList.ToArray());            


          
        }
        /// <summary>
        /// This function calculate the difference between the next elements in vector
        /// </summary>
        /// <param name="In">Vector to differitive</param>
        /// <returns>Vector after</returns>
        private Vector<double> diff(Vector<double> In)
        {
            Vector<double> Output = Vector<double>.Build.Dense(In.Count - 1);
            double[] output = new double[In.Count - 1];
            
            for(int i = 0; i < In.Count - 1; i++)
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
}
