using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace EKG_Project.Modules.T_Wave_Alt
{
    public class T_Wave_Alt_Data : ECG_Data
    {
        private int[] _alternansIndexArray;

        public T_Wave_Alt_Data()
        {
            _alternansIndexArray = new int[1000];
        }

        public int[] AlternansIndexArray
        {
            get
            {
                return _alternansIndexArray;
            }
            set
            {
                _alternansIndexArray = value;
            }
        }
    }
}
