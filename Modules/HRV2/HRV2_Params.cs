using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.HRV2
{

    public class HRV2_Params : ModuleParams
    {
        private Vector<int> _RRintervals;

        public Vector<int> RRintervals
        {
            get
            {
                return _RRintervals;
            }

            set
            {
                _RRintervals = value;
            }
        }
    }
}
