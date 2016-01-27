﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EKG_Project.Modules.HRV1;
using MathNet.Numerics.LinearAlgebra;


namespace EKG_Unit.Modules.HRV1
{
    //to do:
    //GenerateFreqVector - do delate
    //intervalsCorection
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

            var testinstantsarr = new double[]{ 2, 3, 5, 9, 11, 15, 20, 22, 28, 30};
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
            var expectedRMSSD = 124.44 ;
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
            Assert.IsTrue(Math.Abs(actualAVNN-expectedAVNN)< passThreshold);
            
            Assert.IsTrue(Math.Abs(actualSDNN - expectedSDNN) < passThreshold);

            Assert.IsTrue(Math.Abs(actualRMSSD - expectedRMSSD) < passThreshold);

            Assert.IsTrue(Math.Abs(actualNN50 - expectedNN50) < passThreshold);

            double passThreshold2 = 0.1;
            Assert.IsTrue(Math.Abs(actualpNN50 - expectedpNN50) < passThreshold2);
        }

        [TestMethod]
        [Description("Test of LS2")]
        public void lombscargle2Test()
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

            obj.Invoke("lombScargle2");
            var actualResult = (Vector<double>)obj.GetField("PSD");

            // Assert results

            Assert.AreEqual(expectedResult.Count, actualResult.Count);

            for (int i = 0; i < expectedResult.Count; ++i)
            {
                Assert.IsTrue(Math.Abs(expectedResult[i] - actualResult[i]) < 10);
            }
        }
    }
}

