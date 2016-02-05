using System;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using System.Linq;

namespace EKG_Project.Modules.Heart_Cluster
{
    public partial class Heart_Cluster : IModule
    {
        private enum STATE { INIT, BEGIN_CHANNEL, PROCESS_FIRST_STEP, PROCESS_CHANNEL, NEXT_CHANNEL, END_CHANNEL, END };
        private bool _ended;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _samplesProcessed;
        private string _currentLeadName;
        private string[] _leads;
        private string _leadNameChannel2;
        private int _currentIndex;
        private uint _fs;
        private int _numberProcessedComplexes;

        private bool _ml2Processed;
        private int _step;
        private int _numberOfSteps;
        private uint[] _numberOfStepsArray;

        private Vector<Double> _currentVector;
        private STATE _state;


        readonly List<Tuple<int, int, int, int>> _finalResult = new List<Tuple<int, int, int, int>>();


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
                Params = parameters as Heart_Cluster_Params;
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

                if (FindChannel()) // dostawanie się do sygnału
                {
                    InputRpeaksWorker = new R_Peaks_New_Data_Worker(Params.AnalysisName);
                    InputWavesWorker = new Waves_New_Data_Worker(Params.AnalysisName);
                    OutputWorker = new Heart_Cluster_Data_Worker(Params.AnalysisName);

                    _currentChannelIndex = 0;
                    _leadNameChannel2 = _leads[Channel2];

                    // If sth is wrong in earlier modules, this is an insurance ?
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
                    Aborted = true;
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
                    _currentChannelIndex = -1;
                    NumberOfChannels = _leads.Length;
                    _state = STATE.BEGIN_CHANNEL;
                    break;
                case (STATE.BEGIN_CHANNEL):
                    _currentChannelIndex++;
                    _currentLeadName = _leads[Channel2];
                    _currentChannelLength = (int)InputBasicWorker.getNumberOfSamples(_currentLeadName); // ?
                    _currentIndex = 0;
                    _samplesProcessed = 0;

                    //step zawiera odległość miedzy początkiem sygnału a 2 R_peakiem od początku (samplesProcessed+1). 
                    _step = (int)InputRpeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, (_samplesProcessed + 1), _numberProcessedComplexes)[0];

                    _state = STATE.PROCESS_FIRST_STEP;
                    break;
                case (STATE.PROCESS_FIRST_STEP):
                    if (!_ml2Processed)
                    {
                        // Przetwarzanie: pierwsza porcja sygnału od 0 (currentIndex) do drugiego wykrytego Rpeaka (samplesProcessed+1). 
                        // Żeby mieć pewność ze taka porcja sygnału pokryje QRSOnset, R, QRSend dla pierwszego zespołu.

                        var QRSOnSet = InputWavesWorker.LoadSignal(Waves_Signal.QRSOnsets, _currentLeadName, _samplesProcessed, _numberProcessedComplexes)[0];
                        var QRSEnds = InputWavesWorker.LoadSignal(Waves_Signal.QRSEnds, _currentLeadName, _samplesProcessed, _numberProcessedComplexes)[0];
                        var R = (int)InputRpeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, _samplesProcessed, _numberProcessedComplexes)[0];
                        _currentVector = InputEcGbaselineWorker.LoadSignal(_currentLeadName, _currentIndex, _step);


                        var tempClusterResult = new List<Tuple<int, int, int, int>>();
                        var clusterizationResultTemp = Classification(_currentVector, QRSOnSet, QRSEnds, R, _fs);

                        tempClusterResult.Add(clusterizationResultTemp);
                        _finalResult.Add(clusterizationResultTemp);

                        //ODKOMENTOWAC!!!!!
                        OutputWorker.SaveClusterizationResult(_currentLeadName, true, tempClusterResult);


                        // następna porcja sygnału: od R przed chwilą analizowanego, do R+1 (tego, za zespołem który będzie oznaczany w następnej iteracji)
                        _currentIndex = R;
                        _samplesProcessed++;
                        //step: dwa interwały +- od analizowanego R-peaka w kolejnej iteracji
                        _step = (int)(InputRpeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, _samplesProcessed + 1, _numberProcessedComplexes)[0]) - _currentIndex;

                        _state = STATE.PROCESS_CHANNEL;

