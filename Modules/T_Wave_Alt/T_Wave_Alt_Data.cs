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
        private List<Tuple<int,int>> _alternansDetectedList;
        // private double _alternansPercent;

        public T_Wave_Alt_Data()
        {
            _alternansDetectedList = new List<Tuple<int, int>>();
            // _alternansPercent = 0;
        }

        public List<Tuple<int,int>> AlternansDetectedList
        {
            get
            {
                return _alternansDetectedList;
            }
            set
            {
                _alternansDetectedList = value;
            }
        }
        /*
        public double AlternansPercent
        {
            get
            {
                return _alternansPercent;
            }
            set
            {
                _alternansPercent = value;
            }
        }
        */
    }
}
