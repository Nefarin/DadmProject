using System;
using EKG_Project.IO;
using EKG_Project.Modules.R_Peaks;
using EKG_Project.Modules.ECG_Baseline;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using System.Collections.Generic;

namespace EKG_Project.Modules.Waves
{

    public partial class Waves : IModule
    {
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentRpeaksLength;
        private int _rPeaksProcessed;
        private int _numberOfChannels;

        private Basic_Data_Worker _inputWorker;
        private ECG_Baseline_Data_Worker _inputECGWorker;
        private R_Peaks_Data_Worker _inputRpeaksWorker;
        private Waves_Data_Worker _outputWorker;

        private Waves_Data _outputData;
        private Basic_Data _inputData;
        private ECG_Baseline_Data _inputECGData;
        private R_Peaks_Data _inputRpeaksData;

        private Waves_Params _params;

        private double _qrsOnsTresh;
        private double _qrsEndTresh;
        private int _currentStep;
        private List<int> _currentQRSonsetsPart;
        private List<int> _currentQRSendsPart;
        private List<int> _currentPonsetsPart;
        private List<int> _currentPendsPart;
        private List<int> _currentTendsPart;

        private List<int> _currentQRSonsets;
        private List<int> _currentQRSends;
        private List<int> _currentPonsets;
        private List<int> _currentPends;
        private List<int> _currentTends;

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
            Params = parameters as Waves_Params;
            if (Params.DecompositionLevel > 0)
                Aborted = false;
            else
                Aborted = true;
            if (!Runnable()) _ended = true;
            else
            {
                _ended = false;

                InputWorker = new Basic_Data_Worker(Params.AnalysisName);
                InputECGworker = new ECG_Baseline_Data_Worker(Params.AnalysisName);
                InputWorkerRpeaks = new R_Peaks_Data_Worker(Params.AnalysisName);

                InputWorker.Load();
                InputData = InputWorker.BasicData;

                InputECGworker.Load();
                InputECGData = InputECGworker.Data;

                InputWorkerRpeaks.Load();
                InputDataRpeaks = InputWorkerRpeaks.Data;
                //Console.Write(InputDataRpeaks.RPeaks[_currentChannelIndex].Item2.Count);
                //Console.Write("ilosc kanalow ECG ");
                //Console.WriteLine(InputECGData.SignalsFiltered.Count);
                //Console.WriteLine("Ilosc kanalow Rpeaks");
                //Console.WriteLine(InputDataRpeaks.RPeaks.Count);
                _currentStep = _params.RpeaksStep;

                OutputWorker = new Waves_Data_Worker(Params.AnalysisName);
                OutputData = new Waves_Data();

                _currentChannelIndex = 0;
                _rPeaksProcessed = 0;
                //NumberOfChannels = InputData.Signals.Count;
                //najwyrazniej liczba tych kanalow nie byla rowna w trakcie pierwszych testow
                NumberOfChannels = InputDataRpeaks.RPeaks.Count;
                _params.RpeaksStep = _inputRpeaksData.RPeaks[_currentChannelIndex].Item2.Count + 100;
                _currentRpeaksLength = InputDataRpeaks.RPeaks[_currentChannelIndex].Item2.Count;
                _currentQRSonsetsPart = new List<int>();
                _currentQRSendsPart = new List<int>();
                _currentPonsetsPart = new List<int>();
                _currentPendsPart = new List<int>();
                _currentTendsPart = new List<int>();

                _currentQRSonsets = new List<int>();
                _currentQRSends = new List<int>();
                _currentPonsets = new List<int>();
                _currentPends = new List<int>();
                _currentTends = new List<int>();

            }

        }

        public void InitForTestsOnly(Vector<double> ecg, Vector<double> rPeaks, Waves_Params parameters)
        {
            Params = parameters as Waves_Params;
            Aborted = false;
            if (!Runnable()) _ended = true;
            else
            {
                _ended = false;


                InputData = new Basic_Data();
                InputData.Frequency = 360;
                InputData.Signals = new List<Tuple<string, Vector<double>>>();
                InputData.Signals.Add(new Tuple<string, Vector<double>>("dupa", ecg));

                InputDataRpeaks = new R_Peaks_Data();

                InputDataRpeaks.RPeaks = new List<Tuple<string, Vector<double>>>();
                InputDataRpeaks.RPeaks.Add(new Tuple<string, Vector<double>>("dupa", rPeaks));

                OutputWorker = new Waves_Data_Worker(Params.AnalysisName);
                OutputData = new Waves_Data();

                _currentChannelIndex = 0;
                _rPeaksProcessed = 0;
                NumberOfChannels = InputData.Signals.Count;
                _currentRpeaksLength = InputDataRpeaks.RPeaks[_currentChannelIndex].Item2.Count;
                _currentQRSonsetsPart = new List<int>();
                _currentQRSendsPart = new List<int>();
                _currentPonsetsPart = new List<int>();
                _currentPendsPart = new List<int>();
                _currentTendsPart = new List<int>();

                _currentQRSonsets = new List<int>();
                _currentQRSends = new List<int>();
                _currentPonsets = new List<int>();
                _currentPends = new List<int>();
                _currentTends = new List<int>();

            }

        }

