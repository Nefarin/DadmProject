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
    }
}
