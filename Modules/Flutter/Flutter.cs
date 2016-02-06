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
        uint _fs;
        int _indexOfLead;
        int _n;
        List<string> _channels;

        public Flutter_Params Params { get; set; }
        public Waves_New_Data_Worker InputWorkerWaves { get; set; }
        public ECG_Baseline_New_Data_Worker InputWorkerBaseline { get; set; }
        public Flutter_New_Data_Worker OutputWorker { get; set; }
        public Basic_New_Data_Worker InputWorker_basic { get; set; }

        List<double[]> _t2qrsEkgParts;
        List<double[]> _spectralDensityList;
        List<double[]> _frequenciesList;
        List<double> _powerList;
        List<Tuple<int, int>> _aflAnnotations;

        Flutter_Alg _flutter;
        private List<int> _QRSonsets;
        private List<int> _Tends;
        private Vector<double> _samples;

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
            try
            {
                Params = parameters as Flutter_Params;
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

                InputWorker_basic = new Basic_New_Data_Worker(Params.AnalysisName);
                InputWorkerBaseline = new ECG_Baseline_New_Data_Worker(Params.AnalysisName);
                InputWorkerWaves = new Waves_New_Data_Worker(Params.AnalysisName);
                OutputWorker = new Flutter_New_Data_Worker(Params.AnalysisName);

                _actualProgress = 0;

                _fs = InputWorker_basic.LoadAttribute(Basic_Attributes.Frequency);

                _channels = InputWorker_basic.LoadLeads();
                string[] expectedChannels = new string[] { "II", "MLII", "III", "MLIII", "I", "MLI" };
                _indexOfLead = -1;
                int i = 0;
                while (_indexOfLead < 0 && i < expectedChannels.Length)
                {
                    _indexOfLead = _channels.IndexOf(expectedChannels[i++]);
                }
                if (_indexOfLead == -1)
                {
                    _indexOfLead = 0;
                }

                uint length = InputWorkerWaves.getNumberOfSamples(Waves_Signal.QRSOnsets, _channels[_indexOfLead]);
                _QRSonsets = InputWorkerWaves.LoadSignal(Waves_Signal.QRSOnsets, _channels[_indexOfLead], 0, (int)length);

                length = InputWorkerWaves.getNumberOfSamples(Waves_Signal.TEnds, _channels[_indexOfLead]);
                _Tends = InputWorkerWaves.LoadSignal(Waves_Signal.TEnds, _channels[_indexOfLead], 0, (int)length);

                length = InputWorkerBaseline.getNumberOfSamples(_channels[_indexOfLead]);
                _samples = InputWorkerBaseline.LoadSignal(_channels[_indexOfLead], 0, (int)length);

                _currentState = FlutterAlgStates.ExtractEcgFragments;

                _flutter = new Flutter_Alg(_Tends, _QRSonsets, _samples, _fs);
            }

        }

        public void ProcessData()
        {
            if(Runnable())
            {
                //try
                //{
                    processData();
                //}
                //catch(Exception)
                //{                 
                    //_currentState = FlutterAlgStates.Finished;
                //}
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
                    _actualProgress = 0.01;
                    break;

                case FlutterAlgStates.CalculateSpectralDensity:
                    if(_spectralDensityList == null)
                    {
                        _spectralDensityList = new List<double[]>(_t2qrsEkgParts.Count);
                        _frequenciesList = new List<double[]>(_t2qrsEkgParts.Count);
                        _n = 0;
                    }
                    else
                    {
                        if (_n < _t2qrsEkgParts.Count)
                        {
                            _spectralDensityList.AddRange(_flutter.CalculateSpectralDensity(_t2qrsEkgParts.GetRange(_n, 1)));
                            _frequenciesList.AddRange(_flutter.CalculateFrequenciesAxis(_spectralDensityList.GetRange(_n, 1)));
                            _n++;
                            _actualProgress = 95.0 * _n / _t2qrsEkgParts.Count + 0.01;
                        }
                        else
                        {
                            _currentState = FlutterAlgStates.TrimSpectrum;
                            _n = 0;
                        }

                    }                    
                    
                    break;

                case FlutterAlgStates.TrimSpectrum:
                    _flutter.TrimToGivenFreq(_spectralDensityList, _frequenciesList, 70.0);
                    _currentState = FlutterAlgStates.InterpolateSpectrum;
                    _actualProgress = 96.0;
                    break;

                case FlutterAlgStates.InterpolateSpectrum:
                    _flutter.InterpolateSpectralDensity(_spectralDensityList, _frequenciesList, 0.01);
                    _currentState = FlutterAlgStates.CalculatePower;
                    _actualProgress = 97.0;
                    break;

                case FlutterAlgStates.CalculatePower:
                    _powerList = _flutter.CalculateIntegralForEachSpectrum(_frequenciesList, _spectralDensityList);
                    _currentState = FlutterAlgStates.DetectAFL;
                    _actualProgress = 98.0;
                    break;

                case FlutterAlgStates.DetectAFL:
                    _aflAnnotations = _flutter.Detect(_spectralDensityList, _frequenciesList, _powerList);
                    _currentState = FlutterAlgStates.Finished;
                    _actualProgress = 99.0;
                    break;

                case FlutterAlgStates.Finished:
                    _actualProgress = 100.0;
                    foreach(var channel in _channels)
                    {
                        OutputWorker.SaveFlutterAnnotations(channel, true, _aflAnnotations);
                    }
                    
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

            string fileName = "../../Sygnaly_Oznaczacz/iaf7_afw1-result.txt";
            Stream fileStream = File.OpenWrite(fileName);
            StreamWriter writer = new StreamWriter(fileStream);
            foreach (var annotation in fl._aflAnnotations)
            {
                string line = string.Format("{0},{1}", annotation.Item1, annotation.Item2);
                writer.WriteLine(line);
            }
            writer.Close();
            fileStream.Close();

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
