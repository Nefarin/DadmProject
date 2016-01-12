using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.HRV_DFA
{
    class HRV_DFA_Data : IO.ECG_Data
    {
        // outputs
        private List<Tuple<string, Vector<double>>> _dfaNumberN;
        private List<Tuple<string, Vector<double>>> _dfaValueFn;
        private List<Tuple<string, Vector<double>>> _paramAlpha;

        public HRV_DFA_Data() { }

        public List<Tuple<string, Vector<double>>> DfaNumberN
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

        public List<Tuple<string, Vector<double>>> DfaValueFn
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
        public List<Tuple<string, Vector<double>>> ParamAlpha
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
