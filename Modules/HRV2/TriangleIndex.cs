using System;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.HRV2
{
  
    public partial class HRV2 : IModule
    {

        double triangleIndex;
        public void TriangleIndex()
        {
            Vector <double> RRIntervaals = InputData.RRInterval[_outputIndex].Item2;
            triangleIndex = _currentHistogram.MaxCount/_currentRPeaksLength;
        }
    }

}

