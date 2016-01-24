using EKG_Project.Modules;
using EKG_Project.Modules.Waves;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace EKG_Project.GUI.ModuleOptionDialogues
{
    /// <summary>
    /// Interaction logic for Dialogue_Waves_Options.xaml
    /// </summary>
    public partial class Dialogue_Waves_Options : Window
    {
        public ModuleParams returnParameters { get; set; }
        public ModuleParams PendingParameters { get; set; }
        ModulePanel panel;

        public Dialogue_Waves_Options(Object parent, Waves_Params parameters)
        {
            panel= parent as ModulePanel;
            this.returnParameters = parameters;

            this.PendingParameters = new Waves_Params(parameters.AnalysisName);
            this.PendingParameters.CopyFrom(parameters);
            this.DataContext = this.PendingParameters;
            InitializeComponent();
        }

        private void ApplyParameterChanges(object sender, RoutedEventArgs e)
        {
            this.returnParameters.CopyFrom(this.PendingParameters);
            this.Close();
        }

        private void RejectParameterChanges(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
