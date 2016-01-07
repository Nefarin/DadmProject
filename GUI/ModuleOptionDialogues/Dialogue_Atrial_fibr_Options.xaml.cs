using EKG_Project.Modules.Atrial_Fibr;
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
    /// Interaction logic for Dialogue_Atrial_fibr_Options.xaml
    /// </summary>
    public partial class Dialogue_Atrial_fibr_Options : Window
    {
        private Atrial_Fibr_Params returnParameters { get; set; }
        public Atrial_Fibr_Params PendingParameters { get; set; }
        ModulePanel panel;

        public Dialogue_Atrial_fibr_Options(Object parent, Atrial_Fibr_Params parameters)
        {
            panel = parent as ModulePanel;
            this.returnParameters = parameters;

            this.PendingParameters = new Atrial_Fibr_Params(Detect_Method.POINCARE);
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
