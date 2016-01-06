using EKG_Project.GUI.ModuleOptionDialogues;
using EKG_Project.Modules.ECG_Baseline;
using EKG_Project.Modules.R_Peaks;
using EKG_Project.Modules.Waves;
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

        public ModulePanel()
        {
            InitializeComponent();
            var ecgBaseline = new ModuleOption(AvailableOptions.ECG_BASELINE);

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

            var testModule = new ModuleOption(AvailableOptions.TEST_MODULE);
            Options.Add(ecgBaseline);
            Options.Add(testModule);
            this.treeViewModules.ItemsSource = this.Options;
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            ModuleOption option = (ModuleOption)((Button)sender).DataContext;
            Window dialogue = null;
            switch (option.Code)
            {
                case AvailableOptions.ECG_BASELINE:
                    dialogue = new Dialogue_ECG_Baseline_Options((ECG_Baseline_Params)option.ModuleParam);
                    dialogue.ShowDialog();
                    break;
                case AvailableOptions.R_PEAKS:
                    dialogue = new Dialogue_R_Peaks_Options((R_Peaks_Params)option.ModuleParam);
                    dialogue.ShowDialog();
                    break;
                case AvailableOptions.WAVES:
                    dialogue = new Dialogue_Waves_Options((Waves_Params)option.ModuleParam);
                    dialogue.ShowDialog();
                    break;
                default:
                    break;
            }      
        }
    }
}
