using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using EKG_Project.GUI;

namespace EKG_Project
{
    // NIE DOTYKAC
    #region Documentation
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 
    #endregion
    public partial class App : Application
    {
        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startEvent"></param>
        /// 
        #endregion
        override protected void OnStartup(StartupEventArgs startEvent)
        {
           Window_ECG mainWindow = new Window_ECG(this);
            this.MainWindow = mainWindow;
            mainWindow.Show();
        }
        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// 
        #endregion
        override protected void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }

    }
}
