﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.TestModule3
{
    public class TestModule3_Params : ModuleParams
    {
        private int _scale;
        private int _step;
        private string _analysisName;

        public int Scale
        {
            get
            {
                return _scale;
            }

            set
            {
                _scale = value;
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

        public TestModule3_Params(int scale, int step, string analysisName)
        {
            this.Scale = scale;
            this.Step = step;
            this.AnalysisName = analysisName;

        }
    }
}
