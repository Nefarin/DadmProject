using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.Atrial_Fibr
{
    public enum Detect_Method { STATISTIC, POINCARE };
    public class Atrial_Fibr_Params : ModuleParams
    {
        private Detect_Method _method;

        #region Documentation
        /// <summary>
        /// TO DO
        /// </summary>
        #endregion
        public Detect_Method Method
        {
            get { return _method; }
            set { _method = value; }
        }

        #region Documentation
        /// <summary>
        /// TO DO
        /// </summary>
        #endregion
        public Atrial_Fibr_Params(Detect_Method method, string analysisName)
        {
            this.Method = method;
            this.AnalysisName = analysisName;
        }

        #region Documentation
        /// <summary>
        /// TO DO
        /// </summary>
        #endregion
        public Atrial_Fibr_Params(string analysisName) : this()
        {
            this.AnalysisName = analysisName;
        }

        #region Documentation
        /// <summary>
        /// TO DO
        /// </summary>
        #endregion
        public Atrial_Fibr_Params() : base()
        {
            this.Method = Detect_Method.STATISTIC;
        }
    }
}