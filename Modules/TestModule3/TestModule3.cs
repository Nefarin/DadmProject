using System;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;


namespace EKG_Project.Modules.TestModule3
{
    public class TestModule3 : IModule
    {
        private enum STATE {INIT, BEGIN_CHANNEL, PROCESS_CHANNEL, NEXT_CHANNEL, END};
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private string _currentLeadName;
        private string[] _leads;
        private int _currentIndex;
        private int _numberOfChannels;

        private Basic_New_Data_Worker _worker;

        private Basic_Data _outputData;
        private Basic_Data _inputData;
        private TestModule3_Params _params;

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
                Worker = new Basic_New_Data_Worker(Params.AnalysisName);
                InputData = new Basic_Data();
                OutputData = new Basic_Data();
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
            return 100.0 * ((double)_currentChannelIndex / (double)NumberOfChannels + (1.0/NumberOfChannels) * ((double) _currentIndex/ (double) _currentChannelLength));
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
                    _currentIndex = 0;
                    _numberOfChannels = 2; //Tutaj bedzie zmiana po poprawie workerów
                    _currentChannelLength = (int) Worker.LoadAttribute(Basic_Attributes.NumberOfSamples); //Tutaj bedzie zmiana po poprawie workerow
                    _leads = null; //Tutaj bedzie zmiana po poprawie workerów
                    _state = STATE.BEGIN_CHANNEL;
                    break;
                case (STATE.BEGIN_CHANNEL):
                    _currentChannelIndex++;
                    _currentLeadName = _leads[_currentChannelIndex];
                    break;
                case (STATE.PROCESS_CHANNEL):
                    break;
                case (STATE.NEXT_CHANNEL):
                    break;
                case (STATE.END):
                    _ended = true;
                    break;
                default:
                    Abort();
                    break;
            }

        }

        public Basic_Data OutputData
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

        public Basic_New_Data_Worker Worker
        {
            get
            {
                return _worker;
            }

            set
            {
                _worker = value;
            }
        }
    }
}
