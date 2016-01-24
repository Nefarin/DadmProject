using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Architecture.ProcessingStates
{
    public class NextAnalysis : IProcessingState
    {

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        /// <param name="timeoutState"></param>
        /// 
        #endregion
        public void Process(Processing process, out IProcessingState timeoutState)
        {
            timeoutState = new Idle(5);
        }
    }
}
