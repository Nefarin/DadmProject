using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EKG_Project.Modules.HRV1;
using MathNet.Numerics.LinearAlgebra;


namespace EKG_Unit.Modules.HRV1
{
    //to do:
    //calculateFreqBased

    [TestClass]
    public class HRV1_Alg_Test
    {

        [TestMethod]
        [Description("Test of LS")]
        public void lombscargleTest()
        {
            // Init test here

            var hrv1Test = new HRV1_Alg();

            var testintervals = Vector<double>.Build.Dense(10, i => i);
            var testinstants = Vector<double>.Build.Dense(10, i => i);
            var testfreq = Vector<double>.Build.Dense(10, i => (double)i / 10);

            var expectedArr = new double[] { 0, 52.3607, 14.4721, 7.6393, 5.5279, 2.5000, 5.5279, 7.6393, 14.4721, 52.3607 };
            var expectedResult = Vector<double>.Build.Dense(expectedArr);

            // access private fields externally

            PrivateObject obj = new PrivateObject(hrv1Test);
            obj.SetField("rrIntervals", testintervals);
            obj.SetField("rInstants", testinstants);
            obj.SetField("f", testfreq);

            // Process test here

            obj.Invoke("lombScargle");
            var actualResult = (Vector<double>)obj.GetField("PSD");

            // Assert results

            Assert.AreEqual(expectedResult.Count, actualResult.Count);

            for (int i = 0; i < expectedResult.Count; ++i)
            {
                Assert.IsTrue(Math.Abs(expectedResult[i] - actualResult[i]) < 100);
            }
        }

        [TestMethod]
        [Description("Test of instants To Intervals")]
        public void instantsToIntervalsTest()
        {
            // Init test here

            var hrv1Test = new HRV1_Alg();

            var testinstantsarr = new double[] { 2, 3, 5, 9, 11, 15, 20, 22, 28, 30 };
            var testinstantsvec = Vector<double>.Build.Dense(testinstantsarr);

            var expectedarr = new double[] { 1, 2, 4, 2, 4, 5, 2, 6, 2 };
            var expectedvec = Vector<double>.Build.Dense(expectedarr);

            // access private fields externally

            PrivateObject obj = new PrivateObject(hrv1Test);
            obj.SetField("rInstants", testinstantsvec);

            // Process test here

            obj.Invoke("instantsToIntervals");
            var result = (Vector<double>)obj.GetField("rrIntervals");

            // Assert results

            Assert.AreEqual(expectedvec.Count, result.Count);

        }

        [TestMethod]
        [Description("Test of calculate Time Based Parameters")]
        public void calculateTimeBasedTest()
        {
            // Init test here

            var hrv1Test = new HRV1_Alg();

            var testintervals = new double[] { 11, 28, 43, 25, 38, 95, 86, 15, 61, 65, 22, 400, 33, 37, 34, 33, 75, 47, 21, 12 };
            var vectortestintervals = Vector<double>.Build.Dense(testintervals);

            var expectedAVNN = 59.05;
            var expectedSDNN = 83.75;
            var expectedRMSSD = 124.44;
            var expectedNN50 = 4;
            var expectedpNN50 = 20; // wartość w %

            // access private fields externally

            PrivateObject obj = new PrivateObject(hrv1Test);
            obj.SetField("rrIntervals", vectortestintervals);

            // Process test here

            obj.Invoke("calculateTimeBased");
            var actualAVNN = (double)obj.GetField("AVNN");
            var actualSDNN = (double)obj.GetField("SDNN");
            var actualRMSSD = (double)obj.GetField("RMSSD");
            var actualNN50 = (double)obj.GetField("NN50");
            var actualpNN50 = (double)obj.GetField("pNN50");
            // Assert results

            //Assert.AreEqual(expectedAVNN, actualAVNN);
            double passThreshold = 0.1;
            Assert.IsTrue(Math.Abs(actualAVNN - expectedAVNN) < passThreshold);

            Assert.IsTrue(Math.Abs(actualSDNN - expectedSDNN) < passThreshold);

            Assert.IsTrue(Math.Abs(actualRMSSD - expectedRMSSD) < passThreshold);

            Assert.IsTrue(Math.Abs(actualNN50 - expectedNN50) < passThreshold);

            Assert.IsTrue(Math.Abs(actualpNN50 - expectedpNN50) < passThreshold);
        }

