using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.R_Peaks;
using MathNet.Numerics.Statistics;

namespace EKG_Project.Modules.HRV2
{
    public partial class HRV2 : IModule
    {
        public Histogram makeHistogram(Vector <double> RRIntervals)
        {
            RRIntervals = InputData.RPeaks[_currentChannelIndex].Item2;
            int binAmount = (int)((RRIntervals.Max() - RRIntervals.Min()) / 7.8125); //the amount of the bins
            Histogram histogram = new Histogram(RRIntervals, binAmount);
           
            return histogram;
        }
    }
}

