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
        Vector<double> rr_intervals_x = RRIntervaals - RRIntervaals.First;
        Vector<double> rr_intervals_y = RRIntervaals - RRIntervals.Last;

        double mean_rr_intervals; //tu bd wyliczenie średniej

        foreach (double i in RRIntervals) 
            {
                double std_power = std_power + (RRIntervaals(i) - mean_rr_intervals) * (RRIntervaals(i) - mean_rr_intervals);
            }
        double std_power2 = std_power / RRIntervaals.Length;
        double std = Math.Sqrt(std_power2);

        double SD1 = std(rr_intervals_x - rr_intervals_y) / Math.Sqrt(2);
        double SD2 = std(rr_intervals_x + rr_intervals_y) / Math.Sqrt(2);
    }
}
