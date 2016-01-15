using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.HRV2
{
 
    public partial class HRV2 : IModule
    {
        #region Documentation
        /// <summary>
        /// współczynnik TINN czyli podstawa trójkąta dofitowanego do histogramu
        /// </summary>
        /// 
        #endregion
        double makeTinn()
        {
            Vector<double>  RRIntervals = InputData.RRInterval[_outputIndex].Item2;
            double tinn = (RRIntervals.Max() - RRIntervals.Min());
            return tinn;
        }
    }
 
}
