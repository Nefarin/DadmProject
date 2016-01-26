using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.Atrial_Fibr;

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
            for (int i = 0; i < 4506; i++)
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
            Assert.AreEqual(realresult.Item1,result.Item1 );
            Assert.AreEqual(realresult.Item2, result.Item2);
            Assert.AreEqual(realresult.Item3, result.Item3, 0.001);
        }
    }
}
