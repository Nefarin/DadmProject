using EKG_Project.Architecture.GUIMessages;

namespace EKG_Project.Architecture.ProcessingStates
{
    #region Documentation
    /// <summary>
    /// Message to Analysis thread, which controls stats processing.
    /// </summary>
    /// 
    #endregion
    public class ProcessStats : IProcessingState
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
            if (process.Stats.CurrentStats.Ended())
            {
                timeoutState = new StatsEnded();
            }
            else
            {          
                process.Stats.CurrentStats.ProcessStats();
                process.Communication.SendProcessingEvent(new StatsProgress(100D * (double) process.Stats.CurrentModuleIndex / (double) process.Stats.IsComputed.Count));
                timeoutState = new ProcessStats();
            }
        }
    }
}
