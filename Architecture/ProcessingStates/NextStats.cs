using EKG_Project.Architecture;
using EKG_Project.Modules;
using System;

namespace EKG_Project.Architecture.ProcessingStates
{
    public class NextStats : IProcessingState
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
