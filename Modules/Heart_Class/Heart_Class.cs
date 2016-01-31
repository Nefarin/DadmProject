
using System;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using System.Collections.Generic;
using System.Linq;
using EKG_Project.Modules.ECG_Baseline;
using EKG_Project.Modules.R_Peaks;
using EKG_Project.Modules.Waves;

namespace EKG_Project.Modules.Heart_Class
{
    public class Heart_Class : IModule
    {
        private enum STATE { INIT, BEGIN_CHANNEL, PROCESS_FIRST_STEP, PROCESS_CHANNEL, NEXT_CHANNEL, END_CHANNEL, END };
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _samplesProcessed;
        private int _numberOfChannels;
        private string _currentLeadName;
        private string[] _leads;
        private string _leadNameChannel2;
        private int _currentIndex;
        private uint _fs;
        private int _numberProcessedComplexes;

        private int _channel2;
        private int startIndex;
        private bool _ml2Processed;
        private int _step;
        private int _numberOfSteps;
        private uint[] _numberOfStepsArray;

        private Basic_New_Data_Worker _inputBasicWorker;
        private ECG_Baseline_New_Data_Worker _inputECGbaselineWorker;
        private R_Peaks_New_Data_Worker _inputRpeaksWorker;
        private Waves_New_Data_Worker _inputWavesWorker;
        private Heart_Class_New_Data_Worker _outputWorker;

        private Heart_Class_Data _outputData;
        private ECG_Baseline_Data _inputECGbaselineData;
        private R_Peaks_Data _inputRpeaksData;
        private Waves_Data _inputWavesData;
        //private Heart_Class_Data _outputData;
        private Basic_New_Data _basicData;
        private Heart_Class_Params _params;

        private Heart_Class_Alg _alg;
        private Vector<Double> _currentVector;
        private Vector<double> _currentRVector;
        private Vector<double> _currentECGBaselineVector;
        private STATE _state;



        //private ECG_Baseline_Data_Worker _inputECGbaselineWorker;
        //private R_Peaks_Data_Worker _inputRpeaksWorker;
        //private Waves_Data_Worker _inputWavesWorker;
        //private Heart_Class_Data_Worker _outputWorker;
        //private Basic_Data_Worker _basicDataWorker;


        //private Heart_Class_Params _params;

        //private Vector<Double> _currentVector;
        private Tuple<int, int> _tempClassResult;



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
                Params = parameters as Heart_Class_Params;
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
                _ended = false;

                InputBasicWorker = new Basic_New_Data_Worker(Params.AnalysisName);
                BasicData = new Basic_New_Data();
                //_fs = BasicData.Frequency; //? czy tu mam ten sygnal, czy nie. Z nieba się nie bierze?
                _fs = InputBasicWorker.LoadAttribute(Basic_Attributes.Frequency);
                InputEcGbaselineWorker = new ECG_Baseline_New_Data_Worker(Params.AnalysisName);
                InputECGbaselineData = new ECG_Baseline_Data();
                OutputData = new Heart_Class_Data();

                _leads = InputBasicWorker.LoadLeads().ToArray();
                _numberProcessedComplexes = 1;

