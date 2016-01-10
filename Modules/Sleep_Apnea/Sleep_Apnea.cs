using System;
using EKG_Project.IO;
using EKG_Project.Modules.R_Peaks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;

namespace EKG_Project.Modules.Sleep_Apnea
{
    public partial class Sleep_Apnea : IModule
    {
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentRpeaksLength;
        private int _samplesProcessed;
        private int _numberOfChannels;

        private R_Peaks_Data_Worker _inputWorker;
        private Sleep_Apnea_Data_Worker _outputWorker;

        private R_Peaks_Data _inputData;
        private Sleep_Apnea_Data _outputData;

        private Sleep_Apnea _params;

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
            Params = parameters as Sleep_Apnea_Params;
            Aborted = false;
            if (!Runnable()) _ended = true;
            else
            {
                _ended = false;

                InputWorker = new R_Peaks_Data_Worker(Params.AnalysisName);
                InputWorker.Load();
                InputData = InputWorker.RpeaksData;

                OutputWorker = new Sleep_Apnea_Data_Worker(Params.AnalysisName);
                OutputData = new Sleep_Apnea_Data(InputData.Frequency, InputData.SampleAmount);

                _currentChannelIndex = 0;
                _samplesProcessed = 0;
                NumberOfChannels = InputData.Signals.Count;
                _currentChannelLength = InputData.Signals[_currentChannelIndex].Item2.Count;
                _currentVector = Vector<Double>.Build.Dense(_currentChannelLength);
            }
        }

        public void ProcessData(int numberOfSamples)
        {
            if (Runnable()) processData();
            else _ended = true;
        }

        public double Progress()
        {
            return 100.0 * ((double)_currentChannelIndex / (double)NumberOfChannels + (1.0 / NumberOfChannels) * ((double)_samplesProcessed);
        }

        public bool Runnable()
        {
            return Params != null;
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
                    scaleSamples(channel, startIndex, _currentChannelLength - startIndex);
                    _outputData.Output.Add(new Tuple<string, Vector<double>>(_inputData.Signals[_currentChannelIndex].Item1, _currentVector));
                    _currentChannelIndex++;
                    if (_currentChannelIndex < NumberOfChannels))
                        {
                        _samplesProcessed = 0;
                        _currentChannelLength = InputData.Signals[_currentChannelIndex].Item2.Count;
                        _currentVector = Vector<Double>.Build.Dense(_currentRpeaksLength);
                    }
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

        public Sleep_Apnea_Data OutputData
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

        public Sleep_Apnea_Params Params
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

        public Sleep_Apnea_Data_Worker OutputWorker
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

        public static void Main()
        {
            Sleep_Apnea_Params param = new Sleep_Apnea_Params(-2, 5000, "Analysis6");
            Sleep_Apnea_Params param = null;
            Sleep_Apnea testModule = new Sleep_Apnea();
            testModule.Init(param);
            while (true)
            {
                Console.WriteLine("Press key to continue.");
                Console.Read();
                if (testModule.Ended()) break;
                Console.WriteLine(testModule.Progress());
                testModule.ProcessData()
            }
        }
    }
}
