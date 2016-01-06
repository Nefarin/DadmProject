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

namespace EKG_Project.GUI
{
    /// <summary>
    /// Interaction logic for VisualisationTableControl.xaml
    /// </summary>
    public partial class VisualisationTableControl : UserControl
    {
        public class DataToTable
        {
            public int ID { get; set; }
            public string ModuleName { get; set; }
            public string JakiesDane { get; set; }
        }


        public VisualisationTableControl()
        {
            InitializeComponent();

            List<DataToTable> tempData = new List<DataToTable>();
            tempData.Add(new DataToTable { ModuleName = "EcgBaseline", ID = 1, JakiesDane = "Tutaj znajda sie dane od modułów" });
            tempData.Add(new DataToTable { ModuleName = "R_Peaks", ID = 2, JakiesDane = "Tutaj znajda sie dane od modułów" });

            this.VisualisationDataTable.DataContext = tempData;
        }
    }
}
