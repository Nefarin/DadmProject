using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.HRV_DFA
{
    #region HRV_DFA class documentation
    /// <summary>
    /// Class of output results of HRV_DFA module
    /// </summary>
    #endregion
    public class HRV_DFA_Data : ECG_Data
    {
        #region
        /// <summary>
        /// List of tuples with string with lead name, and two vectors for both short and long correlations of lon(n) values
        /// </summary>
        #endregion
        private List<Tuple<string, Vector<double>, Vector<double>>> _dfaNumberN;
        #region
        /// <summary>
        /// List of tuples with string with lead name, and two vectors for both short and long correlations of lof(F(n)) values
        /// </summary>
        #endregion
        private List<Tuple<string, Vector<double>, Vector<double>>> _dfaValueFn;
        #region
        /// <summary>
        /// List of tuples with string with lead name, and two vectors for both short and long correlations of alpha coefficients
        /// </summary>
        #endregion
        private List<Tuple<string, Vector<double>,Vector<double>>> _paramAlpha;
        #region
        /// <summary>
        /// List of tuples with string with lead name, and two vectors of lon(n) and log(F(n)) values of whole range
        /// </summary>
        #endregion
        private List<Tuple<string, Vector<double>, Vector<double>>> _fluctuations;

        #region
        /// <summary>
        /// Default constructor that initialize Lists of tuples
        /// </summary>
        #endregion
        public HRV_DFA_Data()
        {
            DfaNumberN = new List<Tuple<string, Vector<double>, Vector<double>>>();
            DfaValueFn = new List<Tuple<string, Vector<double>, Vector<double>>>();
            ParamAlpha = new List<Tuple<string, Vector<double>, Vector<double>>>();
            Fluctuations = new List<Tuple<string, Vector<double>, Vector<double>>>();
        }
        public List<Tuple<string, Vector<double>, Vector<double>>> DfaNumberN
        {
            get
            {
                return _dfaNumberN;
            }
            set
            {
                _dfaNumberN = value;
            }
        }

        public List<Tuple<string, Vector<double>, Vector<double>>> DfaValueFn
        {
            get
            {
                return _dfaValueFn;
            }
            set
            {
                _dfaValueFn = value;
            }
        }
        public List<Tuple<string, Vector<double>, Vector<double>>> ParamAlpha
        {
            get
            {
                return _paramAlpha;
            }
            set
            {
                _paramAlpha = value;
            }
        }

        public List<Tuple<string, Vector<double>, Vector<double>>> Fluctuations
        {
            get
            {
                return _fluctuations;
            }

            set
            {
                _fluctuations = value;
            }
        }
 
    }
}
