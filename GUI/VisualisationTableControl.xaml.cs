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

        public class DataToTableHRV2
        {

            public string Lead { get; set; }
            public string TINN { get; set; }
            public string TriangleIndex { get; set; }
            public string SD1 { get; set; }
            public string SD2{ get; set; }
        }

        public class DataToTableHRVDFA
        {
            public string Lead { get; set; }
            public string a1 { get; set; }
            public string a2 { get; set; }
        }

        public class DataToTableHeartCluster
        {
            public string Lead { get; set; }
            public string TotalQRSComplex { get; set; }
            public string ClassAmount { get; set; }
            //public List<string> IndefOfClass { get; set; }
            //public List<string> QRSComplexNr { get; set; }
            public string IndefOfClass { get; set; }
            public string QRSComplexNr { get; set; }

        }

        public class DataToTableHRV1
        {

            public string Lead { get; set; }
            public string AVNN { get; set; }
            public string HF { get; set; }
            public string LF { get; set; }
            public string LFHF { get; set; }
            public string NN50 { get; set; }
            public string pNN50 { get; set; }
            public string RMSSD { get; set; }
            public string SDNN { get; set; }
            public string SDSD { get; set; }
            public string TP { get; set; }
            public string VLF { get; set; }
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
                case "HRV2":
                    DisplayHRV2Table();
                    break;
                case "HRV1":
                    DisplayHRV1Table();
                    break;
                case "HRV_DFA":
                    DisplayHRVDFATable();
                    break;
                case "HEART_CLUSTER":
                    DisplayHeartClusterTable();
                    break;
                default:
                    break;
            }


           
        }

        private bool DisplayQTDisplayTable()
        {
            try
            {
                _tableData = new List<DataToTable>();
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

                this.VisualisationDataTable.DataContext = _tableData;
                return true;
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }

        private bool DisplayHRV2Table()
        {
            try
            {
                List<DataToTableHRV2> _dataToPrint = new List<DataToTableHRV2>();
                HRV2_New_Data_Worker hW = new HRV2_New_Data_Worker(_currentAnalysisName);
                foreach (string lead in leadsNameList)
                {
                    DataToTableHRV2 dTT = new DataToTableHRV2();
                    dTT.Lead = lead;
                    dTT.TINN = hW.LoadAttribute(HRV2_Attributes.Tinn, lead).ToString(); 
                    dTT.TriangleIndex = hW.LoadAttribute(HRV2_Attributes.TriangleIndex, lead).ToString();
                    dTT.SD1 = hW.LoadAttribute(HRV2_Attributes.SD1, lead).ToString("0.00");
                    dTT.SD2 = hW.LoadAttribute(HRV2_Attributes.SD2, lead).ToString("0.00");
                    _dataToPrint.Add(dTT);
                }
                this.VisualisationDataTable.DataContext = _dataToPrint;
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }

        private bool DisplayHRV1Table()
        {
            try
            {
                List<DataToTableHRV1> _dataToPrint = new List<DataToTableHRV1>();
                HRV1_New_Data_Worker hW = new HRV1_New_Data_Worker(_currentAnalysisName);
                foreach(string lead in leadsNameList)
                {
                    DataToTableHRV1 dTT = new DataToTableHRV1();
                    dTT.Lead = lead;
                    //dTT.HF = "heheszki";

                    _dataToPrint.Add(dTT);
                }

                this.VisualisationDataTable.DataContext = _dataToPrint;

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }

        private bool DisplayHRVDFATable()
        {
            try
            {
                List<DataToTableHRVDFA> _dataToPrint = new List<DataToTableHRVDFA>();
                HRV_DFA_New_Data_Worker hDW = new HRV_DFA_New_Data_Worker(_currentAnalysisName);

                

                foreach (string lead in leadsNameList)
                {
                    try
                    {
                        Tuple<MathNet.Numerics.LinearAlgebra.Vector<double>, MathNet.Numerics.LinearAlgebra.Vector<double>> paramAlpha = hDW.LoadSignal(HRV_DFA_Signals.ParamAlpha, lead, 0, (int)hDW.getNumberOfSamples(HRV_DFA_Signals.ParamAlpha, lead));

                        DataToTableHRVDFA dTT = new DataToTableHRVDFA();
                        dTT.a1 = paramAlpha.Item1[0].ToString();
                        dTT.a2 = paramAlpha.Item2[0].ToString();
                        dTT.Lead = lead;
                        _dataToPrint.Add(dTT);
                    }
                    catch
                    {

                    }
                }

                this.VisualisationDataTable.DataContext = _dataToPrint;

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }

        private bool DisplayHeartClusterTable()
        {
            try
            {
                List<DataToTableHeartCluster> _dataToPrint = new List<DataToTableHeartCluster>();
                Heart_Cluster_Data_Worker hCW = new Heart_Cluster_Data_Worker(_currentAnalysisName);
                
                foreach (string lead in leadsNameList)
                {
                    try
                    {
                        DataToTableHeartCluster dTT = new DataToTableHeartCluster();

                        dTT.Lead = lead;
                        dTT.ClassAmount = hCW.LoadAttributeI(Heart_Cluster_Attributes_I.NumberofClass, lead).ToString();
                        dTT.TotalQRSComplex = hCW.LoadAttributeI(Heart_Cluster_Attributes_I.TotalQrsComplex, lead).ToString();
                        dTT.IndefOfClass = hCW.LoadAttributeII(Heart_Cluster_Attributes_II.indexOfClass, lead).ToString();
                        dTT.QRSComplexNr = hCW.LoadAttributeII(Heart_Cluster_Attributes_II.QrsComplexNo, lead).ToString();

                        _dataToPrint.Add(dTT);
                    }
                    catch(Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message);
                    }
                }

                this.VisualisationDataTable.DataContext = _dataToPrint;

                return true;
            }
            catch (Exception ex)
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
