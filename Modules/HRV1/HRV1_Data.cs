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

        private List<Tuple<string, Vector<double>>> _rSamples;   

        public List<Tuple<string, Vector<double>>> RSamples
        {
            get
            {
                return _rSamples;
            }
            set
            {
                _rSamples = value;
            }
        }

    }
}
