using System;
using EKG_Project.IO;
using EKG_Project.Modules.R_Peaks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;


namespace EKG_Project.Modules.Sleep_Apnea
{
    public class Sleep_Apnea : IModule
    {
        private const int DEFAULT_STEP = 600;
        private const int NUMBER_OF_STATES = 9;

        private enum State
        {
            FindingRR,
            CalculatingAverage,
            Resampling,
            BandPassFiltering,
            CaclulatingHilbert,
            MedianFiltering,
            AmplitudeFiltering,
            DetecingApnea,
            AmplitudeNormalization,
            Finished
        }

        public bool IsAborted()
        {
            return Aborted;
        }

        private bool _aborted;
        public bool Aborted
        {
            get { return _aborted; }
            set { _aborted = value; }
        }

        private Sleep_Apnea_Params _params;
        public Sleep_Apnea_Params Params
        {
            get { return _params; }
            set { _params = value; }
        }

        private bool _ended;

        double _actualProgress;
        double _lastProgress;
        uint _fs;
        string _lead;
        int _currentRPeak = -1;
        int _step = DEFAULT_STEP;
        int _currentLeadLength;
        private State _currentState;
        private int _numberOfChannels;
        private int _resampFreq = 1;

        private Sleep_Apnea_Alg _sleepApneaAlg;

        private List<List<double>> _RRIntervals;
                
        private List<string> _leads;

        private Basic_New_Data_Worker _inputWorker_basic;
        private R_Peaks_New_Data_Worker _inputWorker;
        private Sleep_Apnea_New_Data_Worker _outputWorker;

        private Basic_Data _inputData_basic;
        private R_Peaks_Data _inputData;
        private Sleep_Apnea_Data _outputData;
        private List<List<double>> _RR_average;
        private List<List<double>> _RR_res;
        private List<List<double>> _RR_HP;
        private List<List<double>> _RR_LP;
        private List<List<double>> _h_amp;
        private List<List<double>> _h_freq;
        private List<bool> _detected;
        private List<double> _time;
        

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
            try
            {
                Params = parameters as Sleep_Apnea_Params;
            }
            catch(Exception)
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
                _ended = false;
                _inputWorker_basic = new Basic_New_Data_Worker(Params.AnalysisName);
                _inputWorker = new R_Peaks_New_Data_Worker(Params.AnalysisName);
                _outputWorker = new Sleep_Apnea_New_Data_Worker(Params.AnalysisName);

                _inputData_basic = new Basic_Data();
                _inputData = new R_Peaks_Data();
                _outputData = new Sleep_Apnea_Data();

