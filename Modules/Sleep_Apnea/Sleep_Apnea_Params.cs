using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.Sleep_Apnea
{
    public class Sleep_Apnea_Params : ModuleParams
    {
        public Sleep_Apnea_Params(string analysisName)
        {
            this.AnalysisName = analysisName;
        }
    }
}