using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using EKG_Project.Modules.ECG_Baseline;
using EKG_Project.Modules.Waves;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

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

        private Heart_Axis_Data_Worker _outputWorker;
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
        //todo: wyświetl w konsoli sobie wartość Fs po przypisaniu
        private int _fs;

        /* Wyznaczone załamki QRS */

        private int[] _QArray;
        private int[] _SArray;
        private int _Q;
        private int _S;

        /*Funkcje do GUI*/

        public void Abort()
        {
            Aborted = true;
            _ended = true;
        }

        public bool IsAborted()
        {
            return Aborted;
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

                // wczytywanie danych

                // inicjalizacja zmiennych


                /* wczytanie sygnałów z odprowadzeń */
                string _analysisName = Params.AnalysisName;
                try
                {

                    InputECGBaselineWorker = new ECG_Baseline_Data_Worker(_analysisName);

                    InputECGBaselineWorker.Load();
                    InputECGBaselineData = InputECGBaselineWorker.Data;


                }
                catch (Exception e)
                {
                    Abort();
                }

                List<Tuple<string, Vector<double>>> allSignalsFiltered = InputECGBaselineData.SignalsFiltered;

                /*
                Ustawiam wszystkie zmienne przechowujące sygnał z odprowadzeń na null,
                żeby potem móc sprawdzić które zostały wykryte i wybrać na których
                będą przeprowadzane obliczenia
                */

                Lead_I = null;
                Lead_II = null;


                try
                {
                    foreach (Tuple<string, Vector<double>> lead in allSignalsFiltered)
                    {
                        String _leadName = lead.Item1;
                        if (_leadName.Equals(LeadISymbol)) // todo: sprawdzić czy o to chodziło przy wyborze odprowadzenia do obliczeń
                        {
                            Lead_I = lead.Item2.ToArray();
                        }
                        else if (_leadName.Equals(LeadIISymbol))
                        {
                            Lead_II = lead.Item2.ToArray();
                        }

                    }

                }
                catch (NullReferenceException e)
                {
                    Abort();
                }


                // przypadki - trzeba ustalić z którego odprowadzenia korzystamy jako głównego
                if ((Lead_I != null) && (Lead_II != null))
                {
                    FirstSignalName = LeadISymbol;
                    FirstSignal = Lead_I;
                }
                else
                {
                    // pozostały przypadek to taki, gdzie wszystkie są nullami - nie udało się wykryć żadnej pary odprowadzeń, wyrzucamy wyjątek
                    _ended = true;
                    Aborted = true;
                }




                // wczytywanie danych QRS


                InputWavesWorker = new Waves_Data_Worker(_analysisName); // todo: nie mam bladego pojęcia czy to jest poprawne - oni dostają nazwę analizy z GUI, a nie widzę nigdzie w kodzie tego
                InputWavesWorker.Load();
                InputWavesData = InputWavesWorker.Data; // todo: czy tu nie powinno być Waves_Data?;



                List<Tuple<string, List<int>>> allQRSOnSets = InputWavesData.QRSOnsets;
                List<Tuple<string, List<int>>> allQRSEnds = InputWavesData.QRSEnds;



                /*wczytywnie list załamków */

                QArray = null;
                SArray = null;

                // QRSOnsets
                try
                {
                    foreach (Tuple<String, List<int>> lead in allQRSOnSets) // pętla po sygnałach z odprowadzeń
                    {
                        String _leadName = lead.Item1;
                        if (_leadName.Equals(FirstSignalName))
                        {
                            QArray = lead.Item2.ToArray(); ;
                            break;
                        }

                    }
                }
                catch (NullReferenceException e)
                {
                    Abort();
                }

                // QRSEnds

                try
                {
                    foreach (Tuple<String, List<int>> lead in allQRSEnds) // pętla po sygnałach z odprowadzeń
                    {

                        String _leadName = lead.Item1;
                        if (_leadName.Equals(FirstSignalName))
                        {
                            SArray = lead.Item2.ToArray();
                            break;
                        }

                    }
                }
                catch (NullReferenceException e)
                {
                    Abort();
                }

                if ((QArray == null) || (SArray == null))
                {
                    Abort();
                }

                Q = 0;
                S = 0;

                // sprawdzanie, czy Q i S jest poprawne

                try
                {
                    for (int QIndex = 0; QIndex < QArray.Length; QIndex++)
                    {
                        Q = QArray[QIndex];
                        S = SArray[QIndex];
                        if ((Q < S) && (Q != -1) && (S != -1))
                        {
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Abort();
                }

                if ((Q == -1) || (S == -1) || (Q >= S))
                {
                    _ended = true;
                    Aborted = true;
                }

                // wczytywanie częstotliwości próbkowania

                try
                {

                    InputBasicDataWorker = new Basic_Data_Worker(_analysisName);
                    InputBasicDataWorker.Load();
                    InputBasicData = InputBasicDataWorker.BasicData;


                    Fs = (int)InputBasicData.Frequency;
                }
                catch (Exception e)
                {
                    Abort();
                }

                // dane wyjściowe - inicjalizacja

                OutputWorker = new Heart_Axis_Data_Worker(Params.AnalysisName);
                OutputData = new Heart_Axis_Data();
            }

        }

        public void ProcessData()
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

            double[] pseudo_tab = PseudoModule(Q, S, FirstSignal);
            double[] fitting_parameters = LeastSquaresMethod(FirstSignal, Q, pseudo_tab, Fs);
            int MaxOfPoly = MaxOfPolynomial(Q, fitting_parameters);
            double[] amplitudes = ReadingAmplitudes(Lead_I, Lead_II, MaxOfPoly);
            OutputData.HeartAxis = IandII(amplitudes);

            OutputWorker.Save(OutputData);
            _ended = true;


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

        public Waves_Data InputWavesData
        {
            get
            {
                return _inputWavesData;
            }
            set
            {
                _inputWavesData = value;
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

        public Heart_Axis_Data OutputData
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

        public int[] QArray
        {
            get
            {
                return _QArray;
            }

            set
            {
                _QArray = value;
            }

        }

        public int[] SArray
        {
            get
            {
                return _SArray;
            }

            set
            {
                _SArray = value;
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

        public double[] Lead_I
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

        public double[] Lead_II
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



        public string FirstSignalName
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



        public Heart_Axis_Data_Worker OutputWorker
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

        /* Test */

        public static void Main()
        {


            Heart_Axis_Params param = new Heart_Axis_Params("TestAnalysis");
            //TestModule3_Params param = null;
            Heart_Axis testModule = new Heart_Axis();
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