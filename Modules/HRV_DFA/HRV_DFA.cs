﻿using System;
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
        private enum STATE { INIT, BEST_LEAD, INTERPOLATE, FAST_PROCESS, CALCULATE_FLUCTUATIONS, LINSQ_APPROX, END };
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentRpeaksLength;
        private int _rPeaksProcessed;
        private int _numberOfLead;
        private int _numberOfChannels;
        private string _currentLeadName;
        private string _bestLeadName;
        private string[] _leads;

        private int _currentIndex;
        private int _currentChannelLength;
        private int _currentVectorLength;

        private Basic_New_Data_Worker _basicWorker;

        private R_Peaks_New_Data_Worker _inputWorker;
        private R_Peaks_Data _inputData;

        private HRV_DFA_New_Data_Worker _outputWorker;
        private HRV_DFA_Data _outputData;

        private HRV_DFA_Params _params;
        private HRV_DFA_Alg _alg;
        private Vector<Double> _currentVector;
        private Vector<Double> _currentRRVector;
        private Vector<Double> _currentResults;
        private Vector<Double> _fluctuations;
        private Vector<Double> _logn;
        private STATE _state;
        
       // private List<Vector<double>> _cFlucts;
        private Tuple< Vector<double>, Vector<double>> _currentnumberN;
        private Tuple< Vector<double>, Vector<double>> _currentfnValue;
        private Tuple< Vector<double>, Vector<double>> _currentparamsAlpha;
        private Tuple< Vector<double>, Vector<double>> _currentflucts;

        private int dfaStep;
        private int start;
        private int stop;

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
                BasicWorker = new Basic_New_Data_Worker(Params.AnalysisName + "temp");
                InputWorker = new R_Peaks_New_Data_Worker(Params.AnalysisName + "temp");
                OutputWorker = new HRV_DFA_New_Data_Worker(Params.AnalysisName + "temp"); 
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
            return 100.0 * (1.0 + 1.0  * ((double)_rPeaksProcessed / (double)_currentRpeaksLength));
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
                    _alg = new HRV_DFA_Alg();
                    _leads = BasicWorker.LoadLeads().ToArray();
                    _numberOfLead = _leads.Count();
                    
                    _state = STATE.BEST_LEAD;
                    //_inputWorker.DeleteFiles(); Do not use yet - will try to handle this during loading.
                    break;
                case (STATE.BEST_LEAD):
                    Vector<int> lengths = Vector<int>.Build.Dense(_numberOfLead);

                    for (int i = 0; i < _numberOfLead; i++)
                    {
                        _currentLeadName = _leads[i];
                        _currentRpeaksLength = (int)InputWorker.getNumberOfSamples(R_Peaks_Attributes.RRInterval, _currentLeadName);
                        lengths[i] = _currentRpeaksLength;
                    }
                    int longestLeadIndex = lengths.AbsoluteMaximumIndex();
                    _bestLeadName = _leads[longestLeadIndex];
                    _currentRpeaksLength = (int)InputWorker.getNumberOfSamples(R_Peaks_Attributes.RRInterval, _bestLeadName);
                    
                    _currentRRVector = InputWorker.LoadSignal(R_Peaks_Attributes.RRInterval, _bestLeadName, 0, _currentRpeaksLength);
                    _state = STATE.INTERPOLATE;
                    break; //done

                case (STATE.INTERPOLATE):

                    _currentVector = _alg.Interpolate(_currentRRVector);
                    _currentVectorLength = _currentVector.Count;

                    if (_currentVectorLength < 20)
                    {
                        _aborted = true;
                    }
                    else if(_currentVectorLength > 20 && _currentVectorLength < 2750)
                    {
                        _state = STATE.FAST_PROCESS;
                    }
                    else
                    {
                        _currentIndex = 0;
                        _state = STATE.CALCULATE_FLUCTUATIONS;
                    }
                    break; //done
                case (STATE.CALCULATE_FLUCTUATIONS):
                    int step = 2750;

                    dfaStep = 100;
                    start = 500;
                    stop = 50000;

                    if (_currentIndex + step < _currentVectorLength)
                    {
                        _currentVector = _currentVector.SubVector(_currentIndex, step);
                       
                        Tuple<Vector<double>, Vector<double>> _currentFlucts = _alg.ObtainFluctuations(dfaStep, start, stop, _currentVector);
                        _currentResults = _currentFlucts.Item2;
                        _fluctuations = _alg.CombineVectors(_fluctuations, _currentResults);
                        _logn = _alg.CombineVectors(_logn, _currentFlucts.Item1);

                        _rPeaksProcessed = _currentIndex + step;
                        _currentIndex += step;
                        _state = STATE.CALCULATE_FLUCTUATIONS;
                    }
                    else
                    {
                        _currentVector = _currentVector.SubVector(_currentIndex, _currentVectorLength - _currentIndex);
                        Tuple<Vector<double>, Vector<double>> _currentres = _alg.ObtainFluctuations(dfaStep, start, stop, _currentVector);
                        _currentResults = _currentres.Item2;
                        _fluctuations = _alg.CombineVectors(_fluctuations, _currentResults);
                        _logn = _alg.CombineVectors(_logn, _currentres.Item1);
                        _rPeaksProcessed = _currentVectorLength;
                    }

                    _currentflucts = new Tuple<Vector<double>, Vector<double>>(_logn,_fluctuations);
                    OutputWorker.SaveSignal(HRV_DFA_Signals.Fluctuations, _bestLeadName, true, _currentflucts);

                    _state = STATE.LINSQ_APPROX;
                    break; //done

                case (STATE.LINSQ_APPROX):
                    List<Tuple<Vector<double>, Vector<double>>> results = _alg.HRV_DFA_Analysis(_currentflucts, true);
                    OutputWorker.SaveSignal(HRV_DFA_Signals.DfaNumberN, _bestLeadName, true, results[0]);
                    OutputWorker.SaveSignal(HRV_DFA_Signals.DfaValueFn, _bestLeadName, true, results[1]);
                    OutputWorker.SaveSignal(HRV_DFA_Signals.ParamAlpha, _bestLeadName, true, results[2]);

                    _state = STATE.END;
                    break; //done

                case (STATE.FAST_PROCESS):

                    dfaStep = 50;
                    start = 500;
                    stop = 5000;
                    Tuple<Vector<double>, Vector<double>> _currentFlucts1 = _alg.ObtainFluctuations(dfaStep, start, stop, _currentVector);
                    List<Tuple<Vector<double>, Vector<double>>> results1 = _alg.HRV_DFA_Analysis(_currentFlucts1, false);
                    OutputWorker.SaveSignal(HRV_DFA_Signals.DfaNumberN, _bestLeadName, true, results1[0]);
                    OutputWorker.SaveSignal(HRV_DFA_Signals.DfaValueFn, _bestLeadName, true, results1[1]);
                    OutputWorker.SaveSignal(HRV_DFA_Signals.ParamAlpha, _bestLeadName, true, results1[2]);
                    OutputWorker.SaveSignal(HRV_DFA_Signals.Fluctuations, _bestLeadName, true, _currentFlucts1);

                    _state = STATE.END;
                    break; //done 

                case (STATE.END):

                    _ended = true;
                    break;

                default:
                    Abort();
                    break;
            }
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

        public Basic_New_Data_Worker BasicWorker
        {
            get
            {
                return _basicWorker;
            }

            set
            {
                _basicWorker = value;
            }
        }

    

        public static void Main()
        {
            HRV_DFA_Params param = new HRV_DFA_Params("Analysis1ps_R_Peaks_New_V1_RRInterval");
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
