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
        private Vector<uint> _QRSonsets;
        private Vector<uint> _QRSends;
        private Vector<uint> _Ponsets;
        private Vector<uint> _Pends;
        private Vector<uint> _Tends;

        public Waves_Data() { }

        public Vector<uint> QRSOnsets
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

        public Vector<uint> QRSEnds
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

        public Vector<uint> POnsets
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

        public Vector<uint> PEnds
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

        public Vector<uint> TEnds
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
