namespace EKG_Project.Architecture
{
    #region Documentation
    /// <summary>
    /// Responosible for provide command with data to Analysis Control thread.
    /// </summary>
    /// 
    #endregion
    public class ToGUIItem
    {
        private ToGUICommand _command;
        private object _data;

        #region Documentation
        /// <summary>
        /// TO DO
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// 
        #endregion
        public ToGUIItem(ToGUICommand command, object data)
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
        public ToGUICommand Command
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
        #endregion
    }
}