        [TestMethod]
        [Description("Test of LS2")]
        public void lombscargle2Test()
        {
            // Init test here

            var hrv1Test = new HRV1_Alg();

            var testintervals = Vector<double>.Build.Dense(10, i => i);
            var testinstants = Vector<double>.Build.Dense(10, i => i);

            var expectedArr = new double[] { 7.5789, 5.3340, 59.8628, 12.6528, 2.3629, 14.6560, 30.4796, 4.9389, 6.2248, 82.4832, 4.5316, 3.1664, 75.0937, 4.6414, 1.3376, 9.1844, 4.3423, 1.4853, 2.3137, 82.2037 };
            var expectedResult = Vector<double>.Build.Dense(expectedArr);

            // access private fields externally

            PrivateObject obj = new PrivateObject(hrv1Test);
            obj.SetField("rrIntervals", testintervals);
            obj.SetField("rInstants", testinstants);

            // Process test here

            obj.Invoke("lombScargle2");
            var actualResult = (Vector<double>)obj.GetField("PSD");

            // Assert results

            Assert.AreEqual(expectedResult.Count, actualResult.Count);

            for (int i = 0; i < expectedResult.Count; ++i)
            {
                Assert.IsTrue(Math.Abs(expectedResult[i] - actualResult[i]) < 200);
            }
        }

        [TestMethod]
        [Description("Test of intervals Corection")]
        public void intervalsCorectionTest()
        {
            // Init test here

            var hrv1Test = new HRV1_Alg();

            var testintervals = new double[] { 1, 1000, 1001, 7, 5, 0, -10, 100 };
            var vectortestintervals = Vector<double>.Build.Dense(testintervals);

            var expectedintervals = new double[] { 7, 100 };
            var vectorexpectedintervals = Vector<double>.Build.Dense(expectedintervals);


            // access private fields externally

            PrivateObject obj = new PrivateObject(hrv1Test);
            obj.SetField("rrIntervals", vectortestintervals);

            // Process test here

            obj.Invoke("intervalsCorection");
            var actualintervals = (Vector<double>)obj.GetField("rrIntervals");

            // Assert results

            Assert.AreEqual(vectorexpectedintervals, actualintervals);

        }

        [TestMethod]
        [Description("Test of frequency based parameters")]
        public void frequencyTest()
        {
            // Init test here

            var hrv1Test = new HRV1_Alg();

            double df = 0.006;
            var vectortestf = Vector<double>.Build.Dense(70, i=>i*df);

            var vectortestPSD = Vector<double>.Build.Dense(70, i=>1);

            var expectedTP = 0.42;
            var expectedHF = 0.264;
            var expectedLF = 0.114;
            var expectedVLF = 0.036;
            var expectedLFHF = 0.4318;

            // access private fields externally

            PrivateObject obj = new PrivateObject(hrv1Test);
            obj.SetField("f", vectortestf);
            obj.SetField("PSD", vectortestPSD);

            // Process test here

            obj.Invoke("calculateFreqBased");
            var actualTP = (double)obj.GetField("TP");
            var actualHF = (double)obj.GetField("HF");
            var actualLF = (double)obj.GetField("LF");
            var actualVLF = (double)obj.GetField("VLF");
            var actualLFHF = (double)obj.GetField("LFHF");

            // Assert results

            double passThreshold = 1;
            double passThreshold2 = 0.1;

            Assert.IsTrue(Math.Abs(actualTP - expectedTP) < passThreshold);
            Assert.IsTrue(Math.Abs(actualHF - expectedHF) < passThreshold);
            Assert.IsTrue(Math.Abs(actualLF - expectedLF) < passThreshold);
            Assert.IsTrue(Math.Abs(actualVLF - expectedVLF) < passThreshold);
            
            Assert.IsTrue(Math.Abs(actualLFHF - expectedLFHF) < passThreshold2);

        }
    }
}

