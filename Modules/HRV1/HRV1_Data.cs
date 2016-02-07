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
        private List<Tuple<string, double>> _timeBasedParams;
        private List<Tuple<string, double>> _freqBasedParams;
        private List<Tuple<string, Vector<double>>> _powerSpectrum;

        public List<Tuple<string, double>> TimeBasedParams
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

        public List<Tuple<string, double>> FreqBasedParams
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

        public List<Tuple<string, Vector<double>>> PowerSpectrum
         {
            get
            {
                return _powerSpectrum;
            }
            set
            {
                _powerSpectrum = value;
            }
        }
    }
}
