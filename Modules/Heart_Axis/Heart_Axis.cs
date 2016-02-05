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
    public class Heart_Axis : IModule
    {

        private enum STATE { INIT, BEGIN_CHANNEL, PROCESS_FIRST_STEP, PROCESS_CHANNEL, NEXT_CHANNEL, END_CHANNEL, END };
        private STATE _state;
        private int _step;
        Heart_Axis_Alg _alg;

        //todo: sprawdzić, czy nie pokrywają się nazwy
        private int _currentChannelIndex;
        private int _currentChannelLength;
        private string _currentLeadName;
        private string[] _leads;
        private int _currentIndex;
        private int _numberOfChannels;

        /* Zmienne do komunikacji z GUI - czy liczenie osi serca się skończyło/zostało przerwane */

        private bool _ended;
        private bool _aborted;

        /* Stałe */

        private const string LeadISymbol = "I";
        private const string LeadIISymbol = "II";

        /* Dane wejściowe */


        //todo: upewnić się, że zmiany na New_Data_Worker nie wprowadzają dodatkowych zmian


        private ECG_Baseline_New_Data_Worker _inputECGBaselineWorker; // obiekt do wczytania sygnału EKG
        private ECG_Baseline_Data _inputECGBaselineData; // obiekt z sygnałem

        /* Wyznaczone próbki z załamkami Q i S - moduł WAVES */
        /* z klasy IO/Waves_Data_Worker.cs */

        private Waves_New_Data_Worker _inputWavesWorker; // obiekt do wczytywania załamków
        private Waves_Data _inputWavesData; // obiekt z załamkami


        // Dane z częstotliwością próbkowania

        private Basic_New_Data_Worker _inputBasicDataWorker;
        private Basic_New_Data _inputBasicData;

        /* Dane wyjściowe - Kąt osi serca  */

        private Heart_Axis_New_Data_Worker _outputWorker;
        private Heart_Axis_Data _outputData;
        private Heart_Axis_Params _params;

        /* Dane tymczasowe potrzebne do obliczeń */

        /* Sygnały z odprowadzeń potrzebnych do policzenia osi (I, II, III) */

        private double[] _lead_I;
        private double[] _lead_II;

        /* Zmienne do ustalenia, na którym sygnale są główne obliczenia, a na który jest tylko używany na koniec */
        private double[] _firstSignal;
        private double[] _secondSignal;
        private string _firstSignalName; // nazwa odprowadzenia, które wybraliśmy jako główne przy obliczeniach

        /* Częstotliwość próbkowania sygnału */
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

            try
            {
                _params = parameters as Heart_Axis_Params;
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
                string _analysisName = Params.AnalysisName;
                InputECGBaselineWorker = new ECG_Baseline_New_Data_Worker(_analysisName);

                InputWavesWorker = new Waves_New_Data_Worker(_analysisName);
                InputWavesData = new Waves_Data();

                InputBasicDataWorker = new Basic_New_Data_Worker(_analysisName);
               InputBasicData = new Basic_New_Data();


                OutputWorker = new Heart_Axis_New_Data_Worker(Params.AnalysisName);
                OutputData = new Heart_Axis_Data();
                _state = STATE.INIT;
            }
               
                //    _ended = false;

                //    // wczytywanie danych

                //    // inicjalizacja zmiennych


                //    /* wczytanie sygnałów z odprowadzeń */
                //    string _analysisName = Params.AnalysisName;
                //    try
                //    {

                //        InputECGBaselineWorker = new ECG_Baseline_Data_Worker(_analysisName);

                //        InputECGBaselineWorker.Load();
                //        InputECGBaselineData = InputECGBaselineWorker.Data;


                //    }
                //    catch (Exception e)
                //    {
                //        Abort();
                //    }

                //    List<Tuple<string, Vector<double>>> allSignalsFiltered = InputECGBaselineData.SignalsFiltered;

                //    /*
                //    Ustawiam wszystkie zmienne przechowujące sygnał z odprowadzeń na null,
                //    żeby potem móc sprawdzić które zostały wykryte i wybrać na których
                //    będą przeprowadzane obliczenia
                //    */

                //    Lead_I = null;
                //    Lead_II = null;


                //    try
                //    {
                //        foreach (Tuple<string, Vector<double>> lead in allSignalsFiltered)
                //        {
                //            String _leadName = lead.Item1;
                //            if (_leadName.Equals(LeadISymbol)) // todo: sprawdzić czy o to chodziło przy wyborze odprowadzenia do obliczeń
                //            {
                //                Lead_I = lead.Item2.ToArray();
                //            }
                //            else if (_leadName.Equals(LeadIISymbol))
                //            {
                //                Lead_II = lead.Item2.ToArray();
                //            }

                //        }

                //    }
                //    catch (NullReferenceException e)
                //    {
                //        Abort();
                //    }


                //    // przypadki - trzeba ustalić z którego odprowadzenia korzystamy jako głównego
                //    if ((Lead_I != null) && (Lead_II != null))
                //    {
                //        FirstSignalName = LeadISymbol;
                //        FirstSignal = Lead_I;
                //    }
                //    else
                //    {
                //        // pozostały przypadek to taki, gdzie wszystkie są nullami - nie udało się wykryć żadnej pary odprowadzeń, wyrzucamy wyjątek
                //        _ended = true;
                //        Aborted = true;
                //    }




                //    // wczytywanie danych QRS


                //    InputWavesWorker = new Waves_Data_Worker(_analysisName); // todo: nie mam bladego pojęcia czy to jest poprawne - oni dostają nazwę analizy z GUI, a nie widzę nigdzie w kodzie tego
                //    InputWavesWorker.Load();
                //    InputWavesData = InputWavesWorker.Data; // todo: czy tu nie powinno być Waves_Data?;



                //    List<Tuple<string, List<int>>> allQRSOnSets = InputWavesData.QRSOnsets;
                //    List<Tuple<string, List<int>>> allQRSEnds = InputWavesData.QRSEnds;



                //    /*wczytywnie list załamków */

                //    QArray = null;
                //    SArray = null;

                //    // QRSOnsets
                //    try
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

                //    // QRSEnds

                //    try
                //    {
                //        foreach (Tuple<String, List<int>> lead in allQRSEnds) // pętla po sygnałach z odprowadzeń
                //        {

                //            String _leadName = lead.Item1;
                //            if (_leadName.Equals(FirstSignalName))
                //            {
                //                SArray = lead.Item2.ToArray();
                //                break;
                //            }

                //        }
                //    }
                //    catch (NullReferenceException e)
                //    {
                //        Abort();
                //    }

                //    if ((QArray == null) || (SArray == null))
                //    {
                //        Abort();
                //    }

                //    Q = 0;
                //    S = 0;

                //    // sprawdzanie, czy Q i S jest poprawne

                //    try
                //    {
                //        for (int QIndex = 0; QIndex < QArray.Length; QIndex++)
                //        {
                //            Q = QArray[QIndex];
                //            S = SArray[QIndex];
                //            if ((Q < S) && (Q != -1) && (S != -1))
                //            {
                //                break;
                //            }
                //        }
                //    }
                //    catch (Exception e)
                //    {
                //        Abort();
                //    }

                //    if ((Q == -1) || (S == -1) || (Q >= S))
                //    {
                //        _ended = true;
                //        Aborted = true;
                //    }

                //    // wczytywanie częstotliwości próbkowania

                //    try
                //    {

                //        InputBasicDataWorker = new Basic_Data_Worker(_analysisName);
                //        InputBasicDataWorker.Load();
                //        InputBasicData = InputBasicDataWorker.BasicData;


                //        Fs = (int)InputBasicData.Frequency;
                //    }
                //    catch (Exception e)
                //    {
                //        Abort();
                //    }

                //    // dane wyjściowe - inicjalizacja

                //    OutputWorker = new Heart_Axis_Data_Worker(Params.AnalysisName);
                //    OutputData = new Heart_Axis_Data();
                //}

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

            switch (_state)
            {
                case (STATE.INIT):
                    Lead_I = null;
                    Lead_II = null;


                    try
                    {

                        int Lead_I_Length = (int) InputECGBaselineWorker.getNumberOfSamples(LeadISymbol);
                        Lead_I = InputECGBaselineWorker.LoadSignal(LeadISymbol, 0, Lead_I_Length).ToArray(); // todo:  czy nazwy są spójne,

                        int Lead_II_Length = (int) InputECGBaselineWorker.getNumberOfSamples(LeadIISymbol);
                        Lead_II = InputECGBaselineWorker.LoadSignal(LeadIISymbol, 0, Lead_II_Length).ToArray(); // todo:  czy nazwy są spójne,


                    }
                    catch (Exception e)
                    {
                        Abort();
                    }


                    if ((Lead_I != null) && (Lead_II != null))
                    {
                        FirstSignalName = LeadISymbol;
                        FirstSignal = Lead_I;
                    }
                    else
                    {
                        Abort();
                    }


                    // Waves_Signal { QRSOnsets, QRSEnds, POnsets, PEnds, TEnds };

                    Waves_Signal QSymbol = Waves_Signal.QRSOnsets;
                    Waves_Signal SSymbol = Waves_Signal.QRSEnds;


                    QArray = null;
                    SArray = null;

                    try
                    {
                       int Lead_I_Length = (int) InputWavesWorker.getNumberOfSamples(QSymbol, LeadISymbol);
                       QArray = InputWavesWorker.LoadSignal(QSymbol, FirstSignalName, 0, Lead_I_Length).ToArray();
                       SArray = InputWavesWorker.LoadSignal(SSymbol, FirstSignalName, 0, Lead_I_Length).ToArray();

                    }
                    catch (Exception e)
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
                        Abort();
                    }

                    Basic_Attributes FsSymbol = Basic_Attributes.Frequency;
                    Fs = (int)InputBasicDataWorker.LoadAttribute(FsSymbol);

                    //zmienne maszyny
                    _currentChannelIndex = -1;
                    _numberOfChannels = 1;
                    _alg = new Heart_Axis_Alg();
                    _state = STATE.BEGIN_CHANNEL;
                    break;

                case (STATE.BEGIN_CHANNEL):
                    _currentChannelIndex++;
                    if (_currentChannelIndex >= _numberOfChannels) _state = STATE.END; //????
                    else
                    {
                        _currentLeadName = LeadISymbol;
                        //U ciebie zamiast tego można przyjąć, że po wykonanie każdej funkcji z algs to jakby jeden channel do przodu _currentChannelLength = (int)InputWorker_basic.getNumberOfSamples(_currentLeadName); //Zmienić na worker ECG_BASELINE
                        _state = STATE.PROCESS_FIRST_STEP;
                    }
                    break;

                case (STATE.PROCESS_FIRST_STEP):
                    _state = STATE.PROCESS_CHANNEL;
                    break;

                case (STATE.PROCESS_CHANNEL):
                    double[] pseudo_tab = _alg.PseudoModule(Q, S, FirstSignal);
                    double[] fitting_parameters = _alg.LeastSquaresMethod(FirstSignal, Q, pseudo_tab, Fs);
                    int MaxOfPoly = _alg.MaxOfPolynomial(Q, fitting_parameters);
                    double[] amplitudes = _alg.ReadingAmplitudes(Lead_I, Lead_II, MaxOfPoly);
                    OutputData.HeartAxis = _alg.IandII(amplitudes);  
                    break;

                case (STATE.END_CHANNEL):
                    _state = STATE.END;
                    break;

                case (STATE.NEXT_CHANNEL):
                    _state = STATE.END;
                    break;

                case (STATE.END):
                    _ended = true;
                    break;

                default:
                    Abort();
                    break;
            }


                    //double[] pseudo_tab = PseudoModule(Q, S, FirstSignal);
                    //double[] fitting_parameters = LeastSquaresMethod(FirstSignal, Q, pseudo_tab, Fs);
                    //int MaxOfPoly = MaxOfPolynomial(Q, fitting_parameters);
                    //double[] amplitudes = ReadingAmplitudes(Lead_I, Lead_II, MaxOfPoly);
                    //OutputData.HeartAxis = IandII(amplitudes);

                    //OutputWorker.Save(OutputData);
                    //_ended = true;


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

        public Basic_New_Data InputBasicData
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

        public ECG_Baseline_New_Data_Worker InputECGBaselineWorker
        {
            get
            {
                return  _inputECGBaselineWorker;
            }

            set
            {
                _inputECGBaselineWorker = value;
            }
        }

        public Waves_New_Data_Worker InputWavesWorker
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

        public Basic_New_Data_Worker InputBasicDataWorker
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



        public Heart_Axis_New_Data_Worker OutputWorker
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

        public static void Main(String[] args)
        {
            IModule testModule = new EKG_Project.Modules.Heart_Axis.Heart_Axis();
            Heart_Axis_Params param = new Heart_Axis_Params("abc123");

            testModule.Init(param);
            while (!testModule.Ended())
            {
                testModule.ProcessData();
                Console.WriteLine(testModule.Progress());
            }
        }

        /* public static void Main()
         {


             Heart_Axis_Params param = new Heart_Axis_Params("TestAnalysis");
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



         }*/

    }

}