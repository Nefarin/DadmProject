using System;
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
                if (value < 1) throw new ArgumentOutOfRangeException();
                _step = value;
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
