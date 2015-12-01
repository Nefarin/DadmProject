using System;
using System.Windows;
using System.Windows.Controls;
using EKG_Project.Architecture;
using EKG_Project.Architecture.ProcessingStates;
using EKG_Project.Architecture.GUIMessages;
using System.Collections.Generic;
using OxyPlot;

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
        private TestPlots _testPlot;

        public class TestPlots
        {
            private string _plotTitle;
            private IList<DataPoint> _plotPoints;

            public TestPlots()
            {
                this.PlotTitle = "Test";
                this.PlotPoints = new List<DataPoint>
                              {
                                  new DataPoint(0, 4),
                                  new DataPoint(10, 13),
                                  new DataPoint(20, 15),
                                  new DataPoint(30, 16),
                                  new DataPoint(40, 12),
                                  new DataPoint(50, 12)
                              };
            }
            public IList<DataPoint> PlotPoints
            {
                get
                {
                    return _plotPoints;
                }

                set
                {
                    _plotPoints = value;
                }
            }
            public string PlotTitle
            {
                get
                {
                    return _plotTitle;
                }

                set
                {
                    _plotTitle = value;
                }
            }
        }

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
            _testPlot = new TestPlots();
            TestPlot.DataContext = _testPlot;
            _communication.SendGUIMessage(new TestState());
        }

    }
}
