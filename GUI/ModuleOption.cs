using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.GUI
{
    public class ModuleOption : INotifyPropertyChanged
    {
        private bool _set = false;
        private List<ModuleOption> _suboptions = null;
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

        public event PropertyChangedEventHandler PropertyChanged;

        public ModuleOption(AvailableOptions code, ModuleOption parent = null)
        {
            this.Code = code;
            this.Name = code.ToString();
            this.Set = false;
            this.Parent = parent;
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
        HEART_AXIS
    }
}
