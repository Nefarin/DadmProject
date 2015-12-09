using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.HRV_DFA
{
    public class HRV_DFA_Params : ModuleParams
    {
        //HRV_DFA Module output parameters
        private int _paramAlpha;
        private uint _dfaNumberN;
        private uint _dfaValueFn;

        public int ParamAlpha { get; set; }
        public uint DfaNumberN { get; set; }
        public uint DfaValueFn { get; set; }

        public HRV_DFA_Params( int paramAlpha, uint dfaNumberN, uint dfaValueFn)
        {
            _paramAlpha = paramAlpha;
            _dfaNumberN = dfaNumberN;
            _dfaValueFn = dfaValueFn;
        }
       
    }
}
