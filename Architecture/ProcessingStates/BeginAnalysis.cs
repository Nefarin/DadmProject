using EKG_Project.Architecture;
using EKG_Project.Modules;
using EKG_Project.GUI;
using System.Collections.Generic;
using EKG_Project.Architecture.GUIMessages;

namespace EKG_Project.Architecture.ProcessingStates
{
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

        public void Process(Processing process, out IProcessingState timeoutState)
        {
            process.Modules = new Modules();
            process.Modules.Init(ModuleParams);

            process.Communication.SendProcessingEvent(new AnalysisStarted());

            timeoutState = new SProcessingEnded();
        }
    }
}
