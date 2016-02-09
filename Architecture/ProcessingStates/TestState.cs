using EKG_Project.Architecture.GUIMessages;

namespace EKG_Project.Architecture.ProcessingStates
{
    #region Documentation
    /// <summary>
    /// Debugging purposes.
    /// </summary>
    /// 
    #endregion
    public class TestState : IProcessingState
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
            timeoutState = new Idle(5);
            process.Communication.SendProcessingEvent(new TestMessage("Dynamic dipsatch test!"));
        }
    }
}
