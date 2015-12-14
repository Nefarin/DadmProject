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

        

    }
}
