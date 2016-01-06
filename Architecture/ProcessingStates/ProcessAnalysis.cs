using EKG_Project.Architecture;
using EKG_Project.Modules;

namespace EKG_Project.Architecture.ProcessingStates
{
    public class ProcessAnalysis : IProcessingState
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
