using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules
{
    public class Basic_New_Data
    {
        private uint _frequency;
        private uint _numberOfSamples;
        private Vector<double> _signals;
        private List<string> _leads;

        public Basic_New_Data() {}

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

        public uint NumberOfSamples
        {
            get
            {
                return _numberOfSamples;
            }
            set
            {
                _numberOfSamples = value;
            }
        }

        public Vector<double> Signals
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

        public List<string> Leads
        {
            get
            {
                return _leads;
            }
            set
            {
                _leads = value;
            }
        }
    }
}
