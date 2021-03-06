﻿using System;
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
using EKG_Project.IO;

namespace EKG_Project.GUI
{
    /// <summary>
    /// Interaction logic for VisualisationPanelControl.xaml
    /// </summary>
    public partial class VisualisationPanelControl : UserControl
    {
        //private List<TabItem> visulisationTabsList;
        private List<TabItem> visulisationDataTabsList;
        public string headerParameters { get; set; }


        public VisualisationPanelControl()
        {

            InitializeComponent();
            VisualisationDataControl ecgVDataControl = new VisualisationDataControl();

            visulisationDataTabsList = new List<TabItem>();

            TabItem ecgBaselineTab = new TabItem();
            ecgBaselineTab.Header = "ECGBaseline";
            ecgBaselineTab.Content = ecgVDataControl;
            visulisationDataTabsList.Add(ecgBaselineTab);

            this.EcgDynamicTab.DataContext = visulisationDataTabsList;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //this.headerTable.ItemsSource = this.CreateHeaderInfoTable().DefaultView;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        public VisualisationPanelControl(string analyseName, List<string> tabNames)
        {
            InitializeComponent();
            //ChooseTabDisplay(analyseName,tabNames);
            NewChooseTabDisplay(analyseName, tabNames);
            headerParameters = getHeaderData(analyseName);
            //HeaderTable.ItemsSource = CreateHeaderInfoTable(analyseName).AsDataView();

        }

        private void ChooseTabDisplay(string analysN, List<string> tabNames)
        {
            //List<string> _endDisplay = new List<string>();
            Dictionary<string, int> _endDisplay = new Dictionary<string, int>();

            //_endDisplay["ECG_BASELINE"] = 1; tylko baseline
            // 2 - baseline i r_peaksy
            // 3 - baseline, r_peaksy, waves
            // 4 - baseline, waves

            if (tabNames.Contains("ECG_BASELINE"))
            {
                _endDisplay["ECG_BASELINE"] = 1;

                if(tabNames.Contains("HRV2"))
                {
                    //_endDisplay["HRV2"] = 10;
                }

                if(tabNames.Contains("R_PEAKS"))
                {

                    _endDisplay["ECG_BASELINE"] = 2;
                    if(tabNames.Contains("WAVES"))
                    {
                        _endDisplay["ECG_BASELINE"] = 3;
                        bool sA = false;
                        bool hC = false; 
                        if(tabNames.Contains("SLEEP_APNEA"))
                        {
                            _endDisplay["ECG_BASELINE"] = 5;
                            sA = true;
                            
                        }
                        if (tabNames.Contains("HEART_CLASS"))
                        {
                            _endDisplay["ECG_BASELINE"] = 4;
                            hC = true;
                            if (tabNames.Contains("HEART_AXIS"))
                            {
                                _endDisplay["HEART_AXIS"] = 9;
                            }
                        }

                        if (sA==true && hC == true)
                        {
                            _endDisplay["ECG_BASELINE"] = 6;
                        }

                        if(tabNames.Contains("ATRIAL_FIBER"))
                        {
                            _endDisplay["ECG_BASELINE"] = 7;
                        }

                        //if (tabNames.Contains("ATRIAL_FIBER"))
                        //{
                        //    _endDisplay["ECG_BASELINE"] = 8;
                        //}

                        if (tabNames.Contains("QT_DISP"))
                        {
                            _endDisplay["ECG_BASELINE"] = 8;
                        }



                    }
                }
            }


            visulisationDataTabsList = new List<TabItem>();
            //foreach(string tabName in tabNames)
            //{
            //    VisualisationDataControl ecgVDataControl = new VisualisationDataControl(analysN, tabName);
            //    TabItem tabItem = new TabItem();
            //    tabItem.Header = tabName;
            //    tabItem.Content = ecgVDataControl;
            //    visulisationDataTabsList.Add(tabItem);
            //}

            foreach(var dic in _endDisplay)
            {
                VisualisationDataControl ecgVDataControl = new VisualisationDataControl(analysN, dic.Key, dic);
                TabItem tabItem = new TabItem();
                tabItem.Header = dic.Key;
                tabItem.Content = ecgVDataControl;
                visulisationDataTabsList.Add(tabItem);
                //MessageBox.Show(dic.Key);
            }

            this.EcgDynamicTab.DataContext = visulisationDataTabsList;
        }

        private void NewChooseTabDisplay(string analysN, List<string> tabNames)
        {
            try
            {
                Dictionary<string, int> independentModules = new Dictionary<string, int>();

                //Split Independent Modules 
                if (tabNames.Contains("ECG_BASELINE"))
                {
                    independentModules["ECG_BASELINE"] = 1;
                }
                if (tabNames.Contains("HEART_AXIS"))
                {
                    independentModules["HEART_AXIS"] = 1;
                }
                if (tabNames.Contains("HRV2"))
                {
                    independentModules["HRV2"] = 1;
                }
                if (tabNames.Contains("SLEEP_APNEA"))
                {
                    independentModules["SLEEP_APNEA"] = 1;
                }
                if (tabNames.Contains("HRV_DFA"))
                {
                    independentModules["HRV_DFA"] = 1;
                }
                if (tabNames.Contains("HRV1"))
                {
                    independentModules["HRV1"] = 1;
                }
                if (tabNames.Contains("HRT"))
                {
                    independentModules["HRT"] = 1;
                }
                if (tabNames.Contains("HEART_CLUSTER"))
                {
                    //independentModules["HEART_CLUSTER"] = 1;
                    MessageBox.Show("Module HEART_CLUSTER was not visualised becouse of authors failure.");
                }

                //add other independent modules



                visulisationDataTabsList = new List<TabItem>();
                foreach (var dic in independentModules)
                {
                    VisualisationDataControl ecgVDataControl = new VisualisationDataControl(analysN, dic.Key, dic, tabNames);
                    TabItem tabItem = new TabItem();
                    tabItem.Header = dic.Key;
                    tabItem.Content = ecgVDataControl;
                    visulisationDataTabsList.Add(tabItem);
                }

                this.EcgDynamicTab.DataContext = visulisationDataTabsList;
            }
            catch
            {

            }
        }


        /// <summary>
        /// Generates a DataTable with information from signal file's header
        /// </summary>
        /// <param name="analysisName">Name of the current analysis</param>
        public System.Data.DataTable CreateHeaderInfoTable(string analysisName)
        {
            Basic_New_Data_Worker dataWorker = new Basic_New_Data_Worker(analysisName);
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("Lead name", Type.GetType("System.String")));
            table.Columns.Add(new DataColumn("Fs", Type.GetType("System.UInt32")));
            table.Columns.Add(new DataColumn("Samples", Type.GetType("System.UInt32")));
            List<string> leads = dataWorker.LoadLeads();
            uint frequency = dataWorker.LoadAttribute(Basic_Attributes.Frequency);

            foreach (string lead in leads)
            {
                uint noOfSamples = dataWorker.getNumberOfSamples(lead);

                table.Rows.Add(lead, frequency, noOfSamples);
            }

            return table;
        }

        /// <summary>
        /// Generates a string with information from signal file's header
        /// </summary>
        /// <param name="analysisName">Name of the current analysis</param>
        public string getHeaderData(string analysisName)
        {
            try {
                Basic_New_Data_Worker dataWorker = new Basic_New_Data_Worker(analysisName);
                uint frequency = dataWorker.LoadAttribute(Basic_Attributes.Frequency);
                List<string> leads = dataWorker.LoadLeads();
                int noOfLeads = leads.Count();
                uint noOfSamples = dataWorker.getNumberOfSamples(leads[0]);
                float duration = (1 / (float)frequency) * noOfSamples;
                string header_data = "Number of leads: " + noOfLeads.ToString() + "   Sampling frequency [Hz]: " + frequency.ToString()
                + "   Signal duration [s]: " + duration.ToString();
                return header_data;
            }
            catch (Exception ex)
            {
                return " ";
            }
        }




    }
}
