using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Vector<double> RRIntervaals = InputData.RPeaks[_currentChannelIndex].Item2;
            double tinn = (RRIntervaals.Max() - RRIntervaals.Min());
            return tinn;
        }
    }
}
