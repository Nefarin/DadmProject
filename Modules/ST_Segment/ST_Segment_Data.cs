
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.ST_Segment
{
    public class ST_Segment_Data
    {
        public ST_Segment_Data ()
        {
            tJs = new List<long>();
            tSTs = new List<long>();
        }

        public List<long> tJs { get; set; }
        public List<long> tSTs { get; set; }
        public int ConcaveCurves { get; set; }
        public int ConvexCurves { get; set; }
        public int IncreasingLines { get; set; }
        public int HorizontalLines { get; set; }
        public int DecreasingLines { get; set; }
    }
}









