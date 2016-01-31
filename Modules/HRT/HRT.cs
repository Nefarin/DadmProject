using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.R_Peaks;
using EKG_Project.Modules.Heart_Class;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;

q

namespace EKG_Project.Modules.HRT
{
    public class HRT : IModule
    {
        private enum STATE { INIT, BEGIN_CHANNEL, PROCESS_FIRST_STEP, PROCESS_CHANNEL, NEXT_CHANNEL, END_CHANNEL, END };
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private string _currentLeadName;
        private string[] _leads;
        private int _currentIndex;
        private int _numberOfChannels;
        private int _samplesProcessed;
        private int _fs;

        private Basic_New_Data_Worker _inputWorker_basic;
        private R_Peaks_New_Data_Worker _inputWorker_Rpeaks;
        private Heart_Class_New_Data_Worker _inputWorker_HeartClass;
        private HRT_New_Data_Worker _outputWorker;

        private Basic_Data _inputData_basic;
        private R_Peaks_Data _inputData_Rpeaks;
        private Heart_Class_Data _inputData_HeartClass;
        private HRT_Data _outputData;
        private HRT_Params _params;


        private STATE _state;
        private HRT_Alg HRT_Algorythms;
        private Vector<double> _rpeaksSelected;
        private Vector<double> _rrintervals;
        private List<Tuple<int, int>> _classAll;
        private List<int> _classPrematureVentrical;
        private List<int> _nrVPC;
        private List<double[]> _tachogram;
        private List<int> _classVentrical;
        private List<double> _turbulenceOnset;
        private Tuple<List<double>,int[],double[]> _turbulenceSlope;
        private double[] _meanTachogram;
        private int[] _xaxis;

        private List<Tuple<string, int[], List<double[]>>> _tachogramGUI = new List<Tuple<string, int[], List<double[]>>>();
        private List<Tuple<string, int[], double[]>> _tachogramMeanGUI = new List<Tuple<string, int[], double[]>>();
        private List<Tuple<string, int[], double[]>> _turbulenceOnsetmeanGUI = new List<Tuple<string, int[], double[]>>();
        private List<Tuple<string, int[], double[]>> _turbulenceSlopeMaxGUI = new List<Tuple<string, int[], double[]>>();
        private List<Tuple<string, List<double>>> _turbulenceOnsetPDF = new List<Tuple<string, List<double>>>();
        private List<Tuple<string, List<double>>> _turbulenceSlopePDF = new List<Tuple<string, List<double>>>();

        


        //

        //Aborted = false;



        public void Abort()
        {
            Aborted = true;
            _ended = true;
        }

        public bool Ended()
        {
            return _ended;
        }

        public bool IsAborted()
        {
            return Aborted;
        }

        public void Init(ModuleParams parameters)
        {
            try
            {
                Params = parameters as HRT_Params;
            }
            catch (Exception e)
            {
                Abort();
                return;
            }

            if (!Runnable())
            {
                _ended = true;
            }
            else
            {
                InputWorker_basic = new Basic_New_Data_Worker(Params.AnalysisName);
                InputRpeaksWorker = new R_Peaks_New_Data_Worker(Params.AnalysisName);
                InputHeartClassWorker = new Heart_Class_New_Data_Worker(Params.AnalysisName);
                OutputWorker = new HRT_New_Data_Worker(Params.AnalysisName);
                InputData_basic = new Basic_Data();
                InputData = new ECG_Baseline_Data();
                OutputData = new R_Peaks_Data();



                InputBasicDataWorker.Load();
                InputBasicData = InputBasicDataWorker.BasicData;
                Fs = (int)InputBasicData.Frequency;
                Console.Write("Częstotliwość: ");
                Console.WriteLine(Fs);
            }

                                
                /**************************************************/
                //proba pobrania parametrow z modulu r_peaks
                /**************************************************/
                try
                {
                    
                    InputRpeaksWorker.Load();
                    InputRpeaksData = InputRpeaksWorker.Data;
                }
                catch (Exception e)
                {
                    Abort();
                }
                                
                /**************************************************/
                //proba pobrania parametrow z modulu heart_class
                /**************************************************/
                try
                {
                    
                    InputHeartClassWorker.Load();
                    InputHeartClassData = InputHeartClassWorker.Data;
                }
                catch (Exception e)
                {
                    Abort();
                }

                /**************************************************/
                //dane wyjściowe - inicjalizacja
                /**************************************************/
                try
                {
                    
                    OutputData = new HRT_Data();
                }
                catch (Exception e)
                {
                    Abort();
                }

                /**************************************************/
                //ustawienie liczby odprowadzen i bierzacego kanalu
                /**************************************************/
                _currentChannelIndex = 0;
                NumberOfChannels = InputRpeaksData.RPeaks.Count;

            }

    
    }

        public void ProcessData()
        {
            if (Runnable()) processData();
            else _ended = true;
        }
      
        public double Progress()
        { 
            return 100.0 * ((double)_currentChannelIndex / (double)NumberOfChannels);
        }

        public bool Runnable() {
            return Params != null;
        }

        public bool IsAborted() {
            return Aborted;
        }