                _fs = _inputWorker_basic.LoadAttribute(Basic_Attributes.Frequency);
                _leads = _inputWorker_basic.LoadLeads();
                _numberOfChannels = _leads.Count;
                _currentLeadLength = 0;
                foreach (var lead in _leads)
                {
                    if(_inputWorker.getNumberOfSamples(R_Peaks_Attributes.RPeaks, lead) > _currentLeadLength)
                    {
                        _currentLeadLength = (int)_inputWorker.getNumberOfSamples(R_Peaks_Attributes.RPeaks, lead);
                        _lead = lead;
                    }
                }
                _currentState = State.FindingRR;
                _sleepApneaAlg = new Sleep_Apnea_Alg();
                _actualProgress = 0;
            }
        }

        public void ProcessData()
        {
            if (Runnable())
            {
                processData();
            }
            else
            {
                _ended = true;
            }
        }



        private void processData()
        {

            switch (_currentState)
            {
                case State.FindingRR:
                    if (_currentRPeak == -1)
                    {                      
                        _RRIntervals = new List<List<double>>(2);
                        _RRIntervals.Add(new List<double>(_currentLeadLength));
                        _RRIntervals.Add(new List<double>(_currentLeadLength));
                        _currentRPeak = 0;
                    }
                    else if(_currentRPeak + _step <= _currentLeadLength)
                    {
                        Vector<double> r = _inputWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _lead, _currentRPeak, _step);
                        List<uint> RPeaks = r.Select(x => (uint)x).ToList();
                        List<List<double>> partOfRRIntervals = _sleepApneaAlg.findIntervals(RPeaks, (int)_fs);
                        _currentRPeak += _step - 1;
                        _actualProgress = 100.0 * _currentRPeak / _currentLeadLength / NUMBER_OF_STATES;
                        _RRIntervals[0].AddRange(partOfRRIntervals[0]);
                        _RRIntervals[1].AddRange(partOfRRIntervals[1]);
                    }
                    else
                    {
                        int restRPeaksToRead = _currentLeadLength - _currentRPeak;
                        Vector<double> r = _inputWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _lead, _currentRPeak, restRPeaksToRead);
                        List<uint> RPeaks = r.Select(x => (uint)x).ToList();
                        List<List<double>> partOfRRIntervals = _sleepApneaAlg.findIntervals(RPeaks, (int)_fs);
                        _actualProgress = 100.0 * _currentRPeak / _currentLeadLength / NUMBER_OF_STATES;
                        _RRIntervals[0].AddRange(partOfRRIntervals[0]);
                        _RRIntervals[1].AddRange(partOfRRIntervals[1]);
                        _currentState = State.CalculatingAverage;
                    }
                    
                    break;

                case State.CalculatingAverage:
                    _RR_average = _sleepApneaAlg.averageFilter(_RRIntervals);
                    _currentState = State.Resampling;
                    _actualProgress = 2 * 100.0 / NUMBER_OF_STATES;
                    break;

                case State.Resampling:
                    _RR_res = _sleepApneaAlg.resampling(_RR_average, (int)_resampFreq);
                    _currentState = State.BandPassFiltering;
                    _actualProgress = 3 * 100.0 / NUMBER_OF_STATES;
                    break;

                case State.BandPassFiltering:
                    _RR_HP = _sleepApneaAlg.HP(_RR_res);
                    _RR_LP = _sleepApneaAlg.LP(_RR_HP);
                    _currentState = State.CaclulatingHilbert;
                    _actualProgress = 4 * 100.0 / NUMBER_OF_STATES;
                    break;

                case State.CaclulatingHilbert:
                    _h_amp = new List<List<double>>(2);
                    _h_freq = new List<List<double>>(2);
                    _sleepApneaAlg.hilbert(_RR_LP, ref _h_amp, ref _h_freq);
                    _currentState = State.MedianFiltering;
                    _actualProgress = 5 * 100.0 / NUMBER_OF_STATES;
                    break;

                case State.MedianFiltering:
                     _sleepApneaAlg.medianFilter(_h_freq, _h_amp);
                    _currentState = State.AmplitudeNormalization;
                    _actualProgress = 6 * 100.0 / NUMBER_OF_STATES;
                    break;

                case State.AmplitudeNormalization:
                    _sleepApneaAlg.ampNormalization(_h_amp);
                    _currentState = State.DetecingApnea;
                    _actualProgress = 7 * 100.0 / NUMBER_OF_STATES;
                    break;

                case State.DetecingApnea:
                    _detected = new List<bool>();
                    _time = new List<double>();
                    _sleepApneaAlg.detectApnea(_h_amp, _h_freq, _detected, _time);
                    _currentState = State.Finished;
                    _actualProgress = 8 * 100.0 / NUMBER_OF_STATES;
                    break;

                case State.Finished:
                    List<Tuple<string, List<Tuple<int, int>>>> detected_Apnea = new List<Tuple<string, List<Tuple<int, int>>>>();
                    List<Tuple<string, List<List<double>>>> h_amp = new List<Tuple<string, List<List<double>>>>();
                    List<Tuple<string, double>> il_Apnea = new List<Tuple<string, double>>();

                    double ilApnea;
                    List<Tuple<int, int>> annotations = _sleepApneaAlg.setResult(_detected, _time, out ilApnea);

                    for (int i = 0; i < _numberOfChannels; i++)
                    {
                        il_Apnea.Add(new Tuple<string, double>(_leads[i], ilApnea));
                        h_amp.Add(new Tuple<string, List<List<double>>>(_leads[i], _h_amp));
                        detected_Apnea.Add(new Tuple<string, List<Tuple<int, int>>>(_leads[i], annotations));

                        _outputWorker.SaveIlApnea(_leads[i], ilApnea);
                        _outputWorker.SaveHAmp(_leads[i], true, _h_amp);
                        _outputWorker.SaveDetectedApnea(_leads[i], true, annotations);
                    }

                   
                    _actualProgress = 100.0;
                    _ended = true;
                    break;

                default:
                    Abort();
                    break;

            }

        }

        public double Progress()
        {
            return _actualProgress;
        }

        public bool Runnable()
        {
            return Params != null;
        }

        public static void Main()
        {
            IModule module = new Sleep_Apnea();
            Sleep_Apnea_Params param = new Sleep_Apnea_Params("analysis1");

            module.Init(param);
            while (!module.Ended())
            {
                module.ProcessData();
                Console.WriteLine(module.Progress());
            }
        }
    }
}
