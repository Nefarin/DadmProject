using System;
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

        private bool _outputFound;
        private int _outputIndex;

        private Histogram2 _currentHistogram;
        private Vector<double> _currentPoincare;
        private Vector<double> _currentRPeaks;

        public bool IsAborted()
        {
            return Aborted;
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
                _currentRPeaksLength = InputData.RRInterval[_currentChannelIndex].Item2.Count;
                _currentPoincare = Vector<Double>.Build.Dense(_currentRPeaksLength);
                _numberOfChannels = InputData.RPeaks.Count;

                //findOutput();
                //if (_outputFound)
                //{
                //    OutputWorker = new HRV2_Data_Worker(Params.AnalysisName);
                //    OutputData = new HRV2_Data();

                //    _currentRPeaksLength = InputData.RRInterval[_outputIndex].Item2.Count;
                //    //_currentHistogram = Vector<Double>.Build.Dense(_currentRPeaksLength);
                //    _currentPoincare = Vector<Double>.Build.Dense(_currentRPeaksLength);
                //}
                //else
                //{
                //    _ended = true;
                //    Aborted = true;
                //}

            }

        }

        private void findOutput()
        {
            for (int i = 0; i < InputData.RRInterval.Count; i++)
            if (InputData.RRInterval[i].Item1 == "II" || InputData.RRInterval[i].Item1 == "MLII")
            {
                    _outputFound = true;
                    _outputIndex = i;
                    return;
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
            if (_currentChannelIndex < _numberOfChannels)
            {
                _currentRPeaksLength = InputData.RRInterval[_currentChannelIndex].Item2.Count;
                _currentPoincare = Vector<Double>.Build.Dense(_currentRPeaksLength);
                makeTinn();
                OutputData.Tinn.Add(new Tuple<string, double> (InputData.RRInterval[_currentChannelIndex].Item1, tinn));
                //TriangleIndex();
                //OutputData.TriangleIndex.Add(new Tuple<string, double> (InputData.RRInterval[_currentChannelIndex].Item1,triangleIndex));
                //OutputData.HistogramData.Add(new Tuple<string, Histogram2>(InputData.RRInterval[_outputIndex].Item1, _currentHistogram));

                PoincarePlot_x();
                PoincarePlot_y();
                OutputData.SD1.Add(new Tuple<string, double>(InputData.RRInterval[_currentChannelIndex].Item1, SD1()));
                OutputData.SD2.Add(new Tuple<string, double>(InputData.RRInterval[_currentChannelIndex].Item1, SD2()));

                OutputData.PoincarePlotData_x.Add(new Tuple<string, Vector<double>>(InputData.RRInterval[_currentChannelIndex].Item1, RR_intervals_x));
                OutputData.PoincarePlotData_y.Add(new Tuple<string, Vector<double>>(InputData.RRInterval[_currentChannelIndex].Item1, RR_intervals_y));

                _currentChannelIndex++;

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
            HRV2_Params param = new HRV2_Params(3, 9, "TestAnalysis6"); 
            HRV2 testModule = new HRV2();
            testModule.Init(param);
            while (true)
            {
                //Console.WriteLine("Press key to continue.");
                //Console.Read();
                if (testModule.Ended()) break;
                Console.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }
        }


    }
}
