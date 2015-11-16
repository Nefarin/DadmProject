using EKG_Project.Modules;
using System.Collections.Generic;

namespace EKG_Project.Architecture
{
    public class Modules
    {
        public List<Module> ECG_Modules;

        public Modules(SortedList<Module, ModuleParams> modulesList)
        {
            foreach (KeyValuePair<Module, ModuleParams> modulesData in modulesList)
            {
                Module module = modulesData.Key;
                ModuleParams parameters = modulesData.Value;
                module.Init(parameters);
                ECG_Modules.Add(module);
            }
        }

        public ECG_Baseline Get_ECG_Baseline
        {
            get
            {
                return (ECG_Baseline) ECG_Modules[0];
            }
        }

        public R_Peaks Get_R_Peaks
        {
            get
            {
                return (R_Peaks) ECG_Modules[1];
            }
        }

        public TestModule Get_TestModule
        {
            get
            {
                return (TestModule) ECG_Modules[2];
            }
        }
    }
}
