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
    public class R_Peaks_Test
    {
        /*
        [TestMethod]
        [Description("Test if interface stops after Abort()")]
        public void iterativeInterfaceTest1()
        {
            IModule testModule = new EKG_Project.Modules.R_Peaks.R_Peaks();
            R_Peaks_Method method = R_Peaks_Method.PANTOMPKINS;
            R_Peaks_Params param = new R_Peaks_Params(method, "Test");

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
        }*/

    }
}
