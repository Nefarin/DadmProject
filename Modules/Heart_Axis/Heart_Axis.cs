using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using EKG_Project.Modules.ECG_Baseline;
using EKG_Project.Modules.Waves;
using MathNet.Numerics;

namespace EKG_Project.Modules.Heart_Axis
{
    public partial class Heart_Axis : IModule
    {

        /* Zmienne do komunikacji z GUI - czy liczenie osi serca się skończyło/zostało przerwane */

        private bool _ended;
        private bool _aborted;

        /* Stałe */

        private const string LeadISymbol = "I";
        private const string LeadIISymbol = "II";

        /* Dane wejściowe */

        /* Sygnał - moduł ECG_BASELINE */
        /* z klasy IO/ECG_Baseline_Data_Worker */

        private ECG_Baseline_Data_Worker _inputECGBaselineWorker;  // obiekt do wczytania sygnału EKG
        private ECG_Baseline_Data _inputECGBaselineData; // obiekt z sygnałem

        /* Wyznaczone próbki z załamkami Q i S - moduł WAVES */
        /* z klasy IO/Waves_Data_Worker.cs */

        private Waves_Data_Worker _inputWavesWorker; // obiekt do wczytywania załamków
        private Waves_Data _inputWavesData; // obiekt z załamkami

        // Dane z częstotliwością próbkowania

        private Basic_Data_Worker _inputBasicDataWorker;
        private Basic_Data _inputBasicData;

        /* Dane wyjściowe - Kąt osi serca  */

        private Heart_Axis_Data_Worker _output;
        private Heart_Axis_Data _outputData;
        private Heart_Axis_Params _params; // tak ma być?

        /* Dane tymczasowe potrzebne do obliczeń */

        /* Sygnały z odprowadzeń potrzebnych do policzenia osi (I, II, III) */

        private double[] _lead_I;
        private double[] _lead_II;

        /* Zmienne do ustalenia, na którym sygnale są główne obliczenia, a na który jest tylko używany na koniec */
        private double[] _firstSignal;
        private double[] _secondSignal;
        private string _firstSignalName; // nazwa odprowadzenia, które wybraliśmy jako główne przy obliczeniach

        /* Częstotliwość próbkowania sygnału */
        //TODO:  2) w module Waves korzysta się z Basic_Data i ustawia pole Frequency, może stąd trzeba wziąć fs?
        //todo: wyświetl w konsoli sobie wartość Fs po przypisaniu
        private int _fs;

        /* Wyznaczone załamki QRS */

        private int[] _QArray;
        private int[] _SArray;
        private int _Q; //uint??
        private int _S; //uint??
        
        /*Funkcje do GUI*/

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
            Params = parameters as Heart_Axis_Params;
            Aborted = false;
            if (!Runnable()) _ended = true;
            else
            {
                _ended = false;
                // HMM!

            }
        }

        public void ProcessData(int numberOfSamples)
        {
            if (Runnable()) processData();
            else _ended = true;
        }

        
        public double Progress()
        {
            return 100;
        }

        public bool Runnable()
        {

            return Params != null;
        }

        /* Obliczenie osi serca */

        private void processData()
        {

            // todo: wstawić kolejne etapy obliczania osi serca

            
        }

        /* Przepływ sterowania dla GUI */

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
        
         // Parametry
 
        public Heart_Axis_Params Params
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
 
        // Data
       /*
        public Waves_Data InputWavesData
        {
            get
            {
                return _inputWavesData;
            }
            set
            {
                inputWavesData = value;
            }
        }
       
        public ECG_Baseline_Data InputECGBaselineData
        {
            get
            {
                return _inputECGBaselineData;
            }
 
            set
            {
                _inputECGBaselineData = value;
            }
        }
       
        public Basic_Data InputBasicData
        {
            get
            {
                return _inputBasicData;
            }
 
            set
            {
                _inputBasicData = value;
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
       
        public List<int> QRSonsets
        {
            get
            {
                return _QRSonsets;
            }
 
            set
            {
                _QRSonsets = value;
            }
           
        }
               
        public List<int> QRSends
        {
            get
            {
                return _QRSends;
            }
 
            set
            {
                _QRSends = value;
            }
           
        }
       
        public int Q
        {
            get
            {
                return _Q;
            }
 
            set
            {
                _Q = value;
            }
        }
       
        public int S
        {
            get
            {
                return _S;
            }
 
            set
            {
                _S = value;
            }
        }
       
        public Vector<double> Lead_I
        {
            get
            {
                return _lead_I;
            }
 
            set
            {
                _lead_I = value;
            }
           
        }
       
        public Vector<double> Lead_II
        {
            get
            {
                return _lead_II;
            }
 
            set
            {
                _lead_II = value;
            }
           
        }
       
        public Vector<double> Lead_III
        {
            get
            {
                return _lead_III;
            }
 
            set
            {
                _lead_III = value;
            }
           
        }
       
        public Vector<double> FirstSignalName
        {
            get
            {
                return _firstSignalName;
            }
 
            set
            {
                _firstSignalName = value;
            }
           
        }
       
        // Workers
       
        public ECG_Baseline_Data_Worker InputECGBaselineWorker
        {
            get
            {
                return _inputECGBaselineWorker;
            }
 
            set
            {
                _inputECGBaselineWorker = value;
            }
        }
       
        public Waves_Data_Worker InputWavesWorker
        {
            get
            {
                return _inputWavesWorker;
            }
 
            set
            {
                _inputWavesWorker = value;
            }
        }
       
        public Basic_Data_Worker InputBasicDataWorker
        {
            get
            {
                return _inputBasicDataWorker;
            }
 
            set
            {
                _inputBasicDataWorker = value;
            }
        }
           
 
       
        public Heart_Data_Worker OutputWorker
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
       */
        public double[] FirstSignal
         {
            get
            {
                return _firstSignal;
            }
 
            set
            {
                _firstSignal = value;
            }
        }
       
        public double[] SecondSignal
         {
            get
            {
                return _secondSignal;
            }
 
            set
            {
                _secondSignal = value;
            }
        }
         
       

        // tu można dać kod do testów
        /*
        public static void Main()
        {
            // TODO: tuta trzeba dopisać testy

            // 1. Parametry dla modułu?
            ECG_Baseline_Params param = new ECG_Baseline_Params();

            // 2. Wczytanie plików?

            //TempInput.setInputFilePath(@"C:\ścieżka.txt");
            //TempInput.setOutputFilePath(@"C:\ścieżka.txt");
            //Vector<double> ecg = TempInput.getSignal();

            //TempInput.setInputFilePath(@"C:\ścieżka.txt");
            //Vector<double> rpeaks = TempInput.getSignal();


            // 3. Stworzenie obiketu modułu
            Heart_Axis testModule = new Heart_Axis();
            //testModule.InitForTestsOnly(ecg, rpeaks, param);

            // 4. Inicjalizacja danych wejściowych
            testModule.Init(param);
            while (true)
            {
                //Console.WriteLine("Press key to continue.");
                //Console.Read();

                // todo: czy to podlega tej samej logice?
                if (testModule.Ended()) break;
                Console.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }

        }*/

    }
     
}
 