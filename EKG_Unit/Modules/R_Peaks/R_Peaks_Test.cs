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
        
        [TestMethod]
        [Description("Test if interface stops after Abort()")]
        public void iterativeInterfaceTest1()
        {
            IModule testModule = new EKG_Project.Modules.R_Peaks.R_Peaks();
            R_Peaks_Method method = R_Peaks_Method.PANTOMPKINS;
            R_Peaks_Params param = new R_Peaks_Params(method, "abc123");

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
        /*
        [TestMethod]
        [Description("Test if interface does not throws any expections during execution")]
        public void iterativeInterfaceTestEnd()
        {
            IModule testModule = new EKG_Project.Modules.R_Peaks.R_Peaks();
            R_Peaks_Method method = R_Peaks_Method.PANTOMPKINS;
            R_Peaks_Params param = new R_Peaks_Params(method, "abc123");

            testModule.Init(param);
            while (!testModule.Ended())
            {
                Debug.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }

            Basic_New_Data_Worker originalWorker_basic = new Basic_New_Data_Worker("abc123");
            ECG_Baseline_New_Data_Worker originalWorker = new ECG_Baseline_New_Data_Worker("abc123");
            R_Peaks_New_Data_Worker resultWorker = new R_Peaks_New_Data_Worker("abc123temp");

            string[] leads = originalWorker_basic.LoadLeads().ToArray();
            Vector<Double> originalVector = originalWorker.LoadSignal(leads[0], 150, 300);
            Vector<Double> resultVector = resultWorker.LoadSignal(R_Peaks_Attributes.RPeaks,leads[0], 150, 300);
            //originalVector = originalVector.Multiply(scale);
            //Assert.AreEqual(originalVector.ToString(), resultVector.ToString()); // to speed it up just coerce the doubles
        }*/

    }
}
