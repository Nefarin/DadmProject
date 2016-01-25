using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
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

        //stworzenie obiektów workerów poszczególnych klas 
        private HRT_Params _params;
        private HRT_Data_Worker _outputWorker;
        private R_Peaks_Data_Worker _inputRpeaksWorker;
        private Heart_Class_Data_Worker _inputHeartClassWorker;

        //stworzenie obiektów danych poszczególnych klas
        private HRT_Data _outputData;
        private R_Peaks_Data _inputRpeaksData;
        private Heart_Class_Data _inputHeartClassData;

        List<Tuple<string, Vector<double>>> _rrintervals;




        //interfejs - metody HRT (maszyny stanow)
        public void Abort() {
            Aborted = true;
            _ended = true;
        }

        public bool Ended() {
            return _ended;
        }

        public void Init(ModuleParams parameters) {

            Params = parameters as HRT_Params;
            Aborted = false;
            if (!Runnable() ) _ended = true;
            else {
                _ended = false;

                InputRpeaksWorker = new R_Peaks_Data_Worker(Params.AnalysisName);
                InputRpeaksWorker.Load();
                InputRpeaksData = InputRpeaksWorker.Data;

                InputHeartClassWorker = new Heart_Class_Data_Worker(Params.AnalysisName);
                InputHeartClassWorker.Load();
                InputHeartClassData = InputHeartClassWorker.Data;

                OutputWorker = new HRT_Data_Worker(Params.AnalysisName);
                OutputData = new HRT_Data();

                _currentChannelIndex = 0;
                _samplesProcessed = 0;
            }
        }

        public void ProcessData()
        {
            if (Runnable()) processData();
            else _ended = true;
        }
        int i = 0;
        public double Progress()
        {
            return i++;
           // return 100.0 * ((double)_currentChannelIndex / (double)NumberOfChannels + (1.0 / NumberOfChannels) * ((double)_samplesProcessed / (double)_currentChannelLength));
        }

        public bool Runnable() {
            return Params != null;
        }

        public bool IsAborted() {
            return Aborted;
        }

        private void processData() {
            
            _rrintervals = InputRpeaksData.RRInterval;
            Console.WriteLine(_rrintervals.Count);
            foreach (Tuple<string, Vector<double>> _licznik in _rrintervals){

                Console.WriteLine(_licznik.Item2);
                _ended = true;
            }

        }

            

        //Wlasciwosci (gettery i settery)
        public HRT_Data_Worker OutputWorker { get { return _outputWorker; } set { _outputWorker = value; } }
        public R_Peaks_Data_Worker InputRpeaksWorker { get { return _inputRpeaksWorker; } set { _inputRpeaksWorker = value; } }
        public HRT_Params Params { get { return _params; } set { _params = value; } }
        public Heart_Class_Data_Worker InputHeartClassWorker { get { return _inputHeartClassWorker; } set { _inputHeartClassWorker = value; } }
        public R_Peaks_Data InputRpeaksData { get { return _inputRpeaksData; } set { _inputRpeaksData = value; } }
        public Heart_Class_Data InputHeartClassData { get { return _inputHeartClassData; } set { _inputHeartClassData = value; } }
        public HRT_Data OutputData { get { return _outputData; } set { _outputData = value; } }
        public bool Aborted { get { return _aborted; } set { _aborted = value; } }
        public int NumberOfChannels { get { return _numberOfChannels; } set { _numberOfChannels = value; } }



        public static void Main() {

            HRT_Params param = new HRT_Params("TestAnalysis8"); 

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