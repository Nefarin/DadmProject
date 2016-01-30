using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.R_Peaks;
using EKG_Project.Modules.Heart_Class;
using EKG_Project.IO;



namespace EKG_Project.Modules.HRT
{
    public class HRT : IModule
    {
        private bool _ended;
        private bool _aborted;
        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _samplesProcessed;
        private int _numberOfChannels;
        private int _fs;
        Vector<double> _rpeaksSelected;
        Vector<double> _rrintervals;
        List<int> _classPrematureVentrical;
        List<Tuple<int, int>> _classAll;
        List<int> _nrVPC;
        double _turbulenceOnsetMean;
        double _turbulenceSlopeMean;
        double _turbulenceSlopeMax;
        double _turbulenceSlopeMin;
        double _turbulenceOnsetMax;
        double _turbulenceOnsetMin;
        List<double[]> _tachogram;
        List<int> _classVentrical;
        List<double> _turbulenceOnset;
        Tuple<List<double>,double[],double[]> _turbulenceSlope;


        //stworzenie obiektów workerów poszczególnych klas 
        private HRT_Params _params;
        private HRT_Data_Worker _outputWorker;
        private HRT_Data _outputData;
        private R_Peaks_Data_Worker _inputRpeaksWorker;
        private R_Peaks_Data _inputRpeaksData;
        private Heart_Class_Data_Worker _inputHeartClassWorker;
        private Heart_Class_Data _inputHeartClassData;
        private Basic_Data_Worker _inputBasicDataWorker;
        private Basic_Data _inputBasicData;
        
        Tuple<double[], double[]> _meanTurbulenceOnsetPLOT;
        int[] _xaxis;

        //zmienne wyjścia GUI
        Tuple<int[],List<double[]>> _tachogramPrepare;
        List<Tuple<int[], List<double[]>>> _tachogramGUI;





        HRT_Alg HRT_Algorythms = new HRT_Alg();


        public void Abort()
        {
            Aborted = true;
            _ended = true;
        }

        public bool Ended()
        {
            return _ended;
        }

        /**************************************************/
        //inicjalizacja modulu
        /**************************************************/
        public void Init(ModuleParams parameters) {

            Params = parameters as HRT_Params;
            
            string _analysisName = Params.AnalysisName;

            Aborted = false;

            if (!Runnable() ) _ended = true;
            else
            {
                _ended = false;

                /**************************************************/
                //wczytanie danych z odprowadzen
                //*************************************************/
                /**************************************************/
                //proba pobrania parametrow z modulu basic
                //*************************************************/
                try
                {
                    InputBasicDataWorker = new Basic_Data_Worker(_analysisName);
                    InputBasicDataWorker.Load();
                    InputBasicData = InputBasicDataWorker.BasicData;
                    Fs = (int)InputBasicData.Frequency;
                    Console.Write("Częstotliwość: ");
                    Console.WriteLine(Fs);
                }
                catch (Exception e)
                {
                    Abort();
                }
                                
                /**************************************************/
                //proba pobrania parametrow z modulu r_peaks
                /**************************************************/
                try
                {
                    InputRpeaksWorker = new R_Peaks_Data_Worker(_analysisName);
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
                    InputHeartClassWorker = new Heart_Class_Data_Worker(_analysisName);
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
                    OutputWorker = new HRT_Data_Worker(Params.AnalysisName);
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
                            _tachogram = HRT_Algorythms.MakeTachogram(_classPrematureVentrical, _rrintervals);
                            _turbulenceOnset = HRT_Algorythms.TurbulenceOnsetsForPDF(_classPrematureVentrical, _rrintervals);
                            _turbulenceSlope = HRT_Algorythms.PrepareTurbulenceSlopeToGUIandPLOT(_classPrematureVentrical, _rrintervals);
                            _meanTurbulenceOnsetPLOT = HRT_Algorythms.PrepareMeanTurbulenceOnsetToPLOT(_tachogram);

                            //HRT_Algorythms.PrintVector(_tachogram);

                            _xaxis = HRT_Algorythms.xPlot();
                            _tachogramPrepare = Tuple.Create(_xaxis, _tachogram);
                            _tachogramGUI.Add(_tachogramPrepare);

                            //_turbulenceOnsetMean = HRT_Algorythms.PrepareMeanTurbulenceOnsetforGUI(_tachogram);
                            ;
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
            

        
        //Accessors (getters and setters)
        public HRT_Data_Worker OutputWorker
        {
            get { return _outputWorker; }
            set { _outputWorker = value; }
        }
        public R_Peaks_Data_Worker InputRpeaksWorker
        {
            get { return _inputRpeaksWorker; }
            set { _inputRpeaksWorker = value; }
        }
        public Basic_Data_Worker InputBasicDataWorker
        {
            get { return _inputBasicDataWorker; }
            set { _inputBasicDataWorker = value; }
        }
        public HRT_Params Params
        {
            get { return _params; }
            set { _params = value; }
        }
        public Heart_Class_Data_Worker InputHeartClassWorker
        {
            get { return _inputHeartClassWorker; }
            set { _inputHeartClassWorker = value; }
        }
        public R_Peaks_Data InputRpeaksData
        {
            get { return _inputRpeaksData; }
            set { _inputRpeaksData = value; }
        }
        public Heart_Class_Data InputHeartClassData
        {
            get { return _inputHeartClassData; }
            set { _inputHeartClassData = value; }
        }
        public HRT_Data OutputData
        {
            get { return _outputData; }
            set { _outputData = value; }
        }
        public Basic_Data InputBasicData
        {
            get { return _inputBasicData; }
            set { _inputBasicData = value; }
        }
        public bool Aborted
        {
            get { return _aborted; }
            set { _aborted = value; }
        }
        public int NumberOfChannels
        {
            get { return _numberOfChannels; }
            set { _numberOfChannels = value; }
        }
        public int Fs
        {
            get { return _fs;}
            set{ _fs = value;}
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
