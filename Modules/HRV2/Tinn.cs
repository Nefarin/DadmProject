using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;


namespace EKG_Project.Modules.HRV2
{
    public partial class HRV2_Alg
    {
        private double tinn;
        public double Tinn
        {
            set
            {
                tinn = value;
            }
            get
            {
                return tinn;
            }
        }
        
        #region Documentation
        /// <summary>
        /// Write to the <double> tinn coefficient, which is 
        /// the base of the triangle fitted to histogram
        /// </summary>
        /// 
        #endregion
            private void makeTinn()
        {
            try
            {
                tinn = (_rrIntervals.Max() - _rrIntervals.Min());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}