using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.TestModule2
{
    public enum Fibonacci_Mode { RECURSIVE, ITERATIVE };

    public class Test_Module2_Param : ModuleParams
    {
        private Fibonacci_Mode _mode;

        public Fibonacci_Mode Mode
        {
            get
            {
                return _mode;
            }

            set
            {
                _mode = value;
            }
        }

        public Test_Module2_Param(Fibonacci_Mode mode)
        {
            this.Mode = mode;
        }
    }
}
