using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.HRV_DFA
{
    public class HRV_DFA_Data : ECG_Data
    {
        // outputs
        private List<Tuple<string, Vector<double>, Vector<double>>> _dfaNumberN;
        private List<Tuple<string, Vector<double>, Vector<double>>> _dfaValueFn;
        private List<Tuple<string, Vector<double>,Vector<double>>> _paramAlpha;
        private List<Tuple<string, Vector<double>, Vector<double>>> _fluctuations;

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

        public HRV_DFA_Data()
        {
           
        }
        public HRV_DFA_Data(List<Tuple<string, Vector<double>, Vector<double>>> dfaNumberN, List<Tuple<string, Vector<double>, Vector<double>>> dfaValueFn, List<Tuple<string, Vector<double>, Vector<double>>> paramAlpha, List<Tuple<string, Vector<double>, Vector<double>>> fluctuations) : this()
        {
            DfaNumberN = dfaNumberN;
            DfaValueFn = dfaValueFn;
            ParamAlpha = paramAlpha;
            Fluctuations = fluctuations;
        }
    }
}
