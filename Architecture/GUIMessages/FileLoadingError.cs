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
    public class FileLoadingError : IGUIMessage
    {
        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctrl"></param>
        /// 
        #endregion
        public void Read(UserControl ctrl)
        {
            AnalysisControl control = (AnalysisControl)ctrl;
            control.fileError();
        }
    }
}
