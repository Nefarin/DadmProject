using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.Modules;
using EKG_Project.Modules.ECG_Baseline;
using EKG_Project.Modules.R_Peaks;
using EKG_Project.Modules.Waves;
using EKG_Project.Modules.Atrial_Fibr;
using EKG_Project.Modules.Heart_Class;
using EKG_Project.Modules.Heart_Axis;
using EKG_Project.Modules.Sleep_Apnea;
using EKG_Project.Modules.HRV1;
using EKG_Project.Modules.HRV2;
using EKG_Project.Modules.QT_Disp;
using EKG_Project.Modules.Flutter;
using EKG_Project.Modules.HRV_DFA;
using EKG_Project.Modules.ST_Segment;
using EKG_Project.Modules.T_Wave_Alt;
using EKG_Project.Modules.SIG_EDR;
using EKG_Project.Modules.HRT;
using EKG_Project.Modules.Heart_Cluster;

namespace EKG_Project.GUI
{
    /// <summary>
    /// Handle treeView,
    /// set contructors of Params Classes,
    /// Set analysis name in all options,
    /// Set options dialogBox where needed.
    /// </summary>
    public class ModuleOption : INotifyPropertyChanged
    {
        #region Private fields

        private bool _set = false;
        private List<ModuleOption> _suboptions = null;

        #endregion

        #region Properties

        public string Name { get; set; }
        public AvailableOptions Code { get; set; }

        /// <summary>
        /// Set an analysis name in every module, show options dialog box in some cases (when chosen and needed)
        /// </summary>
        public bool Set
        {
            get
            {
                return this._set;
            }
            set
            {
                this._set = value;
                if (this._set)
                {
                    switch (this.Code)
                    {
                        case AvailableOptions.ECG_BASELINE:
                            if (this.ModuleParam == null)
                            {
                                this.ModuleParam = new ECG_Baseline_Params();
                                this.ModuleParam.GUIParametersAvailable = true;
                                FillDictionaries();
                            }
                            break;
                        case AvailableOptions.R_PEAKS:
                            if (this.ModuleParam == null)
                            {
                                this.ModuleParam = new R_Peaks_Params(this.AnalysisName);
                                this.ModuleParam.GUIParametersAvailable = true;
                                FillDictionaries();
                            }
                            break;
                        case AvailableOptions.WAVES:
                            if (this.ModuleParam == null)
                            {
                                this.ModuleParam = new Waves_Params(this.AnalysisName);
                                this.ModuleParam.GUIParametersAvailable = true;
                                FillDictionaries();
                            }
                            break;
                        case AvailableOptions.ATRIAL_FIBER:
                            if (this.ModuleParam == null)
                            {
                                this.ModuleParam = new Atrial_Fibr_Params(this.AnalysisName);
                                this.ModuleParam.GUIParametersAvailable = true;
                                FillDictionaries();
                            }
                            break;
                        case AvailableOptions.HEART_CLASS:
                            this.ModuleParam = new Heart_Class_Params(this.AnalysisName);
                            this.ModuleParam.GUIParametersAvailable = false;
                            FillDictionaries();
                            break;
                        case AvailableOptions.HEART_AXIS:
                            this.ModuleParam = new Heart_Axis_Params(this.AnalysisName);
                            this.ModuleParam.GUIParametersAvailable = false;
                            FillDictionaries();
                            break;
                        case AvailableOptions.SLEEP_APNEA:
                            this.ModuleParam = new Sleep_Apnea_Params(this.AnalysisName);
                            this.ModuleParam.GUIParametersAvailable = false;
                            FillDictionaries();
                            break;
                        case AvailableOptions.HRV2:
                            this.ModuleParam = new HRV2_Params(this.AnalysisName);
                            this.ModuleParam.GUIParametersAvailable = false;
                            FillDictionaries();
                            break;
                        case AvailableOptions.QT_DISP:
                            if (this.ModuleParam == null)
                            {
                                this.ModuleParam = new QT_Disp_Params(this.AnalysisName);
                                this.ModuleParam.GUIParametersAvailable = true;
                                FillDictionaries();
                            }
                            break;
                        case AvailableOptions.FLUTTER:
                            this.ModuleParam = new Flutter_Params(this.AnalysisName);
                            this.ModuleParam.GUIParametersAvailable = false;
                            FillDictionaries();
                            break;
                        case AvailableOptions.HRV_DFA:
                            this.ModuleParam = new HRV_DFA_Params(this.AnalysisName);
                            this.ModuleParam.GUIParametersAvailable = false;
                            FillDictionaries();
                            break;
                        case AvailableOptions.HEART_CLUSTER:
                            this.ModuleParam = new Heart_Cluster_Params(this.AnalysisName);
                            this.ModuleParam.GUIParametersAvailable = false;
                            FillDictionaries();
                            break;
                        /*
                        case AvailableOptions.TEST_MODULE:
                            this.ModuleParam = new TestModule_Params(500);
                            this.ModuleParam.GUIParametersAvailable = true;
                            FillDictionaries();
                            break;
                        */
                        case AvailableOptions.HRV1:
                            this.ModuleParam = new HRV1_Params(this.AnalysisName);
                            this.ModuleParam.GUIParametersAvailable = false;
                            FillDictionaries();
                            break;
                        /*case AvailableOptions.ST_SEGMENT:
                            this.ModuleParam = new ST_Segment_Params(this.AnalysisName);
                            this.ModuleParam.GUIParametersAvailable = false;
                            FillDictionaries();
                            break;
                            */
                        case AvailableOptions.T_WAVE_ALT:
                            this.ModuleParam = new T_Wave_Alt_Params(this.AnalysisName);
                            this.ModuleParam.GUIParametersAvailable = false;
                            FillDictionaries();
                            break;
                        /*case AvailableOptions.SIG_EDR:
                            this.ModuleParam = new SIG_EDR_Params(this.AnalysisName);
                            this.ModuleParam.GUIParametersAvailable = false;
                            FillDictionaries();
                            break;
                            */
                        case AvailableOptions.HRT:
                            this.ModuleParam = new HRT_Params(this.AnalysisName);
                            this.ModuleParam.GUIParametersAvailable = false;
                            FillDictionaries();
                            break;
                        default:
                            this.ModuleParam = null;
                            break;
                    }

                    if (this.ModuleParam != null)
                    {
                        this.Panel.OptionParams[this] = this.ModuleParam;
                        this.Panel.Params[this.Code] = this.ModuleParam;
                        this.ModuleParam.AnalysisName = this.AnalysisName;
                    }

                    if (this.Parent != null)
                        this.Parent.Set = true;
                }
                else
                {
                    foreach (ModuleOption option in this.Suboptions)
                        option.Set = false;

                    this.ModuleParam = null;

                    if (this.Panel != null)
                    {
                        this.Panel.OptionParams.Remove(this);
                        this.Panel.Params.Remove(this.Code);
                    }
                }

                this.OnPropertyChanged("Set");
                this.OnPropertyChanged("ParametersAvailable");
            }
        }

