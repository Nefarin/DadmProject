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
        private uint _windowSize;                 //szerokość okna filtracji
        private uint _order;                      //rząd filtru

        public ECG_Baseline_Params(Filtr_Method method, Filtr_Type type, uint order, double fs, double fc)
        {
            _method = method;
            _type = type;
            _fs = fs;
            _fc = fc;
            _order = order;
        }

        public ECG_Baseline_Params(Filtr_Method method, Filtr_Type type, uint windowSize)
        {
            _method = method;
            _type = type;
            _windowSize = windowSize;
        }

        public ECG_Baseline_Params(Filtr_Method method, Filtr_Type type)
        {
            _method = method;
            _type = type;
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

        public uint WindowSize
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

        public uint Order
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
