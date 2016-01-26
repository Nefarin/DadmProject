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
        List<string> NamesOfLeads;
        Vector<double> _Rpeaks;


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

        List<Tuple<string, Vector<double>>> _rrintervals;
        List<Tuple<string, Vector<double>>> _rpeaks;
        List<Tuple<int,int>> _class;

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
                    Console.WriteLine("Częstotliwość: ");
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
                    foreach (Tuple<string, Vector<double>> _licznik in _rpeaks)
                    {
                        if (_licznik.Item1 == "I")
                        {
                            //Console.WriteLine(_licznik.Item2);
                            _Rpeaks = _licznik.Item2;
                            Console.WriteLine(_Rpeaks);
                        }
                    }

                    bool jest= HRT_Algorythms.IsLengthOfTachogramOK(_Rpeaks);
                    Console.WriteLine(jest);
                }
                catch (Exception e)
                {
                    Abort();
                }
                //Console.WriteLine(NamesOfLeads[1]);





                //try
                //    {
                //        foreach (Tuple<String, List<int>> lead in allQRSOnSets) // pętla po sygnałach z odprowadzeń
                //        {
                //            String _leadName = lead.Item1;
                //            if (_leadName.Equals(FirstSignalName))
                //            {
                //                QArray = lead.Item2.ToArray(); ;
                //                break;
                //            }

                //        }
                //    }
                //    catch (NullReferenceException e)
                //    {
                //        Abort();
                //    }






                //InputHeartClassWorker = new Heart_Class_Data_Worker(_analysisName);
                //InputHeartClassWorker.Load();
                //InputHeartClassData = InputHeartClassWorker.Data;

                //OutputWorker = new HRT_Data_Worker(_analysisName);
                //OutputData = new HRT_Data();

                //_currentChannelIndex = 0;
                //_samplesProcessed = 0;




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

            
                

                //Console.WriteLine("RR piki");
                //foreach (Tuple<string, Vector<double>> _licznik in _rpeaks)
                //{
                //    Console.WriteLine(_licznik.Item1);
                //    Console.WriteLine(_licznik.Item2);
                //}

                //_class = InputHeartClassData.ClassificationResult;
                //Console.WriteLine("Klasy QRS");
                //foreach (Tuple<int, int> _licznik in _class)
                //{
                //    Console.WriteLine(_licznik.Item1);
                //    //Console.WriteLine(_licznik.Item2);
                //}
           
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
            get
            {
                return _fs;
            }

            set
            {
                _fs = value;
            }

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
