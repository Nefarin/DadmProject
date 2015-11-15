using System;
using System.Windows;
using System.Windows.Controls;
using EKG_Project.Architecture;

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
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            _communication.sendGUIMessage(new ToProcessingItem(AnalysisState.ADD_TEST, null));
        }

        private void subButton_Click(object sender, RoutedEventArgs e)
        {
            _communication.sendGUIMessage(new ToProcessingItem(AnalysisState.SUB_TEST, null));
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            _communication.sendGUIMessage(new ToProcessingItem(AnalysisState.STOP_ANALYSIS, null));
        }

        #region Documentation
        /// <summary>
        /// analyzeEvent - do not delete - just develop - will be used by both GUI and Architects
        /// </summary>
        /// <param name="item"></param>
        ///
        #endregion
        private void analyzeEvent(ToGUIItem item)
        {
            switch (item.Command)
            {
                case ToGUICommand.ANALYSIS_ENDED:

                    break;
                case ToGUICommand.EXIT_ANALYSIS:
                    _communication.ToGUIEvent -= _analysisEvent;
                    _parent.closeAnalysisTab(_parentTab);
                    break;
                case ToGUICommand.TEST:
                    int value = Convert.ToInt32(item.Data);
                    progressBar.Value = value;
                    break;
                default:
                    break;
            }
        }
    }
}
