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
    public class HRT_Data : IO.ECG_Data
    {

        private Vector<double> _TurbulenceOnset;
        private Vector<double> _TurbulenceSlope;
        private double[] _TurbulenceOnsetMean;
        private double[] _TurbulenceSlopeMean;
        private double[,] _VPCtachogram;
        private int _VPCcount;

        ///<Summary>_TurbulenceOnset- wartości TO</Summary>
        public Vector<double> TurbulenceOnset
        {
            get
            {
                return _TurbulenceOnset;
            }

            set
            {
                this._TurbulenceOnset = value;
            }
        }

        ///<Summary>_TurbulenceSlope - wartości TS</Summary>
        public Vector<double> TurbulenceSlope
        {
            get
            {
                return _TurbulenceSlope;
            }

            set
            {
                this._TurbulenceSlope = value;
            }
        }


        ///<Summary>_TurbulenceOnsetMean -średnia wartości TO, do zaznaczenia na wykresie</Summary>
        public double[] TurbulenceOnsetMean
        {
            get
            {
                return _TurbulenceOnsetMean;
            }

            set
            {
                _TurbulenceOnsetMean = value;
            }
        }

        ///<Summary>_TurbulenceSlopeMean - średnia wartość TS, do zaznaczenia na wykresie</Summary>
        public double[] TurbulenceSlopeMean
        {
            get
            {
                return _TurbulenceSlopeMean;
            }

            set
            {
                _TurbulenceSlopeMean = value;
            }
        }

        ///<Summary>_VPCtachogram - fragmenty tachogramu wokół VPC, do zaznaczenia na wykresie</Summary>
        public double[,] VPCtachogram
        {
            get
            {
                return _VPCtachogram;
            }

            set
            {
                _VPCtachogram = value;
            }
        }

        ///<Summary>_VPCcount - liczba znalezionych VPC</Summary>
        public int VPCcount
        {
            get
            {
                return _VPCcount;
            }

            set
            {
                _VPCcount = value;
            }
        }

        //konktruktor domyślny
        public HRT_Data() { }
    }
}
