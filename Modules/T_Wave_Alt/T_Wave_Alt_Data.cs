using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.T_Wave_Alt
{
    class T_Wave_Alt_Data : IO.ECG_Data
    {
        private int[] _alternansIndexArray;

        public T_Wave_Alt_Data() {}

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
