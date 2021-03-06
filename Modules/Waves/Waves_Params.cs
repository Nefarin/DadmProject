﻿using System;
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
        private int _rpeaks_step;

        public Wavelet_Type WaveType
        {
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
                if (value <= 0) _decompositionLevel = 1;
                else _decompositionLevel = value;
            }
        }

        public int RpeaksStep
        {
            get
            {
                return _rpeaks_step;
            }
            set
            {
                _rpeaks_step = value;
            }
        }
        public Waves_Params(Wavelet_Type waveType, int decompositionLevel, string analysisName, int rpeaksStep)
        {
            if (decompositionLevel < 1)
                throw new ArgumentOutOfRangeException("Decomposition level below 1");

            if (rpeaksStep < 1)
                throw new ArgumentOutOfRangeException("R-peaks step size below 1");
            this.WaveType = waveType;
            this.DecompositionLevel = decompositionLevel;
            this.AnalysisName = analysisName;
            this.RpeaksStep = rpeaksStep;
        }

        public Waves_Params() : base()
        {
            _waveType = Wavelet_Type.haar;
            DecompositionLevel = 3;
            RpeaksStep = 500;
        }

        public Waves_Params(string analysisName) : this()
        {
            this.AnalysisName = analysisName;
        }
    }
}