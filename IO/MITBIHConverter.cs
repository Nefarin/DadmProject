using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    class MITBIHConverter : IECGConverter
    {
        string pathIn;
        string analysisName;

        public MITBIHConverter(string MITBIHAnalysisName)
        {
            analysisName = MITBIHAnalysisName;
        }

        public Basic_Data SaveResult()
        {
            Basic_Data data = new Basic_Data();
            return data;
        }

        public void ConvertFile(string path)
        {
            pathIn = path;
        }
    }
}
