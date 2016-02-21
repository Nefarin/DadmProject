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
        private enum STATE { INIT, BEST_LEAD, INTERPOLATE, FAST_PROCESS, CALCULATE_FLUCTUATIONS_FIRSTSTEP, CALCULATE_FLUCTUATIONS_MIDSTEP, CALCULATE_FLUCTUATIONS_ENDSTEP, LINSQ_APPROX, END };
        private bool _ended;
        private bool _aborted;

        private int _currentRpeaksLength;
        private int _rPeaksProcessed;
        private int _numberOfLead;
        private string _currentLeadName;
        private string[] _leads;
        private string _bestLeadName;

        private int _currentIndex;
        private int _currentVectorLength;

        private Basic_New_Data_Worker _basicWorker;

        private R_Peaks_New_Data_Worker _inputWorker;
        private R_Peaks_Data _inputData;

        private HRV_DFA_New_Data_Worker _outputWorker;
        private HRV_DFA_Data _outputData;

        private HRV_DFA_Params _params;
        private HRV_DFA_Alg _alg;

        private Tuple<Vector<double>, Vector<double>> _currentflucts;
        private Vector<Double> _currentVector;
        private Vector<Double> _currentVector1;
        private Vector<Double> _currentVector2;
        private Vector<Double> _currentVector3;
        private Vector<Double> _currentRRVector;
        private Vector<Double> _currentResults;
        private Vector<Double> _currentResults2;
        private Vector<Double> _currentResultsmid;
        private Vector<Double> _currentResultsend;
        private Vector<Double> _fluctuations;
        private Vector<Double> _fluctuations1;
        private Vector<Double> _fluctuations2;
        private Vector<Double> _logn;
        private Vector<Double> _logn1;
        private STATE _state;
        private int dfaStep;
        private int start;
        private int stop;
        private int _step = 1000;

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
                BasicWorker = new Basic_New_Data_Worker(Params.AnalysisName);
                InputWorker = new R_Peaks_New_Data_Worker(Params.AnalysisName);
                OutputWorker = new HRV_DFA_New_Data_Worker(Params.AnalysisName); 
                InputData = new R_Peaks_Data();
                OutputData = new HRV_DFA_Data();

                _alg = new HRV_DFA_Alg();

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
                    
                    _leads = BasicWorker.LoadLeads().ToArray();
                    _numberOfLead = _leads.Count();

                    Console.WriteLine("init");
                    _state = STATE.BEST_LEAD;
                    //_inputWorker.DeleteFiles(); Do not use yet - will try to handle this during loading.
                    
                    break;
                case (STATE.BEST_LEAD):

                    Vector<double> lengths = Vector<double>.Build.Dense(_numberOfLead);

                     for (int i = 0; i < _numberOfLead; i++)
                     {
                         _currentLeadName = _leads[i];
                         _currentRpeaksLength = (int)InputWorker.getNumberOfSamples(R_Peaks_Attributes.RRInterval, _currentLeadName);
                         lengths[i] = _currentRpeaksLength;
                     }

                     int longestLeadIndex = lengths.MaximumIndex();
                     _bestLeadName = _leads[longestLeadIndex];
                   
                     _currentRpeaksLength = (int)InputWorker.getNumberOfSamples(R_Peaks_Attributes.RRInterval, _bestLeadName);
                    
                    _currentRRVector = InputWorker.LoadSignal(R_Peaks_Attributes.RRInterval, _bestLeadName, 0, _currentRpeaksLength);
                    Console.WriteLine(_bestLeadName + _currentRpeaksLength);
                    Console.WriteLine("bestlead");
                    _state = STATE.INTERPOLATE;
                    break; //done

                case (STATE.INTERPOLATE):
                    _alg = new HRV_DFA_Alg();

                    if (_currentRpeaksLength < 50)
                    {
                       _state = STATE.END;
                    }
                    else if(_currentRpeaksLength > 50 && _currentRpeaksLength < 3000)
                    {
                        _currentVector = _alg.Interpolate(_currentRRVector);
                        _currentVectorLength = _currentVector.Count;
                        Console.WriteLine("interpolate");
                        _state = STATE.FAST_PROCESS;
                    }
                    else
                    {
                        try {
                            _currentVector = _alg.Interpolate(_currentRRVector);
                            _currentVectorLength = _currentVector.Count;
                            _currentIndex = -1;
                            dfaStep = 50;
                            start = 500;
                            stop = 50000;
                            Console.WriteLine("interpolate");
                            _state = STATE.CALCULATE_FLUCTUATIONS_FIRSTSTEP;
                        }
                        catch (Exception e)
                        {
                            Abort();
                            _state = STATE.END;
                        }
                    }
                    break; //done
                case (STATE.CALCULATE_FLUCTUATIONS_FIRSTSTEP):
                    
                    if (_currentIndex + _step > _currentVectorLength)
                    {
                        _state = STATE.CALCULATE_FLUCTUATIONS_ENDSTEP;
                    }
                    else
                    {
                        try
                        {
                            _currentIndex++;
                            _currentVector1 = _currentVector.SubVector(_currentIndex, _step);
                            _alg = new HRV_DFA_Alg();
                            Tuple<Vector<double>, Vector<double>> _currentres = _alg.ObtainFluctuations(dfaStep, start, stop, _currentVector);

                            _currentResults2 = _currentres.Item1;
                            _currentResults = _currentres.Item2;
                   
                            _fluctuations = _currentResults;
                            _logn = _currentResults2;

                            _rPeaksProcessed = _currentIndex + _step;
                            _currentIndex = _currentIndex + _step;
                            _state = STATE.CALCULATE_FLUCTUATIONS_MIDSTEP;
                        }
                        catch(Exception e)
                        {
                            _state = STATE.END;
                        }
                    }
                   
                    Console.WriteLine("fluctuations");
                    
                    break; //done
                case (STATE.CALCULATE_FLUCTUATIONS_MIDSTEP):
                    if (_currentIndex + _step > _currentVectorLength)
                    {
                        _state = STATE.CALCULATE_FLUCTUATIONS_ENDSTEP;
                    }
                    else
                    {
                        try
                        {
                        ++_currentIndex;
                        _currentVector2 = _currentVector.SubVector(_currentIndex, _step);
                        _alg = new HRV_DFA_Alg();
                        Tuple<Vector<double>, Vector<double>> _currentres2 = _alg.ObtainFluctuations(dfaStep, start, stop, _currentVector2);

                        _currentResultsmid = _currentres2.Item2;
                        _fluctuations1 = _alg.CombineVectors(_fluctuations, _currentResultsmid);

                        _rPeaksProcessed = _currentIndex + _step;
                        _currentIndex = _currentIndex + _step;
                        _state = STATE.CALCULATE_FLUCTUATIONS_MIDSTEP;
                        }
                        catch(Exception e)
                        {
                             _state = STATE.LINSQ_APPROX;
                        }
                    }
                    break;

                case (STATE.CALCULATE_FLUCTUATIONS_ENDSTEP):
                    try
                    {
                    
                        _alg = new HRV_DFA_Alg();
                        _currentVector3 = _currentVector.SubVector(_currentIndex, _currentVectorLength - _currentIndex-1);
                        if (_currentVector3.Count < _step )
                        {
                            _fluctuations2 = _fluctuations1;
                        }
                        else
                        {
                            Tuple<Vector<double>, Vector<double>> _currentres = _alg.ObtainFluctuations(dfaStep, start, stop, _currentVector3);
                            _currentResultsend = _currentres.Item2;
                            _fluctuations2 = _alg.CombineVectors(_fluctuations1, _currentResultsend);
                        }
                        
                        _rPeaksProcessed = _currentVectorLength;
                        _state = STATE.LINSQ_APPROX;
                    }
                    catch(Exception e)
                    {
                        _state = STATE.LINSQ_APPROX;
                    }
                    
                    break;
                case (STATE.LINSQ_APPROX):
                    _logn1 = _logn.SubVector(0, _fluctuations2.Count);
                    _currentflucts = new Tuple<Vector<double>, Vector<double>>(_logn1, _fluctuations2);
                    _alg = new HRV_DFA_Alg();
                    int length = _currentflucts.Item2.Count;
                    List<Tuple<Vector<double>, Vector<double>>> results = _alg.HRV_DFA_Analysis(_currentflucts, true);
                    OutputWorker.SaveSignal(HRV_DFA_Signals.DfaNumberN, _bestLeadName, true, results[0]);
                    OutputWorker.SaveSignal(HRV_DFA_Signals.DfaValueFn, _bestLeadName, true, results[1]);
                    OutputWorker.SaveSignal(HRV_DFA_Signals.ParamAlpha, _bestLeadName, true, results[2]);
                    OutputWorker.SaveSignal(HRV_DFA_Signals.Fluctuations, _bestLeadName, true, _currentflucts);
                    Console.WriteLine("linqs");
                    _state = STATE.END;
                    break; //done

                case (STATE.FAST_PROCESS):
                    try
                    {
                        _alg = new HRV_DFA_Alg();
                        dfaStep = 10;
                        start = 50;
                        stop = 500;
                        Tuple<Vector<double>, Vector<double>> _currentFlucts1 = _alg.ObtainFluctuations(dfaStep, start, stop, _currentVector);
                        List<Tuple<Vector<double>, Vector<double>>> results1 = _alg.HRV_DFA_Analysis(_currentFlucts1, false);
                        OutputWorker.SaveSignal(HRV_DFA_Signals.DfaNumberN, _bestLeadName, true, results1[0]);
                        OutputWorker.SaveSignal(HRV_DFA_Signals.DfaValueFn, _bestLeadName, true, results1[1]);
                        OutputWorker.SaveSignal(HRV_DFA_Signals.ParamAlpha, _bestLeadName, true, results1[2]);
                        OutputWorker.SaveSignal(HRV_DFA_Signals.Fluctuations, _bestLeadName, true, _currentFlucts1);
                        Console.WriteLine("fast process");
                        _state = STATE.END;
                    }
                    catch (Exception e)
                    {
                        Abort();
                        _state = STATE.END;
                    }

                    break; //done 

                case (STATE.END):
                    Console.WriteLine("END");
                    _ended = true;
                    break;

                default:
                    Abort();
                    break;
            }
        }

        public bool Aborted
        { get{ return _aborted; }set {_aborted = value;} }

        public int CurrentRpeaksLength
        { get{return _currentRpeaksLength;} set{ _currentRpeaksLength = value;}}

        public int RPeaksProcessed
        { get{return _rPeaksProcessed; } set { _rPeaksProcessed = value; }}

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
        
        public Tuple<Vector<double>, Vector<double>> Currentflucts
        {get {return _currentflucts;}set { _currentflucts = value;}}

        public Basic_New_Data_Worker BasicWorker
        {get{ return _basicWorker;}set{_basicWorker = value;}}

        public static void Main()
        {
            IModule testModule = new HRV_DFA();
            HRV_DFA_Params param = new HRV_DFA_Params("longsignal");

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
