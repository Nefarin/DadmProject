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

        private ECG_Baseline_Data_Worker _inputECGBaselineWorker;
        private ECG_Baseline_Data _inputECGBaselineData;

        private Waves_Data_Worker _inputWavesWorker;
        private Waves_Data _inputWavesData;

        private R_Peaks_Data_Worker _inputRpeaksWorker;
        private ST_Segment_Data_Worker _outputWorker;

        private Basic_Data_Worker _inputBasicDataWorker; //czestotliwosc
        private Basic_Data _inputBasicData;

        private ST_Segment_Data _outputData;
        private R_Peaks_Data _inputData;
        private ST_Segment_Params _params;


        private int _fs;
        private List<double> _tJs;
        private List<double> _tSTs;
        private int _ConcaveCurves;
        private int _ConvexCurves;
        private int _IncreasingLines;
        private int _HorizontalLines;
        private int _DecreasingLines;
        private int _currentConcaveCurves;
        private int _currentIncreasingLines;
        private int _currentDecreasingLines;
        private int _currentHorizontalLines;
        private int _currentConvexCurves;
        private List<long> _currenttST;
        private List<long> _currenttJ;
        private int _rPeaksProcessed;
        private int _currentRpeaksLength;
        private bool rInterval;

        public bool Aborted { get; private set; }
        public ST_Segment_Params Params { get; private set; }
        public Basic_Data_Worker InputWorker { get; private set; }
        public ECG_Baseline_Data_Worker InputECGworker { get; private set; }
        public R_Peaks_Data_Worker InputWorkerRpeaks { get; private set; }
        public Basic_Data InputData { get; private set; }
        public ECG_Baseline_Data InputECGData { get; private set; }
        public R_Peaks_Data InputDataRpeaks { get; private set; }
        public ST_Segment_Data_Worker OutputWorker { get; private set; }
        public ST_Segment_Data OutputData { get; private set; }
        public object NumberOfChannels { get; private set; }

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
            Params = parameters as ST_Segment_Params;
            Aborted = false;
            if (!Runnable()) _ended = true;
            else
            {
                _ended = false;

                InputWorker = new Basic_Data_Worker(Params.AnalysisName);
                InputECGworker = new ECG_Baseline_Data_Worker(Params.AnalysisName);
                InputWorkerRpeaks = new R_Peaks_Data_Worker(Params.AnalysisName);

                InputWorker.Load();
                InputData = InputWorker.BasicData;

                InputECGworker.Load();
                InputECGData = InputECGworker.Data;

                InputWorkerRpeaks.Load();
                InputDataRpeaks = InputWorkerRpeaks.Data;


                OutputWorker = new ST_Segment_Data_Worker(Params.AnalysisName);
                OutputData = new ST_Segment_Data();

                _currentChannelIndex = 0;
                _samplesProcessed = 0;
                _rPeaksProcessed = 0;
                NumberOfChannels = InputData.RPeaks.Count
                _currentRPeaksLength = InputData.RPeaks[_currentChannelIndex].Item2.Count;


                _currenttJ = new List<long>();
                _currenttST = new List<long>();
                _currentConcaveCurves = new int();
                _currentConvexCurves = new int();
                _currentIncreasingLines = new int();
                _currentHorizontalLines = new int();
                _currentDecreasingLines = new int();
            } }

        public void ProcessData(ST_Segment_Params parameters)
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
            int startIndex = _rPeaksProcessed;
            int step = Params.RpeaksStep;

            if (channel < NumberOfChannels)
            {
                if (startIndex + step > _currentRpeaksLength)
                {
                    Method(Vector < double > signal, Vector < uint > tQRS_onset, Vector < uint > tQRS_ends, Vector < double > rInterval, int freq);
                    OutputData.tJ.Add(new List<long>(InputData.Signals[_currentChannelIndex].Item1, _tJ));
                    OutputData.tST.Add(new List<long>(InputData.Signals[_currentChannelIndex].Item1, _tST));


                    OutputData.ConcaveCurves.Add(new < int > (InputData.Signals[_currentChannelIndex].Item1, _ConcaveCurves));
                    OutputData.ConvexCurves.Add(new < int > (InputData.Signals[_currentChannelIndex].Item1, _ConvexCurves));
                    OutputData.IncreasingLines.Add(new < int > (InputData.Signals[_currentChannelIndex].Item1, _IncreasingLines));
                    OutputData.HorizontalLines.Add(new < int > (InputData.Signals[_currentChannelIndex].Item1, _HorizontalLines));
                    OutputData.DecreasingLines.Add(new < int > (InputData.Signals[_currentChannelIndex].Item1, _DecreasingLines));

                    _currentChannelIndex++;
                    if (_currentChannelIndex < NumberOfChannels)
                    {
                        _rPeaksProcessed = 0;

                        _currentRpeaksLength = InputDataRpeaks.RPeaks[_currentChannelIndex].Item2.Count;

                        _currenttJ = new List<long>();
                        _currenttST = new List<long>();
                        _currentConcaveCurves = new int();
                        _currentConvexCurves = new int();
                        _currentIncreasingLines = new int();
                        _currentHorizontalLines = new int();
                        _currentDecreasingLines = new int();
                    }


                }

                else
                {
                    OutputWorker.Save(OutputData);
                    _ended = true;
                }



            } }

        private void Method(bool v1, bool v2, bool v3, bool v4, int v5, object freq)
        {
            throw new NotImplementedException();
        }

        public void ProcessData()
        {
            throw new NotImplementedException();
        }

        public ST_Segment_Data 
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

       
public ST_Segment_Params Params
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





public Basic_Data InputData
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

public R_Peaks_Data InputDataRpeaks
{
    get
    {
        return _inputRpeaksData;
    }

    set
    {
        _inputRpeaksData = value;
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

public Waves_Params Params
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

public Waves_Data OutputData
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

public Basic_Data_Worker InputWorker
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

public R_Peaks_Data_Worker InputWorkerRpeaks
{
    get
    {
        return _inputRpeaksWorker;
    }
    set
    {
        _inputRpeaksWorker = value;
    }
}

public Waves_Data_Worker OutputWorker
{
    get
    {
        return _outputWorker;
    }

    set
    {
        _outputWorker = value;
    }
    public ST_Segment_Data_Worker OutputWorker
    {
        get;
        {
            return _outputWorker;
        }

        set;
        {
            _outputWorker = value;
        }






    }

    //testowanie

    public static void Main()
        {

        ST_Segment _Params param = new ST_Segment _Params("Analysis6");


        //TestModule3_Params param = null;


        ST_Segment testModule = new ST_Segment();


        testModule.Init(param);


        while (true)


        {




            Console.WriteLine("Press key to continue.");


            Console.Read();


            if (testModule.Ended()) break;


            Console.WriteLine(testModule.Progress());


            testModule.ProcessData();


            Console.WriteLine(testModule.OutputData.HeartAxis);


            Console.WriteLine("Press key to continue.");


            Console.Read();

        }



    }
}
}
