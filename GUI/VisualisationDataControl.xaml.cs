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
    /// Interaction logic for VisualisationDataControl.xaml
    /// </summary>
    public partial class VisualisationDataControl : UserControl
    {
        private List<TabItem> visulisationDataTabsList;

        public VisualisationDataControl()
        {
            InitializeComponent();

            VisualisationPlotControl ecgVPControl = new VisualisationPlotControl();
            VisualisationTableControl ecgVTControl = new VisualisationTableControl();

            visulisationDataTabsList = new List<TabItem>();

            TabItem ecgBaselineTab = new TabItem();
            ecgBaselineTab.Header = "Plot";
            ecgBaselineTab.Content = ecgVPControl;
            visulisationDataTabsList.Add(ecgBaselineTab);

            TabItem tableControl = new TabItem();
            tableControl.Header = "Table";
            tableControl.Content = ecgVTControl;
            visulisationDataTabsList.Add(tableControl);

            this.EcgDataDynamicTab.DataContext = visulisationDataTabsList;
        }
    }
}
