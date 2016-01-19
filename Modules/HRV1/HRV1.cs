using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using EKG_Project.Modules.R_Peaks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.HRV1
{
    public partial class HRV1 : IModule
    {
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _samplesProcessed;
        private int _numberOfChannels;
        private int _currentRPeaksLength;

        private R_Peaks_Data_Worker _inputWorker;
        private HRV1_Data_Worker _outputWorker;

        private HRV1_Data _outputData;
        private R_Peaks_Data _inputData;
        private HRV1_Params _params;

        private Vector<Double> _currentVector;

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
            Params = parameters as HRV1_Params;
            Aborted = false;

            if (!Runnable()) _ended = true;
            else
            {
                _ended = false;
                InputWorker = new R_Peaks_Data_Worker(Params.AnalysisName);
                InputWorker.Load();
                InputData = InputWorker.Data;

                OutputWorker = new HRV1_Data_Worker(Params.AnalysisName);
                OutputData = new HRV1_Data();

                _currentChannelIndex = 0;
                _samplesProcessed = 0;
                _numberOfChannels = InputData.RPeaks.Count;

                //_currentRPeaksLength = InputData.RRInterval[_outputIndex].Item2.Count;
                //_currentChannelLength = InputData.Signals[_currentChannelIndex].Item2.Count;
                //_currentVector = Vector<Double>.Build.Dense(_currentChannelLength);
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
            rInstants = InputData.RPeaks[_currentChannelIndex].Item2;
            rrIntervals = InputData.RRInterval[_currentChannelIndex].Item2;
            intervalsCorection();
            calculateTimeBased();
            calculateFreqBased();
            var tparams = Vector<double>.Build.Dense(new double[] { TP, HF, LF, VLF, LFHF });
            var fparams = Vector<double>.Build.Dense(new double[] { AVNN, SDNN, RMSSD, NN50, pNN50 });

            OutputData.TimeBasedParams.Add(new Tuple<string, Vector<double>>("Time-based params", tparams));
            OutputData.FreqBasedParams.Add(new Tuple<string, Vector<double>>("Frequency-based params", fparams));

            OutputData.RInstants.Add(new Tuple<string, Vector<double>>("TachoX", rInstants));
            OutputData.RRIntervals.Add(new Tuple<string, Vector<double>>("TachoY", rrIntervals));

            OutputWorker.Save(OutputData);
            _ended = true;

            bool debug = true;
            if (debug) {
                Console.WriteLine("number of RRs: " + rInstants.Count);
                Console.WriteLine("Min-Max: " + rrIntervals.Min().ToString() + "-" + rrIntervals.Max().ToString());
                Console.WriteLine("Time");
                Console.WriteLine("AVNN:  " + AVNN.ToString());
                Console.WriteLine("SDNN:  " + SDNN.ToString());
                Console.WriteLine("RMSSD: " + RMSSD);
                //Console.WriteLine("SDSD:  " + SDSD.ToString());
                Console.WriteLine("NN50:  " + NN50.ToString());
                Console.WriteLine("pNN50: " + pNN50.ToString());
                Console.WriteLine("Freq");
                Console.WriteLine("TP:   " + TP.ToString());
                Console.WriteLine("VLF:   " + VLF.ToString());
                Console.WriteLine("LF:    " + LF.ToString());
                Console.WriteLine("HF:    " + HF.ToString());
                Console.WriteLine("LFHF:  " + LFHF.ToString());
            }
        }

        public HRV1_Data OutputData
        {
            get { return _outputData; }
            set { _outputData = value; }
        }

        public HRV1_Params Params
        {
            get { return _params; }
            set { _params = value; }
        }

        public int NumberOfChannels
        {
            get { return _numberOfChannels; }
            set { _numberOfChannels = value; }
        }

        public bool Aborted
        {
            get { return _aborted; }
            set { _aborted = value; }
        }

        public R_Peaks_Data InputData
        {
            get { return _inputData; }
            set { _inputData = value; }
        }

        public R_Peaks_Data_Worker InputWorker
        {
            get { return _inputWorker; }
            set { _inputWorker = value; }
        }

        public HRV1_Data_Worker OutputWorker
        {
            get { return _outputWorker; }
            set { _outputWorker = value; }
        }
        

        public static void Main()
        {
            //HRV1.AlgoTest();

            var param = new HRV1_Params("Analysis3");
            //param = null;
            HRV1 Hrv1 = new HRV1();
            Hrv1.Init(param);
            while (true)
            {
                //Console.WriteLine("Press key to continue.");
                //Console.Read();
                if (Hrv1.Ended()) break;
                Console.WriteLine(Hrv1.Progress());
                Hrv1.ProcessData();
            }
        }
    }
}
