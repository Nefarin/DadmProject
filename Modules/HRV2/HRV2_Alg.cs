using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using EKG_Project.Modules.R_Peaks;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.IO;
using System.Linq;

namespace EKG_Project.Modules.HRV2
{
    public partial class HRV2_Alg
    {


        private Vector<double> _rrIntervals;
        #region Documentation
        /// <summary>
        /// Sets and gets RR intervals
        /// </summary>
        /// 
        #endregion
        public Vector<double> RRIntervals
        {
            set
            {
                _rrIntervals = value;
            }
            get
            {
                return _rrIntervals;
            }
        }

        #region Documentation
        /// <summary>
        /// This function returns RR intervals vector after interpolation
        /// </summary>
        /// 
        #endregion
        public void Interpolation()
        {
            for (int i = 1; i <= _rrIntervals.Count; i++)
            {
                if (_rrIntervals[i] > 2*_rrIntervals.Average())
                {
                    _rrIntervals[i] = _rrIntervals[i - 1];
                    i++;
                }
                else
                {
                    i++;
                }
            }
        }
       
        #region Documentation
        /// <summary>
        /// This function analise and write to all parameters needed to analyze.
        /// To see algorithm navigate to the appropriate file: Poincare.cs, Histogram.cs, Tinn.cs, TriangleIndex.cs
        /// </summary>
        /// 
        #endregion
        public void HRV2_Anlalysis()
        {
            //Histogram.cs
            HistogramToVisualisation();
            //Poincare.cs
            PoincarePlot_x();
            PoincarePlot_y();
            SD1();
            SD2();
            eclipseCenter();
            //Tinn.cs
            makeTinn();
            //TriangleIndex.cs
            TriangleIndex();
        }
        public static void Main()
        {
            //read data from file
            TempInput.setInputFilePath(@"C:\Users\Ewa\Desktop\DADM_projekt\DadmProject\RR_100.txt");
            //TempInput.setInputFilePath(@"E:\aaa9semestr\Dadm\DADM_project\RR_100.txt");
            uint fs = TempInput.getFrequency();
            Vector<double> sig = TempInput.getSignal();
            
            HRV2_Alg Analise = new HRV2_Alg();
            Analise.RRIntervals = sig;
            Analise.Interpolation();
            Analise.HRV2_Anlalysis();
            
            //write result to dat file
            TempInput.setOutputFilePath("resultTriInx.txt");
            //TempInput.setOutputFilePath(@"E:\aaa9semestr\Dadm\DADM_project\result.txt");
            TempInput.writeFile(Analise.triangleIndex);
            Console.ReadLine();
        }
    }
}