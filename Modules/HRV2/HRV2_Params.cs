using System;

namespace EKG_Project.Modules.HRV2
{
    public class HRV2_Params : ModuleParams
{
        public HRV2_Params(string analysisName)
        {
            this.AnalysisName = analysisName;
        }

        public HRV2_Params(int scale, int step, string analysisName)
        {
            this.AnalysisName = analysisName;

        }
    }
}
