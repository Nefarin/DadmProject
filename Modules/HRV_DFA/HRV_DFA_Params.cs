using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.HRV_DFA
{

    public class HRV_DFA_Params : ModuleParams
    {
        //HRV_DFA parameters
        private string _analysisName;

        public HRV_DFA_Params(string analysisName)
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
