using EKG_Project.GUI;
using System.Collections.Generic;
using EKG_Project.Architecture.GUIMessages;

#region Documentation
/// <summary>
/// Message to Analysis thread, which begins statistics calculation.
/// </summary>
/// 
#endregion
namespace EKG_Project.Architecture.ProcessingStates
{
    public class BeginStatsCalculation : IProcessingState
    {
        private Dictionary<AvailableOptions, bool> _modulesComputed;

        public Dictionary<AvailableOptions, bool> ModulesComputed
        {
            get
            {
                return _modulesComputed;
            }

            set
            {
                _modulesComputed = value;
            }
        }

        public BeginStatsCalculation(Dictionary<AvailableOptions, bool> modulesComputed)
        {
            ModulesComputed = modulesComputed;
        }

        /// <summary>
        /// Sets next processing state.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="timeoutState"></param>
        public void Process(Processing process, out IProcessingState timeoutState)
        {
            process.Stats.Init(ModulesComputed);
            process.Communication.SendProcessingEvent(new StatsCalculactionBegun());
            timeoutState = new NextStats();
        }
    }
}
