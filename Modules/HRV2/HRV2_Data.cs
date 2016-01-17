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
        private List<Tuple<string, HRV2.Histogram2>> _histogramData;
        private List<Tuple<string, Vector<double>>> _poincarePlotData_x;
        private List<Tuple<string, Vector<double>>> _poincarePlotData_y;
        private List<double> _elipseCenter;
        

        #region Documentation
        /// <summary>
        /// Współczynnik TINN
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
        /// Indeks trójkątny
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
        /// Współczynnik SD1, krótsza przekątna dopasowanej elipsy do wykresu Poincare, zmienność krótko-terminowa
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
        /// Współczynnik SD2,dłuższa przekątna dopasowanej elipsy do wykresu Poincare, zmienność długo-terminowa
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
        /// Histogram długości interwałów RR
        /// </summary>
        /// 
        #endregion
        public List<Tuple<string, HRV2.Histogram2>> HistogramData
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
        /// Wykres Poincare_x
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
        /// Wykres Poincare_y
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
        /// Konstruktor z histogramem i wykresem Poincare
        /// </summary>
        /// 
        #endregion
        public HRV2_Data()
        {
            HistogramData = new List<Tuple<string, HRV2.Histogram2>>();
        }
    }
}
