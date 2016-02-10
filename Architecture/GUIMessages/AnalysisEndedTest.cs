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
    public class AnalysisEndedTest : IGUIMessage
    {
        #region Documentation
        /// <summary>
        /// Reads given message with provided control.
        /// </summary>
        /// <param name="ctrl"></param>
        /// 
        #endregion
        public void Read(UserControl ctrl)
        {
            AnalysisControlTest control = (AnalysisControlTest)ctrl;
            control.Communication.ToGUIEvent -= control.AnalysisEvent;
            control.Parent.closeAnalysisTab(control.ParentTab);
        }
    }
}
