using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using System;

namespace EKG_Project.Modules.HRV2
{
    class HRV2_Data : IO.ECG_Data
    {
        private double _tinn;
        private double _triangleIndex;
        private double _sd1;
        private double _sd2;
        private List<Tuple<string, Vector<double>>> _histogramData;
        private List<Tuple<string, Vector<double>>> _poincarePlotData;

        #region Properties
        #region Documentation
        /// <summary>
        /// Współczynnik TINN
        /// </summary>
        /// 
        #endregion
        public double Tinn
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
        public double TriangleIndex
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
        public double SD1
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
        public double SD2
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
        public List<Tuple<string, Vector<double>>> HistogramData
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
        /// Wykres Poincare
        /// </summary>
        /// 
        #endregion
        public List<Tuple<string, Vector<double>>> PoincarePlotData
        {
            get
            {
                return _poincarePlotData;
            }

            set
            {
                _poincarePlotData = value;
            }
        }
        #endregion

        #region Documentation
        /// <summary>
        /// Konstruktor z histogramem i wykresem Poincare
        /// </summary>
        /// 
        #endregion
        public HRV2_Data()
        {
            HistogramData = new List<Tuple<string, Vector<double>>>();
            PoincarePlotData = new List<Tuple<string, Vector<double>>>(); 
        }
    }
}
