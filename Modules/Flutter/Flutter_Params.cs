using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.Flutter
{
    public class Flutter_Params : ModuleParams
    {
        public string AnalysisName { get; set; }

        public Flutter_Params(string analysisName)
        {
            AnalysisName = analysisName;
        }
    }
}
