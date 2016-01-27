using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.Atrial_Fibr;
using System.Collections.Generic;

namespace EKG_Unit.Modules.Atrial_Fibr
{
    [TestClass]
    public class Atrial_Fibr_Alg_Test
    {
        [TestMethod]
        [Description("Test if turning points is correct for AF signal (07859)")]
        public void TPRTestAF()
        {
            //init
            double[] testArray = { 137, 147, 156, 129, 130, 123, 129, 138, 138, 162, 138, 161, 123, 144, 157, 156, 142, 136, 118, 136, 158, 139, 178, 158, 164, 126, 135, 134, 173, 171, 170, 143 };
            double result = 0.95;
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { testVector };
            double delta = 0.001;
            //test
            double realresult = Convert.ToDouble(obj.Invoke("TPR", args));
            //results
            Assert.AreEqual(realresult, result, delta);
        }

        [TestMethod]
        [Description("Test if turning points is correct for healthy signal (16265)")]
        public void TPRTestNAF()
        {
            //init
            double[] testArray = { 85, 84, 82, 83, 84, 89, 94, 101, 101, 99, 97, 96, 95, 95, 97, 98, 97, 94, 95, 98, 95, 97, 101, 103, 95, 95, 97, 98, 97, 97, 100, 100 };
            double result = 0.35;
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { testVector };
            double delta = 0.001;
            //test
            double realresult = Convert.ToDouble(obj.Invoke("TPR", args));
            //results
            Assert.AreEqual(realresult, result, delta);
        }
        [TestMethod]
        [Description("Test if SE is correct for AF signal")]
        public void SETestAF()
        {
            //init
            double[] testArray = { 137, 147, 156, 129, 130, 123, 129, 138, 138, 162, 138, 161, 123, 144, 157, 156, 142, 136, 118, 136, 158, 139, 178, 158, 164, 126, 135, 134, 173, 171, 170, 143 };
            double result = 0.9865;
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { testVector };
            double delta = 0.001;
            //test
            double realresult = Convert.ToDouble(obj.Invoke("SE", args));
            //results
            Assert.AreEqual(result, realresult, delta);
        }
        [TestMethod]
        [Description("Test if SE is correct for healthy signal")]
        public void SETestNAF()
        {
            //init
            double[] testArray = { 85, 84, 82, 83, 84, 89, 94, 101, 101, 99, 97, 96, 95, 95, 97, 98, 97, 94, 95, 98, 95, 97, 101, 103, 95, 95, 97, 98, 97, 97, 100, 100 };
            double result = 0.9335;
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { testVector };
            double delta = 0.001;
            //test
            double realresult = Convert.ToDouble(obj.Invoke("SE", args));
            //results
            Assert.AreEqual(result, realresult, delta);
        }

        [TestMethod]
        [Description("Test if RMSSD is correct for healthy signal")]
        public void RMSSDTestNAF()
        {
            //init
            double[] testArray = { 85, 84, 82, 83, 84, 89, 94, 101, 101, 99, 97, 96, 95, 95, 97, 98, 97, 94, 95, 98, 95, 97, 101, 103, 95, 95, 97, 98, 97, 97, 100, 100 };
            double result = 0.0301;
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { testVector };
            double delta = 0.001;
            //test
            double realresult = Convert.ToDouble(obj.Invoke("RMSSD", args));
            //results
            Assert.AreEqual(result, realresult, delta);
        }

        [TestMethod]
        [Description("Test if RMSSD is correct for AF signal")]
        public void RMSSDTestAF()
        {
            //init
            double[] testArray = { 137, 147, 156, 129, 130, 123, 129, 138, 138, 162, 138, 161, 123, 144, 157, 156, 142, 136, 118, 136, 158, 139, 178, 158, 164, 126, 135, 134, 173, 171, 170, 143 };
            double result = 0.1370;
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { testVector };
            double delta = 0.001;
            //test
            double realresult = Convert.ToDouble(obj.Invoke("RMSSD", args));
            //results
            Assert.AreEqual(result, realresult, delta);
        }

