using System.Windows.Controls;
using EKG_Project.GUI;

namespace EKG_Project.Architecture.GUIMessages
{
    #region Documentation
    /// <summary>
    /// GUI Message, which is generated when stats updates progress.
    /// </summary>
    ///
    #endregion
    public class StatsProgress : IGUIMessage
    {
        private double _progress;

        #region Documentation
        /// <summary>
        /// Constructor, which sets the module, which updates progress and progress value.
        /// </summary>
        /// <param name="module"></param>
        /// <param name="progress"></param>
        #endregion
        public StatsProgress(double progress)
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
            control.updateStatsProgress(_progress);
        }
    }
}
