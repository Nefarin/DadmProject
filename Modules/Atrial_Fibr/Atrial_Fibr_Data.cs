using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.Atrial_Fibr
{
    class Atrial_Fibr_Data : ECG_Data
    {
        private List<Tuple<bool, int[],string, string>> _afDetection;   
        public Atrial_Fibr_Data() { }

        public List<Tuple<bool, int[], string, string>> AfDetection
        {
            get
            {
                return _afDetection;
            }
            set
            {
                _afDetection = value;
            }
        }
    }
}
