using EKG_Project.Architecture.GUIMessages;

namespace EKG_Project.Architecture.ProcessingStates
{
    #region Documentation
    /// <summary>
    /// Message to Analysis thread, which controls loading file process.
    /// </summary>
    /// 
    #endregion
    public class ProcessFile : IProcessingState
    {
        #region Documentation
        /// <summary>
        /// Sets next processing state.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="timeoutState"></param>
        #endregion
        public void Process(Processing process, out IProcessingState timeoutState)
        {
            if (process.FileProcessor.Ended())
            {
                process.Modules.FileLoaded = true;
                process.Communication.SendProcessingEvent(new FileLoaded());
                timeoutState = new Idle(5);
            }
            else
            {
                process.FileProcessor.Process();
                process.Communication.SendProcessingEvent(new FileProgress(process.FileProcessor.Progress()));
                timeoutState = new ProcessFile();
            }

        }
    }
}
