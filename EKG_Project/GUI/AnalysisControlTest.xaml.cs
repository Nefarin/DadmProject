using System;
using System.Windows;
using System.Windows.Controls;
using EKG_Project.Architecture;
using EKG_Project.Architecture.ProcessingStates;
using EKG_Project.Architecture.GUIMessages;

namespace EKG_Project.GUI
{
    #region Documentation
    /// <summary>
    /// Test class for architects for testing architecture concepts - GUI - do not use (use AnalysisControl instead).
    /// </summary>
    /// 
    #endregion
    public partial class AnalysisControlTest : UserControl
    {
        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            _communication.SendGUIMessage(new Abort());
        }

        #region Documentation
        /// <summary>
        /// analyzeEvent - do not delete - just develop - will be used by both GUI and Architects
        /// </summary>
        /// <param name="message"></param>
        ///
        #endregion
        private void analyzeEvent(IGUIMessage message)
        {
            message.Read(this);
        }

        private void runButton_Click(object sender, RoutedEventArgs e)
        {
            _communication.SendGUIMessage(new TestState());
        }
    }
}
