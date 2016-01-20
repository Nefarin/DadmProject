using EKG_Project.Architecture;
using EKG_Project.Modules;
using EKG_Project.Architecture.GUIMessages;
using System;

namespace EKG_Project.Architecture.ProcessingStates
{
    public class StatsEnded : IProcessingState
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
            process.Stats.Results.Add(process.Stats.CurrentOption, process.Stats.CurrentStats.GetStatsAsString());
            process.Stats.CurrentModuleProcessed++;
            timeoutState = new NextStats();
        }
    }
}
