using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.Waves
{
    public enum Wavelet_Type { haar, db2, db3 };

    public class Waves_Params : ModuleParams
    {
        private Wavelet_Type _waveType;
        private int _decompositionLevel;

        public Wavelet_Type WaveType {
            get
            {
                return _waveType;
            }
            set
            {
                _waveType = value;
            }
        }
        public int DecompositionLevel
        {
            get
            {
                return _decompositionLevel;
            }
            set
            {
                _decompositionLevel = value;
            }
        }

        public Waves_Params( Wavelet_Type waveType, int decompositionLevel)
        {
            this.WaveType = waveType;
            this.DecompositionLevel = decompositionLevel;
        }
    }
}
