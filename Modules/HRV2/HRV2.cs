using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.Statistics;
using EKG_Project.IO;
using EKG_Project.Modules.R_Peaks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.HRV2
{
    public partial class HRV2 : IModule
    {
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentRPeaksLength;
        private int _samplesProcessed;
        private int _numberOfChannels;

        private R_Peaks_Data_Worker _inputWorker;
        private HRV2_Data_Worker _outputWorker;

        private HRV2_Data _outputData;
        private R_Peaks_Data _inputData;
        private HRV2_Params _params;

        private Vector<double> _currentHistogram;
        private int _currentBinAmout;
        private Vector<double> _currentPoincare;

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
            Params = parameters as HRV2_Params;
            Aborted = false;
            if (!Runnable()) _ended = true;
            else
            {
                _ended = false;

                InputWorker = new R_Peaks_Data_Worker(Params.AnalysisName);
                InputWorker.Load();
                InputData = InputWorker.Data;

                OutputWorker = new HRV2_Data_Worker(Params.AnalysisName);
                OutputData = new HRV2_Data();

                _currentChannelIndex = 0;
                _samplesProcessed = 0;
                NumberOfChannels = InputData.RPeaks.Count;
                _currentRPeaksLength = InputData.RPeaks[_currentChannelIndex].Item2.Count;
                //cos z sensem tu bedzie
                _currentHistogram = Vector<Double>.Build.Dense(_currentRPeaksLength);
                _currentPoincare = Vector<Double>.Build.Dense(_currentRPeaksLength);
            }

        }

        public void ProcessData()
        {
            if (Runnable()) processData();
            else _ended = true;
        }

        public double Progress()
        {
            return 100.0 * ((double)_currentChannelIndex / (double)NumberOfChannels + (1.0 / NumberOfChannels) * ((double)_samplesProcessed / (double)_currentRPeaksLength));
        }

        public bool Runnable()
        {
            return Params != null;
        }

        private void processData()
        {
            int channel = _currentChannelIndex;
            int startIndex = _samplesProcessed;

            if (channel < NumberOfChannels)
            {

                Analyse(InputData.RPeaks[_currentChannelIndex].Item2);
                OutputData.HistogramData = new Histogram (_currentHistogram, _currentBinAmout);
                Vector<double> rr_intervals_x = Vector<double>.Build.Dense(1);
                Vector<double> rr_intervals_y = Vector<double>.Build.Dense(1);
                PoincarePlot( rr_intervals_x,  rr_intervals_y);
                OutputData.PoincarePlotData_x = new Tuple<string, Vector<double>>("X", rr_intervals_x);
                OutputData.PoincarePlotData_y = new Tuple<string, Vector<double>>("Y", rr_intervals_y);
                _currentChannelIndex++;
                if (_currentChannelIndex < NumberOfChannels)
                {
                    _currentRPeaksLength = InputData.RPeaks[_currentChannelIndex].Item2.Count;
                    _currentHistogram = Vector<Double>.Build.Dense(_currentRPeaksLength);
                    _currentPoincare = Vector<Double>.Build.Dense(_currentRPeaksLength);
                }


 
            }
            else
            {
                OutputWorker.Save(OutputData);
                _ended = true;
            }



        }

        public HRV2_Data OutputData
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

        public HRV2_Params Params
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

        public R_Peaks_Data InputData
        {
            get
            {
                return _inputData;
            }

            set
            {
                _inputData = value;
            }
        }

        public R_Peaks_Data_Worker InputWorker
        {
            get
            {
                return _inputWorker;
            }

            set
            {
                _inputWorker = value;
            }
        }

        public HRV2_Data_Worker OutputWorker
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

        public static void Main()
        {
            HRV2_Params param = new HRV2_Params(-2, 5000, "Analysis6");
            //HRV2_Params param = null;
            HRV2 hrv2 = new HRV2();
            hrv2.Init(param);
            while (true)
            {
                //Console.WriteLine("Press key to continue.");
                //Console.Read();
                if (hrv2.Ended()) break;
                Console.WriteLine(hrv2.Progress());
                hrv2.ProcessData();
            }
        }
    }
}
