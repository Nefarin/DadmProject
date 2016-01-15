using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.T_Wave_Alt
{
    public class T_Wave_Alt_Params : ModuleParams
    {
        private uint _fs;

        public T_Wave_Alt_Params(uint fs)
        {
            _fs = fs;
        }

        public uint Fs
        {
            get
            {
                return _fs;
            }
        }
    }

    // de facto w naszym module nie ma do podania dodatkowych informacji z GUI...
}