        [TestMethod]
        [Description("Test if detection of AF is correct for healthy signal")]
        public void detectAFStatTestNAF()
        {
            //init
            double[] testArray = { 85, 84, 82, 83, 84, 89, 94, 101, 101, 99, 97, 96, 95, 95, 97, 98, 97, 94, 95, 98, 95, 97, 101, 103, 95, 95, 97, 98, 97, 97, 100, 100 };
            bool result = false;
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { testVector };
            //test
            bool realresult = Convert.ToBoolean(obj.Invoke("detectAFStat", args));
            //results
            Assert.AreEqual(result, realresult);
        }

        [TestMethod]
        [Description("Test if detection of AF is correct for AF signal")]
        public void detectAFStatTestAF()
        {
            //init
            double[] testArray = { 137, 147, 156, 129, 130, 123, 129, 138, 138, 162, 138, 161, 123, 144, 157, 156, 142, 136, 118, 136, 158, 139, 178, 158, 164, 126, 135, 134, 173, 171, 170, 143 };
            bool result = true;
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { testVector };
            //test
            bool realresult = Convert.ToBoolean(obj.Invoke("detectAFStat", args));
            //results
            Assert.AreEqual(result, realresult);
        }

        [TestMethod]
        [Description("Test if detection of AF is correct for healthy signal")]
        public void detectAFTestNAFStat()
        {
            //init
            double[] rrintArray = { 85, 84, 82, 83, 84, 89, 94, 101, 101, 99, 97, 96, 95, 95, 97, 98, 97, 94, 95, 98, 95, 97, 101, 103, 95, 95, 97, 98, 97, 97, 100, 100 };
            double[] rpeaksArray = { 720061, 720146, 720230, 720312, 720395, 720479, 720568, 720662, 720763, 720864, 720963, 721060, 721156, 721251, 721346, 721443, 721541, 721638, 721732, 721827, 721925, 722020, 722117, 722218, 722321, 722416, 722511, 722608, 722706, 722803, 722900, 723000 };
            double[] resultArray = { 0 };
            Vector<double> rrintVector = Vector<double>.Build.DenseOfArray(rrintArray);
            Vector<double> rpeaksVector = Vector<double>.Build.DenseOfArray(rpeaksArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);
            uint fs = 128;
            bool resultbool = false;
            double resultdouble = 0;
            Atrial_Fibr_Params param = new Atrial_Fibr_Params(Detect_Method.STATISTIC, "test");
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { rrintVector, rpeaksVector, fs, param };
            Tuple<bool, Vector<double>, double> result = Tuple.Create(resultbool, resultVector, resultdouble);
            //test
            Tuple<bool, Vector<double>, double> realresult = (Tuple<bool, Vector<double>, double>)obj.Invoke("detectAF", args);
            //results
            Assert.AreEqual(result, realresult);
        }

