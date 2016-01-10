using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;

namespace EKG_Project.Modules.T_Wave_Alt
{
    public partial class T_Wave_Alt : IModule
    {
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _samplesProcessed;
        private int _numberOfChannels;

        private ECG_Baseline_Data_Worker _inputWorkerECG;
        private Waves_Data_Worker _inputWorkerWaves;
        private T_Wave_Alt_Data_Worker _outputWorker;

        private ECG_Baseline_Data _inputDataECG;
        private Waves_Data _inputDataWaves;
        private T_Wave_Alt_Data _outputData;

        private T_Wave_Alt_Params _params;

        private Vector<Double> _currentVector;
        private List<int> _currentTEnds;
        private int[] _alternansIndexArray;

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
            Params = parameters as T_Wave_Alt_Params;
            Aborted = false;
            if (!Runnable()) _ended = true;
            else
            {
                _ended = false;

                InputWorkerECG = new ECG_Baseline_Worker(Params.AnalysisName);
                InputWorkerECG.Load();
                InputDataECG = InputWorkerECG.OutputData;

                InputWorkerWaves = new Waves_Worker(Params.AnalysisName);
                InputWorkerWaves.Load();
                InputDataWaves = InputWorkerWaves.OutputData;

                OutputWorker = new T_Wave_Alt_Data_Worker(Params.AnalysisName);
                OutputData = new T_Wave_Alt_Data();

                _currentChannelIndex = 0;
                _samplesProcessed = 0;
                NumberOfChannels = InputDataECG.Signals.Count;
                _currentChannelLength = InputDataECG.Signals[_currentChannelIndex].Item2.Count;
                _currentVector = Vector<Double>.Build.Dense(_currentChannelLength);
            }
        }

        public void ProcessData()
        {
            if (Runnable()) processData();
            else _ended = true;
        }

        public double Progress()
        {
            return 100.0 * ((double)_currentChannelIndex / (double)NumberOfChannels + (1.0 / NumberOfChannels) * ((double)_samplesProcessed / (double)_currentChannelLength));
        }

        public bool Runnable()
        {
            return Params != null;
        }

        private void processData()
        {
            int channel = _currentChannelIndex;
            int startIndex = _samplesProcessed;

            if (channel < NumberOfChannels)
            {
                _currentVector = InputDataECG.Signals[_currentChannelIndex].Item2.SubVector(0, _currentChannelLength);
                _currentTEnds = InputDataWaves.TEndss[_currentChannelIndex].Item2;

                _alternansIndexArray = findAlternans(_currentTEnds, _currentVector, 360);

                OutputData.Output.Add(new Tuple<string, Vector<double>>(InputDataECG.Signals[_currentChannelIndex].Item1, _alternansIndexArray));
                _currentChannelIndex++;
                if (_currentChannelIndex < NumberOfChannels)
                    {
                        _samplesProcessed = 0;
                        _currentChannelLength = InputDataECG.Signals[_currentChannelIndex].Item2.Count;
                        _currentVector = Vector<Double>.Build.Dense(_currentChannelLength);
                    }

            }
            else
            {
                OutputWorker.Save(OutputData);
                _ended = true;
            }



        }

        public T_Wave_Alt_Data OutputData
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

        public T_Wave_Alt_Params Params
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

        public ECG_Baseline_Data InputDataECG
        {
            get
            {
                return _inputDataECG;
            }

            set
            {
                _inputDataECG = value;
            }
        }

        public ECG_Baseline_Data_Worker InputWorkerECG
        {
            get
            {
                return _inputWorkerECG;
            }

            set
            {
                _inputWorkerECG = value;
            }
        }

        public Waves_Data InputDataWaves
        {
            get
            {
                return _inputDataWaves;
            }

            set
            {
                _inputDataWaves = value;
            }
        }

        public Waves_Data_Worker InputWorkerWaves
        {
            get
            {
                return _inputWorkerWaves;
            }

            set
            {
                _inputWorkerWaves = value;
            }
        }

        public T_Wave_Alt_Data_Worker OutputWorker
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
