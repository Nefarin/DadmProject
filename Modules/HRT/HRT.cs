using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.R_Peaks;
using EKG_Project.Modules.Heart_Class;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;


namespace EKG_Project.Modules.HRT
{
    public class HRT : IModule
    {
        private enum STATE { INIT, BEGIN_CHANNEL, PROCESS_FIRST_STEP, PROCESS_CHANNEL, NEXT_CHANNEL, END_CHANNEL, END };
        private bool _ended;
        private bool _aborted;
        private int _currentChannelIndex;
        private int _currentChannelLength;
        private string _currentLeadName;
        private string[] _leads;
        private int _numberOfChannels;
        private uint _frequency;
        public enum VPC
        {
            NOT_DETECTED,
            NO_VENTRICULAR,
            DETECTED_BUT_IMPOSSIBLE_TO_PLOT,
            LETS_PLOT
        }
        private VPC _vpc;
        private Basic_New_Data_Worker _inputWorker_basic;
        private R_Peaks_New_Data_Worker _inputWorker_R_Peaks;
        private Heart_Class_New_Data_Worker _inputWorker_Heart_Class;
        private HRT_New_Data_Worker _outputWorker;
        private HRT_Params _params;
        private STATE _state;
        private HRT_Alg _alg;
        private Vector<double> _rpeaks;
        private Vector<double> _rrintervals;
        private List<Tuple<int, int>> _classAll;
        private List<int> _classPrematureVentrical;
        private List<int> _nrVPC;
        private List<List<double>> _tachogram;
        private List<int> _classVentrical;
        private Tuple<List<double>, int[], double[]> _turbulenceSlope;
        private int[] _xaxisTachogramGUI;
        private List<List<double>> _tachogramGUI;
        private double[] _tachogramMeanGUI;
        private int[] _xpointsOnsetGUI;
        private double[] _turbulenceOnsetmeanGUI;
        private int[] _xpointsSlopeGUI;
        private double[] _turbulenceSlopeMaxGUI;
        private List<double> _turbulenceOnsetPDF;
        private List<double> _turbulenceSlopePDF;
        int[] _statisticsClassNumbersPDF = { 0, 0, 0 };


        public void Abort()
        {
            Aborted = true;
            _ended = true;
        }

        public bool Ended()
        {
            return _ended;
        }

        public bool IsAborted()
        {
            return Aborted;
        }

