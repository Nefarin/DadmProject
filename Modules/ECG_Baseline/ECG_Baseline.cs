using System;
using EKG_Project.IO;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.ECG_Baseline;
using System.Collections.Generic;

namespace EKG_Project.Modules.ECG_Baseline
{
    public class ECG_Baseline : IModule
    {
        private enum STATE { INIT, BEGIN_CHANNEL, PROCESS_FIRST_STEP, PROCESS_CHANNEL, NEXT_CHANNEL, END_CHANNEL, END };
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private string _currentLeadName;
        private string[] _leads;
        private int _currentIndex;
        private int _numberOfChannels;

        //private int _samplesProcessed;

        private Basic_New_Data_Worker _inputWorker;
        private ECG_Baseline_New_Data_Worker _outputWorker;
        
        //private Basic_Data_Worker _inputWorker;
        //private ECG_Baseline_Data_Worker _outputWorker;

        private ECG_Baseline_Data _outputData;
        private Basic_New_Data _inputData;
        private ECG_Baseline_Params _params;

        ECG_Baseline_Alg.Filter _newFilter; //= new ECG_Baseline_Alg.Filter();  
        private Vector<Double> _currentVector;
        private STATE _state;
        private int _step;

        //Filter _newFilter = new Filter();
        //private Vector<Double> _temporaryVector;
        // private Vector<Double> _temporaryVector2;

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

        public bool IsAborted()
        {
            return Aborted;
        }

        public bool Ended()
        {
            return _ended;
        }

        public void Init(ModuleParams parameters)
        {
            try
            {
                _params = parameters as ECG_Baseline_Params;
            }
            catch (Exception e)
            {
                Abort();
                return;
            }

            if (!Runnable())
            {
                _ended = true;
            }
            else
            {
                InputWorker = new Basic_New_Data_Worker(Params.AnalysisName);
                OutputWorker = new ECG_Baseline_New_Data_Worker(Params.AnalysisName);
                InputData = new Basic_New_Data();
                OutputData = new ECG_Baseline_Data();

                _step = 6000; 
                _state = STATE.INIT;
            }

            //Params = parameters as ECG_Baseline_Params;
            //Aborted = false;
            //if (!Runnable()) _ended = true;
            //else
            //{
            //    _ended = false;

            //    InputWorker = new Basic_Data_Worker(Params.AnalysisName);
            //    InputWorker.Load();
            //    InputData = InputWorker.BasicData;

            //    OutputWorker = new ECG_Baseline_Data_Worker(Params.AnalysisName);
            //    OutputData = new ECG_Baseline_Data();

            //    _currentChannelIndex = 0;
            //    _samplesProcessed = 0;
            //    NumberOfChannels = InputData.Signals.Count;
            //    _currentChannelLength = InputData.Signals[_currentChannelIndex].Item2.Count;
            //    //_currentVector = InputData.Signals[_currentChannelIndex].Item2.CopySubVectorTo(_currentVector,0,0, _currentChannelLength); //Vector<Double>.Build.Dense(_currentChannelLength);
            //    //InputData.Signals[_currentChannelIndex].Item2.CopySubVectorTo(_currentVector, 0, 0, _currentChannelLength);
            //    _currentVector = Vector<Double>.Build.Dense(_currentChannelLength);
            //    _temporaryVector2 = Vector<Double>.Build.Dense(_currentChannelLength);
            //}
        }

        public void ProcessData()
        {
            if (Runnable()) processData();
            else _ended = true;
        }

        public double Progress()
        {
            return 100.0 * ((double)_currentChannelIndex / (double)NumberOfChannels + (1.0 / NumberOfChannels) * ((double)_currentIndex / (double)_currentChannelLength));
        }

        public bool Runnable()
        {
            return Params != null;
        }

