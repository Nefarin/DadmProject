﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.ECG_Baseline
{
    public class ECG_Baseline_Data : ECG_Data
    {
        private List<Tuple<string, Vector<double>>> _signalsFiltered;    

        public ECG_Baseline_Data() {}

        public List<Tuple<string, Vector<double>>> SignalsFiltered
        {
            get
            {
                return _signalsFiltered;
            }
            set
            {
                _signalsFiltered = value;
            }
        }
    }
}
