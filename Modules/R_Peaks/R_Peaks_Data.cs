using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EKG_Project.Modules.R_Peaks
{
    #region R_Peaks_Data Class doc
    /// <summary>
    /// Class that includes output results of class R_peaks
    /// </summary>
    #endregion
    public class R_Peaks_Data : ECG_Data
    {
        /// <summary>
        /// List of tuples with vectors of detected indexes of R_peaks  for every channel in signal
        /// </summary>
        private List<Tuple<string, Vector<double>>> _rPeaks;
        /// <summary>
        /// List of tuples with vector of intervals between next R_peaks in ms for every channel  in signal
        /// </summary>
        private List<Tuple<string, Vector<double>>> _rRInterval;

        /// <summary>
        /// Default construkctor of R_Peaks_Data Class (initialize empty lists of tuples)
        /// </summary>
        public R_Peaks_Data()
        {
            RPeaks = new List<Tuple<string, Vector<double>>>();
            RRInterval = new List<Tuple<string, Vector<double>>>();
        }


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