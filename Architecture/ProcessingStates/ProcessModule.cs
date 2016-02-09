using EKG_Project.Architecture.GUIMessages;

namespace EKG_Project.Architecture.ProcessingStates
{
    #region Documentation
    /// <summary>
    /// Message to Analysis thread, which ends current processing.
    /// </summary>
    /// 
    #endregion
    public class ProcessModule : IProcessingState
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
