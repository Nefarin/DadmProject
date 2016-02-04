using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.T_Wave_Alt;
using EKG_Project.Modules;
using EKG_Project.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace EKG_Unit.Modules.T_Wave_Alt
{
    [TestClass]
    public class T_Wave_Alt_Test
    {
        [TestMethod]
        [Description("Test if interface stops after abort")]
        public void InterfaceT_Wave_AltTest()
        {
            IModule testModule = new EKG_Project.Modules.T_Wave_Alt.T_Wave_Alt();
            T_Wave_Alt_Params test_params = new T_Wave_Alt_Params("Analysis 1");

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
        [Description("Test if interface does not throw any expections during execution")]
        public void InterfaceT_Wave_AltTest2()
        {
            IModule testModule = new EKG_Project.Modules.T_Wave_Alt.T_Wave_Alt();
            T_Wave_Alt_Params test_params = new T_Wave_Alt_Params("Analysis 1");

            testModule.Init(test_params);
            while (!testModule.Ended())
            {
                Debug.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }

            Basic_New_Data_Worker _basicWorker = new Basic_New_Data_Worker("Analysis 1");
            ECG_Baseline_New_Data_Worker _baselineWorker = new ECG_Baseline_New_Data_Worker("Analysis 1");
            Waves_New_Data_Worker _wavesWorker = new Waves_New_Data_Worker("Analysis 1");
            T_Wave_Alt_New_Data_Worker _worker = new T_Wave_Alt_New_Data_Worker("Analysis 1");

            string[] leads = _basicWorker.LoadLeads().ToArray();
            T_Wave_Alt_Alg test = new T_Wave_Alt_Alg();
            uint _sampFreq = _basicWorker.LoadAttribute(Basic_Attributes.Frequency);
            test.Fs = _sampFreq;

            Vector<double> testVector = _baselineWorker.LoadSignal(leads[0], 0, 4000);
            List<int> testWaves = _wavesWorker.LoadSignal(Waves_Signal.TEnds, leads[0], 0, 2000);
            List<Vector<double>> T_WavesArray = test.buildTWavesArray(testVector, testWaves, 0);
            Vector<double> medianT_Wave = test.calculateMedianTWave(T_WavesArray);
            Vector<double> ACI = test.calculateACI(T_WavesArray, medianT_Wave);
            List<int> Flucts = test.findFluctuations(ACI);
            Vector<double> Alternans1 = test.findAlternans(Flucts);
            List<Tuple<int, int>> finalDetection = test.alternansDetection(Alternans1, testWaves);

            List<Tuple<int, int>> resultT_Wave_Alt = _worker.LoadAlternansDetectedList(leads[0], 0, (int)_worker.getNumberOfSamples(leads[0]));

            Assert.AreEqual(finalDetection.ToString(), resultT_Wave_Alt.ToString());
        }
    }


}