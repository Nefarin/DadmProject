using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.Waves;
using EKG_Project.Modules;
using EKG_Project.IO;
using System.Diagnostics;
using System.Collections.Generic;


namespace EKG_Unit.Modules.Waves
{
    [TestClass]
    public class Waves_Test
    {
        [TestMethod]
        [Description("Test if interface stops after abort")]
        public void InterfaceTest()
        {
            IModule testModule = new EKG_Project.Modules.Waves.Waves();
            Waves_Params test_params = new Waves_Params(Wavelet_Type.haar,3, "Analysis 1", 500);

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
        [Description("Test if interface does not throws any expections during execution of Haar Wavelet")]
        public void InterfaceTestHaar()
        {
            IModule testModule = new EKG_Project.Modules.Waves.Waves();
            Waves_Params test_params = new Waves_Params(Wavelet_Type.haar, 3, "Analysis 1", 500);

            testModule.Init(test_params);
            while (!testModule.Ended())
            {
                Debug.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }

            Basic_New_Data_Worker _basicWorker = new Basic_New_Data_Worker("Analysis 1");
            ECG_Baseline_New_Data_Worker _baselineWorker = new ECG_Baseline_New_Data_Worker("Analysis 1");
            R_Peaks_New_Data_Worker _rpeaksWorker = new R_Peaks_New_Data_Worker("Analysis 1");
            Waves_New_Data_Worker _worker = new Waves_New_Data_Worker("Analysis 1");

            List<int> QRSonset = new List<int>();
            List<int> QRSend = new List<int>();
            List<int> Ponset = new List<int>();
            List<int> Pend = new List<int>();
            List<int> Tend = new List<int>();

            string[] leads = _basicWorker.LoadLeads().ToArray();
            Waves_Alg test = new Waves_Alg(test_params);
            Vector<Double> testVector = _baselineWorker.LoadSignal(leads[0], 0, 4000);
            Vector<Double> testVector2 = _rpeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks,leads[0],0,(int)_rpeaksWorker.getNumberOfSamples(R_Peaks_Attributes.RPeaks,leads[0]));
            test.analyzeSignalPart(testVector,testVector2,QRSonset,QRSend,Ponset,Pend,Tend,1000, _basicWorker.LoadAttribute(Basic_Attributes.Frequency));
            List<int> resultQRSonset = _worker.LoadSignal(Waves_Signal.QRSOnsets, leads[0], 0, (int)_worker.getNumberOfSamples(Waves_Signal.QRSOnsets, leads[0]));

            Assert.AreEqual(QRSonset.ToString(), resultQRSonset.ToString());
        }

        [TestMethod]
        [Description("Test if interface does not throws any expections during execution of db2 Wavelet")]
        public void InterfaceTestdb2()
        {
            IModule testModule = new EKG_Project.Modules.Waves.Waves();
            Waves_Params test_params = new Waves_Params(Wavelet_Type.db2, 3, "Analysis 1", 500);

            testModule.Init(test_params);
            while (!testModule.Ended())
            {
                Debug.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }

            Basic_New_Data_Worker _basicWorker = new Basic_New_Data_Worker("Analysis 1");
            ECG_Baseline_New_Data_Worker _baselineWorker = new ECG_Baseline_New_Data_Worker("Analysis 1");
            R_Peaks_New_Data_Worker _rpeaksWorker = new R_Peaks_New_Data_Worker("Analysis 1");
            Waves_New_Data_Worker _worker = new Waves_New_Data_Worker("Analysis 1");

            List<int> QRSonset = new List<int>();
            List<int> QRSend = new List<int>();
            List<int> Ponset = new List<int>();
            List<int> Pend = new List<int>();
            List<int> Tend = new List<int>();

            string[] leads = _basicWorker.LoadLeads().ToArray();
            Waves_Alg test = new Waves_Alg(test_params);
            Vector<Double> testVector = _baselineWorker.LoadSignal(leads[0], 0, 4000);
            Vector<Double> testVector2 = _rpeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, leads[0], 0, (int)_rpeaksWorker.getNumberOfSamples(R_Peaks_Attributes.RPeaks, leads[0]));
            test.analyzeSignalPart(testVector, testVector2, QRSonset, QRSend, Ponset, Pend, Tend, 1000, _basicWorker.LoadAttribute(Basic_Attributes.Frequency));
            List<int> resultQRSonset = _worker.LoadSignal(Waves_Signal.QRSOnsets, leads[0], 0, (int)_worker.getNumberOfSamples(Waves_Signal.QRSOnsets, leads[0]));

            Assert.AreEqual(QRSonset.ToString(), resultQRSonset.ToString());
        }

