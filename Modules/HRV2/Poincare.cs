using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;


namespace EKG_Project.Modules.HRV2
{
    /*
    public partial class HRV2 : IModule

    {
        private Vector<double> RR_intervals_x;
        private Vector<double> RR_intervals_y;

        private void PoincarePlot_x ()
        {
            Vector<double> RRIntervals = InputData.RRInterval[_outputIndex].Item2.Clone();
            Vector<double> rr_intervals_x = Vector<double>.Build.Dense(RRIntervals.Count - 1);
            rr_intervals_x = RRIntervals.SubVector(1, RRIntervals.Count - 1);
            //Console.WriteLine(rr_intervals_x.Count);
            RR_intervals_x = rr_intervals_x;
        }

        private void PoincarePlot_y()
        {
            Vector<double> RRIntervaals = InputData.RRInterval[_outputIndex].Item2.Clone();
            Vector<double> rr_intervals_y = Vector<double>.Build.Dense(RRIntervaals.Count - 1);
            rr_intervals_y = RRIntervaals.SubVector(0, RRIntervaals.Count - 1);
            //Console.WriteLine(rr_intervals_y.Count);
            RR_intervals_y = rr_intervals_y;
        }
        private double getStandardDeviation(Vector<double> dataVector)
        {
            double average = dataVector.Average();
            double sumOfDerivation = 0;
            foreach (double value in dataVector)
            {
                sumOfDerivation += (value) * (value);
            }
            double sumOfDerivationAverage = sumOfDerivation / (dataVector.Count - 1);
            return Math.Sqrt(sumOfDerivationAverage - (average * average));
        }

        private double SD1()
        {
            double SD1 = getStandardDeviation(RR_intervals_x.Subtract(RR_intervals_y)) / Math.Sqrt(2);

            return SD1;
        }

        private double SD2()
        {
            double SD2 = getStandardDeviation(RR_intervals_x.Add(RR_intervals_y)) / Math.Sqrt(2);

            return SD2;
        }
    }
   */  
}
    


