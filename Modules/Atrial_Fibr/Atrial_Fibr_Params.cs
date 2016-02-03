using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.Atrial_Fibr
{
    #region Documentation
    /// <summary>
    /// /// Enum includes names for available methods of detection Atrial Fibrillation
    /// </summary>
    #endregion
    public enum Detect_Method { STATISTIC, POINCARE };
    public class Atrial_Fibr_Params : ModuleParams
    {
        #region Documentation
        /// <summary>
        /// Method of detection Atrial Fibrillation chosen by user
        /// </summary>
         #endregion
        private Detect_Method _method;

        public Detect_Method Method
        {
            get { return _method; }
            set { _method = value; }
        }

        #region Documentation
        /// <summary>
        /// Constructor for Atrial_Fibr_Params with parameters method and analysis name
        /// </summary>
        #endregion
        public Atrial_Fibr_Params(Detect_Method method, string analysisName)
        {
            this.Method = method;
            this.AnalysisName = analysisName;
        }

        #region Documentation
        /// <summary>
        /// Construkctor for Atrial_Fibr_params with analysis name as parameter
        /// </summary>
        #endregion
        public Atrial_Fibr_Params(string analysisName) : this()
        {
            this.AnalysisName = analysisName;
        }

        #region Documentation
        /// <summary>
        /// Default constructor for Atrial_Fibr_Params
        /// </summary>
        #endregion
        public Atrial_Fibr_Params() : base()
        {
            this.Method = Detect_Method.STATISTIC;
        }
    }
}