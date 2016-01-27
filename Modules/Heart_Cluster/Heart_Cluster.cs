using System;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using System.Collections.Generic;
using System.Linq;
using EKG_Project.Modules.ECG_Baseline;
using EKG_Project.Modules.R_Peaks;
using EKG_Project.Modules.Waves;

namespace EKG_Project.Modules.Heart_Cluster
{
    public class Heart_Cluster : IModule
    {
        private bool _ended;
        public bool Aborted { get; set; } //private bool _aborted ?

        // roboczo i do zmiany:
        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _samplesProcessed;

        private int _numberOfChannels;
        private int _channel2;
        private int startIndex;
        private bool _ml2Processed;
        private int _numberOfSteps;
        private int[] _numberOfStepsArray; //roboczo i do zmiany, na public!

        private ECG_Baseline_Data_Worker _inputECGbaselineWorker;
        private R_Peaks_Data_Worker _inputRpeaksWorker;
        private Waves_Data_Worker _inputWavesWorker;
        private Heart_Class_Data_Worker _outputWorker; //? cluster? public Heart_Class_Data_Worker OutputWorker { get; set; }
        private Basic_Data_Worker _basicDataWorker;

        private ECG_Baseline_Data _inputECGbaselineData; //public ECG_Baseline_Data InputECGbaselineData { get; set; }
        private R_Peaks_Data _inputRpeaksData; //public R_Peaks_Data InputRpeaksData { get; set; }
        private Waves_Data _inputWavesData; //public Waves_Data InputWavesData { get; set; }
        private Heart_Cluster_Data _outputData;
        private Basic_Data _basicData; //public Basic_Data InputData { get; set; }


        private Heart_Cluster_Params _params; //public void Init(ModuleParams parameters)

        // moze sie przydać:
        private Vector<Double> _currentVector;
        private Tuple<int, int> _tempClusterResult; //




        public void Init(ModuleParams parameters)
        {

        }



        public void ProcessData()
        {
            if (Runnable())
                processData();
            else
                _ended = true;
        }


        private void processData()
        { } //przemyśleć!

        //znajdź mi wybrany kanał na którym chcę pracować
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
        } // znalazłem.






        public double Progress()
        {
            return 100.0 * _samplesProcessed / _numberOfSteps;
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

        public bool IsAborted()
        {
            return Aborted;
        }

        public bool Ended()
        {
            return _ended;
        }
    }
}