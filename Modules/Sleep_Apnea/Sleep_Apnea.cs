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

        private bool _ended;
        private bool _aborted;

        private double _actualProgress;

        public Sleep_Apnea_Params Params { get; set; }
        public R_peaks_Data_Worker InputWorker { get; set; }
        public R_peaks_Data InputData { get; set; }
        public Sleep_Apnea_Data_Worker OutputWorker { get; set; }
        public Sleep_Apnea_Data OutputData { get; set; }
        public Basic_Data_Worker InputWorker_basic { get; set; }
        public Basic_Data InputData_basic { get; set; }

        //tu mam dac to co mi wystepuje w definicjach metod?
        List<uint> _R_detected;
        int _freq;
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
            _aborted = false;
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

                _fs = InputData_basic.Frequency;

                
                _R_detected = InputData.RPeaks.Select(x => x.Item2).First().Cast<uint>().ToList();
                _currentState = SleepApneaAlgStates.FindingRR;

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
                    _RR = findIntervals(_R_detected, _fs);
                    _currentState = SleepApneaAlgStates.CalculatingAverage;
                    _actualProgress = 100.0 / 9;
                    break;

                case SleepApneaAlgStates.CalculatingAverage:
                    _RR_average = averageFilter(_RR);
                    _currentState = SleepApneaAlgStates.Resampling;
                    _actualProgress = 2 * 100.0 / 9;
                    break;

                case SleepApneaAlgStates.Resampling:
                    _RR_res = resampling(_RR_average, _fs);
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
                    _actualProgress = 100.0;
                    OutputWorker.Save(OutputData);
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

        
        public Sleep_Apnea_Data OutputData
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

        public Sleep_Apnea_Params Params
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

        public Sleep_Apnea_Data_Worker OutputWorker
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
