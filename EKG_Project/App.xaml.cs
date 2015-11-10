using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;

namespace EKG_Project
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        override protected void OnStartup(StartupEventArgs startEvent)
        {
            MainWindow mainWindow = new MainWindow(this);
            this.MainWindow = mainWindow;
            mainWindow.Show();
        }
        override protected void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }

    }
}
