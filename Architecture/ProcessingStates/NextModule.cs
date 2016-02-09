using System;

namespace EKG_Project.Architecture.ProcessingStates
{
    #region Documentation
    /// <summary>
    /// Message to Analysis thread, which starts next module.
    /// </summary>
    /// 
    #endregion
    public class NextModule : IProcessingState
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
            if (process.Modules.CurrentModuleProcessed < process.Modules.Amount())
            {
                try {
                    process.Modules.initNextModule();

                    if (process.Modules.CurrentModule.Runnable())
                    {
                        timeoutState = new ProcessModule();
                    }
                    else
                    {
                        timeoutState = new NextModule();
                    }
                }

                catch (Exception e)
                {
                    timeoutState = new NextModule();
                }


            }
            else
            {
                timeoutState = new SProcessingEnded();
            }

        }
    }
}
