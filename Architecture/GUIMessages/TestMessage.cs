using System.Windows.Controls;
using EKG_Project.GUI;

namespace EKG_Project.Architecture.GUIMessages
{
    #region Documentation
    /// <summary>
    /// Debugging purposes.
    /// </summary>
    /// 
    #endregion
    public class TestMessage : IGUIMessage
    {
        private string _testText;

        #region Documentation
        /// <summary>
        /// Debugging purposes.
        /// </summary>
        /// <param name="text"></param>
        /// 
        #endregion
        public TestMessage(string text)
        {
            _testText = text;
        }

        #region Documentation
        /// <summary>
        /// Reads given message with provided control.
        /// </summary>
        /// <param name="control"></param>
        /// 
        #endregion
        public void Read(UserControl control)
        {
            AnalysisControlTest ctrl = (AnalysisControlTest) control;
            ctrl.moduleName.Text = _testText;
        }
    }
}
