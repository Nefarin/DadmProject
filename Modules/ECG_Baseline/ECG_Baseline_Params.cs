using MathNet.Numerics.LinearAlgebra;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EKG_Project.Modules.ECG_Baseline
{

    public enum Filtr_Method { MOVING_AVG, BUTTERWORTH, SAV_GOL, LMS };
    public enum Filtr_Type { LOWPASS, HIGHPASS, BANDPASS };

    public class ECG_Baseline_Params : ModuleParams, INotifyPropertyChanged
    {
        private Filtr_Method _method;             //metoda filtracji
        private Filtr_Type _type;                 //typ filtracji
        private double _fcLow;                    //częstotliwość odcięcia dolnoprzepustowy
        private double _fcHigh;                   //częstotliwość odcięcia górnoprzepustowy
        private int _windowSizeLow;               //szerokość okna filtracji dolnoprzepustowy
        private int _windowSizeHigh;              //szerokość okna filtracji górnoprzepustowy
        private int _orderLow;                    //rząd filtru dolnoprzepustowy
        private int _orderHigh;                   //rząd filtru górnoprzepustowy

        public event PropertyChangedEventHandler PropertyChanged;

        public ECG_Baseline_Params() : base() // konstruktor domyślny
        {
            this.Method = Filtr_Method.BUTTERWORTH;
            this.Type = Filtr_Type.BANDPASS;
            this._fcLow = 50;
            this._fcHigh = 0.5;
            this.OrderLow = 3;
            this.OrderHigh = 3;
        }

        public ECG_Baseline_Params(Filtr_Method method, Filtr_Type type, int order, double fc, string analysisName) //konstruktor BUTTERWORTH LOW, HIGH
        {
            this.Method = method;
            this.AnalysisName = analysisName;
            this.Type = type;
            if (type == Filtr_Type.LOWPASS)
                this.FcLow = fc;
            else if (type == Filtr_Type.HIGHPASS)
                this.FcHigh = fc;
            if (type == Filtr_Type.LOWPASS)
                this.OrderLow = order;
            else if (type == Filtr_Type.HIGHPASS)
                this.OrderHigh = order;
        }

        public ECG_Baseline_Params(Filtr_Method method, Filtr_Type type, int orderLow, int orderHigh, double fcLow, double fcHigh, string analysisName) //konstruktor BUTTERWORTH BAND
        {
            this.Method = method;
            this.AnalysisName = analysisName;
            this.Type = type;
            this.FcLow = fcLow;
            this.FcHigh = fcHigh;
            this.OrderLow = orderLow;
            this.OrderHigh = orderHigh;
        }

        public ECG_Baseline_Params(Filtr_Method method, Filtr_Type type, int windowSize, string analysisName) // konstruktor MOVING_AVG, SAV_GOL, LMS LOW, HIGH
        {
            this.Method = method;
            this.AnalysisName = analysisName;
            this.Type = type;
            if (type == Filtr_Type.LOWPASS)
                this.WindowSizeLow = windowSize;
            else if (type == Filtr_Type.HIGHPASS)
                this.WindowSizeHigh = windowSize;
        }

        public ECG_Baseline_Params(Filtr_Method method, Filtr_Type type, int windowSizeLow, int windowSizeHigh, string analysisName) // konstruktor MOVING_AVG, SAV_GOL, LMS BAND
        {
            this.Method = method;
            this.AnalysisName = analysisName;
            this.Type = type;
            this.WindowSizeLow = windowSizeLow;
            this.WindowSizeHigh = windowSizeHigh;
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
                this.NotifyPropertyChanged("IsButterworthLowPass");
                this.NotifyPropertyChanged("IsButterworthHighPass");
                this.NotifyPropertyChanged("IsOtherLowPass");
                this.NotifyPropertyChanged("IsOtherHighPass");
            }
        }

        public bool IsButterworthLowPass
        {
            get
            {
                return this.Method == Filtr_Method.BUTTERWORTH &&
                    (this.Type == Filtr_Type.LOWPASS || this.Type == Filtr_Type.BANDPASS);
            }
        }

        public bool IsButterworthHighPass
        {
            get
            {
                return this.Method == Filtr_Method.BUTTERWORTH &&
                    (this.Type == Filtr_Type.HIGHPASS || this.Type == Filtr_Type.BANDPASS);
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
                this.NotifyPropertyChanged("IsButterworthLowPass");
                this.NotifyPropertyChanged("IsButterworthHighPass");
                this.NotifyPropertyChanged("IsOtherLowPass");
                this.NotifyPropertyChanged("IsOtherHighPass");
            }
        }

        public bool IsOtherLowPass { get { return this.Type == Filtr_Type.LOWPASS || this.Type == Filtr_Type.BANDPASS; } }
        public bool IsOtherHighPass { get { return this.Type == Filtr_Type.HIGHPASS || this.Type == Filtr_Type.BANDPASS; } }

        public double FcLow
        {
            get
            {
                return _fcLow;
            }

            set
            {
                if (_fcLow >= 0)
                    _fcLow = value;
                else
                    _fcLow = 0;
            }
        }

        public double FcHigh
        {
            get
            {
                return _fcHigh;
            }

            set
            {
                if (_fcHigh >= 0)
                    _fcHigh = value;
                else
                    _fcHigh = 0;
            }
        }

        public int WindowSizeLow
        {
            get
            {
                return _windowSizeLow;
            }

            set
            {
                if (_windowSizeLow >= 0)
                    _windowSizeLow = value;
                else
                    _windowSizeLow = 0;
            }
        }

        public int WindowSizeHigh
        {
            get
            {
                return _windowSizeHigh;
            }

            set
            {
                if (_windowSizeHigh >= 0)
                    _windowSizeHigh = value;
                else
                    _windowSizeHigh = 0;
            }
        }

        public int OrderLow
        {
            get
            {
                return _orderLow;
            }

            set
            {
                if (_orderLow >= 0)
                    _orderLow = value;
                else
                    _orderLow = 0;
            }
        }

        public int OrderHigh
        {
            get
            {
                return _orderHigh;
            }

            set
            {
                if (_orderHigh >= 0)
                    _orderHigh = value;
                else
                    _orderHigh = 0;
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}