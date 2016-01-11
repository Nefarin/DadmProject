using EKG_Project.Modules.R_Peaks;
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
    /// Interaction logic for Dialogue_R_Peaks_Options.xaml
    /// </summary>
    public partial class Dialogue_R_Peaks_Options : Window
    {


        private R_Peaks_Params returnParameters { get; set; }
        public R_Peaks_Params PendingParameters { get; set; }
        ModulePanel panel;

        public Dialogue_R_Peaks_Options(Object parent, R_Peaks_Params parameters)
        {
            panel = parent as ModulePanel;
            this.returnParameters = parameters;

            this.PendingParameters = new R_Peaks_Params(R_Peaks_Method.EMD, panel.AnalysisName);
            this.PendingParameters.CopyParametersFrom(parameters);
            this.DataContext = this.PendingParameters;
            InitializeComponent();
        }

        private void ApplyParameterChanges(object sender, RoutedEventArgs e)
        {
            this.returnParameters.CopyParametersFrom(this.PendingParameters);
            this.Close();
        }

        private void RejectParameterChanges(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
