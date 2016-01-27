using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.HRT;

namespace EKG_Unit.Modules.HRT
{
    [TestClass]
    public class HRT_Alg_Test
    {
        //ChangeVectorIntoTimeDomain
        [TestMethod]
        [Description("Test if vector changes into time domain properly - equality test")]
        public void ChangeVectorIntoTimeDomainTest1()
        {
            HRT_Params testParams = new HRT_Params("Test");

            double[] testArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            double[] resultArray = { 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
            int samplingFreq =  100 ;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            HRT_Alg testAlg = new HRT_Alg();
            Vector<double> testResult = testAlg.ChangeVectorIntoTimeDomain(testVector, samplingFreq);
            Assert.AreEqual(testResult, resultVector);

        }

        [TestMethod]
        [Description("Test if vector changes into time domain properly - not equality test")]
        public void ChangeVectorIntoTimeDomainTest2()
        {
            HRT_Params testParams = new HRT_Params("Test");

            double[] testArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            double[] resultArray = { 0, 10, 20, 31, 40, 50, 60, 70, 81, 90, 100 };
            int samplingFreq = 100;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            HRT_Alg testAlg = new HRT_Alg();
            Vector<double> testResult = testAlg.ChangeVectorIntoTimeDomain(testVector, samplingFreq);
            Assert.AreNotEqual(testResult, resultVector);

        }

        [TestMethod]
        [Description("Test if ChangeVectorIntoTimeDomain throws null if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void ChangeVectorIntoTimeDomainNullTest()
        {
            HRT_Params testParams = new HRT_Params("Test");

            Vector<double> testVector = null;
            int samplingFreq = 100;

            HRT_Alg testAlg = new HRT_Alg();
            Vector<double> testResult = testAlg.ChangeVectorIntoTimeDomain(testVector, samplingFreq);
            
        }

        [TestMethod]
        [Description("Test if ChangeVectorIntoTimeDomain throws null if argument is out of range")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Parameter out of range")]
        public void ChangeVectorIntoTimeDomainOutOfRangeTest()
        {
            HRT_Params testParams = new HRT_Params("Test");

            double[] testArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            int samplingFreq = 0;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            HRT_Alg testAlg = new HRT_Alg();
            Vector<double> testResult = testAlg.ChangeVectorIntoTimeDomain(testVector, samplingFreq);

        }

        //GetNrVPC
        [TestMethod]
        [Description("Test if funcion properly finds VPC nr by R_peak index - equality test")]
        public void GetNrVPC1()
        {
            HRT_Params testParams = new HRT_Params("Test");

            double[] testrrTimesArray = { 0, 15, 30, 45, 60, 75, 90, 105 };
            int[] testrrTimesVPCArray = { 30, 60, 105 };
            int testVPCcount = 3;

            double[] testnrVPCArray = { 2, 4, 7 };

            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(testnrVPCArray);

            HRT_Alg testAlg = new HRT_Alg();
            Vector<double> testResult = testAlg.GetNrVPC(testrrTimesArray, testrrTimesVPCArray, testVPCcount);
            Assert.AreEqual(testResult, resultVector);

        }

        [TestMethod]
        [Description("Test if funcion properly finds VPC nr by R_peak index - not equality test")]
        public void GetNrVPC2()
        {
            HRT_Params testParams = new HRT_Params("Test");

            double[] testrrTimesArray = { 0, 15, 30, 45, 60, 75, 90, 105 };
            int[] testrrTimesVPCArray = { 30, 60, 155 };
            int testVPCcount = 3;

            double[] testnrVPCArray = { 2, 4, 7 };

            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(testnrVPCArray);

            HRT_Alg testAlg = new HRT_Alg();
            Vector<double> testResult = testAlg.GetNrVPC(testrrTimesArray, testrrTimesVPCArray, testVPCcount);
            Assert.AreNotEqual(testResult, resultVector);

        }

        [TestMethod]
        [Description("Test if GetNrVPC throws null if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void GetNrVPCNullTest()
        {
            HRT_Params testParams = new HRT_Params("Test");

            double[] testrrTimesArray = null;
            int[] testrrTimesVPCArray = { 30, 60, 155 };
            int testVPCcount = 3;

            HRT_Alg testAlg = new HRT_Alg();
            Vector<double> testResult = testAlg.GetNrVPC(testrrTimesArray, testrrTimesVPCArray, testVPCcount);
            
        }

        [TestMethod]
        [Description("Test if GetNrVPC throws null if argument is out of ranged")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Parameter out of range")]
        public void GetNrVPCOutOfRangeTest1()
        {
            HRT_Params testParams = new HRT_Params("Test");

            double[] testrrTimesArray = { 0, 15, 30, 45, 60, 75, 90, 105 };
            int[] testrrTimesVPCArray = { 30, 60, 155 };
            int testVPCcount = 4;

            HRT_Alg testAlg = new HRT_Alg();
            Vector<double> testResult = testAlg.GetNrVPC(testrrTimesArray, testrrTimesVPCArray, testVPCcount);

        }

        [TestMethod]
        [Description("Test if GetNrVPC throws null if argument is out of ranged")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "rrTimesVPC array is larger than rrTimes array")]
        public void GetNrVPCOutOfRangeTest2()
        {
            HRT_Params testParams = new HRT_Params("Test");

            double[] testrrTimesArray = { 30, 60, 155 };
            int[] testrrTimesVPCArray = { 0, 15, 30, 45, 60, 75, 90, 105 };
            int testVPCcount = 8;

            HRT_Alg testAlg = new HRT_Alg();
            Vector<double> testResult = testAlg.GetNrVPC(testrrTimesArray, testrrTimesVPCArray, testVPCcount);

        }

    }
}
