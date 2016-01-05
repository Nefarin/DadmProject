using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.Flutter
{
    class Flutter_Data : ECG_Data
    {
        /// <summary>
        /// Lista krotek wskazujących trzepotanie przedsionków,
        /// krotka <int, int> - <początek, koniec> afl
        /// </summary>
        public List<Tuple<int, int>> FlutterAnnotations { get; set; }
    }
}
