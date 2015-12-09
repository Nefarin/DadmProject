using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.TestModule2
{
    public partial class Test_Module2 : IModule
    {
        private Test_Module2_Param _moduleParams;

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
            _moduleParams = (Test_Module2_Param)parameters;
        }

        public void ProcessData(int numberOfSamples)
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
    }
}
