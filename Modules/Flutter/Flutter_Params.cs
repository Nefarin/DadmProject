using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.Flutter
{
    public class Flutter_Params : ModuleParams
    {
        //FLUTTER Module output parameters
        private uint _paramAFl;
        private int _indexAFl;
        private int _valueAFl;

        public uint ParamAFl { get; set; }
        public int IndexAfl { get; set; }
        public int ValueAFl { get; set; }

        public Flutter_Params(uint paramAFl, int indexAFl, int valueAFl)
        {
            _paramAFl = paramAFl;
            _indexAFl = indexAFl;
            _valueAFl = valueAFl;
        }
    }
}
