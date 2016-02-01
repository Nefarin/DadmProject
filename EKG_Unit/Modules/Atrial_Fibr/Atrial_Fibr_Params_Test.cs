using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.Atrial_Fibr;

namespace EKG_Unit.Modules.Atrial_Fibr
{
    [TestClass]
    public class Atrial_Fibr_Params_Test
    {
        [TestMethod]
        [Description("Test if method is properly initialized")]
        public void paramTest()
        {
            Atrial_Fibr_Params param = new Atrial_Fibr_Params(Detect_Method.POINCARE, "test");
            Detect_Method result = Detect_Method.POINCARE;
            Assert.AreEqual(param.Method, result);
        }
    }
}
