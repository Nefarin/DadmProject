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

        public double Breakpoint
        {
            get
            {
                return _breakpoint;
            }

            set
            {
                _breakpoint = value;
            }
        }

        public int BoxStart
        {
            get
            {
                return _boxStart;
            }

            set
            {
                _boxStart = value;
            }
        }

        public int BoxEnd
        {
            get
            {
                return _boxEnd;
            }

            set
            {
                _boxEnd = value;
            }
        }

        public int StepSize
        {
            get
            {
                return _stepSize;
            }

            set
            {
                _stepSize = value;
            }
        }

        public HRV_DFA_Params(double breakpoint, int boxStart, int boxEnd, int stepSize)
          {
              Breakpoint = breakpoint;
              BoxStart = boxStart;
              BoxEnd = boxEnd;
              StepSize = stepSize;

          }

       
    }
}
