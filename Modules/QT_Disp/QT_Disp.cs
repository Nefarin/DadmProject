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
    public partial class QT_Disp : IModule
    {
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _samplesProcessed;
        private int _numberOfChannels;

        int step = 0;
        int R_Peak_step = 1;

        //input workers
        private ECG_Baseline_Data_Worker _inputECGBaselineWorker;
        private R_Peaks_Data_Worker _inputRPeaksWorker;
        private Waves_Data_Worker _inputWavesWorker;
        private Basic_Data_Worker _inputBasicWorker;

        //output worker
        private QT_Disp_Data_Worker _outputWorker;

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
            Params = parameters as QT_Disp_Params;
            Aborted = false;
            if (!Runnable()) _ended = true;
            else
            {
                //input workers
                InputECGBaselineWorker = new ECG_Baseline_Data_Worker(Params.AnalysisName);
                InputECGBaselineWorker.Load();
                InputECGBaselineData = InputECGBaselineWorker.Data;

                InputRPeaksWorker = new R_Peaks_Data_Worker(Params.AnalysisName);
                InputRPeaksWorker.Load();
                InputRPeaksData = InputRPeaksWorker.Data;

                InputWavesWorker = new Waves_Data_Worker(Params.AnalysisName);
                InputWavesWorker.Load();
                InputWavesData = InputWavesWorker.Data;

                InputBasicWorker = new Basic_Data_Worker(Params.AnalysisName);
                InputBasicWorker.Load();
                InputBasicData = InputBasicWorker.BasicData;
                //output workers

                OutputWorker = new QT_Disp_Data_Worker(Params.AnalysisName);
                OutputWorker.Load();
                OutputData = new QT_Disp_Data();

                _currentChannelIndex = 0;
            
                //Console.WriteLine(InputRPeaksData.RPeaks[_currentChannelIndex].Item2.Count);
                _samplesProcessed = (int)InputRPeaksData.RPeaks[_currentChannelIndex].Item2.ElementAt(0);
                NumberOfChannels = InputECGBaselineData.SignalsFiltered.Count;
                _currentChannelLength = InputECGBaselineData.SignalsFiltered[_currentChannelIndex].Item2.Count;
                //Console.WriteLine(_currentChannelLength);
                _currentVector = Vector<double>.Build.Dense(_currentChannelLength);
                step = (int)InputRPeaksData.RPeaks[_currentChannelIndex].Item2.ElementAt(1)- (int)InputRPeaksData.RPeaks[_currentChannelIndex].Item2.ElementAt(0);
                TODoInInit(InputWavesData.QRSOnsets[_currentChannelIndex].Item2, InputWavesData.TEnds[_currentChannelIndex].Item2,
                    InputWavesData.QRSEnds[_currentChannelIndex].Item2, InputRPeaksData.RPeaks[_currentChannelIndex].Item2,
                    Params.TEndMethod, Params.QTMethod, InputBasicData.Frequency);
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
            int channel = _currentChannelIndex;
            int startIndex = _samplesProcessed;
            bool end = false;
            if (channel < NumberOfChannels)
            {
                if (R_Peak_step >= InputRPeaksData.RPeaks[_currentChannelIndex].Item2.Count-1)
                {
                    //Console.WriteLine("Jestem tutraj");
                    int[] temp = new int[R_Peak_step];
                    T_End_Index.CopyTo(temp);
                    OutputData.QT_mean.Add(Tuple.Create(InputECGBaselineData.SignalsFiltered[_currentChannelIndex].Item1, getMean()));
                    OutputData.QT_disp_local.Add(Tuple.Create(InputECGBaselineData.SignalsFiltered[_currentChannelIndex].Item1, getLocal()));
                    OutputData.QT_std.Add(Tuple.Create(InputECGBaselineData.SignalsFiltered[_currentChannelIndex].Item1, getStd()));
                    OutputData.T_End_Local.Add(Tuple.Create(InputECGBaselineData.SignalsFiltered[_currentChannelIndex].Item1, temp.ToList()));
                    OutputWorker.Save(OutputData);
                    _currentChannelIndex++;
                    if (_currentChannelIndex < NumberOfChannels)
                    {
                        T_End_Index.Clear();
                        DeleteQT_Intervals();
                        _samplesProcessed = (int)InputRPeaksData.RPeaks[_currentChannelIndex].Item2.ElementAt(0);
                        R_Peak_step = 0;
                        _currentChannelLength = InputECGBaselineData.SignalsFiltered[_currentChannelIndex].Item2.Count;
                        _currentVector = Vector<double>.Build.Dense(_currentChannelLength);
                        TODoInInit(InputWavesData.QRSOnsets[_currentChannelIndex].Item2, InputWavesData.TEnds[_currentChannelIndex].Item2,
                            InputWavesData.QRSEnds[_currentChannelIndex].Item2, InputRPeaksData.RPeaks[_currentChannelIndex].Item2,
                            Params.TEndMethod, Params.QTMethod, InputBasicData.Frequency);

                    }
                    else
                    {
                        end = true;

                    }
                }
                else
                {
                    _currentVector = InputECGBaselineData.SignalsFiltered[_currentChannelIndex].Item2.SubVector(startIndex, step);
                    T_End_Index.Add(ToDoInProccessData(_currentVector, R_Peak_step-1));
                    _samplesProcessed = startIndex + step;
                }
            }
            else
            {
                OutputData.QT_disp_global = CalculateQT_Disp(NumberOfChannels);
                OutputWorker.Save(OutputData);
                _ended = true;
                end = true;
            }
            if (!end)
            {
                R_Peak_step += 1;
                Console.WriteLine("R Peak index:\t\t" + R_Peak_step);
                Console.WriteLine("Current channel:\t" + _currentChannelIndex);
                //Console.WriteLine(_samplesProcessed);
                step = (int)InputRPeaksData.RPeaks[_currentChannelIndex].Item2[R_Peak_step] - (int)InputRPeaksData.RPeaks[_currentChannelIndex].Item2[R_Peak_step - 1];


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
        public List<int> T_End_Index
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


        //input workers
        public ECG_Baseline_Data_Worker InputECGBaselineWorker
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
        public Waves_Data_Worker InputWavesWorker
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
        public R_Peaks_Data_Worker InputRPeaksWorker
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
        public Basic_Data_Worker InputBasicWorker
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
        public QT_Disp_Data_Worker OutputWorker
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
        public static void Main()
        {
            QT_Disp_Params param = new QT_Disp_Params();
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
           

        }



    }
}
