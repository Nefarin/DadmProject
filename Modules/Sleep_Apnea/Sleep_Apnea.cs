using System;
using EKG_Project.IO;
using EKG_Project.Modules.R_Peaks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EKG_Project.Modules.Sleep_Apnea
{
    public partial class Sleep_Apnea : IModule
    {
        private enum SleepApneaAlgStates
        {
            FindingRR,
            CalculatingAverage,
            Resampling,
            BandPassFiltering,
            CaclulatingHilbert,
            MedianFiltering,
            AmplitudeFIltering,
            DetecingApnea,
            Finished
        }

        private SleepApneaAlgStates _currentState;

        private double _actualProgress;
        private bool _ended;
        private int _numberOfChannels;
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
        private Basic_Data _inputData_basic;

        public Basic_Data InputData_basic
        {
            get { return _inputData_basic; }
            set { _inputData_basic = value; }
        }
        private Basic_Data_Worker _inputWorker_basic;

        public Basic_Data_Worker InputWorker_basic
        {
            get { return _inputWorker_basic; }
            set { _inputWorker_basic = value; }
        }
        private R_Peaks_Data_Worker _inputWorker;

        public R_Peaks_Data_Worker InputWorker
        {
            get { return _inputWorker; }
            set { _inputWorker = value; }
        }
        private R_Peaks_Data _inputData;

        public R_Peaks_Data InputData
        {
            get { return _inputData; }
            set { _inputData = value; }
        }
        private Sleep_Apnea_Data_Worker _outputWorker;

        public Sleep_Apnea_Data_Worker OutputWorker
        {
            get { return _outputWorker; }
            set { _outputWorker = value; }
        }
        private Sleep_Apnea_Data _outputData;

        public Sleep_Apnea_Data OutputData
        {
            get { return _outputData; }
            set { _outputData = value; }
        }

        string[] _channelsNames;

        List<uint> _R_detected;
        double _fs;
        List<List<double>> _RR;
        List<List<double>> _RR_average;
        List<List<double>> _RR_HPLP;
        List<List<double>> _RR_res;
        List<List<double>> _h_amp;
        List<List<double>> _h_freq;
        double _il_Apnea;
        List<Tuple<int, int>> _Detected_Apnea;

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
            Params = parameters as Sleep_Apnea_Params;
            Aborted = false;
            if (!Runnable())
            {
                _ended = true;
            }
            else
            {
                _ended = false;

                InputWorker_basic = new Basic_Data_Worker(Params.AnalysisName);
                InputWorker_basic.Load();
                InputData_basic = InputWorker_basic.BasicData;

                InputWorker = new R_Peaks_Data_Worker(Params.AnalysisName);
                InputWorker.Load();
                InputData = InputWorker.Data;

                OutputWorker = new Sleep_Apnea_Data_Worker(Params.AnalysisName);
                OutputData = new Sleep_Apnea_Data();

                _actualProgress = 0;
                _numberOfChannels = InputData_basic.Signals.Count;
                _fs = InputData_basic.Frequency;
                _R_detected = InputData.RPeaks.Select(x => x.Item2).First().Cast<uint>().ToList();
                _currentState = SleepApneaAlgStates.FindingRR;
                _channelsNames = InputData_basic.Signals.Select(x => x.Item1).ToArray();

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
                case SleepApneaAlgStates.FindingRR:
                    _RR = findIntervals(_R_detected, (int)_fs);
                    _currentState = SleepApneaAlgStates.CalculatingAverage;
                    _actualProgress = 100.0 / 9;
                    break;

                case SleepApneaAlgStates.CalculatingAverage:
                    _RR_average = averageFilter(_RR);
                    _currentState = SleepApneaAlgStates.Resampling;
                    _actualProgress = 2 * 100.0 / 9;
                    break;

                case SleepApneaAlgStates.Resampling:
                    _RR_res = resampling(_RR_average, (int)_fs);
                    _currentState = SleepApneaAlgStates.BandPassFiltering;
                    _actualProgress = 3 * 100.0 / 9;
                    break;

                case SleepApneaAlgStates.BandPassFiltering:
                    _RR_HPLP = HPLP(_RR_res);
                    _currentState = SleepApneaAlgStates.CaclulatingHilbert;
                    _actualProgress = 4 * 100.0 / 9;
                    break;

                case SleepApneaAlgStates.CaclulatingHilbert:
                    _h_amp = new List<List<double>>(2);
                    _h_freq = new List<List<double>>(2);
                    hilbert(_RR_HPLP, ref _h_amp, ref _h_freq);
                    _currentState = SleepApneaAlgStates.MedianFiltering;
                    _actualProgress = 5 * 100.0 / 9;
                    break;

                case SleepApneaAlgStates.MedianFiltering:
                    median_filter(_h_freq, _h_amp);
                    _currentState = SleepApneaAlgStates.AmplitudeFIltering;
                    _actualProgress = 6 * 100.0 / 9;
                    break;

                case SleepApneaAlgStates.AmplitudeFIltering:
                    amp_filter(_h_amp);
                    _currentState = SleepApneaAlgStates.DetecingApnea;
                    _actualProgress = 7 * 100.0 / 9;
                    break;

                case SleepApneaAlgStates.DetecingApnea:
                    _Detected_Apnea = apnea_detection(_h_amp, _h_freq, out _il_Apnea);
                    _currentState = SleepApneaAlgStates.Finished;
                    _actualProgress = 8 * 100.0 / 9;
                    break;

                case SleepApneaAlgStates.Finished:
                    List<Tuple<string, List<Tuple<int, int>>>> detected_Apnea = new List<Tuple<string, List<Tuple<int, int>>>>();
                    List<Tuple<string, List<List<double>>>> h_amp = new List<Tuple<string, List<List<double>>>>();
                    List<Tuple<string, double>> il_Apnea = new List<Tuple<string, double>>();

                    for (int i = 0; i < _numberOfChannels; i++)
                    {
                        il_Apnea.Add(new Tuple<string, double>(_channelsNames[i], _il_Apnea));
                        h_amp.Add(new Tuple<string, List<List<double>>>(_channelsNames[i], _h_amp));
                        detected_Apnea.Add(new Tuple<string, List<Tuple<int, int>>>(_channelsNames[i], _Detected_Apnea));
                    }


                    OutputData.Detected_Apnea = detected_Apnea;
                    OutputData.h_amp = h_amp;
                    OutputData.il_Apnea = il_Apnea;

                    OutputWorker.Save(OutputData);

                    _actualProgress = 100.0;
                    _ended = true;
                    break;

                default:
                    throw new InvalidOperationException("Undefined state");

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
            Sleep_Apnea_Params param = new Sleep_Apnea_Params("Sleep Apnea");
            Sleep_Apnea sleep_apnea = new Sleep_Apnea();
            sleep_apnea.Init(param);
            while (true)
            {
                if (sleep_apnea.Ended())
                {
                    break;
                }
                Console.WriteLine(sleep_apnea.Progress());
                sleep_apnea.ProcessData();
            }
        }
    }
}
