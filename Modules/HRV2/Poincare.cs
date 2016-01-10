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
        void PoincarePlot(Vector<double> rr_intervals_x, Vector<double> rr_intervals_y)
        {
            Vector<double> RRIntervaals = InputData.RPeaks[_currentChannelIndex].Item2;
            rr_intervals_x = RRIntervaals.Subtract(RRIntervaals[0]);
            rr_intervals_y = RRIntervaals.Subtract(RRIntervaals.Last());
            
            double SD1 = getStandardDeviation(rr_intervals_x.Subtract(rr_intervals_y)) / Math.Sqrt(2);
            double SD2 = getStandardDeviation(rr_intervals_x.Add(rr_intervals_y)) / Math.Sqrt(2);
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
    }
    }
    


