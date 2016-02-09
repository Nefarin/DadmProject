using EKG_Project.Modules;
using EKG_Project.GUI;
using System.Collections.Generic;
using EKG_Project.Architecture.GUIMessages;

namespace EKG_Project.Architecture.ProcessingStates
{
    #region Documentation
    /// <summary>
    /// Message to Analysis thread, which begins processing based on given parameters.
    /// </summary>
    /// 
    #endregion
    public class BeginAnalysis : IProcessingState
    {
        private Dictionary<AvailableOptions, ModuleParams> _moduleParams;

        public BeginAnalysis(Dictionary<AvailableOptions, ModuleParams> moduleParams)
        {
            ModuleParams = moduleParams;
        }

        public Dictionary<AvailableOptions, ModuleParams> ModuleParams
        {
            get
            {
                return _moduleParams;
            }

            set
            {
                _moduleParams = value;
            }
        }


        /// <summary>
        /// Sets next processing state.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="timeoutState"></param>
        public void Process(Processing process, out IProcessingState timeoutState)
        {
            if (process.Modules.FileLoaded)
            {
                process.Modules.Init(ModuleParams);
                process.Communication.SendProcessingEvent(new AnalysisStarted());
                timeoutState = new NextModule();
            }
            else
            {
                process.Communication.SendProcessingEvent(new FileNotLoaded());
                timeoutState = new Idle(5);
            }

        }
    }
}
