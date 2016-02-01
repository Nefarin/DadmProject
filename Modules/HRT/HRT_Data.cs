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
        /////<Summary>_TurbulenceOnset- wartości TO</Summary>
        //public Vector<double> _TurbulenceOnset { get; set; }

        /////<Summary>_TurbulenceSlope - wartości TS</Summary>
        //public Vector<double> _TurbulenceSlope { get; set; }

        /////<Summary>_TurbulenceOnsetMean -średnia wartości TO, do zaznaczenia na wykresie</Summary>
        //public double[] _TurbulenceOnsetMean { get; set; }

        /////<Summary>_TurbulenceSlopeMean - średnia wartość TS, do zaznaczenia na wykresie</Summary>
        //public double[] _TurbulenceSlopeMean { get; set; }

        /////<Summary>_VPCtachogram - fragmenty tachogramu wokół VPC, do zaznaczenia na wykresie</Summary>
        //public Vector<double> _VPCtachogram { get; set; }

        /////<Summary>_VPCcount - liczba znalezionych VPC</Summary>
        //public int _VPCcount { get; set; }


        ////konktruktor domyślny
        //public HRT_Data() { }

        private enum VPC { NOT_DETECTED, NO_VENTRICULAR, DETECTED_BUT_IMPOSSIBLE_TO_PLOT, DETECTED };

        /// <summary>
        /// Indicate whether module is able to plot or not
        /// </summary>
        List<Tuple<string, VPC>> _VPC_plotable =  new List<Tuple<string, VPC>>();

        /// <summary>
        /// List of tuples with tachograms for each VPC detected for every channel in signal
        /// </summary>
        List<Tuple<string, int[], List<double[]>>> _tachogramGUI = new List<Tuple<string, int[], List<double[]>>>();

        /// <summary>
        /// List of tuples with average tachogram for each VPC detected for every channel in signal
        /// </summary>
        List<Tuple<string, int[], double[]>> _tachogramMeanGUI = new List<Tuple<string, int[], double[]>>();

        /// 
        /// <summary>
        /// List of tuples with values of average value of Turbulence Onset for each VPC detected for every channel in signal
        /// </summary>
        List<Tuple<string, int[], double[]>> _turbulenceOnsetmeanGUI = new List<Tuple<string, int[], double[]>>();


        /// <summary>
        /// List of tuples with coordinates of largest slope (regression lines) for each VPC detected for every channel in signal
        /// </summary>
        List<Tuple<string, int[], double[]>> _turbulenceSlopeMaxGUI = new List<Tuple<string, int[], double[]>>();

        /// <summary>
        /// List of tuples with values of Turbulence Onset for each VPC detected for every channel in signal
        /// </summary>
        List<Tuple<string, List<double>>> _turbulenceOnsetPDF = new List<Tuple<string, List<double>>>();

        /// <summary>
        /// List of tuples with values of Turbulence Slope for each VPC detected for every channel in signal
        /// </summary>
        List<Tuple<string, List<double>>> _turbulenceSlopePDF = new List<Tuple<string, List<double>>>();



        /// <summary>
        /// Default constructor of HRT_Data Class (initialize empty lists of tuples)
        /// </summary>
        public HRT_Data()
        {
            _VPC_plotable = new List<Tuple<string, VPC>>();
            _tachogramGUI = new List<Tuple<string, int[], List<double[]>>>();
            _tachogramMeanGUI = new List<Tuple<string, int[], double[]>>();
            _turbulenceOnsetmeanGUI = new List<Tuple<string, int[], double[]>>();
            _turbulenceSlopeMaxGUI = new List<Tuple<string, int[], double[]>>();
            _turbulenceOnsetPDF = new List<Tuple<string, List<double>>>();
            _turbulenceSlopePDF = new List<Tuple<string, List<double>>>();
        }
    }
    #endregion
}