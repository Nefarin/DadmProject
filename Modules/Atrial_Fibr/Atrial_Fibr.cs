using System;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.R_Peaks;
using MathNet.Numerics;
using System.Collections.Generic;

namespace EKG_Project.Modules.Atrial_Fibr
{
    public partial class Atrial_Fibr : IModule
    {
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _samplesProcessed;
        private int _numberOfChannels;

        int qrsEndStep;// o tyle qrs się przesuwamy 
        int i; //do inkrementacji co 10
        int step; // ilość próbek o którą sie przesuwamy

        //private R_Peaks_Data_Worker _inputRpeaksWorker;
        //private Atrial_Fibr_Data_Worker _outputWorker;

        private R_Peaks_Data _inputRpeaksData;
        private Atrial_Fibr_Data _outputData;

        private Atrial_Fibr_Params _params;

        private Vector<Double> _currentVector;
        //private// List<Tuple<int, int>> _tempClassResult; - napisac to co zwraca nasza funkcja

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
            Params = parameters as Atrial_Fibr_Params;
            Aborted = false;
            if (!Runnable()) _ended = true;
            else
            {
                _ended = false;

                //    InputRpeaksWorker = new R_Peaks_Data_Worker(Params.AnalysisName);
                //    InputRpeaksWorker.Load();
                //    InputRpeaksData = InputRpeaksWorker.Data;

                //    OutputWorker = new Atrial_Fibr_Data_Worker(Params.AnalysisName);
                //    OutputData = new Atrial_Fibr_Data();

                //    _currentChannelIndex = 0;
                //    _samplesProcessed = 0;
                //    NumberOfChannels = InputRpeaksData.SignalsFiltered.Count;
                //    _currentChannelLength = InputRpeaksData.SignalsFiltered[_currentChannelIndex].Item2.Count;
                //    _currentVector = Vector<Double>.Build.Dense(_currentChannelLength);
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
            //int qrsEndStep = 10;// o tyle qrs się przesuwamy
            //int i = 10; //do inkrementacji co 10
            //int step; // ilość próbek o którą sie przesuwamy

            //if (channel < NumberOfChannels)
            //{
            //    //step = InputWavesData.QRSEnds[_currentChannelIndex].Item2[qrsEndStep];

            //    if (startIndex + step > _currentChannelLength)
            //    {



            //        _currentVector = InputRpeaksData.SignalsFiltered[_currentChannelIndex].Item2.SubVector(startIndex, _currentChannelLength - startIndex);

            //        //OutputData.ClassificationResult.AddRange(new List<Tuple<int, int>>(Classification(_currentVector, fs, InputRpeaksData.RPeaks[_currentChannelIndex].Item2, InputWavesData.QRSOnsets[_currentChannelIndex].Item2, InputWavesData.QRSEnds[_currentChannelIndex].Item2)));
            //        _tempClassResult = Classification(_currentVector, fs,
            //            InputRpeaksData.RPeaks[_currentChannelIndex].Item2,
            //            InputWavesData.QRSOnsets[_currentChannelIndex].Item2,
            //            InputWavesData.QRSEnds[_currentChannelIndex].Item2);
            //        OutputData.ClassificationResult.AddRange(new List<Tuple<int, int>>(_tempClassResult));
            //        _currentChannelIndex++;

            //        if (_currentChannelIndex < NumberOfChannels)
            //        {
            //            _samplesProcessed = 0;
            //            _currentChannelLength = InputECGbaselineData.SignalsFiltered[_currentChannelIndex].Item2.Count;
            //            _currentVector = Vector<Double>.Build.Dense(_currentChannelLength);
            //            //_tempClassResult = new List<Tuple<int, int>>(); //kasowanie tego co było wczesniej zapisane, czy tak moze byc?
            //        }


            //    }
            //    else
            //    {

            //        _currentVector = InputECGbaselineData.SignalsFiltered[_currentChannelIndex].Item2.SubVector(startIndex, step);

            //        _tempClassResult = Classification(_currentVector, fs,
            //            InputRpeaksData.RPeaks[_currentChannelIndex].Item2,
            //            InputWavesData.QRSOnsets[_currentChannelIndex].Item2,
            //            InputWavesData.QRSEnds[_currentChannelIndex].Item2);
            //        OutputData.ClassificationResult.AddRange(new List<Tuple<int, int>>(_tempClassResult));

            //        _samplesProcessed = startIndex + step;

            //    }
            //}
            //else
            //{
            //    OutputWorker.Save(OutputData);
            //    _ended = true;
            //}

            //qrsEndStep += i;
            //step = InputWavesData.QRSEnds[_currentChannelIndex].Item2[qrsEndStep] - step;
        }


        //public Atrial_Fibr_Data_Worker OutputWorker
        //{
        //    get { return _outputWorker; }
        //    set { _outputWorker = value; }
        //}

        public bool Aborted
        {
            get { return _aborted; }
            set { _aborted = value; }
        }

        public Atrial_Fibr_Params Params
        {
            get { return _params; }
            set { _params = value; }
        }
        Atrial_Fibr_Data OutputData
        {
            get { return _outputData; }
            set { _outputData = value; }
        }


        public int NumberOfChannels
        {
            get { return _numberOfChannels; }
            set { _numberOfChannels = value; }
        }


        //public R_Peaks_Data_Worker InputRpeaksWorker
        //{
        //    get { return _inputRpeaksWorker; }
        //    set { _inputRpeaksWorker = value; }
        //}

        //public R_Peaks_Data InputRpeaksData
        //{
        //    get { return _inputRpeaksData; }
        //    set { _inputRpeaksData = value; }
        //}

        public static void Main()
        {
            //Atrial_Fibr_Params param = new Atrial_Fibr_Params("Analysis6"); // "Analysis6");
            Atrial_Fibr_Params param = new Atrial_Fibr_Params(Detect_Method.STATISTIC);

            Atrial_Fibr testModule = new Atrial_Fibr();
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