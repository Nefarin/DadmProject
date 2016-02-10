using EKG_Project.IO;

namespace EKG_Project.Architecture
{
    #region Documentation
    /// <summary>
    /// Class responsible for handling DLL conflicts and problems.
    /// </summary>
    #endregion
    public class DLLLoader
    {
        #region Documentation
        /// <summary>
        /// Copies the MITBIH format loading library to .exe folder.
        /// </summary>
        #endregion
        public static void CopyWFDBDLL()
        {
            IECGPath pathBuilder = new DebugECGPath();
            string basePath = pathBuilder.getBasePath();
            string dllPath = System.IO.Path.Combine(basePath, "DLL");
            string currentPath = pathBuilder.getCurrentPath();
            string fileName = "wfdb.dll";
            System.IO.File.Copy(System.IO.Path.Combine(dllPath, fileName), System.IO.Path.Combine(currentPath, fileName), true);
        }
    }
}

