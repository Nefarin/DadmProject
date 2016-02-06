using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.Sleep_Apnea
{
    public class Sleep_Apnea_Params : ModuleParams
    {
        #region Sleep_Apnea_Params Class doc
        /// <summary>
        /// Constructor for Sleep_Apnea_params with analysis name as parameter
        /// </summary>
        /// <param name="analysisName"> Name of current analysis </param>
        #endregion

        public Sleep_Apnea_Params(string analysisName)
        {
            this.AnalysisName = analysisName;
        }
    }
}