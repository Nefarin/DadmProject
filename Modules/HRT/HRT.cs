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
        //zmienne
        private bool _ended;
        private bool _aborted;
        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _samplesProcessed;
        private int _numberOfChannels;
        private int _fs;
        Vector<double> _rpeaksSelected;
        Vector<double> _rrintervalsSelected;
        int [] _classSelected;
        List<Tuple<string, Vector<double>>> _rrintervals;
        List<Tuple<string, Vector<double>>> _rpeaks;
        List<Tuple<int, int>> _class;
        Vector<double> _rpeaksTime;
        Vector<double> _classTime;
        int _VPCcount;
        Vector<double> _nrVPC;




        //stworzenie obiektów workerów poszczególnych klas 
        private HRT_Params _params;
        private HRT_Data_Worker _outputWorker;
        private R_Peaks_Data_Worker _inputRpeaksWorker;
        private Heart_Class_Data_Worker _inputHeartClassWorker;
        private Basic_Data_Worker _inputBasicDataWorker;
        private Basic_Data _inputBasicData;
        HRT_Alg HRT_Algorythms = new HRT_Alg();

        //stworzenie obiektów danych poszczególnych klas
        private HRT_Data _outputData;
        private R_Peaks_Data _inputRpeaksData;
        private Heart_Class_Data _inputHeartClassData;

        

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
                    Console.Write("Częstotliwość: ");
                    Console.WriteLine(Fs);
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
                            Console.Write("Jest ");
                            Console.Write(_VPCcount);
                            Console.WriteLine(" załamków VPC");
                        }
                    }
                    catch (Exception e)
                    {
                        Abort();
                    }


                //_currentChannelIndex = 0;
                //_samplesProcessed = 0;
                //HRT_Algorythms.PrintVector(_rpeaksSelected);
                _rpeaksTime =HRT_Algorythms.ChangeVectorIntoTimeDomain(_rpeaksSelected, _fs);
                _classTime = HRT_Algorythms.ChangeVectorIntoTimeDomain(_classSelected, _fs);



                //double[] _rpeaksTimeArray;
                //double[] _classTimeArray;
                //_rpeaksTimeArray = _rpeaksTime.ToArray();
                //_classTimeArray =_classTime.ToArray()

                _nrVPC = HRT_Algorythms.GetNrVPC(_rpeaksTime.ToArray(), _classTime.ToArray(), _VPCcount);


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
            HRT_Algorythms.PrintVector(_rpeaksTime);
            // HRT_Algorythms.PrintVector(_rrintervalsSelected);
            Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
            HRT_Algorythms.PrintVector(_classTime);
            //HRT_Algorythms.PrintVector(_nrVPC);


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
                Console.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }
        }

    }
}
