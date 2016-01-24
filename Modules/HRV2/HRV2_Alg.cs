using System;
using System.Collections.Generic;
using EKG_Project.Modules.R_Peaks;
using MathNet.Numerics.LinearAlgebra;



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
    }
}