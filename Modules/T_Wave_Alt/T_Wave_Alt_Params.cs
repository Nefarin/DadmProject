using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.T_Wave_Alt
{
    public class T_Wave_Alt_Params : ModuleParams
    {
        private string _analysisName;

        public T_Wave_Alt_Params()
        {
            this.AnalysisName = "Analysis0";
        }

        public T_Wave_Alt_Params(string analysisName)
        {
            this.AnalysisName = analysisName;
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
    }
}
