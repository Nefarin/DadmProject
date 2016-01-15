using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.HRV2
{
 
    public partial class HRV2 : IModule
    {

        private double tinn;
        #region Documentation
        /// <summary>
        /// współczynnik TINN czyli podstawa trójkąta dofitowanego do histogramu
        /// </summary>
        /// 
        #endregion
        private void makeTinn()
        {
            Vector<double>  RRIntervals = InputData.RRInterval[_outputIndex].Item2;
            double Tinn = (RRIntervals.Max() - RRIntervals.Min());
            tinn = Tinn;
        }
    }
 
}
