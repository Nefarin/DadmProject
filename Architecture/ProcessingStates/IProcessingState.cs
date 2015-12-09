namespace EKG_Project.Architecture.ProcessingStates
{
    #region Documentation
    /// <summary>
    /// 
    /// </summary>
    /// 
    #endregion
    public interface IProcessingState
    {
        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        /// <param name="timeoutState"></param>
        /// 
        #endregion
        void Process(Processing process, out IProcessingState timeoutState);
    }
}
