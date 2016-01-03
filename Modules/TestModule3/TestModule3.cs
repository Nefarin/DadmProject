using System;
using EKG_Project.IO;


namespace EKG_Project.Modules.TestModule3
{
    public partial class TestModule3 : IModule
    {
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _samplesProcessed;
        private int _numberOfChannels;

        private Basic_Data_Worker _inputWorker;
        private TestModule3_Data_Worker _outputWorker;

        private TestModule3_Data _outputData;
        private Basic_Data _inputData;
        private TestModule3_Params _params;

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
            Params = parameters as TestModule3_Params;
            Aborted = false;
            if (Runnable()) _ended = true;
            else
            {
                _ended = false;

                InputWorker = new Basic_Data_Worker(Params.AnalysisName);
                InputWorker.Load();
                InputData = InputWorker.BasicData;

                OutputWorker = new TestModule3_Data_Worker(Params.AnalysisName);
                OutputData = new TestModule3_Data(InputData.Frequency, InputData.SampleAmount);
                
                _currentChannelIndex = 0;
                _samplesProcessed = 0;
                NumberOfChannels = InputData.Signals.Count;
                _currentChannelLength = InputData.Signals[_currentChannelIndex].Item2.Count;
            }

        }

        public void ProcessData()
        {
            if (Runnable()) processData();
            else _ended = true;
        }

        public double Progress()
        {
            return ((double) _currentChannelIndex * ((_samplesProcessed + 1.0)/(_currentChannelLength + 1.0))) / ((double)NumberOfChannels + 1.0);
        }

        public bool Runnable()
        {
            return Params == null;
        }

        private void processData()
        {
            int channel = _currentChannelIndex;
            int startIndex = _samplesProcessed;
            int step = Params.Step;

            if (channel < NumberOfChannels)
            {
                if (startIndex + step > _currentChannelLength)
                {
                    scaleSamples(channel, startIndex, _currentChannelLength - startIndex - 1);
                    _currentChannelIndex++;
                    _samplesProcessed = 0;
                    _currentChannelLength = InputData.Signals[_currentChannelIndex].Item2.Count;
                }
                else
                {
                    scaleSamples(channel, startIndex, step);
                    _samplesProcessed = startIndex + step;
                }
            }
            else
            {
                OutputWorker.Save(OutputData);
                _ended = true;
            }



        }

        public TestModule3_Data OutputData
        {
            get
            {
                return _outputData;
            }

            set
            {
                _outputData = value;
            }
        }

        public TestModule3_Params Params
        {
            get
            {
                return _params;
            }

            set
            {
                _params = value;
            }
        }

        public int NumberOfChannels
        {
            get
            {
                return _numberOfChannels;
            }

            set
            {
                _numberOfChannels = value;
            }
        }

        public bool Aborted
        {
            get
            {
                return _aborted;
            }

            set
            {
                _aborted = value;
            }
        }

        public Basic_Data InputData
        {
            get
            {
                return _inputData;
            }

            set
            {
                _inputData = value;
            }
        }

        public Basic_Data_Worker InputWorker
        {
            get
            {
                return _inputWorker;
            }

            set
            {
                _inputWorker = value;
            }
        }

        public TestModule3_Data_Worker OutputWorker
        {
            get
            {
                return _outputWorker;
            }

            set
            {
                _outputWorker = value;
            }
        }
    }
}
