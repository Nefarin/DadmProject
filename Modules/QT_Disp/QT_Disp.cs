using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.IO;
using EKG_Project.Modules.ECG_Baseline;
using EKG_Project.Modules.Waves;
using EKG_Project.Modules.R_Peaks;

namespace EKG_Project.Modules.QT_Disp
{
    public class QT_Disp : IModule
    {
        private enum STATE { INIT, BEGIN_CHANNEL, PROCESS_CHANNEL, NEXT_CHANNEL, END };

        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _currentChannelWaves_indexes;
        private int _samplesProcessed;
        private int _numberOfChannels;
        private string _currentLeadName;
        private string _R_Peak_Lead;
        private List<string> _leads;
        private int _currentIndex;
        private int _currentLength;
        private int _amountOfIndexesInInput;
        private int _indexesProcessed;
        private int _maxInputIndexes;
        bool createNewObject;
        private QT_Disp_Alg _qt_disp_algorithms;

        int step = 0;                   //stores actual count of sample signal vector
        int R_Peak_step = 1;            //we execute a process data with every R_peak
        int previousIndex = 0;
      
        private Vector<double> _r_peaks;
        private List<double> R_Peaks_List;
        //input workers
        private ECG_Baseline_New_Data_Worker _inputECGBaselineWorker;
        private R_Peaks_New_Data_Worker _inputRPeaksWorker;
        private Waves_New_Data_Worker _inputWavesWorker;
        private Basic_New_Data_Worker _inputBasicWorker;
        //output worker
        private Qt_Disp_New_Data_Worker _outputWorker;
        //input data
        private ECG_Baseline_Data _inputECGBaselineData;
        private R_Peaks_Data _inputRPeaksData;
        private Waves_Data _inputWavesData;
        private Basic_Data _inputBasicData;
        //output data
        private QT_Disp_Data _outputData;

        private QT_Disp_Params _params;

        private Vector<double> _currentVector;
        //private List<Tuple<string, List<int>>> _t_end_loacl;
        private List<int> _t_end_index = new List<int>();
        //private List<Tuple<string, List<double>>> _qt_intervals;
        List<double> qt_intervals = new List<double>();
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
                _params = parameters as QT_Disp_Params;
            }
            catch(Exception e)
            {
                Abort();
                return;
            }
         
