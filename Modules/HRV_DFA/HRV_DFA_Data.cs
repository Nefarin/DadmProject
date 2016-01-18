using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.HRV_DFA
{
    #region HRV_DFA_Data Class doc
    /// <summary>
    /// Class that includes output results of class HRV_DFA
    /// </summary>
    #endregion
    public class HRV_DFA_Data : ECG_Data
    {
        /// <summary>
        /// List of tuples with vectors of values for horizontal axis log(n)
        /// </summary>
        private List<Tuple<string, Vector<double>, Vector<double>>> _dfaNumberN;
        /// <summary>
        /// List of tuples with vectors of values for vertical axis log(F(n))
        /// </summary>
        private List<Tuple<string, Vector<double>, Vector<double>>> _dfaValueFn;
        /// <summary>
        /// List of tuples with vectors of values for linear-least square approximation
        /// </summary>
        private List<Tuple<string, Vector<double>,Vector<double>>> _paramAlpha;
        /// <summary>
        /// Deafult constructor of HRV_DFA_Data Class
        /// </summary>
        public HRV_DFA_Data() {
            _dfaNumberN = new List<Tuple<string, Vector<double>, Vector<double>>>();
            _dfaValueFn = new List<Tuple<string, Vector<double>, Vector<double>>>();
            _paramAlpha = new List<Tuple<string, Vector<double>, Vector<double>>>();
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

    }
}
