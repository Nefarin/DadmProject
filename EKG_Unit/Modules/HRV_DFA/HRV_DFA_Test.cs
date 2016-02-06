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
            HRV_DFA_Params test_params = new HRV_DFA_Params("a100dat");

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
        [TestMethod]
        [Description("Test if interface performs in fast process state")]
        public void Interface_FastProcessTest()
        {
            IModule testModule = new EKG_Project.Modules.HRV_DFA.HRV_DFA();
            HRV_DFA_Params test_params = new HRV_DFA_Params("a100dat");
            testModule.Init(test_params);

            int counter = 0;
            while (!testModule.Ended())
            {
                counter++;
                Debug.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }
            Basic_New_Data_Worker _basicWorker = new Basic_New_Data_Worker("a100dat");
            R_Peaks_New_Data_Worker _rpeaksWorker = new R_Peaks_New_Data_Worker("a100dat");

            HRV_DFA_New_Data_Worker _worker = new HRV_DFA_New_Data_Worker("a100dat");
            string[] leads = _basicWorker.LoadLeads().ToArray();
            Vector<double> testVector = _rpeaksWorker.LoadSignal(R_Peaks_Attributes.RRInterval, leads[0], 0, (int)_rpeaksWorker.getNumberOfSamples(R_Peaks_Attributes.RRInterval, leads[0]));

            Tuple<Vector<double>, Vector<double>> results = _worker.LoadSignal(HRV_DFA_Signals.DfaValueFn, leads[0], 0, (int)_worker.getNumberOfSamples(HRV_DFA_Signals.ParamAlpha, leads[0]));
            Assert.IsNotNull(results.Item1);

        }

    }
}
