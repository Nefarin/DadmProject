using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EKG_Project.Modules.TestModule3;
using MathNet.Numerics.LinearAlgebra;
namespace EKG_Unit.Modules.TestModule3
{
    [TestClass]
    public class TestModule3_Params_Test
    {
        [TestMethod]
        [Description("Test if step is bounded")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Step below 1")]
        public void stepTest1()
        {
            int stepValue = -1;
            TestModule3_Params param = new TestModule3_Params(5, stepValue, "Test");
        }

        [TestMethod]
        [Description("Test if step is properly initialized")]
        public void stepTest2()
        {
            int stepValue = 5;
            TestModule3_Params param = new TestModule3_Params(5, stepValue, "Test");
            Assert.AreEqual(param.Step, stepValue);
        }
    }
}
