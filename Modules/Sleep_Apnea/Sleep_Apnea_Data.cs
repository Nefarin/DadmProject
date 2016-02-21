using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using EKG_Project.Modules.ECG_Baseline;
using EKG_Project.Modules.R_Peaks;

namespace EKG_Project.Modules.Sleep_Apnea
{

    #region Sleep_Apnea_Data Class doc
    /// <summary>
    /// Class that includes output results of class Sleep_Apnea
    /// </summary>
    #endregion
    public class Sleep_Apnea_Data : ECG_Data
    {
        #region
        /// <summary>
        /// List of tuples with list of tuples with indexes of beginning and ending of detected sleep apnea
        /// </summary>
        #endregion

        private List<Tuple<string, List<Tuple<int, int>>>> _Detected_Apnea;

        #region
        /// <summary>
        /// List of tuples with list of lists with indexes of R peaks and calculated Hibert's amplitude
        /// </summary>
        #endregion

        private List<Tuple<string, List<List<double>>>> _h_amp;

        #region
        /// <summary>
        /// List of tuples with percentage value of detected sleep apnea
        /// </summary>
        #endregion

        private List<Tuple<string, double>> _il_Apnea;

        public Sleep_Apnea_Data()
        {
            _Detected_Apnea = new List<Tuple<string, List<Tuple<int, int>>>>();
            _h_amp = new List<Tuple<string, List<List<double>>>>();
            _il_Apnea = new List<Tuple<string, double>>();
        }

        public List<Tuple<string, List<Tuple<int, int>>>> Detected_Apnea
        {
            get
            {
                return _Detected_Apnea;
            }
            set
            {
                _Detected_Apnea = value;
            }
        }

        public List<Tuple<string, List<List<double>>>> h_amp
        {
            get
            {
                return _h_amp;
            }
            set
            {
                _h_amp = value;
            }
        }


        public List<Tuple<string, double>> il_Apnea
        {
            get
            {
                return _il_Apnea;
            }
            set
            {
                _il_Apnea = value;
            }
        }
    }
}


