namespace EKG_Project.Architecture.ProcessingStates
{
    #region Documentation
    /// <summary>
    /// Message to Analysis thread interface.
    /// </summary>
    /// 
    #endregion
    public interface IProcessingState
    {
        #region Documentation
        /// <summary>
        /// Sets next processing state.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="timeoutState"></param>
        /// 
        #endregion
        void Process(Processing process, out IProcessingState timeoutState);
    }
}