        [TestMethod]
        [Description("Test if detection of AF is correct for AF signal")]
        public void detectAFTestAFStat()
        {
            //init
            double[] rrintArray = { 137, 147, 156, 129, 130, 123, 129, 138, 138, 162, 138, 161, 123, 144, 157, 156, 142, 136, 118, 136, 158, 139, 178, 158, 164, 126, 135, 134, 173, 171, 170, 143 };
            double[] rpeaksArray = { 140, 277, 424, 580, 709, 839, 962, 1091, 1229, 1367, 1529, 1667, 1828, 1951, 2095, 2252, 2408, 2550, 2686, 2804, 2940, 3098, 3237, 3415, 3573, 3737, 3863, 3998, 4132, 4305, 4476, 4646 };
            double[] resultArray = new double[4646 - 140 + 143];
            for (int i = 0; i < 4649; i++)
            {
                resultArray[i] = i + 140;
            }
            Vector<double> rrintVector = Vector<double>.Build.DenseOfArray(rrintArray);
            Vector<double> rpeaksVector = Vector<double>.Build.DenseOfArray(rpeaksArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);
            uint fs = 250;
            bool resultbool = true;
            double resultdouble = 18.596;
            Atrial_Fibr_Params param = new Atrial_Fibr_Params(Detect_Method.STATISTIC, "test");
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { rrintVector, rpeaksVector, fs, param };
            Tuple<bool, Vector<double>, double> result = Tuple.Create(resultbool, resultVector, resultdouble);
            //test
            Tuple<bool, Vector<double>, double> realresult = (Tuple<bool, Vector<double>, double>)obj.Invoke("detectAF", args);
            //results
            Assert.AreEqual(realresult.Item1, result.Item1);
            Assert.AreEqual(realresult.Item2, result.Item2);
            Assert.AreEqual(realresult.Item3, result.Item3, 0.001);
        }
        [TestMethod]
        [Description("Test if detection of AF is correct for signal with 0 and negative rr intervals")]
        public void detectAFTestwrongrr()
        {
            //init
            double[] rrintArray = { 85, 84, 82, 83, 84, 0, 94, 101, 101, 99, 97, -10, 95, 95, 97, 98, 97, 94, 95, 98, 95, 0, -1, 103, 95, 95, 97, 98, 97, 97, 100, 100 };
            double[] rpeaksArray = { 720061, 720146, 720230, 720312, 720395, 720479, 720568, 720662, 720763, 720864, 720963, 721060, 721156, 721251, 721346, 721443, 721541, 721638, 721732, 721827, 721925, 722020, 722117, 722218, 722321, 722416, 722511, 722608, 722706, 722803, 722900, 723000 };
            double[] resultArray = { 0 };
            Vector<double> rrintVector = Vector<double>.Build.DenseOfArray(rrintArray);
            Vector<double> rpeaksVector = Vector<double>.Build.DenseOfArray(rpeaksArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);
            uint fs = 128;
            bool resultbool = false;
            double resultdouble = 0;
            Atrial_Fibr_Params param = new Atrial_Fibr_Params(Detect_Method.STATISTIC, "test");
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { rrintVector, rpeaksVector, fs, param };
            Tuple<bool, Vector<double>, double> result = Tuple.Create(resultbool, resultVector, resultdouble);
            //test
            Tuple<bool, Vector<double>, double> realresult = (Tuple<bool, Vector<double>, double>)obj.Invoke("detectAF", args);
            //results
            Assert.AreEqual(result, realresult);
        }
        [TestMethod]
        [Description("Test if creating list of data points for clusterization is correct.")]
        public void InitilizeRawDataTest()
        {
            //init
            double[] testArray1 = { 137, 147, 156, 129, 130, 123, 129, 138, 138, 162, 138 };
            double[] testArray2 = { 147, 156, 129, 130, 123, 129, 138, 138, 162, 138, 161 };
            List<Atrial_Fibr_Alg.DataPoint> result = new List<Atrial_Fibr_Alg.DataPoint>();
            result.Add(new Atrial_Fibr_Alg.DataPoint(137, 147));
            result.Add(new Atrial_Fibr_Alg.DataPoint(147, 156));
            result.Add(new Atrial_Fibr_Alg.DataPoint(156, 129));
            result.Add(new Atrial_Fibr_Alg.DataPoint(129, 130));
            result.Add(new Atrial_Fibr_Alg.DataPoint(130, 123));
            result.Add(new Atrial_Fibr_Alg.DataPoint(123, 129));
            result.Add(new Atrial_Fibr_Alg.DataPoint(129, 138));
            result.Add(new Atrial_Fibr_Alg.DataPoint(138, 138));
            result.Add(new Atrial_Fibr_Alg.DataPoint(138, 162));
            result.Add(new Atrial_Fibr_Alg.DataPoint(162, 138));
            result.Add(new Atrial_Fibr_Alg.DataPoint(138, 161));
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { testArray1, testArray2 };
            //test
            List<Atrial_Fibr_Alg.DataPoint> realresult = (List<Atrial_Fibr_Alg.DataPoint>)obj.Invoke("InitilizeRawData", args);
            //results
            CollectionAssert.AreEqual(result, realresult);
        }

        [TestMethod]
        [Description("Test if min index in array is correct")]
        public void MinIndexTest()
        {
            //init
            double[] testArray = { 137, 147, 156, 129, 130, 123.0, 129, 138, 138, 162, 138, 161, 123.4, 144, 157, 156, 142, 136, 118.9, 136, 158, 139, 178, 158, 164, 126, 135, 134, 173, 171, 170, 143 };
            double result = 18;
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { testArray };
            //test
            double realresult = Convert.ToDouble(obj.Invoke("MinIndex", args));
            //results
            Assert.AreEqual(result, realresult);
        }