        public void ProcessData()
        {
            if (Runnable()) processData();
            else _ended = true;
        }

        public double Progress()
        {
            return 100.0 * ((double)_currentChannelIndex / (double)NumberOfChannels + (1.0 / NumberOfChannels) * ((double)_rPeaksProcessed / (double)_currentRpeaksLength));
        }

        public bool Runnable()
        {
            return Params != null && Params.DecompositionLevel > 0;
        }

        private void processData()
        {
            int channel = _currentChannelIndex;
            int startIndex = _rPeaksProcessed;
            int step = Params.RpeaksStep;

            if (channel < NumberOfChannels)
            {
                if (startIndex + step > _currentRpeaksLength)
                {
                    analyzeSignalPart();
                    OutputData.QRSOnsets.Add(new Tuple<string, List<int>>(InputData.Signals[_currentChannelIndex].Item1, _currentQRSonsets));
                    OutputData.QRSEnds.Add(new Tuple<string, List<int>>(InputData.Signals[_currentChannelIndex].Item1, _currentQRSends));

                    OutputData.POnsets.Add(new Tuple<string, List<int>>(InputData.Signals[_currentChannelIndex].Item1, _currentPonsets));
                    OutputData.PEnds.Add(new Tuple<string, List<int>>(InputData.Signals[_currentChannelIndex].Item1, _currentPends));

                    OutputData.TEnds.Add(new Tuple<string, List<int>>(InputData.Signals[_currentChannelIndex].Item1, _currentTends));

                    //Console.Write("Rpeaks: ");
                    //Console.WriteLine(InputDataRpeaks.RPeaks[_currentChannelIndex].Item2.Count);

                    //Console.Write("QRSonset: ");
                    //Console.WriteLine(_currentQRSonsets.Count);

                    //Console.Write("QRSend: ");
                    //Console.WriteLine(_currentQRSends.Count);

                    //Console.Write("Ponsets: ");
                    //Console.WriteLine(_currentPonsets.Count);

                    //Console.Write("Pends: ");
                    //Console.WriteLine(_currentPends.Count);

                    //Console.Write("Tends: ");
                    //Console.WriteLine(_currentTends.Count);

                    _currentChannelIndex++;
                    if (_currentChannelIndex < NumberOfChannels)
                    {
                        _rPeaksProcessed = 0;

                        _currentRpeaksLength = InputDataRpeaks.RPeaks[_currentChannelIndex].Item2.Count;

                        _currentQRSonsets = new List<int>();
                        _currentQRSends = new List<int>();
                        _currentPonsets = new List<int>();
                        _currentPends = new List<int>();
                        _currentTends = new List<int>();
                    }


                }
                else
                {
                    analyzeSignalPart();
                    _rPeaksProcessed = startIndex + step;
                    //Console.WriteLine("Jedna sesja poszla!");
                    //Console.WriteLine(_rPeaksProcessed);
                }
            }
            else
            {
                OutputWorker.Save(OutputData);
                _ended = true;
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

        public ECG_Baseline_Data InputECGData
        {
            get
            {
                return _inputECGData;
            }

            set
            {
                _inputECGData = value;
            }
        }

        public R_Peaks_Data InputDataRpeaks
        {
            get
            {
                return _inputRpeaksData;
            }

            set
            {
                _inputRpeaksData = value;
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

        public Waves_Params Params
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

        public Waves_Data OutputData
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

        public ECG_Baseline_Data_Worker InputECGworker
        {
            get
            {
                return _inputECGWorker;
            }
            set
            {
                _inputECGWorker = value;
            }
        }

        public R_Peaks_Data_Worker InputWorkerRpeaks
        {
            get
            {
                return _inputRpeaksWorker;
            }
            set
            {
                _inputRpeaksWorker = value;
            }
        }

        public Waves_Data_Worker OutputWorker
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
            Waves_Params param = new Waves_Params(Wavelet_Type.haar, 2, "TestAnalysis100", 500);

            //TempInput.setInputFilePath(@"C:\Users\Michał\Documents\biomed\II stopien\dadm\lab2\EKG.txt");
            //TempInput.setOutputFilePath(@"C:\Users\Michał\Documents\biomed\II stopien\dadm\lab2\EKGQRSonsets3.txt");
            //Vector<double> ecg = TempInput.getSignal();

            //TempInput.setInputFilePath(@"C:\Users\Michał\Documents\biomed\II stopien\dadm\lab2\EKG3Rpeaks.txt");
            //Vector<double> rpeaks = TempInput.getSignal();

            Waves testModule = new Waves();
            //testModule.InitForTestsOnly(ecg, rpeaks, param);
            testModule.Init(param);
            while (true)
            {
                //Console.WriteLine("Press key to continue.");
                //Console.Read();
                if (testModule.Ended()) break;
                Console.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }
            Console.Read();
            
        }

    }

}
