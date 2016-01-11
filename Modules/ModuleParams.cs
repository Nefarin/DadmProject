namespace EKG_Project.Modules
{
    #region Documentation
    /// <summary>
    /// Basic parameters will be provided to modules in successors to this class (for example if R_Peaks should be calculated by Hilbert Transform or something else)
    /// </summary>
    /// 
    #endregion
    public class ModuleParams
    {
        public string AnalysisName { get; set; }

        public ModuleParams()
        {
            this.AnalysisName = "undefined";
        }
    }
}
