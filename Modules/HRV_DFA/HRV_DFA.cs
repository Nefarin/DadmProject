using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using EKG_Project.Modules.R_Peaks;
using EKG_Project.Modules;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;

namespace EKG_Project.Modules.HRV_DFA
{
    
    public class HRV_DFA : IModule
    {
        private enum STATE { INIT, BEGIN_CHANNEL, PROCESS_FIRST_STEP, PROCESS_CHANNEL, NEXT_CHANNEL, END_CHANNEL, END };
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentRpeaksLength;
        private int _rPeaksProcessed;
        private int _numberOfChannels;
        private string _currentLeadName;
        private string[] _leads;
        private int _currentIndex;
        private int _currentChannelLength;
        

        private Basic_New_Data_Worker _inputBasicWorker;
        private Basic_New_Data_Worker _outputBasicWorker;

        private Basic_New_Data _outputBasicData;
        private Basic_New_Data _inputBasicData;

        private R_Peaks_New_Data_Worker _inputWorker;
        private R_Peaks_Data _inputData;

        private HRV_DFA_New_Data_Worker _outputWorker;
        private HRV_DFA_Data _outputData;

        private HRV_DFA_Params _params;
        private HRV_DFA_Alg _alg;
        private Vector<Double> _currentVector;
        private STATE _state;

        private Tuple< Vector<double>, Vector<double>> _currentnumberN;
        private Tuple< Vector<double>, Vector<double>> _currentfnValue;
        private Tuple< Vector<double>, Vector<double>> _currentparamsAlpha;
        private Tuple< Vector<double>, Vector<double>> _currentflucts;

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
                _params = parameters as HRV_DFA_Params;
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
                InputWorker = new R_Peaks_New_Data_Worker(Params.AnalysisName);
                OutputWorker = new HRV_DFA_New_Data_Worker(Params.AnalysisName + "temp"); // tutaj generalnie uzywacie swojego workera jako wyjsciowy, dla uproszczenia uzywam gotowego o innej nazwie
                InputData = new R_Peaks_Data();
                OutputData = new HRV_DFA_Data();
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
            return 100.0 * ((double)_currentChannelIndex / (double)NumberOfChannels + (1.0 / NumberOfChannels) * ((double)_rPeaksProcessed / (double)_currentRpeaksLength));
        }

        public bool Runnable()
        {
            return Params != null;
        }

