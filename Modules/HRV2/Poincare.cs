using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;


namespace EKG_Project.Modules.HRV2
{

    public partial class HRV2_Alg

    {
        private Vector<double> RR_intervals_x;
        private Vector<double> RR_intervals_y;

        #region Documentation
        /// <summary>
        /// Makes vector of x coefficients (R(i))
        /// which are used in Poincare plot visualisation 
        /// </summary>
        /// 
        #endregion
        public void PoincarePlot_x()
        {
            Vector<double> rr_intervals_x = Vector<double>.Build.Dense(_rrIntervals.Count - 1);
            rr_intervals_x = _rrIntervals.SubVector(1, _rrIntervals.Count - 1);
            RR_intervals_x = rr_intervals_x;
        }
        #region Documentation
        /// <summary>
        /// Makes vector of y coefficients (R(i+1)) 
        /// which are used in Poincare plot visualisation 
        /// </summary>
        /// 
        #endregion
        public void PoincarePlot_y()
        {
            Vector<double> rr_intervals_y = Vector<double>.Build.Dense(_rrIntervals.Count - 1);
            rr_intervals_y = _rrIntervals.SubVector(0, _rrIntervals.Count - 1);
            RR_intervals_y = rr_intervals_y;
        }

        #region Documentation
        /// <summary>
        /// Get the standard deviation 
        /// </summary>
        /// 
        #endregion
        private double getStandardDeviation(Vector<double> dataVector)
        {
            double average = dataVector.Average();
            double sumOfDerivation = 0;
            foreach (double value in dataVector)
            {
                sumOfDerivation += (value) * (value);
            }
            double sumOfDerivationAverage = sumOfDerivation / (dataVector.Count);
            return Math.Sqrt(sumOfDerivationAverage - (average * average));
        }

        #region Documentation
        /// <summary>
        /// Returns a <double> SD1 coefficient, the shorter axis of the ellipse
        /// </summary>
        /// 
        #endregion
        public double SD1()
        {
            double SD1 = getStandardDeviation(RR_intervals_x.Subtract(RR_intervals_y)) / Math.Sqrt(2);

            return SD1;
        }
        #region Documentation
        /// <summary>
        /// Returns a <double> SD2 coefficient, the longer axis of the ellipse
        /// </summary>
        /// 
        #endregion
        public double SD2()
        {
            double SD2 = getStandardDeviation(RR_intervals_x.Add(RR_intervals_y)) / Math.Sqrt(2);

            return SD2;
        }

        #region Documentation
        /// <summary>
        /// Returns a Vector <double> center (x,y) of an elipse fitted to Poincare plot 
        /// </summary>
        /// 
        #endregion
        public double elipseCenter_x()
        {
            double elipseCenter_x = RR_intervals_x.Average();
            return elipseCenter_x;
        }
        public double elipseCenter_y()
        {
            double elipseCenter_y = RR_intervals_x.Average();
            return elipseCenter_y;
        }
    }
}




