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
using EKG_Project.GUI;
namespace EKG_Project.Modules
{
    public class StatsFactory
    {
        public static IModuleStats New(AvailableOptions option)
        {
            IModuleStats stats;
            switch (option)
            {
                case AvailableOptions.ECG_BASELINE:
                    stats = new ECG_Baseline.ECG_Baseline_Stats();
                    break;
                case AvailableOptions.R_PEAKS:
                    stats = new R_Peaks.R_Peaks_Stats();
                    break;
                case AvailableOptions.ATRIAL_FIBER:
                    stats = new Atrial_Fibr.Atrial_Fibr_Stats();
                    break;
                case AvailableOptions.FLUTTER:
                    stats = new Flutter.Flutter_Stats();
                    break;
                case AvailableOptions.HEART_AXIS:
                    stats = new Heart_Axis.Heart_Axis_Stats();
                    break;
                case AvailableOptions.HEART_CLASS:
                    stats = new Heart_Class.Heart_Class_Stats();
                    break;
                case AvailableOptions.HRT:
                    stats = new HRT.HRT_Stats();
                    break;
                case AvailableOptions.HRV1:
                    stats = new HRV1.HRV1_Stats();
                    break;
                case AvailableOptions.HRV2:
                    stats = new HRV2.HRV2_Stats();
                    break;
                case AvailableOptions.HRV_DFA:
                    stats = new HRV_DFA.HRV_DFA_Stats();
                    break;
                case AvailableOptions.QT_DISP:
                    stats = new QT_Disp.QT_Disp_Stats();
                    break;
                case AvailableOptions.SIG_EDR:
                    stats = new SIG_EDR.SIG_EDR_Stats();
                    break;
                case AvailableOptions.SLEEP_APNEA:
                    stats = new Sleep_Apnea.Sleep_Apnea_Stats();
                    break;
                case AvailableOptions.ST_SEGMENT:
                    stats = new ST_Segment.ST_Segment_Stats();
                    break;
                case AvailableOptions.TEST_MODULE:
                    stats = new TestModule3.TestModule3_Stats();
                    break;
                case AvailableOptions.T_WAVE_ALT:
                    stats = new T_Wave_Alt.T_Wave_Alt_Stats();
                    break;
                case AvailableOptions.WAVES:
                    stats = new Waves.Waves_Stats();
                    break;
                default:
                    stats = new TestModule3.TestModule3_Stats();
                    break;
            }

            return stats;
        }
    }
}
