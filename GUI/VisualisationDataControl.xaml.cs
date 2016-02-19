using EKG_Project.IO;
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
        private List<string> tableModuleList; 
  
        //private ECG_Baseline_Data_Worker _ecg_Baseline_Data_worker;
        //private Basic_Data_Worker _ecg_Basic_Data_Worker;
        //private R_Peaks_Data_Worker _r_Peaks_Data_Worker;


        //0 - plot
        //1 - plot and table
        //2 - plot and histogram
        //3 - plot and table and histogram 
        private Dictionary<string, uint> modulesVisualisationNeeds = new Dictionary<string, uint>()
        {
            {"ECG_BASELINE", 0 },
            {"ecgBasic", 0 },
            {"R_PEAKS", 3 },
            {"WAVES", 0 },
            {"HEART_CLASS", 0 },
            {"HEART_AXIS", 0 },
            {"ARTRIAL_FIBER", 0 },
            {"HRV2", 3 }
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

        public VisualisationDataControl(string analyseName, string moduleName, KeyValuePair<string, int> moduleDict)
        {
            InitializeComponent();
            visulisationDataTabsList = new List<TabItem>();

            if (moduleDict.Value == 8)
            {
                modulesVisualisationNeeds[moduleName] = 1;

            }



            //if needed and what where needed? 
            switch (modulesVisualisationNeeds[moduleName])
            {
                case 0:                  
                    //StartPlot(analyseName, moduleName, moduleDict);
                    break;
                    
                case 1:
                    //StartPlot(analyseName, moduleName, moduleDict);
                    //StartTable(analyseName, moduleName, moduleDict);
                    break;                   
                case 2:
                    //StartPlot(analyseName, moduleName, moduleDict);
                    //StartHistogram(analyseName, moduleName, moduleDict);
                    break;                   
                case 3:
                    //StartPlot(analyseName, moduleName, moduleDict);
                    //StartTable(analyseName, moduleName, moduleDict);
                    //StartHistogram(analyseName, moduleName, moduleDict);
                    break;
                    
                default:
                    break;
                            
            }

            if (moduleDict.Value == 9)
            {
                //StartPlot(analyseName, "HEART_AXIS", new KeyValuePair<string, int>( "HEART_AXIS", 9 ) );
            }


            this.EcgDataDynamicTab.DataContext = visulisationDataTabsList;

        }

        //Program ver2.0

        public VisualisationDataControl(string analyseName, string moduleName, KeyValuePair<string, int> moduleDict, List<string> analysedModules)
        {
            InitializeComponent();
            visulisationDataTabsList = new List<TabItem>();
            tableModuleList = new List<string>();
            uint plotAmount = 0;
            uint tableAmount = 0;
            uint histAmount = 0;

            //System.Windows.MessageBox.Show(moduleName);

            if (moduleName == "ECG_BASELINE")
            {
                plotAmount = 1;

                if (analysedModules.Contains("QT_DISP"))
                {
                    tableModuleList.Add("QT_DISP");
                    //tableAmount += 1;
                }


                //start wszystkiego pod baseline: 

                for (int i = 0; i < plotAmount; i++)
                {
                    StartPlot(analyseName, moduleName, moduleDict, analysedModules);
                }

                foreach(string modName in tableModuleList)
                { 
                    StartTable(analyseName, modName, moduleDict, tableModuleList);
                }

                for (int i = 0; i < histAmount; i++)
                {
                    //need logic to not duble some hist
                    StartHistogram(analyseName, moduleName, moduleDict);
                }
            }

            //pozostale moduly

            if (moduleName == "HEART_AXIS")
            {
                StartPlot(analyseName, moduleName, moduleDict, analysedModules);
            }

            if (moduleName == "SLEEP_APNEA")
            {
               // StartPlot(analyseName, moduleName, moduleDict, analysedModules);
            }

            if (moduleName == "HRV_DFA")
            {
                StartPlot(analyseName, moduleName, moduleDict, analysedModules);
                StartTable(analyseName, moduleName, moduleDict, tableModuleList);
            }

            if (moduleName == "HRV2")
            {
                StartPlot(analyseName, moduleName, moduleDict, analysedModules);
                StartTable(analyseName, moduleName, moduleDict, tableModuleList);
                StartHistogram(analyseName, moduleName, moduleDict);
            }

            if (moduleName == "HRV1")
            {
                StartPlot(analyseName, moduleName, moduleDict, analysedModules);
                StartTable(analyseName, moduleName, moduleDict, tableModuleList);                
            }

            if (moduleName == "HRT")
            {
                StartPlot(analyseName, moduleName, moduleDict, analysedModules);
            }
            if (moduleName == "HEART_CLUSTER")
            {
                StartPlot(analyseName, moduleName, moduleDict, analysedModules);
                StartTable(analyseName, moduleName, moduleDict, tableModuleList);
            }

            this.EcgDataDynamicTab.DataContext = visulisationDataTabsList;

        }






        public void StartPlot(string anName,string modName, KeyValuePair<string, int> moduleDic, List<string> modL)
        {
            VisualisationPlotControl ecgVPControl = new VisualisationPlotControl(anName,modName, moduleDic, modL);

            TabItem ecgBaselineTab = new TabItem();
            ecgBaselineTab.Header = "Plot";
            ecgBaselineTab.Content = ecgVPControl;
            visulisationDataTabsList.Add(ecgBaselineTab);
        }

        public void StartTable(string anName, string modName, KeyValuePair<string, int> moduleDict, List<string> modL)
        {
            VisualisationTableControl ecgVTControl = new VisualisationTableControl(anName, modName, moduleDict, modL);

            TabItem tableControl = new TabItem();
            tableControl.Header = "Table";
            tableControl.Content = ecgVTControl;
            visulisationDataTabsList.Add(tableControl);
        }

        public void StartHistogram(string anName, string modName, KeyValuePair<string, int> moduleDict)
        {
            VisualisationHistogramControl ecgVHControl = new VisualisationHistogramControl(anName, modName);

            TabItem histogramControl = new TabItem();
            histogramControl.Header = "Histogram";
            histogramControl.Content = ecgVHControl;
            visulisationDataTabsList.Add(histogramControl);
        }


    }
}
