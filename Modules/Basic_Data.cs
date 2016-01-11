using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules
{
    public class Basic_Data : ECG_Data
    {
        private uint _frequency;
        private uint _sampleAmount;
        private List<Tuple<string, Vector<double>>> _signals;

        public Basic_Data() {}

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

        public object RPeaks { get; internal set; }

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

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(String.Format("Frequency: {0} \n", this.Frequency));
            builder.Append(String.Format("Sample amount : {0} \n", this.SampleAmount));
            foreach (var signal in this.Signals)
            {
                builder.Append(String.Format("Signal {0}: {1} \n", signal.Item1, signal.Item2.ToString()));
            }
            
            return builder.ToString();
        }
    }
}
