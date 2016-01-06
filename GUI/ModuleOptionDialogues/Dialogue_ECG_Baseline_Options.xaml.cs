using EKG_Project.Modules;
using EKG_Project.Modules.ECG_Baseline;
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
using System.Windows.Shapes;

namespace EKG_Project.GUI.ModuleOptionDialogues
{
    /// <summary>
    /// Interaction logic for Dialogue_ECG_Baseline_Options.xaml
    /// </summary>
    public partial class Dialogue_ECG_Baseline_Options : Window
    {
        // muszę znać typ (nie mogę użyć samgo ModuleParams), ponieważ klasa bazowa (a nie np. interface), 
        // nie udostępnia żadnych metod kopiujących itp.
        private ECG_Baseline_Params returnParameters { get; set; }
        public ECG_Baseline_Params PendingParameters { get; set; }

        public Dialogue_ECG_Baseline_Options(ECG_Baseline_Params parameters)
        {
            this.returnParameters = parameters;

            // do dupy kawałek kodu, bo nie ma odpowiednich konstruktorów w klasie ECG_Baseline_Parameters 
            // (ani domyślnego, ani ustawiającego wszystkie parametry)
            this.PendingParameters = new ECG_Baseline_Params(Filtr_Method.BUTTERWORTH, Filtr_Type.HIGHPASS);
            this.PendingParameters.CopyParametersFrom(parameters);
            this.DataContext = this.PendingParameters;
            InitializeComponent();
        }

        private void ApplyParameterChanges(object sender, RoutedEventArgs e)
        {
            this.returnParameters.CopyParametersFrom(this.PendingParameters);
            this.Close();
        }

        private void RejectParameterChanges(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
