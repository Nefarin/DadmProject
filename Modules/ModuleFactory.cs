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
using EKG_Project.Modules.Heart_Cluster;
using EKG_Project.GUI;

namespace EKG_Project.Modules
{
    public class ModuleFactory
    {
        public static IModule NewModule(AvailableOptions option)
        {
            IModule module;
            switch(option)
            {
                case AvailableOptions.ECG_BASELINE:
                    module = new ECG_Baseline.ECG_Baseline();
                    break;
                case AvailableOptions.R_PEAKS:
                    module = new R_Peaks.R_Peaks();
                    break;
                case AvailableOptions.ATRIAL_FIBER:
                    module = new Atrial_Fibr.Atrial_Fibr();
                    break;
                case AvailableOptions.FLUTTER:
                    module = new Flutter.Flutter();
                    break;
                case AvailableOptions.HEART_AXIS:
                    module = new Heart_Axis.Heart_Axis();
                    break;
                case AvailableOptions.HEART_CLASS:
                    module = new Heart_Class.Heart_Class();
                    break;
                case AvailableOptions.HRT:
                    module = new HRT.HRT();
                    break;
                case AvailableOptions.HRV1:
                    module = new HRV1.HRV1();
                    break;
                case AvailableOptions.HRV2:
                    module = new HRV2.HRV2();
                    break;
                case AvailableOptions.HRV_DFA:
                    module = new HRV_DFA.HRV_DFA();
                    break;
                case AvailableOptions.QT_DISP:
                    module = new QT_Disp.QT_Disp();
                    break;
                case AvailableOptions.SIG_EDR:
                    module = new SIG_EDR.SIG_EDR();
                    break;
                case AvailableOptions.SLEEP_APNEA:
                    module = new Sleep_Apnea.Sleep_Apnea();
                    break;
                case AvailableOptions.ST_SEGMENT:
                    module = new ST_Segment.ST_Segment();
                    break;
                case AvailableOptions.TEST_MODULE:
                    module = new TestModule3.TestModule3();
                    break;
                case AvailableOptions.T_WAVE_ALT:
                    module = new T_Wave_Alt.T_Wave_Alt();
                    break;
                case AvailableOptions.WAVES:
                    module = new Waves.Waves();
                    break;
                case AvailableOptions.HEART_CLUSTER:
                    module = new Heart_Cluster.Heart_Cluster();
                    break;
                default:
                    module = new TestModule3.TestModule3();
                    break;
            }

            return module;

        }
    }
}
