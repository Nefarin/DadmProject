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

        private double _triangleIndex;
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

        private double _sd1;
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

        private double _sd2;
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

        private List<double> _histogramData;
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

        private List<double> _poincarePlotData;
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
