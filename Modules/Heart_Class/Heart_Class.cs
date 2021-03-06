﻿using System;
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
        private enum STATE { INIT, BEGIN_CHANNEL, LOAD_PART_OF_SIGNAL, PROCESS_FIRST_STEP, PROCESS_CHANNEL, NEXT_CHANNEL, END_CHANNEL, END };
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
        private int _numberOfClassifiedComplexes;

        private int _channel2;
        private bool _ml2Processed;

        private int _numberOfSteps;

        //modyfikacja:
        private int _totalNumberOfR;
        private int _step;
        private Vector<double> _arrayR;
        private List<int> _arrayQRSOnSet;
        private List<int> _arrayQRSEnds;
        private int _numberProcessedComplexesCounter;
        private int _numberOfSignalLoading;
        private bool _lastLoading;

        private uint[] _numberOfStepsArray;
        private List<Vector<double>> _trainDataList;
        private List<int> _trainClass;

        private Basic_New_Data_Worker _inputBasicWorker;
        private ECG_Baseline_New_Data_Worker _inputECGbaselineWorker;
        private R_Peaks_New_Data_Worker _inputRpeaksWorker;
        private Waves_New_Data_Worker _inputWavesWorker;
        private Heart_Class_New_Data_Worker _outputWorker;


        private Heart_Class_Params _params;

        private Heart_Class_Alg _alg;
        private Vector<Double> _currentVector;
        private STATE _state;
        private List<Tuple<int, int>> _tempClassResult;
        private Tuple<int, int> _tempTuple;

        //dorobion:
        private int _numberOfRpeaks;
        private List<int> _QRSOnSetList;
        private List<int> _QRSEndsList;
        private Vector<double> _RVector;



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
                _fs = InputBasicWorker.LoadAttribute(Basic_Attributes.Frequency);
                InputEcGbaselineWorker = new ECG_Baseline_New_Data_Worker(Params.AnalysisName);


                _leads = InputBasicWorker.LoadLeads().ToArray();
                

                if (findChannel())
                {
                    InputRpeaksWorker = new R_Peaks_New_Data_Worker(Params.AnalysisName);
                    InputWavesWorker = new Waves_New_Data_Worker(Params.AnalysisName);
                    OutputWorker = new Heart_Class_New_Data_Worker(Params.AnalysisName);

                    _currentChannelIndex = -1;
                    _leadNameChannel2 = _leads[_channel2];
                    _numberProcessedComplexes = 500;

                    // ubezpieczenie się na wypadek błedów w wyższych modułach (nierówna ilośc próbek w Rpeaks, albo QRSonsets, QRSends)

                    _numberOfStepsArray = new uint[3];
                    _numberOfStepsArray[0] = InputWavesWorker.getNumberOfSamples(Waves_Signal.QRSOnsets, _leadNameChannel2);
                    _numberOfStepsArray[1] = InputWavesWorker.getNumberOfSamples(Waves_Signal.QRSEnds, _leadNameChannel2);
                    _numberOfStepsArray[2] = InputRpeaksWorker.getNumberOfSamples(R_Peaks_Attributes.RPeaks, _leadNameChannel2);

                    _numberOfSteps = (int)_numberOfStepsArray.Min();
                    Console.WriteLine(_numberOfSteps);
                    _totalNumberOfR = (int)_numberOfStepsArray[2];

                    if (_numberOfSteps < _numberProcessedComplexes)
                    {
                        _numberProcessedComplexes = _numberOfSteps;
                    }

                    OutputWorker.SaveChannelMliiDetected(true);
                    _ml2Processed = false;
                    _state = STATE.INIT;

                }
                else
                {
                    OutputWorker = new Heart_Class_New_Data_Worker(Params.AnalysisName);
                    OutputWorker.SaveChannelMliiDetected(false);
                    _ended = true;
                    _aborted = true;

                }

            }


        }

        public void ProcessData()
        {
            if (Runnable()) processData();
            else _ended = true;
        }

        public double Progress()
        {

            return 100.0 * _samplesProcessed / _numberOfSteps;
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

                    _numberOfChannels = _leads.Length;
                    _alg = new Heart_Class_Alg();

                    //to było w BEGIN CHANNEL, ale go usunęłam i przeniosłam tu:
                    _currentChannelIndex++;
                    _currentLeadName = _leads[_channel2];
                    _currentChannelLength = (int)InputBasicWorker.getNumberOfSamples(_currentLeadName); //to potrzebuje? Chyba nie
                    _currentIndex = 0;
                    _samplesProcessed = 0;
                    _numberProcessedComplexesCounter = 0;
                    _numberOfSignalLoading = 0;
                    _lastLoading = false;

                    //WCZYTANIE ZBIORU TRENINGOWEGO
                    DebugECGPath loader = new DebugECGPath();
                    _trainDataList = new List<Vector<double>>(
                        _alg.loadFile(System.IO.Path.Combine(loader.getTempPath(), "train_d.txt")));

                    //WCZYTANIE ETYKIET ZBIORU TRENINGOWEGO: 0-V, 1-SV
                    List<Vector<double>> trainClassList =
                        _alg.loadFile(System.IO.Path.Combine(loader.getTempPath(), "train_d_label.txt"));

                    int oneClassElement;
                    _trainClass = new List<int>();
                    foreach (var item in trainClassList)
                    {
                        foreach (var element in item)
                        {
                            oneClassElement = (int)element;
                            _trainClass.Add(oneClassElement);
                        }

                    }

                    _state = STATE.LOAD_PART_OF_SIGNAL;
                    break;

                case (STATE.LOAD_PART_OF_SIGNAL):

                    if (!_lastLoading)
                    {
                        _arrayR = InputRpeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName,
                            _samplesProcessed, _numberProcessedComplexes);
                        _arrayQRSOnSet =
                            new List<int>(InputWavesWorker.LoadSignal(Waves_Signal.QRSOnsets, _currentLeadName,
                                _samplesProcessed,_numberProcessedComplexes));
                        _arrayQRSEnds =
                            new List<int>(InputWavesWorker.LoadSignal(Waves_Signal.QRSEnds, _currentLeadName,
                                _samplesProcessed,_numberProcessedComplexes));

                        _step = (int)_arrayR[_numberProcessedComplexes - 1] - _currentIndex;
                        _currentVector = InputEcGbaselineWorker.LoadSignal(_currentLeadName, _currentIndex, _step);

                        _numberOfSignalLoading++;
                        _numberProcessedComplexesCounter = 0;
                        _state = STATE.PROCESS_CHANNEL;
                    }
                    else
                    {
                        _numberProcessedComplexesCounter = 0;
                        _arrayR = InputRpeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName,
                              _samplesProcessed,
                              (_totalNumberOfR) - _samplesProcessed);
                        _arrayQRSOnSet =
                            new List<int>(InputWavesWorker.LoadSignal(Waves_Signal.QRSOnsets, _currentLeadName,
                                _samplesProcessed,
                                _numberOfSteps - _samplesProcessed));
                        _arrayQRSEnds =
                            new List<int>(InputWavesWorker.LoadSignal(Waves_Signal.QRSEnds, _currentLeadName,
                                _samplesProcessed,
                                _numberOfSteps - _samplesProcessed));

                        _currentVector = InputEcGbaselineWorker.LoadSignal(_currentLeadName, _currentIndex,
                            _currentChannelLength - _currentIndex);


                        _state = STATE.END_CHANNEL;
                    }

                    break;

                case (STATE.PROCESS_CHANNEL):
                    if (!_ml2Processed)
                    {
                        if ((_numberProcessedComplexesCounter + 1) < _numberProcessedComplexes)
                        {
                            double Ractual = _arrayR[_numberProcessedComplexesCounter] - _currentIndex;
                            int QRSOnSetActual = _arrayQRSOnSet[_numberProcessedComplexesCounter] - _currentIndex;
                            int QRSEndActual = _arrayQRSEnds[_numberProcessedComplexesCounter] - _currentIndex;

                            _alg = new Heart_Class_Alg();
                            _tempClassResult = new List<Tuple<int, int>>();
                            _tempClassResult.Add(_alg.Classification(_currentVector, QRSOnSetActual, QRSEndActual, Ractual, _fs,
                                _trainDataList, _trainClass));

                            //powrót do R rzeczywistego, a nie tego zmodyfikowanego na potrzeby cięcia sygnału
                            int classResult = _tempClassResult[0].Item2;
                            int realR = (int)Ractual + _currentIndex;
                            _tempTuple = new Tuple<int, int>((int)realR, classResult);
                            _tempClassResult = new List<Tuple<int, int>>();
                            _tempClassResult.Add(_tempTuple);
                            OutputWorker.SaveClassificationResult(_currentLeadName, true, _tempClassResult);


                            _samplesProcessed++;
                            _numberProcessedComplexesCounter++;
                            _state = STATE.PROCESS_CHANNEL;

                            if (_samplesProcessed + 2 >= _numberOfSteps)
                            {
                                _ml2Processed = true;
                            }
                        }
                        else
                        {
                            if (((_numberOfSignalLoading + 1) * _numberProcessedComplexes) < _totalNumberOfR)
                            {
                                _currentIndex = (int)_arrayR[(_numberProcessedComplexesCounter - 1)];
                                _state = STATE.LOAD_PART_OF_SIGNAL;
                            }
                            else
                            {
                                _lastLoading = true;
                                _state = STATE.LOAD_PART_OF_SIGNAL;
                            }

                        }


                    }
                    else
                    {
                        _state = STATE.END_CHANNEL;
                    }


                    break;
                case (STATE.END_CHANNEL):

                    if (!_ml2Processed)
                    {
                        double Ractual = _arrayR[_numberProcessedComplexesCounter] - _currentIndex;
                        int QRSOnSetActual = _arrayQRSOnSet[_numberProcessedComplexesCounter] - _currentIndex;
                        int QRSEndActual = _arrayQRSEnds[_numberProcessedComplexesCounter] - _currentIndex;


                        _alg = new Heart_Class_Alg();
                        _tempClassResult = new List<Tuple<int, int>>();
                        _tempClassResult.Add(_alg.Classification(_currentVector, QRSOnSetActual, QRSEndActual, Ractual,
                            _fs,
                            _trainDataList, _trainClass));

                        //powrót do R rzeczywistego, a nie tego zmodyfikowanego na potrzeby cięcia sygnału
                        int classResult = _tempClassResult[0].Item2;
                        int realR = (int)Ractual + _currentIndex;
                        _tempTuple = new Tuple<int, int>((int)realR, classResult);
                        _tempClassResult = new List<Tuple<int, int>>();
                        _tempClassResult.Add(_tempTuple);
                        OutputWorker.SaveClassificationResult(_currentLeadName, true, _tempClassResult);

                        _samplesProcessed++;
                        _numberProcessedComplexesCounter++;
                        _state = STATE.END_CHANNEL;
                        if (_samplesProcessed + 2 >= _numberOfSteps)
                        {
                            _ml2Processed = true;
                        }

                    }
                    else
                    {
                        _state = STATE.END;
                    }



                    break;
                case (STATE.NEXT_CHANNEL):

                    break;
                case (STATE.END):

                    //przepisanie wyników na inne kanały

                    int startIndex = 0;
                    _tempClassResult = new List<Tuple<int, int>>();
                    _numberOfClassifiedComplexes = (int)OutputWorker.getNumberOfSamples(_leadNameChannel2);

                    _tempClassResult = OutputWorker.LoadClassificationResult(_currentLeadName, startIndex,
                        _numberOfClassifiedComplexes);
                    for (int i = 0; i < _numberOfChannels; i++)
                    {
                        _currentLeadName = _leads[i];

                        if (_currentLeadName != _leadNameChannel2)
                        {
                            OutputWorker.SaveClassificationResult(_currentLeadName, true, _tempClassResult);
                        }

                    }
                    _ended = true;
                    break;
                default:
                    Abort();
                    break;
            }

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
                    return true;
                }
                i++;
            }
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


        public static void Main(String[] args)
        {
            IModule testModule = new EKG_Project.Modules.Heart_Class.Heart_Class();
            Heart_Class_Params param = new Heart_Class_Params("Analysis106");

            testModule.Init(param);
            while (!testModule.Ended())
            {
                testModule.ProcessData();
                Console.WriteLine(testModule.Progress());
            }

            Console.ReadKey();
        }

    }
}