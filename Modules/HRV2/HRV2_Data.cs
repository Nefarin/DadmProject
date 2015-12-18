﻿using System.Collections.Generic;

namespace EKG_Project.Modules.HRV2
{
    class HRV2_Data : IO.ECG_Data
    {
        private double _tinn;
        private double _triangleIndex;
        private double _sd1;
        private double _sd2;
        private List<double> _histogramData; //nie do konca wiemy czy to bd lista double
        private List<double> _poincarePlotData; //nie do konca wiemy czy to bd lista double

        #region Properties
        #region Documentation
        /// <summary>
        /// TODO - uzupełnić dokumentację
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
        /// TODO - uzupełnić dokumentację
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
        /// TODO - uzupełnić dokumentację
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
        /// TODO - uzupełnić dokumentację
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
        /// TODO - uzupełnić dokumentację
        /// </summary>
        /// 
        #endregion
        public List<double> HistogramData
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
        /// TODO - uzupełnić dokumentację
        /// </summary>
        /// 
        #endregion
        public List<double> PoincarePlotData
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
        /// TODO - uzupełnić dokumentację konstruktora
        /// </summary>
        /// 
        #endregion
        public HRV2_Data()
        {
            HistogramData = new List<double>(); // Dzięki inicjalizacji, nawet pustej, nie będzie NullPointerException
            PoincarePlotData = new List<double>(); // Dzięki inicjalizacji, nawet pustej, nie będzie NullPointerException
        }

        // TODO Pewnie będzie trzeba zrobić więcej konstruktorów - kilku elementowych, etc.
    }
}
