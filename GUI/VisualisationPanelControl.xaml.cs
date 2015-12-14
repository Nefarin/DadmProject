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

namespace EKG_Project.GUI
{
    /// <summary>
    /// Interaction logic for VisualisationPanelControl.xaml
    /// </summary>
    public partial class VisualisationPanelControl : UserControl
    {
        private List<TabItem> visulisationTabsList;
        


        public VisualisationPanelControl()
        {
            InitializeComponent();

            VisualisationPlotControl ecgVPControl = new VisualisationPlotControl();

            visulisationTabsList = new List<TabItem>();

            TabItem ecgBaselineTab = new TabItem();
            ecgBaselineTab.Header = "ECGBaseline";
            ecgBaselineTab.Content = ecgVPControl;
            visulisationTabsList.Add(ecgBaselineTab);

            TabItem r_peaksTab = new TabItem();
            r_peaksTab.Header = "R_Peaks";
            visulisationTabsList.Add(r_peaksTab);

            TabItem addInfo = new TabItem();
            addInfo.Header = "Read This";
            addInfo.Content = "Tab będzie dodawany zgodnie z tym co po lewej stronie";
            visulisationTabsList.Add(addInfo);

            this.EcgDynamicTab.DataContext = visulisationTabsList;
        }
    }
}