        [TestMethod]
        [Description("Test if ElucidanDistance in array is correct")]
        public void ElucidanDistanceTest()
        {
            //init
            Atrial_Fibr_Alg.DataPoint dataPoint = new Atrial_Fibr_Alg.DataPoint(105, 125);
            Atrial_Fibr_Alg.DataPoint mean = new Atrial_Fibr_Alg.DataPoint(100, 100);
            double result = 25.495;
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { dataPoint, mean };
            //test
            double realresult = Convert.ToDouble(obj.Invoke("ElucidanDistance", args));
            //results
            Assert.AreEqual(result, realresult, 0.001);
        }

        [TestMethod]
        [Description("Test if Cluster is really empty")]
        public void EmptyClusterTest()
        {
            //init
            List<Atrial_Fibr_Alg.DataPoint> data = new List<Atrial_Fibr_Alg.DataPoint>();
            data.Add(new Atrial_Fibr_Alg.DataPoint(137, 147, 0));
            data.Add(new Atrial_Fibr_Alg.DataPoint(147, 156, 1));
            data.Add(new Atrial_Fibr_Alg.DataPoint(156, 129, 2));
            data.Add(new Atrial_Fibr_Alg.DataPoint(129, 130, 1));
            data.Add(new Atrial_Fibr_Alg.DataPoint(130, 123, 1));
            bool result = true;
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            obj.SetField("amountOfCluster", 4);
            object[] args = { data };
            //test
            bool realresult = Convert.ToBoolean(obj.Invoke("EmptyCluster", args));
            //results
            Assert.AreEqual(result, realresult);
        }
        [TestMethod]
        [Description("Test if Cluster is  empty when it's not")]
        public void EmptyClusterTest2()
        {
            //init
            List<Atrial_Fibr_Alg.DataPoint> data = new List<Atrial_Fibr_Alg.DataPoint>();
            data.Add(new Atrial_Fibr_Alg.DataPoint(137, 147, 0));
            data.Add(new Atrial_Fibr_Alg.DataPoint(147, 156, 1));
            data.Add(new Atrial_Fibr_Alg.DataPoint(156, 129, 2));
            data.Add(new Atrial_Fibr_Alg.DataPoint(129, 130, 1));
            data.Add(new Atrial_Fibr_Alg.DataPoint(130, 123, 1));
            bool result = false;
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            obj.SetField("amountOfCluster", 3);
            object[] args = { data };
            //test
            bool realresult = Convert.ToBoolean(obj.Invoke("EmptyCluster", args));
            //results
            Assert.AreEqual(result, realresult);
        }
        [TestMethod]
        [Description("Test if d coefficient is correct for AF signal")]
        public void dCoeffTestAF()
        {
            //init
            double[] rrArray = { 137, 147, 156, 129, 130, 123, 129, 138, 138, 162, 138, 161, 123, 144, 157, 156, 142, 136, 118, 136, 158, 139, 178, 158, 164, 126, 135, 134, 173, 171 };
            double result = 0.0610;
            double delta = 0.0001;
            double[] Ii = { 137, 147, 156, 129, 130, 123, 129, 138, 138, 162, 138, 161, 123, 144, 157, 156, 142, 136, 118, 136, 158, 139, 178, 158, 164, 126, 135, 134, 173 };
            double[] Ii1 = { 147, 156, 129, 130, 123, 129, 138, 138, 162, 138, 161, 123, 144, 157, 156, 142, 136, 118, 136, 158, 139, 178, 158, 164, 126, 135, 134, 173, 171 };
            Vector<double> rrVector = Vector<double>.Build.DenseOfArray(rrArray);
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { Ii, Ii1, rrVector };
            //test
            double realresult = Convert.ToDouble(obj.Invoke("dCoeff", args));
            //results
            Assert.AreEqual(result, realresult, delta);
        }

