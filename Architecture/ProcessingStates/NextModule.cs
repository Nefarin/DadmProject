using EKG_Project.Architecture;
using EKG_Project.Modules;
using System;

namespace EKG_Project.Architecture.ProcessingStates
{
    public class NextModule : IProcessingState
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