        private void processData()
        {
            int step = 10000;
            switch (_state)
            {
                case (STATE.INIT):
                    _currentChannelIndex = -1;
                    _leads = InputBasicWorker.LoadLeads().ToArray();
                    _numberOfChannels = _leads.Length;
                    _state = STATE.BEGIN_CHANNEL;
                    //_inputWorker.DeleteFiles(); Do not use yet - will try to handle this during loading.
                    break;
                case (STATE.BEGIN_CHANNEL):
                    _currentChannelIndex++;
                    if (_currentChannelIndex >= _numberOfChannels) _state = STATE.END;
                    else
                    {
                        _currentLeadName = _leads[_currentChannelIndex];
                        CurrentChannelLength = (int)InputWorker.getNumberOfSamples(R_Peaks_Attributes.RRInterval, _currentLeadName);
                        _currentIndex = 0;
                        _state = STATE.PROCESS_FIRST_STEP;
                    }
                    break;
                case (STATE.PROCESS_FIRST_STEP):
                    if (_currentIndex + step > CurrentChannelLength) _state = STATE.END_CHANNEL;
                    else
                    {
                        try
                        {
                            _currentVector = InputWorker.LoadSignal(R_Peaks_Attributes.RRInterval , _currentLeadName, _currentIndex, _currentChannelLength);
                            _alg = new HRV_DFA_Alg(_currentVector);
                            _currentnumberN = _alg.ResultN;
                            _currentfnValue = _alg.ResultFn;
                            _currentparamsAlpha = _alg.ResultAlpha;
                            _currentflucts = _alg.ResultFluctuations;
                            OutputWorker.SaveSignal(HRV_DFA_Signals.DfaNumberN,_currentLeadName, false, _currentnumberN);
                            OutputWorker.SaveSignal(HRV_DFA_Signals.DfaNumberN, _currentLeadName, false, _currentfnValue);
                            OutputWorker.SaveSignal(HRV_DFA_Signals.Fluctuations, _currentLeadName, false, _currentflucts);
                            OutputWorker.SaveSignal(HRV_DFA_Signals.ParamAlpha, _currentLeadName, false, _currentparamsAlpha);
                            _currentIndex += step;
                            _state = STATE.PROCESS_CHANNEL;
                        }
                        catch (Exception e)
                        {
                            _state = STATE.NEXT_CHANNEL;
                        }
                    }
                    break;
                case (STATE.PROCESS_CHANNEL): // this state can be divided to load state, process state and save state, good decision especially for ECG_Baseline, R_Peaks, Waves and Heart_Class
                    if (_currentIndex + step > CurrentChannelLength) _state = STATE.END_CHANNEL;
                    else
                    {
                        try
                        {
                            _currentVector = InputWorker.LoadSignal(R_Peaks_Attributes.RRInterval, _currentLeadName, _currentIndex, step);
                            _alg = new HRV_DFA_Alg(_currentVector); // its possible to create just one instance somewhere in the Init - your choice
                            _currentnumberN = _alg.ResultN;
                            _currentfnValue = _alg.ResultFn;
                            _currentparamsAlpha = _alg.ResultAlpha;
                            _currentflucts = _alg.ResultFluctuations;
                            OutputWorker.SaveSignal(HRV_DFA_Signals.DfaNumberN, _currentLeadName, true, _currentnumberN);
                            OutputWorker.SaveSignal(HRV_DFA_Signals.DfaNumberN, _currentLeadName, true, _currentfnValue);
                            OutputWorker.SaveSignal(HRV_DFA_Signals.Fluctuations, _currentLeadName, true, _currentflucts);
                            OutputWorker.SaveSignal(HRV_DFA_Signals.ParamAlpha, _currentLeadName, true, _currentparamsAlpha);
                            _currentIndex += step;
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
                        _currentVector = InputWorker.LoadSignal(R_Peaks_Attributes.RRInterval, _currentLeadName, _currentIndex, CurrentChannelLength - _currentIndex);
                        _alg = new HRV_DFA_Alg(_currentVector); // its possible to create just one instance somewhere in the Init - your choice
                        _currentnumberN = _alg.ResultN;
                        _currentfnValue = _alg.ResultFn;
                        _currentparamsAlpha = _alg.ResultAlpha;
                        _currentflucts = _alg.ResultFluctuations;
                        OutputWorker.SaveSignal(HRV_DFA_Signals.DfaNumberN, _currentLeadName, true, _currentnumberN);
                        OutputWorker.SaveSignal(HRV_DFA_Signals.DfaNumberN, _currentLeadName, true, _currentfnValue);
                        OutputWorker.SaveSignal(HRV_DFA_Signals.Fluctuations, _currentLeadName, true, _currentflucts);
                        OutputWorker.SaveSignal(HRV_DFA_Signals.ParamAlpha, _currentLeadName, true, _currentparamsAlpha);
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
            //int channel = _currentChannelIndex;
            //int startIndex = _rPeaksProcessed;
            //int step = 10000;

            //if (channel < NumberOfChannels)
            //{
            //    Console.WriteLine("Len: " +_currentRpeaksLength);
            //    if (_currentRpeaksLength > 20 && _currentRpeaksLength < 1000)
            //    {
            //        this.boxVal = 100;
            //        this.startValue = 10;
            //        this.stepVal = 10;
            //        this.longCorrelations = false;
            //    }
            //    if (_currentRpeaksLength > 1000)
            //    {
            //        this.boxVal = 1000;
            //        this.startValue = 50;
            //        this.stepVal = 100;
            //        this.longCorrelations = true;
            //    }
            //    if (_currentRpeaksLength < 20)
            //    {
            //        Console.WriteLine("Number of R - Peaks is too short");
            //        _aborted = true;
            //    }

            //    if (startIndex + step > _currentRpeaksLength && _aborted != true)
            //    {

            //            HRV_DFA_Analysis(InputData.RRInterval[_currentChannelIndex].Item2, stepVal, boxVal);
            //            Tuple<string, Vector<double>, Vector<double>> numberN = new Tuple<string, Vector<double>, Vector<double>>(InputData.RRInterval[_currentChannelIndex].Item1, veclogn1, veclogn2);
            //            Tuple<string, Vector<double>, Vector<double>> fnValue = new Tuple<string, Vector<double>, Vector<double>>(InputData.RRInterval[_currentChannelIndex].Item1, veclogFn1, veclogFn2);
            //            Tuple<string, Vector<double>, Vector<double>> pAlpha = new Tuple<string, Vector<double>, Vector<double>>(InputData.RRInterval[_currentChannelIndex].Item1, vecparam1, vecparam2);
            //            OutputData.DfaNumberN.Add(numberN);
            //            OutputData.DfaValueFn.Add(fnValue);
            //            OutputData.ParamAlpha.Add(pAlpha);


            //        _currentChannelIndex++;

            //        if (_currentChannelIndex < NumberOfChannels)
            //        {
            //            _rPeaksProcessed = 0;

            //            _currentRpeaksLength = InputData.RRInterval[_currentChannelIndex].Item2.Count;
            //            _currentVector = Vector<double>.Build.Dense(_currentRpeaksLength);
            //        }
            //    }
            //    else
            //    {
            //        if (_aborted != true)
            //        {
            //            HRV_DFA_Analysis(InputData.RRInterval[_currentChannelIndex].Item2, stepVal, boxVal);
            //            _currentVector = InputData.RRInterval[_currentChannelIndex].Item2.SubVector(0, stepVal);
            //            _rPeaksProcessed = startIndex + stepVal;
            //        }
            //        else _ended = true;

            //    }
            //}
            //else
            //{
            //    Console.WriteLine("Here");
            //    OutputWorker.Save(OutputData);
            //    _ended = true;
            //}

        }

        public bool Aborted
        { get{ return _aborted; }set {_aborted = value;} }

        public int CurrentChannelIndex
        { get {return _currentChannelIndex;} set{ _currentChannelIndex = value;}}

        public int CurrentRpeaksLength
        { get{return _currentRpeaksLength;} set{ _currentRpeaksLength = value;}}

        public int RPeaksProcessed
        { get{return _rPeaksProcessed; } set { _rPeaksProcessed = value; }}

        public int NumberOfChannels
        {get{ return _numberOfChannels;}set {_numberOfChannels = value;} }

        public HRV_DFA_New_Data_Worker OutputWorker
        { get {return _outputWorker;}set { _outputWorker = value;}}

        public HRV_DFA_Data OutputData
        {get{ return _outputData;}set {_outputData = value;}}
        
        public HRV_DFA_Params Params
        {get {return _params;} set{_params = value;}}

        public R_Peaks_New_Data_Worker InputWorker
        {get {return _inputWorker; }set { _inputWorker = value;}}

        public R_Peaks_Data InputData
        { get { return _inputData; } set{ _inputData = value;} }

        public Basic_New_Data_Worker InputBasicWorker
        {
            get
            {
                return _inputBasicWorker;
            }

            set
            {
                _inputBasicWorker = value;
            }
        }

        public Basic_New_Data_Worker OutputBasicWorker
        {
            get
            {
                return _outputBasicWorker;
            }

            set
            {
                _outputBasicWorker = value;
            }
        }

        public Basic_New_Data OutputBasicData
        {
            get
            {
                return _outputBasicData;
            }

            set
            {
                _outputBasicData = value;
            }
        }

        public Basic_New_Data InputBasicData
        {
            get
            {
                return _inputBasicData;
            }

            set
            {
                _inputBasicData = value;
            }
        }

        public int CurrentChannelLength
        {
            get
            {
                return _currentChannelLength;
            }

            set
            {
                _currentChannelLength = value;
            }
        }

        public Tuple<Vector<double>, Vector<double>> CurrentnumberN
        {
            get
            {
                return _currentnumberN;
            }

            set
            {
                _currentnumberN = value;
            }
        }

        public Tuple<Vector<double>, Vector<double>> CurrentfnValue
        {
            get
            {
                return _currentfnValue;
            }

            set
            {
                _currentfnValue = value;
            }
        }

        public Tuple<Vector<double>, Vector<double>> CurrentparamsAlpha
        {
            get
            {
                return _currentparamsAlpha;
            }

            set
            {
                _currentparamsAlpha = value;
            }
        }

        public Tuple<Vector<double>, Vector<double>> Currentflucts
        {
            get
            {
                return _currentflucts;
            }

            set
            {
                _currentflucts = value;
            }
        }

        public static void Main()
        {
            HRV_DFA_Params param = new HRV_DFA_Params("Analysisnsr");
            HRV_DFA testModule = new HRV_DFA();

            testModule.Init(param);
            while (true)
            {
                //Console.WriteLine("Press key to continue.");
                //Console.Read();
                if (testModule.Ended()) break;
                Console.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }
            //Console.Read();
        }
    }
     
}
