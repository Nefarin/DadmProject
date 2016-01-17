using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.T_Wave_Alt
{
    public class T_Wave_Alt_Params : ModuleParams
    {
        public T_Wave_Alt_Params() : base()  {}

        public T_Wave_Alt_Params(string analysisName)
        {
            this.AnalysisName = analysisName;
        }
    }
}
