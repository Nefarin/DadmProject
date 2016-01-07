using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;


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




    



