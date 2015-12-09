using System.Windows.Controls;
using EKG_Project.GUI;

namespace EKG_Project.Architecture.GUIMessages
{
    #region Documentation
    /// <summary>
    /// 
    /// </summary>
    /// 
    #endregion
    public class TestMessage : IGUIMessage
    {
        private string _testText;

        #region Documentation
        /// <summary>
        /// 
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
        /// 
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
