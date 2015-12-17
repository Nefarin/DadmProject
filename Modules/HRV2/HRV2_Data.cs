using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.HRV2
{
    class HRV2_Data : IO.ECG_Data
    {
        private double _tinn;
        private double _triangleIndex;
        private double _sd1;
        private double _sd2;
        private List<double> _histogramData; //nie do konca wiemy czy to bd lista double
        private List<double> _poincarePlotData; //nie do konca wiemy czy to bd lista double

        public double Tinn
        {
            get
            {
                return _tinn;
            }

            set
            {
                _tinn = value;
            }
        }

        public double TriangleIndex
        {
            get
            {
                return _triangleIndex;
            }

            set
            {
                _triangleIndex = value;
            }
        }

        public double SD1
        {
            get
            {
                return _sd1;
            }

            set
            {
                _sd1 = value;
            }
        }

        public double SD2
        {
            get
            {
                return _sd2;
            }

            set
            {
                _sd2 = value;
            }
        }

        public List<double> HistogramData
        {
            get
            {
                return _histogramData;
            }

            set
            {
                _histogramData = value;
            }
        }

        public List<double> PoincarePlotData
        {
            get
            {
                return _poincarePlotData;
            }

            set
            {
                _poincarePlotData = value;
            }
        }
    }
}
