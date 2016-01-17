using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.ST_Segment
{
    public class ST_Segment_Params : ModuleParams
    {
        public ST_Segment_Params(string analysisName)
        {
            this.AnalysisName = analysisName;
        }
        public int RpeaksStep { get; internal set; }
    }
}
