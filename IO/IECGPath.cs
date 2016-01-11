using System;

namespace EKG_Project.IO
{
    public interface IECGPath
    {
        String getDataPath();
        String getResourcesPath();
        String getTempPath();
        String getBasePath();
    }
}
