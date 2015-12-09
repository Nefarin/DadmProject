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
    /// Interaction logic for ChooseModule.xaml
    /// </summary>
    public partial class ChooseModule : UserControl
    {
        ECG_BASELINE ecgBaseline;
        R_peaks rpeaks;

        public ChooseModule()
        {
            InitializeComponent();
            this.ecgBaseline = new ECG_BASELINE(null);
            this.rpeaks = new R_peaks();
;        }

        private void comboChooseModule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var value = ((ComboBoxItem)comboChooseModule.SelectedValue).Content as string;
            switch (value)
            {
                case "ECG_Baseline":
                    this.stackParameters.Children.RemoveRange(0, this.stackParameters.Children.Count);
                    this.stackParameters.Children.Add(this.ecgBaseline);
                    break;
                case "R_Peaks":
                    this.stackParameters.Children.RemoveRange(0, this.stackParameters.Children.Count);
                    this.stackParameters.Children.Add(this.rpeaks);
                    break;
                default:
                    this.stackParameters.Children.RemoveRange(0, this.stackParameters.Children.Count);
                    break;
            }
        }
    }
}
