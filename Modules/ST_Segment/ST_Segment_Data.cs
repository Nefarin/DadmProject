
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.ST_Segment
{
    public class StAnalysisResult
    {
        public StAnalysisResult()
        {
            tJs = new List<int>();
            tSTs = new List<int>();
        }

        public List<int> tJs { get; set; }
        public List<int> tSTs { get; set; }
        public int ConcaveCurves { get; set; }
        public int ConvexCurves { get; set; }
        public int IncreasingLines { get; set; }
        public int HorizontalLines { get; set; }
        public int DecreasingLines { get; set; }
    }
}








