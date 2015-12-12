using EKG_Project.Architecture.GUIMessages;

namespace EKG_Project.Architecture.ProcessingStates
{
    #region Documentation
    /// <summary>
    /// 
    /// </summary>
    /// 
    #endregion
    public class Abort : IProcessingState
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
            timeoutState = new Abort();
            process.Communication.SendProcessingEvent(new AnalysisEnded());
            process.Stop = true;
        }
    }
}
