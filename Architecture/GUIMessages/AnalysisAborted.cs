using System.Windows.Controls;
using EKG_Project.GUI;

namespace EKG_Project.Architecture.GUIMessages
{
    #region Documentation
    /// <summary>
    /// GUI Message, which is generated after particular analysis was aborted in current window.
    /// </summary>
    ///
    #endregion
    public class AnalysisAborted : IGUIMessage
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
            AnalysisControl control = ctrl as AnalysisControl;
            control.analysisAborted();
        }
    }
}
