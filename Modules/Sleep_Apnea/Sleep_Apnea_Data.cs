using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace EKG_Project.Modules.Sleep_Apnea
{
    class Sleep_Apnea_Data : ECG_Data
    {
        private Matrix<int> _Detected_Apnea;
        private Vector<double> _ecg;
        private int _il_Apnea;

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

        public Matrix<int> Detected_Apnea
        {
            get
            {
                return _Detected_Apnea;
            }
            set
            {
                _Detected_Apnea = value;
            }
        }

        public int il_Apnea
        {
            get
            {
                return _il_Apnea;
            }
            set
            {
                _il_Apnea = value;
            }
        }
    }
}
    
