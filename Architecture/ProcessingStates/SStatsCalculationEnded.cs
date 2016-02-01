using EKG_Project.Architecture;
using EKG_Project.Modules;
using EKG_Project.GUI;
using System.Collections.Generic;
using EKG_Project.Architecture.GUIMessages;
using System;


namespace EKG_Project.Architecture.ProcessingStates
{
    public class SStatsCalculationEnded : IProcessingState
    {
        public void Process(Processing process, out IProcessingState timeoutState)
        {
            process.Communication.SendProcessingEvent(new StatsCalculationEnded(process.Stats.Results));
            timeoutState = new Idle(5);
        }
    }
}
