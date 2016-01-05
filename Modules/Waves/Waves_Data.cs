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
        private List<Tuple<string, Vector<uint>>> _QRSonsets;
        private List<Tuple<string, Vector<uint>>> _QRSends;
        private List<Tuple<string, Vector<uint>>> _Ponsets;
        private List<Tuple<string, Vector<uint>>> _Pends;
        private List<Tuple<string, Vector<uint>>> _Tends;

        public Waves_Data() { }

        public List<Tuple<string, Vector<uint>>> QRSOnsets
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

        public List<Tuple<string, Vector<uint>>> QRSEnds
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

        public List<Tuple<string, Vector<uint>>> POnsets
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

        public List<Tuple<string, Vector<uint>>> PEnds
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

        public List<Tuple<string, Vector<uint>>> TEnds
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
