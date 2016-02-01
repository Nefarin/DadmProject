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

q

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
        private int _currentIndex;
        private int _numberOfChannels;
        private int _samplesProcessed;
        private int _fs;

        private Basic_New_Data_Worker _inputWorker_basic;
        private R_Peaks_New_Data_Worker _inputWorker_Rpeaks;
        private Heart_Class_New_Data_Worker _inputWorker_Heart_Class;
        private HRT_New_Data_Worker _outputWorker;

        private Basic_Data _inputData_basic;
        private R_Peaks_Data _inputData_R_peaks;
        private Heart_Class_Data _inputData_Heart_Class;
        private HRT_Data _outputData;
        private HRT_Params _params;


        private STATE _state;
        private HRT_Alg HRT_Algorythms;
        private Vector<double> _rpeaksSelected;
        private Vector<double> _rrintervals;
        private List<Tuple<int, int>> _classAll;
        private List<int> _classPrematureVentrical;
        private List<int> _nrVPC;
        private List<double[]> _tachogram;
        private List<int> _classVentrical;
        private List<double> _turbulenceOnset;
        private Tuple<List<double>,int[],double[]> _turbulenceSlope;
        private double[] _meanTachogram;
        private int[] _xaxis;

        private List<Tuple<string, int[], List<double[]>>> _tachogramGUI = new List<Tuple<string, int[], List<double[]>>>();
        private List<Tuple<string, int[], double[]>> _tachogramMeanGUI = new List<Tuple<string, int[], double[]>>();
        private List<Tuple<string, int[], double[]>> _turbulenceOnsetmeanGUI = new List<Tuple<string, int[], double[]>>();
        private List<Tuple<string, int[], double[]>> _turbulenceSlopeMaxGUI = new List<Tuple<string, int[], double[]>>();
        private List<Tuple<string, List<double>>> _turbulenceOnsetPDF = new List<Tuple<string, List<double>>>();
        private List<Tuple<string, List<double>>> _turbulenceSlopePDF = new List<Tuple<string, List<double>>>();

        


        //

        //Aborted = false;



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
                Params = parameters as HRT_Params;
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
                InputData_basic = new Basic_Data();
                InputData_R_Peaks = new R_Peaks_Data();
                InputData_Heart_Class = new Heart_Class_Data();
                OutputData = new HRT_Data();

                _frequency = InputWorker_basic.LoadAttribute(Basic_Attributes.Frequency);
                _step = Convert.ToInt32(_frequency * 16);
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
                    _alg = new R_Peaks_Alg();
                    _state = STATE.BEGIN_CHANNEL;
                    break;
                case (STATE.BEGIN_CHANNEL):
                    _currentChannelIndex++;
                    if (_currentChannelIndex >= _numberOfChannels) _state = STATE.END;
                    else
                    {
                        _currentLeadName = _leads[_currentChannelIndex];
                        _currentChannelLength = (int)InputWorker_basic.getNumberOfSamples(_currentLeadName); //Zmienić na worker ECG_BASELINE
                        _currentIndex = 0;
                        _state = STATE.PROCESS_FIRST_STEP;
                    }
                    break;
                case (STATE.PROCESS_FIRST_STEP):
                    if (_currentIndex + _step > _currentChannelLength) _state = STATE.END_CHANNEL;
                    else
                    {
                        try
                        {
                            _currentVector = InputWorker.LoadSignal(_currentLeadName, _currentIndex, _step);
                            Console.WriteLine("_currentLeadName " + _currentLeadName);
                            Console.WriteLine("_currentIndex " + _currentIndex);
                            Console.WriteLine("_step " + _step);
                            try         //zagniezdzone wyjatki?????????
                            {
                                //choosing and performing algorithm
                                switch (Params.Method)
                                {
                                    case R_Peaks_Method.PANTOMPKINS:
                                        _currentVector = _alg.PanTompkins(_currentVector, _frequency);
                                        break;
                                    case R_Peaks_Method.HILBERT:
                                        _currentVector = _alg.Hilbert(_currentVector, _frequency);
                                        break;
                                    case R_Peaks_Method.EMD:
                                        _currentVector = _alg.EMD(_currentVector, _frequency);
                                        break;
                                }
                                OutputWorker.SaveSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, false, _currentVector);
                                if (_currentVector.Count > 1)
                                {
                                    _currentVectorRRInterval = _alg.RRinMS(_currentVector, _frequency);
                                    //save results
                                    OutputWorker.SaveSignal(R_Peaks_Attributes.RRInterval, _currentLeadName, false, _currentVectorRRInterval);
                                }
                                //updating current index
                                _currentIndex = Convert.ToInt32(_currentVector.Last()) + Convert.ToInt32(_frequency * 0.1);

                            }
                            catch (Exception ex)
                            {
                                _currentIndex = _currentIndex + _step;
                                Console.WriteLine("No detected R peaks in this part of signal");
                            }
                            _state = STATE.PROCESS_CHANNEL;
                        }
                        catch (Exception e)
                        {
                            _state = STATE.NEXT_CHANNEL;
                            Console.WriteLine("PROCESS_FIRST_STEP - Exception e");
                        }
                    }
                    break;
                case (STATE.PROCESS_CHANNEL):  // this state can be divided to load state, process state and save state, good decision especially for ECG_Baseline, R_Peaks, Waves and Heart_Class
                    if (_currentIndex + _step > _currentChannelLength) _state = STATE.END_CHANNEL;
                    else
                    {
                        try
                        {
                            _currentVector = InputWorker.LoadSignal(_currentLeadName, _currentIndex, _step);
                            try         //zagniezdzone wyjatki?????????
                            {
                                //choosing and performing algorithm
                                switch (Params.Method)
                                {
                                    case R_Peaks_Method.PANTOMPKINS:
                                        _currentVector = _alg.PanTompkins(_currentVector, _frequency);
                                        break;
                                    case R_Peaks_Method.HILBERT:
                                        _currentVector = _alg.Hilbert(_currentVector, _frequency);
                                        break;
                                    case R_Peaks_Method.EMD:
                                        _currentVector = _alg.EMD(_currentVector, _frequency);
                                        break;
                                }
                                _currentVector.Add(_currentIndex, _currentVector);
                                //save results
                                OutputWorker.SaveSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, true, _currentVector);
                                if (_currentVector.Count > 1)
                                {
                                    _currentVectorRRInterval = _alg.RRinMS(_currentVector, _frequency);
                                    //save results
                                    OutputWorker.SaveSignal(R_Peaks_Attributes.RRInterval, _currentLeadName, true, _currentVectorRRInterval);
                                }
                                //updating current index
                                _currentIndex = Convert.ToInt32(_currentVector.Last()) + Convert.ToInt32(_frequency * 0.1);
                            }
                            catch (Exception ex)
                            {
                                _currentIndex = _currentIndex + _step;
                                Console.WriteLine("No detected R peaks in this part of signal");
                            }
                            _state = STATE.PROCESS_CHANNEL;
                        }
                        catch (Exception e)
                        {
                            _state = STATE.NEXT_CHANNEL;
                            Console.WriteLine("PROCESS_CHANNEL - Exception e");
                        }
                    }
                    break;
                case (STATE.END_CHANNEL):
                    try
                    {
                        _currentVector = InputWorker.LoadSignal(_currentLeadName, _currentIndex, _currentChannelLength - _currentIndex);
                        try         //zagniezdzone wyjatki?????????
                        {
                            //choosing and performing algorithm
                            switch (Params.Method)
                            {
                                case R_Peaks_Method.PANTOMPKINS:
                                    _currentVector = _alg.PanTompkins(_currentVector, _frequency);
                                    break;
                                case R_Peaks_Method.HILBERT:
                                    _currentVector = _alg.Hilbert(_currentVector, _frequency);
                                    break;
                                case R_Peaks_Method.EMD:
                                    _currentVector = _alg.EMD(_currentVector, _frequency);
                                    break;
                            }
                            _currentVector.Add(_currentIndex, _currentVector);
                            //save results
                            OutputWorker.SaveSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, true, _currentVector);
                            if (_currentVector.Count > 1)
                            {
                                _currentVectorRRInterval = _alg.RRinMS(_currentVector, _frequency);
                                //save results
                                OutputWorker.SaveSignal(R_Peaks_Attributes.RRInterval, _currentLeadName, true, _currentVectorRRInterval);
                            }
                            //updating current index
                            _currentIndex = Convert.ToInt32(_currentVector.Last()) + Convert.ToInt32(_frequency * 0.1);
                        }
                        catch (Exception ex)
                        {
                            _currentIndex = _currentIndex + _step;
                            Console.WriteLine("No detected R peaks in this part of signal");
                        }
                        _state = STATE.NEXT_CHANNEL;
                    }
                    catch (Exception e)
                    {
                        _state = STATE.NEXT_CHANNEL;
                        Console.WriteLine("END_CHANNEL - Exception e");
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














            if (_currentChannelIndex < NumberOfChannels)
            {
                Console.Write(_currentChannelIndex);
                Console.Write("/");
                Console.WriteLine(NumberOfChannels);

                string _channelName = InputRpeaksData.RPeaks[_currentChannelIndex].Item1;

                _rpeaksSelected = InputRpeaksData.RPeaks[_currentChannelIndex].Item2;
                _rrintervals = InputRpeaksData.RRInterval[_currentChannelIndex].Item2;
                _classAll = InputHeartClassData.ClassificationResult;

             
                //_rpeaksSelected = HRT_Algorythms.rrTimesShift(_rpeaksSelected);


                // _classVentrical = HRT_Algorythms.checkVPCifnotNULL(_classVentrical);
                if (_rpeaksSelected.Count < _classAll.Count)
                {
                    Console.WriteLine("Wykryto więcej klas niż załamków, błędnie skonstruowany plik wejściowy");
                }
                else
                {
                    _classVentrical = HRT_Algorythms.TakeNonAtrialComplexes(_classAll);

                  
                    if (_classVentrical.Capacity == 0)
                    {
                        Console.WriteLine("Brak jakiegokolwiek załamka mającego pochodzenie komorowe");
                    }
                    else
                    {
                        _nrVPC = HRT_Algorythms.GetNrVPC(_rpeaksSelected.ToArray(), _classVentrical.ToArray());
                        _tachogram = HRT_Algorythms.MakeTachogram(_nrVPC, _rrintervals);
                        _classPrematureVentrical = HRT_Algorythms.SearchPrematureTurbulences(_tachogram, _nrVPC);
                        if (_classPrematureVentrical.Capacity == 0)
                        {
                            Console.WriteLine("Są komorowe załamki, ale nie ma przedwczesnych");
                        }
                        else
                        {
                            Tuple<int[], double[]> _meanTurbulenceOnset;
                            

                            _tachogram = HRT_Algorythms.MakeTachogram(_classPrematureVentrical, _rrintervals);
                            _turbulenceOnset = HRT_Algorythms.TurbulenceOnsetsPDF(_classPrematureVentrical, _rrintervals);
                            _turbulenceSlope = HRT_Algorythms.TurbulenceSlopeGUIandPDF(_classPrematureVentrical, _rrintervals);

                            



                            _meanTurbulenceOnset = HRT_Algorythms.TurbulenceOnsetMeanGUI(_tachogram);
                            _meanTachogram = HRT_Algorythms.MeanTachogram(_tachogram);

           


                            _xaxis = HRT_Algorythms.xPlot();
                           



                            _tachogramGUI.Add(Tuple.Create(_channelName, _xaxis, _tachogram));
                            _tachogramMeanGUI.Add(Tuple.Create(_channelName, _xaxis, _meanTachogram));
                            _turbulenceOnsetmeanGUI.Add(Tuple.Create(_channelName, _meanTurbulenceOnset.Item1, _meanTurbulenceOnset.Item2));
                            _turbulenceSlopeMaxGUI.Add(Tuple.Create(_channelName, _turbulenceSlope.Item2, _turbulenceSlope.Item3));


                            _turbulenceOnsetPDF.Add(Tuple.Create(_channelName, _turbulenceOnset));
                            _turbulenceSlopePDF.Add(Tuple.Create(_channelName, _turbulenceSlope.Item1));


                            HRT_Algorythms.PrintVector(_turbulenceSlopeMaxGUI);

                        }
                    }
                }
            }
            else
            {
               // _outputData._TurbulenceOnset.Add();
               // _outputData._VPCtachogram.Add(FinalResults.Item1);
                //NASZE DANE WYJSCIOWE PRZYGOTOWAC I WYSLAC DO ZAPISU
                //OutputWorker.Save(OutputData);
                _ended = true;
            }

            _currentChannelIndex++;


           

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

    public R_Peaks_New_Data_Worker InputWorker_Rpeaks
    {
        get
        {
            return _inputWorker_Rpeaks;
        }

        set
        {
            _inputWorker_Rpeaks = value;
        }
    }

    public Heart_Class_New_Data_Worker InputWorker_HeartClass
    {
        get
        {
            return _inputWorker_HeartClass;
        }

        set
        {
            _inputWorker_HeartClass = value;
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

    public Basic_Data InputData_basic
    {
        get
        {
            return _inputData_basic;
        }

        set
        {
            _inputData_basic = value;
        }
    }

    public R_Peaks_Data InputData_Rpeaks
    {
        get
        {
            return _inputData_Rpeaks;
        }

        set
        {
            _inputData_Rpeaks = value;
        }
    }

    public Heart_Class_Data InputData_HeartClass
    {
        get
        {
            return _inputData_HeartClass;
        }

        set
        {
            _inputData_HeartClass = value;
        }
    }

    public HRT_Data OutputData
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


    public static void Main() {

            HRT_Params param = new HRT_Params("TestAnalysisEgz");   

            HRT testModule = new HRT();
            testModule.Init(param);
            while (true)
            {
                if (testModule.Ended()) break;
                Console.Write("Progress: ");
                Console.Write(testModule.Progress());
                Console.WriteLine(" %");
                testModule.ProcessData();
            }
        }

    }
}
