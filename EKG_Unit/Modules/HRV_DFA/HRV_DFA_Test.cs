using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EKG_Project.Modules.HRV_DFA;
using EKG_Project.Modules;
using EKG_Project.IO;
using System.Diagnostics;
using System.Collections.Generic;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Unit.Modules.HRV_DFA
{
    [TestClass]
    public class HRV_DFA_Test
    {
        [TestMethod]
        [Description("Test if interface stops after abort")]
        public void GeneralInterface_Test()
        {
            IModule testModule = new EKG_Project.Modules.HRV_DFA.HRV_DFA();
            HRV_DFA_Params test_params = new HRV_DFA_Params("Analysis 1");

            testModule.Init(test_params);
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

    }
}
