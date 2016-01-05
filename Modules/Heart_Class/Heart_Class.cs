using System;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using System.Collections.Generic;

namespace EKG_Project.Modules.Heart_Class
{
    public partial class Heart_Class : IModule
    {
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _samplesProcessed;
        private int _numberOfChannels;



        private Basic_Data_Worker _inputWorker;
        //private Heart_Class_Data_Worker _outputWorker;

        private Heart_Class_Data _outputData;
        private Basic_Data _inputData;
        private Heart_Class_Params _params;

        private Vector<Double> _currentVector;

        public void Abort()
        {
            Aborted = true;
            _ended = true;
        }

        public bool Ended()
        {
            return _ended;
        }

        public void Init(ModuleParams parameters)
        {
            /*
            Params = parameters as Heart_Class_Params;
            

            Aborted = false;
            if (!Runnable()) _ended = true;
            else
            {
                _ended = false;

                InputWorker = new Basic_Data_Worker(Params.AnalysisName);
                InputWorker.Load();
                InputData = InputWorker.BasicData;

                OutputWorker = new Heart_Class_Data_Worker(Params.AnalysisName);
                OutputData = new Heart_Class_Data(InputData.Frequency, InputData.SampleAmount);

                _currentChannelIndex = 0;
                _samplesProcessed = 0;
                NumberOfChannels = InputData.Signals.Count;
                _currentChannelLength = InputData.Signals[_currentChannelIndex].Item2.Count;
                _currentVector = Vector<Double>.Build.Dense(_currentChannelLength);

            }
            */
        }

        public void ProcessData(int numberOfSamples)
        {
            if (Runnable()) processData();
            else _ended = true;
        }
        
        public double Progress()
        {
            throw new NotImplementedException();
            //return 100.0 * ((double)_currentChannelIndex / (double)NumberOfChannels + (1.0 / NumberOfChannels) * ((double)_samplesProcessed / (double)_currentChannelLength));
        }
        
        public bool Runnable()
        {
            return Params != null;
        }

        private void processData()
        {
            //////
        }

        public Basic_Data_Worker InputWorker
        {
            get { return _inputWorker; }
            set { _inputWorker = value; }
        }
        /*
        public Heart_Class_Data_Worker OutputWorker
        {
            get { return _outputWorker; }
            set { _outputWorker = value; }
        }
        */
        public bool Aborted
        {
            get { return _aborted; }
            set { _aborted = value; }
        }

        public Heart_Class_Params Params
        {
            get { return _params; }
            set { _params = value; }
        }
        /*
        public Heart_Class_Data OutputData
        {
            get { return _outputData; }
            set { _outputData = value; }
        }
        */
        public Basic_Data InputData
        {
            get { return _inputData; }
            set { _inputData = value; }
        }

    }
}
