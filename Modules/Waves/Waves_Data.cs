using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.Waves
{
    public class Waves_Data : ECG_Data
    {
        private Vector<double> _ecg;
        private List<int> _Rpeaks;

        private List<int> _QRSonsets;
        private List<int> _QRSends;
        private List<int> _Ponsets;
        private List<int> _Pends;
        private List<int> _Tends;

        private uint _fs;

        public Waves_Data() {
            _ecg = Vector<double>.Build.Dense(1);

            _Rpeaks = new List<int>();
            _QRSends = new List<int>();
            _QRSonsets = new List<int>();
            _Pends = new List<int>();
            _Ponsets = new List<int>();
            _Tends = new List<int>();
        }

        public Waves_Data( Vector<double> ecg, List<int> RpeaksIn, uint fs):this()
        {
            ECG = ecg;
            Rpeaks = RpeaksIn;
            Fs = fs;
        }

        public Vector<double> ECG
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

        public List<int> Rpeaks
        {
            get
            {
                return _Rpeaks;
            }
            set
            {
                _Rpeaks = value;
            }
        }

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
        public uint Fs
        {
            get
            {
                return _fs;
            }
            set
            {
                _fs = value;
            }
        }

    }
}
