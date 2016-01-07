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
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _samplesProcessed;
        private int _numberOfChannels;

        // Czy te zmienne muszą być private? 
        int qrsEndStep;// o tyle qrs się przesuwamy 
        int i; //do inkrementacji co 10
        int step; // ilość próbek o którą sie przesuwamy

        private ECG_Baseline_Data_Worker _inputECGbaselineWorker;
        private R_Peaks_Data_Worker _inputRpeaksWorker;
        private Waves_Data_Worker _inputWavesWorker;
        private Heart_Class_Data_Worker _outputWorker;

        private ECG_Baseline_Data _inputECGbaselineData;
        private R_Peaks_Data _inputRpeaksData;
        private Waves_Data _inputWavesData;
        private Heart_Class_Data _outputData;

        private Heart_Class_Params _params;

        private Vector<Double> _currentVector;
        private List<Tuple<int, int>> _tempClassResult;



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
            
            Params = parameters as Heart_Class_Params;
            

            Aborted = false;
            if (!Runnable()) _ended = true;
            else
            {
                _ended = false;

                InputEcGbaselineWorker = new ECG_Baseline_Data_Worker(Params.AnalysisName);
                InputEcGbaselineWorker.Load();
                InputECGbaselineData = InputEcGbaselineWorker.Data;

                InputRpeaksWorker = new R_Peaks_Data_Worker(Params.AnalysisName);
                InputRpeaksWorker.Load();
                InputRpeaksData = InputRpeaksWorker.Data;

                InputWavesWorker = new Waves_Data_Worker(Params.AnalysisName);
                InputWavesWorker.Load();
                InputWavesData = InputWavesWorker.Data;


                OutputWorker = new Heart_Class_Data_Worker(Params.AnalysisName);
                OutputData = new Heart_Class_Data();


                _currentChannelIndex = 0;
                _samplesProcessed = 0;
                NumberOfChannels = InputECGbaselineData.SignalsFiltered.Count;
                _currentChannelLength = InputECGbaselineData.SignalsFiltered[_currentChannelIndex].Item2.Count;
                _currentVector = Vector<Double>.Build.Dense(_currentChannelLength);
                qrsEndStep = 10;
                i = 10; 
                step = InputWavesData.QRSEnds[_currentChannelIndex].Item2[qrsEndStep]; //ilośc próbek, aż do indeksu końca 10 załamka
                _tempClassResult = new List<Tuple<int, int>>();


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

            if (channel < NumberOfChannels)
            {
                //step = InputWavesData.QRSEnds[_currentChannelIndex].Item2[qrsEndStep];

                if (startIndex + step > _currentChannelLength)
                {
                    


                    _currentVector = InputECGbaselineData.SignalsFiltered[_currentChannelIndex].Item2.SubVector(startIndex, _currentChannelLength - startIndex);

                    //OutputData.ClassificationResult.AddRange(new List<Tuple<int, int>>(Classification(_currentVector, fs, InputRpeaksData.RPeaks[_currentChannelIndex].Item2, InputWavesData.QRSOnsets[_currentChannelIndex].Item2, InputWavesData.QRSEnds[_currentChannelIndex].Item2)));
                    _tempClassResult = Classification(_currentVector, fs,
                        InputRpeaksData.RPeaks[_currentChannelIndex].Item2,
                        InputWavesData.QRSOnsets[_currentChannelIndex].Item2,
                        InputWavesData.QRSEnds[_currentChannelIndex].Item2);
                    OutputData.ClassificationResult.AddRange(new List<Tuple<int, int>>(_tempClassResult));
                    _currentChannelIndex++;

                    if (_currentChannelIndex < NumberOfChannels)
                    {
                        _samplesProcessed = 0;
                        _currentChannelLength = InputECGbaselineData.SignalsFiltered[_currentChannelIndex].Item2.Count;
                        _currentVector = Vector<Double>.Build.Dense(_currentChannelLength);
                        //_tempClassResult = new List<Tuple<int, int>>(); //kasowanie tego co było wczesniej zapisane, czy tak moze byc?
                    }

                   
                }
                else
                {
                    
                    _currentVector = InputECGbaselineData.SignalsFiltered[_currentChannelIndex].Item2.SubVector(startIndex, step);

                    _tempClassResult = Classification(_currentVector, fs,
                        InputRpeaksData.RPeaks[_currentChannelIndex].Item2,
                        InputWavesData.QRSOnsets[_currentChannelIndex].Item2,
                        InputWavesData.QRSEnds[_currentChannelIndex].Item2);
                    OutputData.ClassificationResult.AddRange(new List<Tuple<int, int>>(_tempClassResult));

                    _samplesProcessed = startIndex + step;
                   
                }
            }
            else
            {
                OutputWorker.Save(OutputData);
                _ended = true;
            }

            qrsEndStep += i;
            step = InputWavesData.QRSEnds[_currentChannelIndex].Item2[qrsEndStep] - step;
        }


        public Heart_Class_Data_Worker OutputWorker
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
        


        public ECG_Baseline_Data_Worker InputEcGbaselineWorker
        {
            get { return _inputECGbaselineWorker; }
            set { _inputECGbaselineWorker = value; }
        }

        public R_Peaks_Data_Worker InputRpeaksWorker
        {
            get { return _inputRpeaksWorker; }
            set { _inputRpeaksWorker = value; }
        }

        public Waves_Data_Worker InputWavesWorker
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
    }
}
