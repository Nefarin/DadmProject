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
        private string _analysisName;
        private int _step;

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

        public string AnalysisName
        {
            get
            {
                return _analysisName;
            }
            set
            {
                _analysisName = value;
            }
        }

        public int Step
        {
            get
            {
                return _step;
            }
            set
            {
                _step = value;
            }
        }
        public Waves_Params( Wavelet_Type waveType, int decompositionLevel, string analysisName, int step)
        {
            this.WaveType = waveType;
            this.DecompositionLevel = decompositionLevel;
            this.AnalysisName = analysisName;
            this.Step = step;
        }
    }
}
