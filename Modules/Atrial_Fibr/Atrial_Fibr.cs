using System;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.R_Peaks;
using MathNet.Numerics;
using System.Collections.Generic;
using System.Globalization;

namespace EKG_Project.Modules.Atrial_Fibr
{
    public class Atrial_Fibr : IModule
    {
        private enum STATE { INIT, BEGIN_CHANNEL, PROCESS_FIRST_STEP, PROCESS_CHANNEL, NEXT_CHANNEL, END_CHANNEL, END };
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private string _currentLeadName;
        private string[] _leads;
        private int _currentIndex;
        private int _numberOfChannels;
        private uint _fs;

        private R_Peaks_New_Data_Worker _inputRpeaksWorker;
        private Atrial_Fibr_New_Data_Worker _outputWorker;
        private Basic_New_Data_Worker _inputWorker_basic;

        private Atrial_Fibr_Params _params;

        private Atrial_Fibr_Alg _object;
        private Vector<Double> _vectorOfRpeaks;
        private Vector<Double> _vectorOfIntervals;
        private Tuple<bool, Vector<double>, double> _result;
        private bool _detectedAF;
        private double _lengthOfData;
        private double _lengthOfDetection;
        private double _percentOfDetection;
        private string _detection;
        private string _description;
        private bool _append;
        private STATE _state;
        private int _step = 480;

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
                _params = parameters as Atrial_Fibr_Params;
            }
            catch (Exception e)
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
                InputWorker_basic = new Basic_New_Data_Worker(Params.AnalysisName);
                InputRpeaksWorker = new R_Peaks_New_Data_Worker(Params.AnalysisName);
                OutputWorker = new Atrial_Fibr_New_Data_Worker(Params.AnalysisName);
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
            return 100.0 * ((double)_currentChannelIndex / (double)NumberOfChannels + (1.0 / NumberOfChannels) * ((double)_currentIndex / (double)_currentChannelLength));
        }

        public bool Runnable()
        {
            return Params != null;
        }

        private void processData()
        {
            switch (_state)
            {
                case (STATE.INIT):
                   _currentChannelIndex = -1;
                   _leads = InputWorker_basic.LoadLeads().ToArray();
                   _numberOfChannels = _leads.Length;
                    _fs = InputWorker_basic.LoadAttribute(Basic_Attributes.Frequency);
                   _state = STATE.BEGIN_CHANNEL;
                   break;
                case (STATE.BEGIN_CHANNEL):
                    _currentChannelIndex++;
                    if (_currentChannelIndex >= _numberOfChannels) _state = STATE.END;
                    else
                    {
                        _currentLeadName = _leads[_currentChannelIndex];
                        _currentChannelLength = (int)InputRpeaksWorker.getNumberOfSamples(R_Peaks_Attributes.RRInterval, _currentLeadName);
                        _currentIndex = 0;
                        _detectedAF = false;
                        _lengthOfDetection = 0;
                        _percentOfDetection = 0;
                        _append = true;
                        _lengthOfData = (InputRpeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, _currentChannelLength-1, 1).At(0)-InputRpeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName,0,1).At(0))/_fs;
                        _state = STATE.PROCESS_FIRST_STEP;
                    }
                    break;
                case (STATE.PROCESS_FIRST_STEP):
                    if (_currentIndex + _step >= _currentChannelLength)
                    {
                        _state = STATE.END_CHANNEL;
                        _append = false;
                    }
                    else
                    {
                        try
                        {
                            _vectorOfRpeaks = InputRpeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, _currentIndex, _step);
                            _vectorOfIntervals = InputRpeaksWorker.LoadSignal(R_Peaks_Attributes.RRInterval, _currentLeadName, _currentIndex, _step).Multiply((double)_fs / 1000);
                            _object = new Atrial_Fibr_Alg();
                            _result = _object.detectAF(_vectorOfIntervals, _vectorOfRpeaks, _fs, Params);
                            if (_result.Item1)
                            {
                                OutputWorker.SaveAfDetection(_currentLeadName, false, false, new Tuple<bool, Vector<double>, string, string>(false, _result.Item2, "", ""));
                                _detectedAF = true;
                                _lengthOfDetection += _result.Item3;
                            }
                            _currentIndex += _step;
                            _state = STATE.PROCESS_CHANNEL;
                        }
                        catch (Exception e)
                        {
                            _state = STATE.NEXT_CHANNEL;
                        }
                    }
                    break;
                case (STATE.PROCESS_CHANNEL):
                    if (_currentIndex + _step >= _currentChannelLength) _state = STATE.END_CHANNEL;
                    else
                    {
                        try
                        {
                            _vectorOfRpeaks = InputRpeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, _currentIndex, _step);
                            _vectorOfIntervals = InputRpeaksWorker.LoadSignal(R_Peaks_Attributes.RRInterval, _currentLeadName, _currentIndex, _step).Multiply((double)_fs / 1000);
                            _object = new Atrial_Fibr_Alg();
                            _result = _object.detectAF(_vectorOfIntervals, _vectorOfRpeaks, _fs, Params);
                            if (_result.Item1)
                            {
                                OutputWorker.SaveAfDetection(_currentLeadName, true, false, new Tuple<bool, Vector<double>, string, string>(false, _result.Item2, "", ""));
                                _detectedAF = true;
                                _lengthOfDetection += _result.Item3;
                            }
                            _currentIndex += _step;
                            _state = STATE.PROCESS_CHANNEL;
                        }
                        catch (Exception e)
                        {
                            _state = STATE.NEXT_CHANNEL;
                        }
                    }
                    break;
                case (STATE.END_CHANNEL):
                    try
                    {
                        _vectorOfRpeaks = InputRpeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, _currentIndex, _currentChannelLength - _currentIndex);
                        _vectorOfIntervals = InputRpeaksWorker.LoadSignal(R_Peaks_Attributes.RRInterval, _currentLeadName, _currentIndex, _currentChannelLength - _currentIndex).Multiply((double)_fs / 1000);
                        _object = new Atrial_Fibr_Alg();
                        _result = _object.detectAF(_vectorOfIntervals, _vectorOfRpeaks, _fs, Params);
                        if (_result.Item1)
                        {
                            _detectedAF = true;
                            _lengthOfDetection += _result.Item3;
                        }
                        if (_detectedAF)
                        {
                            _percentOfDetection =( _lengthOfDetection / _lengthOfData)*100;
                            _detection = "Wykryto migotanie przedsionków.";
                            _description="Wykryto migotanie trwające " + _lengthOfDetection.ToString("F1") + "s. Stanowi to " +_percentOfDetection.ToString("F1") + "% trwania sygnału.";
                            OutputWorker.SaveAfDetection(_currentLeadName, _append, true, new Tuple<bool, Vector<double>, string, string>(_detectedAF, _result.Item2, _detection, _description));
                        }
                        else
                        { 
                            _detection = "Nie wykryto migotania przedsionków.";
                            OutputWorker.SaveAfDetection(_currentLeadName, _append, true, new Tuple<bool, Vector<double>, string, string>(_detectedAF, _result.Item2, _detection,""));
                        }

                        _state = STATE.NEXT_CHANNEL;
                    }
                    catch (Exception e)
                    {
                        _state = STATE.NEXT_CHANNEL;
                    }
                    break;
                case (STATE.NEXT_CHANNEL):
                    _state = STATE.BEGIN_CHANNEL;
                    break;
                case (STATE.END):
                    _ended = true;
                    break;
                default:
                    Abort();
                    break;
            }
        }


        public bool Aborted
        {
            get { return _aborted; }
            set { _aborted = value; }
        }

        public Atrial_Fibr_Params Params
        {
            get { return _params; }
            set { _params = value; }
        }

        Atrial_Fibr_New_Data_Worker OutputWorker
        {
            get { return _outputWorker; }
            set { _outputWorker = value; }
        }

        public int NumberOfChannels
        {
            get { return _numberOfChannels; }
            set { _numberOfChannels = value; }
        }

        public Basic_New_Data_Worker InputWorker_basic
        {
            get { return _inputWorker_basic; }
            set { _inputWorker_basic = value; }
        }

        public R_Peaks_New_Data_Worker InputRpeaksWorker
        {
            get { return _inputRpeaksWorker; }
            set { _inputRpeaksWorker = value; }
        }

        public static void Main()
        {
            Atrial_Fibr_Params param = new Atrial_Fibr_Params(Detect_Method.STATISTIC, "analiza1");
            Atrial_Fibr testModule = new Atrial_Fibr();
            testModule.Init(param);
            while (!testModule.Ended())
            {
                testModule.ProcessData();
                Console.WriteLine(testModule.Progress());
            }
            Console.WriteLine("Analiza zakonczona. Press key to continue.");
            Console.Read();
        }
    }
}