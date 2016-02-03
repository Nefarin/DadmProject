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
        uint _fs;
        string _lead;
        int _currentRPeak = 0;

        private Basic_New_Data_Worker _inputWorker_basic;
        private R_Peaks_New_Data_Worker _inputWorker;
        private Sleep_Apnea_New_Data_Worker _outputWorker;

        private Basic_Data _inputData_basic;
        private R_Peaks_Data _inputData;
        private Sleep_Apnea_Data _outputData;

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
            catch (Exception)
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
                _inputWorker_basic = new Basic_New_Data_Worker();
                _inputWorker = new R_Peaks_New_Data_Worker();
                _outputWorker = new Sleep_Apnea_New_Data_Worker();

                _inputData_basic = new Basic_Data();
                _inputData = new R_Peaks_Data();
                _outputData = new Sleep_Apnea_Data();

                //_fs = _inputWorker_basic.LoadAttribute(Basic_Attributes.Frequency);
                List<string> leads = _inputWorker_basic.LoadLeads();
                uint maxRs = 0;
                foreach (var lead in leads)
                {
                    if (_inputWorker.getNumberOfSamples(R_Peaks_Attributes.RPeaks, lead) > maxRs)
                    {
                        maxRs = _inputWorker.getNumberOfSamples(R_Peaks_Attributes.RPeaks, lead);
                        _lead = lead;
                    }
                }
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


            //switch (_currentState)
            //{
            //    case SleepApneaAlgStates.FindingRR:
            //        if (_R_detected.Count < 42)
            //        {
            //            _currentState = SleepApneaAlgStates.Finished;
            //            _il_Apnea = 0.0;
            //            _h_amp = new List<List<double>>();
            //            _Detected_Apnea = new List<Tuple<int, int>>();
            //            _Detected_Apnea.Add(new Tuple<int, int>(0, 0));
            //        }
            //        else
            //        {
            //            _RR = findIntervals(_R_detected, (int)_fs);
            //            _currentState = SleepApneaAlgStates.CalculatingAverage;
            //            _actualProgress = 100.0 / 9;
            //        }
            //        break;

            //    case SleepApneaAlgStates.CalculatingAverage:
            //        _RR_average = averageFilter(_RR);
            //        _currentState = SleepApneaAlgStates.Resampling;
            //        _actualProgress = 2 * 100.0 / 9;
            //        break;

            //    case SleepApneaAlgStates.Resampling:
            //        _RR_res = resampling(_RR_average, (int)_fs);
            //        _currentState = SleepApneaAlgStates.BandPassFiltering;
            //        _actualProgress = 3 * 100.0 / 9;
            //        break;

            //    case SleepApneaAlgStates.BandPassFiltering:
            //        _RR_HPLP = HPLP(_RR_res);
            //        _currentState = SleepApneaAlgStates.CaclulatingHilbert;
            //        _actualProgress = 4 * 100.0 / 9;
            //        break;

            //    case SleepApneaAlgStates.CaclulatingHilbert:
            //        _h_amp = new List<List<double>>(2);
            //        _h_freq = new List<List<double>>(2);
            //        hilbert(_RR_HPLP, ref _h_amp, ref _h_freq);
            //        _currentState = SleepApneaAlgStates.MedianFiltering;
            //        _actualProgress = 5 * 100.0 / 9;
            //        break;

            //    case SleepApneaAlgStates.MedianFiltering:
            //        median_filter(_h_freq, _h_amp);
            //        _currentState = SleepApneaAlgStates.AmplitudeFIltering;
            //        _actualProgress = 6 * 100.0 / 9;
            //        break;

            //    case SleepApneaAlgStates.AmplitudeFIltering:
            //        amp_filter(_h_amp);
            //        _currentState = SleepApneaAlgStates.DetecingApnea;
            //        _actualProgress = 7 * 100.0 / 9;
            //        break;

            //    case SleepApneaAlgStates.DetecingApnea:
            //        _Detected_Apnea = apnea_detection(_h_amp, _h_freq, out _il_Apnea);
            //        _currentState = SleepApneaAlgStates.Finished;
            //        _actualProgress = 8 * 100.0 / 9;
            //        break;

            //    case SleepApneaAlgStates.Finished:
            //        List<Tuple<string, List<Tuple<int, int>>>> detected_Apnea = new List<Tuple<string, List<Tuple<int, int>>>>();
            //        List<Tuple<string, List<List<double>>>> h_amp = new List<Tuple<string, List<List<double>>>>();
            //        List<Tuple<string, double>> il_Apnea = new List<Tuple<string, double>>();

            //        for (int i = 0; i < _numberOfChannels; i++)
            //        {
            //            il_Apnea.Add(new Tuple<string, double>(_channelsNames[i], _il_Apnea));
            //            h_amp.Add(new Tuple<string, List<List<double>>>(_channelsNames[i], _h_amp));
            //            detected_Apnea.Add(new Tuple<string, List<Tuple<int, int>>>(_channelsNames[i], _Detected_Apnea));
            //        }


            //        OutputData.Detected_Apnea = detected_Apnea;
            //        OutputData.h_amp = h_amp;
            //        OutputData.il_Apnea = il_Apnea;

            //        OutputWorker.Save(OutputData);

            //        _actualProgress = 100.0;
            //        _ended = true;
            //        break;

            //    default:
            //        throw new InvalidOperationException("Undefined state");

            //}

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
