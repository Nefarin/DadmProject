using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.R_Peaks
{
    public enum R_Peaks_Method { PANTOMPKINS, HILBERT, EMD };

    public class R_Peaks_Params : ModuleParams
    {
        private R_Peaks_Method _method;
        /// Metoda detekcji zalamków R

        public R_Peaks_Params(R_Peaks_Method method)
        {
            Method = method;
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
        public void CopyParametersFrom(R_Peaks_Params parameters)
        {
            this.Method = parameters.Method;
        }
    }
}
