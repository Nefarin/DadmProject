using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

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
