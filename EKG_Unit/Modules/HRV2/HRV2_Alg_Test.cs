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

            HRV2_Alg testAlgs = new HRV2_Alg();
            
            //testAlgs.HRV2_Anlalysis();
            //testAlgs.histogramCounts();
            
            double histoCounts =  testAlgs.histogramCount; 
            double RRIntervalVectorLength = testAlgs.RRIntervals.Count;

            PrivateObject obj = new PrivateObject(testAlgs);

            // Process test here

            obj.Invoke("EqualVectors");

            // Assert results

            Assert.AreEqual(histoCounts, RRIntervalVectorLength);
        }

        [TestMethod]
        [Description("Test if Tinn is calculate properly")]
        public void TinnTest()
        {

            HRV2_Alg testAlgs = new HRV2_Alg();

            double[] testArray = { 1, 2, 3, 4, 5 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            testAlgs.RRIntervals = testVector;
            double testTinn = testAlgs.Tinn;
            double resultTinn = 4;

            PrivateObject obj = new PrivateObject(testAlgs);

            obj.Invoke("Tinn OK");

            Assert.AreEqual(testTinn, resultTinn);
        }

    }
}
