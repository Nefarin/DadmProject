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

namespace EKG_Project.GUI
{
    public class ModuleOption : INotifyPropertyChanged
    {
        #region Private fields

        private bool _set = false;
        private List<ModuleOption> _suboptions = null;
        private String _analysisName;

        #endregion

        #region Properties

        public string Name { get; set; }
        public AvailableOptions Code { get; set; }
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
                    if (this.Parent != null)
                        this.Parent.Set = true;
                }
                else
                {
                    foreach (ModuleOption option in this.Suboptions)
                        option.Set = false;
                }
                this.OnPropertyChanged("Set");
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
        public bool ParametersAvailable { get { return this.ModuleParam != null; } }

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

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public ModuleOption(AvailableOptions code, ModuleOption parent = null)
        {
            this.Code = code;
            this.Name = code.ToString();
            this.Set = false;
            this.Parent = parent;

            switch (this.Code)
            {
                case AvailableOptions.ECG_BASELINE:
                    this.ModuleParam = new ECG_Baseline_Params(Filtr_Method.BUTTERWORTH, Filtr_Type.HIGHPASS);            
                    break;
                case AvailableOptions.R_PEAKS:
                    this.ModuleParam = new R_Peaks_Params(R_Peaks_Method.EMD, this.getAnalysisName());
                    break;
                case AvailableOptions.WAVES:
                    this.ModuleParam = new Waves_Params();
                    break;
                case AvailableOptions.ATRIAL_FIBER:
                    this.ModuleParam = new Atrial_Fibr_Params(Detect_Method.POINCARE);
                    break;
                default:
                    this.ModuleParam = null;
                    break;
            }        
        }

        #endregion

        #region Methods

        private String getAnalysisName()
        {
            if (this.Parent == null)
            {
                return this.AnalysisName;
            }
            else
            {
                return Parent.getAnalysisName();
            }
        }

        public ModuleOption AddSuboption(AvailableOptions code)
        {
            this.Suboptions.Add(new ModuleOption(code, this));
            return this;
        }
        public ModuleOption AddSuboptionAndMoveDown(AvailableOptions code)
        {
            var suboption = new ModuleOption(code, this);
            this.Suboptions.Add(suboption);
            return suboption;
        }
        public ModuleOption AddSuboptionAndMoveUp(AvailableOptions code)
        {
            this.Suboptions.Add(new ModuleOption(code, this));
            return this.Parent;
        }

        void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }

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
        TEST_MODULE
    }
}
