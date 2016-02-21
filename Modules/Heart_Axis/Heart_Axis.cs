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

        Heart_Axis_Alg _alg;


        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _samplesProcessed;
        private int _numberOfChannels;
        private string _currentLeadName;
        private string[] _leads;
        private int _currentIndex;

        private int _step = 0;
        private int _numberOfSteps = 5;

        /* Zmienne do komunikacji z GUI - czy liczenie osi serca się skończyło/zostało przerwane */

        private bool _ended;
        private bool _aborted;

        /* Stałe */

        private const string LeadISymbol = "I";
        private const string LeadIISymbol = "II";

        /* Dane wejściowe */



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
        private string _firstSignalName; // nazwa głównego odprowadzenia

        /* Częstotliwość próbkowania sygnału */
        private int _fs;

        /* Wyznaczone załamki QRS */

        private int[] _QArray;
        private int[] _SArray;
        private int _Q;
        private int _S;
        private int _steps;

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

        /// <summary>
        /// Function that initialize all parameters and states
        /// </summary>
        /// <param name="parameters"></param>

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

                // inicjalizacja

                Lead_I = null;
                Lead_II = null;


                try
                {

                    int Lead_I_Length = (int)InputECGBaselineWorker.getNumberOfSamples(LeadISymbol);
                    Lead_I = InputECGBaselineWorker.LoadSignal(LeadISymbol, 0, Lead_I_Length).ToArray();

                    int Lead_II_Length = (int)InputECGBaselineWorker.getNumberOfSamples(LeadIISymbol);
                    Lead_II = InputECGBaselineWorker.LoadSignal(LeadIISymbol, 0, Lead_II_Length).ToArray();


                }
                catch (Exception e)
                {
                    handleInitError();
                    return;
                }


                if ((Lead_I != null) && (Lead_II != null))
                {
                    FirstSignalName = LeadISymbol;
                    FirstSignal = Lead_I;
                }
                else
                {
                    handleInitError();
                    return;
                }


                // Waves_Signal { QRSOnsets, QRSEnds, POnsets, PEnds, TEnds };

                Waves_Signal QSymbol = Waves_Signal.QRSOnsets;
                Waves_Signal SSymbol = Waves_Signal.QRSEnds;


                QArray = null;
                SArray = null;

                try
                {
                    int Lead_I_Length = (int)InputWavesWorker.getNumberOfSamples(QSymbol, LeadISymbol);
                    QArray = InputWavesWorker.LoadSignal(QSymbol, FirstSignalName, 0, Lead_I_Length).ToArray();
                    SArray = InputWavesWorker.LoadSignal(SSymbol, FirstSignalName, 0, Lead_I_Length).ToArray();

                }
                catch (Exception e)
                {
                    handleInitError();
                    return;
                }


                if ((QArray == null) || (SArray == null))
                {
                    handleInitError();
                    return;
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
                    handleInitError();
                    return;
                }

                if ((Q == -1) || (S == -1) || (Q >= S))
                {
                    handleInitError();
                    return;
                }

                Basic_Attributes FsSymbol = Basic_Attributes.Frequency;
                Fs = (int)InputBasicDataWorker.LoadAttribute(FsSymbol);


                _state = STATE.INIT;
            }


        }

        public void ProcessData()
        {
            if (Runnable()) processData();
            else _ended = true;
        }

        /// <summary>
        /// Function that calculates progress of analysis of current module
        /// </summary>
        /// <returns>Percentage of current progress </returns>

        public double Progress()
        {
            return 100 * (_step / _numberOfSteps);
        }

        public bool Runnable()
        {

            return Params != null;
        }

        /* Obliczenie osi serca */

        /// <summary>
        /// Function in which data are processed (made as machine of states)
        /// </summary>

        private void processData()
        {

            switch (_state)
            {
                case (STATE.INIT):


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
                        _state = STATE.PROCESS_FIRST_STEP;
                    }
                    break;

                case (STATE.PROCESS_FIRST_STEP):
                    _state = STATE.PROCESS_CHANNEL;
                    break;

                case (STATE.PROCESS_CHANNEL):
                    double[] pseudo_tab = _alg.PseudoModule(Q, S, FirstSignal);
                    _steps++;

                    double[] fitting_parameters = _alg.LeastSquaresMethod(FirstSignal, Q, pseudo_tab, Fs);
                    _steps++;

                    int MaxOfPoly = _alg.MaxOfPolynomial(Q, fitting_parameters);
                    _steps++;

                    double[] amplitudes = _alg.ReadingAmplitudes(Lead_I, Lead_II, MaxOfPoly);
                    _steps++;

                    double angle = _alg.IandII(amplitudes);
                    OutputWorker.SaveAttribute(angle);
                    OutputData.HeartAxis = angle;
                    _steps++;



                    //todo: jak on ma stąd wyjść?
                    _state = STATE.END_CHANNEL;
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
                return _inputECGBaselineWorker;
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
            Heart_Axis_Params param = new Heart_Axis_Params("TestAnalysis2");

            testModule.Init(param);
            while (!testModule.Ended())
            {
                testModule.ProcessData();
                Console.WriteLine(testModule.Progress());
            }
        }

        public void handleInitError()
        {
            OutputWorker.SaveAttribute(0.0);
            _ended = true; // todo: Abort()?
            _aborted = true;
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