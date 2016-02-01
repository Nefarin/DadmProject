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
           Assert.AreEqual(2, param.FcHigh);
           Assert.AreEqual(30, param.OrderLow);
           Assert.AreEqual(30, param.OrderHigh);
        }
        [TestMethod]
        [Description("Test if Butterworth LP constructor works properly")]
        public void ButterworthLPConstructorTest()
        {
            ECG_Baseline_Params param = new ECG_Baseline_Params("Analysis1", Filtr_Method.BUTTERWORTH, Filtr_Type.LOWPASS, 5, 50);
            Assert.AreEqual(Filtr_Method.BUTTERWORTH, param.Method);
            Assert.AreEqual(Filtr_Type.LOWPASS, param.Type);
            Assert.AreEqual(50, param.FcLow);
            Assert.AreEqual(5, param.OrderLow);
            Assert.AreEqual("Analysis1", param.AnalysisName);
        }
        [TestMethod]
        [Description("Test if Butterworth HP constructor works properly")]
        public void ButterworthHPConstructorTest()
        {
            ECG_Baseline_Params param = new ECG_Baseline_Params("Analysis1", Filtr_Method.BUTTERWORTH, Filtr_Type.HIGHPASS, 5, 5);
            Assert.AreEqual(Filtr_Method.BUTTERWORTH, param.Method);
            Assert.AreEqual(Filtr_Type.HIGHPASS, param.Type);
            Assert.AreEqual(5, param.FcHigh);
            Assert.AreEqual(5, param.OrderHigh);
            Assert.AreEqual("Analysis1", param.AnalysisName);
        }
        [TestMethod]
        [Description("Test if Butterworth BP constructor works properly")]
        public void ButterworthBPConstructorTest()
        {
            ECG_Baseline_Params param = new ECG_Baseline_Params(Filtr_Method.BUTTERWORTH, Filtr_Type.BANDPASS, 10, 10, 50, 5, "Analysis1");
            Assert.AreEqual(Filtr_Method.BUTTERWORTH, param.Method);
            Assert.AreEqual(Filtr_Type.BANDPASS, param.Type);
            Assert.AreEqual(50, param.FcLow);
            Assert.AreEqual(10, param.OrderLow);
            Assert.AreEqual(5, param.FcHigh);
            Assert.AreEqual(10, param.OrderHigh);
            Assert.AreEqual("Analysis1", param.AnalysisName);
        }
        [TestMethod]
        [Description("Test if MovingAvg LowPass constructor works properly")]
        public void MovingAvgLPConstructorTest()
        {
            ECG_Baseline_Params param = new ECG_Baseline_Params(Filtr_Method.MOVING_AVG, Filtr_Type.LOWPASS, 5, "Analysis1");
            Assert.AreEqual(Filtr_Method.MOVING_AVG, param.Method);
            Assert.AreEqual(Filtr_Type.LOWPASS, param.Type);
            Assert.AreEqual(5, param.WindowSizeLow);
            Assert.AreEqual("Analysis1", param.AnalysisName);
        }
        [TestMethod]
        [Description("Test if MovingAvg HighPass constructor works properly")]
        public void MovingAvgHPConstructorTest()
        {
            ECG_Baseline_Params param = new ECG_Baseline_Params(Filtr_Method.MOVING_AVG, Filtr_Type.HIGHPASS, 90, "Analysis1");
            Assert.AreEqual(Filtr_Method.MOVING_AVG, param.Method);
            Assert.AreEqual(Filtr_Type.HIGHPASS, param.Type);
            Assert.AreEqual(90, param.WindowSizeHigh);
            Assert.AreEqual("Analysis1", param.AnalysisName);
        }
        [TestMethod]
        [Description("Test if Savitzky-Golay LowPass constructor works properly")]
        public void SavGolLPConstructorTest()
        {
            ECG_Baseline_Params param = new ECG_Baseline_Params(Filtr_Method.SAV_GOL, Filtr_Type.LOWPASS, 5, "Analysis1");
            Assert.AreEqual(Filtr_Method.SAV_GOL, param.Method);
            Assert.AreEqual(Filtr_Type.LOWPASS, param.Type);
            Assert.AreEqual(5, param.WindowSizeLow);
            Assert.AreEqual("Analysis1", param.AnalysisName);
        }
        [TestMethod]
        [Description("Test if Savitzky-Golay HighPass constructor works properly")]
        public void SavGolHPConstructorTest()
        {
            ECG_Baseline_Params param = new ECG_Baseline_Params(Filtr_Method.SAV_GOL, Filtr_Type.HIGHPASS, 90, "Analysis1");
            Assert.AreEqual(Filtr_Method.SAV_GOL, param.Method);
            Assert.AreEqual(Filtr_Type.HIGHPASS, param.Type);
            Assert.AreEqual(90, param.WindowSizeHigh);
            Assert.AreEqual("Analysis1", param.AnalysisName);
        }
        [TestMethod]
        [Description("Test if LMS LowPass constructor works properly")]
        public void LmsLPConstructorTest()
        {
            ECG_Baseline_Params param = new ECG_Baseline_Params(Filtr_Method.LMS, Filtr_Type.LOWPASS, 5, "Analysis1", 0.5);
            Assert.AreEqual(Filtr_Method.LMS, param.Method);
            Assert.AreEqual(Filtr_Type.LOWPASS, param.Type);
            Assert.AreEqual(5, param.WindowLMS);
            Assert.AreEqual("Analysis1", param.AnalysisName);
            Assert.AreEqual(0.5, param.Mi);
        }
        [TestMethod]
        [Description("Test if LMS HighPass constructor works properly")]
        public void LmsHPConstructorTest()
        {
            ECG_Baseline_Params param = new ECG_Baseline_Params(Filtr_Method.LMS, Filtr_Type.HIGHPASS, 90, "Analysis1", 0.5);
            Assert.AreEqual(Filtr_Method.LMS, param.Method);
            Assert.AreEqual(Filtr_Type.HIGHPASS, param.Type);
            Assert.AreEqual(90, param.WindowLMS);
            Assert.AreEqual("Analysis1", param.AnalysisName);
            Assert.AreEqual(0.5, param.Mi);
        }
        [TestMethod]
        [Description("Test if Moving Average BandPass constructor works properly")]
        public void MovingAvgBPConstructorTest()
        {
          
            ECG_Baseline_Params param = new ECG_Baseline_Params(Filtr_Method.MOVING_AVG, Filtr_Type.BANDPASS, 10, 100, "analysis");
            Assert.AreEqual(Filtr_Method.MOVING_AVG, param.Method);
            Assert.AreEqual(Filtr_Type.BANDPASS, param.Type);
            Assert.AreEqual("analysis", param.AnalysisName);
            Assert.AreEqual(10, param.WindowSizeLow);
            Assert.AreEqual(100, param.WindowSizeHigh);
        }
        [TestMethod]
        [Description("Test if Savitzky Golay BandPass constructor works properly")]
        public void SavitzkyGolayBPConstructorTest()
        {

            ECG_Baseline_Params param = new ECG_Baseline_Params(Filtr_Method.SAV_GOL, Filtr_Type.BANDPASS, 10, 100, "analysis");
            Assert.AreEqual(Filtr_Method.SAV_GOL, param.Method);
            Assert.AreEqual(Filtr_Type.BANDPASS, param.Type);
            Assert.AreEqual("analysis", param.AnalysisName);
            Assert.AreEqual(10, param.WindowSizeLow);
            Assert.AreEqual(100, param.WindowSizeHigh);
        }

        [TestMethod]
        [Description("Test if LMS BandPass constructor works properly")]
        public void LmsBPConstructorTest()
        {
            ECG_Baseline_Params param = new ECG_Baseline_Params(Filtr_Method.LMS, Filtr_Type.BANDPASS, 10, "Analysis1", 0.5);
            Assert.AreEqual(Filtr_Method.LMS, param.Method);
            Assert.AreEqual(Filtr_Type.BANDPASS, param.Type);
            Assert.AreEqual(10, param.WindowLMS);
            //Assert.AreEqual(90, param.WindowSizeHigh);
            Assert.AreEqual("Analysis1", param.AnalysisName);
            Assert.AreEqual(0.5, param.Mi);
        }
    }
}
