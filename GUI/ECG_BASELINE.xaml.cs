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
using EKG_Project.Modules.ECG_Baseline;
using System.Threading;

namespace EKG_Project.GUI
{
    /// <summary>
    /// Interaction logic for ECG_BASELINE.xaml
    /// </summary>
    public partial class ECG_BASELINE : UserControl
    {
        ECG_Baseline_Params parameters;
        private ECGPlot ecgPlot;

        /// <summary>
        /// 
        /// </summary>
        public ECG_BASELINE(ECG_Baseline_Params parameters)
        {
            this.InitializeComponent();
            this.parameters = parameters;
            ecgPlot = new ECGPlot("ECG_BASELINE");
            DataContext = ecgPlot;
            ecgPlot.DisplayAnything();
        }

        private void textBoxNumeric_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !this.isNumericInput(e.Text);

        }

        private void textBoxNumeric_Pasting(object sender, DataObjectPastingEventArgs e)
        {

            string input = string.Empty;
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                if (!this.isNumericInput((string)e.DataObject.GetData(typeof(string))))
                    e.CancelCommand();
            }
            else
                e.CancelCommand();
        }

        private bool isNumericInput(string input)
        {
            int result;
            return int.TryParse(input, out result);
        }

        private void cbCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            
        }

        private void updatePlot()
        {
            for (int i = 0; i < 1000; i++)
            {
                ecgPlot.ClearPlot();
                ecgPlot.DisplaySin(i, i + 50);
                ECGPlotView.InvalidatePlot(true);
                Thread.Sleep(100);
            }
        }

        private void cbCombo_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(updatePlot));
            //thread.Start();
        }
    }
}