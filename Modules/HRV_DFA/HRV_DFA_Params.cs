using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.HRV_DFA
{
    public enum Plot_Breakpoint { ONE_THIRD, HALF, TWO_THIRD };
    public enum Box_Max_Size { _10000,_50000, _100000};
    public enum Box_StepSize { _10,_50, _100};


    public class HRV_DFA_Params : ModuleParams
    {
        //HRV_DFA parameters
        private Plot_Breakpoint _breakpoint;
        private Box_Max_Size _boxSize;
        private Box_StepSize _stepSize;

        public HRV_DFA_Params(Plot_Breakpoint breakpoint, Box_Max_Size boxSize, Box_StepSize stepSize)
        {
            this.Breakpoint = breakpoint;
            this.BoxSize = boxSize;
            this.StepSize = stepSize;
        }

        public Plot_Breakpoint Breakpoint
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

        public Box_Max_Size BoxSize
        {
            get
            {
                return _boxSize;
            }

            set
            {
                _boxSize = value;
            }
        }


        public Box_StepSize StepSize
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

        public void CopyParametersFrom(HRV_DFA_Params parameters)
        {
            this.Breakpoint = parameters.Breakpoint;
            this.BoxSize = parameters.BoxSize;
            this.StepSize = parameters.StepSize;
        
        }


    }
}
