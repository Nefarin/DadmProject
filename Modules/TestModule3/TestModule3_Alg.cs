using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using EKG_Project.IO;

namespace EKG_Project.Modules.TestModule3
{
    public partial class TestModule3 : IModule
    {
        private void scaleSamples(int channel, int startIndex, int step)
        {
            Vector<Double> temp = InputData.Signals[channel].Item2.SubVector(startIndex, step);
            temp = temp.Multiply(Params.Scale);
        }
    }
}
