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
    public class Modules
    {
        private Dictionary<AvailableOptions, bool> _isComputed;

        public static AvailableOptions[] MODULE_ORDER =
        {
            AvailableOptions.ECG_BASELINE,
            AvailableOptions.R_PEAKS,
            AvailableOptions.HRV1,
            AvailableOptions.HRV2,
            AvailableOptions.HRV_DFA,
            AvailableOptions.WAVES,
            AvailableOptions.FLUTTER,
            //AvailableOptions.SIG_EDR,
            AvailableOptions.ST_SEGMENT,
            AvailableOptions.SLEEP_APNEA,
            AvailableOptions.ATRIAL_FIBER,
            AvailableOptions.QT_DISP,
            AvailableOptions.FLUTTER,
            AvailableOptions.T_WAVE_ALT,
            AvailableOptions.HRT,
            AvailableOptions.HEART_CLASS,
            AvailableOptions.HEART_AXIS,
            //AvailableOptions.TEST_MODULE
        };

        Dictionary<AvailableOptions, ModuleParams> _moduleParams;
        private string _analysisName;
        private int _currentModuleIndex;
        private int _currentModuleProcessed;
        private AvailableOptions _currentOption;
        private bool _fileLoaded = false;
        IModule _currentModule;

        public Modules(string analysisName)
        {
            AnalysisName = analysisName;
        }

        public void Init(Dictionary<AvailableOptions, ModuleParams> moduleParams)
        {
            _isComputed = new Dictionary<AvailableOptions, bool>();
            CurrentModuleIndex = -1;
            CurrentModuleProcessed = 0;
            ModuleParams = new Dictionary<AvailableOptions, ModuleParams>(moduleParams);
        }

        public void initNextModule()
        {
            try
            {
                CurrentModule = nextModule();
                CurrentOption = MODULE_ORDER[CurrentModuleIndex];
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private IModule nextModule()
        {
            CurrentModuleIndex++;
            AvailableOptions option = MODULE_ORDER[CurrentModuleIndex];
            //Console.WriteLine(option);
            ModuleParams param;
            try
            {
                param = ModuleParams[option];
                IModule module = ModuleFactory.NewModule(option);
                module.Init(param);
                return module;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public int Amount()
        {
            return ModuleParams.Count;
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

        public IModule CurrentModule
        {
            get
            {
                return _currentModule;
            }

            set
            {
                _currentModule = value;
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

        public bool FileLoaded
        {
            get
            {
                return _fileLoaded;
            }

            set
            {
                _fileLoaded = value;
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
    }
}
