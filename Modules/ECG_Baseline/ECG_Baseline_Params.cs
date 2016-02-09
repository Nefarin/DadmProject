using MathNet.Numerics.LinearAlgebra;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EKG_Project.Modules.ECG_Baseline
{

    /// <summary>
    /// Enum includes names of available methods for the ECG signal filtration
    /// </summary>
    public enum Filtr_Method { MOVING_AVG, BUTTERWORTH, SAV_GOL, LMS };
    /// <summary>
    /// Enum includes names of avaible types of filtration
    /// </summary>
    public enum Filtr_Type { LOWPASS, HIGHPASS, BANDPASS };

    /// <summary>
    /// Class containing parameters connected with ECG signal filtration and constructors for differents sets of parameters
    /// </summary>
    public class ECG_Baseline_Params : ModuleParams, INotifyPropertyChanged
    {
        /// <summary>
        /// Filtration method chosen by user
        /// </summary>
        private Filtr_Method _method;             //metoda filtracji
        /// <summary>
        /// Filtration type chosen by user
        /// </summary>
        private Filtr_Type _type;                 //typ filtracji
        /// <summary>
        /// Low cutoff frequency connected with Butterworth filter
        /// </summary>
        private double _fcLow;                    //częstotliwość odcięcia dolnoprzepustowy
        /// <summary>
        /// High cutoff frequency connected with Butterworth filter
        /// </summary>
        private double _fcHigh;                   //częstotliwość odcięcia górnoprzepustowy
        /// <summary>
        /// Adaptarion rate factor connected with LMS filter
        /// </summary>
        private double _mi;                       //Współczynnik adaptacji LMS
        /// <summary>
        /// Window size for lowpass filtration with Savitzky-Golay or Moving Average
        /// </summary>
        private int _windowSizeLow;               //szerokość okna filtracji dolnoprzepustowy
        /// <summary>
        /// Window size for highpass filtration with Savitzky-Golay or Moving Average
        /// </summary>
        private int _windowSizeHigh;              //szerokość okna filtracji górnoprzepustowy
        /// <summary>
        /// Window size for LMS filtration
        /// </summary>
        private int _windowLMS;                   //szerokość okna w algorytmie LMS
        /// <summary>
        /// Butterworth lowpass filter order
        /// </summary>
        private int _orderLow;                    //rząd filtru dolnoprzepustowy
        /// <summary>
        /// Butterworth highpass filter order
        /// </summary>
        private int _orderHigh;                   //rząd filtru górnoprzepustowy

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Default constructor for ECG_Baseline_Params - Butterworth bandpass 2-50 Hz
        /// </summary>
        public ECG_Baseline_Params() : base() // konstruktor domyślny
        {
            this.Method = Filtr_Method.BUTTERWORTH;
            this.Type = Filtr_Type.BANDPASS;
            this._fcLow = 50;
            this._fcHigh = 2;
            this.OrderLow = 30;
            this.OrderHigh = 30;
        }

        /// <summary>
        /// Constructor dedicated for Butterworth lowpass and highpass filter
        /// </summary>
        /// <param name="analysisName">Name of the current analysis</param>
        /// <param name="method">Method chosen by user (MOVING_AVG, BUTTERWORTH, SAV_GOL, LMS)</param>
        /// <param name="type">Filter type(HIGHPASS, LOWPASS, BANDPASS)</param>
        /// <param name="order">Filter order</param>
        /// <param name="fc">Cutoff frequency</param>
        public ECG_Baseline_Params(string analysisName, Filtr_Method method, Filtr_Type type, int order, double fc) //konstruktor BUTTERWORTH LOW, HIGH
        {
            this.Method = method;
            this.AnalysisName = analysisName;
            this.Type = type;
            if (type == Filtr_Type.LOWPASS)
            {
                this.FcLow = fc;
                this.OrderLow = order;
            }
            else if (type == Filtr_Type.HIGHPASS)
            {
                this.FcHigh = fc;
                this.OrderHigh = order;
            }
        }

        /// <summary>
        /// Constructor dedicated for Butterworth bandpass filter
        /// </summary>
        /// <param name="method">Method chosen by user (MOVING_AVG, BUTTERWORTH, SAV_GOL, LMS)</param>
        /// <param name="type">Filter type(HIGHPASS, LOWPASS, BANDPASS)</param>
        /// <param name="orderLow">Lowpass filter order</param>
        /// <param name="orderHigh">Highpass filter order</param>
        /// <param name="fcLow">Lowpass cutoff frequency</param>
        /// <param name="fcHigh">Highpass cutoff frequency</param>
        /// <param name="analysisName">Name of the current analysis</param>
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

        /// <summary>
        /// Constructor dedicated for Moving Average and Savitkzy-Golay highpass and lowpass filter
        /// </summary>
        /// <param name="method">Method chosen by user (MOVING_AVG, BUTTERWORTH, SAV_GOL, LMS)</param>
        /// <param name="type">Filter type(HIGHPASS, LOWPASS, BANDPASS)</param>
        /// <param name="windowSize">Size of the filtration window</param>
        /// <param name="analysisName">Name of the current analysis</param>
        public ECG_Baseline_Params(Filtr_Method method, Filtr_Type type, int windowSize, string analysisName) // konstruktor MOVING_AVG, SAV_GOL LOW, HIGH
        {
            this.Method = method;
            this.AnalysisName = analysisName;
            this.Type = type;
            if (type == Filtr_Type.LOWPASS)
                this.WindowSizeLow = windowSize;
            else if (type == Filtr_Type.HIGHPASS)
                this.WindowSizeHigh = windowSize;
        }

        /// <summary>
        /// Constructor dedicated for Moving Average and Savitkzy-Golay bandpass filter
        /// </summary>
        /// <param name="method">Method chosen by user (MOVING_AVG, BUTTERWORTH, SAV_GOL, LMS)</param>
        /// <param name="type">Filter type(HIGHPASS, LOWPASS, BANDPASS)</param>
        /// <param name="windowSizeLow">Size of the lowpass filtration window</param>
        /// <param name="windowSizeHigh">Size of the highpass filtration window</param>
        /// <param name="analysisName">Name of the current analysis</param>
        public ECG_Baseline_Params(Filtr_Method method, Filtr_Type type, int windowSizeLow, int windowSizeHigh, string analysisName) // konstruktor MOVING_AVG, SAV_GOL BAND
        {
            this.Method = method;
            this.AnalysisName = analysisName;
            this.Type = type;
            this.WindowSizeLow = windowSizeLow;
            this.WindowSizeHigh = windowSizeHigh;
        }

        /// <summary>
        /// Constructor dedicated for LMS lowpass, highpass and bandpass filter
        /// </summary>
        /// <param name="method">Method chosen by user (MOVING_AVG, BUTTERWORTH, SAV_GOL, LMS)</param>
        /// <param name="type">Filter type(HIGHPASS, LOWPASS, BANDPASS)</param>
        /// <param name="windowLMS">Size of the lowpass and highpass filtration window</param>
        /// <param name="analysisName">Name of the current analysis</param>
        /// <param name="mi">Adaptarion rate factor</param>
        public ECG_Baseline_Params(Filtr_Method method, Filtr_Type type, int windowLMS, string analysisName, double mi) // konstruktor LMS LOW, HIGH, BAND
        {
            this.Method = method;
            this.AnalysisName = analysisName;
            this.Type = type;
            this.Mi = mi;
            this.WindowLMS = windowLMS;
        }

        //Geters and seters for all the parameters in ECG_Baseline_Params

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
                this.NotifyPropertyChanged("IsLMS");
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
                this.NotifyPropertyChanged("IsLMS");
            }
        }

        public bool IsLMS
        {
            get
            {
                return (this.Type == Filtr_Type.LOWPASS || this.Type == Filtr_Type.BANDPASS || this.Type == Filtr_Type.HIGHPASS) &&
                    this.Method == Filtr_Method.LMS;
            }
        }


        public bool IsOtherLowPass
        {
            get
            {
                return (this.Type == Filtr_Type.LOWPASS || this.Type == Filtr_Type.BANDPASS) &&
                    this.Method != Filtr_Method.BUTTERWORTH && this.Method != Filtr_Method.LMS;
            }
        }

        public bool IsOtherHighPass
        {
            get
            {
                return (this.Type == Filtr_Type.HIGHPASS || this.Type == Filtr_Type.BANDPASS) &&
                    this.Method != Filtr_Method.BUTTERWORTH && this.Method != Filtr_Method.LMS;
            }
        }

        public double FcLow
        {
            get
            {
                return _fcLow;
            }

            set
            {
                if (value > 0)
                    _fcLow = value;
                else
                    _fcLow = 0.001;
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
                if (value > 0)
                    _fcHigh = value;
                else
                    _fcHigh = 0.001;
            }
        }

        public double Mi
        {
            get
            {
                return _mi;
            }
            set
            {
                if (value >= 0 && value <= 1)
                    _mi = value;
                else
                    _mi = 0.07;
            }
        }

        public int WindowLMS
        {
            get
            {
                return _windowLMS;
            }
            set
            {
                if (value >= 2)
                    _windowLMS = value;
                else
                    _windowLMS = 2;
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
                if (value >= 2)
                    _windowSizeLow = value;
                else
                    _windowSizeLow = 2;
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
                if (value >= 2)
                    _windowSizeHigh = value;
                else
                    _windowSizeHigh = 2;
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
                if (value >= 1)
                    _orderLow = value;
                else
                    _orderLow = 1;
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
                if (value >= 1)
                    _orderHigh = value;
                else
                    _orderHigh = 1;
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