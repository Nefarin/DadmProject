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
        Vector<double> _rrintervalsSelected;
        int[] _classSelected;
        List<Tuple<string, Vector<double>>> _rrintervals;
        List<Tuple<string, Vector<double>>> _rpeaks;
        List<Tuple<int, int>> _class;
        Vector<double> _rpeaksTime;
        Vector<double> _classTime;
        int _VPCcount;
        int[] _nrVPC;
        Tuple<double[,], double[], double[]> FinalResults;
        double _turbulenceOnsetMean;
        double _turbulenceSlopeMean;
        double _turbulenceSlopeMax;
        double _turbulenceSlopeMin;
        double _turbulenceOnsetMax;
        double _turbulenceOnsetMin;
        double[,] _tachogram;
        double[] _turbulenceSlope;
        double[] _turbulenceOnset;




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

        public void Init(ModuleParams parameters) {

            Params = parameters as HRT_Params;
            string _analysisName = Params.AnalysisName;
            Aborted = false;
            if (!Runnable() ) _ended = true;
            else {
                _ended = false;

                try
                {
                    InputBasicDataWorker = new Basic_Data_Worker(_analysisName);
                    InputBasicDataWorker.Load();
                    InputBasicData = InputBasicDataWorker.BasicData;

                    Fs = (int)InputBasicData.Frequency;
                    //Console.Write("Częstotliwość: ");
                    //Console.WriteLine(Fs);
                }
                catch (Exception e)
                {
                    Abort();
                }

                try
                {
                    InputRpeaksWorker = new R_Peaks_Data_Worker(_analysisName);
                    InputRpeaksWorker.Load();
                    InputRpeaksData = InputRpeaksWorker.Data;
                    _rpeaks = InputRpeaksData.RPeaks;
                    _rrintervals = InputRpeaksData.RRInterval;

                    //selekcja kanałów, na razie biorę tylko I
                    foreach (Tuple<string, Vector<double>> _licznik in _rpeaks)
                    {
                        if (_licznik.Item1 == "I") {
                            _rpeaksSelected = _licznik.Item2;
                        }
                        else {; }
                    }
                    foreach (Tuple<string, Vector<double>> _licznik in _rrintervals)
                    {
                        if (_licznik.Item1 == "I")
                        { 
                            _rrintervalsSelected = _licznik.Item2;
                        }
                        else {; }
                    }
                }
                catch (Exception e)
                {
                    Abort();
                }




                try
                {
                    InputHeartClassWorker = new Heart_Class_Data_Worker(_analysisName);
                    InputHeartClassWorker.Load();
                    InputHeartClassData = InputHeartClassWorker.Data;
                    _class = InputHeartClassData.ClassificationResult;


                    List<int> Klasy = new List<int>();
                    foreach (Tuple<int, int> _licznik in _class)
                    {
                        if (_licznik.Item2 == 1)
                        {
                            Klasy.Add(_licznik.Item1);

                        }
                        else {; }
                    }
                     _classSelected = Klasy.ToArray();
            
                    _VPCcount = _classSelected.Length;
                    if (_classSelected.Length == 0)
                    {
                        Console.WriteLine("Brak załamków VPC");
                    }
                    else
                    {
                        //Console.Write("Jest ");
                        //Console.Write(_VPCcount);
                        //Console.WriteLine(" załamków VPC");
                    }
                }
                catch (Exception e)
                {
                    Abort();
                }


                _rpeaksSelected = HRT_Algorythms.rrTimesShift(_rpeaksSelected);
                _classSelected = HRT_Algorythms.checkVPCifnotNULL(_classSelected);
                _nrVPC = HRT_Algorythms.GetNrVPC(_rpeaksSelected.ToArray(), _classSelected, _VPCcount);

            }
        }

        public void ProcessData()
        {
            if (Runnable()) processData();
            else _ended = true;
        }
      
        public double Progress()
        {
            return 0;
           // return 100.0 * ((double)_currentChannelIndex / (double)NumberOfChannels + (1.0 / NumberOfChannels) * ((double)_samplesProcessed / (double)_currentChannelLength));
        }

        public bool Runnable() {
            return Params != null;
        }

        public bool IsAborted() {
            return Aborted;
        }

        private void processData()
        {

            //HRT_Algorythms.PrintVector(_rpeaksSelected);
            // HRT_Algorythms.PrintVector(_rrintervalsSelected);
            //HRT_Algorythms.PrintVector(_classSelected);
            //HRT_Algorythms.PrintVector(_nrVPC);
            FinalResults = HRT_Algorythms.MakeTachogram(_nrVPC, _rrintervalsSelected.ToArray());
            HRT_Algorythms.PrintVector(FinalResults.Item3);

            //_turbulenceOnsetMean = (HRT_Algorythms.CountMean(FinalResults.Item2));
            //Console.Write("Turbulence Onset Mean: ");
            //Console.WriteLine(_turbulenceOnsetMean);

            //_turbulenceSlopeMean = HRT_Algorythms.CountMean(FinalResults.Item3);
            //Console.Write("Turbulence Slope Mean: ");
            //Console.WriteLine(_turbulenceSlopeMean);

            //_turbulenceOnsetMax = HRT_Algorythms.CountMax(FinalResults.Item2);
            //Console.Write("Turbulence Onset Max: ");
            //Console.WriteLine(_turbulenceOnsetMax);

            //_turbulenceOnsetMin = HRT_Algorythms.CountMin(FinalResults.Item2);
            //Console.Write("Turbulence Onset Min ");
            //Console.WriteLine(_turbulenceOnsetMin);

            //_turbulenceSlopeMax = HRT_Algorythms.CountMax(FinalResults.Item3);
            //Console.Write("Turbulence Slope Max: ");
            //Console.WriteLine(_turbulenceSlopeMax);

            //_turbulenceSlopeMin = HRT_Algorythms.CountMin(FinalResults.Item3);
            //Console.Write("Turbulence Slope Min: ");
            //Console.WriteLine(_turbulenceSlopeMin);


            Tuple<string, double[,]> t3 = Tuple.Create("I", FinalResults.Item1);
            List<Tuple<string, double[,]>> _tachogram = new List<Tuple<string, double[,]>>();
            _tachogram.Add(t3);

            Tuple<string, double[]> t4 = Tuple.Create("I", FinalResults.Item2);
            List<Tuple<string, double[]>> _turbulenceOnset = new List<Tuple<string, double[]>>();
            _turbulenceOnset.Add(t4);

            Tuple<string, double[]> t5 = Tuple.Create("I", FinalResults.Item3);
            List<Tuple<string, double[]>> _turbulenceSlope = new List<Tuple<string, double[]>>();
            _turbulenceOnset.Add(t5);

            Tuple<string, double, double, double> t1 = Tuple.Create("I", _turbulenceOnsetMean, _turbulenceOnsetMax, _turbulenceOnsetMin);
            List<Tuple<string, double, double, double>> _tachogramOnsetValues = new List<Tuple<string, double, double, double>>();
            _tachogramOnsetValues.Add(t1);

            Tuple<string, double, double, double> t2 = Tuple.Create("I", _turbulenceSlopeMean, _turbulenceSlopeMax, _turbulenceSlopeMin);
            List<Tuple<string, double, double, double>> _tachogramSlopeValues = new List<Tuple<string, double, double, double>>();
            _tachogramSlopeValues.Add(t2);
            

            _ended = true;
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
                //Console.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }
        }

    }
}