            if (!Runnable()) _ended = true;
            else
            {
                //input workers
                InputECGBaselineWorker = new ECG_Baseline_New_Data_Worker(Params.AnalysisName);               
                InputRPeaksWorker = new R_Peaks_New_Data_Worker(Params.AnalysisName);
                InputWavesWorker = new Waves_New_Data_Worker(Params.AnalysisName);
                InputBasicWorker = new Basic_New_Data_Worker(Params.AnalysisName);              
                //output workers
                OutputWorker = new Qt_Disp_New_Data_Worker(Params.AnalysisName);  
            }
        }
        public void ProcessData()
        {
            if (Runnable()) processData();
            else _ended = true;
        }
        public double Progress()
        {
            return 100.0 * ((double)_currentChannelIndex / (double)NumberOfChannels + (1.0 / NumberOfChannels) * ((double)_samplesProcessed / (double)_currentChannelLength));
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
                    _currentChannelIndex = 0;
                    _currentIndex = 0;
                    _leads = InputBasicWorker.LoadLeads(); //Tutaj bedzie zmiana po poprawie workerów
                    _currentChannelWaves_indexes = (int)InputWavesWorker.getNumberOfSamples(Waves_Signal.QRSOnsets, _leads[_currentChannelIndex]);   // it's going to be in new worker
                    _currentLength = 2;
                    _R_Peak_Lead = _leads[0];

                    _state = STATE.BEGIN_CHANNEL;
                    _maxInputIndexes = 2000;
                    createNewObject = true;
                    OutputData = new QT_Disp_Data();
                    NumberOfChannels = _leads.Count;
                    _currentChannelLength = (int) InputBasicWorker.getNumberOfSamples(_leads[_currentChannelIndex]);
                    break;
                case (STATE.BEGIN_CHANNEL):
                    
                   
                    QT_Disp_Algorithms = new QT_Disp_Alg(_currentChannelWaves_indexes);      //create new object to calculate algorithms
                    _currentChannelWaves_indexes = (int) InputWavesWorker.getNumberOfSamples(Waves_Signal.QRSOnsets, _leads[_currentChannelIndex]);   // it's going to be in new worker

                    //set number of indexes to get from waves and r_peaks
                    if (_currentChannelWaves_indexes < _maxInputIndexes)
                    {
                        _amountOfIndexesInInput = _currentChannelWaves_indexes;
                    }
                    else
                    {
                        _amountOfIndexesInInput = _maxInputIndexes;
                        
                    }
                    //_currentChannelIndex++;
                    _currentLeadName = _leads[_currentChannelIndex];
                   
                    R_Peaks = InputRPeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _R_Peak_Lead, 0, _amountOfIndexesInInput);  //stores r peaks
                    R_Peaks_List = R_Peaks.ToList();
                    _currentIndex = (int)R_Peaks.ElementAt(0);     // get first index in r peaks
                    _currentLength = (int)(R_Peaks.ElementAt(1) - R_Peaks.ElementAt(0)); //get length between the first and next one r peak

                    // init new data to do some algorithms
                    QT_Disp_Algorithms.TODoInInit(InputWavesWorker.LoadSignal(Waves_Signal.QRSOnsets, _currentLeadName, 0, _amountOfIndexesInInput),
                        InputWavesWorker.LoadSignal(Waves_Signal.TEnds, _currentLeadName, 0, _amountOfIndexesInInput),
                        InputWavesWorker.LoadSignal(Waves_Signal.QRSEnds, _currentLeadName, 0, _amountOfIndexesInInput),
                        InputRPeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _R_Peak_Lead, 0, _amountOfIndexesInInput),
                        _params.TEndMethod, _params.QTMethod, InputBasicWorker.LoadAttribute(Basic_Attributes.Frequency)
                        );
                    _indexesProcessed = _amountOfIndexesInInput;
                    step = 0;
                    _state = STATE.PROCESS_CHANNEL;

                    break;
                case (STATE.PROCESS_CHANNEL):
                  
                    if(step > _amountOfIndexesInInput-3)
                    {
                        /*
                        Console.WriteLine("Lead:\t " + _currentLeadName);
                        Console.WriteLine("Index processed:\t" + _indexesProcessed);
                        Console.WriteLine("Current step:\t" + step);
                        Console.WriteLine("samples processed:\t" + _samplesProcessed);
                        Console.ReadKey();*/
                        // delete existing file if we writting firt time
                        if(previousIndex == 0)
                        {
                           OutputWorker.SaveQTIntervals(_currentLeadName, false, QT_Disp_Algorithms.QT_INTERVALS.
                            GetRange(previousIndex, step));
                           OutputWorker.SaveTEndLocal(_currentLeadName, false, QT_Disp_Algorithms.T_END_LOCAL.
                            GetRange(previousIndex, step));
                        }
                        else
                        {
                           OutputWorker.SaveQTIntervals(_currentLeadName, true, QT_Disp_Algorithms.QT_INTERVALS.
                            GetRange(previousIndex, step));
                           OutputWorker.SaveTEndLocal(_currentLeadName, true, QT_Disp_Algorithms.T_END_LOCAL.
                            GetRange(previousIndex, step));
                        }
                     
                        previousIndex += (step);
                        if (_indexesProcessed < _currentChannelWaves_indexes)
                        {
                            if (_currentChannelWaves_indexes - _indexesProcessed > _maxInputIndexes)
                            {
                                _amountOfIndexesInInput = _maxInputIndexes;
                            }
                            else
                            {
                                _amountOfIndexesInInput = _currentChannelWaves_indexes - _indexesProcessed;
                            }
                            R_Peaks = InputRPeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _R_Peak_Lead, _indexesProcessed, _amountOfIndexesInInput);  //stores r peaks
                            R_Peaks_List = R_Peaks.ToList();
                            _currentIndex = (int)R_Peaks.ElementAt(0);     // get first index in r peaks
                            _currentLength = (int)(R_Peaks.ElementAt(1) - R_Peaks.ElementAt(0)); //get length between the first and next one r peak

                            QT_Disp_Algorithms.TODoInInit(InputWavesWorker.LoadSignal(Waves_Signal.QRSOnsets, _currentLeadName, _indexesProcessed, _amountOfIndexesInInput),
                                InputWavesWorker.LoadSignal(Waves_Signal.TEnds, _currentLeadName, _indexesProcessed, _amountOfIndexesInInput),
                                InputWavesWorker.LoadSignal(Waves_Signal.QRSEnds, _currentLeadName, _indexesProcessed, _amountOfIndexesInInput),
                                InputRPeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _R_Peak_Lead, _indexesProcessed, _amountOfIndexesInInput),
                                _params.TEndMethod, _params.QTMethod, InputBasicWorker.LoadAttribute(Basic_Attributes.Frequency)
                            );
                            _indexesProcessed += _amountOfIndexesInInput;
                            _state = STATE.PROCESS_CHANNEL;
                            step = 0;

                        }
                        else
                        {
                            _state = STATE.NEXT_CHANNEL;
                            step = 0;

                        }
                       
                    }
                    else
                    {
                        _currentVector = InputECGBaselineWorker.LoadSignal(_currentLeadName, _currentIndex, _currentLength);
                        QT_Disp_Algorithms.ToDoInProccessData(_currentVector, step);
                        _samplesProcessed = _currentIndex + _currentLength;
                        step++;
                        _currentIndex = (int) R_Peaks_List.ElementAt(step);
                        _currentLength =(int)( R_Peaks_List.ElementAt(step+1) - R_Peaks_List.ElementAt(step));
                                    

                    }
                    break;
                case (STATE.NEXT_CHANNEL):
                    if (_currentChannelIndex < NumberOfChannels-1)
                    {
                        _currentChannelIndex++;

                        double[] qt_temp = new double[QT_Disp_Algorithms.QT_INTERVALS.Count()];
                        int[] t_end = new int[QT_Disp_Algorithms.T_END_LOCAL.Count()];

                        QT_Disp_Algorithms.QT_INTERVALS.CopyTo(qt_temp);
                        QT_Disp_Algorithms.T_END_LOCAL.CopyTo(t_end);

                        OutputData.QT_Intervals.Add(Tuple.Create(_currentLeadName,qt_temp.ToList()));
                        OutputData.T_End_Local.Add(Tuple.Create(_currentLeadName, t_end.ToList()));

                        OutputData.QT_mean.Add(Tuple.Create(_currentLeadName, QT_Disp_Algorithms.getMean()));
                        OutputData.QT_std.Add(Tuple.Create(_currentLeadName, QT_Disp_Algorithms.getStd()));
                        OutputData.QT_disp_local.Add(Tuple.Create(_currentLeadName,QT_Disp_Algorithms.getLocal()));

                        OutputWorker.SaveAttribute(Qt_Disp_Attributes.QT_disp_local,_currentLeadName, QT_Disp_Algorithms.getLocal());
                        OutputWorker.SaveAttribute(Qt_Disp_Attributes.QT_mean,_currentLeadName, QT_Disp_Algorithms.getMean());
                        OutputWorker.SaveAttribute(Qt_Disp_Attributes.QT_std,_currentLeadName, QT_Disp_Algorithms.getStd());                      

                        QT_Disp_Algorithms.DeleteQT_Intervals();

                        _state = STATE.BEGIN_CHANNEL;
                        step = 0;
                        previousIndex = 0;
                    }
                    else
                    {
                        double[] qt_temp = new double[QT_Disp_Algorithms.QT_INTERVALS.Count()];
                        int[] t_end = new int[QT_Disp_Algorithms.T_END_LOCAL.Count()];

                        QT_Disp_Algorithms.QT_INTERVALS.CopyTo(qt_temp);
                        QT_Disp_Algorithms.T_END_LOCAL.CopyTo(t_end);

                        OutputData.QT_Intervals.Add(Tuple.Create(_currentLeadName, qt_temp.ToList()));
                        OutputData.T_End_Local.Add(Tuple.Create(_currentLeadName, t_end.ToList()));

                        OutputData.QT_disp_local.Add(Tuple.Create(_currentLeadName, QT_Disp_Algorithms.getLocal()));
                        OutputData.QT_mean.Add(Tuple.Create(_currentLeadName, QT_Disp_Algorithms.getMean()));
                        OutputData.QT_std.Add(Tuple.Create(_currentLeadName, QT_Disp_Algorithms.getStd()));

                        OutputWorker.SaveAttribute(Qt_Disp_Attributes.QT_disp_local, _currentLeadName, QT_Disp_Algorithms.getLocal());
                        OutputWorker.SaveAttribute(Qt_Disp_Attributes.QT_mean,_currentLeadName, QT_Disp_Algorithms.getMean());
                        OutputWorker.SaveAttribute(Qt_Disp_Attributes.QT_std, _currentLeadName, QT_Disp_Algorithms.getStd());

                        _state = STATE.END;
                    }
                    break;
                case (STATE.END):
                    
                    _ended = true;
                    break;
                default:
                    Abort();
                    break;
            }
      
        }
        //Getters and Setters
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
        public List<int> T_End_Local
        {
            get
            {
                return _t_end_index;
            }
            set
            {
                _t_end_index = value;
            }
        }
        public List<double> QT_Intervals
        {
            get
            {
                return qt_intervals;
            }
            set
            {
                qt_intervals = value;
            }
        }
       


        //input workers
        

        public ECG_Baseline_New_Data_Worker InputECGBaselineWorker
        {
            get
            {
                return _inputECGBaselineWorker;
            }
            set
            {
                _inputECGBaselineWorker = value;
            }
        }
        public Waves_New_Data_Worker InputWavesWorker
        {
            get
            {
                return _inputWavesWorker;
            }
            set
            {
                _inputWavesWorker = value;
            }
        }
        public R_Peaks_New_Data_Worker InputRPeaksWorker
        {
            get
            {
                return _inputRPeaksWorker;
            }
            set
            {
                _inputRPeaksWorker = value;
            }
        }
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

        //output worker
        private Qt_Disp_New_Data_Worker OutputWorker
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
        //input Data
        public ECG_Baseline_Data InputECGBaselineData
        {
            get
            {
                return _inputECGBaselineData;
            }
            set
            {
                _inputECGBaselineData = value;
            }
        }
        public Waves_Data InputWavesData
        {
            get
            {
                return _inputWavesData;
            }
            set
            {
                _inputWavesData = value;
            }
        }
        public R_Peaks_Data InputRPeaksData
        {
            get
            {
                return _inputRPeaksData;
            }
            set
            {
                _inputRPeaksData = value;
            }
        }
        public Basic_Data InputBasicData
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
        //output Data
        public QT_Disp_Data OutputData
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

        public QT_Disp_Params Params
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
        public QT_Disp_Alg QT_Disp_Algorithms
        {
            get
            {
                return _qt_disp_algorithms;
            }
            set
            {
                _qt_disp_algorithms = value;
            }
        }
        public Vector<double> R_Peaks
        {
            get
            {
                return _r_peaks;
            }
            set
            {
                _r_peaks = value;
            }
        }
        public static void Main()
        {
            QT_Disp_Params param = new QT_Disp_Params("Analysis QT");
            //param.AnalysisName = "Analysis 1";
            QT_Disp testModule = new QT_Disp();
            testModule.Init(param);
            Console.WriteLine("Let's start");
            while (true)
            {
                //Console.WriteLine("Press key to continue...");
                //Console.ReadKey();
                if (testModule.Ended()) break;
                Console.WriteLine(testModule.Progress().ToString("#.00"));
                testModule.ProcessData();
            }
            Console.WriteLine("Finish");
            Console.ReadKey();         

        }/*
        public static void Main(String[] args)
        {
            QT_Disp_Stats stats = new QT_Disp_Stats();
            stats.Init("Analysis 1");


            while (true)
            {
                if (stats.Ended()) break;
                stats.ProcessStats();
            }

            foreach (var key in stats.GetStatsAsString().Keys)
            {
                Console.WriteLine(key + stats.GetStatsAsString()[key]);
            }
            Console.Read();

        }*/



    }
}