        public void Init(ModuleParams parameters)
        {
            try
            {
                _params = parameters as HRT_Params;
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
                InputWorker_R_Peaks = new R_Peaks_New_Data_Worker(Params.AnalysisName);
                InputWorker_Heart_Class = new Heart_Class_New_Data_Worker(Params.AnalysisName);
                OutputWorker = new HRT_New_Data_Worker(Params.AnalysisName);
                _frequency = InputWorker_basic.LoadAttribute(Basic_Attributes.Frequency);
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
            return 100.0 * ((double)_currentChannelIndex / (double)NumberOfChannels);
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
                    _alg = new HRT_Alg();
                    _state = STATE.BEGIN_CHANNEL;
                    break;
                case (STATE.BEGIN_CHANNEL):
                    _currentChannelIndex++;
                    if (_currentChannelIndex >= _numberOfChannels) _state = STATE.END;
                    else
                    {
                        _currentLeadName = _leads[_currentChannelIndex];
                        _currentChannelLength = (int)_inputWorker_Heart_Class.getNumberOfSamples(_currentLeadName);
                        _state = STATE.PROCESS_CHANNEL;
                        int NoOfSamplesHeartClass = (int)InputWorker_Heart_Class.getNumberOfSamples(_currentLeadName);
                        int NoOfSamplesRPeaks = (int)InputWorker_R_Peaks.getNumberOfSamples(R_Peaks_Attributes.RPeaks, _currentLeadName);
                        int NoOfSamplesRRInterval = (int)InputWorker_R_Peaks.getNumberOfSamples(R_Peaks_Attributes.RRInterval, _currentLeadName);
                        _rpeaks = InputWorker_R_Peaks.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, 0, NoOfSamplesRPeaks);
                        _rrintervals = InputWorker_R_Peaks.LoadSignal(R_Peaks_Attributes.RRInterval, _currentLeadName, 0, NoOfSamplesRRInterval);
                        _classAll = InputWorker_Heart_Class.LoadClassificationResult(_currentLeadName, 0, NoOfSamplesHeartClass);
                        _state = STATE.PROCESS_CHANNEL;
                    }
                    break;
                case (STATE.PROCESS_CHANNEL):
                    if (_currentChannelIndex >= _numberOfChannels) _state = STATE.END_CHANNEL;
                    else
                    {
                        try
                        {
                            if (_rpeaks.Count < _classAll.Count)
                            {
                                _vpc = VPC.NOT_DETECTED;
                                System.Diagnostics.Debug.WriteLine("Wykryto więcej klas niż załamków, błędnie skonstruowany plik wejściowy");
                                OutputWorker.SaveVPC(_currentLeadName, _vpc);
                            }
                            else
                            {
                                _classVentrical = _alg.TakeNonAtrialComplexes(_classAll);
                                if (_classVentrical.Count == 0)
                                {
                                    System.Diagnostics.Debug.WriteLine("Brak jakiegokolwiek załamka mającego pochodzenie komorowe");
                                    _vpc = VPC.NOT_DETECTED;
                                    OutputWorker.SaveVPC(_currentLeadName, _vpc);
                                }
                                else
                                {
                                    _nrVPC = _alg.GetNrVPC(_rpeaks.ToArray(), _classVentrical.ToArray());
                                    _tachogram = _alg.MakeTachogram(_nrVPC, _rrintervals);
                                    _classPrematureVentrical = _alg.SearchPrematureTurbulences(_tachogram, _nrVPC);
                                    if (_classPrematureVentrical.Count == 0)
                                    {
                                        System.Diagnostics.Debug.WriteLine("Są komorowe załamki, ale nie ma przedwczesnych");
                                        _vpc = VPC.NO_VENTRICULAR;
                                        OutputWorker.SaveVPC(_currentLeadName, _vpc);
                                    }
                                    else
                                    {
                                        Tuple<int[], double[]> _meanTurbulenceOnset;

                                        _tachogram = _alg.MakeTachogram(_classPrematureVentrical, _rrintervals);
                                        if (_tachogram.Count == 0)
                                        {
                                            System.Diagnostics.Debug.WriteLine("Są VPC, ale nie można wygenerować wokół nich tachogramu. Prawdopodobna przyczyna to niewystarczająca ilość wykrytych załamków QRS w pobliżu");
                                            _vpc = VPC.DETECTED_BUT_IMPOSSIBLE_TO_PLOT;
                                            OutputWorker.SaveVPC(_currentLeadName, _vpc);
                                        }
                                        else
                                        {
                                            _vpc = VPC.LETS_PLOT;
                                            _turbulenceSlope = _alg.TurbulenceSlopeGUIandPDF(_classPrematureVentrical, _rrintervals);
                                            _meanTurbulenceOnset = _alg.TurbulenceOnsetMeanGUI(_tachogram);
                                            _xaxisTachogramGUI = _alg.xPlot();
                                            _tachogramGUI = _tachogram;
                                            _tachogramMeanGUI = _alg.MeanTachogram(_tachogram);
                                            _xpointsOnsetGUI = _meanTurbulenceOnset.Item1;
                                            _turbulenceOnsetmeanGUI = _meanTurbulenceOnset.Item2;
                                            _xpointsSlopeGUI = _turbulenceSlope.Item2;
                                            _turbulenceSlopeMaxGUI = _turbulenceSlope.Item3;
                                            _turbulenceOnsetPDF = _alg.TurbulenceOnsetsPDF(_classPrematureVentrical, _rrintervals);
                                            _turbulenceSlopePDF = _turbulenceSlope.Item1;
                                            _statisticsClassNumbersPDF[0] = _rpeaks.Count;
                                            _statisticsClassNumbersPDF[1] = _classVentrical.Count;
                                            _statisticsClassNumbersPDF[2] = _classPrematureVentrical.Count;
                                            _state = STATE.END_CHANNEL;

                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            _state = STATE.NEXT_CHANNEL;
                            System.Diagnostics.Debug.WriteLine("PROCESS_CHANNEL - Exception e");
                            _vpc = VPC.NOT_DETECTED;
                            OutputWorker.SaveVPC(_currentLeadName, _vpc);
                        }
                    }
                    break;
                case (STATE.END_CHANNEL):
                    try
                    {
                        OutputWorker.SaveVPC(_currentLeadName, _vpc);
                        OutputWorker.SaveXAxisTachogramGUI(_currentLeadName, false, _xaxisTachogramGUI);
                        OutputWorker.SaveTachogramGUI(_currentLeadName, false, _tachogramGUI);
                        OutputWorker.SaveMeanTachogramGUI(_currentLeadName, false, _tachogramMeanGUI);
                        OutputWorker.SaveXPointsMeanOnsetGUI(_currentLeadName, false, _xpointsOnsetGUI);
                        OutputWorker.SaveTurbulenceOnsetMeanGUI(_currentLeadName, false, _turbulenceOnsetmeanGUI);
                        OutputWorker.SaveXPointsMaxSlopeGUI(_currentLeadName, false, _xpointsSlopeGUI);
                        OutputWorker.SaveTurbulenceSlopeMaxGUI(_currentLeadName, false, _turbulenceSlopeMaxGUI);
                        OutputWorker.SaveTurbulenceOnsetPDF(_currentLeadName, false, _turbulenceOnsetPDF);
                        OutputWorker.SaveTurbulenceSlopePDF(_currentLeadName, false, _turbulenceSlopePDF);
                        OutputWorker.SaveStatisticsClassNumbersPDF(_currentLeadName, false, _statisticsClassNumbersPDF);

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
            get
            {
                return _aborted;
            }

            set
            {
                _aborted = value;
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

        public Basic_New_Data_Worker InputWorker_basic
        {
            get
            {
                return _inputWorker_basic;
            }

            set
            {
                _inputWorker_basic = value;
            }
        }

        public R_Peaks_New_Data_Worker InputWorker_R_Peaks
        {
            get
            {
                return _inputWorker_R_Peaks;
            }

            set
            {
                _inputWorker_R_Peaks = value;
            }
        }

        public Heart_Class_New_Data_Worker InputWorker_Heart_Class
        {
            get
            {
                return _inputWorker_Heart_Class;
            }

            set
            {
                _inputWorker_Heart_Class = value;
            }
        }

        public HRT_New_Data_Worker OutputWorker
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

        public HRT_Params Params
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

        public static void Main()
        {
            string[] lista = { "105", "106", "107", "108", "109", "114", "116", "118", "119", "124", "200", "201", "202", "203", "207" };
            foreach (string liczba in lista)
            {
                //System.Diagnostics.Debug.WriteLine("Analysis" + liczba);
                HRT_Params param = new HRT_Params("Analysis" + liczba);

                HRT testModule = new HRT();
                testModule.Init(param);
                while (true)
                {
                    if (testModule.Ended()) break;
                    //System.Diagnostics.Debug.Write("Progress: ");
                    // System.Diagnostics.Debug.Write(testModule.Progress());
                    // System.Diagnostics.Debug.WriteLine(" %");
                    testModule.ProcessData();
                }
            }
        }
    }
}