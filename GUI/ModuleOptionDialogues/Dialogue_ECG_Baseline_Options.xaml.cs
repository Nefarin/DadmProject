using EKG_Project.Modules;
using EKG_Project.Modules.ECG_Baseline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for Dialogue_ECG_Baseline_Options.xaml
    /// </summary>
    public partial class Dialogue_ECG_Baseline_Options : Window
    {
        public ModuleParams returnParameters { get; set; }
        public ModuleParams PendingParameters { get; set; }
        ModulePanel panel;

        public static CultureInfo CurrentCulture
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture;
            }
        }

        public Dialogue_ECG_Baseline_Options(Object parent, ECG_Baseline_Params parameters)
        {
            panel = parent as ModulePanel;
            this.returnParameters = parameters;

            this.PendingParameters = new ECG_Baseline_Params();
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
