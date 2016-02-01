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
using EKG_Project.GUI;

using System;

using System.Collections.Generic;


// TO DO
namespace EKG_Project.Architecture
{
    public class Stats
    {
        private Dictionary<AvailableOptions, bool> _isComputed;
        private Dictionary<AvailableOptions, Dictionary<String, String>> _results;

        public static AvailableOptions[] MODULE_ORDER =
        {
            AvailableOptions.ECG_BASELINE,
            AvailableOptions.R_PEAKS,
            //AvailableOptions.HRV1,
            AvailableOptions.HRV2,
            AvailableOptions.HRV_DFA,
            AvailableOptions.WAVES,
            AvailableOptions.FLUTTER,
            //AvailableOptions.SIG_EDR,
            //AvailableOptions.ST_SEGMENT,
            AvailableOptions.SLEEP_APNEA,
            AvailableOptions.ATRIAL_FIBER,
            AvailableOptions.QT_DISP,
            AvailableOptions.FLUTTER,
            //AvailableOptions.HRT,
            AvailableOptions.HEART_CLASS,
            AvailableOptions.HEART_AXIS,
            //AvailableOptions.TEST_MODULE
        };

        private string _analysisName;
        private int _currentModuleIndex;
        private int _currentModuleProcessed;
        private AvailableOptions _currentOption;
        IModuleStats _currentStats;

        public Stats(string analysisName)
        {
            AnalysisName = analysisName;
        }

        public void Init(Dictionary<AvailableOptions, bool> moduleComputed)
        {
            _isComputed = moduleComputed;
            _results = new Dictionary<AvailableOptions, Dictionary<string, string>>();
            CurrentModuleIndex = -1;
            CurrentModuleProcessed = 0;
        }

        public void initNextStats()
        {
            try
            {
                CurrentStats = nextStats();
                CurrentOption = MODULE_ORDER[CurrentModuleIndex];
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private IModuleStats nextStats()
        {
            CurrentModuleIndex++;
            AvailableOptions option = MODULE_ORDER[CurrentModuleIndex];
            try
            {
                bool computed;
                try
                {
                    computed = _isComputed[option];
                }
                catch (Exception e)
                {
                    computed = false;
                }

                if (computed == true)
                {
                    IModuleStats stats = StatsFactory.New(option);
                    stats.Init(AnalysisName);
                    return stats;
                }
                else
                {
                    throw new Exception();
                }

            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public int Amount()
        {
            return MODULE_ORDER.Length;
        }


        public int CurrentModuleIndex
        {
            get
            {
                return _currentModuleIndex;
            }

            set
            {
                _currentModuleIndex = value;
            }
        }



        public AvailableOptions CurrentOption
        {
            get
            {
                return _currentOption;
            }

            set
            {
                _currentOption = value;
            }
        }


        public string AnalysisName
        {
            get
            {
                return _analysisName;
            }

            set
            {
                _analysisName = value;
            }
        }

        public int CurrentModuleProcessed
        {
            get
            {
                return _currentModuleProcessed;
            }

            set
            {
                _currentModuleProcessed = value;
            }
        }

        public IModuleStats CurrentStats
        {
            get
            {
                return _currentStats;
            }

            set
            {
                _currentStats = value;
            }
        }

        public Dictionary<AvailableOptions, Dictionary<string, string>> Results
        {
            get
            {
                return _results;
            }

            set
            {
                _results = value;
            }
        }
    }
}
