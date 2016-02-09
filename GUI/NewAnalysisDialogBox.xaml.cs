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
    /// Interaction logic for NewAnalysisDialogBox.xaml
    /// Create an analysis name
    /// </summary>
    public partial class NewAnalysisDialogBox : Window
    {
        public string Answer
        {
            get { return txtAnswer.Text; }
        }

        /// <summary>
        /// Initializes component and set default analysis name
        /// </summary>
        /// <param name="defaultAnswer"></param>
        public NewAnalysisDialogBox(string defaultAnswer = "")
        {
            InitializeComponent();
            txtAnswer.Text = defaultAnswer;
        }

        /// <summary>
        /// Method onClick button which sets dialogResult to true
        /// </summary>
        /// <param name="sender">Supports class in .NET, default param</param>
        /// <param name="e">Contains state information and event data associated with a routed event</param>
        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        /// <summary>
        /// Sets dialog result to false, opposite to btnDialogOK_Click
        /// </summary>
        protected void OnClosing()
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// Occurs after a window's content has been rendered
        /// </summary>
        /// <param name="sender">Supports class in .NET, default param</param>
        /// <param name="e">Contains state information and event data associated with a routed event</param>
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtAnswer.SelectAll();
            txtAnswer.Focus();
        }
    }
}
