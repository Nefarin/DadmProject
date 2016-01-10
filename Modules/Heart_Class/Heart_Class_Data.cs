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
        private uint _totalNumberOfQrsComplex;
        private uint _numberOfClass;
        private double _percentOfNormalComplex;
        private Qrs_Class _cluster;


        public uint TotalNumberOfQrsComplex
        {
            get { return _totalNumberOfQrsComplex; }
            set { _totalNumberOfQrsComplex = value; }
        }

        public uint NumberOfClass
        {
            get { return _numberOfClass; }
            set { _numberOfClass = value; }
        }

        public double PercentOfNormalComplex
        {
            get { return _percentOfNormalComplex; }
            set { _percentOfNormalComplex = value; }
        }

        public List<Tuple<int, int>> ClassificationResult
        {
            get { return _classificationResult; }
            set { _classificationResult = value; }
        }

        public bool ChannelMliiDetected
        {
            get { return _channelMLIIDetected; }
            set { _channelMLIIDetected = value; }
        }


        public class Qrs_Class
        {
            private int _indexOfClass;
            private int _numberOfQrsComplex;
            private int _indexOfRepresentative;

            public int IndexOfClass
            {
                get { return _indexOfClass; }

            }

            public int NumberOfQrsComplex
            {
                get{ return _numberOfQrsComplex; }
            }

            public int IndexOfRepresentative
            {
                get  { return _indexOfRepresentative; }
            }

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
        }
    }
}
