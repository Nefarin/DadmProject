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
    public class EndedModule : IGUIMessage
    {

        private AvailableOptions _module;
        private bool _aborted;
        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctrl"></param>
        /// 
        #endregion
        public EndedModule(AvailableOptions module, bool aborted)
        {
            _module = module;
            _aborted = aborted;
        }
        public void Read(UserControl ctrl)
        {
            AnalysisControl control = (AnalysisControl)ctrl;
            control.moduleEnded(_module, _aborted);
        }
    }
}
