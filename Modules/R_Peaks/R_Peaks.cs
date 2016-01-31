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
    public class R_Peaks : IModule
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

        private ECG_Baseline_New_Data_Worker _inputWorker;
        private Basic_New_Data_Worker _inputWorker_basic;
        private R_Peaks_New_Data_Worker _outputWorker;

        private R_Peaks_Data _outputData;
        private ECG_Baseline_Data _inputData;
        private Basic_Data _inputData_basic;
        private R_Peaks_Params _params;

        private STATE _state;
        private R_Peaks_Alg _alg;
        private int _step;
        private Vector<Double> _currentVector;
                private Vector<double> _currentVectorRRInterval;
  
        public void Abort()
        {
            Aborted = true;
            _ended = true;
        }

        public bool Ended()
        {
            return _ended;
        }

        public bool IsAborted()
        {
            return Aborted;
        }

        public void Init(ModuleParams parameters)
        {
            try
            {
                _params = parameters as R_Peaks_Params;
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
                InputWorker_basic = new Basic_New_Data_Worker(Params.AnalysisName);
                InputWorker = new ECG_Baseline_New_Data_Worker(Params.AnalysisName);
                OutputWorker = new R_Peaks_New_Data_Worker(Params.AnalysisName); //+"temp" tez ma byc?
                InputData_basic = new Basic_Data();
                InputData = new ECG_Baseline_Data();
                OutputData = new R_Peaks_Data();

                //_step = 6000; //od fs?
                _step = Convert.ToInt32(InputData_basic.Frequency*16);
                _state = STATE.INIT;
            }
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
                    _leads = InputWorker_basic.LoadLeads().ToArray();
                    _numberOfChannels = _leads.Length;
                    _alg = new R_Peaks_Alg();
                    _state = STATE.BEGIN_CHANNEL;
                    break;
                case (STATE.BEGIN_CHANNEL):
                    _currentChannelIndex++;
                    if (_currentChannelIndex >= _numberOfChannels) _state = STATE.END;
                    else
                    {
                        _currentLeadName = _leads[_currentChannelIndex];
                        _currentChannelLength = (int)InputWorker_basic.getNumberOfSamples(_currentLeadName); //Zmienić na worker ECG_BASELINE
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
                            try         //zagniezdzone wyjatki?????????
                            {
                                //choosing and performing algorithm
                                switch (Params.Method)
                                {
                                case R_Peaks_Method.PANTOMPKINS:
                                    _currentVector = _alg.PanTompkins(_currentVector, InputData_basic.Frequency);
                                    break;
                                case R_Peaks_Method.HILBERT:
                                    _currentVector = _alg.Hilbert(_currentVector, InputData_basic.Frequency);
                                    break;
                                case R_Peaks_Method.EMD:
                                    _currentVector = _alg.EMD(_currentVector, InputData_basic.Frequency);
                                    break;
                                }
                                OutputWorker.SaveSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, false, _currentVector);
                                if (_currentVector.Count > 1)
                                {
                                    _currentVectorRRInterval = _alg.RRinMS(_currentVector, InputData_basic.Frequency);
                                    //save results
                                    OutputWorker.SaveSignal(R_Peaks_Attributes.RRInterval, _currentLeadName, false, _currentVectorRRInterval);
                                }
                                //updating current index
                                _currentIndex = Convert.ToInt32(_currentVector.Last()) + Convert.ToInt32(InputData_basic.Frequency * 0.1);
                                
                            }
                            catch(Exception ex)
                            {
                                _currentIndex = _currentIndex + _step;
                                //Console.WriteLine("No detected R peaks in this part of signal");
                            }
                            _state = STATE.PROCESS_CHANNEL;
                        }
                        catch (Exception e)
                        {
                            _state = STATE.NEXT_CHANNEL;
                        }
                    }
                    break;
                case (STATE.PROCESS_CHANNEL):  // this state can be divided to load state, process state and save state, good decision especially for ECG_Baseline, R_Peaks, Waves and Heart_Class
                    if (_currentIndex + _step > _currentChannelLength) _state = STATE.END_CHANNEL;
                    else
                    {
                        try
                        {
                            _currentVector = InputWorker.LoadSignal(_currentLeadName, _currentIndex, _step);
                            try         //zagniezdzone wyjatki?????????
                            {
                                //choosing and performing algorithm
                                switch (Params.Method)
                                {
                                    case R_Peaks_Method.PANTOMPKINS:
                                        _currentVector = _alg.PanTompkins(_currentVector, InputData_basic.Frequency);
                                        break;
                                    case R_Peaks_Method.HILBERT:
                                        _currentVector = _alg.Hilbert(_currentVector, InputData_basic.Frequency);
                                        break;
                                    case R_Peaks_Method.EMD:
                                        _currentVector = _alg.EMD(_currentVector, InputData_basic.Frequency);
                                        break;
                                }
                                _currentVector.Add(_currentIndex, _currentVector);
                                //save results
                                OutputWorker.SaveSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, true, _currentVector);
                                if (_currentVector.Count > 1)
                                {
                                    _currentVectorRRInterval = _alg.RRinMS(_currentVector, InputData_basic.Frequency);
                                    //save results
                                    OutputWorker.SaveSignal(R_Peaks_Attributes.RRInterval, _currentLeadName, true, _currentVectorRRInterval);
                                }
                                //updating current index
                                _currentIndex = Convert.ToInt32(_currentVector.Last()) + Convert.ToInt32(InputData_basic.Frequency * 0.1);
                            }
                            catch (Exception ex)
                            {
                                _currentIndex = _currentIndex + _step;
                                //Console.WriteLine("No detected R peaks in this part of signal");
                            }
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
                        try         //zagniezdzone wyjatki?????????
                        {
                            //choosing and performing algorithm
                            switch (Params.Method)
                            {
                                case R_Peaks_Method.PANTOMPKINS:
                                    _currentVector = _alg.PanTompkins(_currentVector, InputData_basic.Frequency);
                                    break;
                                case R_Peaks_Method.HILBERT:
                                    _currentVector = _alg.Hilbert(_currentVector, InputData_basic.Frequency);
                                    break;
                                case R_Peaks_Method.EMD:
                                    _currentVector = _alg.EMD(_currentVector, InputData_basic.Frequency);
                                    break;
                            }
                            _currentVector.Add(_currentIndex, _currentVector);
                            //save results
                            OutputWorker.SaveSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, true, _currentVector);
                            if (_currentVector.Count > 1)
                            {
                                _currentVectorRRInterval = _alg.RRinMS(_currentVector, InputData_basic.Frequency);
                                //save results
                                OutputWorker.SaveSignal(R_Peaks_Attributes.RRInterval, _currentLeadName, true, _currentVectorRRInterval);
                            }
                            //updating current index
                            _currentIndex = Convert.ToInt32(_currentVector.Last()) + Convert.ToInt32(InputData_basic.Frequency * 0.1);
                        }
                        catch (Exception ex)
                        {
                            _currentIndex = _currentIndex + _step;
                            //Console.WriteLine("No detected R peaks in this part of signal");
                        }
                        _state = STATE.NEXT_CHANNEL;
                    }
                    catch(Exception e)
                    {
                        _state = STATE.NEXT_CHANNEL;
                    }
                        break;
                case (STATE.NEXT_CHANNEL):
                    _state = STATE.BEGIN_CHANNEL;
                    break;
                case (STATE.END):
                    _ended = true;
                    break;
                default:
                    Abort();
                    break;
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

        public ECG_Baseline_New_Data_Worker InputWorker
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

        public Basic_New_Data_Worker InputWorker_basic
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

        public R_Peaks_New_Data_Worker OutputWorker
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
            IModule testModule = new EKG_Project.Modules.R_Peaks.R_Peaks();
            R_Peaks_Params param = new R_Peaks_Params(R_Peaks_Method.PANTOMPKINS, "abc123");

            testModule.Init(param);
            while (!testModule.Ended())
            {
                testModule.ProcessData();
                Console.WriteLine(testModule.Progress());
            }
            Console.ReadKey();
        }
    }
}
