using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.HRV1
{
    public class HRV1_Data : ECG_Data
    {

        private List<Tuple<string, Vector<double>>> _timeBasedParams;
        private List<Tuple<string, Vector<double>>> _freqBasedParams;
        private List<Tuple<string, Vector<double>>> _freqVector;
        private List<Tuple<string, Vector<double>>> _PSD;

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

        public List<Tuple<string, Vector<double>>> FreqVector
        {
            get
            {
                return _freqVector;
            }
            set
            {
                _freqVector = value;
            }
        }

        public List<Tuple<string, Vector<double>>> PSD
        {
            get
            {
                return _PSD;
            }
            set
            {
                _PSD = value;
            }
        }

        // depreciated - to be removed
        private List<Tuple<string, Vector<double>>> _rInstants;
        // depreciated - to be removed
        private List<Tuple<string, Vector<double>>> _rrIntervals;

        // obsolete - to be removed
        public List<Tuple<string, Vector<double>>> RInstants
        {
            get
            {
                return _rInstants;
            }
            set
            {
                _rInstants = value;
            }
        }

        // obsolete - to be rempoved
        public List<Tuple<string, Vector<double>>> RRIntervals
        {
            get
            {
                return _rrIntervals;
            }
            set
            {
                _rrIntervals = value;
            }
        }
    }
}
