using System;

namespace EKG_Project.IO
{
    public interface IECGPath
    {
        String getDataPath();
        String getCurrentPath();
        String getResourcesPath();
        String getTempPath();
        String getBasePath();
    }
}
