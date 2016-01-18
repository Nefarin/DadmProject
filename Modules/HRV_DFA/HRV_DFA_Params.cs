using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.HRV_DFA
{
    /// <summary>
    /// Class that includes input paramteters for class HRV_DFA
    /// </summary>
    public class HRV_DFA_Params : ModuleParams
    {
        /// <summary>
        /// Constructor for HRV_DFA_Params with analysisName parameter
        /// </summary>
        /// <param name="analysisName">Name of current analysis</param>
        public HRV_DFA_Params(string analysisName)
        {
            this.AnalysisName = analysisName;
        }
    }
}
