using System;
using MathNet.Numerics.LinearAlgebra;


namespace EKG_Project.Modules.HRV2
{

    public partial class HRV2_Alg
    {
        private double triangleIndex;
        public double TriangleIndex
        {
            set
            {
                triangleIndex = value;
            }
            get
            {
                return triangleIndex;
            }
        }
        #region Documentation
        /// <summary>
        /// Write to Triangle Index coefficient, which is 
        /// the value of the highest bin, devided by all RR intervals count
        /// </summary>
        /// 
        #endregion
        private void makeTriangleIndex()
        {
            //Vector<double> RRIntervaals = InputData.RRInterval[_outputIndex].Item2;
            //try
            //{
            triangleIndex = (double)_rrIntervals.Count / (double)_currentHistogram.MaxCount;
            //}
            //catch (NullReferenceException e)
            //{
            //    Console.WriteLine(e.Message);
            //}
        }
    }

}

