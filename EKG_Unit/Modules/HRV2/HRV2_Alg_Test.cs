using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.HRV2;
using System.Collections.Generic;

namespace EKG_Unit.Modules.HRV2
{
    [TestClass]
    public class HRV2_Alg_Test
    {
        [TestMethod]
        [Description("Test if vectors RRIntervals and Counts in histogram are equal")]
        public void HistogramCountsTest()
        {
            double[] testArray = { 1, 2, 3, 4, 5, 6, 6, 7, 4, 4, 4 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            HRV2_Alg testAlgs = new HRV2_Alg();
            testAlgs.RRIntervals = testVector;

            testAlgs.HRV2_Anlalysis();

            double histoCounts =  testAlgs.histogramCount; 
            double RRIntervalVectorLength = testAlgs.RRIntervals.Count;

            Assert.AreEqual(histoCounts, RRIntervalVectorLength);
        }

        [TestMethod]
        [Description("Test if Tinn is calculate properly")]
        public void TinnTest()
        {
            double[] testArray = { 1, 2, 3, 4, 5 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            HRV2_Alg testAlgs = new HRV2_Alg();
            testAlgs.RRIntervals = testVector;
            testAlgs.HRV2_Anlalysis();

            double testTinn = testAlgs.Tinn;
            double resultTinn = 4;

            Assert.AreEqual(testTinn, resultTinn);
        }

    }
}
