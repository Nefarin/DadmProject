using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.Heart_Axis
{
    public class Heart_Axis_Params : ModuleParams
    {
        private string _analysisName;
        public Heart_Axis_Params(string analysisName)
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

