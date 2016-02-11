using EKG_Project.Architecture.GUIMessages;

namespace EKG_Project.Architecture.ProcessingStates
{
    #region Documentation
    /// <summary>
    /// Message to Analysis thread, which aborts loading current file.
    /// </summary>
    /// 
    #endregion
    public class AbortStats : IProcessingState
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
            process.Communication.SendProcessingEvent(new StatsAborted());
            timeoutState = new Idle(5);
        }
    }
}
