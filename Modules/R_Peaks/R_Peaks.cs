using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using EKG_Project.Modules.ECG_Baseline;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.R_Peaks
{
    public partial class R_Peaks : IModule
    {
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _samplesProcessed;
        private int _lastRPeak;
        private int _numberOfChannels;

        private ECG_Baseline_Data_Worker _inputWorker;
        private Basic_Data_Worker _inputWorker_basic;
        private R_Peaks_Data_Worker _outputWorker;

        private R_Peaks_Data _outputData;
        private ECG_Baseline_Data _inputData;
        private Basic_Data _inputData_basic;
        private R_Peaks_Params _params;

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
            Params = parameters as R_Peaks_Params;
            Aborted = false;
            if (!Runnable()) _ended = true;
            else
            {
                _ended = false;

                InputWorker = new ECG_Baseline_Data_Worker(Params.AnalysisName);
                InputWorker.Load();
                InputData = InputWorker.Data;

                OutputWorker = new R_Peaks_Data_Worker(Params.AnalysisName);
                OutputData = new R_Peaks_Data();

                _currentChannelIndex = 0;
                _samplesProcessed = 0;
                _lastRPeak = 0;
                NumberOfChannels = InputData.SignalsFiltered.Count;
                _currentChannelLength = InputData.SignalsFiltered[_currentChannelIndex].Item2.Count;
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
            int startIndex = (_samplesProcessed == 0) ? _samplesProcessed : _lastRPeak;
            int step = 6000;

            if (channel < NumberOfChannels)
            {
                if (startIndex + step > _currentChannelLength)
                {
                    _currentVector = InputData.SignalsFiltered[_currentChannelIndex].Item2.SubVector(startIndex, _currentChannelLength - startIndex);
                    switch (Params.Method)
                    {
                        case R_Peaks_Method.PANTOMPKINS:
                            _currentVector = Hilbert(_currentVector, InputData_basic.Frequency);
                            break;
                        case R_Peaks_Method.HILBERT:
                            _currentVector = PanTompkins(_currentVector, InputData_basic.Frequency);
                            break;
                        case R_Peaks_Method.EMD:
                            //_currentVector = EMD(_currentVector, InputData.Frequency);
                            break;
                    }
                    _lastRPeak = Convert.ToInt32(_currentVector[_currentVector.Count]) + 30;
                    Vector<double> _currentVectorRRInterval = RRinMS(_currentVector, InputData_basic.Frequency);

                    OutputData.RPeaks.Add(new Tuple<string, Vector<double>>(InputData.SignalsFiltered[_currentChannelIndex].Item1, _currentVector)); 
                    OutputData.RRInterval.Add(new Tuple<string, Vector<double>>(InputData.SignalsFiltered[_currentChannelIndex].Item1, _currentVectorRRInterval));
                    _currentChannelIndex++;
                    if (_currentChannelIndex < NumberOfChannels)
                    {
                        _samplesProcessed = 0;
                        _lastRPeak = 0;
                        _currentChannelLength = InputData.SignalsFiltered[_currentChannelIndex].Item2.Count;
                        _currentVector = Vector<Double>.Build.Dense(_currentChannelLength);
                    }

                }
                else
                {
                    _currentVector = InputData.SignalsFiltered[_currentChannelIndex].Item2.SubVector(startIndex, step);
                    switch (Params.Method)
                    {
                        case R_Peaks_Method.PANTOMPKINS:
                            _currentVector = Hilbert(_currentVector, InputData_basic.Frequency);
                            break;
                        case R_Peaks_Method.HILBERT:
                            _currentVector = PanTompkins(_currentVector, InputData_basic.Frequency);
                            break;
                        case R_Peaks_Method.EMD:
                            //_currentVector = EMD(_currentVector, InputData.Frequency);
                            break;
                    }
                    _lastRPeak = Convert.ToInt32(_currentVector[_currentVector.Count]) + 30;
                    Vector<double> _currentVectorRRInterval = RRinMS(_currentVector, InputData_basic.Frequency);

                    _samplesProcessed = startIndex + step;
                }
            }
            else
            {
                OutputWorker.Save(OutputData);
                _ended = true;
            }


        }

        public R_Peaks_Data OutputData
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

        public R_Peaks_Params Params
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

        public ECG_Baseline_Data InputData
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

        public ECG_Baseline_Data_Worker InputWorker
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

        public Basic_Data InputData_basic
        {
            get
            {
                return _inputData_basic;
            }

            set
            {
                _inputData_basic = value;
            }
        }

        public Basic_Data_Worker InputWorker_basic
        {
            get
            {
                return _inputWorker_basic;
            }

            set
            {
                _inputWorker_basic = value;
            }
        }

        public R_Peaks_Data_Worker OutputWorker
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

        /*public static void Main()
        {
            R_Peaks_Params param = new R_Peaks_Params(R_Peaks_Method.PANTOMPKINS, "Analysis6");
            //R_Peaks_Params param = null;
            R_Peaks testModule = new R_Peaks();
            testModule.Init(param);
            while (true)
            {
                //Console.WriteLine("Press key to continue.");
                //Console.Read();
                if (testModule.Ended()) break;
                Console.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }
        }*/
    }
}
