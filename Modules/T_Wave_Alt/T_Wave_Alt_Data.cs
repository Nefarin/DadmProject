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
    class T_Wave_Alt_Data : ECG_Data
    {
        private Vector<int> _t_end_Vec;
        private Vector<double> _ecg;

        public T_Wave_Alt_Data() {}

        public Vector<int> t_end_Vec
        {
            get
            {
                return _t_end_Vec;
            }
            set
            {
                _t_end_Vec = value;
            }
        }

        public Vector<double> ecg
        {
            get
            {
                return _ecg;
            }
            set
            {
                _ecg = value;
            }
        }
    }
}
