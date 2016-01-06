using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.R_Peaks
{
    public partial class R_Peaks : IModule
    {
        public R_Peaks()
        {
            Delay = 0;
            LocsR = Vector<double>.Build.Dense(1);
            RRms = Vector<double>.Build.Dense(1);

        }

        public void Abort()
        {
            throw new NotImplementedException();
        }

        public bool Ended()
        {
            throw new NotImplementedException();
        }

        public void Init(ModuleParams parameters)
        {
            throw new NotImplementedException();
        }

        public void ProcessData()
        {
            R_Peaks pt = new R_Peaks();
            throw new NotImplementedException();
        }

        public double Progress()
        {
            throw new NotImplementedException();
        }

        public bool Runnable()
        {
            throw new NotImplementedException();
        }
    }
}
