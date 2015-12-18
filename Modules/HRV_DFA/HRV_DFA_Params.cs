using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.HRV_DFA
{

    public class HRV_DFA_Params : ModuleParams
    {
        //HRV_DFA parameters
        private double _breakpoint;
        private int _boxStart;
        private int _boxEnd;
        private int _stepSize;

        public HRV_DFA_Params(double breakpoint, int boxStart, int boxEnd, int stepSize)
        {
            _breakpoint = breakpoint;
            _boxStart = boxStart;
            _boxEnd = boxEnd;
            _stepSize = stepSize;
        }

        public double Breakpoint { get; set; }
        public int BoxStart { get; set; }
        public int BoxEnd { get; set; }
        public int StepSize { get; set; }

       
    }
}
