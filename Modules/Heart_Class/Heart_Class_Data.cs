using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EKG_Project.Modules.Heart_Class
{
    class Heart_Class_Data : ECG_Data
    {
        private Vector<double> _signal;          // inicjalizacja przez wczytanie Vector z pliku
        private Vector<double> _qrsOnset;        // inicjalizacja przez wczytanie Vector z pliku
        private Vector<double> _qrsEnd;          // inicjalizacja przez wczytanie Vector z pliku
        private int _qrsNumber;                  // inicjalizacja przez zliczenie elementów _qrsOnset
        private Vector<double> _qrsR;            // inicjalizacja przez wczytanie Vector z pliku
        private Vector<double> _singleQrs;       // inicjalizacja w konstruktorze
        private List<Tuple<int, Vector<double>>> _QrsComplex; // inicjalizacja w kontruktorze
        private List<Tuple<int, Vector<double>>> _qrsCoefficients;

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        public Vector<double> Signal
        {
            get { return _signal; }
            set { _signal = value; }
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        public Vector<double> QrsOnset
        {
            get { return _qrsOnset; }
            set
            {
                //powinien byc typ int! ale to pozniej, bo klasa TempInut nie wczytuje int
                _qrsOnset = value;
                QrsNumber = _qrsOnset.Count();
            }
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        public Vector<double> QrsEnd
        {
            get { return _qrsEnd; }
            set { _qrsEnd = value; }
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        public Vector<double> QrsR
        {
            get { return _qrsR; }
            set { _qrsR = value; }
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        public int QrsNumber
        {
            get { return _qrsNumber; }
            set { _qrsNumber = value; }
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        public Vector<double> SingleQrs
        {
            get { return _singleQrs; }
            set { _singleQrs = value; }
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        public List<Tuple<int, Vector<double>>> QrsComplex
        {
            get { return _QrsComplex; }
            set { _QrsComplex = value; }
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        public List<Tuple<int, Vector<double>>> QrsCoefficients
        {
            get { return _qrsCoefficients; }
            set { _qrsCoefficients = value; }
        }

        #region Documentation
        /// <summary>
        /// Not parameterized constructor of Heart_Class_Data.cs data class
        /// </summary>
        #endregion
        public Heart_Class_Data()
        {
            _signal = Vector<double>.Build.Dense(1);
            _qrsOnset = Vector<double>.Build.Dense(1);        
            _qrsEnd = Vector<double>.Build.Dense(1);          
            _qrsNumber = new int();                 
            _qrsR = Vector<double>.Build.Dense(1);           
            _singleQrs = Vector<double>.Build.Dense(1);       
            _QrsComplex = new List<Tuple<int, Vector<double>>>();
            _qrsCoefficients = new List<Tuple<int, Vector<double>>>();
            _singleQrs = Vector<double>.Build.Dense(1);
        }
    }
}
