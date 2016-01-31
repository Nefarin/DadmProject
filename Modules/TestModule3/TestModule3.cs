using System;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;

namespace EKG_Project.Modules.TestModule3
{
    public class TestModule3 : IModule
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

        private Basic_New_Data_Worker _inputWorker;
        private Basic_New_Data_Worker _outputWorker;

        private Basic_New_Data _outputData;
        private Basic_New_Data _inputData;
        private TestModule3_Params _params;

        private TestModule3_Alg _alg;
        private Vector<Double> _currentVector;
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
                _params = parameters as TestModule3_Params;
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
                OutputWorker = new Basic_New_Data_Worker(Params.AnalysisName + "temp"); // tutaj generalnie uzywacie swojego workera jako wyjsciowy, dla uproszczenia uzywam gotowego o innej nazwie
                InputData = new Basic_New_Data();
                OutputData = new Basic_New_Data();
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
                    _leads = InputWorker.LoadLeads().ToArray();
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
                        _currentChannelLength = (int)InputWorker.getNumberOfSamples(_currentLeadName);
                        _currentIndex = 0;
                        _state = STATE.PROCESS_FIRST_STEP;
                    }
                    break;
                case (STATE.PROCESS_FIRST_STEP):
                    if (_currentIndex + Params.Step > _currentChannelLength) _state = STATE.END_CHANNEL;
                    else
                    {
                        try
                        {
                            _currentVector = InputWorker.LoadSignal(_currentLeadName, _currentIndex, Params.Step);
                            _alg = new TestModule3_Alg(_currentVector, Params);
                            _alg.scaleSamples();
                            _currentVector = _alg.CurrentVector;
                            OutputWorker.SaveSignal(_currentLeadName, false, _currentVector);
                            _currentIndex += Params.Step;
                            _state = STATE.PROCESS_CHANNEL;
                        }
                        catch (Exception e)
                        {
                            _state = STATE.NEXT_CHANNEL;
                        }
                    }
                    break;
                case (STATE.PROCESS_CHANNEL): // this state can be divided to load state, process state and save state, good decision especially for ECG_Baseline, R_Peaks, Waves and Heart_Class
                    if (_currentIndex + Params.Step > _currentChannelLength) _state = STATE.END_CHANNEL;
                    else
                    {
                        try
                        {
                            _currentVector = InputWorker.LoadSignal(_currentLeadName, _currentIndex, Params.Step);
                            _alg = new TestModule3_Alg(_currentVector, Params); // its possible to create just one instance somewhere in the Init - your choice
                            _alg.scaleSamples();
                            _currentVector = _alg.CurrentVector; // not needed, because reference is the same, but shows the point
                            OutputWorker.SaveSignal(_currentLeadName, true, _currentVector);
                            _currentIndex += Params.Step;
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
                        _alg = new TestModule3_Alg(_currentVector, Params);
                        _currentVector = _alg.CurrentVector; // not needed, because reference is the same, but shows the point
                        _alg.scaleSamples();
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
                    _ended = true;
                    break;
                default:
                    Abort();
                    break;
            }

        }

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

        public Basic_New_Data_Worker OutputWorker
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
            IModule testModule = new EKG_Project.Modules.TestModule3.TestModule3();
            int scale = 5;
            TestModule3_Params param = new TestModule3_Params(scale, 1000, "abc123");

            testModule.Init(param);
            while (!testModule.Ended())
            {
                testModule.ProcessData();
                Console.WriteLine(testModule.Progress());
            }
        }
    }
}
