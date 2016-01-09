using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EKG_Project.Modules.Heart_Class
{
    public class Heart_Class_Data : ECG_Data
    {

        //output:
        private List<Tuple<int, int>> _classificationResult;
        private bool _channelMLIIDetected;

        //niezrealizowane:
        private Qrs_Class _cluster;


        public uint TotalNumberOfQrsComplex { get; set; }

        public uint NumberOfClass { get; set; }

        public double PercentOfNormalComplex { get; set; }

        public List<Tuple<string, List<Tuple<int, int>>>> ClassificationResult { get; set; }

        public List<Tuple<int, int>> QrsComplexLabel { get; set; }

        public bool ChannelMliiDetected
        {
            get { return _channelMLIIDetected; }
            set { _channelMLIIDetected = value; }
        }


        public class Qrs_Class
        {
            public int IndexOfClass { get; }

            public int NumberOfQrsComplex { get; }

            public int IndexOfRepresentative { get; }
            }

        #region Documentation
        /// <summary>
        /// Not parameterized constructor of Heart_Class_Data.cs data class
        /// </summary>
        #endregion
        public Heart_Class_Data()
        {
            _classificationResult = new List<Tuple<int, int>>();
            ChannelMliiDetected = new bool();
            ClassificationResult = new List<Tuple<string, List<Tuple<int, int>>>>();
        }
    }
}
