using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules
{
    public interface IModuleStats
    {
        void Init(String analysisName);
        void ProcessStats();
        bool Ended();
        bool Aborted();
        void Abort();
        Dictionary<String, String> GetStatsAsString();
        Dictionary<String, Object> GetStats();
    }
}

