using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;

namespace EKG_Project.Architecture
{
    public class DLLLoader
    {
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
