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

        public VisualisationHistogramControl()
        {
            InitializeComponent();

            ecgHistogramPlot = new ECGPlot("ECG_HISTOGRAM");
            DataContext = ecgHistogramPlot;
            ecgHistogramPlot.DisplayAnything();
        }
    }
}
