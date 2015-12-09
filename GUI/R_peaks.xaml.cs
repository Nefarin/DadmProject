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
using System.Threading;
using System.Windows.Shapes;

namespace EKG_Project.GUI
{
    /// <summary>
    /// Interaction logic for R_peaks.xaml
    /// </summary>
    public partial class R_peaks : UserControl
    {
        private ECGPlot ecgPlot;


        public R_peaks()
        {
            InitializeComponent();
            ecgPlot = new ECGPlot("R_peaks");
            DataContext = ecgPlot;
            //ecgPlot.DisplayAnything();
            Thread thread = new Thread(new ThreadStart(updatePlot));
            //thread.Start();
        }

        private void updatePlot()
        {
            for (int i = 0; i < 1000; i++)
            {
                ecgPlot.ClearPlot();
                ecgPlot.CurrentPlot.Series.Add(ecgPlot.DisplayR_Peaks(i + 10, 0.8));
                ecgPlot.DisplaySin(i, i + 20);
                ECGPlotView.InvalidatePlot(true);
                Thread.Sleep(100);
            }
        }


    }
}
