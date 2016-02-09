using EKG_Project.Architecture.GUIMessages;

namespace EKG_Project.Architecture.ProcessingStates
{
    #region Documentation
    /// <summary>
    /// Message to Analysis thread, which ends current module.
    /// </summary>
    /// 
    #endregion
    public class ModuleEnded : IProcessingState
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
            process.Modules.CurrentModuleProcessed++;
            process.Communication.SendProcessingEvent(new EndedModule(process.Modules.CurrentOption, process.Modules.CurrentModule.IsAborted()));
            timeoutState = new NextModule();
        }
    }
}
