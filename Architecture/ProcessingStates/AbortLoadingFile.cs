using EKG_Project.Architecture.GUIMessages;

namespace EKG_Project.Architecture.ProcessingStates
{
    public class AbortLoadingFile : IProcessingState
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
            process.Communication.SendProcessingEvent(new LoadingAborted());
            timeoutState = new Idle(5);
        }
    }
}
