using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.Heart_Axis
{
    public class Heart_Axis_Params : ModuleParams
    {

        /// <summary>
        /// Construkctor for Heart_Axis_Params with analysis name as parameter
        /// </summary>
        /// <param name="analysisName"> Name of current analysis </param>

        public Heart_Axis_Params(string analysisName)
        {
            this.AnalysisName = analysisName;
        }
    }
}

