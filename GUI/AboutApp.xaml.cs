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
    /// Interaction logic for AboutApp Window
    /// </summary>
    public partial class AboutApp : Window
    {
        /// <summary>
        /// Initialize component (window)
        /// </summary>
        public AboutApp()
        {
            InitializeComponent();
        }

        /// <summary>
        /// On clicking OK user close the window.
        /// </summary>
        /// <param name="sender">Supports class in .NET, default param</param>
        /// <param name="e">Contains state information and event data associated with a routed event</param>
        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
