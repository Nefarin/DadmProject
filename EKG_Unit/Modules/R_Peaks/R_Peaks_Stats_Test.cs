using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.R_Peaks;
using EKG_Project.Modules;
using EKG_Project.IO;
using System.Diagnostics;


namespace EKG_Unit.Modules.R_Peaks
{
    [TestClass]
    public class R_Peaks_Stats_Test
    {
        [TestMethod]
        [Description("Test if stats does not throws any expections during execution")]
        public void statsTest()
        {
            IModule testModule = new EKG_Project.Modules.R_Peaks.R_Peaks();
            R_Peaks_Method method = R_Peaks_Method.PANTOMPKINS;
            R_Peaks_Params param = new R_Peaks_Params(method, "x");

            R_Peaks_Stats stats = new R_Peaks_Stats();
            stats.Init("x");
            while (true)
            {
                if (stats.Ended()) break;
                stats.ProcessStats();
            }
        }
    }
}
