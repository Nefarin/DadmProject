using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using EKG_Project.Modules.R_Peaks;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.IO;

namespace EKG_Project.Modules.HRV2
{
    public partial class HRV2_Alg
    {
        private Vector<double> _rrIntervals;
        private Vector<double> _rrIntervalsFixed;
       
       // tu bd interpolacja
       
        #region Documentation
        /// <summary>
        /// This function returns all parameters needed to analyze.
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
            //Analise.Anlalysis();
            Analise.HistogramToVisualisation().ForEach(Console.WriteLine);
            Console.WriteLine(fs);
            Console.ReadLine();

            //write result to dat file
            //TempInput.setOutputFilePath(@"C:\Users\Ewa\Desktop\DADM_projekt\DadmProject\result.txt");
            //TempInput.setOutputFilePath(@"E:\aaa9semestr\Dadm\DADM_project\result.txt");
            //TempInput.writeFile();

            //HRV2_Params param = new HRV2_Params(3, 9, "TestAnalysis6");
            //HRV2 testModule = new HRV2();
            //testModule.Init(param);
            //while (true)
            //{
            //    //Console.WriteLine("Press key to continue.");
            //    //Console.Read();
            //    if (testModule.Ended()) break;
            //    Console.WriteLine(testModule.Progress());
            //    testModule.ProcessData();
            //}
        }
    }
}