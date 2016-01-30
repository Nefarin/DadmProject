using System;
using System.Collections.Generic;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.Atrial_Fibr
{
   public class Atrial_Fibr_Data : ECG_Data
    {
        private List<Tuple<bool, Vector<double>,string, string>> _afDetection;   

        public List<Tuple<bool, Vector<double>, string, string>> AfDetection
        {
            get
            {
                return _afDetection;
            }
            set
            {
                _afDetection = value;
            }
        }

        #region Documentation
        /// <summary>
        /// TO DO
        /// </summary>
        #endregion
        public Atrial_Fibr_Data()
        {
            AfDetection = new List<Tuple<bool, Vector<double>, string, string>>();
        }

    }
}
