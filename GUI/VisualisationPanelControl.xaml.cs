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
using System.Data;

namespace EKG_Project.GUI
{
    /// <summary>
    /// Interaction logic for VisualisationPanelControl.xaml
    /// </summary>
    public partial class VisualisationPanelControl : UserControl
    {
        //private List<TabItem> visulisationTabsList;
        private List<TabItem> visulisationDataTabsList;

        public VisualisationPanelControl()
        {

            InitializeComponent();

            //VisualisationPlotControl ecgVPControl = new VisualisationPlotControl();
            //VisualisationTableControl ecgVTControl = new VisualisationTableControl();


            //visulisationTabsList = new List<TabItem>();

            //TabItem ecgBaselineTab = new TabItem();
            //ecgBaselineTab.Header = "ECGBaseline";
            //ecgBaselineTab.Content = ecgVPControl;
            //visulisationTabsList.Add(ecgBaselineTab);

            //TabItem r_peaksTab = new TabItem();
            //r_peaksTab.Header = "R_Peaks";
            //visulisationTabsList.Add(r_peaksTab);

            //TabItem addInfo = new TabItem();
            //addInfo.Header = "Read This";
            //addInfo.Content = "Tab będzie dodawany zgodnie z tym co po lewej stronie";
            //visulisationTabsList.Add(addInfo);

            //TabItem tableControl = new TabItem();
            //tableControl.Header = "Table";
            //tableControl.Content = ecgVTControl;
            //visulisationTabsList.Add(tableControl);

            //this.EcgDynamicTab.DataContext = visulisationTabsList;

            VisualisationDataControl ecgVDataControl = new VisualisationDataControl();

            visulisationDataTabsList = new List<TabItem>();

            TabItem ecgBaselineTab = new TabItem();
            ecgBaselineTab.Header = "ECGBaseline";
            ecgBaselineTab.Content = ecgVDataControl;
            visulisationDataTabsList.Add(ecgBaselineTab);

            //TabItem ecgBasicDataTab = new TabItem();
            //ecgBasicDataTab.Header = "ecgBasicData";
            //ecgBasicDataTab.Content = ecgVDataControl;
            //visulisationDataTabsList.Add(ecgBasicDataTab);

            //TabItem r_peaksTab = new TabItem();
            //r_peaksTab.Header = "R_Peaks";
            //r_peaksTab.Content = ecgVDataControl;
            //visulisationDataTabsList.Add(r_peaksTab);

            //TabItem addInfo = new TabItem();
            //addInfo.Header = "Info";
            //addInfo.Content = "W ten sposób będą dodawane pozostałe moduły";
            //visulisationDataTabsList.Add(addInfo);


            this.EcgDynamicTab.DataContext = visulisationDataTabsList;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.headerTable.ItemsSource = this.CreateHeaderInfoTable().DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public VisualisationPanelControl(List<string> tabNames)
        {
            InitializeComponent();
            ChooseTabDisplay(tabNames);
        }

        private void ChooseTabDisplay(List<string> tabNames)
        {
            visulisationDataTabsList = new List<TabItem>();
            foreach(string tabName in tabNames)
            {
                VisualisationDataControl ecgVDataControl = new VisualisationDataControl(tabName);
                TabItem tabItem = new TabItem();
                tabItem.Header = tabName;
                tabItem.Content = ecgVDataControl;
                visulisationDataTabsList.Add(tabItem);
            }

            this.EcgDynamicTab.DataContext = visulisationDataTabsList;
        }

        public DataTable CreateHeaderInfoTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("Fs", Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn("Samples", Type.GetType("System.Int32")));
            table.Rows.Add(25, 2000);

            return table;
        }



    }
}
