using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

#region R_Peaks_Data Class doc
/// <summary>
/// Class that includes output results of class HRT
/// </summary>
namespace EKG_Project.Modules.HRT
{
    public class HRT_Data : ECG_Data
    {
        ///<Summary>_TurbulenceOnset- wartości TO</Summary>
        public Vector<double> _TurbulenceOnset { get; set; }

        ///<Summary>_TurbulenceSlope - wartości TS</Summary>
        public Vector<double> _TurbulenceSlope { get; set; }

        ///<Summary>_TurbulenceOnsetMean -średnia wartości TO, do zaznaczenia na wykresie</Summary>
        public double[] _TurbulenceOnsetMean { get; set; }

        ///<Summary>_TurbulenceSlopeMean - średnia wartość TS, do zaznaczenia na wykresie</Summary>
        public double[] _TurbulenceSlopeMean { get; set; }

        ///<Summary>_VPCtachogram - fragmenty tachogramu wokół VPC, do zaznaczenia na wykresie</Summary>
        public Vector<double> _VPCtachogram { get; set; }

        ///<Summary>_VPCcount - liczba znalezionych VPC</Summary>
        public int _VPCcount { get; set; }


        //konktruktor domyślny
        public HRT_Data() { }
    }
}


    //public class HRT_Data : ECG_Data
    //{
    //    /// <summary>
    //    /// List of tuples with tachogram (each kolumn is one tachogram) around each VPC detected for every channel in signal
    //    /// </summary>
    //    List<Tuple<string, double[,]>> _tachogram = new List<Tuple<string, double[,]>>();

//    /// <summary>
//    /// List of tuples with values of Turbulence Onset for each VPC detected for every channel in signal
//    /// </summary>
//    List<Tuple<string, double[]>> _turbulenceOnset = new List<Tuple<string, double[]>>();

//    /// <summary>
//    /// List of tuples with values of Turbulence Slope for each VPC detected for every channel in signal
//    /// </summary>
//    List<Tuple<string, double[]>> _turbulenceSlope = new List<Tuple<string, double[]>>();

//    /// <summary>
//    /// List of tuples with Mean, Max and Min results of Onset Values for each VPC detected for every channel in signal
//    /// </summary>
//    List<Tuple<string, double, double, double>> _tachogramOnsetValues;

//    /// <summary>
//    /// List of tuples with Mean, Max and Min results of Slope Values for each VPC detected for every channel in signal
//    /// </summary>
//    List<Tuple<string, double, double, double>> _tachogramSlopeValues;

//    /// <summary>
//    /// List of tuples with coordinates of slopes (regression lines) for each VPC detected for every channel in signal
//    /// </summary>
//    List<Tuple<string, double[,], double[,]>> _tachogramSlopePoints;




//    public List<Tuple<string, double[,]>> Tachogram
//    {
//        get
//        {
//            return _tachogram;
//        }

//        set
//        {
//            _tachogram = value;
//        }
//    }

//    public List<Tuple<string, double[]>> TurbulenceOnset
//    {
//        get
//        {
//            return _turbulenceOnset;
//        }

//        set
//        {
//            _turbulenceOnset = value;
//        }
//    }

//    public List<Tuple<string, double[]>> TurbulenceSlope
//    {
//        get
//        {
//            return _turbulenceSlope;
//        }

//        set
//        {
//            _turbulenceSlope = value;
//        }
//    }

//    public List<Tuple<string, double, double, double>> TachogramSlopeValues
//    {
//        get
//        {
//            return _tachogramSlopeValues;
//        }

//        set
//        {
//            _tachogramSlopeValues = value;
//        }
//    }

//    public List<Tuple<string, double, double, double>> TachogramOnsetValues
//    {
//        get
//        {
//            return _tachogramOnsetValues;
//        }

//        set
//        {
//            _tachogramOnsetValues = value;
//        }
//    }





//    /// <summary>
//    /// Default constructor of HRT_Data Class (initialize empty lists of tuples)
//    /// </summary>
//    public HRT_Data()
//    {
//        Tachogram = new List<Tuple<string, double[,]>>();
//        TurbulenceOnset = new List<Tuple<string, double[]>>();
//        TurbulenceSlope = new List<Tuple<string, double[]>>();
//        TachogramSlopeValues = new List<Tuple<string, double, double, double>>();
//        TachogramOnsetValues = new List<Tuple<string, double, double, double>>();
//    }
//}
//    #endregion
//}