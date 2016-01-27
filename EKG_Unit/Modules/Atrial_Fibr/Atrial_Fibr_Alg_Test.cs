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
            double realresult= Convert.ToDouble(obj.Invoke("TPR",args));
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
            Assert.AreEqual(result, realresult,  delta);
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
            double[] rpeaksArray = { 720061, 720146, 720230, 720312, 720395, 720479, 720568, 720662, 720763, 720864, 720963, 721060, 721156, 721251, 721346, 721443, 721541, 721638, 721732, 721827, 721925, 722020, 722117, 722218, 722321, 722416, 722511, 722608, 722706, 722803, 722900, 723000};
            double[] resultArray = { 0};
            Vector<double> rrintVector = Vector<double>.Build.DenseOfArray(rrintArray);
            Vector<double> rpeaksVector = Vector<double>.Build.DenseOfArray(rpeaksArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);
            uint fs = 128;
            bool resultbool = false;
            double resultdouble = 0;
            Atrial_Fibr_Params param = new Atrial_Fibr_Params(Detect_Method.STATISTIC,"test");
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { rrintVector, rpeaksVector ,fs,param};
            Tuple<bool, Vector<double>, double> result = Tuple.Create(resultbool, resultVector, resultdouble);
            //test
            Tuple<bool, Vector<double>, double> realresult = (Tuple < bool, Vector< double >, double> )obj.Invoke("detectAF", args);
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
            double[] resultArray =new double [ 4646-140+143];
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
            for (int i=0; i< realresult.Item2.Count ; i++)
            {
                Console.WriteLine(realresult.Item2.At(i));
            }
            Assert.AreEqual(realresult.Item1,result.Item1 );
            //for (int i = 0; i < realresult.Item2.Count; i++)
            //{
            //    Assert.AreEqual(realresult.Item2.At(i), result.Item2.At(i));
            //}
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
            double[] testArray1 = { 137, 147, 156, 129, 130, 123, 129, 138, 138, 162, 138};
            double[] testArray2 = { 147, 156, 129, 130, 123, 129, 138, 138, 162, 138, 161};
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
            object[] args = { testArray1 , testArray2 };
            //test
            List<Atrial_Fibr_Alg.DataPoint> realresult = (List<Atrial_Fibr_Alg.DataPoint>)obj.Invoke("InitilizeRawData", args);
            //results
            Console.WriteLine("predykcje");
            //foreach (Atrial_Fibr_Alg.DataPoint re in result)
            //{
            //    Console.WriteLine(re.A);
            //    Console.WriteLine(re.B);
            //    Console.WriteLine(re.Cluster);
            //}

            //Console.WriteLine("wyniki");
            //foreach (Atrial_Fibr_Alg.DataPoint re in realresult)
            //{
            //    Console.WriteLine(re.A);
            //    Console.WriteLine(re.B);
            //    Console.WriteLine(re.Cluster);
            //}
            Assert.AreEqual(result.Count, realresult.Count);
            for (int i= 0;i< result.Count; i++)
            {
                Assert.AreEqual(result[i].A, realresult[i].A);
                Assert.AreEqual(result[i].B, realresult[i].B);
                Assert.AreEqual(result[i].Cluster, realresult[i].Cluster);
                Assert.AreEqual(result[i], realresult[i]);

            }
            //Assert.AreEqual(result, realresult);


        }
    }
}
