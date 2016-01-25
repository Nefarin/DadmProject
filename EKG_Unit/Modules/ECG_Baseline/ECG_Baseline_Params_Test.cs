using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.ECG_Baseline;

namespace EKG_Unit.Modules.ECG_Baseline
{
    [TestClass]
    public class ECG_Baseline_Params_Test
    {
        [TestMethod]
        [Description("Test if default constructor works properly")]
        public void defaultConstructorTest()
        {
           ECG_Baseline_Params param = new ECG_Baseline_Params();
           Assert.AreEqual (Filtr_Method.BUTTERWORTH, param.Method);
           Assert.AreEqual(Filtr_Type.BANDPASS, param.Type);
           Assert.AreEqual(50, param.FcLow);
           Assert.AreEqual(0.5, param.FcHigh);
           Assert.AreEqual(3, param.OrderLow);
           Assert.AreEqual(3, param.OrderHigh);
        }

        [TestMethod]
        [Description("Test if default constructor works properly")]
        public void BandPassConstructorForLMSMovingAverageSavitzkyGolayTest()
        {
            ECG_Baseline_Params expectedParams = new ECG_Baseline_Params();
            expectedParams.Method = Filtr_Method.LMS;
            expectedParams.AnalysisName = "analysis";
            expectedParams.Type = Filtr_Type.BANDPASS;
            expectedParams.WindowSizeLow = 10;
            expectedParams.WindowSizeHigh = 100;

            Assert.AreEqual(Filtr_Method.LMS, expectedParams.Method);
            Assert.AreEqual("analysis", expectedParams.AnalysisName);
            Assert.AreEqual(Filtr_Type.BANDPASS, expectedParams.Type);
            Assert.AreEqual(10, expectedParams.WindowSizeLow);
            Assert.AreEqual(100, expectedParams.WindowSizeHigh);
        }

    }
}
