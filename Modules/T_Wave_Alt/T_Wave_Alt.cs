using System;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;

namespace EKG_Project.Modules.T_Wave_Alt
{
    public class T_Wave_Alt : IModule
    {
        private enum STATE { INIT, BEGIN_CHANNEL, PROCESS_FIRST_STEP, PROCESS_CHANNEL, NEXT_CHANNEL, END_CHANNEL, END };
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _currentChannelTEndsLength;
        private string _currentLeadName;
        private string[] _leads;
        private int _currentIndex;
        private int _numberOfChannels;

        private uint _sampFreq;
        private int _step = 12000;
        private int _lastTEndIndex;
        private int _TEndStep = 80;

        private List<Vector<double>> T_WavesArray;
        private Vector<double> medianT_Wave;
        private Vector<double> ACI;
        private List<int> Flucts;
        private Vector<double> Alternans1;
        private List<Tuple<int, int>> finalDetection;

        private Basic_New_Data_Worker _inputBasicWorker;
        private ECG_Baseline_New_Data_Worker _inputECGWorker;
        private Waves_New_Data_Worker _inputWavesWorker;
        private T_Wave_Alt_New_Data_Worker _outputWorker;

        // private Basic_New_Data _outputData;
        // private Basic_New_Data _inputData;
        private T_Wave_Alt_Params _params;

        private T_Wave_Alt_Alg _alg;
        private Vector<double> _currentVector;
        private List<int> _currentTEndsList;

        private STATE _state;

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
                _params = parameters as T_Wave_Alt_Params;
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
                InputBasicWorker = new Basic_New_Data_Worker(Params.AnalysisName);
                InputECGWorker = new ECG_Baseline_New_Data_Worker(Params.AnalysisName);
                InputWavesWorker = new Waves_New_Data_Worker(Params.AnalysisName);
                OutputWorker = new T_Wave_Alt_New_Data_Worker(Params.AnalysisName);
                /*
                InputData = new Basic_New_Data();
                OutputData = new Basic_New_Data();
                */
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
                    _alg = new T_Wave_Alt_Alg();
                    _sampFreq = InputBasicWorker.LoadAttribute(Basic_Attributes.Frequency);
                    _alg.Fs = _sampFreq;
                    _leads = InputBasicWorker.LoadLeads().ToArray();
                    _numberOfChannels = _leads.Length;
                    _state = STATE.BEGIN_CHANNEL;
                    //_inputWorker.DeleteFiles(); Do not use yet - will try to handle this during loading.
                    break;
                case (STATE.BEGIN_CHANNEL):
                    _currentChannelIndex++;
                    if (_currentChannelIndex >= _numberOfChannels) _state = STATE.END;
                    else
                    {
                        _currentLeadName = _leads[_currentChannelIndex];
                        _currentChannelLength = (int)InputECGWorker.getNumberOfSamples(_currentLeadName);
                        _currentChannelTEndsLength = (int)InputWavesWorker.getNumberOfSamples(Waves_Signal.TEnds, _currentLeadName);
                        _currentIndex = 0;
                        _lastTEndIndex = 0;
                        _state = STATE.PROCESS_FIRST_STEP;
                    }
                    break;
                case (STATE.PROCESS_FIRST_STEP):
                    if (_currentIndex + _step > _currentChannelLength) _state = STATE.END_CHANNEL;
                    else
                    {
                        try
                        {
                            _currentVector = InputECGWorker.LoadSignal(_currentLeadName, _currentIndex, _step);
                            _currentTEndsList = InputWavesWorker.LoadSignal(Waves_Signal.TEnds, _currentLeadName, _lastTEndIndex, _TEndStep);
                            
                            T_WavesArray = _alg.buildTWavesArray(_currentVector, _currentTEndsList, _currentIndex);
                            medianT_Wave = _alg.calculateMedianTWave(T_WavesArray);
                            ACI = _alg.calculateACI(T_WavesArray, medianT_Wave);
                            Flucts = _alg.findFluctuations(ACI);
                            Alternans1 = _alg.findAlternans(Flucts);
                            finalDetection = _alg.alternansDetection(Alternans1, _currentTEndsList);

                            OutputWorker.SaveAlternansDetectedList(_currentLeadName, false, finalDetection);
                            _lastTEndIndex += (T_WavesArray.Count + _alg.NoDetectionCount - 1);
                            _currentIndex += _step;
                            _state = STATE.PROCESS_CHANNEL;
                        }
                        catch (Exception e)
                        {
                            _state = STATE.NEXT_CHANNEL;
                        }
                    }
                    break;
                case (STATE.PROCESS_CHANNEL):
                    if (_currentIndex + _step > _currentChannelLength) _state = STATE.END_CHANNEL;
                    else
                    {
                        try
                        {
                            _currentVector = InputECGWorker.LoadSignal(_currentLeadName, _currentIndex, _step);

                            if (_lastTEndIndex + _TEndStep < _currentChannelTEndsLength) _currentTEndsList = InputWavesWorker.LoadSignal(Waves_Signal.TEnds, _currentLeadName, _lastTEndIndex, _TEndStep);
                            else _currentTEndsList = InputWavesWorker.LoadSignal(Waves_Signal.TEnds, _currentLeadName, _lastTEndIndex, _currentChannelTEndsLength - _lastTEndIndex);

                            T_WavesArray = _alg.buildTWavesArray(_currentVector, _currentTEndsList, _currentIndex);
                            medianT_Wave = _alg.calculateMedianTWave(T_WavesArray);
                            ACI = _alg.calculateACI(T_WavesArray, medianT_Wave);
                            Flucts = _alg.findFluctuations(ACI);
                            Alternans1 = _alg.findAlternans(Flucts);
                            finalDetection = _alg.alternansDetection(Alternans1, _currentTEndsList);
                            
                            OutputWorker.SaveAlternansDetectedList(_currentLeadName, true, finalDetection);
                            _lastTEndIndex += (T_WavesArray.Count + _alg.NoDetectionCount - 1);
                            _currentIndex += _step;
                            _state = STATE.PROCESS_CHANNEL;
                        }
                        catch (Exception e)
                        {
                            _state = STATE.NEXT_CHANNEL;
                            Console.WriteLine(e.Message);
                        }
                    }

