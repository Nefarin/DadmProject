using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;


namespace EKG_Project.Modules.HRV2
{

    public partial class HRV2_Alg
    {
        private double tinn;
        #region Documentation
        /// <summary>
        /// Get the TINN coefficient, which is 
        /// the base of the triangle fitted to histogram
        /// </summary>
        /// 
        #endregion
            private void makeTinn()
        {
            //Vector<double> RRIntervals = InputData.RRInterval[_currentChannelIndex].Item2;
            double Tinn = (_rrIntervals.Max() - _rrIntervals.Min());
            tinn = Tinn;
        }
    }
}