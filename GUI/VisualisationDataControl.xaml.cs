﻿using EKG_Project.IO;
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

        //private ECG_Baseline_Data_Worker _ecg_Baseline_Data_worker;
        //private Basic_Data_Worker _ecg_Basic_Data_Worker;
        //private R_Peaks_Data_Worker _r_Peaks_Data_Worker;


        //0 - plot
        //1 - plot and table
        //2 - plot and histogram
        //3 - plot and table and histogram 
        private Dictionary<string, uint> modulesVisualisationNeeds = new Dictionary<string, uint>()
        {
            {"ECG_BASELINE", 1 },
            {"ecgBasic", 0 },
            {"R_PEAKS", 3 },
            {"WAVES", 0 },
            { "HEART_CLASS",0 },
            { "HEART_AXIS", 0 },
            {"ARTRIAL_FIBER", 0 }
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
            

            //if needed and what where needed? 
            switch (modulesVisualisationNeeds[moduleName])
            {
                case 0:                  
                    StartPlot(analyseName, moduleName, moduleDict);
                    break;
                    
                case 1:
                    StartPlot(analyseName, moduleName, moduleDict);
                    StartTable(analyseName, moduleName, moduleDict);
                    break;                   
                case 2:
                    StartPlot(analyseName, moduleName, moduleDict);
                    StartHistogram(analyseName, moduleName, moduleDict);
                    break;                   
                case 3:
                    StartPlot(analyseName, moduleName, moduleDict);
                    StartTable(analyseName, moduleName, moduleDict);
                    StartHistogram(analyseName, moduleName, moduleDict);
                    break;
                    
                default:
                    break;
                            
            }


            this.EcgDataDynamicTab.DataContext = visulisationDataTabsList;

        }


        //public VisualisationDataControl(string analyseName, KeyValuePair<string, int> moduleDict)
        //{
        //    InitializeComponent();
        //    visulisationDataTabsList = new List<TabItem>();


        //    //if needed and what where needed? 
        //    switch (modulesVisualisationNeeds[moduleName])
        //    {
        //        case 0:
        //            StartPlot(moduleName);
        //            break;

        //        case 1:
        //            StartPlot(moduleName);
        //            StartTable(moduleName);
        //            break;
        //        case 2:
        //            StartPlot(moduleName);
        //            StartHistogram(moduleName);
        //            break;
        //        case 3:
        //            StartPlot(moduleName);
        //            StartTable(moduleName);
        //            StartHistogram(moduleName);
        //            break;

        //        default:
        //            break;

        //    }


        //    this.EcgDataDynamicTab.DataContext = visulisationDataTabsList;

        //}

        public void StartPlot(string anName,string modName, KeyValuePair<string, int> moduleDic)
        {
            VisualisationPlotControl ecgVPControl = new VisualisationPlotControl(anName,modName, moduleDic);

            TabItem ecgBaselineTab = new TabItem();
            ecgBaselineTab.Header = "Plot";
            ecgBaselineTab.Content = ecgVPControl;
            visulisationDataTabsList.Add(ecgBaselineTab);
        }

        public void StartTable(string anName, string modName, KeyValuePair<string, int> moduleDict)
        {
            VisualisationTableControl ecgVTControl = new VisualisationTableControl();

            TabItem tableControl = new TabItem();
            tableControl.Header = "Table";
            tableControl.Content = ecgVTControl;
            visulisationDataTabsList.Add(tableControl);
        }

        public void StartHistogram(string anName, string modName, KeyValuePair<string, int> moduleDict)
        {
            VisualisationHistogramControl ecgVHControl = new VisualisationHistogramControl();

            TabItem histogramControl = new TabItem();
            histogramControl.Header = "Histogram";
            histogramControl.Content = ecgVHControl;
            visulisationDataTabsList.Add(histogramControl);
        }


    }
}
