namespace EKG_Project.Architecture
{
    #region Documentation
    /// <summary>
    /// Enum responsible for sending 
    /// </summary>
    /// 
    #endregion
    public enum AnalysisState {
        #region Documentation
        /// <summary>
        /// Initialization
        /// </summary>
        /// 
        #endregion
        INIT,
        #region Documentation
        /// <summary>
        /// Idle state
        /// </summary>
        /// 
        #endregion
        IDLE,
        #region Documentation
        /// <summary>
        /// Stop Analysis
        /// </summary>
        /// 
        #endregion
        STOP_ANALYSIS,
        #region Documentation
        /// <summary>
        /// Timeout
        /// </summary>
        /// 
        #endregion
        TIMEOUT,
        ADD_TEST,
        SUB_TEST
    };
}
