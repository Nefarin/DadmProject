using System;
using System.Collections.Generic;
using EKG_Project.Modules.R_Peaks;
using MathNet.Numerics.LinearAlgebra;


namespace EKG_Project.Modules.HRV2
{
    public partial class HRV2 : IModule
        {
            public HRV2_Data Analyse(Vector<double> inputData)
            {
                HRV2_Data OUT_Data = new HRV2_Data();
                inputData = InputData.RPeaks[_currentChannelIndex].Item2;
            
                //OUT_Data.HistogramData = makeHistogram(inputData);
                //OUT_Data.HistogramData.Add._currentHistogram;
                OUT_Data.Tinn = makeTinn(inputData);
                OUT_Data.TriangleIndex = TriangleIndex(inputData);
                
            return OUT_Data;
            }

        }
}