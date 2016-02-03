
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.ST_Segment
{
    public class ST_Segment_Data : ECG_Data
    {
        private int[] _st_shapes;
        private Vector<int> _tJ;
        private Vector<int> _tST;

        public ST_Segment_Data()
        {
         
        }

        public int[] St_shapes
        {
            get
            {
                return _st_shapes;
            }

            set
            {
                _st_shapes = value;
            }
        }

        public Vector<int> TJ
        {
            get
            {
                return _tJ;
            }

            set
            {
                _tJ = value;
            }
        }

        public Vector<int> TST
        {
            get
            {
                return _tST;
            }

            set
            {
                _tST = value;
            }
        }
    }
}







