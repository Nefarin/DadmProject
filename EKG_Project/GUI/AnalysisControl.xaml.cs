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
using System.Threading;
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
        private SynchronizationContext _context;
        public Analysis_To_GUI _analysisEvent;
        public AnalysisControl(ECG_Communication communication, MainWindow parent, TabItem parentTab, SynchronizationContext context)
        {
            _context = context;
            _communication = communication;
            _analysisEvent = new Analysis_To_GUI(analysisEventHandler);
            _communication.Analysis_To_GUI_Event += _analysisEvent;
            _parent = parent;
            _parentTab = parentTab;
            InitializeComponent();
        }

        private void analysisEventHandler(object sender, Analysis_To_GUI_Item item)
        {
            _context.Post(new SendOrPostCallback((o) => {
                analyzeEvent(item);
            }), null);

        }

        private void analyzeEvent(Analysis_To_GUI_Item item)
        {
            switch (item.Command)
            {
                case Analysis_To_GUI_Command.ANALYSIS_ENDED:

                    break;
                case Analysis_To_GUI_Command.EXIT_ANALYSIS:
                    _communication.Analysis_To_GUI_Event -= _analysisEvent;
                    _parent.closeAnalysisTab(_parentTab);
                    break;
                case Analysis_To_GUI_Command.TEST:
                    int value = Convert.ToInt32(item.Data);
                    progressBar.Value = value;
                    break;
                default:
                    break;
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            _communication.sendGUIMessage(new GUI_To_Analysis_Item(GUI_To_Analysis_Command.ADD_TEST, null));
        }

        private void subButton_Click(object sender, RoutedEventArgs e)
        {
            _communication.sendGUIMessage(new GUI_To_Analysis_Item(GUI_To_Analysis_Command.SUB_TEST, null));
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            _communication.sendGUIMessage(new GUI_To_Analysis_Item(GUI_To_Analysis_Command.STOP_ANALYSIS, null));
        }
    }
}
