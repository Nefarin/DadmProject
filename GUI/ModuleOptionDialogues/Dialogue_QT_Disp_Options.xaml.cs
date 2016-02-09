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
using System.Windows.Navigation;
using System.Windows.Shapes;
using EKG_Project.Modules.QT_Disp;
using EKG_Project.Modules;



namespace EKG_Project.GUI.ModuleOptionDialogues
{
    /// <summary>
    /// Interaction logic for DialogueBox of QT_DISP Options
    /// </summary>


    public partial class Dialogue_QT_Disp_Options : Window
    {
        public ModuleParams returnParameters { get; set; }
        public ModuleParams PendingParameters { get; set; }
        ModulePanel panel;

        /// <summary>
        /// Set Options in QT_DISP DialogBox, set also parameters of window location
        /// </summary>
        /// <param name="parent">Parent is a ModulPanel object</param>
        /// <param name="parameters">Get parameters from Params Class, here it sets the name of analysis</param>
        public Dialogue_QT_Disp_Options(Object parent, QT_Disp_Params parameters)
        {
            panel = parent as ModulePanel;
            this.returnParameters = parameters;

            this.PendingParameters = new QT_Disp_Params(parameters.AnalysisName);
            this.PendingParameters.CopyFrom(parameters);
            this.DataContext = this.PendingParameters;
            InitializeComponent();
            this.Left = SystemParameters.PrimaryScreenWidth - this.Width - 400;
            this.Top = SystemParameters.PrimaryScreenHeight - this.Height - 350;
        }

        /// <summary>
        /// Apply changes in dialog box with options
        /// </summary>
        /// <param name="sender">Supports class in .NET, default param</param>
        /// <param name="e">Contains state information and event data associated with a routed event</param>
        private void ApplyParameterChanges(object sender, RoutedEventArgs e)
        {
            this.returnParameters.CopyFrom(this.PendingParameters);
            this.Close();
        }

        /// <summary>
        /// Simply close the window with parameters
        /// </summary>
        /// <param name="sender">Supports class in .NET, default param</param>
        /// <param name="e">Contains state information and event data associated with a routed event</param>
        private void RejectParameterChanges(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
