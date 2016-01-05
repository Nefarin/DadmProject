using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.R_Peaks
{
    class R_Peaks_Data : ECG_Data
    {
        private List<Tuple<string, Vector<double>>> _rPeaks;
        /// Wektor indeksów wykrytych zalamków R
        private List<Tuple<string, Vector<double>>> _rRInterval;
        /// Wektor odleglosci miedzy kolejnymi zalamkami R [ms]

        public R_Peaks_Data() { }

        public List<Tuple<string, Vector<double>>> RPeaks
        {
            get
            {
                return _rPeaks;
            }
            set
            {
                _rPeaks = value;
            }
        }

        public List<Tuple<string, Vector<double>>> RRInterval
        {
            get
            {
                return _rRInterval;
            }
            set
            {
                _rRInterval = value;
            }
        }
    }
}
