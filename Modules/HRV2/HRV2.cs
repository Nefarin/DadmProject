using System;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;


namespace EKG_Project.Modules.HRV2
{
    public class HRV2 : IModule
    {
        private enum STATE { INIT, BEGIN_CHANNEL, INTERPOLATION, POINCAREX, POINCAREY, HISTOGRAM, ATRIBUTES, END_CHANNEL, END };
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private string _currentLeadName;
        private string[] _leads;
        private int _currentIndex;
        private int _numberOfChannels;

        private Basic_New_Data_Worker _basicWorker;
        private R_Peaks_New_Data_Worker _inputWorker;
        private HRV2_New_Data_Worker _outputWorker;

        private HRV2_Params _params;
        private HRV2_Alg _alg;
        private Vector<double> _currentVector;
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
                _params = parameters as HRV2_Params;
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
                InputWorker = new R_Peaks_New_Data_Worker(Params.AnalysisName);
                OutputWorker = new HRV2_New_Data_Worker(Params.AnalysisName);

                BasicWorker = new Basic_New_Data_Worker(Params.AnalysisName);
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
                    _leads = BasicWorker.LoadLeads().ToArray();
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
                        _currentChannelLength = (int)InputWorker.getNumberOfSamples(R_Peaks_Attributes.RRInterval, _currentLeadName);
                        _currentIndex = 0;
                        _state = STATE.INTERPOLATION;
                        _currentVector = InputWorker.LoadSignal(R_Peaks_Attributes.RRInterval, _currentLeadName, 0, _currentChannelLength);                        
                        _alg = new HRV2_Alg(_currentVector);
                    }
                    break;

                case (STATE.INTERPOLATION):

                    _alg.Interpolation();

                    _state = STATE.POINCAREX;
                    break;

                case (STATE.POINCAREX):
                    _alg.PoincarePlot_x();
                    OutputWorker.SaveSignal(HRV2_Signal.PoincarePlotData_x, _currentLeadName, false, _alg.RR_intervals_x);

                    _state = STATE.POINCAREY;
                    break;

                case (STATE.POINCAREY):
                    _alg.PoincarePlot_y();
                    OutputWorker.SaveSignal(HRV2_Signal.PoincarePlotData_y, _currentLeadName, false, _alg.RR_intervals_y);

                    _state = STATE.HISTOGRAM;
                    break;

                case (STATE.HISTOGRAM):
                    _alg.HistogramToVisualisation();
                    OutputWorker.SaveHistogram(_currentLeadName, false, _alg.HistogramToVisualisation());

                    _state = STATE.ATRIBUTES;
                    break;

                case (STATE.ATRIBUTES):
                    OutputWorker.SaveAttribute(HRV2_Attributes.SD1, _currentLeadName, _alg.SD1());
                    OutputWorker.SaveAttribute(HRV2_Attributes.SD2, _currentLeadName, _alg.SD2());

                    OutputWorker.SaveAttribute(HRV2_Attributes.ElipseCenter_x, _currentLeadName, _alg.elipseCenter_x());
                    OutputWorker.SaveAttribute(HRV2_Attributes.ElipseCenter_y, _currentLeadName, _alg.elipseCenter_y());

                    _alg.makeTinn();
                    OutputWorker.SaveAttribute(HRV2_Attributes.Tinn, _currentLeadName, _alg.tinn);

                    _alg.makeTriangleIndex();
                    OutputWorker.SaveAttribute(HRV2_Attributes.TriangleIndex, _currentLeadName, _alg.triangleIndex);

                    _state = STATE.END_CHANNEL;
                    break;

                case (STATE.END_CHANNEL):
                        _currentVector = InputWorker.LoadSignal(R_Peaks_Attributes.RRInterval, _currentLeadName, _currentIndex, _currentChannelLength - _currentIndex);
                        _alg = new HRV2_Alg(_currentVector);
                        _currentVector = _alg.RRIntervals;


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

        public HRV2_Params Params
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


        public R_Peaks_New_Data_Worker InputWorker
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
        

        public HRV2_New_Data_Worker OutputWorker
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

        public Basic_New_Data_Worker BasicWorker
        {
            get
            {
                return _basicWorker;
            }

            set
            {
                _basicWorker = value;
            }
        }

        public static void Main(String[] args)
        {
            IModule HRV2 = new EKG_Project.Modules.HRV2.HRV2();
            int scale = 5;
            HRV2_Params param = new HRV2_Params(scale, 1000, "cba123");

            HRV2.Init(param);
            while (!HRV2.Ended())
            {
                HRV2.ProcessData();
                Console.WriteLine(HRV2.Progress());
            }
        }
    }
}
