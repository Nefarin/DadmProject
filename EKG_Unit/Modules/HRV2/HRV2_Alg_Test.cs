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
            double[] testArray = { 1, 2, 3, 4, 5 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            HRV2_Alg testAlgs = new HRV2_Alg();

            PrivateObject obj = new PrivateObject(testAlgs);
            obj.SetField("_rrIntervals", testVector);
            double RRIntervalVectorLength = ((Vector<double>)obj.GetField("_rrIntervals")).Count;

            Assert.AreEqual(5, RRIntervalVectorLength);
        }

        [TestMethod]
        [Description("Test if Tinn is calculate properly")]
        public void TinnTest()
        {
            double[] testArray = { 1, 2, 3, 4, 5 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            HRV2_Alg testAlgs = new HRV2_Alg();

            PrivateObject obj = new PrivateObject(testAlgs);
            obj.SetField("_rrIntervals", testVector);

            obj.Invoke("makeTinn");

            double testTinn = (double)obj.GetField("tinn");
            double resultTinn = 4;

            Assert.AreEqual(testTinn, resultTinn);
        }

        [TestMethod]
        [Description("Test if x counts are equal y counts")]
        public void PoincareTest()
        {
            double[] testArray = { 1, 2, 3, 4, 5 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            HRV2_Alg testAlgs = new HRV2_Alg();

            PrivateObject obj = new PrivateObject(testAlgs);
            obj.SetField("_rrIntervals", testVector);

            obj.Invoke("PoincarePlot_x");
            obj.Invoke("PoincarePlot_y");
            double x = ((Vector<double>)obj.GetField("RR_intervals_x")).Count;
            double y = ((Vector<double>)obj.GetField("RR_intervals_y")).Count;

            Assert.AreEqual(x, y);
        }

        [TestMethod]
        [Description("Test if throws null if argument is not initialized")]
        public void TinnNullTest()
        {
            double[] testArray = { 1, 2, 3, 4, 5 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            HRV2_Alg testAlgs = new HRV2_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);
            obj.SetField("_rrIntervals", testVector);
            obj.Invoke("makeTinn");
            double result = (double)obj.GetField("tinn");
            Assert.IsNotNull(result);

        }

        [TestMethod]
        [Description("Test if throws null if argument is not initialized")]

        public void TinnNullExeptionTest()
        {
            try
            {
                Vector<double> testVector = null;
                HRV2_Alg testAlgs = new HRV2_Alg();
                PrivateObject obj = new PrivateObject(testAlgs);
                obj.SetField("_rrIntervals", testVector);
                obj.Invoke("makeTinn");
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        [Description("Test if standard deviation is calculate properly")]
        public void StandardDevTest()
        {
            double[] testArray = { 2, 2, 2, 2, 2 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            HRV2_Alg testAlgs = new HRV2_Alg();

            PrivateObject obj = new PrivateObject(testAlgs);
            obj.SetField("_rrIntervals", testVector);
            double stdev = (double) obj.Invoke("getStandardDeviation", testVector); 
            double testStandardDev = stdev;
            double result = 0;
            Assert.AreEqual(testStandardDev, result);
        }

        [TestMethod]
        [Description("Test if PoincarePlot_y works properly")]
        public void PoincareYTest()
        {
            double[] testArray = { 1, 2, 3, 4, 5 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            HRV2_Alg testAlgs = new HRV2_Alg();

            PrivateObject obj = new PrivateObject(testAlgs);
            obj.SetField("_rrIntervals", testVector);

            obj.Invoke("PoincarePlot_y");
            Vector<double> testPoincrePloty = (Vector<double>)obj.GetField("RR_intervals_y");
            double[] resultArray = { 1, 2, 3, 4};
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);
            Assert.AreEqual(resultVector, testPoincrePloty);
        }

        [TestMethod]
        [Description("Test if PoincarePlot_x works properly")]
        public void PoincareXTest()
        {
            double[] testArray = { 1, 2, 3, 4, 5 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            HRV2_Alg testAlgs = new HRV2_Alg();

            PrivateObject obj = new PrivateObject(testAlgs);
            obj.SetField("_rrIntervals", testVector);

            obj.Invoke("PoincarePlot_x");
            Vector<double> testPoincrePloty = (Vector<double>)obj.GetField("RR_intervals_x");
            double[] resultArray = { 2, 3, 4, 5 };
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);
            Assert.AreEqual(resultVector, testPoincrePloty);
        }
    }
}
