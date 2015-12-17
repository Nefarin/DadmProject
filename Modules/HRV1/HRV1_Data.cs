using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.HRV1
{
    class HRV1_Data : ECG_Data
    {

        private List<Tuple<string, Vector<double>>> _timeBasedParams;
        private List<Tuple<string, Vector<double>>> _freqBasedParams;


        public List<Tuple<string, Vector<double>>> TimeBasedParams
        {
            get
            {
                return _timeBasedParams;
            }
            set
            {
                _timeBasedParams = value;
            }
        }

        public List<Tuple<string, Vector<double>>> FreqBasedParams
        {
            get
            {
                return _freqBasedParams;
            }
            set
            {
                _freqBasedParams = value;
            }
        }
    }
}
