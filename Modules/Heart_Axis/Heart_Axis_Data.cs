using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.Heart_Axis
{
    class Heart_Axis_Data : ECG_Data
    {
        //Heart_Axis Module output parameters
        private double _heartAxis;

        public double HeartAxis { get; set; }

        public Heart_Axis_Params(double heartAxis)
        {
            _heartAxis = heartAxis;
        }
    }
}