        [TestMethod]
        [Description("Test if d coefficient is correct for healthy signal")]
        public void dCoeffTestNAF()
        {
            //init
            double[] rrArray = { 85, 84, 82, 83, 84, 89, 94, 101, 101, 99, 97, 96, 95, 95, 97, 98, 97, 94, 95, 98, 95, 97, 101, 103, 95, 95, 97, 98, 97, 97 };
            double result = 0.0152;
            double delta = 0.0001;
            double[] Ii = { 85, 84, 82, 83, 84, 89, 94, 101, 101, 99, 97, 96, 95, 95, 97, 98, 97, 94, 95, 98, 95, 97, 101, 103, 95, 95, 97, 98, 97 };
            double[] Ii1 = { 84, 82, 83, 84, 89, 94, 101, 101, 99, 97, 96, 95, 95, 97, 98, 97, 94, 95, 98, 95, 97, 101, 103, 95, 95, 97, 98, 97, 97 };
            Vector<double> rrVector = Vector<double>.Build.DenseOfArray(rrArray);
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { Ii, Ii1, rrVector };
            //test
            double realresult = Convert.ToDouble(obj.Invoke("dCoeff", args));
            //results
            Assert.AreEqual(result, realresult, delta);
        }

        [TestMethod]
        [Description("Test if AF detection is correct for healthy signal")]
        public void detectAFPoinTestNAF()
        {
            //init
            double[] rrArray = { 85, 84, 82, 83, 84, 89, 94, 101, 101, 99, 97, 96, 95, 95, 97, 98, 97, 94, 95, 98, 95, 97, 101, 103, 95, 95, 97, 98, 97, 97 };
            bool result = false;
            Vector<double> rrVector = Vector<double>.Build.DenseOfArray(rrArray);
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { rrVector };
            //test
            bool realresult = Convert.ToBoolean(obj.Invoke("detectAFPoin", args));
            //results
            Assert.AreEqual(result, realresult);
        }

        [TestMethod]
        [Description("Test if AF detection is correct for AF signal")]
        public void detectAFPoinTestAF()
        {
            //init
            double[] rrArray = { 137, 147, 156, 129, 130, 123, 129, 138, 138, 162, 138, 161, 123, 144, 157, 156, 142, 136, 118, 136, 158, 139, 178, 158, 164, 126, 135, 134, 173, 171 };
            bool result = true;
            Vector<double> rrVector = Vector<double>.Build.DenseOfArray(rrArray);
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { rrVector };
            //test
            bool realresult = Convert.ToBoolean(obj.Invoke("detectAFPoin", args));
            //results
            Assert.AreEqual(result, realresult);
        }

        [TestMethod]
        [Description("Test if detection of AF is correct for healthy signal")]
        public void detectAFTestNAFPoin()
        {
            //init
            double[] rrintArray = { 85, 84, 82, 83, 84, 89, 94, 101, 101, 99, 97, 96, 95, 95, 97, 98, 97, 94, 95, 98, 95, 97, 101, 103, 95, 95, 97, 98, 97, 97 };
            double[] rpeaksArray = { 720061, 720146, 720230, 720312, 720395, 720479, 720568, 720662, 720763, 720864, 720963, 721060, 721156, 721251, 721346, 721443, 721541, 721638, 721732, 721827, 721925, 722020, 722117, 722218, 722321, 722416, 722511, 722608, 722706, 722803 };
            double[] resultArray = { 0 };
            Vector<double> rrintVector = Vector<double>.Build.DenseOfArray(rrintArray);
            Vector<double> rpeaksVector = Vector<double>.Build.DenseOfArray(rpeaksArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);
            uint fs = 128;
            bool resultbool = false;
            double resultdouble = 0;
            Atrial_Fibr_Params param = new Atrial_Fibr_Params(Detect_Method.POINCARE, "test");
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { rrintVector, rpeaksVector, fs, param };
            Tuple<bool, Vector<double>, double> result = Tuple.Create(resultbool, resultVector, resultdouble);
            //test
            Tuple<bool, Vector<double>, double> realresult = (Tuple<bool, Vector<double>, double>)obj.Invoke("detectAF", args);
            //results
            Assert.AreEqual(result, realresult);
        }

