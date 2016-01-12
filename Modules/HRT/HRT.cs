using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using EKG_Project.Modules.R_Peaks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;

namespace EKG_Project.Modules.HRT
{
    public partial class HRT : IModule
    {
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _samplesProcessed;
        private int _numberOfChannels;

        private R_Peaks_Data_Worker _inputRpeaksWorker;


        public void Abort()
        {
            throw new NotImplementedException();
        }

        public bool Ended()
        {
            throw new NotImplementedException();
        }

        public void Init(ModuleParams parameters)
        {
            throw new NotImplementedException();
        }

        public void ProcessData(int numberOfSamples)
        {
            throw new NotImplementedException();
        }

        public double Progress()
        {
            throw new NotImplementedException();
        }

        public bool Runnable()
        {
            throw new NotImplementedException();
        }
    }
}
