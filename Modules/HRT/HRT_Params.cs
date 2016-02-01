using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.HRT
{
    /// <summary>
    /// Class that includes input parameters for class HRT
    /// </summary>
    public class HRT_Params : ModuleParams
    { 
        public string AnalysisName;

        /// <summary>
        /// Constructor for HRT_params with analysis name as parameter
        /// </summary>
        /// <param name="analysisName"> Name of current analysis </param>
        public HRT_Params(string analysisname)
        {
            this.AnalysisName = analysisname;
        }
    }
}
