using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.R_Peaks;

namespace EKG_Unit.Modules.R_Peaks
{
    [TestClass]
    public class R_Peaks_Params_Test
    {
        [TestMethod]
        [Description("Test if method is properly intialised")]
        public void methodTest()
        {
            R_Peaks_Method  method = R_Peaks_Method.PANTOMPKINS;
            R_Peaks_Params param = new R_Peaks_Params(method, "Test");
            Assert.AreEqual(param.Method, method);
        }

    }
}
