using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.IO.PDF
{
    public class StoreDataPDF
    {
        public string AnalisysName { get; set; }
        public System.Collections.Generic.List<string> ModuleList { get; set; }
        public string Filename { get; set; }
        public GUI.AvailableOptions ModuleOption { get; set; }
        public Dictionary<String, String> statsDictionary { get; set; }

        public StoreDataPDF()
        {
            AnalisysName = "Unnamed Analisys";
            ModuleList = new System.Collections.Generic.List<string>();
            ModuleList.Clear();
            Filename = "";
            ModuleOption = 0;
            statsDictionary = new Dictionary<string, string>();
        }
    }
}
