using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EKG_Project.Modules.TestModule3;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules;

namespace EKG_Unit.Modules.TestModule3
{
    [TestClass]
    public class TestModule3_Test
    {
        [TestMethod]
        [Description("Test if interface does not throws any expections during execution")]
        public void iterativeInterfaceTest()
        {
            IModule testModule = new EKG_Project.Modules.TestModule3.TestModule3();
            TestModule3_Params param = new TestModule3_Params(5, 1000, "");

            testModule.Init(param);
            while (!testModule.Ended())
            {
                Console.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }

            Assert.AreEqual(1, 1);
        }
    }
}
