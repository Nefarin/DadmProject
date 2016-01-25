using System;
using System.Collections.Generic;
using EKG_Project.Modules.R_Peaks;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.IO;

namespace EKG_Project.Modules.HRV2
{
    public partial class HRV2_Alg
    {
        #region Documentation
        /// <summary>
        /// This function returns all parameters needed to analyze.
        /// To see algorithm navigate to the appropriate file: Poincare.cs, Histogram.cs, Tinn.cs, TriangleIndex.cs
        /// </summary>
        /// 
        #endregion
        public void Anlalysis(Vector<double> rrIntervals)
        {
            //Histogram.cs
            HistogramToVisualisation(rrIntervals);
            makeHistogram(rrIntervals);
            //Poincare.cs
            PoincarePlot_x(rrIntervals);
            PoincarePlot_y(rrIntervals);
            SD1();
            SD2();
            eclipseCenter();
            //Tinn.cs
            makeTinn(rrIntervals);
            //TriangleIndex.cs
            TriangleIndex(rrIntervals);
        }
        public static void Main()
        {
            //read data from file
            TempInput.setInputFilePath(@"C:\Users\Ewa\Desktop\DADM_projekt\DadmProject\RR_100.txt");
            uint fs = TempInput.getFrequency();
            Vector<double> sig = TempInput.getSignal();

            HRV2_Alg Analise = new HRV2_Alg();
            Analise.Anlalysis(sig);

            //write result to dat file
            TempInput.setOutputFilePath(@"C:\Users\Ewa\Desktop\DADM_projekt\DadmProject\result.txt");
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