using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.R_Peaks
{
    public enum R_Peaks_Method { PANTOMPKINS, HILBERT, EMD };

    public class R_Peaks_Params : ModuleParams
    {
        private R_Peaks_Method _method;
        // private Vector<double> _ecgFiltered;
        private uint _fs;

        public R_Peaks_Params(R_Peaks_Method method, /*Vector<double> ecgFiltered,*/ uint fs)
        {
            _method = method;
            // _ecgFiltered = ecgFiltered;
            _fs = fs;
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

        /*public Vector<double> EcgFiltered
        {
            get
            {
                return _ecgFiltered;
            }
        }*/

        public uint Fs
        {
            get
            {
                return _fs;
            }
            //set?
        }


    }
}
