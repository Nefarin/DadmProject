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





        private enum VPC { NOT_DETECTED, NO_VENTRICULAR, DETECTED_BUT_IMPOSSIBLE_TO_PLOT, LETS_PLOT };
        /// <summary>
        /// Indicate whether module is able to plot or not
        /// </summary>
        VPC _vpc;

        /// <summary>
        /// x axis to plot
        /// </summary>
        int[] _xaxisTachogramGUI;

        /// <summary>
        /// List with tachograms for each VPC detected
        /// </summary>
        List<List<double>> _tachogramGUI;

        /// <summary>
        /// List with average tachogram for each VPC detected 
        /// </summary>
        double[] _tachogramMeanGUI;

        /// <summary>
        /// Point of x axis to plot mean turbulence onset
        /// </summary>
        int[] _xpointsMeanOnsetGUI;

        /// <summary>
        /// List with values of average value of Turbulence Onset for each VPC detected 
        /// </summary>
        double[] _turbulenceOnsetmeanGUI;

        /// <summary>
        /// Point of x axis to plot max turbulence slope
        /// </summary>
        int[] _xpointsMaxSlopeGUI;

        /// <summary>
        /// Listwith coordinates of largest slope (regression lines) for each VPC detected 
        /// </summary>
        double[] _turbulenceSlopeMaxGUI;

        /// <summary>
        /// List with values of Turbulence Onset for each VPC detected 
        /// </summary>
        List<double> _turbulenceOnsetPDF;

        /// <summary>
        /// List with values of Turbulence Slope for each VPC detected 
        /// </summary>
        List<double> _turbulenceSlopePDF;


        /// <summary>
        /// Default constructor of HRT_Data Class (initialize empty)
        /// </summary>
        public HRT_Data()
        {
            VPC _vpc = new VPC();
            int[] _xaxisTachogramGUI = new int[0];
            List<List<double[]>> _tachogramGUI = new List<List<double[]>>();
            double[] _tachogramMeanGUI = new double[0];
            int[] _xpointsMeanOnsetGUI = new int[0];
            double[] _turbulenceOnsetmeanGUI = new double[0];
            int[] _xpointsMaxSlopeGUI = new int[0];
            double[] _turbulenceSlopeMaxGUI = new double[0];
            List<double> _turbulenceOnsetPDF = new List<double>();
            List<double> _turbulenceSlopePDF = new List<double>();
        }
    }
    #endregion
}