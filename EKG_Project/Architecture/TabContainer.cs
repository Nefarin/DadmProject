using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls;

namespace EKG_Project.Architecture
{
    #region Documentation
    /// <summary>
    /// 
    /// </summary>
    /// 
    #endregion
    public class TabContainer
    {
        private List<TabItem> _tabItems;
        private List<ProcessSync> _communicationList;
        private List<Processing> _ecgAnalysisList;
        private List<UserControl> _analysisControlList;
        private List<Thread> _threadList;

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// 
        #endregion
        public TabContainer()
        {
            _tabItems = new List<TabItem>();
            _communicationList = new List<ProcessSync>();
            _ecgAnalysisList = new List<Processing>();
            _analysisControlList = new List<UserControl>();
            _threadList = new List<Thread>();
        }

        #region Properties
        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// 
        #endregion
        public List<TabItem> TabItems
        {
            get
            {
                return _tabItems;
            }

            set
            {
                _tabItems = value;
            }
        }

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// 
        #endregion
        public List<ProcessSync> CommunicationList
        {
            get
            {
                return _communicationList;
            }

            set
            {
                _communicationList = value;
            }
        }

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// 
        #endregion
        public List<Processing> EcgAnalysisList
        {
            get
            {
                return _ecgAnalysisList;
            }

            set
            {
                _ecgAnalysisList = value;
            }
        }

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// 
        #endregion
        public List<UserControl> AnalysisControlList
        {
            get
            {
                return _analysisControlList;
            }

            set
            {
                _analysisControlList = value;
            }
        }

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// 
        #endregion
        public List<Thread> ThreadList
        {
            get
            {
                return _threadList;
            }

            set
            {
                _threadList = value;
            }
        }
        #endregion
    }
}