                        if (_samplesProcessed + 2 >= _numberOfSteps)
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
                        var Ractual = (int)InputRpeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, _samplesProcessed, _numberProcessedComplexes)[0];
                        var R = Ractual - _currentIndex;
                        int QRSOnSet = InputWavesWorker.LoadSignal(Waves_Signal.QRSOnsets, _currentLeadName, _samplesProcessed, _numberProcessedComplexes)[0] - _currentIndex; //-_currentIndex, aby uwzględnić cięcie sygnału i zmianę indeksu dla qrsOnset i qrsEnd
                        int QRSEnds = InputWavesWorker.LoadSignal(Waves_Signal.QRSEnds, _currentLeadName, _samplesProcessed, _numberProcessedComplexes)[0] - _currentIndex;

                        if (!(R < -1 || QRSOnSet < -1 || QRSEnds < -1))
                        {
                            _currentVector = InputEcGbaselineWorker.LoadSignal(_currentLeadName, _currentIndex, _step);

                            var tempClusterResult = new List<Tuple<int, int, int, int>>();
                            var clusterizationResultTemp = Classification(_currentVector, QRSOnSet, QRSEnds, R, _fs);

                            var clusterizationResult = new Tuple<int, int, int, int>(clusterizationResultTemp.Item1 + _currentIndex, clusterizationResultTemp.Item2 + _currentIndex, Ractual, clusterizationResultTemp.Item4);


                            //teraz trzeba powrócić do R rzeczywistego, a nie tego zmodyfikowanego na potrzeby cięcia sygnału... :/

                            tempClusterResult.Add(clusterizationResult);
                            _finalResult.Add(clusterizationResultTemp);

                            //ODKOMENTOWAC!!!!!
                            OutputWorker.SaveClusterizationResult(_currentLeadName, true, tempClusterResult);
                        }
                        _currentIndex = R;
                        _samplesProcessed++;
                        _step = (int)(InputRpeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, _samplesProcessed + 1, _numberProcessedComplexes)[0]) - _currentIndex;

                        _state = STATE.PROCESS_CHANNEL;

                        if (_samplesProcessed + 2 >= _numberOfSteps)
                        {
                            _ml2Processed = true;
                        }

                    }
                    else
                    {
                        OutputWorker.SaveAttributeI(Heart_Cluster_Attributes_I.NumberofClass, _currentLeadName, (uint)classCounts.Count);
                        OutputWorker.SaveAttributeI(Heart_Cluster_Attributes_I.TotalQrsComplex, _currentLeadName, (uint)classCounts.Sum());

                        for (int i = 0; i < classCounts.Count; i++)
                        {
                            OutputWorker.SaveAttributeII(Heart_Cluster_Attributes_II.indexOfClass, true, _currentLeadName, i);
                            OutputWorker.SaveAttributeII(Heart_Cluster_Attributes_II.QrsComplexNo, true, _currentLeadName, classCounts[i]);
                        }

                        _state = STATE.END_CHANNEL;
                    }


                    break;
                case (STATE.END_CHANNEL):
                    // dla ostatniego zespołu nie wykrywamy, bo byc moze ze bedzie ucięty albo inne rzeczy - nie chce wyjątków
                    // w tym stanie można przepisać wynik dla każdego innego odprowadzenia

                    for (int i = 0; i < NumberOfChannels; i++)
                    {
                        _currentLeadName = _leads[i];

                        if (_currentLeadName != _leadNameChannel2)
                        {
                            //ODKOMENTOWAC!!!!!!!!!!!!!
                            OutputWorker.SaveClusterizationResult(_currentLeadName, true, _finalResult);

                            OutputWorker.SaveAttributeI(Heart_Cluster_Attributes_I.NumberofClass, _currentLeadName, (uint)classCounts.Count);
                            OutputWorker.SaveAttributeI(Heart_Cluster_Attributes_I.TotalQrsComplex, _currentLeadName, (uint)classCounts.Sum());

                            for (int j = 0; j < classCounts.Count; j++)
                            {
                                OutputWorker.SaveAttributeII(Heart_Cluster_Attributes_II.indexOfClass, true, _currentLeadName, j);
                                OutputWorker.SaveAttributeII(Heart_Cluster_Attributes_II.QrsComplexNo, true, _currentLeadName, classCounts[j]);
                            }
                        }
                    }

                    _state = STATE.END;
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

        }

        private bool FindChannel()
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

        public Heart_Cluster_Data_Worker OutputWorker { get; set; }

        public bool Aborted { get; set; }

        public Heart_Cluster_Params Params { get; set; }

        public ECG_Baseline_New_Data_Worker InputEcGbaselineWorker { get; set; }

        public R_Peaks_New_Data_Worker InputRpeaksWorker { get; set; }

        public Waves_New_Data_Worker InputWavesWorker { get; set; }

        public int NumberOfChannels { get; set; }

        public int Channel2 { get; set; }

        public Basic_New_Data_Worker InputBasicWorker { get; set; }


        public static void Main(String[] args)
        {
            IModule testModule = new Heart_Cluster();
            Heart_Cluster_Params param = new Heart_Cluster_Params("Analysis 223");

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