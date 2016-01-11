using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.ECG_Baseline
{

    public enum Filtr_Method {MOVING_AVG, BUTTERWORTH, SAV_GOL, LMS};
    public enum Filtr_Type {LOWPASS, HIGHPASS};

    public class ECG_Baseline_Params : ModuleParams
    {
        private Filtr_Method _method;             //metoda filtracji
        private Filtr_Type _type;                 //typ filtracji
        private Vector<double> _ecgFiltered;      //przefiltrowany sygnał do algorytmu LMS
        private double _fs;                       //częstotliwość próbkowania 
        private double _fc;                       //częstotliwość odcięcia
        private int _windowSize;                 //szerokość okna filtracji
        private int _order;                      //rząd filtru
        private string _analysisName;             //analysisName

        public ECG_Baseline_Params()
        {
            this.Method = Filtr_Method.MOVING_AVG;
            this.AnalysisName = "Analysis6";
            this.Fc = 50;
            this.Type = Filtr_Type.LOWPASS;
        }

        public ECG_Baseline_Params(Filtr_Method method, Filtr_Type type, int order, double fs, double fc, string analysisName)
        {
           this.Method = method;
           this.AnalysisName = analysisName;
           this.Type = type;
           this.Fs = fs;
           this.Fc = fc;
           this.Order = order;
        }

        public ECG_Baseline_Params(Filtr_Method method, Filtr_Type type, int windowSize, string analysisName)
        {
            this.Method = method;
            this.AnalysisName = analysisName;
            this.Type = type;
            this.WindowSize = windowSize;
        }

        public ECG_Baseline_Params(Filtr_Method method, string analysisName)
        {
            this.Method = method;
            this.AnalysisName = analysisName;
        }

        //Tymczasowy konstruktor, bo ktoś coś odj**
        public ECG_Baseline_Params(Filtr_Method method, Filtr_Type type)
        {
            this.Method = method;
            this.Type = type;
        }

        public void CopyParametersFrom(ECG_Baseline_Params parameters)
        {
            this.Method = parameters.Method;
            this.Type = parameters.Type;
            this.Fc = parameters.Fc;
            this.Fs = parameters.Fs;
            this.Order = parameters.Order;
            this.WindowSize = parameters.WindowSize;
        }

        public string AnalysisName
        {
            get
            {
                return _analysisName;
            }

            set
            {
                _analysisName = value;
            }
        }

        public Filtr_Method Method
        {
            get
            {
                return _method;
            }

            set
            {
                _method = value;
            }
        }

        public Filtr_Type Type
        {
            get
            {
                return _type;
            }

            set
            {
                _type = value;
            }
        }

        public Vector<double> EcgFiltered
        {
            get
            {
                return _ecgFiltered;
            }

            set
            {
                _ecgFiltered = value;
            }
        }

        public double Fs
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

        public double Fc
        {
            get
            {
                return _fc;
            }

            set
            {
                _fc = value;
            }
        }

        public int WindowSize
        {
            get
            {
                return _windowSize;
            }

            set
            {
                _windowSize = value;
            }
        }

        public int Order
        {
            get
            {
                return _order;
            }

            set
            {
                _order = value;
            }
        }
    }
}