        [TestMethod]
        [Description("Test if detection of AF is correct for AF signal")]
        public void detectAFTestAFPoin()
        {
            //init
            double[] rrintArray = { 137, 147, 156, 129, 130, 123, 129, 138, 138, 162, 138, 161, 123, 144, 157, 156, 142, 136, 118, 136, 158, 139, 178, 158, 164, 126, 135, 134, 173, 171 };
            double[] rpeaksArray = { 140, 277, 424, 580, 709, 839, 962, 1091, 1229, 1367, 1529, 1667, 1828, 1951, 2095, 2252, 2408, 2550, 2686, 2804, 2940, 3098, 3237, 3415, 3573, 3737, 3863, 3998, 4132, 4305 };
            double[] resultArray = new double[4305 - 140 + 171];
            for (int i = 0; i < (4305 - 140 + 171); i++)
            {
                resultArray[i] = i + 140;
            }
            Vector<double> rrintVector = Vector<double>.Build.DenseOfArray(rrintArray);
            Vector<double> rpeaksVector = Vector<double>.Build.DenseOfArray(rpeaksArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);
            uint fs = 250;
            bool resultbool = true;
            double resultdouble = 17.344;
            Atrial_Fibr_Params param = new Atrial_Fibr_Params(Detect_Method.POINCARE, "test");
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { rrintVector, rpeaksVector, fs, param };
            Tuple<bool, Vector<double>, double> result = Tuple.Create(resultbool, resultVector, resultdouble);
            //test
            Tuple<bool, Vector<double>, double> realresult = (Tuple<bool, Vector<double>, double>)obj.Invoke("detectAF", args);
            //results
            Assert.AreEqual(realresult.Item1, result.Item1);
            Assert.AreEqual(realresult.Item2, result.Item2);
            Assert.AreEqual(realresult.Item3, result.Item3, 0.001);
        }
        [TestMethod]
        [Description("Test if data normalization is correct")]
        public void NormalizeDataTest()
        {
            //init
            List<Atrial_Fibr_Alg.DataPoint> data = new List<Atrial_Fibr_Alg.DataPoint>();
            data.Add(new Atrial_Fibr_Alg.DataPoint(137, 147));
            data.Add(new Atrial_Fibr_Alg.DataPoint(147, 156));
            data.Add(new Atrial_Fibr_Alg.DataPoint(156, 129));
            data.Add(new Atrial_Fibr_Alg.DataPoint(129, 130));
            data.Add(new Atrial_Fibr_Alg.DataPoint(130, 123));
            List<Atrial_Fibr_Alg.DataPoint> result = new List<Atrial_Fibr_Alg.DataPoint>();
            result.Add(new Atrial_Fibr_Alg.DataPoint(-0.0262, 0.0649));
            result.Add(new Atrial_Fibr_Alg.DataPoint(0.0673, 0.1234));
            result.Add(new Atrial_Fibr_Alg.DataPoint(0.1515, -0.0519));
            result.Add(new Atrial_Fibr_Alg.DataPoint(-0.1010, -0.0455));
            result.Add(new Atrial_Fibr_Alg.DataPoint(-0.0916, -0.0909));
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { data };
            //test
            List<Atrial_Fibr_Alg.DataPoint> realresult = (List<Atrial_Fibr_Alg.DataPoint>)obj.Invoke("NormalizeData", args);
            //results
            CollectionAssert.AreEqual(result, realresult);
        }

