using EKG_Project.IO;
using EKG_Project.Modules.Waves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.Flutter
{
    public partial class Flutter : IModule
    {
        private enum FlutterAlgStates 
        {
            ExtractEcgFragments,
            CalculateSpectralDensity,
            TrimSpectrum,
            InterpolateSpectrum,
            CalculatePower,
            DetectAFL,
            DetectingAFL,
            Finished
        }

        private FlutterAlgStates _currentState;

        private bool _ended;
        private bool _aborted;

        private double _actualProgress;

        public Flutter_Params Params { get; set; }
        public Waves_Data_Worker InputWorker { get; set; }
        public Waves_Data InputData { get; set; }
        public Flutter_Data_Worker OutputWorker { get; set; }
        public Flutter_Data OutputData { get; set; }
        public Basic_Data_Worker InputWorker_basic { get; set; }
        public Basic_Data InputData_basic { get; set; }

        List<double[]> _t2qrsEkgParts;
        List<double[]> _spectralDensityList;
        List<double[]> _frequenciesList;
        List<double> _powerList;
        List<Tuple<int, int>> _aflAnnotations;

        public void Abort()
        {
            _aborted = true;
            _ended = true;
        }

        public bool Ended()
        {
            return _ended;
        }

        public void Init(ModuleParams parameters)
        {
            _aborted = false;
            Params = parameters as Flutter_Params;
            if(!Runnable())
            {
                _ended = true;
            }
            else
            {
                _ended = false;

                InputWorker_basic = new Basic_Data_Worker(Params.AnalysisName);
                InputWorker_basic.Load();
                InputData_basic = InputWorker_basic.BasicData;

                InputWorker = new Waves_Data_Worker(Params.AnalysisName);
                InputWorker.Load();
                InputData = InputWorker.Data;

                OutputWorker = new Flutter_Data_Worker(Params.AnalysisName);
                OutputData = new Flutter_Data();

                _actualProgress = 0;

                _fs = InputData_basic.Frequency;

                string[] channels = InputData.QRSOnsets.Select(x => x.Item1).ToArray();
                string[] expectedChannels = new string[] { "II", "III", "I" };
                int indexOf = -1;
                int i = 0;
                while(indexOf < 0 && i < expectedChannels.Length)
                {
                    indexOf = Array.IndexOf(channels, expectedChannels[i++]);
                }
                if(indexOf == -1)
                {
                    indexOf = 0;
                }
                _QRSonsets = InputData.QRSOnsets[indexOf].Item2;
                _Tends = InputData.TEnds[indexOf].Item2;
                _samples = InputData_basic.Signals[indexOf].Item2;
                _currentState = FlutterAlgStates.ExtractEcgFragments;
            }

        }

        public void ProcessData()
        {
            if(Runnable())
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
            switch(_currentState)
            {
                case FlutterAlgStates.ExtractEcgFragments:
                    _t2qrsEkgParts = GetEcgPart();
                    _currentState = FlutterAlgStates.CalculateSpectralDensity;
                    _actualProgress = 100.0 / 6;
                    break;

                case FlutterAlgStates.CalculateSpectralDensity:
                    _spectralDensityList = CalculateSpectralDensity(_t2qrsEkgParts);
                    _frequenciesList = CalculateFrequenciesAxis(_spectralDensityList);
                    _currentState = FlutterAlgStates.TrimSpectrum;
                    _actualProgress = 2* 100.0 / 6;
                    break;

                case FlutterAlgStates.TrimSpectrum:
                    TrimToGivenFreq(_spectralDensityList, _frequenciesList, 70.0);
                    _currentState = FlutterAlgStates.InterpolateSpectrum;
                    _actualProgress = 3 * 100.0 / 6;
                    break;

                case FlutterAlgStates.InterpolateSpectrum:
                    InterpolateSpectralDensity(_spectralDensityList, _frequenciesList, 0.01);
                    _currentState = FlutterAlgStates.CalculatePower;
                    _actualProgress = 4 * 100.0 / 6;
                    break;

                case FlutterAlgStates.CalculatePower:
                    _powerList = CalculateIntegralForEachSpectrum(_frequenciesList, _spectralDensityList);
                    _currentState = FlutterAlgStates.DetectAFL;
                    _actualProgress = 5 * 100.0 / 6;
                    break;

                case FlutterAlgStates.DetectAFL:
                    _aflAnnotations = Detect(_spectralDensityList, _frequenciesList, _powerList);
                    OutputData.FlutterAnnotations = _aflAnnotations;
                    _currentState = FlutterAlgStates.Finished;
                    _actualProgress = 99.0;
                    break;

                case FlutterAlgStates.Finished:
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

        public static void Main()
        {
            Flutter_Params param = new Flutter_Params("TestAnalysis8");
            Flutter flutter = new Flutter();
            flutter.Init(param);
            while(true)
            {
                if(flutter.Ended())
                {
                    break;
                }
                Console.WriteLine(flutter.Progress());
                flutter.ProcessData();
            }
        }
    }
}
