using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.Modules.ECG_Baseline;
using EKG_Project.Modules.Waves;
using EKG_Project.Modules.R_Peaks;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using System.Windows;

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

        private Basic_Data_Worker _inputWorker;

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
        private List<long> _tJs;
        private List<long> _tSTs;
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
        private Basic_Data_Worker InputWorker;
        private bool Aborted;
        private ECG_Baseline_Data_Worker InputECGWorker;
        private R_Peaks_Data_Worker InputRpeaksWorker;
        private Waves_Data_Worker InputWavesWorker;
        //private InputWorker.BasicData InputData;
       // private InputECGWorker.Data InputECGData;
        private int NumberOfChannels;
        //private InputRpeaksWorker.Data InputRpeaksData;
        private Waves_Data InputWavesData;
        private ST_Segment_Data_Worker OutputWorker;
        private ST_Segment_Data OutputData;
        // InputData.Signals.Count NumberOfChannels;
        private Basic_Data InputData;
        private ECG_Baseline_Data InputECGData;
        private R_Peaks_Data InputRpeaksData;






        public void Abort()
        {
            Aborted = true; //przez bool
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
                InputECGWorker = new ECG_Baseline_Data_Worker(Params.AnalysisName);
                InputRpeaksWorker = new R_Peaks_Data_Worker(Params.AnalysisName);
                InputWavesWorker = new Waves_Data_Worker(Params.AnalysisName);

                InputWorker.Load();
                InputData = InputWorker.BasicData;

                InputECGWorker.Load();
                InputECGData = InputECGWorker.Data;

                InputRpeaksWorker.Load();
                InputRpeaksData = InputRpeaksWorker.Data;

                InputWavesWorker.Load();
                InputWavesData = InputWavesWorker.Data;


                OutputWorker = new ST_Segment_Data_Worker(Params.AnalysisName);
                OutputData = new ST_Segment_Data();

                _currentChannelIndex = 0;
                _samplesProcessed = 0;
                _rPeaksProcessed = 0;
                //startIndex = _samplesProcessed;
                NumberOfChannels = InputData.Signals.Count;
               
                _currentRPeaksLength = InputRpeaksData.RPeaks[_currentChannelIndex].Item2.Count;


                _currenttJ = new List<long>();
                _currenttST = new List<long>();
                _currentConcaveCurves = new int();
                _currentConvexCurves = new int();
                _currentIncreasingLines = new int();
                _currentHorizontalLines = new int();
                _currentDecreasingLines = new int();
            }
        }

        public void ProcessData(ST_Segment_Params parameters)
        {

            if (Runnable()) processData();
            else _ended = true;

        }

        public double Progress()

        {
            return 100.0 * ((double)_currentChannelIndex / (double)NumberOfChannels + (1.0 / (NumberOfChannels) * ((double)_rPeaksProcessed / (double)_currentRpeaksLength));
        }

        public bool Runnable()
        {
            return Params != null;
        }


        private void processData()
        {
            //OutputData = Method(argumenty);

            int channel = _currentChannelIndex;
            int startIndex = _rPeaksProcessed;
            int step = Params.RpeaksStep;

            if (channel < NumberOfChannels)
            {
                if (startIndex + step > _currentRpeaksLength)
                {
                   // Method(Vector<double>signal, Vector < uint > tQRS_onset, Vector < uint > tQRS_ends, Vector < double > rInterval, int freq);
                    OutputData.tJs.Add(_tJs);
                    OutputData.tSTs.Add(new List<long>(InputData.Signals[_currentChannelIndex].Item1, _tSTs));

                    //OutputData.ConcaveCurves.Add(new Tuple<string, int> (InputData.Signals[_currentChannelIndex].Item1, _ConcaveCurves));
                    //OutputData.ConcaveCurves.Add(new Tuple<string, int>(InputData.Signals[_currentChannelIndex].Item1, _ConcaveCurves));
                    OutputData.ConvexCurves=_ConvexCurves;
                    //OutputData.IncreasingLines.Add(new < int > (InputData.Signals[_currentChannelIndex].Item1, _IncreasingLines));
                    OutputData.IncreasingLines=_IncreasingLines;
                    OutputData.HorizontalLines=_HorizontalLines;
                    OutputData.DecreasingLines=_DecreasingLines;

                    _currentChannelIndex++;
                    if (_currentChannelIndex < NumberOfChannels)
                    {
                        _rPeaksProcessed = 0;

                        _currentRpeaksLength = InputRpeaksData.RPeaks[_currentChannelIndex].Item2.Count;

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

public R_Peaks_Data InputRpeaksData
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

        ST_Segment _Params param = new ST_Segment_Params("Analysis6"); 


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

