using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.Modules.ECG_Baseline;
using EKG_Project.Modules.Waves;
using EKG_Project.Modules.R_Peaks;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;


namespace EKG_Project.Modules.ST_Segment
{
    public class ST_Segment_Params : ModuleParams
    {
        private int _pointJ;
        private int _pointST;
        private int _pointBL;
        private int _nachylenie;
        private float _odleglosc;

        public int PointJ { get; set; }
        public int PointST { get; set; }
        public int PointBL { get; set; }
        public int Nachylenie { get; set; }
        public float Odleglosc { get; set; }

        public ST_Segment_Params(int pointJ, int pointST, int pointBL, int nachylenie, float odleglosc)
        {
            _pointJ = pointJ;
            _pointST = pointST;
            _pointBL = pointBL;
            _nachylenie = nachylenie;
            _odleglosc = odleglosc;
        }
    }
}


        public ST_Segment_Params(int pointJ, int pointST, int pointBL, int nachylenie, float odleglosc)
        {
            _pointJ = pointJ;
            _pointST = pointST;
            _pointBL = pointBL;
            _nachylenie = nachylenie;
            _odleglosc = odleglosc;
        }
    }
}

namespace EKG_Project.Modules.ST_Segment
{
    class ST_Segment_Data : ECG_Data
    {
        private uint _frequency;
        private uint _dJ;
        private uint _dST; // czy musze definowac wszystkie zeminne ktorych uzyje ja a nie gui ?


        private List<Tuple<string, Vector<double>>> _J;   /// Wektor początków odcinka ST,położenia J (nr próbki i jej wartość)
        private List<Tuple<string, Vector<double>>> _ST;     /// Wektor końców odcinka ST,położenia ST (nr próbki i jej wartość)
            // nie powinno być List<Tuple<double,double>>> _J; ? bo nie jestem pewna

        private string>>> _diagnoza;    /// zwróci informacje tekstową o jednym z 6 szablonów odcinka (normlany, obniżenie równoległe, obniżenie skośne, obniżenie miseczkowate, uniesienie wklęsłe, uniesienie wypukłe
        
            public ST_Segment_Data() { }

        public List<Tuple<string, Vector<double>>> J
        {
            get
            {
                return _J;
            }
            set
            {
                _J = value;
            }
        }


        public List<Tuple<string, Vector<double>>> ST
        {
            get
            {
                return _ST;
            }
            set
            {
                _ST = value;
            }
        }


        public List<Tuple<string, Vector<double>>> diagnoza   //tu raczej nie powinno być List<Tuple ?
        {
            get
            {
                return _diagnoza;
            }
            set
            {
                _diagnoza = value;
            }
        }

    }




    



