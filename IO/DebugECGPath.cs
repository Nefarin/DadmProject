using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.IO
{
    public class DebugECGPath : IECGPath
    {
        public string getBasePath()
        {
            return System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName
                (System.IO.Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)));
        }

        public string getDataPath()
        {
            return System.IO.Path.Combine(getBasePath(), "IO", "data");
        }

        public string getResourcesPath()
        {
            return System.IO.Path.Combine(getBasePath(), "Resources");
        }

        public string getTempPath()
        {
            return System.IO.Path.Combine(getBasePath(), "IO", "temp");
        }

    }
}
