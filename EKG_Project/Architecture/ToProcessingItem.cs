namespace EKG_Project.Architecture
{
    #region Documentation
    /// <summary>
    /// Responosible for provide command with data to processing thread.
    /// </summary>
    /// 
    #endregion
    public class ToProcessingItem
    {
        private AnalysisState _command;
        private object _data;

        #region Documentation
        /// <summary>
        /// TO DO
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// 
        #endregion
        public ToProcessingItem(AnalysisState command, object data)
        {
            _command = command;
            _data = data;
        }
        #region Properties
        #region Documentation
        /// <summary>
        /// TO DO
        /// </summary>
        ///
        #endregion
        public object Data
        {
            get
            {
                return _data;
            }

            set
            {
                _data = value;
            }
        }

        #region Documentation
        /// <summary>
        /// TO DO
        /// </summary>
        /// 
        #endregion
        internal AnalysisState Command
        {
            get
            {
                return _command;
            }

            set
            {
                _command = value;
            }
        }
        #endregion
    }
}
