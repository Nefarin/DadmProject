using EKG_Project.Architecture;
using EKG_Project.Modules;
using EKG_Project.IO;
using EKG_Project.Architecture.GUIMessages;
using System;


namespace EKG_Project.Architecture.ProcessingStates
{
    public class ProcessFile : IProcessingState
    {
        public void Process(Processing process, out IProcessingState timeoutState)
        {
            if (process.FileProcessor.Ended())
            {
                process.Communication.SendProcessingEvent(new FileLoaded());
                timeoutState = new Idle(5);
            }
            else
            {
                process.FileProcessor.Process();
                timeoutState = new ProcessFile();
            }

        }
    }
}
