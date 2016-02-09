using EKG_Project.Architecture.GUIMessages;

namespace EKG_Project.Architecture.ProcessingStates
{
    #region Documentation
    /// <summary>
    /// Message to Analysis thread, which ends stats calculation.
    /// </summary>
    #endregion
    public class SStatsCalculationEnded : IProcessingState
    {
        /// <summary>
        /// Sets next processing state.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="timeoutState"></param>
        public void Process(Processing process, out IProcessingState timeoutState)
        {
            process.Communication.SendProcessingEvent(new StatsCalculationEnded(process.Stats.Results));
            timeoutState = new Idle(5);
        }
    }
}
