using System.Windows.Controls;

namespace EKG_Project.Architecture.GUIMessages
{
    #region Documentation
    /// <summary>
    /// 
    /// </summary>
    /// 
    #endregion
    public interface IGUIMessage
    {
        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// 
        #endregion
        void Read(UserControl control);

    }
}
