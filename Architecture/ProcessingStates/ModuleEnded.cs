﻿using EKG_Project.Architecture;
using EKG_Project.Modules;
using EKG_Project.Architecture.GUIMessages;

namespace EKG_Project.Architecture.ProcessingStates
{
    public class ModuleEnded : IProcessingState
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
            process.Communication.SendProcessingEvent(new EndedModule(process.Modules.CurrentOption, process.Modules.CurrentModule.IsAborted()));
            timeoutState = new NextModule();
        }
    }
}