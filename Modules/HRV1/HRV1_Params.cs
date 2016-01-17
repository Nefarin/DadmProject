using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;


namespace EKG_Project.Modules.HRV1
{
    public class HRV1_Params : ModuleParams
    {
        public HRV1_Params(string analysisname)
        {
            this.AnalysisName = analysisname;
        }
    }
}
