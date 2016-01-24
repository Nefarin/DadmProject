using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.HRT
{
    ///<Summary>To są parametry jakie mogę ustawić na panelu użytkownika
   ///  i które są wczytywane do mojego modułu z GUI</Summary>
    public class HRT_Params : ModuleParams
    {

        enum RodzajWykresu { LINIOWY, SCHODKOWY};
        public string AnalysisName;

        public HRT_Params(string analysisname)
        {
            this.AnalysisName = analysisname;
        }
    }
}
