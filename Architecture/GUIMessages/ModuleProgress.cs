using System.Windows.Controls;
using EKG_Project.GUI;

namespace EKG_Project.Architecture.GUIMessages
{
    #region Documentation
    /// <summary>
    /// GUI Message, which is generated when module updates progress.
    /// </summary>
    ///
    #endregion
    public class ModuleProgress : IGUIMessage
    {

        private AvailableOptions _module;
        private double _progress;

        #region Documentation
        /// <summary>
        /// Constructor, which sets the module, which updates progress and progress value.
        /// </summary>
        /// <param name="module"></param>
        /// <param name="progress"></param>
        #endregion
        public ModuleProgress(AvailableOptions module, double progress)
        {
            _module = module;
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
            control.updateModuleProgress(_module, _progress);
        }
    }
}
