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
        public bool Aborted { get; set; }
        
        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _samplesProcessed;

        public Heart_Class_Params Params { get; set; }
        public int NumberOfChannels { get; set; }

        public Heart_Class_Data_Worker OutputWorker { get; set; }
        public Heart_Class_Data OutputData { get; set; }

        public Basic_Data InputData { get; set; }

        public R_Peaks_Data InputRpeaksData { get; set; }

        public ECG_Baseline_Data InputECGbaselineData { get; set; }

        public Waves_Data InputWavesData { get; set; }

        public void Init(ModuleParams parameters)
        {
            Params = parameters as Heart_Class_Params;

            Aborted = false;
            if (!Runnable())
                _ended = true;
            else
            {
                _ended = false;

                var inputWorker = new Basic_Data_Worker(Params.AnalysisName);
                inputWorker.Load();
                InputData = inputWorker.BasicData;

                var inputEcGbaselineWorker = new ECG_Baseline_Data_Worker(Params.AnalysisName);
                inputEcGbaselineWorker.Load();
                InputECGbaselineData = inputEcGbaselineWorker.Data;

                var inputRpeaksWorker = new R_Peaks_Data_Worker(Params.AnalysisName);
                inputRpeaksWorker.Load();
                InputRpeaksData = inputRpeaksWorker.Data;

                var inputWavesWorker = new Waves_Data_Worker(Params.AnalysisName);
                inputWavesWorker.Load();
                InputWavesData = inputWavesWorker.Data;

                OutputWorker = new Heart_Class_Data_Worker(Params.AnalysisName);
                OutputData = new Heart_Class_Data();
                
                _currentChannelIndex = 0;
                _samplesProcessed = 0;
                NumberOfChannels = InputData.Signals.Count;
                _currentChannelLength = InputData.Signals[_currentChannelIndex].Item2.Count;
            }

        }

        private void processData()
        {
            int channel = _currentChannelIndex;

            if (channel < NumberOfChannels)
            {
                OutputData.ClassificationResult.Add(
                    new Tuple<string, List<Tuple<int, int>>>(InputECGbaselineData.SignalsFiltered[_currentChannelIndex].Item1, // nazwa kanału ?
                    Classification(InputECGbaselineData.SignalsFiltered[_currentChannelIndex].Item2, InputData.Frequency, InputRpeaksData.RPeaks[_currentChannelIndex].Item2, InputWavesData.QRSOnsets[_currentChannelIndex].Item2, InputWavesData.QRSEnds[_currentChannelIndex].Item2)) // klasyfikacja
                    );

                _currentChannelIndex++;
            }
            else
            {
                OutputWorker.Save(OutputData);
                _ended = true;
            }
        }

        public void ProcessData()
        {
            if (Runnable())
                processData();
            else
                _ended = true;
        }

        public double Progress()
        {
            return 100.0 * ((double)_currentChannelIndex / (double)NumberOfChannels + (1.0 / NumberOfChannels) * ((double)_samplesProcessed / (double)_currentChannelLength));
        }

        public bool Runnable()
        {
            return Params != null;
        }

        public void Abort()
        {
            Aborted = true;
            _ended = true;
        }

        public bool Ended()
        {
            return _ended;
        }
    }
}
