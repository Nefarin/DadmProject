using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.Sleep_Apnea;
using System.Collections.Generic;


namespace EKG_Unit.Modules.Sleep_Apnea
{
    [TestClass]
    public class Sleep_Apnea_Alg_Test
    {
        [TestMethod]
        [Description("Test if rr intervals are calculated correctly")]
        public void Sleep_Apnea_findIntervals_Test1()
        {
            // Init test here
            Sleep_Apnea_Params testParams = new Sleep_Apnea_Params("Test");

            int fs = 200;
            List<uint> R_detected = new List<uint>() { 1, 2, 3, 4, 5, 6, 7 };

            List<List<double>> RR_intervals = new List<List<double>>()
            {
                new List<double>() { 1,2,3,4,5,6,7},
                new List<double>() {0.005, 0.005, 0.005, 0.005, 0.005, 0.005, 0}
            };

            Sleep_Apnea_Alg testAlgs = new Sleep_Apnea_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);

            // Process test here
            List<List<double>> result = (List<List<double>>)obj.Invoke("findIntervals", R_detected, fs);

            // Assert results
            CollectionAssert.AreEqual(RR_intervals[0], result[0]);
            CollectionAssert.AreEqual(RR_intervals[1], result[1]);
        }

    }
}
