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
            Console.WriteLine("Start index: " + startIndex);
            Console.WriteLine("Start step: " + step);
            Console.WriteLine("Current length: " + _currentChannelLength);
            if (step > 0)
            {
                Vector<Double> temp = InputData.Signals[channel].Item2.SubVector(startIndex, step);
                temp = temp.Multiply(Params.Scale);
                _currentVector.SetSubVector(startIndex, step, temp);
            }

        }
    }
}
