using EKG_Project.Architecture.GUIMessages;

namespace EKG_Project.Architecture
{
    #region Documentation
    /// <summary>
    /// ToGUIDelegate signature.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message"></param>
    /// 
    #endregion
    public delegate void ToGUIDelegate(object sender, IGUIMessage message);
}
