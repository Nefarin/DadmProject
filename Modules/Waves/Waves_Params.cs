using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.Waves
{
    public enum Waves_Method { DWT, MORLET };

    public class Waves_Params : ModuleParams
    {
 
        private Waves_Method _method;


        public Waves_Params(Waves_Method method)
        {
            _method = method;
        }



        public Waves_Method Method
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
