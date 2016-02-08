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
        //output:
        private List<Tuple<int, int, int, int>> _clusterizationResult;
        private Qrs_Out _out; // do zmiany na atrybuty?
        private Qrs_Class _cluster; // do zmiany na atrybuty?

        public List<Tuple<int, int, int, int>> clusterizationResult
        {
            get { return _clusterizationResult; }
            set { _clusterizationResult = value; }
        }

        public bool ChannelMliiDetected { get; set; }


        public class Qrs_Out
        {
            public int TotalQRSComplex { get; }

            public int NumberofClass { get; }
        }
        public class Qrs_Class
        {
            public int IndexOfClass { get; }

            public int QRSComplexNo { get; }
        }
    }
}
