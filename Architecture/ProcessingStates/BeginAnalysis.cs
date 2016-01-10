using EKG_Project.Architecture;
using EKG_Project.Modules;

namespace EKG_Project.Architecture.ProcessingStates
{
    public class BeginAnalysis : IProcessingState
    {
        public void Process(Processing process, out IProcessingState timeoutState)
        {
            process.Modules = new Modules();
            process.Modules.Init();

            timeoutState = new NextAnalysis();
        }
    }
}
