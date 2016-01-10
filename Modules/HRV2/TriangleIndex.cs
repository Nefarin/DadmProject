using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.HRV2
{
    public partial class HRV2 : IModule
    {
      
        public double TriangleIndex()
        {
            Vector<double> RRIntervaals = InputData.RPeaks[_currentChannelIndex].Item2;
            double triangleIndex = 0;
            return triangleIndex;
        }
    }
}