        public ModuleOption Parent { get; set; }

        public List<ModuleOption> Suboptions
        {
            get
            {
                if (this._suboptions == null)
                    this._suboptions = new List<ModuleOption>();
                return this._suboptions;
            }
        }

        public ModuleParams ModuleParam { get; set; }
        public ModulePanel Panel;

        /// <summary>
        /// Method which shows if parameters are available in chosen module
        /// </summary>
        public bool ParametersAvailable
        {
            get
            {
                return this.ModuleParam != null ? this.ModuleParam.GUIParametersAvailable : false;
            }
        }

        /// <summary>
        /// Get analysis name when set
        /// </summary>
        public string AnalysisName
        {
            get
            {
                return this.Panel != null ? this.Panel.AnalysisName : string.Empty;
            }
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Set current parameters od module options
        /// </summary>
        /// <param name="code">Analysis name</param>
        /// <param name="panel">ModuleOptions object - panel</param>
        /// <param name="parent">ModuleOption object = parent(default set to null)</param>
        public ModuleOption(AvailableOptions code, ModulePanel panel, ModuleOption parent = null)
        {
            this.Code = code;
            this.Name = code.ToString();
            this.Set = false;
            this.Parent = parent;
            this.Panel = panel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Method which returs an analysis name
        /// </summary>
        /// <returns>AReturn analysis name</returns>
        private String getAnalysisName()
        {
            return this.AnalysisName;
        }

        /// <summary>
        /// Adds suboption (submodule) in module tree
        /// </summary>
        /// <param name="code">Analysis name</param>
        /// <returns>Returns suboption</returns>
        public ModuleOption AddSuboption(AvailableOptions code)
        {
            this.Suboptions.Add(new ModuleOption(code, this.Panel, this));
            return this;
        }

        /// <summary>
        /// Adds suboption (submodule) and moves one level down (create a child branch)
        /// </summary>
        /// <param name="code">Analysis name</param>
        /// <returns>Returns suboption</returns>
        public ModuleOption AddSuboptionAndMoveDown(AvailableOptions code)
        {
            var suboption = new ModuleOption(code, this.Panel, this);
            this.Suboptions.Add(suboption);
            return suboption;
        }

        /// <summary>
        /// Adds suboption (submodule) and moves one level up (return to parent branch)
        /// </summary>
        /// <param name="code">Analysis name</param>
        /// <returns>Returns suboption</returns>
        public ModuleOption AddSuboptionAndMoveUp(AvailableOptions code)
        {
            this.Suboptions.Add(new ModuleOption(code, this.Panel, this));
            return this.Parent;
        }

        /// <summary>
        /// Handles property changed events
        /// </summary>
        /// <param name="propertyName">Which property was changed</param>
        void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Fills dictionares with codes and options of modules
        /// </summary>
        public void FillDictionaries()
        {
            this.Panel.Params[this.Code] = this.ModuleParam;
            this.Panel.OptionParams[this] = this.ModuleParam;
        }


        #endregion
    }

    /// <summary>
    /// Shown all available options (modules) as enums
    /// </summary>
    public enum AvailableOptions
    {
        ECG_BASELINE,
        R_PEAKS,
        VCG_T_LOOP,
        HRV1,
        HRV2,
        WAVES,
        HRV_DFA,
        SIG_EDR,
        ST_SEGMENT,
        T_WAVE_ALT,
        SLEEP_APNEA,
        HEART_CLASS,
        ATRIAL_FIBER,
        QT_DISP,
        FLUTTER,
        HRT,
        ECTOPIC_BEAT,
        HEART_AXIS,
        HEART_CLUSTER,
        TEST_MODULE
    }
}
