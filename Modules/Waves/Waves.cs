using System;
using EKG_Project.IO;
using EKG_Project.Modules.R_Peaks;
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
        private R_Peaks_Data_Worker _inputRpeaksWorker;
        private Waves_Data_Worker _outputWorker;

        private Waves_Data _outputData;
        private Basic_Data _inputData;
        private R_Peaks_Data _inputRpeaksData;
        
        private Waves_Params _params;

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

        public bool Ended()
        {
            return _ended;
        }

        public void Init(ModuleParams parameters)
        {
            Params = parameters as Waves_Params;
            Aborted = false;
            if (!Runnable()) _ended = true;
            else
            {
                _ended = false;

                InputWorker = new Basic_Data_Worker(Params.AnalysisName);
                InputWorkerRpeaks = new R_Peaks_Data_Worker(Params.AnalysisName);
                InputWorker.Load();
                InputData = InputWorker.BasicData;
                InputDataRpeaks = InputWorkerRpeaks.Data;

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

        public void InitForTestsOnly( Vector<double> ecg, Vector<double> rPeaks, Waves_Params parameters)
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
                InputDataRpeaks.RPeaks.Add(new Tuple < string, Vector<double>>("dupa", rPeaks));

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
            return Params != null;
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
                    analyzeSignalPart( );
                    _rPeaksProcessed = startIndex + step;
                    Console.WriteLine("Jedna sesja poszla!");
                    Console.WriteLine(_rPeaksProcessed);
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
            Waves_Params param = new Waves_Params( Wavelet_Type.haar, 2, "Analysis6", 10);

            TempInput.setInputFilePath(@"C:\Users\Michał\Documents\biomed\II stopien\dadm\lab2\EKG.txt");
            TempInput.setOutputFilePath(@"C:\Users\Michał\Documents\biomed\II stopien\dadm\lab2\EKGQRSonsets3.txt");
            Vector<double> ecg = TempInput.getSignal();

            TempInput.setInputFilePath(@"C:\Users\Michał\Documents\biomed\II stopien\dadm\lab2\EKG3Rpeaks.txt");
            Vector<double> rpeaks = TempInput.getSignal();

            Waves testModule = new Waves();
            testModule.InitForTestsOnly(ecg, rpeaks, param);
            while (true)
            {
                //Console.WriteLine("Press key to continue.");
                //Console.Read();
                if (testModule.Ended()) break;
                Console.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }

            Vector<double> onsets = Vector<double>.Build.Dense(testModule.OutputData.QRSOnsets[0].Item2.Count);
            Console.WriteLine("fajrant");
            for (int i = 0; i < onsets.Count; i++)
            {
                onsets[i] = (double)testModule.OutputData.QRSOnsets[0].Item2[i];

            }

            TempInput.writeFile(360, onsets);

            //POKI CO BIERZEMY DANE Z NASZYCH GOWNIANYCH PLIKOW


            //
            //TempInput.setInputFilePath(@"C:\Users\Phantom\Desktop\DADM Project\Nowy folder\EKG.txt");
            //TempInput.setOutputFilePath(@"C:\Users\Phantom\Desktop\DADM Project\Nowy folder\EKGQRSonsets.txt");
            //uint fs = TempInput.getFrequency();
            //Vector<double> ecg = TempInput.getSignal();
            ////Vector<double> dwt = ListDWT(_ecg, 3, Wavelet_Type.db2)[1];
            //Vector<double> temp = Vector<double>.Build.Dense(2);

            //
            ////TempInput.setInputFilePath(@"C:\Users\Phantom\Desktop\DADM Project\Nowy folder\EKG3Rpeaks.txt");

            //List<int> Rpeaks = new List<int>();
            //Vector<double> rpeaks = TempInput.getSignal();
            //foreach (double singlePeak in rpeaks)
            //{
            //    Rpeaks.Add((int)singlePeak);
            //}


            //Waves_Params param = new Waves_Params(Wavelet_Type.haar , 2 , "Analysis6");
            //Waves_Data data = new Waves_Data(ecg, Rpeaks, fs);


            //Waves testModule = new Waves();

            //testModule.Init(param, data);
            //testModule.ProcessData();
            //data = testModule.Data;

            //Vector<double> onsets = Vector<double>.Build.Dense(data.QRSOnsets.Count);
            //for (int i = 0; i < data.QRSOnsets.Count; i++)
            //{
            //    //onsets[i] = (double)data.QRSOnsets[i];

            //}

            //TempInput.writeFile(360, onsets);
            //Vector<double> ends = Vector<double>.Build.Dense(_QRSends.Count);
            //for (int i = 0; i < _QRSends.Count; i++)
            //{
            //    ends[i] = (double)_QRSends[i];

            //}
            //FindP();
            //Vector<double> ponset = Vector<double>.Build.Dense(_Ponsets.Count);
            //for (int i = 0; i < _Ponsets.Count; i++)
            //{
            //    ponset[i] = (double)_Ponsets[i];

            //}
            //Vector<double> pends = Vector<double>.Build.Dense(_Pends.Count);
            //for (int i = 0; i < _Pends.Count; i++)
            //{
            //    pends[i] = (double)_Pends[i];

            //}
            //FindT();
            //Vector<double> tends = Vector<double>.Build.Dense(_Tends.Count);
            //for (int i = 0; i < _Tends.Count; i++)
            //{
            //    tends[i] = (double)_Tends[i];

            //}

            //TempInput.writeFile(360, onsets);
            //TempInput.setOutputFilePath(@"C:\Users\Phantom\Desktop\DADM Project\Nowy folder\EKGQRSends.txt");
            //TempInput.writeFile(360, ends);
            //TempInput.setOutputFilePath(@"C:\Users\Phantom\Desktop\DADM Project\Nowy folder\EKGPonsets.txt");
            //TempInput.writeFile(360, ponset);
            //TempInput.setOutputFilePath(@"C:\Users\Phantom\Desktop\DADM Project\Nowy folder\EKGPends.txt");
            //TempInput.writeFile(360, pends);
            //TempInput.setOutputFilePath(@"C:\Users\Phantom\Desktop\DADM Project\Nowy folder\EKGTends.txt");
            //TempInput.writeFile(360, tends);
            //TempInput.setOutputFilePath(@"C:\Users\Michał\Documents\biomed\II stopien\dadm\lab2\d2ekg.txt");
            //TempInput.setOutputFilePath(@"C:\Users\Phantom\Desktop\DADM Project\Nowy folder\d2ekg.txt");
            //TempInput.writeFile(360, dwt);
            Console.Read();
        }

    }
     
}
