using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.Waves
{
    public enum Waves_Method { DWT, MORLET };

    public class Waves_Params : ModuleParams
    {
        private uint _signalSize;
        private Waves_Method _method;
        private Vector <double> _ecg;
        private Vector<uint> _Rpeaks;
        //moze bd tak, a moze nie, kto to wie...
        private Vector< uint > _QRSonsets;
        private Vector< uint > _QRSends;
        private Vector< uint > _Ponsets;
        private Vector< uint > _Pends;
        private Vector< uint > _Tends;

        public Waves_Params( uint signalSize, Waves_Method method, Vector <double> ecg, Vector < uint > Rpeaks)
        {
            _signalSize = signalSize;
            _method = method;
            _ecg = ecg;
            _Rpeaks = Rpeaks;
        }

        public uint SignalSize
        {
            get
            {
                return _signalSize;
            }

            set
            {
                _signalSize = value;
            }
        }

        public Waves_Method Method
        {
            get
            {
                return _method;
            }

            set
            {
                _method = value;
            }
        }

        public Vector<double> Ecg
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

        public Vector<uint> Rpeaks
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

        public Vector<uint> QRSOnsets
        {
            get
            {
                return _QRSonsets;
            }

        }

        public Vector<uint> QRSEnds
        {
            get
            {
                return _QRSends;
            }

        }

        public Vector<uint> POnsets
        {
            get
            {
                return _Ponsets;
            }

        }

        public Vector<uint> PEnds
        {
            get
            {
                return _Pends;
            }

        }

        public Vector<uint> TEnds
        {
            get
            {
                return _Tends;
            }

        }

    }
}
