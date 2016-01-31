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
            //obj.Invoke("Histogram");

            //double histoCounts = (double)obj.GetField("histogramCount"); 
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
        [Description("Test if Triangle Index is calculate properly")]
        public void TriangleIndexTest()
        {
            double[] testArray = { 1,1,2,2,2,3,3,3,3,4,5,5,5,5,5,5,5,5,5,5,6,7};
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            HRV2_Alg testAlgs = new HRV2_Alg();

            PrivateObject obj = new PrivateObject(testAlgs);
            obj.SetField("_rrIntervals", testVector);

            obj.Invoke("makeTriangleIndex");
            
            double testTriangleIndex = (double)obj.GetField("triangleIndex");
            double resultTriangleIndex = 2;

            Assert.AreEqual(testTriangleIndex, resultTriangleIndex);
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
        [Description("Test if...)]
        public void Test()
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


    }
}
