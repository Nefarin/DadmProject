using System;
using System.Collections.Generic;
using EKG_Project.IO;

namespace EKG_Project.Modules.Atrial_Fibr
{
   class Atrial_Fibr_Data : ECG_Data
    {
        private List<Tuple<bool, int[],string, string>> _afDetection;   

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

        public Atrial_Fibr_Data()
        {
            AfDetection = new List<Tuple<bool, int[], string, string>>();
        }
    }
}
