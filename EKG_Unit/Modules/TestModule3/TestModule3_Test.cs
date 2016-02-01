using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EKG_Project.Modules.TestModule3;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules;
using EKG_Project.IO;
using System.Diagnostics;

namespace EKG_Unit.Modules.TestModule3
{
    [TestClass]
    public class TestModule3_Test
    {
        [TestMethod]
        [Description("Test if interface stops after Abort()")]
        public void iterativeInterfaceTest1()
        {
            IModule testModule = new EKG_Project.Modules.TestModule3.TestModule3();
            TestModule3_Params param = new TestModule3_Params(5, 1000, "abc123");

            testModule.Init(param);
            int counter = 0;
            while (!testModule.Ended())
            {
                counter++;
                Debug.WriteLine(testModule.Progress());
                testModule.ProcessData();
                testModule.Abort();
            }
            Assert.AreEqual(1, counter);

        }

        [TestMethod]
        [Description("Test if interface does not throws any expections during execution and checks if value is correctly scaled")]
        public void iterativeInterfaceTestEnd()
        {
            IModule testModule = new EKG_Project.Modules.TestModule3.TestModule3();
            int scale = 5;
            TestModule3_Params param = new TestModule3_Params(scale, 1000, "abc123");

            testModule.Init(param);
            while (!testModule.Ended())
            {
                Debug.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }

            Basic_New_Data_Worker originalWorker = new Basic_New_Data_Worker("abc123");
            Basic_New_Data_Worker resultWorker = new Basic_New_Data_Worker("abc123temp");

            string[] leads = originalWorker.LoadLeads().ToArray();
            Vector<Double> originalVector = originalWorker.LoadSignal(leads[0], 150, 300);
            Vector<Double> resultVector = resultWorker.LoadSignal(leads[0], 150, 300);
            originalVector = originalVector.Multiply(scale);
            Assert.AreEqual(originalVector.ToString(), resultVector.ToString()); // to speed it up just coerce the doubles
        }
    }
}
