using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.Heart_Axis
{
    public class Heart_Axis_Data : ECG_Data
    {
        /// <summary>
        /// Class that includes output results of class Heart_Axis
        /// </summary>

        private double _heartAxis;

        public double HeartAxis
        {
            get
            {
                return _heartAxis;
            }
            set
            {
                _heartAxis = value;
            }
        }
    }
}
