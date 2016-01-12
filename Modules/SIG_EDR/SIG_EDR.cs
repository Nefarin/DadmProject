using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.SIG_EDR
{
    public partial class SIG_EDR : IModule
    {
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

        public bool IsAborted()
        {
            return true;
            //return Aborted;
        }
    }
}
