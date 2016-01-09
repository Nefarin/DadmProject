using System;
using EKG_Project.IO;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;

namespace EKG_Project.Modules.ECG_Baseline
{
    public partial class ECG_Baseline : IModule
    {

        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _samplesProcessed;
        private int _numberOfChannels;

        private Basic_Data_Worker _inputWorker;
        private ECG_Baseline_Data_Worker _outputWorker;

        private ECG_Baseline_Data _outputData;
        private Basic_Data _inputData;
        private ECG_Baseline_Params _params;

        private Vector<Double> _currentVector;
        private Vector<Double> _temporaryVector;
        private Vector<Double> _temporaryVector2;

        Filter _newFilter = new Filter();

        /*
        public ECG_Baseline()
        {

        }
        */

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
            Params = parameters as ECG_Baseline_Params;
            Aborted = false;
            if (!Runnable()) _ended = true;
            else
            {
                _ended = false;

                InputWorker = new Basic_Data_Worker(Params.AnalysisName);
                InputWorker.Load();
                InputData = InputWorker.BasicData;

                OutputWorker = new ECG_Baseline_Data_Worker(Params.AnalysisName);
                OutputData = new ECG_Baseline_Data();

                _currentChannelIndex = 0;
                _samplesProcessed = 0;
                NumberOfChannels = InputData.Signals.Count;
                _currentChannelLength = InputData.Signals[_currentChannelIndex].Item2.Count;
                //_currentVector = InputData.Signals[_currentChannelIndex].Item2.CopySubVectorTo(_currentVector,0,0, _currentChannelLength); //Vector<Double>.Build.Dense(_currentChannelLength);
                //InputData.Signals[_currentChannelIndex].Item2.CopySubVectorTo(_currentVector, 0, 0, _currentChannelLength);
                _currentVector = Vector<Double>.Build.Dense(_currentChannelLength);
                _temporaryVector2 = Vector<Double>.Build.Dense(_currentChannelLength);
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
            int step = 1000;

            if (channel < NumberOfChannels)
            {
                if (startIndex + step >= _currentChannelLength)
                {
                      
                    
                    _currentVector = InputData.Signals[_currentChannelIndex].Item2.SubVector(startIndex, _currentChannelLength - startIndex);
                    
                    switch (Params.Method)
                    {
                        case Filtr_Method.MOVING_AVG:
                            _currentVector = _newFilter.moving_average(_currentVector, Params.WindowSize, Params.Type);
                            break;
                        case Filtr_Method.BUTTERWORTH:
                            _currentVector = _newFilter.butterworth(_currentVector, InputData.Frequency, Params.Fc, Params.Order, Params.Type);
                            break;
                        case Filtr_Method.SAV_GOL:
                            _currentVector = _newFilter.savitzky_golay(_currentVector, Params.WindowSize, Params.Type);
                            break;
                        case Filtr_Method.LMS:
                            _temporaryVector = _currentVector;
                            _temporaryVector = _newFilter.savitzky_golay(_currentVector, Params.WindowSize, Params.Type);
                            _currentVector = _newFilter.lms(_currentVector, _temporaryVector, Params.WindowSize);
                            break;

                    }

                    _currentVector.CopySubVectorTo(_temporaryVector2, 0, startIndex, _currentVector.Count);
                    //OutputData.SignalsFiltered.Add(new Tuple<string, Vector<double>>(InputData.Signals[_currentChannelIndex].Item1, _currentVector));
                    OutputData.SignalsFiltered.Add(new Tuple<string, Vector<double>>(InputData.Signals[_currentChannelIndex].Item1, _temporaryVector2));
                    _currentChannelIndex++;

                    if (_currentChannelIndex < NumberOfChannels)
                    {
                        _samplesProcessed = 0;
                        _currentChannelLength = InputData.Signals[_currentChannelIndex].Item2.Count;
                        _currentVector = Vector<Double>.Build.Dense(_currentChannelLength);
                        _temporaryVector2 = Vector<Double>.Build.Dense(_currentChannelLength);
                    }


                }
                else
                {

                    _currentVector = InputData.Signals[_currentChannelIndex].Item2.SubVector(startIndex, step);
                    
                    switch (Params.Method)
                    {
                        case Filtr_Method.MOVING_AVG:
                            _currentVector = _newFilter.moving_average(_currentVector, Params.WindowSize, Params.Type);
                            break;
                        case Filtr_Method.BUTTERWORTH:
                            _currentVector = _newFilter.butterworth(_currentVector, InputData.Frequency, Params.Fc, Params.Order, Params.Type);
                            break;
                        case Filtr_Method.SAV_GOL:
                            _currentVector = _newFilter.savitzky_golay(_currentVector, Params.WindowSize, Params.Type);
                            break;
                        case Filtr_Method.LMS:
                            _temporaryVector = _currentVector;
                            _temporaryVector = _newFilter.savitzky_golay(_currentVector, Params.WindowSize, Params.Type);
                            _currentVector = _newFilter.lms(_currentVector, _temporaryVector, Params.WindowSize);
                            break;

                    }
                    _currentVector.CopySubVectorTo(_temporaryVector2, 0, startIndex, _currentVector.Count);
                    //OutputData.SignalsFiltered.Add(new Tuple<string, Vector<double>>(InputData.Signals[_currentChannelIndex].Item1, _currentVector));
                    _samplesProcessed = startIndex + step;
                }
            }
            else
            { 
                OutputWorker.Save(OutputData);
                _ended = true;
            }

        }

        public ECG_Baseline_Data OutputData
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

        public ECG_Baseline_Params Params
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

        public ECG_Baseline_Data_Worker OutputWorker
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
            ECG_Baseline_Params param = new ECG_Baseline_Params(Filtr_Method.SAV_GOL, Filtr_Type.LOWPASS, 9, "Analysis6"); //Filtr_Method.MOVING_AVG, Filtr_Type.LOWPASS, 5, "Analysis6");
            //ECG_Baseline_Params param = new ECG_Baseline_Params(Filtr_Method.MOVING_AVG, Filtr_Type.LOWPASS, 5, "Analysis6");
            ECG_Baseline testModule = new ECG_Baseline();
            testModule.Init(param);
            while (true)
            {
                //Console.WriteLine("Press key to continue.");
                //Console.Read();
                if (testModule.Ended()) break;
                Console.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }
        }
    }
}
