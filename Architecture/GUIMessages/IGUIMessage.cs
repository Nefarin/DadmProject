using System.Windows.Controls;

namespace EKG_Project.Architecture.GUIMessages
{
    #region Documentation
    /// <summary>
    /// GUI Message interface, which provides easy way to send data to UI thread from Analysis thread.
    /// </summary>
    /// 
    #endregion
    public interface IGUIMessage
    {
        #region Documentation
        /// <summary>
        /// Reads given message with provided control.
        /// </summary>
        /// <param name="control"></param>
        /// 
        #endregion
        void Read(UserControl control);

    }
}
