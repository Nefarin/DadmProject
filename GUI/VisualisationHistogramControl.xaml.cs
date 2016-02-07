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
    /// Interaction logic for VisualisationHistogramControl.xaml
    /// </summary>
    public partial class VisualisationHistogramControl : UserControl
    {
        private ECGPlot ecgHistogramPlot;
        private string _currentAnalysisName;
        private List<string> leadsNameList;
        private List<CheckBox> _seriesChecbox;
        private bool first = true;
        private string firstLead;
        private string _plotType;

        public VisualisationHistogramControl()
        {
            InitializeComponent();



            ecgHistogramPlot = new ECGPlot("ECG_HISTOGRAM");
            DataContext = ecgHistogramPlot;
            ecgHistogramPlot.DisplayHistogram();
        }

        public VisualisationHistogramControl(string analyseName, string moduleName)
        {
            InitializeComponent();       
            _currentAnalysisName = analyseName;
            _plotType = moduleName;
            _seriesChecbox = new List<CheckBox>();

            ecgHistogramPlot = new ECGPlot(analyseName, moduleName);
            DataContext = ecgHistogramPlot;
            

            CreateAllCheckBoxesInCurrentAnalyse(analyseName);
            this.CheckBoxList.DataContext = _seriesChecbox;

            ecgHistogramPlot.DisplayHRV2HistogramLeads(firstLead);

        }

        private void CreateAllCheckBoxesInCurrentAnalyse(string currentAnalyseName)
        {
            try
            {
                IO.Basic_New_Data_Worker basicDataForLeads = new IO.Basic_New_Data_Worker(currentAnalyseName);
                leadsNameList = basicDataForLeads.LoadLeads();
                firstLead = leadsNameList.First();

                foreach (string lead in leadsNameList)
                {
                    //System.Windows.MessageBox.Show(lead);
                    CheckBox cB = new CheckBox();
                    cB.IsChecked = first;
                    first = false;
                    cB.Name = lead;
                    cB.Content = lead;
                    cB.Checked += CheckBox_Checked;
                    cB.Unchecked += CheckBox_Unchecked;
                    _seriesChecbox.Add(cB);
                }
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message + System.Environment.NewLine + ex.Source);
            }
        }


        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var c = sender as CheckBox;
            //MessageBox.Show("Checked=" + c.Name);
            if (leadsNameList.Contains(c.Name))
            {
                foreach (var cB in this.CheckBoxList.Items)
                {
                    var cc = cB as CheckBox;
                    if (cc.Name != c.Name)
                    {
                        cc.Checked -= CheckBox_Checked;
                        cc.Unchecked -= CheckBox_Unchecked;
                        cc.IsChecked = false;
                        cc.Checked += CheckBox_Checked;
                        cc.Unchecked += CheckBox_Unchecked;
                    }

                }
                ecgHistogramPlot.RemoveAllPlotSeries();
                //wyswietlenie żadnego leadu
                if (_plotType == "ECG_BASELINE")
                {
                    ecgHistogramPlot.DisplayBaselineLeads(c.Name);
                }
                if (_plotType == "HRV2")
                {
                    ecgHistogramPlot.DisplayHRV2HistogramLeads(c.Name);
                }

            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var c = sender as CheckBox;
            if (leadsNameList.Contains(c.Name))
            {
                foreach (var cB in this.CheckBoxList.Items)
                {
                    var cc = cB as CheckBox;
                    cc.Checked -= CheckBox_Checked;
                    cc.Unchecked -= CheckBox_Unchecked;
                    cc.IsChecked = false;
                    cc.Checked += CheckBox_Checked;
                    cc.Unchecked += CheckBox_Unchecked;

                    ecgHistogramPlot.RemoveAllPlotSeries();
                }
            }
        }


        private void SavePlotButton_Click(object sender, RoutedEventArgs e)
        {
            ecgHistogramPlot.SavePlot();
        }
    }
}
