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
        void PoincarePlot( Vector<double> rr_intervals_x, Vector<double> rr_intervals_y) {

            Vector<double> RRIntervaals = InputData.RPeaks[_currentChannelIndex].Item2;
            rr_intervals_x = RRIntervaals.Subtract( RRIntervaals[0]);
            rr_intervals_y = RRIntervaals.Subtract( RRIntervaals.Last());

            //double mean_rr_intervals; //tu bd wyliczenie średniej

            //foreach (double i in RRIntervals)
            //{
            //    double std_power = std_power + (RRIntervaals(i) - mean_rr_intervals) * (RRIntervaals(i) - mean_rr_intervals);
            //}
            //double std_power2 = std_power / RRIntervaals.Length;
            //double std = Math.Sqrt(std_power2);

            //double SD1 = std(rr_intervals_x - rr_intervals_y) / Math.Sqrt(2);
            //double SD2 = std(rr_intervals_x + rr_intervals_y) / Math.Sqrt(2);
        }
    }
     * */
}
