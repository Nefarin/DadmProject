using System;
using System.Collections.Generic;
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

    public class StatsCalculationEnded : IGUIMessage
    {
        private Dictionary<AvailableOptions, Dictionary<String, String>> _results;

        public StatsCalculationEnded(Dictionary<AvailableOptions, Dictionary<String, String>> results)
        {
            _results = new Dictionary<AvailableOptions, Dictionary<string, string>>(results);
        }
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
            control.statsCalculationEnded(_results);
        }
    }
}
