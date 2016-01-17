using EKG_Project.Architecture;
using EKG_Project.Modules;
using EKG_Project.GUI;
using System.Collections.Generic;
using EKG_Project.Architecture.GUIMessages;
using System;


namespace EKG_Project.Architecture.ProcessingStates
{
    public class SProcessingEnded : IProcessingState
    {
        public void Process(Processing process, out IProcessingState timeoutState)
        {
            process.Communication.SendProcessingEvent(new ProcessingEnded());
            timeoutState = new Idle(5);
        }
    }
}
