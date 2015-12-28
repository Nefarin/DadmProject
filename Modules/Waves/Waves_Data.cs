using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.Waves
{
    class Waves_Data : ECG_Data
    {
        private List<int> _QRSonsets;
        private List<int> _QRSends;
        private List<int> _Ponsets;
        private List<int> _Pends;
        private List<int> _Tends;

        public Waves_Data() { }

        public List<int> QRSOnsets
        {
            get
            {
                return _QRSonsets;
            }
            set
            {
                _QRSonsets = value;
            }
        }

        public List<int> QRSEnds
        {
            get
            {
                return _QRSends;
            }
            set
            {
                _QRSends = value;
            }
        }

        public List<int> POnsets
        {
            get
            {
                return _Ponsets;
            }
            set
            {
                _Ponsets = value;
            }

        }

        public List<int> PEnds
        {
            get
            {
                return _Pends;
            }

            set
            {
                _Pends = value;
            }
        }

        public List<int> TEnds
        {
            get
            {
                return _Tends;
            }
            set
            {
                _Tends = value;
            }
        }
    }
}
