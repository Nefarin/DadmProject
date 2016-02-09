using System;

namespace EKG_Project.Architecture.ProcessingStates
{
    #region Documentation
    /// <summary>
    /// Message to Analysis thread, which starts next statistics calculation.
    /// </summary>
    /// 
    #endregion
    public class NextStats : IProcessingState
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
            if (process.Stats.CurrentModuleIndex < process.Stats.Amount())
            {
                try
                {
                    process.Stats.initNextStats();
                    timeoutState = new ProcessStats();
                }

                catch (Exception e)
                {
                    timeoutState = new NextStats();
                }


            }
            else
            {
                timeoutState = new SStatsCalculationEnded();
            }

        }
    }
}
