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
    public class ModuleProgress : IGUIMessage
    {

        private AvailableOptions _module;
        private double _progress;
        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctrl"></param>
        /// 
        #endregion
        public ModuleProgress(AvailableOptions module, double progress)
        {
            _module = module;
            _progress = progress;
        }
        public void Read(UserControl ctrl)
        {
            AnalysisControl control = (AnalysisControl)ctrl;
            control.updateProgress(_module, _progress);
        }
    }
}
