using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.Waves
{
    public class Waves_Data : ECG_Data
    {

        private List<Tuple<string, List<int>>> _QRSonsets;
        private List<Tuple<string, List<int>>> _QRSends;
        private List<Tuple<string, List<int>>> _Ponsets;
        private List<Tuple<string, List<int>>> _Pends;
        private List<Tuple<string, List<int>>> _Tends;

        public Waves_Data() {

            _QRSends = new List<Tuple<string, List<int>>>();
            _QRSonsets = new List<Tuple<string, List<int>>>();
            _Pends = new List<Tuple<string, List<int>>>();
            _Ponsets = new List<Tuple<string, List<int>>>();
            _Tends = new List<Tuple<string, List<int>>>();
        }

        public List<Tuple<string, List<int>>> QRSOnsets
        {
            get
            {
                return _QRSonsets;
            }
            set
            {
                _QRSonsets = value;
            }
        }

        public List<Tuple<string, List<int>>> QRSEnds
        {
            get
            {
                return _QRSends;
            }
            set
            {
                _QRSends = value;
            }
        }

        public List<Tuple<string, List<int>>> POnsets
        {
            get
            {
                return _Ponsets;
            }
            set
            {
                _Ponsets = value;
            }

        }

        public List<Tuple<string, List<int>>> PEnds
        {
            get
            {
                return _Pends;
            }

            set
            {
                _Pends = value;
            }
        }

        public List<Tuple<string, List<int>>> TEnds
        {
            get
            {
                return _Tends;
            }
            set
            {
                _Tends = value;
            }
        }

    }
}
