using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.Modules.ECG_Baseline;
using EKG_Project.Modules.Waves;
using EKG_Project.Modules.R_Peaks;
using EKG_Project.IO;

namespace EKG_Project.Modules.ST_Segment
{


    public partial class ST_Segment : IModule
    {
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentRPeaksLength;
        private int _samplesProcessed;
        private int _numberOfChannels;

        private Basic_Data_Worker _inputWorker; //
        private R_Peaks_Data_Worker _inputRpeaksWorker;
        private ST_Segment_Data_Worker _outputWorker;

        private ST_Segment_Data _outputData;
        private R_Peaks_Data _inputData;
        private ST_Segment_Params _params;

        private List<double> _tJs;
        private List<double> _tSTs;
        private int _ConcaveCurves;
        private int _ConvexCurves;
        private int _IncreasingLines;
        private int _HorizontalLines;
        private int _DecreasingLines;

        public void Abort()
        {
            //Aborted = true;
            _ended = true;

        }

        public bool Ended()
        {
            return _ended;
        }

        public void Init(ModuleParams parameters)
        {
            //Params = parameters as ST_Segment_Params;
            //Aborted = false;
            //if (!Runnable()) _ended = true;
            //else
            //{
            //    _ended = false;


            //    InputWorkerRpeaks = new R_Peaks_Data_Worker(Params.AnalysisName);
            //    InputWorkerRpeaks.Load();
            //    // InputData = InputWorkerRpeaks.Data;
            //    InputDataRpeaks = InputWorkerRpeaks.Data;

            //    OutputWorker = new ST_Segment_Data_Worker(Params.AnalysisName);
            //    OutputData = new ST_Segment_Data();

            //    _currentChannelIndex = 0;
            //    _samplesProcessed = 0;
            //    NumberOfChannels = InputData.RPeaks.Count;
            //    _currentRPeaksLength = InputData.RPeaks[_currentChannelIndex].Item2.Count;

            //    _currenttJ = new List<int>();
            //    _currenttST = new List<int>(); // tu cos co mamy miec 
            //}


        }

        public void ProcessData()
        {

            if (Runnable()) processData();
            else _ended = true;

        }

        public double Progress()

        {
            return 0;
            //return 100.0 * ((double)_currentChannelIndex / (double)NumberOfChannels + (1.0 / NumberOfChannels) * ((double)_samplesProcessed / (double)_currentRPeaksLength));
        }

        public bool Runnable()
        {
            return false;
            //return Params != null;
        }


        private void processData()
        {

            //int channel = _currentChannelIndex;
            //int startIndex = _rPeaksProcessed;
            //int step = Params.RpeaksStep;

            //if (channel < NumberOfChannels)
            //{
            //    if (startIndex + step > _currentRpeaksLength)
            //    {
            //        analyzeSignalPart();
            //        OutputData.QRSOnsets.Add(new Tuple<string, List<int>>(InputData.Signals[_currentChannelIndex].Item1, _currentQRSonsets));
            //        OutputData.QRSEnds.Add(new Tuple<string, List<int>>(InputData.Signals[_currentChannelIndex].Item1, _currentQRSends));

            //        OutputData.POnsets.Add(new Tuple<string, List<int>>(InputData.Signals[_currentChannelIndex].Item1, _currentPonsets));
            //        OutputData.PEnds.Add(new Tuple<string, List<int>>(InputData.Signals[_currentChannelIndex].Item1, _currentPends));

            //        OutputData.TEnds.Add(new Tuple<string, List<int>>(InputData.Signals[_currentChannelIndex].Item1, _currentTends));

            //        _currentChannelIndex++;
            //        if (_currentChannelIndex < NumberOfChannels)
            //        {
            //            _rPeaksProcessed = 0;

            //            _currentRpeaksLength = InputDataRpeaks.RPeaks[_currentChannelIndex].Item2.Count;

            //            _currentQRSonsets = new List<int>();
            //            _currentQRSends = new List<int>();
            //            _currentPonsets = new List<int>();
            //            _currentPends = new List<int>();
            //            _currentTends = new List<int>();
            //        }


            //    }
            //    else
            //    {
            //        analyzeSignalPart();
            //        _rPeaksProcessed = startIndex + step;
            //        Console.WriteLine("Jedna sesja poszla!");
            //        Console.WriteLine(_rPeaksProcessed);
            //    }
            //}
            //else
            //{
            //    OutputWorker.Save(OutputData);
            //    _ended = true;
            //}



        }

        //public Basic_Data InputData
        //{
        //    get
        //    {
        //        return _inputData;
        //    }

        //    set
        //    {
        //        _inputData = value;
        //    }
        //}

        //public R_Peaks_Data InputDataRpeaks
        //{
        //    get
        //    {
        //        return _inputRpeaksData;
        //    }

        //    set
        //    {
        //        _inputRpeaksData = value;
        //    }
        //}


        //public int NumberOfChannels
        //{
        //    get
        //    {
        //        return _numberOfChannels;
        //    }

        //    set
        //    {
        //        _numberOfChannels = value;
        //    }
        //}

        //public bool Aborted
        //{
        //    get
        //    {
        //        return _aborted;
        //    }

        //    set
        //    {
        //        _aborted = value;
        //    }
        //}

        //public Waves_Params Params
        //{
        //    get
        //    {
        //        return _params;
        //    }

        //    set
        //    {
        //        _params = value;
        //    }
        //}

        //public Waves_Data OutputData
        //{
        //    get
        //    {
        //        return _outputData;
        //    }
        //    set
        //    {
        //        _outputData = value;
        //    }
        //}

        //public Basic_Data_Worker InputWorker
        //{
        //    get
        //    {
        //        return _inputWorker;
        //    }

        //    set
        //    {
        //        _inputWorker = value;
        //    }
        //}

        //public R_Peaks_Data_Worker InputWorkerRpeaks
        //{
        //    get
        //    {
        //        return _inputRpeaksWorker;
        //    }
        //    set
        //    {
        //        _inputRpeaksWorker = value;
        //    }
        //}

        //public ST_Segment_Data_Worker OutputWorker
        //{
        //    get
        //    {
        //        return _outputWorker;
        //    }

        //    set
        //    {
        //        _outputWorker = value;
        //    }
        //}

        public static void Main()
        {
            Waves_Params param = new Waves_Params(Wavelet_Type.haar, 2, "Analysis6", 100);

            //TempInput.setInputFilePath(@"C:\Users\Michał\Documents\biomed\II stopien\dadm\lab2\EKG.txt");
            //TempInput.setOutputFilePath(@"C:\Users\Michał\Documents\biomed\II stopien\dadm\lab2\EKGQRSonsets3.txt");
            //Vector<double> ecg = TempInput.getSignal();

            //TempInput.setInputFilePath(@"C:\Users\Michał\Documents\biomed\II stopien\dadm\lab2\EKG3Rpeaks.txt");
            //Vector<double> rpeaks = TempInput.getSignal();

            Waves.Waves testModule = new Waves.Waves();
            //testModule.InitForTestsOnly(ecg, rpeaks, param);
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
