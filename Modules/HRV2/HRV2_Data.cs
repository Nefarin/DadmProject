using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using System;
using MathNet.Numerics.Statistics;

namespace EKG_Project.Modules.HRV2
{
    public class HRV2_Data : ECG_Data
    {
        private List<double> _tinn;
        private List<double> _triangleIndex;
        private List<double> _sd1;
        private List<double> _sd2;
        private List<Tuple<string, Histogram>> _histogramData;
        private List<Tuple<string, Vector<double>>> _poincarePlotData_x;
        private List<Tuple<string, Vector<double>>> _poincarePlotData_y;
        private List<double> _elipseCenter;


        #region Documentation
        /// <summary>
        /// TINN coefficient, the base of the triangle fitted to histogram
        /// </summary>
        /// 
        #endregion
        public List<double> Tinn
        {
            get
            {
                return _tinn;
            }

            set
            {
                _tinn = value;
            }
        }

        #region Documentation
        /// <summary>
        /// Triangle index coefficient, value of the highest bin, devided by all RR intervals count
        /// </summary>
        /// 
        #endregion
        public List<double> TriangleIndex
        {
            get
            {
                return _triangleIndex;
            }

            set
            {
                _triangleIndex = value;
            }
        }

        #region Documentation
        /// <summary>
        /// SD1 coefficient, the shorter axis of the ellipse
        /// </summary>
        /// 
        #endregion
        public List<double> SD1
        {
            get
            {
                return _sd1;
            }

            set
            {
                _sd1 = value;
            }
        }

        #region Documentation
        /// <summary>
        /// SD2 coefficient, the longer axis of the ellipse
        /// </summary>
        /// 
        #endregion
        public List<double> SD2
        {
            get
            {
                return _sd2;
            }

            set
            {
                _sd2 = value;
            }
        }

        #region Documentation
        /// <summary>
        /// The RR intervals histogram
        /// </summary>
        /// 
        #endregion
        public List<Tuple<string, Histogram>> HistogramData
        {
            get
            {
                return _histogramData;
            }

            set
            {
                _histogramData = value;
            }
        }

        #region Documentation
        /// <summary>
        /// x coefficien of Poincare plot
        /// </summary>
        /// 
        #endregion
        public List<Tuple<string, Vector<double>>> PoincarePlotData_x
        {
            get
            {
                return _poincarePlotData_x;
            }

            set
            {
                _poincarePlotData_x = value;
            }
        }

        #region Documentation
        /// <summary>
        /// y coefficents of Poincare plot 
        /// </summary>
        /// 
        #endregion
        public List<Tuple<string, Vector<double>>> PoincarePlotData_y
        {
            get
            {
                return _poincarePlotData_y;
            }

            set
            {
                _poincarePlotData_y = value;
            }
        }

        #region Documentation
        /// <summary>
        /// The center of an elipse fitted to Poincare plot 
        /// </summary>
        /// 
        #endregion
        public List<double> ElipseCenter
        {
            get
            {
                return _elipseCenter;
            }

            set
            {
                _elipseCenter = value;
            }
        }

        #region Documentation
        /// <summary>
        /// Empty output constructors of HRV2 module
        /// </summary>
        /// 
        #endregion
        public HRV2_Data()
        {
            HistogramData = new List<Tuple<string, Histogram>>();
            PoincarePlotData_x = new List<Tuple<string, Vector<double>>>();
            PoincarePlotData_y = new List<Tuple<string, Vector<double>>>();
            SD1 = new List<double>();
            SD2 = new List<double>();
            Tinn = new List<double>();
            TriangleIndex = new List<double>();
            ElipseCenter = new List<double>();
        }
    }
}