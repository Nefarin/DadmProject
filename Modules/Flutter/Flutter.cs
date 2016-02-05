using EKG_Project.IO;
using EKG_Project.Modules.Sleep_Apnea;
using EKG_Project.Modules.Waves;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.Flutter
{
    public class Flutter : IModule
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

        Flutter_Alg _flutter;

        public void Abort()
        {
            _aborted = true;
            _ended = true;
        }

        public bool IsAborted()
        {
            return _aborted;
        }

        public bool Ended()
        {
            return _ended;
        }

        public void Init(ModuleParams parameters)
        {
            //_aborted = false;
            //Params = parameters as Flutter_Params;
            //if(!Runnable())
            //{
            //    _ended = true;
            //}
            //else
            //{
            //    _ended = false;

            //    InputWorker_basic = new Basic_Data_Worker(Params.AnalysisName);
            //    InputWorker_basic.Load();
            //    InputData_basic = InputWorker_basic.BasicData;

            //    InputWorker = new Waves_Data_Worker(Params.AnalysisName);
            //    InputWorker.Load();
            //    InputData = InputWorker.Data;

            //    OutputWorker = new Flutter_Data_Worker(Params.AnalysisName);
            //    OutputData = new Flutter_Data();

            //    _actualProgress = 0;

            //    _fs = InputData_basic.Frequency;

            //    string[] channels = InputData.QRSOnsets.Select(x => x.Item1).ToArray();
            //    string[] expectedChannels = new string[] { "II", "III", "I" };
            //    int indexOf = -1;
            //    int i = 0;
            //    while(indexOf < 0 && i < expectedChannels.Length)
            //    {
            //        indexOf = Array.IndexOf(channels, expectedChannels[i++]);
            //    }
            //    if(indexOf == -1)
            //    {
            //        indexOf = 0;
            //    }
            //    _QRSonsets = InputData.QRSOnsets[indexOf].Item2;
            //    _Tends = InputData.TEnds[indexOf].Item2;
            //    _samples = InputData_basic.Signals[indexOf].Item2;
            //    _currentState = FlutterAlgStates.ExtractEcgFragments;
            //}

        }

        public void ProcessData()
        {
            if(Runnable())
            {
                try
                {
                    processData();
                }
                catch(Exception)
                {
                    OutputData.FlutterAnnotations = new List<Tuple<int,int>>();
                    _currentState = FlutterAlgStates.Finished;
                }
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
                case FlutterAlgStates.ExtractEcgFragments:
                    _t2qrsEkgParts = _flutter.GetEcgPart();
                    _currentState = FlutterAlgStates.CalculateSpectralDensity;
                    _actualProgress = 100.0 / 6;
                    break;

                case FlutterAlgStates.CalculateSpectralDensity:
                    _spectralDensityList = _flutter.CalculateSpectralDensity(_t2qrsEkgParts);
                    _frequenciesList = _flutter.CalculateFrequenciesAxis(_spectralDensityList);
                    _currentState = FlutterAlgStates.TrimSpectrum;
                    _actualProgress = 2 * 100.0 / 6;
                    break;

                case FlutterAlgStates.TrimSpectrum:
                    _flutter.TrimToGivenFreq(_spectralDensityList, _frequenciesList, 70.0);
                    _currentState = FlutterAlgStates.InterpolateSpectrum;
                    _actualProgress = 3 * 100.0 / 6;
                    break;

                case FlutterAlgStates.InterpolateSpectrum:
                    _flutter.InterpolateSpectralDensity(_spectralDensityList, _frequenciesList, 0.01);
                    _currentState = FlutterAlgStates.CalculatePower;
                    _actualProgress = 4 * 100.0 / 6;
                    break;

                case FlutterAlgStates.CalculatePower:
                    _powerList = _flutter.CalculateIntegralForEachSpectrum(_frequenciesList, _spectralDensityList);
                    _currentState = FlutterAlgStates.DetectAFL;
                    _actualProgress = 5 * 100.0 / 6;
                    break;

                case FlutterAlgStates.DetectAFL:
                    _aflAnnotations = _flutter.Detect(_spectralDensityList, _frequenciesList, _powerList);
                    //OutputData.FlutterAnnotations = _aflAnnotations;
                    _currentState = FlutterAlgStates.Finished;
                    _actualProgress = 99.0;
                    break;

                case FlutterAlgStates.Finished:
                    _actualProgress = 100.0;
                    //OutputWorker.Save(OutputData);
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
            string signal = "../../Sygnaly_Oznaczacz/iaf7_afw.txt";
            string qrs = "../../Sygnaly_Oznaczacz/iaf7_afw1-59999_qrsonsets.txt";
            string t = "../../Sygnaly_Oznaczacz/iaf7_afw1-59999_tends.txt";

            List<double> sammples = ReadSignal(signal);
            List<double> qrsonsets = ReadFromCSV(qrs);
            List<double> tends = ReadFromCSV(t);

            Flutter fl = new Flutter();
            fl._flutter = new Flutter_Alg(tends.Select(x => (int)x).ToList(), tends.Select(x => (int)x).ToList(), Vector<double>.Build.Dense(sammples.ToArray()), 100.0);

            while (fl.Progress() < 100.0)
            {
                fl.processData();
            }
        }

        private static List<double> ReadSignal(string path)
        {
            List<double> samples = new List<double>();
            StreamReader reader = new StreamReader(File.OpenRead(path));
            int n = 0;
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                n++;
                if (n <= 2) continue;
                              
                string[] values = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                samples.Add(double.Parse(values[1].Replace('.', ','), System.Globalization.NumberStyles.Float));

            }
            return samples;
        }

        private static List<double> ReadFromCSV(string path)
        {
            List<double> samples = new List<double>();
            StreamReader reader = new StreamReader(File.OpenRead(path));
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] values = line.Split(' ');

                samples.Add(double.Parse(values[0].Replace('.', ','), System.Globalization.NumberStyles.Float));

            }
            return samples;
        }
    }
}
