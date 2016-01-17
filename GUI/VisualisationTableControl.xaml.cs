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


        public VisualisationTableControl(string analyseName, string moduleName, KeyValuePair<string, int> moduleInfo)
        {
            InitializeComponent();

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



    }
}
