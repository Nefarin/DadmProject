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
        private Tuple<int, Vector<double>> _coefficientsForOneComplex;
        

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

        public Tuple<int, Vector<double>> CoefficientsForOneComplex
        {
            get { return _coefficientsForOneComplex; }
            set { _coefficientsForOneComplex = value; }
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
            _coefficientsForOneComplex = new Tuple<int, Vector<double>>(0,Vector<double>.Build.Dense(4));

        }
    }
}
