using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.HRT
{
    ///<Summary>Klasa przechowująca parametry wejścia i wyjścia do modułu HRT</Summary>
    public class HRT_Params : ModuleParams
    {
        //wejścia

        ///<Summary>_EcgFiltered -   przefiltrowany sygnał; może nie będzie potrzebny, </Summary>
        public Vector<double> _EcgFiltered { get; set; }

        ///<Summary>_fs  - częstotliwość próbkowania </Summary>
        public double _fs { get; set; }

        ///<Summary>_RRTimes nr próbek wystąpienia załamków R(chyba którys moduł to produkuje)</Summary>
        public Vector<double> _RRTimes { get; set; }

        ///<Summary>_RRTimesVPC - nr próbek wystąpienia załamków Ventricular Premature Complex (moduł Heart_Class?)</Summary>
        public Vector<double> _RRTimesVPC { get; set; }

        ///<Summary>_Tachogram -Tachogram (od modułu HRV1 lub HRV2)</Summary>
        public Vector<double> _Tachogram { get; set; }

        //wyjścia
        ///<Summary>_TurbulenceOnset- wartości TO</Summary>
        public Vector<double> _TurbulenceOnset { get; set; }

        ///<Summary>_TurbulenceSlope - wartości TS</Summary>
        public Vector<double> _TurbulenceSlope { get; set; }

        ///<Summary>_TurbulenceOnsetMean -średnia wartości TO, do zaznaczenia na wykresie</Summary>
        public int _TurbulenceOnsetMean { get; set; }

        ///<Summary>_TurbulenceSlopeMean - średnia wartości TS</Summary>
        public int _TurbulenceSlopeMean { get; set; }

        ///<Summary>_xlabel-podpis osi x</Summary>
        public string _xlabel { get; set; }   

        ///<Summary>_ylabel - podpis osi y</Summary>
        public string _ylabel { get; set; }

        ///<Summary>_VPCcount - liczba znalezionych VPC</Summary>
        public int _VPCcount { get; set; }

        //stałe
        private const int _back=5;
        private const int _foward = 15;

        ///<Summary>konstruktor używany, gdy mamy czasy występowania VPC od modułu Heart_Class</Summary>
        public HRT_Params(Vector<double>Tachogram, Vector<double> RRTimes, Vector<double> RRTimesVPC)
        {
            _Tachogram = Tachogram;
            _RRTimes = RRTimes;
            _RRTimesVPC = RRTimesVPC;
        }

        ///<Summary>konstruktor używany, gdy musimy sami zainplementować detekcję VPC</Summary>
        public HRT_Params(Vector<double> Tachogram, Vector<double> RRTimes, Vector<double> EcgFiltered, double fs)
        {
            _Tachogram = Tachogram;
            _RRTimes = RRTimes;
            _EcgFiltered = EcgFiltered;
            _fs = fs;
        }
    }
}
