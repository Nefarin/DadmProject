using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;


namespace EKG_Project.Modules.HRV2
{
    public partial class HRV2 : IModule

    {
        private Vector<double> RR_intervals_x;
        private Vector<double> RR_intervals_y;

        private Vector<double> PoincarePlot_x ()
        {
            Vector<double> RRIntervaals = InputData.RPeaks[_currentChannelIndex].Item2;
            Vector<double> rr_intervals_x = RRIntervaals.Subtract(RRIntervaals[0]);
            return rr_intervals_x;
        }

        private Vector<double> PoincarePlot_y()
        {
            Vector<double> RRIntervaals = InputData.RPeaks[_currentChannelIndex].Item2;
            Vector<double> rr_intervals_y = RRIntervaals.Subtract(RRIntervaals.Last());
            return rr_intervals_y;
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
            RR_intervals_x = PoincarePlot_x();
            RR_intervals_y = PoincarePlot_y();
            double SD1 = getStandardDeviation(RR_intervals_x.Subtract(RR_intervals_y)) / Math.Sqrt(2);

            return SD1;
        }

        private double SD2()
        {
            RR_intervals_x = PoincarePlot_x();
            RR_intervals_y = PoincarePlot_y();
            double SD2 = getStandardDeviation(RR_intervals_x.Add(RR_intervals_y)) / Math.Sqrt(2);

            return SD2;
        }
    }
}
    


