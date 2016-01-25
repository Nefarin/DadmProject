using System;
using MathNet.Numerics.LinearAlgebra;


namespace EKG_Project.Modules.HRV2
{

    public partial class HRV2_Alg
    {
        double triangleIndex;
        #region Documentation
        /// <summary>
        /// Get Triangle Index coefficient, which is 
        /// the value of the highest bin, devided by all RR intervals count
        /// </summary>
        /// 
        #endregion
        public void TriangleIndex()
        {
            //Vector<double> RRIntervaals = InputData.RRInterval[_outputIndex].Item2;
            triangleIndex = _currentHistogram.MaxCount / _rrIntervals.Count;
        }
    }

}

