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
            R_Peaks_Params param = new R_Peaks_Params(method, "x");

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
        [Description("Test if interface does not throws any expections during execution for PanTompkins")]
        public void iterativeInterfaceTestEndPT()
        {
            IModule testModule = new EKG_Project.Modules.R_Peaks.R_Peaks();
            R_Peaks_Method method = R_Peaks_Method.PANTOMPKINS;
            R_Peaks_Params param = new R_Peaks_Params(method, "x");

            testModule.Init(param);
            while (!testModule.Ended())
            {
                Debug.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }

            Basic_New_Data_Worker originalWorker_basic = new Basic_New_Data_Worker("x");
            ECG_Baseline_New_Data_Worker originalWorker = new ECG_Baseline_New_Data_Worker("x");
            R_Peaks_New_Data_Worker resultWorker = new R_Peaks_New_Data_Worker("x");

            string[] leads = originalWorker_basic.LoadLeads().ToArray();
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<Double> testVector = originalWorker.LoadSignal(leads[0], 0, 4000);
            Vector<double> originalVector = test.PanTompkins(testVector, originalWorker_basic.LoadAttribute(Basic_Attributes.Frequency));
            Vector<Double> resultVector = resultWorker.LoadSignal(R_Peaks_Attributes.RPeaks, leads[0], 0, 14);
            Assert.AreEqual(originalVector.ToString(), resultVector.ToString()); // to speed it up just coerce the doubles
        }

        [TestMethod]
        [Description("Test if interface does not throws any expections during execution for Hilbert")]
        public void iterativeInterfaceTestEndH()
        {
            IModule testModule = new EKG_Project.Modules.R_Peaks.R_Peaks();
            R_Peaks_Method method = R_Peaks_Method.HILBERT;
            R_Peaks_Params param = new R_Peaks_Params(method, "x");

            testModule.Init(param);
            while (!testModule.Ended())
            {
                Debug.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }

            Basic_New_Data_Worker originalWorker_basic = new Basic_New_Data_Worker("x");
            ECG_Baseline_New_Data_Worker originalWorker = new ECG_Baseline_New_Data_Worker("x");
            R_Peaks_New_Data_Worker resultWorker = new R_Peaks_New_Data_Worker("x");

            string[] leads = originalWorker_basic.LoadLeads().ToArray();
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<Double> testVector = originalWorker.LoadSignal(leads[0], 0, 4000);
            Vector<double> originalVector = test.Hilbert(testVector, originalWorker_basic.LoadAttribute(Basic_Attributes.Frequency));
            Vector<Double> resultVector = resultWorker.LoadSignal(R_Peaks_Attributes.RPeaks, leads[0], 0, 14);
            Assert.AreEqual(originalVector.ToString(), resultVector.ToString()); // to speed it up just coerce the doubles
        }

        [TestMethod]
        [Description("Test if interface does not throws any expections during execution for EMD")]
        public void iterativeInterfaceTestEndEMD()
        {
            IModule testModule = new EKG_Project.Modules.R_Peaks.R_Peaks();
            R_Peaks_Method method = R_Peaks_Method.EMD;
            R_Peaks_Params param = new R_Peaks_Params(method, "x");

            testModule.Init(param);
            while (!testModule.Ended())
            {
                Debug.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }

            Basic_New_Data_Worker originalWorker_basic = new Basic_New_Data_Worker("x");
            ECG_Baseline_New_Data_Worker originalWorker = new ECG_Baseline_New_Data_Worker("x");
            R_Peaks_New_Data_Worker resultWorker = new R_Peaks_New_Data_Worker("x");

            string[] leads = originalWorker_basic.LoadLeads().ToArray();
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<Double> testVector = originalWorker.LoadSignal(leads[1], 0, 4000);
            Vector<double> originalVector = test.EMD(testVector, originalWorker_basic.LoadAttribute(Basic_Attributes.Frequency));
            Vector<Double> resultVector = resultWorker.LoadSignal(R_Peaks_Attributes.RPeaks, leads[1], 0, 17);
            Assert.AreEqual(originalVector.ToString(), resultVector.ToString()); // to speed it up just coerce the doubles
        }

    }
}
