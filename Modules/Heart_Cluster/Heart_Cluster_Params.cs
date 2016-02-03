using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Solvers;

namespace EKG_Project.Modules.Heart_Cluster
{
    public class Heart_Cluster_Params : ModuleParams
    {
        public Heart_Cluster_Params() : base() { }

        public Heart_Cluster_Params(string analysisName)
        {
            this.AnalysisName = analysisName;
        }
    }
}
