using EKG_Project.GUI.ModuleOptionDialogues;
using EKG_Project.Modules.Atrial_Fibr;
using EKG_Project.Modules.ECG_Baseline;
using EKG_Project.Modules.R_Peaks;
using EKG_Project.Modules.Waves;
using EKG_Project.Modules.QT_Disp;
using EKG_Project.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EKG_Project.GUI
{
    /// <summary>
    /// Interaction logic for ModulePanel.xaml
    /// </summary>
    public partial class ModulePanel : UserControl
    {
        public List<ModuleOption> Options = new List<ModuleOption>();
        public Dictionary<AvailableOptions, ModuleParams> Params = new Dictionary<AvailableOptions, ModuleParams>();
        public Dictionary<ModuleOption, ModuleParams> OptionParams = new Dictionary<ModuleOption, ModuleParams>();

        private string _analysisName;

        public ModulePanel()
        {
            InitializeComponent();
            var ecgBaseline = new ModuleOption(AvailableOptions.ECG_BASELINE, this);

            ecgBaseline.
                AddSuboptionAndMoveDown(AvailableOptions.R_PEAKS).
                    AddSuboption(AvailableOptions.HRV1).
                    AddSuboption(AvailableOptions.HRV2).
                    AddSuboptionAndMoveDown(AvailableOptions.WAVES).
                        AddSuboption(AvailableOptions.ST_SEGMENT).
                        AddSuboption(AvailableOptions.T_WAVE_ALT).
                        AddSuboption(AvailableOptions.SLEEP_APNEA).
                        AddSuboptionAndMoveDown(AvailableOptions.HEART_CLASS).
                            AddSuboption(AvailableOptions.HRT).
                            AddSuboptionAndMoveUp(AvailableOptions.HEART_AXIS).
                        AddSuboption(AvailableOptions.ATRIAL_FIBER).
                        AddSuboption(AvailableOptions.QT_DISP).
                        AddSuboptionAndMoveUp(AvailableOptions.FLUTTER).
                    AddSuboption(AvailableOptions.HRV_DFA).
                    AddSuboptionAndMoveUp(AvailableOptions.SIG_EDR);

            var testModule = new ModuleOption(AvailableOptions.TEST_MODULE, this);
            Options.Add(ecgBaseline);
            Options.Add(testModule);
            this.treeViewModules.ItemsSource = this.Options;
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

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            ModuleOption option = (ModuleOption)((Button)sender).DataContext;
            switch (option.Code)
            {
                case AvailableOptions.ECG_BASELINE:
                        var baseline_dialogue = new Dialogue_ECG_Baseline_Options(this, (ECG_Baseline_Params)option.ModuleParam);
                        baseline_dialogue.ShowDialog();
                        Params[option.Code] = baseline_dialogue.returnParameters;
                        OptionParams[option] = baseline_dialogue.returnParameters;
                    break;
                case AvailableOptions.R_PEAKS:
                        var peaks_dialogue = new Dialogue_R_Peaks_Options(this, (R_Peaks_Params)option.ModuleParam);
                        peaks_dialogue.ShowDialog();
                        Params[option.Code] = peaks_dialogue.returnParameters;
                        OptionParams[option] = peaks_dialogue.returnParameters;
                    break;
                case AvailableOptions.WAVES:
                        var waves_dialogue = new Dialogue_Waves_Options(this, (Waves_Params)option.ModuleParam);
                        waves_dialogue.ShowDialog();
                        Params[option.Code] = waves_dialogue.returnParameters;
                        OptionParams[option] = waves_dialogue.returnParameters;
                    break;
                case AvailableOptions.ATRIAL_FIBER:
                        var atrial_dialogue = new Dialogue_Atrial_fibr_Options(this, (Atrial_Fibr_Params)option.ModuleParam);
                        atrial_dialogue.ShowDialog();
                        Params[option.Code] = atrial_dialogue.returnParameters;
                        OptionParams[option] = atrial_dialogue.returnParameters;
                    break;
                case AvailableOptions.QT_DISP:
                        var qt_dialogue = new Dialogue_QT_Disp_Options(this, (QT_Disp_Params)option.ModuleParam);
                        qt_dialogue.ShowDialog();
                        Params[option.Code] = qt_dialogue.returnParameters;
                        OptionParams[option] = qt_dialogue.returnParameters;
                    break;
                default:
                    break;
            }
        }


        public List<ModuleOption> getAllOptions()
        {
            List<ModuleOption> AllOptions = new List<ModuleOption>();

            foreach (var option in Options)
            {
                AllOptions = getSuboptions(option, AllOptions);

            }

            return AllOptions;
        }

        public List<ModuleOption> getSuboptions(ModuleOption option, List<ModuleOption> currentOptions)
        {
            currentOptions.Add(option);

            if (option.Suboptions.Any())
            {
                foreach (var suboption in option.Suboptions)
                {
                    currentOptions = getSuboptions(suboption, currentOptions);
                }
            }
            return currentOptions;
        }

        public Tuple<ModuleOption, ModuleParams> ModuleOptionAndParams(AvailableOptions code)
        {

            foreach (KeyValuePair<ModuleOption, ModuleParams> entry in OptionParams)
            {
                if (entry.Key.Code == code && entry.Key.Set == true)
                {
                    Tuple<ModuleOption, ModuleParams> correctOptionParams = new Tuple<ModuleOption, ModuleParams>(entry.Key, entry.Value);

                        return correctOptionParams;
                }

            }

            return null;
                
        }


    }

}
