﻿using System.Windows;
using EKG_Project.GUI;
using System;
using System.Runtime;

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
            //GCSettings.LatencyMode = GCLatencyMode.LowLatency;
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
