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

        //0 - plot
        //1 - plot and table
        //2 - plot and histogram
        //3 - plot and table and histogram 
        private Dictionary<string, uint> modulesVisualisationNeeds = new Dictionary<string, uint>()
        {
            {"ecgBaseline", 1 },
            {"ecgBasic", 0 }
        };


        public VisualisationDataControl()
        {
            InitializeComponent();

            VisualisationPlotControl ecgVPControl = new VisualisationPlotControl();
            VisualisationTableControl ecgVTControl = new VisualisationTableControl();
            VisualisationHistogramControl ecgVHControl = new VisualisationHistogramControl();

            visulisationDataTabsList = new List<TabItem>();

            TabItem ecgBaselineTab = new TabItem();
            ecgBaselineTab.Header = "Plot";
            ecgBaselineTab.Content = ecgVPControl;
            visulisationDataTabsList.Add(ecgBaselineTab);

            TabItem tableControl = new TabItem();
            tableControl.Header = "Table";
            tableControl.Content = ecgVTControl;
            visulisationDataTabsList.Add(tableControl);

            TabItem histogramControl = new TabItem();
            histogramControl.Header = "Histogram";
            histogramControl.Content = ecgVHControl;
            visulisationDataTabsList.Add(histogramControl);

            this.EcgDataDynamicTab.DataContext = visulisationDataTabsList;
        }

        public VisualisationDataControl(string moduleName)
        {
            InitializeComponent();
            visulisationDataTabsList = new List<TabItem>();
            

            //if needed and what where needed? 
            switch (modulesVisualisationNeeds[moduleName])
            {
                case 0:                  
                    StartPlot();
                    break;
                    
                case 1:                    
                    StartPlot();
                    StartTable();
                    break;                   
                case 2:
                    StartPlot();
                    StartHistogram();
                    break;                   
                case 3:                   
                    StartPlot();
                    StartTable();
                    StartHistogram();
                    break;
                    
                default:
                    break;
                            
            }


            this.EcgDataDynamicTab.DataContext = visulisationDataTabsList;

        }

        public void StartPlot()
        {
            VisualisationPlotControl ecgVPControl = new VisualisationPlotControl();

            TabItem ecgBaselineTab = new TabItem();
            ecgBaselineTab.Header = "Plot";
            ecgBaselineTab.Content = ecgVPControl;
            visulisationDataTabsList.Add(ecgBaselineTab);
        }

        public void StartTable()
        {
            VisualisationTableControl ecgVTControl = new VisualisationTableControl();

            TabItem tableControl = new TabItem();
            tableControl.Header = "Table";
            tableControl.Content = ecgVTControl;
            visulisationDataTabsList.Add(tableControl);
        }

        public void StartHistogram()
        {
            VisualisationHistogramControl ecgVHControl = new VisualisationHistogramControl();

            TabItem histogramControl = new TabItem();
            histogramControl.Header = "Histogram";
            histogramControl.Content = ecgVHControl;
            visulisationDataTabsList.Add(histogramControl);
        }


    }
}
