
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
        private int _numberOfClassifiedComplexes;

        private int _channel2;
        private bool _ml2Processed;
        private int _step;
        private int _numberOfSteps;
        private uint[] _numberOfStepsArray;

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
                _numberProcessedComplexes = 1;

                if (findChannel()) //? - jak dostać sie do sygnalu?
                {
                    InputRpeaksWorker = new R_Peaks_New_Data_Worker(Params.AnalysisName);
                    InputWavesWorker = new Waves_New_Data_Worker(Params.AnalysisName);
                    OutputWorker = new Heart_Class_New_Data_Worker(Params.AnalysisName);

                    _currentChannelIndex = 0;
                    _leadNameChannel2 = _leads[_channel2];

                    // If sth is wrong in earlier modules, this is an insurance :
                    // to tak nei działa
                    _numberOfStepsArray = new uint[3];
                    _numberOfStepsArray[0] = InputWavesWorker.getNumberOfSamples(Waves_Signal.QRSOnsets, _leadNameChannel2);
                    _numberOfStepsArray[1] = InputWavesWorker.getNumberOfSamples(Waves_Signal.QRSEnds, _leadNameChannel2);
                    _numberOfStepsArray[2] = InputRpeaksWorker.getNumberOfSamples(R_Peaks_Attributes.RPeaks, _leadNameChannel2);

                    _numberOfSteps = (int)_numberOfStepsArray.Min();

                    OutputWorker.SaveChannelMliiDetected(true);
                    _ml2Processed = false;
                    _state = STATE.INIT;

                }
                else
                {
                    _ended = true;
                    _aborted = true;
                    OutputWorker.SaveChannelMliiDetected(false);
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
                    _currentChannelLength = (int)InputBasicWorker.getNumberOfSamples(_currentLeadName); //to potrzebuje? Chyba nie
                    _currentIndex = 0;
                    _samplesProcessed = 0;

                    //step zawiera odległość pom początkiem sygnału a 2 r_peakiem w kolejności. 
                    _step = (int)InputRpeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, (_samplesProcessed+1), _numberProcessedComplexes)[0];
                    Console.WriteLine("/// step : "+ _step);
                    _state = STATE.PROCESS_FIRST_STEP;
                    break;
                case (STATE.PROCESS_FIRST_STEP):
                    if (!_ml2Processed)
                    {
                        // Ogólnie, jak procesuję sygnał:
                        // Biorę pierwszą porcję od 0 (currentIndex) do drugiego wykrytego Rpeaka (samplesProcessed+1). Żeby mieć pewność ze taka porcja sygnału pokryje QRSOnset, R, QRSend dla pierwszego zespołu.

                        int QRSOnSet = InputWavesWorker.LoadSignal(Waves_Signal.QRSOnsets, _currentLeadName, _samplesProcessed,
                                _numberProcessedComplexes)[0];
                        int QRSEnds = InputWavesWorker.LoadSignal(Waves_Signal.QRSEnds, _currentLeadName, _samplesProcessed,
                                _numberProcessedComplexes)[0];
                        double R = InputRpeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, _samplesProcessed,
                                _numberProcessedComplexes)[0];
                        _currentVector = InputEcGbaselineWorker.LoadSignal(_currentLeadName, _currentIndex, _step);

                        
                        Console.WriteLine("/// number of steps : " + _numberOfSteps);
                        Console.WriteLine("/// Onset : " + QRSOnSet);
                        Console.WriteLine("/// R : " + R);
                        Console.WriteLine("/// End : " + QRSEnds);
                        Console.ReadLine();

                        _alg = new Heart_Class_Alg();
                        _tempClassResult = new List<Tuple<int, int>>();
                        _tempClassResult.Add(_alg.Classification(_currentVector, QRSOnSet, QRSEnds, R, _fs));
                        OutputWorker.SaveClassificationResult(_currentLeadName, true, _tempClassResult);
                        
                        
                        // następna porcja sygnału: od R przed chwilą analizowanego, do R+1 (tego, za zespołem który będzie oznaczany w następnej iteracji)
                        _currentIndex = (int) R;
                        _samplesProcessed++;
                        //step: dwa interwały +- od analizowanego R-peaka w kolejnej iteracji
                        _step = (int)(InputRpeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, _samplesProcessed+1, _numberProcessedComplexes)[0]) - _currentIndex;

                        Console.WriteLine("/// current index : " + _currentIndex);
                        Console.WriteLine("/// step : " + _step);
                        _state = STATE.PROCESS_CHANNEL;

                        if (_samplesProcessed+2 >= _numberOfSteps)
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
                        double R = InputRpeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, _samplesProcessed,
                                 _numberProcessedComplexes)[0] - _currentIndex;
                        int QRSOnSet = InputWavesWorker.LoadSignal(Waves_Signal.QRSOnsets, _currentLeadName, _samplesProcessed,
                                _numberProcessedComplexes)[0] - _currentIndex; //-_currentIndex, aby uwzględnić cięcie sygnału i zmianę indeksu dla qrsOnset i qrsEnd
                        int QRSEnds = InputWavesWorker.LoadSignal(Waves_Signal.QRSEnds, _currentLeadName, _samplesProcessed,
                                _numberProcessedComplexes)[0] - _currentIndex;

                        Console.WriteLine("/// Onset : " + QRSOnSet);
                        Console.WriteLine("/// R : " + R);
                        Console.WriteLine("/// End : " + QRSEnds);

                        _currentVector = InputEcGbaselineWorker.LoadSignal(_currentLeadName, _currentIndex, _step);
                        _alg = new Heart_Class_Alg();
                        _tempClassResult = new List<Tuple<int, int>>();
                        _tempClassResult.Add(_alg.Classification(_currentVector, QRSOnSet, QRSEnds, R, _fs));
                        OutputWorker.SaveClassificationResult(_currentLeadName, true, _tempClassResult);

                        
                        _currentIndex = (int)R;
                        _samplesProcessed++;
                        _step = (int)(InputRpeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, _samplesProcessed+1, _numberProcessedComplexes)[0]) - _currentIndex;

                        Console.WriteLine("/// samplesProcessed : " + _samplesProcessed);
                        Console.WriteLine("/// current index : " + _currentIndex);
                        Console.WriteLine("/// step : " + _step);

                        _state = STATE.PROCESS_CHANNEL;

                        if (_samplesProcessed+2 >= _numberOfSteps)
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
                    // dla ostatniego zespołu nie wykrywamy, bo byc moze ze bedzie ucięty albo inne rzeczy - nie chce wyjątków
                    // w tym stanie można przepisać wynik dla każdego innego odprowadzenia

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


                    _state = STATE.END;
                    break;
                case (STATE.NEXT_CHANNEL):
                    //nie potrzeba
                    break;
                case (STATE.END):
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
            Heart_Class_Params param = new Heart_Class_Params("Analysis 1");

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
