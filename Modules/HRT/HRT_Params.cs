using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.HRT
{
    public class HRT_Params : ModuleParams
    {

        enum RodzajWykresu { Pojedynczy, Zlozony};
        public string AnalysisName;

        public HRT_Params(string analysisname)
        {
            this.AnalysisName = analysisname;
        }
    }
}
