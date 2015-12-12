using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Solvers;

namespace EKG_Project.Modules.Heart_Class
{
    public class Heart_Class_Params : ModuleParams
    {
        //HEART_CLASS Module input parameters - to podobno idzie kaj indziej
        //private Vector<double> _ecg;
        //private Vector<uint> _QRSonsets;
        //private Vector<uint> _QRSends;



        //HEART_CLASS Module output parameters
        private Vector<int> _qrsComplexLabel;
        private uint _totalNumberOfQrsComplex;
        private uint _numberOfClass;
        private double _percentOfNormalComplex;
        private Qrs_Class _cluster;

        public class Qrs_Class
        {
            private int _indexOfClass;
            private int _numberOfQrsComplex;
            private int _indexOfRepresentative;
            
            public int IndexOfClass
            {
                get
                {
                    return _indexOfClass;
                }

            }

            public int NumberOfQrsComplex
            {
                get
                {
                    return _numberOfQrsComplex;
                }

            }

            public int IndexOfRepresentative
            {
                get
                {
                    return _indexOfRepresentative;
                }

            }

        }

        /*
        public Vector<double> Ecg
        {
            get
            {
                return _ecg;
            }

            set
            {
                _ecg = value;
            }
        }
        

        public Vector<uint> QRSOnsets
        {
            get
            {
                return _QRSonsets;
            }

        }

        public Vector<uint> QRSEnds
        {
            get
            {
                return _QRSends;
            }

        }
        */

        public Vector<int> QrsComplexLabel
        {
            get
            {
                return _qrsComplexLabel;
            }

        }

        public uint TotalNumberOfQrsComplex
        {
            get
            {
                return _totalNumberOfQrsComplex;
            }

            set
            {
                _totalNumberOfQrsComplex = value;
            }

        }

        public uint NumberOfClass
        {
            get
            {
                return _numberOfClass;
            }

        }

        public double PercentOfNormalComplex
        {
            get
            {
                return _percentOfNormalComplex;
            }

        }




    }
}
