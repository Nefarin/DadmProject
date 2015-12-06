using System;


namespace EKG_Project.Modules
{
    public class TestModule_Params : ModuleParams
    {
        private uint _numberOfSamples;

        public TestModule_Params(uint numberOfSamples)
        {
            _numberOfSamples = numberOfSamples;
        }

        public uint NumberOfSamples
        {
            get
            {
                return _numberOfSamples;
            }

            set
            {
                _numberOfSamples = value;
            }
        }
    }
}
