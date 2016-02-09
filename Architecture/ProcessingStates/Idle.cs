using System.Threading;

namespace EKG_Project.Architecture.ProcessingStates
{
    #region Documentation
    /// <summary>
    /// Message to Analysis thread, which sleeps current analysis.
    /// </summary>
    /// 
    #endregion
    public class Idle : IProcessingState
    {
        private int _sleepTime;

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sleepTime"></param>
        /// 
        #endregion
        public Idle(int sleepTime)
        {
            _sleepTime = sleepTime;
        }

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
            Thread.Sleep(_sleepTime);
        }
    }
}
