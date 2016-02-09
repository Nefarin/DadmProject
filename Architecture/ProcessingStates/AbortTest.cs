using EKG_Project.Architecture.GUIMessages;

namespace EKG_Project.Architecture.ProcessingStates
{
    #region Documentation
    /// <summary>
    /// Debugging purposes.
    /// </summary>
    /// 
    #endregion
    public class AbortTest : IProcessingState
    {
        #region Documentation
        /// <summary>
        /// Sets next processing state.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="timeoutState"></param>
        /// 
        #endregion
        public void Process(Processing process, out IProcessingState timeoutState)
        {
            timeoutState = new AbortTest();
            process.Communication.SendProcessingEvent(new AnalysisEndedTest());
            process.Stop = true;
        }
    }
}
