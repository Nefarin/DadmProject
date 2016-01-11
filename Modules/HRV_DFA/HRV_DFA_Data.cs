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
        private List<Tuple<string, Vector<double>, Vector<double>>> _paramAlpha;

        public HRV_DFA_Data()
        {
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