        [TestMethod]
        [Description("Test if updating means of clusters is correct")]
        public void UpdateDataPointMeansTest()
        {
            //init
            List<Atrial_Fibr_Alg.DataPoint> data = new List<Atrial_Fibr_Alg.DataPoint>();
            data.Add(new Atrial_Fibr_Alg.DataPoint(137, 147, 0));
            data.Add(new Atrial_Fibr_Alg.DataPoint(147, 156, 0));
            data.Add(new Atrial_Fibr_Alg.DataPoint(156, 129, 1));
            data.Add(new Atrial_Fibr_Alg.DataPoint(129, 130, 2));
            data.Add(new Atrial_Fibr_Alg.DataPoint(130, 123, 0));
            List<Atrial_Fibr_Alg.DataPoint> clusters = new List<Atrial_Fibr_Alg.DataPoint>();
            List<Atrial_Fibr_Alg.DataPoint> resultList = new List<Atrial_Fibr_Alg.DataPoint>();
            resultList.Add(new Atrial_Fibr_Alg.DataPoint(138, 142));
            resultList.Add(new Atrial_Fibr_Alg.DataPoint(156, 129));
            resultList.Add(new Atrial_Fibr_Alg.DataPoint(129, 130));
            bool resultBool = true;
            Tuple<bool, List<Atrial_Fibr_Alg.DataPoint>> result = new Tuple<bool, List<Atrial_Fibr_Alg.DataPoint>>(resultBool, resultList);

            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { data, clusters };
            //test
            Tuple<bool, List<Atrial_Fibr_Alg.DataPoint>> realresult = (Tuple<bool, List<Atrial_Fibr_Alg.DataPoint>>)obj.Invoke("UpdateDataPointMeans", args);
            //results
            Assert.AreEqual(result.Item1, realresult.Item1);
            CollectionAssert.AreEqual(result.Item2, realresult.Item2);
        }
        [TestMethod]
        [Description("Test if resulting false when some cluster is empty")]
        public void UpdateDataPointMeansTest2()
        {
            //init
            List<Atrial_Fibr_Alg.DataPoint> data = new List<Atrial_Fibr_Alg.DataPoint>();
            data.Add(new Atrial_Fibr_Alg.DataPoint(137, 147, 0));
            data.Add(new Atrial_Fibr_Alg.DataPoint(147, 156, 0));
            data.Add(new Atrial_Fibr_Alg.DataPoint(156, 129, 1));
            data.Add(new Atrial_Fibr_Alg.DataPoint(129, 130, 2));
            data.Add(new Atrial_Fibr_Alg.DataPoint(130, 123, 0));
            List<Atrial_Fibr_Alg.DataPoint> clusters = new List<Atrial_Fibr_Alg.DataPoint>();
            bool resultBool = false;
            Tuple<bool, List<Atrial_Fibr_Alg.DataPoint>> result = new Tuple<bool, List<Atrial_Fibr_Alg.DataPoint>>(resultBool, clusters);
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            obj.SetField("amountOfCluster", 4);
            object[] args = { data, clusters };
            //test
            Tuple<bool, List<Atrial_Fibr_Alg.DataPoint>> realresult = (Tuple<bool, List<Atrial_Fibr_Alg.DataPoint>>)obj.Invoke("UpdateDataPointMeans", args);
            //results
            Assert.AreEqual(result.Item1, realresult.Item1);
            CollectionAssert.AreEqual(result.Item2, realresult.Item2);
        }

