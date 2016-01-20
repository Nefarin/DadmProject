using EKG_Project.Architecture;
using EKG_Project.Architecture.GUIMessages;
using EKG_Project.Modules;
using System;

namespace EKG_Project.Architecture.ProcessingStates
{
    public class ProcessStats : IProcessingState
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
            if (process.Stats.CurrentStats.Ended())
            {
                timeoutState = new StatsEnded();
            }
            else
            {
                process.Stats.CurrentStats.ProcessStats();
                timeoutState = new ProcessStats();
            }
        }
    }
}
