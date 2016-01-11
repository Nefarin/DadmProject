using EKG_Project.Modules;
using EKG_Project.Modules.Atrial_Fibr;
using EKG_Project.Modules.ECG_Baseline;
using EKG_Project.Modules.Flutter;
using EKG_Project.Modules.Heart_Axis;
using EKG_Project.Modules.Heart_Class;
using EKG_Project.Modules.HRT;
using EKG_Project.Modules.HRV1;
using EKG_Project.Modules.HRV2;
using EKG_Project.Modules.HRV_DFA;
using EKG_Project.Modules.QT_Disp;
using EKG_Project.Modules.R_Peaks;
using EKG_Project.Modules.SIG_EDR;
using EKG_Project.Modules.Sleep_Apnea;
using EKG_Project.Modules.ST_Segment;
using EKG_Project.Modules.T_Wave_Alt;
using EKG_Project.Modules.Waves;

using System.Collections.Generic;


// TO DO
namespace EKG_Project.Architecture
{
    public class Modules
    {
        private List<IModule> ecgModules;
        private int currentAnalysisIndex;

        public Modules()
        {

        }

        public void Init()
        {
            CurrentAnalysisIndex = -1;
        }

        public int Amount()
        {
            return ecgModules.Count;
        }

        public List<IModule> EcgModules
        {
            get
            {
                return ecgModules;
            }

            set
            {
                ecgModules = value;
            }
        }

        public int CurrentAnalysisIndex
        {
            get
            {
                return currentAnalysisIndex;
            }

            set
            {
                currentAnalysisIndex = value;
            }
        }
    }
}
