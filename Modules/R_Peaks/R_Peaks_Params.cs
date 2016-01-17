using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.R_Peaks
{   
    /// <summary>
    /// Enum includes names for available methods of detection R_peaks in class R_peaks
    /// </summary>
    public enum R_Peaks_Method { PANTOMPKINS, HILBERT, EMD };

    /// <summary>
    /// Class that includes input paramteters for class R_peaks
    /// </summary>
    public class R_Peaks_Params : ModuleParams
    {
        /// <summary>
        /// Method of detection R_peaks chosen by user
        /// </summary>
        private R_Peaks_Method _method;

        /// <summary>
        /// Constructor for R_Peaks_Params with parameters method and analysis name
        /// </summary>
        /// <param name="method"> Method chosen by user ( PANTOMPKINS, HILBERT or EMD)</param>
        /// <param name="analysisName"> Names of current analysis </param>
        public R_Peaks_Params(R_Peaks_Method method, string analysisName)
        {
            this.Method = method;
            this.AnalysisName = analysisName;
        }

        /// <summary>
        /// Default constructor for R_Peaks_Params
        /// </summary>
        public R_Peaks_Params() : base()
        {
            this.Method = R_Peaks_Method.PANTOMPKINS;
        }

        /// <summary>
        /// Construkctor for R_peak_params with analysis name as parameter
        /// </summary>
        /// <param name="analysisName"> Name of current analysis </param>
        public R_Peaks_Params(string analysisName) : this()
        {
            this.AnalysisName = analysisName;
        }

        public R_Peaks_Method Method
        {
            get
            {
                return _method;
            }

            set
            {
                _method = value;
            }
        }
    }
}