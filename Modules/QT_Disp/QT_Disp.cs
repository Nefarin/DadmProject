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
        private int _currentChannelR_Peaks_indexes;
        private int _samplesProcessed;
        private int _numberOfChannels;
        private string _currentLeadName;
        private string[] _leads;
        private int _currentIndex;
        private int _currentLength;
        private int _amountOfIndexesInInput;
        private int _indexesProcessed;
        private int _maxInputIndexes;
        bool createNewObject;
        private QT_Disp_Alg _qt_disp_algorithms;

        int step = 0;                   //stores actual count of sample signal vector
        int R_Peak_step = 1;            //we execute a process data with every R_peak
        private Vector<double> _r_peaks;
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
        private List<Tuple<string, List<int>>> _t_end_loacl;
        private List<int> _t_end_index = new List<int>();
        private List<Tuple<string, List<double>>> _qt_intervals;
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
                    _currentChannelIndex = -1;
                    _currentIndex = 0;
                    _currentChannelR_Peaks_indexes = 1000;   // it's going to be in new worker
                    _currentLength = 2;
                    _numberOfChannels = 2; //Tutaj bedzie zmiana po poprawie workerów
                    _currentChannelLength = (int)InputBasicWorker.LoadAttribute(Basic_Attributes.NumberOfSamples); //Tutaj bedzie zmiana po poprawie workerow
                    _leads = null; //Tutaj bedzie zmiana po poprawie workerów
                    _state = STATE.BEGIN_CHANNEL;
                    _maxInputIndexes = 1000;
                    createNewObject = true;
                   
                    break;
                case (STATE.BEGIN_CHANNEL):
                    
                   
                    QT_Disp_Algorithms = new QT_Disp_Alg();      //create new object to calculate algorithms
                        
                    //set number of indexes to get from waves and r_peaks
                    if(_currentChannelR_Peaks_indexes < _maxInputIndexes)
                    {
                        _amountOfIndexesInInput = _currentChannelR_Peaks_indexes;
                    }
                    else
                    {
                        _amountOfIndexesInInput = _maxInputIndexes;
                        
                    }
                    _currentChannelIndex++;
                    _currentLeadName = _leads[_currentChannelIndex];
                   
                    R_Peaks = InputRPeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, 0, _amountOfIndexesInInput);  //stores r peaks
                    _currentIndex = (int)R_Peaks.ElementAt(0);     // get first index in r peaks
                    _currentLength = (int)(R_Peaks.ElementAt(1) - R_Peaks.ElementAt(0)); //get length between the first and next one r peak

                    // init new data to do some algorithms
                    QT_Disp_Algorithms.TODoInInit(InputWavesWorker.LoadSignal(Waves_Attributes.QRSOnsets, _currentLeadName, 0, _amountOfIndexesInInput),
                        InputWavesWorker.LoadSignal(Waves_Attributes.TEnds, _currentLeadName, 0, _amountOfIndexesInInput),
                        InputWavesWorker.LoadSignal(Waves_Attributes.QRSEnds, _currentLeadName, 0, _amountOfIndexesInInput),
                        InputRPeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, 0, _amountOfIndexesInInput),
                        _params.TEndMethod, _params.QTMethod, InputBasicWorker.LoadAttribute(Basic_Attributes.Frequency)
                        );
                    _indexesProcessed = _amountOfIndexesInInput;
                    _state = STATE.PROCESS_CHANNEL;

                    break;
                case (STATE.PROCESS_CHANNEL):
                  
                    if(step > _amountOfIndexesInInput-3)
                    {
                        if (_indexesProcessed < _currentChannelR_Peaks_indexes)
                        {
                            //need to be implemented

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
                        QT_Disp_Algorithms.ToDoInProccessData(_currentVector, _currentIndex);
                        _samplesProcessed = _currentIndex + _currentLength;
                        step++;
                        _currentIndex = (int) R_Peaks.ElementAt(step);
                        _currentLength =(int)( R_Peaks.ElementAt(step+1) - R_Peaks.ElementAt(step));                   

                    }
                    break;
                case (STATE.NEXT_CHANNEL):
                    if (_currentChannelIndex < NumberOfChannels)
                    {
                        //_currentChannelIndex++;
                        OutputData.QT_Intervals.Add(Tuple.Create(_currentLeadName, QT_Disp_Algorithms.QT_INTERVALS));
                        OutputData.T_End_Local.Add(Tuple.Create(_currentLeadName, QT_Disp_Algorithms.T_END_LOCAL));
                        OutputData.QT_mean.Add(Tuple.Create(_currentLeadName, QT_Disp_Algorithms.getMean()));
                        OutputData.QT_std.Add(Tuple.Create(_currentLeadName, QT_Disp_Algorithms.getStd()));
                        OutputData.QT_disp_local.Add(Tuple.Create(_currentLeadName,QT_Disp_Algorithms.getLocal()));

                        OutputWorker.SaveAttribute(Qt_Disp_Attributes.QT_disp_local, QT_Disp_Algorithms.getLocal());
                        OutputWorker.SaveAttribute(Qt_Disp_Attributes.QT_mean, QT_Disp_Algorithms.getMean());
                        OutputWorker.SaveAttribute(Qt_Disp_Attributes.QT_std, QT_Disp_Algorithms.getStd());

                        QT_Disp_Algorithms.DeleteQT_Intervals();

                        _state = STATE.BEGIN_CHANNEL;
                        step = 0;
                    }
                    else
                    {
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
            //int channel = _currentChannelIndex;
            //int startIndex = _samplesProcessed;
            //bool end = false;                       //if we exceed number of channels
            ////check if a channel is in a range
            //if (channel < NumberOfChannels)
            //{
            //    //here we check if we get the last element in R peak
            //    if (R_Peak_step >= InputRPeaksData.RPeaks[_currentChannelIndex].Item2.Count-1)
            //    {

            //        // here we creat a temp array to store a T_End index
            //        int[] temp = new int[R_Peak_step];
            //        //coping
            //        T_End_Index.CopyTo(temp);
            //        //adding calculated parameters for actual drain
            //        OutputData.QT_mean.Add(Tuple.Create(InputECGBaselineData.SignalsFiltered[_currentChannelIndex].Item1, getMean()));
            //        OutputData.QT_disp_local.Add(Tuple.Create(InputECGBaselineData.SignalsFiltered[_currentChannelIndex].Item1, getLocal()));
            //        OutputData.QT_std.Add(Tuple.Create(InputECGBaselineData.SignalsFiltered[_currentChannelIndex].Item1, getStd()));
            //        OutputData.T_End_Local.Add(Tuple.Create(InputECGBaselineData.SignalsFiltered[_currentChannelIndex].Item1, temp.ToList()));
            //        //saving data
            //        OutputWorker.Save(OutputData);
            //        //go to next channel
            //        _currentChannelIndex++;
            //        //check if we do not exceed the numbers of channels
            //        if (_currentChannelIndex < NumberOfChannels)
            //        {
            //            // here we clear our t_end_index from previous channel
            //            T_End_Index.Clear();
            //            // here we clear our qt interval list from previous channel
            //            DeleteQT_Intervals();
            //            // new start index 
            //            _samplesProcessed = (int)InputRPeaksData.RPeaks[_currentChannelIndex].Item2.ElementAt(0);
            //            // set new R peak step at begin
            //            R_Peak_step = 0;
            //            _currentChannelLength = InputECGBaselineData.SignalsFiltered[_currentChannelIndex].Item2.Count;
            //            _currentVector = Vector<double>.Build.Dense(_currentChannelLength);
            //            // get points from waves r peaks and basic data from current channel
            //            TODoInInit(InputWavesData.QRSOnsets[_currentChannelIndex].Item2, InputWavesData.TEnds[_currentChannelIndex].Item2,
            //                InputWavesData.QRSEnds[_currentChannelIndex].Item2, InputRPeaksData.RPeaks[_currentChannelIndex].Item2,
            //                Params.TEndMethod, Params.QTMethod, InputBasicData.Frequency);

            //        }
            //        else
            //        {
            //            //if we exceed number of channel we set true
            //            end = true;

            //        }
            //    }
            //    else
            //    {
            //        // we are here with every r peak 
            //        // get current vector a signal between actual R peak and previos
            //        _currentVector = InputECGBaselineData.SignalsFiltered[_currentChannelIndex].Item2.SubVector(startIndex, step);
            //        // filling T_End index
            //        T_End_Index.Add(ToDoInProccessData(_currentVector, R_Peak_step-1));
            //        // set new startIndex for next processdata calling
            //        _samplesProcessed = startIndex + step;
            //    }
            //}
            //else
            //{
            //    //if we finish 
            //    //calculate a qt disp global
            //    OutputData.QT_disp_global = CalculateQT_Disp(NumberOfChannels);
            //    //saving data
            //    OutputWorker.Save(OutputData);
            //    _ended = true;
            //    end = true;
            //}
            //if (!end)
            //{
            //    //we are here with every processdata calling excepting the p
            //    R_Peak_step += 1;
            //    //only for tests
            //    /*Console.WriteLine("R Peak index:\t\t" + R_Peak_step);
            //    Console.WriteLine("Current channel:\t" + _currentChannelIndex);*/
            //    //
            //    //set a new step
            //    step = (int)InputRPeaksData.RPeaks[_currentChannelIndex].Item2[R_Peak_step] - (int)InputRPeaksData.RPeaks[_currentChannelIndex].Item2[R_Peak_step - 1];
            //}
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
        public List<Tuple<string, List<int>>> T_End_Local
        {
            get
            {
                return _t_end_loacl;
            }
            set
            {
                _t_end_loacl = value;
            }
        }
        public List<Tuple<string,List<double>>> QT_Intervals
        {
            get
            {
                return _qt_intervals;
            }
            set
            {
                _qt_intervals = value;
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
        /*public static void Main()
        {
            QT_Disp_Params param = new QT_Disp_Params();
            param.AnalysisName = "QtTest2";
            QT_Disp testModule = new QT_Disp();
            testModule.Init(param);
            while (true)
            {
                //Console.WriteLine("Press key to continue...");
                //Console.ReadKey();
                if (testModule.Ended()) break;
                Console.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }
            Console.WriteLine("Finish");
            Console.ReadKey();         

        }*/
        public static void Main(String[] args)
        {
            QT_Disp_Stats stats = new QT_Disp_Stats();
            stats.Init("TestAnalysis100");


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

        }



    }
}
