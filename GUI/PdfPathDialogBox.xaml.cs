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

namespace EKG_Project.GUI
{
    /// <summary>
    /// Interaction logic for PdfPathDialogBox.xaml
    /// </summary>
    public partial class PdfPathDialogBox : Window
    {
        public PdfPathDialogBox()
        {
            InitializeComponent();
        }

        public string Path
        {
            get { return txtPdfPath.Text; }
        }


        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtPdfPath.SelectAll();
            txtPdfPath.Focus();
        }
    }
}