                if (findChannel()) //? - jak dostać sie do sygnalu?
                {
                    InputRpeaksWorker = new R_Peaks_New_Data_Worker(Params.AnalysisName);
                    InputRpeaksData = new R_Peaks_Data();

                    InputWavesWorker = new Waves_New_Data_Worker(Params.AnalysisName);
                    InputWavesData = new Waves_Data();

                    //czy to ponizej ma sens?
                    _currentRVector = InputRpeaksData.RPeaks[_channel2].Item2;
                    _currentECGBaselineVector = InputECGbaselineData.SignalsFiltered[_channel2].Item2;

                    OutputWorker = new Heart_Class_New_Data_Worker(Params.AnalysisName);

                    _currentChannelIndex = 0;
                    startIndex = _samplesProcessed;
                    NumberOfChannels = InputECGbaselineData.SignalsFiltered.Count;
                    _currentChannelLength = InputECGbaselineData.SignalsFiltered[_currentChannelIndex].Item2.Count;
                    _currentVector = Vector<Double>.Build.Dense(_currentChannelLength);
                    _leadNameChannel2 = _leads[_channel2];

                    // If sth is wrong in earlier modules, this is an insurance :
                    // to tak nei działa
                    _numberOfStepsArray = new uint[3];
                    _numberOfStepsArray[0] = InputWavesWorker.getNumberOfSamples(Waves_Signal.QRSOnsets, _leadNameChannel2);
                    _numberOfStepsArray[1] = InputWavesWorker.getNumberOfSamples(Waves_Signal.QRSEnds, _leadNameChannel2);
                    _numberOfStepsArray[2] = InputRpeaksWorker.getNumberOfSamples(R_Peaks_Attributes.RPeaks, _leadNameChannel2);

                    _numberOfSteps = (int)_numberOfStepsArray.Min();
                    

                    _tempClassResult = new Tuple<int, int>(0, 0);
                    _ml2Processed = false;
                    _state = STATE.INIT;

                }
                else
                {
                    _ended = true;
                    _aborted = true;
                }
                
            }
            
            

            //Aborted = false;
            //if (!Runnable()) _ended = true;
            //else
            //{
            //    _ended = false;

            //    InputEcGbaselineWorker = new ECG_Baseline_Data_Worker(Params.AnalysisName);
            //    InputEcGbaselineWorker.Load();
            //    InputECGbaselineData = InputEcGbaselineWorker.Data;
            //    OutputData = new Heart_Class_Data();

            //    if (findChannel())
            //    {
            //        InputRpeaksWorker = new R_Peaks_Data_Worker(Params.AnalysisName);
            //        InputRpeaksWorker.Load();
            //        InputRpeaksData = InputRpeaksWorker.Data;

            //        BasicDataWorker = new Basic_Data_Worker(Params.AnalysisName);
            //        BasicDataWorker.Load();
            //        BasicData = BasicDataWorker.BasicData;
            //        fs = BasicData.Frequency;

            //        InputWavesWorker = new Waves_Data_Worker(Params.AnalysisName);
            //        InputWavesWorker.Load();
            //        InputWavesData = InputWavesWorker.Data;
            //        _currentRVector = InputRpeaksData.RPeaks[_channel2].Item2;
            //        _currentECGBaselineVector = InputECGbaselineData.SignalsFiltered[_channel2].Item2;
                   
            //        OutputWorker = new Heart_Class_Data_Worker(Params.AnalysisName);
              
            //        _currentChannelIndex = 0;
            //        _samplesProcessed = 0;
            //        startIndex = _samplesProcessed;
            //        NumberOfChannels = InputECGbaselineData.SignalsFiltered.Count;
            //        _currentChannelLength = InputECGbaselineData.SignalsFiltered[_currentChannelIndex].Item2.Count;
            //        _currentVector = Vector<Double>.Build.Dense(_currentChannelLength);


            //        _numberOfStepsArray = new int[3];
            //        _numberOfStepsArray[0]= InputWavesData.QRSEnds[_channel2].Item2.Count;
            //        _numberOfStepsArray[1] = InputWavesData.QRSOnsets[_channel2].Item2.Count;
            //        _numberOfStepsArray[2] = InputRpeaksData.RPeaks[_channel2].Item2.Count;

            //        _numberOfSteps = _numberOfStepsArray.Min();

            //        _tempClassResult = new Tuple<int, int>(0,0);
            //        _ml2Processed = false;