        private void processData()
        {
            switch (_state)
            {
                case (STATE.INIT):
                    _currentChannelIndex = -1;
                    _leads = InputWorker.LoadLeads().ToArray();
                    _numberOfChannels = _leads.Length;
                    _newFilter = new ECG_Baseline_Alg.Filter();
                    _state = STATE.BEGIN_CHANNEL;
                    break;
                case (STATE.BEGIN_CHANNEL):
                    System.Console.WriteLine("haloasdoadsoadspdaspadso");
                    _currentChannelIndex++;
                    System.Console.WriteLine("======================");
                    System.Console.WriteLine(_currentChannelIndex);
                    System.Console.WriteLine("======================");
                    System.Console.WriteLine(_numberOfChannels);
                    if (_currentChannelIndex >= _numberOfChannels) _state = STATE.END;
                    else
                    {
                        _currentLeadName = _leads[_currentChannelIndex];
                        _currentChannelLength = (int)InputWorker.getNumberOfSamples(_currentLeadName);
                        _currentIndex = 0;
                        _state = STATE.PROCESS_FIRST_STEP;
                    }
                    break;
                case (STATE.PROCESS_FIRST_STEP):
                    if (_currentIndex + _step > _currentChannelLength) _state = STATE.END_CHANNEL;
                    else
                    {
                        try
                        {
                            _currentVector = InputWorker.LoadSignal(_currentLeadName, _currentIndex, _step);
                            System.Console.WriteLine(_currentVector.ToString());
                            //System.Console.WriteLine(Params.Method);
                            //Selecting filtration method
                            switch (Params.Method)
                            {
                                case Filtr_Method.MOVING_AVG:
                                    if (Params.Type == Filtr_Type.LOWPASS)
                                    {
                                        _currentVector = _newFilter.moving_average(_currentVector, Params.WindowSizeLow, Filtr_Type.LOWPASS);
                                    }
                                    if (Params.Type == Filtr_Type.HIGHPASS)
                                    {
                                        _currentVector = _newFilter.moving_average(_currentVector, Params.WindowSizeHigh, Filtr_Type.HIGHPASS);
                                    }
                                    if (Params.Type == Filtr_Type.BANDPASS)
                                    {
                                        _currentVector = _newFilter.moving_average(_currentVector, Params.WindowSizeLow, Params.WindowSizeHigh, Filtr_Type.BANDPASS);
                                    }
                                    break;
                                case Filtr_Method.BUTTERWORTH:
                                    if (Params.Type == Filtr_Type.LOWPASS)
                                    {
                                        _currentVector = _newFilter.butterworth(_currentVector, InputData.Frequency, Params.FcLow, Params.OrderLow, Filtr_Type.LOWPASS);
                                    }
                                    if (Params.Type == Filtr_Type.HIGHPASS)
                                    {
                                        _currentVector = _newFilter.butterworth(_currentVector, InputData.Frequency, Params.FcHigh, Params.OrderHigh, Filtr_Type.HIGHPASS);
                                    }
                                    if (Params.Type == Filtr_Type.BANDPASS)
                                    {
                                        _currentVector = _newFilter.butterworth(_currentVector, InputData.Frequency, Params.FcLow, Params.OrderLow, Params.FcHigh, Params.OrderHigh, Filtr_Type.BANDPASS);
                                    }
                                    break;
                                case Filtr_Method.SAV_GOL:
                                    if (Params.Type == Filtr_Type.LOWPASS)
                                    {
                                        System.Console.WriteLine(Params.Method);
                                        System.Console.WriteLine("============Window Size Low=============");
                                        System.Console.WriteLine(Params.WindowSizeLow);
                                        System.Console.WriteLine("============Window Size Low=============");
                                        Vector<Double> _currentVector2;
                                        _currentVector2 = _newFilter.savitzky_golay(_currentVector, Params.WindowSizeLow, Filtr_Type.LOWPASS);
                                        System.Console.WriteLine("==============Vurenrenr===========");
                                        System.Console.WriteLine(_currentVector2.ToString());
                                        System.Console.WriteLine("==============Vurenrenr===========");
                                    }
                                    if (Params.Type == Filtr_Type.HIGHPASS)
                                    {
                                        _currentVector = _newFilter.savitzky_golay(_currentVector, Params.WindowSizeHigh, Filtr_Type.HIGHPASS);
                                    }
                                    if (Params.Type == Filtr_Type.BANDPASS)
                                    {
                                        _currentVector = _newFilter.savitzky_golay(_currentVector, Params.WindowSizeLow, Params.WindowSizeHigh, Filtr_Type.BANDPASS);
                                    }
                                    break;
                                case Filtr_Method.LMS:
                                    _currentVector = _newFilter.lms(_currentVector, InputData.Frequency, Params.WindowLMS, Filtr_Type.LOWPASS, Params.Mi);
                                    break;
                            }

                            OutputWorker.SaveSignal(_currentLeadName, true, _currentVector);
                            _currentIndex += _step;
                            _state = STATE.PROCESS_CHANNEL;
                        }
                        catch (Exception e)
                        {
                            _state = STATE.NEXT_CHANNEL;
                        }
                    }

                    break;
                case (STATE.PROCESS_CHANNEL): // this state can be divided to load state, process state and save state, good decision especially for ECG_Baseline, R_Peaks, Waves and Heart_Class
                    if (_currentIndex + _step > _currentChannelLength) _state = STATE.END_CHANNEL;
                    else
                    {
                        try
                        {
                            _currentVector = InputWorker.LoadSignal(_currentLeadName, _currentIndex, _step);

                            //Selecting filtration method
                            switch (Params.Method)
                            {
                                case Filtr_Method.MOVING_AVG:
                                    if (Params.Type == Filtr_Type.LOWPASS)
                                    {
                                        _currentVector = _newFilter.moving_average(_currentVector, Params.WindowSizeLow, Filtr_Type.LOWPASS);
                                    }
                                    if (Params.Type == Filtr_Type.HIGHPASS)
                                    {
                                        _currentVector = _newFilter.moving_average(_currentVector, Params.WindowSizeHigh, Filtr_Type.HIGHPASS);
                                    }
                                    if (Params.Type == Filtr_Type.BANDPASS)
                                    {
                                        _currentVector = _newFilter.moving_average(_currentVector, Params.WindowSizeLow, Params.WindowSizeHigh, Filtr_Type.BANDPASS);
                                    }
                                    break;
                                case Filtr_Method.BUTTERWORTH:
                                    if (Params.Type == Filtr_Type.LOWPASS)
                                    {
                                        _currentVector = _newFilter.butterworth(_currentVector, InputData.Frequency, Params.FcLow, Params.OrderLow, Filtr_Type.LOWPASS);
                                    }
                                    if (Params.Type == Filtr_Type.HIGHPASS)
                                    {
                                        _currentVector = _newFilter.butterworth(_currentVector, InputData.Frequency, Params.FcHigh, Params.OrderHigh, Filtr_Type.HIGHPASS);
                                    }
                                    if (Params.Type == Filtr_Type.BANDPASS)
                                    {
                                        _currentVector = _newFilter.butterworth(_currentVector, InputData.Frequency, Params.FcLow, Params.OrderLow, Params.FcHigh, Params.OrderHigh, Filtr_Type.BANDPASS);
                                    }
                                    break;
                                case Filtr_Method.SAV_GOL:
                                    if (Params.Type == Filtr_Type.LOWPASS)
                                    {
                                        _currentVector = _newFilter.savitzky_golay(_currentVector, Params.WindowSizeLow, Filtr_Type.LOWPASS);
                                        //System.Console.WriteLine(_currentVector.ToString());
                                        System.Console.WriteLine("=========================");
                                        System.Console.WriteLine(_currentVector.ToString());
                                    }
                                    if (Params.Type == Filtr_Type.HIGHPASS)
                                    {
                                        _currentVector = _newFilter.savitzky_golay(_currentVector, Params.WindowSizeHigh, Filtr_Type.HIGHPASS);
                                    }
                                    if (Params.Type == Filtr_Type.BANDPASS)
                                    {
                                        _currentVector = _newFilter.savitzky_golay(_currentVector, Params.WindowSizeLow, Params.WindowSizeHigh, Filtr_Type.BANDPASS);
                                    }
                                    break;
                                case Filtr_Method.LMS:
                                    _currentVector = _newFilter.lms(_currentVector, InputData.Frequency, Params.WindowLMS, Filtr_Type.LOWPASS, Params.Mi);
                                    break;
                            }

                            OutputWorker.SaveSignal(_currentLeadName, true, _currentVector);
                            _currentIndex += _step;
                            _state = STATE.PROCESS_CHANNEL;
                        }
                        catch (Exception e)
                        {
                            _state = STATE.NEXT_CHANNEL;
                        }
                    }

                    break;
                case (STATE.END_CHANNEL):
                    try
                    {
                        _currentVector = InputWorker.LoadSignal(_currentLeadName, _currentIndex, _currentChannelLength - _currentIndex);

                        //Selecting filtration method
                        switch (Params.Method)
                        {
                            case Filtr_Method.MOVING_AVG:
                                if (Params.Type == Filtr_Type.LOWPASS)
                                {
                                    _currentVector = _newFilter.moving_average(_currentVector, Params.WindowSizeLow, Filtr_Type.LOWPASS);
                                }
                                if (Params.Type == Filtr_Type.HIGHPASS)
                                {
                                    _currentVector = _newFilter.moving_average(_currentVector, Params.WindowSizeHigh, Filtr_Type.HIGHPASS);
                                }
                                if (Params.Type == Filtr_Type.BANDPASS)
                                {
                                    _currentVector = _newFilter.moving_average(_currentVector, Params.WindowSizeLow, Params.WindowSizeHigh, Filtr_Type.BANDPASS);
                                }
                                break;
                            case Filtr_Method.BUTTERWORTH:
                                if (Params.Type == Filtr_Type.LOWPASS)
                                {
                                    _currentVector = _newFilter.butterworth(_currentVector, InputData.Frequency, Params.FcLow, Params.OrderLow, Filtr_Type.LOWPASS);
                                }
                                if (Params.Type == Filtr_Type.HIGHPASS)
                                {
                                    _currentVector = _newFilter.butterworth(_currentVector, InputData.Frequency, Params.FcHigh, Params.OrderHigh, Filtr_Type.HIGHPASS);
                                }
                                if (Params.Type == Filtr_Type.BANDPASS)
                                {
                                    _currentVector = _newFilter.butterworth(_currentVector, InputData.Frequency, Params.FcLow, Params.OrderLow, Params.FcHigh, Params.OrderHigh, Filtr_Type.BANDPASS);
                                }
                                break;
                            case Filtr_Method.SAV_GOL:
                                if (Params.Type == Filtr_Type.LOWPASS)
                                {
                                    _currentVector = _newFilter.savitzky_golay(_currentVector, Params.WindowSizeLow, Filtr_Type.LOWPASS);
                                    System.Console.WriteLine("=========================");
                                    System.Console.WriteLine(_currentVector.ToString());
                                }
                                if (Params.Type == Filtr_Type.HIGHPASS)
                                {
                                    _currentVector = _newFilter.savitzky_golay(_currentVector, Params.WindowSizeHigh, Filtr_Type.HIGHPASS);
                                }
                                if (Params.Type == Filtr_Type.BANDPASS)
                                {
                                    _currentVector = _newFilter.savitzky_golay(_currentVector, Params.WindowSizeLow, Params.WindowSizeHigh, Filtr_Type.BANDPASS);
                                }
                                break;
                            case Filtr_Method.LMS:
                                _currentVector = _newFilter.lms(_currentVector, InputData.Frequency, Params.WindowLMS, Filtr_Type.LOWPASS, Params.Mi);
                                break;
                        }
                        OutputWorker.SaveSignal(_currentLeadName, true, _currentVector);
                        _state = STATE.NEXT_CHANNEL;
                    }
                    catch (Exception e)
                    {
                        _state = STATE.NEXT_CHANNEL;
                    }
                    break;
                case (STATE.NEXT_CHANNEL):
                    _state = STATE.BEGIN_CHANNEL;
                    break;
                case (STATE.END):
                    System.Console.WriteLine("Wylączyć");
                    _ended = true;
                    break;
                default:
                    Abort();
                    break;
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

        public Basic_New_Data InputData
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

        public Basic_New_Data_Worker InputWorker
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

        public ECG_Baseline_New_Data_Worker OutputWorker
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

        public static void Main(String[] args)
        {
            IModule testModule = new EKG_Project.Modules.ECG_Baseline.ECG_Baseline();
            //ECG_Baseline_Params param = new ECG_Baseline_Params();
            //ECG_Baseline_Params param = new ECG_Baseline_Params(Filtr_Method.SAV_GOL, Filtr_Type.LOWPASS, 10, "abc123");
            //ECG_Baseline_Params param = new ECG_Baseline_Params(Filtr_Method.SAV_GOL, Filtr_Type.BANDPASS, 10, 50, "abc123");
            //ECG_Baseline_Params param = new ECG_Baseline_Params(Filtr_Method.LMS, Filtr_Type.BANDPASS, 50, "abc123", 0.07);
            //ECG_Baseline_Params param = new ECG_Baseline_Params(Filtr_Method.MOVING_AVG, Filtr_Type.LOWPASS, 10, "abc123");
            //ECG_Baseline_Params param = new ECG_Baseline_Params(Filtr_Method.MOVING_AVG, Filtr_Type.BANDPASS, 10, 100, "abc123");
            ECG_Baseline_Params param = new ECG_Baseline_Params("abc123", Filtr_Method.BUTTERWORTH, Filtr_Type.LOWPASS, 5, 5);


            testModule.Init(param);
            while (!testModule.Ended())
            {
                testModule.ProcessData();
                Console.WriteLine(testModule.Progress());
            }
            //Console.ReadKey();
        }
    }
}