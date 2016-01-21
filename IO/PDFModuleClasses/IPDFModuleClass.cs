using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.IO.PDFModuleClasses
{
    interface IPDFModuleClass
    {
        void FillReportForModule(string _header, Dictionary<String, String> _statsDictionary);

        void InsertStatisticsTable(Dictionary<string, string> _strToStr);      
    }
}
