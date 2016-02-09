using EKG_Project.Modules;
using EKG_Project.GUI;
using System;
using System.Collections.Generic;

namespace EKG_Project.Architecture
{
    #region Documentation
    /// <summary>
    /// Class responsible for handling ECG processing modules logic.
    /// </summary>
    #endregion
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
            //AvailableOptions.ST_SEGMENT,
            AvailableOptions.SLEEP_APNEA,
            AvailableOptions.ATRIAL_FIBER,
            AvailableOptions.QT_DISP,
            AvailableOptions.FLUTTER,
            AvailableOptions.T_WAVE_ALT,
            AvailableOptions.HEART_CLASS,
            AvailableOptions.HRT,
            AvailableOptions.HEART_AXIS,
            AvailableOptions.HEART_CLUSTER,
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
