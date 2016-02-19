using System.Windows.Controls;
using EKG_Project.GUI;

namespace EKG_Project.Architecture.GUIMessages
{
    #region Documentation
    /// <summary>
    /// GUI Message, which is generated when file processor updates progress.
    /// </summary>
    ///
    #endregion
    public class FileProgress : IGUIMessage
    {
        private double _progress;

        #region Documentation
        /// <summary>
        /// Constructor, which sets the module, which updates progress and progress value.
        /// </summary>
        /// <param name="progress"></param>
        #endregion
        public FileProgress(double progress)
        {
            _progress = progress;
        }

        #region Documentation
        /// <summary>
        /// Reads given message with provided control.
        /// </summary>
        /// <param name="ctrl"></param>
        /// 
        #endregion
        public void Read(UserControl ctrl)
        {
            AnalysisControl control = (AnalysisControl)ctrl;
            control.updateFileProgress(_progress);
        }
    }
}
