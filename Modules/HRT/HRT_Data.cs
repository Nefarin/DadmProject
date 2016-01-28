using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

/// <summary>
/// Data ma przechowywać moje dane - czyli co dostaję z pliku, na czym operuje, i co później mi analiza wypluwa
/// Data -> po przemyśleniu sprawy mają tam być dane, które moduł ma na wyjściu i ew. dane, które moduł tymczasowo potrzebuje
/// </summary>
namespace EKG_Project.Modules.HRT
{
    public class HRT_Data : ECG_Data
    {
        ///<Summary>_TurbulenceOnset- wartości TO</Summary>
        public Vector<double> _TurbulenceOnset { get; set; }

        ///<Summary>_TurbulenceSlope - wartości TS</Summary>
        public Vector<double> _TurbulenceSlope { get; set; }

        ///<Summary>_TurbulenceOnsetMean -średnia wartości TO, do zaznaczenia na wykresie</Summary>
        public double[] _TurbulenceOnsetMean { get; set; }

        ///<Summary>_TurbulenceSlopeMean - średnia wartość TS, do zaznaczenia na wykresie</Summary>
        public double[] _TurbulenceSlopeMean { get; set; }

        ///<Summary>_VPCtachogram - fragmenty tachogramu wokół VPC, do zaznaczenia na wykresie</Summary>
        public Vector<double> _VPCtachogram { get; set; }

        ///<Summary>_VPCcount - liczba znalezionych VPC</Summary>
        public int _VPCcount { get; set; }


        //konktruktor domyślny
        public HRT_Data() { }
    }
}