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
        public void scaleSamplesTest1()
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
    }
}
