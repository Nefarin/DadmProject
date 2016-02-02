using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.HRV_DFA
{
    #region HRV_DFA_Params Class documentation
    /// <summary>
    /// Class that defines input parameters for HRV_DFA module
    /// </summary>
    #endregion
    public class HRV_DFA_Params : ModuleParams
    {
        #region
        /// <summary>
        /// Constructor for HRV_DFA_Params 
        /// </summary>
        /// <param name="analysisName">Name of current analysis</param>
        #endregion
        public HRV_DFA_Params(string analysisName)
        {
            this.AnalysisName = analysisName;
        }
    }
}
