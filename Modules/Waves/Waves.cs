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
        private enum STATE { INIT, BEGIN_CHANNEL, PROCESS_FIRST_STEP, PROCESS_CHANNEL, END_CHANNEL };
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentRpeaksLength;
        private string _currentLeadName;
        private string[] _leads;
        private int _rPeaksProcessed;
        private int _numberOfChannels;

        private Basic_New_Data_Worker _inputWorker;
        private ECG_Baseline_New_Data_Worker _inputECGWorker;
        private R_Peaks_New_Data_Worker _inputRpeaksWorker;
        private Waves_New_Data_Worker _outputWorker;

        //private Waves_Data _outputData;
        //private Basic_Data _inputData;
        //private ECG_Baseline_Data _inputECGData;
        //private R_Peaks_Data _inputRpeaksData;

        private Waves_Params _params;
        private Waves_Alg _alg;

        private int _currentStep;
        private List<int> _currentQRSonsetsPart;
        private List<int> _currentQRSendsPart;
        private List<int> _currentPonsetsPart;
        private List<int> _currentPendsPart;
        private List<int> _currentTendsPart;

        private Vector<double> _currentECG;
        private Vector<double> _currentRpeaks;

        private int _offset;

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
                Params = parameters as Waves_Params;
            }
            catch (Exception e)
            {
                Abort();
                return;
            }

            
            if (!Runnable())
                _ended = true;
            else
            {
                _ended = false;

                InputWorker = new Basic_New_Data_Worker(Params.AnalysisName);
                InputECGworker = new ECG_Baseline_New_Data_Worker(Params.AnalysisName);
                InputWorkerRpeaks = new R_Peaks_New_Data_Worker(Params.AnalysisName);

                //InputData = new Basic_Data();
                //InputECGData = new ECG_Baseline_Data();
                //InputDataRpeaks = new R_Peaks_Data();
                
                _currentStep = _params.RpeaksStep;

                OutputWorker = new Waves_New_Data_Worker(Params.AnalysisName);
                OutputWorker.DeleteFiles();
                //OutputData = new Waves_Data();

                _alg = new Waves_Alg(_params);

                _currentChannelIndex = 0;
                _rPeaksProcessed = 0;

              
                _currentQRSonsetsPart = new List<int>();
                _currentQRSendsPart = new List<int>();
                _currentPonsetsPart = new List<int>();
                _currentPendsPart = new List<int>();
                _currentTendsPart = new List<int>();

                _offset = 0;
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
            return 100.0 * ( ((double)_rPeaksProcessed / (double)_currentRpeaksLength));
        }

        public bool Runnable()
        {
            return Params != null && Params.DecompositionLevel > 0;
        }

        //private void processDataOld()
        //{
        //    int channel = _currentChannelIndex;
        //    int startIndex = _rPeaksProcessed;
        //    int step = Params.RpeaksStep;

        //    if (channel < NumberOfChannels)
        //    {
        //        if (startIndex + step >= _currentRpeaksLength)
        //        {
        //            cutSignals();
        //            _alg.analyzeSignalPart(_currentECG, _currentRpeaks,
        //                _currentQRSonsetsPart, _currentQRSendsPart,
        //                _currentPonsetsPart, _currentPendsPart, _currentTendsPart, _offset, InputData.Frequency);

        //            _currentQRSonsets.AddRange(_currentQRSonsetsPart);
        //            _currentQRSends.AddRange(_currentQRSendsPart);
        //            _currentPonsets.AddRange(_currentPonsetsPart);
        //            _currentPends.AddRange(_currentPendsPart);
        //            _currentTends.AddRange(_currentTendsPart);

        //            OutputData.QRSOnsets.Add(new Tuple<string, List<int>>(InputData.Signals[_currentChannelIndex].Item1, _currentQRSonsets));
        //            OutputData.QRSEnds.Add(new Tuple<string, List<int>>(InputData.Signals[_currentChannelIndex].Item1, _currentQRSends));

        //            OutputData.POnsets.Add(new Tuple<string, List<int>>(InputData.Signals[_currentChannelIndex].Item1, _currentPonsets));
        //            OutputData.PEnds.Add(new Tuple<string, List<int>>(InputData.Signals[_currentChannelIndex].Item1, _currentPends));

        //            OutputData.TEnds.Add(new Tuple<string, List<int>>(InputData.Signals[_currentChannelIndex].Item1, _currentTends));

        //            _currentChannelIndex++;
        //            if (_currentChannelIndex < NumberOfChannels)
        //            {
        //                _rPeaksProcessed = 0;

        //                _currentRpeaksLength = InputDataRpeaks.RPeaks[_currentChannelIndex].Item2.Count;

        //                _currentQRSonsets = new List<int>();
        //                _currentQRSends = new List<int>();
        //                _currentPonsets = new List<int>();
        //                _currentPends = new List<int>();
        //                _currentTends = new List<int>();
        //            }


        //        }
        //        else
        //        {
        //            cutSignals();
        //            _alg.analyzeSignalPart(_currentECG, _currentRpeaks,
        //                _currentQRSonsetsPart, _currentQRSendsPart,
        //                _currentPonsetsPart, _currentPendsPart, _currentTendsPart, _offset, InputData.Frequency);

        //            _currentQRSonsets.AddRange(_currentQRSonsetsPart);
        //            _currentQRSends.AddRange(_currentQRSendsPart);
        //            _currentPonsets.AddRange(_currentPonsetsPart);
        //            _currentPends.AddRange(_currentPendsPart);
        //            _currentTends.AddRange(_currentTendsPart);

        //            _rPeaksProcessed += step;
        //        }
        //    }
        //    else
        //    {
        //        //OutputWorker.Save(OutputData);
        //        _ended = true;
        //    }

        //}
        private void processData()
        {
            switch (_state)
            {
                case (STATE.INIT):
                    _currentChannelIndex = -1;
                    _leads = InputWorker.LoadLeads().ToArray();
                    _numberOfChannels = _leads.Length;
                    _state = STATE.BEGIN_CHANNEL;
                    //_inputWorker.DeleteFiles(); Do not use yet - will try to handle this during loading.
                    break;
                case (STATE.BEGIN_CHANNEL):
                    _currentChannelIndex++;
                    if (_currentChannelIndex >= _numberOfChannels) _state = STATE.END_CHANNEL;
                    else
                    {
                        setChannel();          

                        _currentRpeaksLength = (int)InputWorkerRpeaks.getNumberOfSamples(R_Peaks_Attributes.RPeaks, _currentLeadName);

                        _rPeaksProcessed = 0;
                        _state = STATE.PROCESS_FIRST_STEP;
                    }

                    break;
                case (STATE.PROCESS_FIRST_STEP):
                    if (_rPeaksProcessed + Params.RpeaksStep > _currentRpeaksLength) _state = STATE.END_CHANNEL;
                    else
                    {
                        try
                        {
                            ProcesPart(false);
                            _state = STATE.PROCESS_CHANNEL;
                        }
                        catch (Exception e)
                        {
                            _state = STATE.END_CHANNEL;
                        }

                    }
                    break;
                case (STATE.PROCESS_CHANNEL): // this state can be divided to load state, process state and save state, good decision especially for ECG_Baseline, R_Peaks, Waves and Heart_Class
                    if (_rPeaksProcessed + Params.RpeaksStep > _currentRpeaksLength) _state = STATE.END_CHANNEL;
                    else
                    {
                        try
                        {
                            ProcesPart(true);
                            _state = STATE.PROCESS_CHANNEL;
                        }
                        catch (Exception e)
                        {
                            _state = STATE.END_CHANNEL;
                        }
                    }


                    break;
                case (STATE.END_CHANNEL):
                    try
                    {
                        ProcesPart(true);
                        _ended = true;
                    }
                            catch (Exception e)
                    {
                        _ended = true;
                    }
            Console.WriteLine("end ");
                    break;
                default:
                    Abort();
                    break;
            }

        }

        void setChannel()
        {
            for(int i=0; i< _numberOfChannels; i++)
            {
                if (_leads[i] == "MLII")
                {
                    _currentLeadName = "MLII";
                    _currentChannelIndex = i;
                    return;
                }
            }
            _currentLeadName = _leads[0];
            _currentChannelIndex = 0;
        }

        private void ProcesPart( bool isNotEmpty)
        {

            cutSignals();
            _alg.analyzeSignalPart(_currentECG, _currentRpeaks,
                _currentQRSonsetsPart, _currentQRSendsPart,
                _currentPonsetsPart, _currentPendsPart, _currentTendsPart, _offset, InputWorker.LoadAttribute(Basic_Attributes.Frequency));

            SaveAll(isNotEmpty);

            Console.WriteLine(Params.RpeaksStep);

            _rPeaksProcessed += Params.RpeaksStep;

        }

        private void SaveAll( bool isNotEmpty)
        {
            for(int i=0; i< _numberOfChannels; i++)
            {
                string name = _leads[i];
                OutputWorker.SaveSignal(Waves_Signal.QRSOnsets, name, isNotEmpty, _currentQRSonsetsPart);
                OutputWorker.SaveSignal(Waves_Signal.QRSEnds, name, isNotEmpty, _currentQRSendsPart);

                OutputWorker.SaveSignal(Waves_Signal.POnsets, name, isNotEmpty, _currentPonsetsPart);
                OutputWorker.SaveSignal(Waves_Signal.PEnds, name, isNotEmpty, _currentPendsPart);

                OutputWorker.SaveSignal(Waves_Signal.TEnds, name, isNotEmpty, _currentTendsPart);
            }
        }

        private void cutSignals()
        {
            int signalStart = 0;
            if (_rPeaksProcessed > 1)
            {
                Vector<double> signalBegin = InputWorkerRpeaks.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, _rPeaksProcessed - 1, 1);
                signalStart = (int)signalBegin[0];
            }
                

            int signalEnd = 0;
            int rPeaks2cut = Params.RpeaksStep;

            if (_rPeaksProcessed + Params.RpeaksStep + 2 > _currentRpeaksLength)
                signalEnd = (int) (InputWorker.getNumberOfSamples(_currentLeadName) - 1);
            else
            {
                Vector<double> RpeaksEnds = InputWorkerRpeaks.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, _rPeaksProcessed + Params.RpeaksStep + 1, 1);
                int lastRpeak = (int)RpeaksEnds[0];
                    
                signalEnd = lastRpeak;
            }

            if (_rPeaksProcessed + Params.RpeaksStep > _currentRpeaksLength)
                rPeaks2cut = _currentRpeaksLength - _rPeaksProcessed;

            _currentECG = InputECGworker.LoadSignal(_currentLeadName, signalStart, signalEnd - signalStart);

            _currentRpeaks = InputWorkerRpeaks.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, _rPeaksProcessed, rPeaks2cut);
            _currentRpeaks = _currentRpeaks.Subtract(signalStart);

            _offset = signalStart;
        }
        //public Basic_Data InputData
        //{
        //    get
        //    {
        //        return _inputData;
        //    }

        //    set
        //    {
        //        _inputData = value;
        //    }
        //}

        //public ECG_Baseline_Data InputECGData
        //{
        //    get
        //    {
        //        return _inputECGData;
        //    }

        //    set
        //    {
        //        _inputECGData = value;
        //    }
        //}

        //public R_Peaks_Data InputDataRpeaks
        //{
        //    get
        //    {
        //        return _inputRpeaksData;
        //    }

        //    set
        //    {
        //        _inputRpeaksData = value;
        //    }
        //}


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

        //public Waves_Data OutputData
        //{
        //    get
        //    {
        //        return _outputData;
        //    }
        //    set
        //    {
        //        _outputData = value;
        //    }
        //}

        public Basic_New_Data_Worker InputWorker
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

        public ECG_Baseline_New_Data_Worker InputECGworker
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

        public R_Peaks_New_Data_Worker InputWorkerRpeaks
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

        public Waves_New_Data_Worker OutputWorker
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
            Waves_Params param = new Waves_Params(Wavelet_Type.haar, 2, "Analysis 1", 500);
            IModule testModule = new Waves();


            testModule.Init(param);
            while (!testModule.Ended())
            {
                testModule.ProcessData();
                Console.WriteLine(testModule.Progress());
            }
            Console.WriteLine("fajrant");
            Console.Read();
        }

    }

}
