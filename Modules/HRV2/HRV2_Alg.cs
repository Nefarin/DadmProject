using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using EKG_Project.Modules.R_Peaks;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.IO;
using System.Linq;

namespace EKG_Project.Modules.HRV2
{
    public partial class HRV2_Alg
    {

        public HRV2_Alg() { }
        public HRV2_Alg(Vector<double> signal)
        {
            RRIntervals = signal;
        }
        private Vector<double> _rrIntervals;
        private double _histogramCount;
        #region Documentation
        /// <summary>
        /// Sets and gets RR intervals
        /// </summary>
        /// 
        #endregion
        public Vector<double> RRIntervals
        {
            set
            {
                _rrIntervals = value;
            }
            get
            {
                return _rrIntervals;
            }
        }
        
        public double histogramCount
        {
             set
            {
                _histogramCount = value;
            }
            get
            {
                return _histogramCount;
            }
           
         }

        #region Documentation
        /// <summary>
        /// This function returns RR intervals vector after interpolation
        /// </summary>
        /// 
        #endregion
        public void Interpolation()
        {
            for (int i = 1; i < _rrIntervals.Count; i++)
            {
                if (_rrIntervals[i] > 2*_rrIntervals.Average())
                {
                    _rrIntervals[i] = _rrIntervals[i - 1];
                    i++;
                }
                else
                {
                    i++;
                }
            }
        }
    }
}