                    break;
                case (STATE.END_CHANNEL):
                    try
                    {
                        _currentVector = InputECGWorker.LoadSignal(_currentLeadName, _currentIndex, _currentChannelLength - _currentIndex);
                        _currentTEndsList = InputWavesWorker.LoadSignal(Waves_Signal.TEnds, _currentLeadName, _lastTEndIndex, _currentChannelTEndsLength - _lastTEndIndex);

                        T_WavesArray = _alg.buildTWavesArray(_currentVector, _currentTEndsList, _currentIndex);
                        medianT_Wave = _alg.calculateMedianTWave(T_WavesArray);
                        ACI = _alg.calculateACI(T_WavesArray, medianT_Wave);
                        Flucts = _alg.findFluctuations(ACI);
                        Alternans1 = _alg.findAlternans(Flucts);
                        finalDetection = _alg.alternansDetection(Alternans1, _currentTEndsList);

                        OutputWorker.SaveAlternansDetectedList(_currentLeadName, true, finalDetection);
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
                    _ended = true;
                    break;
                default:
                    Abort();
                    break;
            }

        }
        /*
        public Basic_New_Data OutputData
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
        */
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
        /*
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
        */
        public Basic_New_Data_Worker InputBasicWorker
        {
            get
            {
                return _inputBasicWorker;
            }

            set
            {
                _inputBasicWorker = value;
            }
        }

        public ECG_Baseline_New_Data_Worker InputECGWorker
        {
            get
            {
                return _inputECGWorker;
            }

            set
            {
                _inputECGWorker = value;
            }
        }

        public Waves_New_Data_Worker InputWavesWorker
        {
            get
            {
                return _inputWavesWorker;
            }

            set
            {
                _inputWavesWorker = value;
            }
        }

        public T_Wave_Alt_New_Data_Worker OutputWorker
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
            /*
            IModule testModule = new EKG_Project.Modules.TestModule3.TestModule3();
            int scale = 5;
            TestModule3_Params param = new TestModule3_Params(scale, 1000, "abc123");

            testModule.Init(param);
            while (!testModule.Ended())
            {
                testModule.ProcessData();
                Console.WriteLine(testModule.Progress());
            }
            */
        }
    }
}
