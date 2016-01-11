using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using EKG_Project.Modules.R_Peaks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;

namespace EKG_Project.Modules.HRV_DFA
{
    public partial class HRV_DFA : IModule
    {
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentRpeaksLength;
        private int _rPeaksProcessed;
        private int _numberOfChannels;

        private Basic_Data_Worker _inputWorker;
        private R_Peaks_Data_Worker _inputRpeaksWorker;
        private HRV_DFA_Data_Worker _outputWorker;

        private HRV_DFA_Data _outputData;
        private Basic_Data _inputData;
        private R_Peaks_Data _inputRpeaksData;

        private HRV_DFA_Params _params;

        private Vector<double> _currentdfaNumberN;
        private Vector<double> _currentdfaValueFn;
        private Vector<double> _currentparamAlpha;

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
            Params = parameters as HRV_DFA_Params;
            Aborted = false;
            if (!Runnable()) _ended = true;
            else
            {
                _ended = false;

                InputWorker = new Basic_Data_Worker(Params.AnalysisName);
                InputWorkerRpeaks = new R_Peaks_Data_Worker(Params.AnalysisName);
                InputWorker.Load();
                InputData = InputWorker.BasicData;
                InputDataRpeaks = InputWorkerRpeaks.Data;

                OutputWorker = new HRV_DFA_Data_Worker(Params.AnalysisName);
                OutputData = new HRV_DFA_Data();

                _currentChannelIndex = 0;
                _rPeaksProcessed = 0;
                NumberOfChannels = InputData.Signals.Count;
                _currentRpeaksLength = InputDataRpeaks.RPeaks[_currentChannelIndex].Item2.Count;

                _currentdfaNumberN = Vector<double>.Build.Dense(_currentRpeaksLength);
                _currentdfaValueFn = Vector<double>.Build.Dense(_currentRpeaksLength);
                _currentparamAlpha = Vector<double>.Build.Dense(_currentRpeaksLength);
            }
        }

        public void ProcessData()
        {
            if (Runnable()) processData();
            else _ended = true;
        }

        public double Progress()
        {
            return 100.0 * ((double)_currentChannelIndex / (double)NumberOfChannels + (1.0 / NumberOfChannels) * ((double)_rPeaksProcessed / (double)_currentRpeaksLength));
        }

        public bool Runnable()
        {
            return Params != null;
        }
        private void processData()
        {
            int channel = _currentChannelIndex;
            int startIndex = _rPeaksProcessed;
            int step = 0;

            if (channel < NumberOfChannels)
            {
                
                if (startIndex + step > _currentRpeaksLength)
                {
                    OutputData.DfaNumberN.Add(new Tuple<string, Vector<double>>(InputData.Signals[_currentChannelIndex].Item1, _currentdfaNumberN));
                    OutputData.DfaValueFn.Add(new Tuple<string, Vector<double>>(InputData.Signals[_currentChannelIndex].Item1, _currentdfaValueFn));
                    OutputData.ParamAlpha.Add(new Tuple<string, Vector<double>>(InputData.Signals[_currentChannelIndex].Item1, _currentparamAlpha));

                    _currentChannelIndex++;

                    if (_currentChannelIndex < NumberOfChannels)
                    {
                        _rPeaksProcessed = 0;

                        _currentRpeaksLength = InputDataRpeaks.RPeaks[_currentChannelIndex].Item2.Count;
                        _currentVector = Vector<Double>.Build.Dense(_currentRpeaksLength);
                    }
                    else
                    {
                        _currentVector = InputDataRpeaks.RPeaks[_currentChannelIndex].Item2.SubVector(startIndex, step);
                        _rPeaksProcessed = startIndex + step;
                        
                    }
                }
                else
                {
                    OutputWorker.Save(OutputData);
                    _ended = true;
                }
            }

        }
//
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

        public int CurrentChannelIndex
        {
            get
            {
                return _currentChannelIndex;
            }

            set
            {
                _currentChannelIndex = value;
            }
        }

        public int CurrentRpeaksLength
        {
            get
            {
                return _currentRpeaksLength;
            }

            set
            {
                _currentRpeaksLength = value;
            }
        }

        public int RPeaksProcessed
        {
            get
            {
                return _rPeaksProcessed;
            }

            set
            {
                _rPeaksProcessed = value;
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

        public R_Peaks_Data_Worker InputWorkerRpeaks
        {
            get
            {
                return _inputRpeaksWorker;
            }

            set
            {
                _inputRpeaksWorker = value;
            }
        }

        public HRV_DFA_Data_Worker OutputWorker
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

        public HRV_DFA_Data OutputData
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

        public R_Peaks_Data InputDataRpeaks
        {
            get
            {
                return _inputRpeaksData;
            }

            set
            {
                _inputRpeaksData = value;
            }
        }

        public HRV_DFA_Params Params
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

        public static void Main()
        {
            HRV_DFA_Params param = new HRV_DFA_Params("TestAnalysis");
            HRV_DFA testModule = new HRV_DFA();

            while (true)
            {
                //Console.WriteLine("Press key to continue.");
                //Console.Read();
                if (testModule.Ended()) break;
                Console.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }
            //Console.ReadKey();
        }
    }
}