        /**************************************************/
        //glowny proces wykonywania modulu
        //*************************************************/
        private void processData()
        {
            if (_currentChannelIndex < NumberOfChannels)
            {
                Console.Write(_currentChannelIndex);
                Console.Write("/");
                Console.WriteLine(NumberOfChannels);

                string _channelName = InputRpeaksData.RPeaks[_currentChannelIndex].Item1;

                _rpeaksSelected = InputRpeaksData.RPeaks[_currentChannelIndex].Item2;
                _rrintervals = InputRpeaksData.RRInterval[_currentChannelIndex].Item2;
                _classAll = InputHeartClassData.ClassificationResult;

             
                //_rpeaksSelected = HRT_Algorythms.rrTimesShift(_rpeaksSelected);


                // _classVentrical = HRT_Algorythms.checkVPCifnotNULL(_classVentrical);
                if (_rpeaksSelected.Count < _classAll.Count)
                {
                    Console.WriteLine("Wykryto więcej klas niż załamków, błędnie skonstruowany plik wejściowy");
                }
                else
                {
                    _classVentrical = HRT_Algorythms.TakeNonAtrialComplexes(_classAll);

                  
                    if (_classVentrical.Capacity == 0)
                    {
                        Console.WriteLine("Brak jakiegokolwiek załamka mającego pochodzenie komorowe");
                    }
                    else
                    {
                        _nrVPC = HRT_Algorythms.GetNrVPC(_rpeaksSelected.ToArray(), _classVentrical.ToArray());
                        _tachogram = HRT_Algorythms.MakeTachogram(_nrVPC, _rrintervals);
                        _classPrematureVentrical = HRT_Algorythms.SearchPrematureTurbulences(_tachogram, _nrVPC);
                        if (_classPrematureVentrical.Capacity == 0)
                        {
                            Console.WriteLine("Są komorowe załamki, ale nie ma przedwczesnych");
                        }
                        else
                        {
                            Tuple<int[], double[]> _meanTurbulenceOnset;
                            

                            _tachogram = HRT_Algorythms.MakeTachogram(_classPrematureVentrical, _rrintervals);
                            _turbulenceOnset = HRT_Algorythms.TurbulenceOnsetsPDF(_classPrematureVentrical, _rrintervals);
                            _turbulenceSlope = HRT_Algorythms.TurbulenceSlopeGUIandPDF(_classPrematureVentrical, _rrintervals);

                            



                            _meanTurbulenceOnset = HRT_Algorythms.TurbulenceOnsetMeanGUI(_tachogram);
                            _meanTachogram = HRT_Algorythms.MeanTachogram(_tachogram);

           


                            _xaxis = HRT_Algorythms.xPlot();
                           



                            _tachogramGUI.Add(Tuple.Create(_channelName, _xaxis, _tachogram));
                            _tachogramMeanGUI.Add(Tuple.Create(_channelName, _xaxis, _meanTachogram));
                            _turbulenceOnsetmeanGUI.Add(Tuple.Create(_channelName, _meanTurbulenceOnset.Item1, _meanTurbulenceOnset.Item2));
                            _turbulenceSlopeMaxGUI.Add(Tuple.Create(_channelName, _turbulenceSlope.Item2, _turbulenceSlope.Item3));


                            _turbulenceOnsetPDF.Add(Tuple.Create(_channelName, _turbulenceOnset));
                            _turbulenceSlopePDF.Add(Tuple.Create(_channelName, _turbulenceSlope.Item1));


                            HRT_Algorythms.PrintVector(_turbulenceSlopeMaxGUI);

                        }
                    }
                }
            }
            else
            {
               // _outputData._TurbulenceOnset.Add();
               // _outputData._VPCtachogram.Add(FinalResults.Item1);
                //NASZE DANE WYJSCIOWE PRZYGOTOWAC I WYSLAC DO ZAPISU
                //OutputWorker.Save(OutputData);
                _ended = true;
            }

            _currentChannelIndex++;


           

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

    public Basic_New_Data_Worker InputWorker_basic
    {
        get
        {
            return _inputWorker_basic;
        }

        set
        {
            _inputWorker_basic = value;
        }
    }

    public R_Peaks_New_Data_Worker InputWorker_Rpeaks
    {
        get
        {
            return _inputWorker_Rpeaks;
        }

        set
        {
            _inputWorker_Rpeaks = value;
        }
    }

    public Heart_Class_New_Data_Worker InputWorker_HeartClass
    {
        get
        {
            return _inputWorker_HeartClass;
        }

        set
        {
            _inputWorker_HeartClass = value;
        }
    }

    public HRT_New_Data_Worker OutputWorker
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

    public Basic_Data InputData_basic
    {
        get
        {
            return _inputData_basic;
        }

        set
        {
            _inputData_basic = value;
        }
    }

    public R_Peaks_Data InputData_Rpeaks
    {
        get
        {
            return _inputData_Rpeaks;
        }

        set
        {
            _inputData_Rpeaks = value;
        }
    }

    public Heart_Class_Data InputData_HeartClass
    {
        get
        {
            return _inputData_HeartClass;
        }

        set
        {
            _inputData_HeartClass = value;
        }
    }

    public HRT_Data OutputData
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

    public HRT_Params Params
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


    public static void Main() {

            HRT_Params param = new HRT_Params("TestAnalysisEgz");   

            HRT testModule = new HRT();
            testModule.Init(param);
            while (true)
            {
                if (testModule.Ended()) break;
                Console.Write("Progress: ");
                Console.Write(testModule.Progress());
                Console.WriteLine(" %");
                testModule.ProcessData();
            }
        }

    }
}
