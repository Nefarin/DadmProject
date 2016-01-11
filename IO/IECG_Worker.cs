using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    public interface IECG_Worker
    {
        void Save(ECG_Data data);
        void Load();
    }
}