        [TestMethod]
        [Description("Test if interface does not throws any expections during execution of db3 Wavelet")]
        public void InterfaceTestdb3()
        {
            IModule testModule = new EKG_Project.Modules.Waves.Waves();
            Waves_Params test_params = new Waves_Params(Wavelet_Type.db3, 3, "Analysis 1", 500);

            testModule.Init(test_params);
            while (!testModule.Ended())
            {
                Debug.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }

            Basic_New_Data_Worker _basicWorker = new Basic_New_Data_Worker("Analysis 1");
            ECG_Baseline_New_Data_Worker _baselineWorker = new ECG_Baseline_New_Data_Worker("Analysis 1");
            R_Peaks_New_Data_Worker _rpeaksWorker = new R_Peaks_New_Data_Worker("Analysis 1");
            Waves_New_Data_Worker _worker = new Waves_New_Data_Worker("Analysis 1");

            List<int> QRSonset = new List<int>();
            List<int> QRSend = new List<int>();
            List<int> Ponset = new List<int>();
            List<int> Pend = new List<int>();
            List<int> Tend = new List<int>();

            string[] leads = _basicWorker.LoadLeads().ToArray();
            Waves_Alg test = new Waves_Alg(test_params);
            Vector<Double> testVector = _baselineWorker.LoadSignal(leads[0], 0, 4000);
            Vector<Double> testVector2 = _rpeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, leads[0], 0, (int)_rpeaksWorker.getNumberOfSamples(R_Peaks_Attributes.RPeaks, leads[0]));
            test.analyzeSignalPart(testVector, testVector2, QRSonset, QRSend, Ponset, Pend, Tend, 1000, _basicWorker.LoadAttribute(Basic_Attributes.Frequency));
            List<int> resultQRSonset = _worker.LoadSignal(Waves_Signal.QRSOnsets, leads[0], 0, (int)_worker.getNumberOfSamples(Waves_Signal.QRSOnsets, leads[0]));

            Assert.AreEqual(QRSonset.ToString(), resultQRSonset.ToString());
        }

        [TestMethod]
        [Description("Test if interface does not throws any expections during execution of third decomposition level")]
        public void InterfaceTestDecompLevel()
        {
            IModule testModule = new EKG_Project.Modules.Waves.Waves();
            Waves_Params test_params = new Waves_Params(Wavelet_Type.haar, 3, "Analysis 1", 500);

            testModule.Init(test_params);
            while (!testModule.Ended())
            {
                Debug.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }

            Basic_New_Data_Worker _basicWorker = new Basic_New_Data_Worker("Analysis 1");
            ECG_Baseline_New_Data_Worker _baselineWorker = new ECG_Baseline_New_Data_Worker("Analysis 1");
            R_Peaks_New_Data_Worker _rpeaksWorker = new R_Peaks_New_Data_Worker("Analysis 1");
            Waves_New_Data_Worker _worker = new Waves_New_Data_Worker("Analysis 1");

            List<int> QRSonset = new List<int>();
            List<int> QRSend = new List<int>();
            List<int> Ponset = new List<int>();
            List<int> Pend = new List<int>();
            List<int> Tend = new List<int>();

            string[] leads = _basicWorker.LoadLeads().ToArray();
            Waves_Alg test = new Waves_Alg(test_params);
            Vector<Double> testVector = _baselineWorker.LoadSignal(leads[0], 0, 4000);
            Vector<Double> testVector2 = _rpeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, leads[0], 0, (int)_rpeaksWorker.getNumberOfSamples(R_Peaks_Attributes.RPeaks, leads[0]));
            test.analyzeSignalPart(testVector, testVector2, QRSonset, QRSend, Ponset, Pend, Tend, 1000, _basicWorker.LoadAttribute(Basic_Attributes.Frequency));
            List<int> resultQRSonset = _worker.LoadSignal(Waves_Signal.QRSOnsets, leads[0], 0, (int)_worker.getNumberOfSamples(Waves_Signal.QRSOnsets, leads[0]));

            Assert.AreEqual(QRSonset.ToString(), resultQRSonset.ToString());
        }

        [TestMethod]
        [Description("Test if interface does not throws any expections during execution of step size equal to 500")]
        public void InterfaceTestStep()
        {
            IModule testModule = new EKG_Project.Modules.Waves.Waves();
            Waves_Params test_params = new Waves_Params(Wavelet_Type.haar, 3, "Analysis 1", 500);

            testModule.Init(test_params);
            while (!testModule.Ended())
            {
                Debug.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }

            Basic_New_Data_Worker _basicWorker = new Basic_New_Data_Worker("Analysis 1");
            ECG_Baseline_New_Data_Worker _baselineWorker = new ECG_Baseline_New_Data_Worker("Analysis 1");
            R_Peaks_New_Data_Worker _rpeaksWorker = new R_Peaks_New_Data_Worker("Analysis 1");
            Waves_New_Data_Worker _worker = new Waves_New_Data_Worker("Analysis 1");

            List<int> QRSonset = new List<int>();
            List<int> QRSend = new List<int>();
            List<int> Ponset = new List<int>();
            List<int> Pend = new List<int>();
            List<int> Tend = new List<int>();

            string[] leads = _basicWorker.LoadLeads().ToArray();
            Waves_Alg test = new Waves_Alg(test_params);
            Vector<Double> testVector = _baselineWorker.LoadSignal(leads[0], 0, 4000);
            Vector<Double> testVector2 = _rpeaksWorker.LoadSignal(R_Peaks_Attributes.RPeaks, leads[0], 0, (int)_rpeaksWorker.getNumberOfSamples(R_Peaks_Attributes.RPeaks, leads[0]));
            test.analyzeSignalPart(testVector, testVector2, QRSonset, QRSend, Ponset, Pend, Tend, 1000, _basicWorker.LoadAttribute(Basic_Attributes.Frequency));
            List<int> resultQRSonset = _worker.LoadSignal(Waves_Signal.QRSOnsets, leads[0], 0, (int)_worker.getNumberOfSamples(Waves_Signal.QRSOnsets, leads[0]));

            Assert.AreEqual(QRSonset.ToString(), resultQRSonset.ToString());
        }
    }
}
