using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EKG_Project.Modules.Heart_Cluster
{
    public class Heart_Cluster_Data : ECG_Data
    {
        //private uint _totalNumberOfQrsComplex;
        //private uint _numberOfClass;
        //private double _percentOfNormalComplex;
        private Qrs_Class _cluster; 

        public uint TotalNumberOfQrsComplex { get; set; }

        public uint NumberOfClass { get; set; }

        public double PercentOfNormalComplex { get; set; }

        public bool ChannelMliiDetected { get; set; }


        public class Qrs_Class
        {
            public int IndexOfClass { get; }

            public int NumberOfQrsComplex { get; }

            public int IndexOfRepresentative { get; }
        }
    }
}
