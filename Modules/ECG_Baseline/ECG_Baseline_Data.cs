using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.ECG_Baseline
{
    class ECG_Baseline_Data : IO.ECG_Data
    {
        private List<Tuple<string, Vector<double>>> _signalsFiltered;   //taka konwencja -> jak Wam bardzo przeszkadza 
                                                                        //mozemy pomyslec nad zmiana

        public ECG_Baseline_Data() {}

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
