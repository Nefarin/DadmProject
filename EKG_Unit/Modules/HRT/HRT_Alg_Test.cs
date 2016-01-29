using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.HRT;
using System.Collections.Generic;
using System.IO;

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

            int[] resultnrVPCArray = { 2, 4, 7 };
            
            HRT_Alg testAlg = new HRT_Alg();
           // int[] testResult = testAlg.GetNrVPC(testrrTimesArray, testrrTimesVPCArray, testVPCcount);
            //Assert.AreEqual(resultnrVPCArray[2], testResult[2] );
           // CollectionAssert.AreEqual(resultnrVPCArray, testResult );

        }

        [TestMethod]
        [Description("Test if funcion properly finds VPC nr by R_peak index - not equality test")]
        public void GetNrVPC2()
        {
            HRT_Params testParams = new HRT_Params("Test");

            double[] testrrTimesArray = { 0, 15, 30, 45, 60, 75, 90, 105 };
            int[] testrrTimesVPCArray = { 30, 60, 155 };
            int testVPCcount = 3;

            int[] resultnrVPCArray = { 2, 4, 7 };

            HRT_Alg testAlg = new HRT_Alg();
           // int[] testResult = testAlg.GetNrVPC(testrrTimesArray, testrrTimesVPCArray, testVPCcount);
           // Assert.AreNotEqual(testResult, resultnrVPCArray);

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
           // int[] testResult = testAlg.GetNrVPC(testrrTimesArray, testrrTimesVPCArray, testVPCcount);
            
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
          //  int[] testResult = testAlg.GetNrVPC(testrrTimesArray, testrrTimesVPCArray, testVPCcount);

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
           // int[] testResult = testAlg.GetNrVPC(testrrTimesArray, testrrTimesVPCArray, testVPCcount);

        }

        //removeRedundant
        [TestMethod]
        [Description("Test if funcion properly remove redundant elements - equality test")]
        public void removeRedundant1()
        {
            HRT_Params testParams = new HRT_Params("Test");

            int[] testArray = { 2, 4, 6, 8, 10, 12, 14 };
            int testLength = 3;

            int[] expectedArray = { 2, 4, 6, 8 };

            HRT_Alg testAlg = new HRT_Alg();
            int[] actualArray = testAlg.removeRedundant(testArray, testLength);
            CollectionAssert.AreEqual(actualArray, expectedArray);

        }

        [TestMethod]
        [Description("Test if removeRedundant throws null if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void removeRedundantNullTest()
        {
            HRT_Params testParams = new HRT_Params("Test");

            int[] testArray = null;
            int testLength = 3;

            HRT_Alg testAlg = new HRT_Alg();
            int[] actualArray = testAlg.removeRedundant(testArray, testLength);
            
        }

        [TestMethod]
        [Description("Test if removeRedundant throws null if argument is out of range")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Length parameter must be grather than 0 and smaller than given array size")]
        public void removeRedundantOutOfRangeTest()
        {
            HRT_Params testParams = new HRT_Params("Test");

            int[] testArray = { 2, 4, 6, 8 };
            int testLength = 5;

            HRT_Alg testAlg = new HRT_Alg();
            int[] actualArray = testAlg.removeRedundant(testArray, testLength);

        }

        //rrTimesShift
        [TestMethod]
        [Description("Test if vector shitfs properly - equality test")]
        public void rrTimesShift1()
        {
            HRT_Params testParams = new HRT_Params("Test");

            double[] testArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            double[] resultArray = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            HRT_Alg testAlg = new HRT_Alg();
            Vector<double> testResult = testAlg.rrTimesShift(testVector);
            Assert.AreEqual(testResult, resultVector);

        }

        //SearchPrematureTurbulences
        [TestMethod]
        [Description("Test function if properly searches premature turbulences - equality test")]
        public void SearchPrematureTurbulences_EQTest_1()
        {
            HRT_Params testParams = new HRT_Params("Test");
            
            // Init test here

            double[] testTachogram1 = { 733, 738, 702, 702, 427, 1005, 755, 758, 744, 722, 694, 722, 725, 721, 720, 725, 736, 733, 713, 694, 680 };
            double[] testTachogram2 = { 722, 694, 722, 725, 431, 1022, 761, 736, 733, 714, 694, 681, 722, 764, 736, 731, 722, 711, 700, 708, 711 };
            double[] testTachogram3 = { 700, 708, 711, 728, 489, 1006, 736, 708, 689, 697, 686, 733, 747, 742, 717, 733, 719, 694, 697, 708, 733 };
            double[] testTachogram4 = { 706, 703, 692, 742, 475, 1039, 728, 728, 697, 703, 711, 706, 728, 736, 764, 733, 692, 689, 458, 969, 753 };
            double[] testTachogram5 = { 764, 733, 692, 689, 458, 969,  753, 750, 736, 714, 728, 708, 692, 703, 711, 739, 753, 747, 708, 681, 706 };
            List<double[]> testTachogram = new List<double[]>();
            testTachogram.Add(testTachogram1);
            testTachogram.Add(testTachogram2);
            testTachogram.Add(testTachogram3);
            testTachogram.Add(testTachogram4);
            testTachogram.Add(testTachogram5);

            List<int> testPikVC = new List<int>();
            testPikVC.Add(5562);
            testPikVC.Add(7923);
            testPikVC.Add(12626);
            testPikVC.Add(21203);
            testPikVC.Add(21203);

            List<int> expectedPikVC = new List<int>();
            expectedPikVC.Add(5562);
            expectedPikVC.Add(7923);
            expectedPikVC.Add(12626);
            expectedPikVC.Add(21203);
            expectedPikVC.Add(21203);

            // Process test here

            HRT_Alg testAlg = new HRT_Alg();
            List<int> resultPikVC = testAlg.SearchPrematureTurbulences(testTachogram, testPikVC);
            
            // Assert results

             CollectionAssert.AreEqual(resultPikVC, expectedPikVC);
            
        }

        //SearchPrematureTurbulences
        [TestMethod]
        [Description("Test function if properly searches premature turbulences but for another set of parameters - equality test")]
        public void SearchPrematureTurbulences_EQTest_2()
        {
            HRT_Params testParams = new HRT_Params("Test");

            // Init test here

            double[] testTachogram1 = { 708, 717, 714, 686, 431, 889, 694, 686, 700, 708, 717, 697, 694, 683, 664, 656, 669, 725, 708, 706, 686 };
            double[] testTachogram2 = { 719, 697, 692, 656, 383, 975, 703, 700, 708, 714, 706, 686, 669, 669, 667, 711, 711, 697, 683, 667, 681 };
            double[] testTachogram3 = { 636, 664, 672, 678, 461, 914, 697, 672, 653, 636, 647, 667, 683, 703, 681, 672, 642, 661, 656, 678, 694 };
            double[] testTachogram4 = { 642, 647, 644, 653, 561, 831, 664, 639, 639, 617, 442, 911, 689, 700, 672, 653, 642, 661, 669, 706, 725 };
            double[] testTachogram5 = { 664, 639, 639, 617, 442, 911, 689, 700, 672, 653, 642, 661, 669, 706, 725, 694, 681, 669, 681, 658, 669 };
            List<double[]> testTachogram = new List<double[]>();
            testTachogram.Add(testTachogram1);
            testTachogram.Add(testTachogram2);
            testTachogram.Add(testTachogram3);
            testTachogram.Add(testTachogram4);
            testTachogram.Add(testTachogram5);

            List<int> testPikVC = new List<int>();
            testPikVC.Add(568646);
            testPikVC.Add(579800);
            testPikVC.Add(608859);
            testPikVC.Add(617523); //and should be rejected - test check this
            testPikVC.Add(619136);

            List<int> expectedPikVC = new List<int>();
            expectedPikVC.Add(568646);
            expectedPikVC.Add(579800);
            expectedPikVC.Add(608859);
            expectedPikVC.Add(619136);

            // Process test here

            HRT_Alg testAlg = new HRT_Alg();
            List<int> resultPikVC = testAlg.SearchPrematureTurbulences(testTachogram, testPikVC);
            
            // Assert results

            CollectionAssert.AreEqual(resultPikVC, expectedPikVC);

        }

    }
}
