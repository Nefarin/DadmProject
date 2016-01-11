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

        public ECG_Baseline_Params() // konstruktor domyślny
        {
            this.Method = Filtr_Method.MOVING_AVG;
            this.AnalysisName = "Analysis6";
            this._windowSizeLow = 5;
            this.Type = Filtr_Type.LOWPASS;
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

        public void CopyParametersFrom(ECG_Baseline_Params parameters)
        {
            this.Method = parameters.Method;
            this.Type = parameters.Type;
            this.FcLow = parameters.FcLow;
            this.FcHigh = parameters.FcHigh;
            this.OrderLow = parameters.OrderLow;
            this.OrderHigh = parameters.OrderHigh;
            this.WindowSizeLow = parameters.WindowSizeLow;
            this.WindowSizeHigh = parameters.WindowSizeHigh;
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
                _fcLow = value;
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
                _fcHigh = value;
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
                _windowSizeLow = value;
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
                _windowSizeHigh = value;
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
                _orderLow = value;
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
                _orderHigh = value;
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