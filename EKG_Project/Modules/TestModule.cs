using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules
{
    #region Documentation
    /// <summary>
    /// Test module class.
    /// </summary>
    /// 
    #endregion
    public class TestModule : Module
    {
        private bool _aborted;
        private bool _ended;
        private uint _numberOfSamples;
        private uint _numberOfProcessedSamples;

        public TestModule()
        {
            _aborted = false;
            _ended = false;
        }

        public bool Ended()
        {
            return _ended;
        }

        public void Abort()
        {
            _aborted = true;
            _ended = true;
        }

        public void Init(TestModule_Params parameters)
        {
            _numberOfSamples = parameters.NumberOfSamples;
        }

        public void ProcessData(int numberOfSamples)
        {

        }

        public double Progress()
        {
            return (double)_numberOfProcessedSamples / (double)_numberOfSamples;
        }
    }
}
