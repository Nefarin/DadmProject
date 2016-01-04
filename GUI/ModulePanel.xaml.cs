using EKG_Project.GUI.ModuleOptionDialogues;
using EKG_Project.Modules.ECG_Baseline;
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
            var ecgBaseline = new ModuleOption(AvailableOptions.ECG_BASELINE, null);

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
                            AddSuboption(AvailableOptions.ECTOPIC_BEAT).
                            AddSuboptionAndMoveUp(AvailableOptions.HEART_AXIS).
                        AddSuboption(AvailableOptions.ATRIAL_FIBER).
                        AddSuboption(AvailableOptions.QT_DISP).
                        AddSuboptionAndMoveUp(AvailableOptions.FLUTTER).
                    AddSuboption(AvailableOptions.HRV_DFA).
                    AddSuboptionAndMoveUp(AvailableOptions.SIG_EDR).
                AddSuboption(AvailableOptions.VCG_T_LOOP);

            var testModule = new ModuleOption(AvailableOptions.TEST_MODULE, null);
            Options.Add(ecgBaseline);
            Options.Add(testModule);
            this.treeViewModules.ItemsSource = this.Options;
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            ModuleOption option = (ModuleOption)((Button)sender).DataContext;
            switch (option.Code)
            {
                case AvailableOptions.ECG_BASELINE:
                    var dialogue = new Dialogue_ECG_Baseline_Options((ECG_Baseline_Params)option.ModuleParam);
                    dialogue.ShowDialog();
                    break;
                case AvailableOptions.R_PEAKS:
                    break;
                default:
                    break;
            }      
        }
    }
}