        [TestMethod]
        [Description("Test if updating cluster membership is correct")]
        public void UpdateClusterMembershipTest()
        {
            //init
            List<Atrial_Fibr_Alg.DataPoint> data = new List<Atrial_Fibr_Alg.DataPoint>();
            data.Add(new Atrial_Fibr_Alg.DataPoint(5, 5, 0));
            data.Add(new Atrial_Fibr_Alg.DataPoint(5, 5, 0));
            data.Add(new Atrial_Fibr_Alg.DataPoint(10, 10, 1));
            data.Add(new Atrial_Fibr_Alg.DataPoint(10, 10, 1));
            data.Add(new Atrial_Fibr_Alg.DataPoint(15, 15, 2));
            data.Add(new Atrial_Fibr_Alg.DataPoint(15, 15, 2));
            data.Add(new Atrial_Fibr_Alg.DataPoint(6, 6, 1));
            data.Add(new Atrial_Fibr_Alg.DataPoint(11, 11, 2));
            List<Atrial_Fibr_Alg.DataPoint> data2 = new List<Atrial_Fibr_Alg.DataPoint>();
            data2.Add(new Atrial_Fibr_Alg.DataPoint(-0.3193, -0.3193, 0));
            data2.Add(new Atrial_Fibr_Alg.DataPoint(-0.3193, -0.3193, 0));
            data2.Add(new Atrial_Fibr_Alg.DataPoint(0.0259, 0.0259, 1));
            data2.Add(new Atrial_Fibr_Alg.DataPoint(0.0259, 0.0259, 1));
            data2.Add(new Atrial_Fibr_Alg.DataPoint(0.3711, 0.3711, 2));
            data2.Add(new Atrial_Fibr_Alg.DataPoint(0.3711, 0.3711, 2));
            data2.Add(new Atrial_Fibr_Alg.DataPoint(-0.2503, -0.2503, 1));
            data2.Add(new Atrial_Fibr_Alg.DataPoint(0.0949, 0.0949, 2));
            List<Atrial_Fibr_Alg.DataPoint> resut1 = new List<Atrial_Fibr_Alg.DataPoint>();
            resut1.Add(new Atrial_Fibr_Alg.DataPoint(5, 5, 0));
            resut1.Add(new Atrial_Fibr_Alg.DataPoint(5, 5, 0));
            resut1.Add(new Atrial_Fibr_Alg.DataPoint(10, 10, 1));
            resut1.Add(new Atrial_Fibr_Alg.DataPoint(10, 10, 1));
            resut1.Add(new Atrial_Fibr_Alg.DataPoint(15, 15, 2));
            resut1.Add(new Atrial_Fibr_Alg.DataPoint(15, 15, 2));
            resut1.Add(new Atrial_Fibr_Alg.DataPoint(6, 6, 0));
            resut1.Add(new Atrial_Fibr_Alg.DataPoint(11, 11, 1));
            List<Atrial_Fibr_Alg.DataPoint> result2 = new List<Atrial_Fibr_Alg.DataPoint>();
            result2.Add(new Atrial_Fibr_Alg.DataPoint(-0.3193, -0.3193, 0));
            result2.Add(new Atrial_Fibr_Alg.DataPoint(-0.3193, -0.3193, 0));
            result2.Add(new Atrial_Fibr_Alg.DataPoint(0.0259, 0.0259, 1));
            result2.Add(new Atrial_Fibr_Alg.DataPoint(0.0259, 0.0259, 1));
            result2.Add(new Atrial_Fibr_Alg.DataPoint(0.3711, 0.3711, 2));
            result2.Add(new Atrial_Fibr_Alg.DataPoint(0.3711, 0.3711, 2));
            result2.Add(new Atrial_Fibr_Alg.DataPoint(-0.2503, -0.2503, 0));
            result2.Add(new Atrial_Fibr_Alg.DataPoint(0.0949, 0.0949, 1));
            List<Atrial_Fibr_Alg.DataPoint> clusters = new List<Atrial_Fibr_Alg.DataPoint>();
            clusters.Add(new Atrial_Fibr_Alg.DataPoint(-0.3193, -0.3193, 0));
            clusters.Add(new Atrial_Fibr_Alg.DataPoint(-0.0662, -0.0662, 1));
            clusters.Add(new Atrial_Fibr_Alg.DataPoint(0.2790, 0.2790, 2));
            bool resultBool = true;
            Tuple<bool, List<Atrial_Fibr_Alg.DataPoint>, List<Atrial_Fibr_Alg.DataPoint>> result = new Tuple<bool, List<Atrial_Fibr_Alg.DataPoint>, List<Atrial_Fibr_Alg.DataPoint>>(resultBool, result2, resut1);
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            obj.SetField("amountOfCluster", 3);
            object[] args = { data, data2, clusters };
            //test
            Tuple<bool, List<Atrial_Fibr_Alg.DataPoint>, List<Atrial_Fibr_Alg.DataPoint>> realresult = (Tuple<bool, List<Atrial_Fibr_Alg.DataPoint>, List<Atrial_Fibr_Alg.DataPoint>>)obj.Invoke("UpdateClusterMembership", args);
            //results
            Assert.AreEqual(result.Item1, realresult.Item1);
            CollectionAssert.AreEqual(result.Item2, realresult.Item2);
            CollectionAssert.AreEqual(result.Item3, realresult.Item3);
        }

        [TestMethod]
        [Description("Test if SilhouetteCoefficient is correct")]
        public void SilhouetteCoefficientTest()
        {
            //init
            List<Atrial_Fibr_Alg.DataPoint> raw_data = new List<Atrial_Fibr_Alg.DataPoint>();
            raw_data.Add(new Atrial_Fibr_Alg.DataPoint(137, 147, 0));
            raw_data.Add(new Atrial_Fibr_Alg.DataPoint(147, 156, 0));
            raw_data.Add(new Atrial_Fibr_Alg.DataPoint(156, 129, 1));
            raw_data.Add(new Atrial_Fibr_Alg.DataPoint(129, 130, 2));
            raw_data.Add(new Atrial_Fibr_Alg.DataPoint(130, 123, 1));
            double result = 0.447;
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { raw_data };
            //test
            double realresult = Convert.ToDouble(obj.Invoke("SilhouetteCoefficient", args));
            //results
            Assert.AreEqual(result, realresult, 0.001);
        }

    }
}
