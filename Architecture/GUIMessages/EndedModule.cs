using System.Windows.Controls;
using EKG_Project.GUI;

namespace EKG_Project.Architecture.GUIMessages
{
    #region Documentation
    /// <summary>
    /// GUI Message, which is generated after particular module ended its analysis.
    /// </summary>
    ///
    #endregion
    public class EndedModule : IGUIMessage
    {

        private AvailableOptions _module;
        private bool _aborted;

        #region Documentation
        /// <summary>
        /// Constructor, which sets the module, which ended analysis and information if it was aborted.
        /// </summary>
        /// <param name="module"></param>
        /// <param name="aborted"></param>
        #endregion
        public EndedModule(AvailableOptions module, bool aborted)
        {
            _module = module;
            _aborted = aborted;
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
            control.moduleEnded(_module, _aborted);
        }
    }
}
