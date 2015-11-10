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
using EKG_Project.Architecture;

namespace EKG_Project.GUI
{
    /// <summary>
    /// Interaction logic for AnalysisControl.xaml
    /// </summary>
    public partial class AnalysisControl : UserControl
    {
        private MainWindow _parent;
        private TabItem _parentTab;
        private ECG_Communication _communication;
        public AnalysisControl(ECG_Communication communication, MainWindow parent, TabItem parentTab)
        {
            _communication = communication;
            _communication.Analysis_To_GUI_Event += new Analysis_To_GUI(analysisEventHandler);
            _parent = parent;
            _parentTab = parentTab;
            InitializeComponent();
        }

        private void analysisEventHandler(object sender, Analysis_To_GUI_Item item)
        {
            switch (item.Command)
            {
                case Analysis_To_GUI_Command.ANALYSIS_ENDED:

                    break;
                case Analysis_To_GUI_Command.EXIT_ANALYSIS:
                    _parent.closeAnalysisTab(_parentTab);
                    break;
                default:
                    break;
            }
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            _communication.sendGUIMessage(new GUI_To_Analysis_Item(GUI_To_Analysis_Command.STOP_ANALYSIS, null));
        }
    }
}
