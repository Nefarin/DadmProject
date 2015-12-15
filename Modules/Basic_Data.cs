using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.IO
{
    class Basic_Data : ECG_Data
    {
        private uint _frequency;
        private uint _sampleAmount;
        private List<Tuple<string, Vector<double>>> _signals;

        public  Basic_Data() {}

        public uint Frequency
        {
            get
            {
                return _frequency;
            }
            set
            {
                _frequency = value;
            }
        }

        public uint SampleAmount
        {
            get
            {
                return _sampleAmount;
            }
            set
            {
                _sampleAmount = value;
            }
        }

        public List<Tuple<string, Vector<double>>> Signals
        {
            get
            {
                return _signals;
            }
            set
            {
                _signals = value;
            }
        }
    }
}
