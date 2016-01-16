using System;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using System.Collections.Generic;
using EKG_Project.Modules.ECG_Baseline;
using EKG_Project.Modules.R_Peaks;
using EKG_Project.Modules.Waves;

namespace EKG_Project.Modules.Heart_Class
{
    public partial class Heart_Class : IModule
    {
        private bool _ended;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _samplesProcessed;
        //private int startIndex;
        private bool _ml2Processed;
        private int _numberOfSteps;

        // Czy te zmienne muszą być private? 
        //int qrsEndStep;// o tyle qrs się przesuwamy 
        //int i; //do inkrementacji co 10
        //int step; // ilość próbek o którą sie przesuwamy

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

            Params = parameters as Heart_Class_Params;


            Aborted = false;
            if (!Runnable()) _ended = true;
            else
            {
                _ended = false;

                InputEcGbaselineWorker = new ECG_Baseline_Data_Worker(Params.AnalysisName);
                InputEcGbaselineWorker.Load();
                InputECGbaselineData = InputEcGbaselineWorker.Data;
                OutputData = new Heart_Class_Data();

                if (findChannel())
                {
                    InputRpeaksWorker = new R_Peaks_Data_Worker(Params.AnalysisName);
                    InputRpeaksWorker.Load();
                    InputRpeaksData = InputRpeaksWorker.Data;

                    BasicDataWorker = new Basic_Data_Worker(Params.AnalysisName);
                    BasicDataWorker.Load();
                    BasicData = BasicDataWorker.BasicData;
                    _fs = BasicData.Frequency;

                    InputWavesWorker = new Waves_Data_Worker(Params.AnalysisName);
                    InputWavesWorker.Load();
                    InputWavesData = InputWavesWorker.Data;
                    //_currentRVector = InputRpeaksData.RPeaks[_channel2].Item2;
                    //_currentECGBaselineVector = InputECGbaselineData.SignalsFiltered[_channel2].Item2;

                    OutputWorker = new Heart_Class_Data_Worker(Params.AnalysisName);

                    _currentChannelIndex = 0;
                    _samplesProcessed = 0;
                    //startIndex = _samplesProcessed;
                    NumberOfChannels = InputECGbaselineData.SignalsFiltered.Count;
                    _currentChannelLength = InputECGbaselineData.SignalsFiltered[_currentChannelIndex].Item2.Count;
                    //_currentVector = Vector<Double>.Build.Dense(_currentChannelLength);
                    //qrsEndStep = 10;
                    //i = 10;
                    _numberOfSteps = InputWavesData.QRSEnds[Channel2].Item2.Count;
                    //step = 1;
                    //ilośc próbek, aż do indeksu końca 10 załamka
                    _tempClassResult = new Tuple<int, int>(0, 0);
                    _ml2Processed = false;

                }
                else
                {
                    _ended = true;
                    Aborted = true;
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

            if (!_ml2Processed)
            {

                int qrsOnSet = InputWavesData.QRSOnsets[Channel2].Item2[_samplesProcessed];
                int qrsEnds = InputWavesData.QRSEnds[Channel2].Item2[_samplesProcessed];
                double R = InputRpeaksData.RPeaks[Channel2].Item2[_samplesProcessed];

                if (qrsOnSet == -1 || qrsEnds == -1)
                {
                    _samplesProcessed++;
                }
                else
                {
                    _tempClassResult = ClassificationOneQrs(InputECGbaselineData.SignalsFiltered[Channel2].Item2, qrsOnSet, qrsEnds, R);
                    OutputData.ClassificationResult.Add(_tempClassResult);

                    _samplesProcessed++;
                }

                if (_samplesProcessed >= _numberOfSteps)
                {
                    _ml2Processed = true;
                }
            }
            else
            {
                _ended = true;
                OutputWorker.Save(OutputData);
            }

        }

        private bool findChannel()
        {
            int i = 0;

            foreach (var tuple in InputECGbaselineData.SignalsFiltered)
            {
                string name = tuple.Item1;
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

        public Heart_Class_Data_Worker OutputWorker { get; set; }

        public bool Aborted { get; set; }

        public Heart_Class_Params Params { get; set; }

        public Heart_Class_Data OutputData { get; set; }


        public ECG_Baseline_Data_Worker InputEcGbaselineWorker { get; set; }

        public R_Peaks_Data_Worker InputRpeaksWorker { get; set; }

        public Waves_Data_Worker InputWavesWorker { get; set; }

        public ECG_Baseline_Data InputECGbaselineData { get; set; }

        public R_Peaks_Data InputRpeaksData { get; set; }

        public Waves_Data InputWavesData { get; set; }

        public int NumberOfChannels { get; set; }

        public int Channel2 { get; set; }

        public Basic_Data_Worker BasicDataWorker { get; set; }

        public Basic_Data BasicData { get; set; }


        public static void Main()
        {
            Heart_Class_Params param = new Heart_Class_Params("TestAnalysis2"); // "Analysis6");
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