            //    }
            //    else
            //    {
            //        _ended = true;
            //        _aborted = true;
            //    }
            //}
            
        }

        public void ProcessData()
        {
            if (Runnable()) processData();
            else _ended = true;
        }
        
        public double Progress()
        {

            return 100.0*_samplesProcessed/_numberOfSteps;
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
                    _numberOfChannels = _leads.Length;
                    _alg = new Heart_Class_Alg();
                    _state = STATE.BEGIN_CHANNEL;
                    break;
                case (STATE.BEGIN_CHANNEL):
                    _currentChannelIndex ++;
                    _currentLeadName = _leads[_channel2];
                    _currentChannelLength = (int)InputBasicWorker.getNumberOfSamples(_currentLeadName);
                    _currentIndex = 0;
                    _samplesProcessed = 0;

                    //nweralgiczny punkt, sprawdzić:
                    _step = InputWavesWorker.LoadSignal(Waves_Signal.QRSEnds, _currentLeadName, _samplesProcessed, _numberProcessedComplexes)[0];
                    _step = _step + 1; //aby ostatnią załadowaną próbką był QRSEND
                    _state = STATE.PROCESS_FIRST_STEP;
                    break;
                case (STATE.PROCESS_FIRST_STEP):
                    if (!_ml2Processed)
                    {
                        // Ogólnie:
                        // 1. pobrać wcześniej ilość wszystkich R, QRSonset, QRSend
                        // 2. _step = OrsEnd - _currentIndex;
                        // 3. _currentIndex = QRSEnd w tym samym stanie i sprawdzić czy QrsEnd nie przekroczyło max ilości, jak tak to stan END.Channel
                        // 4. Nowy obieg pętli: nowy step - kolejny
                        // smaplesProcessed jest do kolejnych elementow R, QRSOnsets, QRSend
                        // currentIndex jest do pobierania sygnału z ECGBaseline
                        int QRSOnSet = InputWavesWorker.LoadSignal(Waves_Signal.QRSOnsets, _currentLeadName, _samplesProcessed,
                                _numberProcessedComplexes)[0];
                        int QRSEnds = InputWavesWorker.LoadSignal(Waves_Signal.QRSEnds, _currentLeadName, _samplesProcessed,
                                _numberProcessedComplexes)[0];
                        double R = InputRpeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, _samplesProcessed,
                                _numberProcessedComplexes)[0];
                        _currentVector = InputEcGbaselineWorker.LoadSignal(_currentLeadName, _currentIndex, _step);


                        //klasyfikacja




                        _currentIndex += _step;
                        _samplesProcessed++;
                        _state = STATE.PROCESS_CHANNEL;

                        if (_samplesProcessed >= _numberOfSteps)
                        {
                            _ml2Processed = true;
                        }

                    }
                    else
                    {
                        _state = STATE.END_CHANNEL;
                    }

                        break;
                case (STATE.PROCESS_CHANNEL):
                    if (!_ml2Processed)
                    {
                        _step = (InputWavesWorker.LoadSignal(Waves_Signal.QRSEnds, _currentLeadName, _samplesProcessed, _numberProcessedComplexes)[0]) - _currentIndex;
                        _step = _step + 1; //zeby znowu skopiować az do QRSend włącznie
         
                        int QRSOnSet = InputWavesWorker.LoadSignal(Waves_Signal.QRSOnsets, _currentLeadName, _samplesProcessed,
                                _numberProcessedComplexes)[0];
                        int QRSEnds = InputWavesWorker.LoadSignal(Waves_Signal.QRSEnds, _currentLeadName, _samplesProcessed,
                                _numberProcessedComplexes)[0];
                        double R = InputRpeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, _samplesProcessed,
                                _numberProcessedComplexes)[0];
                        _currentVector = InputEcGbaselineWorker.LoadSignal(_currentLeadName, _currentIndex, _step);


                        //klasyfikacja




                        _currentIndex += _step;
                        _samplesProcessed++;
                        _state = STATE.PROCESS_CHANNEL;

                        if (_samplesProcessed >= _numberOfSteps)
                        {
                            _ml2Processed = true;
                        }

                    }
                    else
                    {
                        _state = STATE.END_CHANNEL;
                    }




                    break;
                case (STATE.END_CHANNEL):



                    break;
                case (STATE.NEXT_CHANNEL):



                    break;
                case (STATE.END):
                    _ended = true;
                    break;
                default:
                    Abort();
                    break;
            }

            //if (!_ml2Processed)
                    //{
                    //    int QRSOnSet = InputWavesData.QRSOnsets[_channel2].Item2[_samplesProcessed];
                    //    int QRSEnds = InputWavesData.QRSEnds[_channel2].Item2[_samplesProcessed];
                    //    double R = InputRpeaksData.RPeaks[_channel2].Item2[_samplesProcessed];

                    //    if (QRSOnSet == -1 || QRSEnds == -1)
                    //    {
                    //        _samplesProcessed++;
                    //    }
                    //    else
                    //    {
                    //        _tempClassResult = Classification(InputECGbaselineData.SignalsFiltered[_channel2].Item2,
                    //                            QRSOnSet, QRSEnds, R); // UWAGA -> dodać do argumentów fs !!! bo jak nie to exceptiony!
                    //        OutputData.ClassificationResult.Add(_tempClassResult);

                    //        _samplesProcessed++;
                    //    }

                    //    if (_samplesProcessed >= _numberOfSteps)
                    //    {
                    //        _ml2Processed = true;
                    //    }
                    //}
                    //else
                    //{
                    //    //Console.WriteLine("   ilsc step  "+_numberOfSteps);
                    //    _ended = true;
                    //    OutputWorker.Save(OutputData);
                    //}

            }

        private bool findChannel()
        {
            int i = 0;
            
            foreach (var value in _leads)
            {
                string name = value;
                if (name == "MLII" || name == "II")
                {
                    Channel2 = i;
                    OutputData.ChannelMliiDetected = true;
                    return true;
                }
                i++;
            }
            OutputData.ChannelMliiDetected = false;
            return false;
        }

        public Heart_Class_New_Data_Worker OutputWorker
        {
            get { return _outputWorker; }
            set { _outputWorker = value; }
        }
        
        public bool Aborted
        {
            get { return _aborted; }
            set { _aborted = value; }
        }

        public Heart_Class_Params Params
        {
            get { return _params; }
            set { _params = value; }
        }
       
        public Heart_Class_Data OutputData
        {
            get { return _outputData; }
            set { _outputData = value; }
        }
        


        public ECG_Baseline_New_Data_Worker InputEcGbaselineWorker
        {
            get { return _inputECGbaselineWorker; }
            set { _inputECGbaselineWorker = value; }
        }

        public R_Peaks_New_Data_Worker InputRpeaksWorker
        {
            get { return _inputRpeaksWorker; }
            set { _inputRpeaksWorker = value; }
        }

        public Waves_New_Data_Worker InputWavesWorker
        {
            get { return _inputWavesWorker; }
            set { _inputWavesWorker = value; }
        }

        public ECG_Baseline_Data InputECGbaselineData
        {
            get { return _inputECGbaselineData; }
            set { _inputECGbaselineData = value; }
        }

        public R_Peaks_Data InputRpeaksData
        {
            get { return _inputRpeaksData; }
            set { _inputRpeaksData = value; }
        }

        public Waves_Data InputWavesData
        {
            get { return _inputWavesData; }
            set { _inputWavesData = value; }
        }

        public int NumberOfChannels
        {
            get { return _numberOfChannels; }
            set { _numberOfChannels = value; }
        }

        public int Channel2
        {
            get { return _channel2; }
            set { _channel2 = value; }
        }

        public Basic_New_Data_Worker InputBasicWorker
        {
            get { return _inputBasicWorker; }
            set { _inputBasicWorker = value; }
        }

        public Basic_New_Data BasicData
        {
            get { return _basicData; }
            set { _basicData = value; }
        }

        
        public static void Main()
        {
            Heart_Class_Params param = new Heart_Class_Params("TestAnalysis6"); // "Analysis6");
            Heart_Class testModule = new Heart_Class();
            testModule.Init(param);


            while (true)
            {
                Console.WriteLine("Press key to continue.");
                Console.Read();
                if (testModule.Ended()) break;
                Console.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }

        }
        
    }
}
