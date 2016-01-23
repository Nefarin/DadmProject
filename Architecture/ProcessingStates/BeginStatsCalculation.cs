using EKG_Project.Architecture;
using EKG_Project.Modules;
using System;
using EKG_Project.GUI;
using System.Collections.Generic;
using EKG_Project.Architecture.GUIMessages;

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



        public void Process(Processing process, out IProcessingState timeoutState)
        {
            process.Stats.Init(ModulesComputed);
            process.Communication.SendProcessingEvent(new StatsCalculactionBegun());
            timeoutState = new NextStats();
        }
    }
}
