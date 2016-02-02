using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.ECG_Baseline
{
    #region
    /// <summary>
    /// This class includes the results of ECG_Baseline class 
    /// </summary>
    #endregion
    public class ECG_Baseline_Data : ECG_Data
    {
        /// <summary>
        /// List of tuple that includes vectors of filtered signals for every leads
        /// </summary>
        private List<Tuple<string, Vector<double>>> _signalsFiltered;

        /// <summary>
        /// Default constructor ECG_Baseline_Data() that initializes epmty list of tuple for filtered signal
        /// </summary>
        public ECG_Baseline_Data()
        {
            this.SignalsFiltered = new List<Tuple<string, Vector<double>>>();
        }

        public List<Tuple<string, Vector<double>>> SignalsFiltered
        {
            get
            {
                return _signalsFiltered;
            }
            set
            {
                _signalsFiltered = value;
            }
        }

    }
}
