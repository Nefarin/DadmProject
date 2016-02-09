
namespace EKG_Project.Architecture.ProcessingStates
{
    #region Documentation
    /// <summary>
    /// Message to Analysis thread, which ends current stats module.
    /// </summary>
    #endregion
    public class StatsEnded : IProcessingState
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
            process.Stats.Results.Add(process.Stats.CurrentOption, process.Stats.CurrentStats.GetStatsAsString());
            process.Stats.CurrentModuleProcessed++;
            timeoutState = new NextStats();
        }
    }
}
