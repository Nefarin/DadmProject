using EKG_Project.Architecture;
using EKG_Project.Architecture.GUIMessages;
using EKG_Project.Modules;
using System;

namespace EKG_Project.Architecture.ProcessingStates
{
    public class ProcessModule : IProcessingState
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
            if (process.Modules.CurrentModule.Ended())
            {
                timeoutState = new ModuleEnded();
            }
            else
            {
                process.Modules.CurrentModule.ProcessData();
                process.Communication.SendProcessingEvent(new ModuleProgress(process.Modules.CurrentOption, process.Modules.CurrentModule.Progress()));
                timeoutState = new ProcessModule();
            }
        }
    }
}
