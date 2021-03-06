﻿using System.Windows.Controls;
using EKG_Project.GUI;

namespace EKG_Project.Architecture.GUIMessages
{
    #region Documentation
    /// <summary>
    /// GUI Message, which is generated during analysis closing.
    /// </summary>
    ///
    #endregion
    public class AnalysisEnded : IGUIMessage
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
            AnalysisControl control = (AnalysisControl) ctrl;
            control.Communication.ToGUIEvent -= control.AnalysisEvent;
            control.Parent.closeAnalysisTab(control.ParentTab);
        }
    }
}
