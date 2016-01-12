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
    /// Interaction logic for VisualisationPlotControl.xaml
    /// </summary>
    public partial class VisualisationPlotControl : UserControl
    {
        private ECGPlot ecgPlot;
        private string _plotType; 



        public VisualisationPlotControl()
        {
            //InitializeComponent();
            //ecgPlot = new ECGPlot("ECG_BASELINE");
            //DataContext = ecgPlot;
            //ecgPlot.DisplayBasicData();
            ////ecgPlot.DisplayEcgBaseline();
            ////ecgPlot.DisplayR_Peaks();
        }

        public VisualisationPlotControl(string moduleName)
        {
            InitializeComponent();
            _plotType = moduleName;
            ecgPlot = new ECGPlot(moduleName);
            DataContext = ecgPlot;
            
            ecgPlot.DisplayControler(_plotType);

            //ecgPlot.DisplayEcgBaseline();
            //ecgPlot.DisplayBasicData();
            //ecgPlot.DisplayControler(_plotType);
        }

        private void PlotForwardButton_Click(object sender, RoutedEventArgs e)
        {
            ecgPlot.MovePlot(500);
            ecgPlot.DisplayControler(_plotType);
           
        }

        private void PlotBackwardButton_Click(object sender, RoutedEventArgs e)
        {
            ecgPlot.MovePlot(-500);
            ecgPlot.DisplayControler(_plotType);
        }
    }
}
