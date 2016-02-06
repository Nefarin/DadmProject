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
using System.Collections.ObjectModel;
using System.Diagnostics;
using EKG_Project.IO;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace EKG_Project.GUI
{
    /// <summary>
    /// Interaction logic for VisualisationTableControl.xaml
    /// </summary>
    public partial class VisualisationTableControl : UserControl
    {
        public class DataToTable
        {
            
            public string Lead { get; set; }
            public string QT_Disp_Lcal { get; set; }
            public string QT_Mean { get; set; }
            public string QT_Std { get; set; }
        }

        //public int ID { get; set; }

        List<DataToTable> _tableData; 
        private QT_Disp_Data_Worker _qt_Disp_Data_Worker;
        private string _moduleInTable;
        private string _analyseName;

        private string _currentAnalysisName;
        private List<string> leadsNameList;



        public VisualisationTableControl(string analyseName, string moduleName, KeyValuePair<string, int> moduleInfo)
        {
            InitializeComponent();
            _moduleInTable = moduleName;
            _analyseName = analyseName;
            //List<DataToTable> tempData = new List<DataToTable>();
            //tempData.Add(new DataToTable { ModuleName = "EcgBaseline", ID = 1, JakiesDane = "Tutaj znajda sie dane od modułów" });
            //tempData.Add(new DataToTable { ModuleName = "R_Peaks", ID = 2, JakiesDane = "Tutaj znajda sie dane od modułów" });
            _tableData = new List<DataToTable>();
            switch (moduleInfo.Value)
            {
                case 8:
                    Get_QT_DISP_Data(analyseName);
                    //MessageBox.Show("analyseName=" + analyseName + ", moduleName=" + moduleName + ", moduleInfoKey=" + moduleInfo.Key + "=" + moduleInfo.Value);
                    break;
                default:
                    break;
            }


                    this.VisualisationDataTable.DataContext = _tableData;
        }


        //Program ver 2.0

        public VisualisationTableControl(string analyseName, string moduleName, KeyValuePair<string, int> moduleInfo, List<string> moduleList)
        {
            InitializeComponent();
            _moduleInTable = moduleName;
            _analyseName = analyseName;
            _currentAnalysisName = analyseName;
            
            _tableData = new List<DataToTable>();

            try
            {
                Basic_New_Data_Worker basicDataForLeads = new Basic_New_Data_Worker(analyseName);
                leadsNameList = basicDataForLeads.LoadLeads();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

            switch (moduleName)
            {
                case "QT_DISP":
                    DisplayQTDisplayTable();
                    break;
                default:
                    break;
            }


            this.VisualisationDataTable.DataContext = _tableData;
        }

        private bool DisplayQTDisplayTable()
        {
            try
            {
                System.Windows.MessageBox.Show("TABE11LE");
                Qt_Disp_New_Data_Worker qDW = new Qt_Disp_New_Data_Worker(_currentAnalysisName);
                foreach (string lead in leadsNameList)
                {
                    DataToTable dTT = new DataToTable();
                    dTT.Lead = lead;
                    dTT.QT_Disp_Lcal = qDW.LoadAttribute(Qt_Disp_Attributes.QT_disp_local, lead).ToString("0.00");
                    dTT.QT_Mean = qDW.LoadAttribute(Qt_Disp_Attributes.QT_mean, lead).ToString("0.00");
                    dTT.QT_Std = qDW.LoadAttribute(Qt_Disp_Attributes.QT_std, lead).ToString("0.00");
                    _tableData.Add(dTT);
                }

                return true;
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }



        public VisualisationTableControl()
        {
            InitializeComponent();
        }

        private void Get_QT_DISP_Data(string currentAnalyseName)
        {
            _qt_Disp_Data_Worker = new QT_Disp_Data_Worker(currentAnalyseName);
            _qt_Disp_Data_Worker.Load();

            
            
            foreach (var dat in _qt_Disp_Data_Worker.Data.QT_disp_local)
            {
                DataToTable dTT = new DataToTable();
                dTT.Lead = dat.Item1;
                dTT.QT_Disp_Lcal = dat.Item2.ToString("0.00");
                dTT.QT_Mean = _qt_Disp_Data_Worker.Data.QT_mean.First(a => a.Item1 == dat.Item1).Item2.ToString("0.00");
                dTT.QT_Std = _qt_Disp_Data_Worker.Data.QT_std.First(a => a.Item1 == dat.Item1).Item2.ToString("0.00");
                _tableData.Add(dTT);

            }
            

            

        }

        private void SavePlotButton_Click(object sender, RoutedEventArgs e)
        {

            string filename = "";
            //string fullPath = "";
            bool toSave = false;
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".xml";
            dlg.Filter = "Xml documents (.xml)|*.xml";
            if (dlg.ShowDialog() == true)
            {
                toSave = true;
                filename = dlg.FileName;

                
                
            }

            if (toSave)
            {
                //string fileNameToSave = _analyseName + "_" + _moduleInTable + "_" + filename + ".xml";
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<DataToTable>));
                TextWriter Filestream = new StreamWriter(filename);
                xmlSerializer.Serialize(Filestream, _tableData);
                Filestream.Close();
            }

        }
    }
}
