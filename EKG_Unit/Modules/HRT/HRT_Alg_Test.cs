using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.HRT;
using System.Collections.Generic;
using System.IO;

namespace EKG_Unit.Modules.HRT
{
    //Allows create and initalize List of 2-element tuples.
    public class TupleList<T1, T2> : List<Tuple<T1, T2>>
    {
        public void Add(T1 item, T2 item2)
        {
            Add(new Tuple<T1, T2>(item, item2));
        }
    }

    [TestClass]
    public class HRT_Alg_Test
    {
        //ChangeVectorIntoTimeDomain
        [TestMethod]
        [Description("Test if vector changes into time domain properly - equality test")]
        public void ChangeVectorIntoTimeDomain_EQTest_1()
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
        public void ChangeVectorIntoTimeDomain_NonEQTest_2()
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
        public void ChangeVectorIntoTimeDomain_NullTest()
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
        public void GetNrVPC_EQTest_1()
        {
            HRT_Params testParams = new HRT_Params("Test");

            // Init test here

            double[] testrrTimesArray = { 100, 1000, 2000, 2500, 3000, 4000, 5000, 1200 };
            int[] testrrTimesVPCArray = { 120, 2022, 3010 };

            // Process test here

            List<int> resultnrVPCArray = new List<int>();
            resultnrVPCArray.Add(0);
            resultnrVPCArray.Add(2);
            resultnrVPCArray.Add(4);

            // Assert results

            HRT_Alg testAlg = new HRT_Alg();
            List<int> testResult = testAlg.GetNrVPC(testrrTimesArray, testrrTimesVPCArray);
            CollectionAssert.AreEqual(resultnrVPCArray, testResult );

        }

        //GetNrVPC
        [TestMethod]
        [Description("Test if funcion properly finds VPC nr by R_peak index - equality test")]
        public void GetNrVPC_EQTest_2()
        {
            HRT_Params testParams = new HRT_Params("Test");

            // Init test here

            double[] testrrTimesArray = { 198,  460,  709,  966,  1223, 1480, 1742,
                                          2016, 2288, 2551, 2804, 3053, 3304, 3564,
                                          3836, 4103, 4372, 4636, 4902, 5155, 5408,
                                          5562, 5924, 6196, 6469, 6737, 6997, 7247,
                                          7507, 7768, 7923, 8291, 8565, 8830, 9094,
                                          9351, 9601, 9846, 10106,10381,10646,10909,
                                          11169,11425,11677,11932,12188,12450,12626 };
            int[] testrrTimesVPCArray = { 3030, 5401, 9116 };

            // Process test here

            List<int> resultnrVPCArray = new List<int>();
            resultnrVPCArray.Add(11);
            resultnrVPCArray.Add(20);
            resultnrVPCArray.Add(34);

            // Assert results

            HRT_Alg testAlg = new HRT_Alg();
            List<int> testResult = testAlg.GetNrVPC(testrrTimesArray, testrrTimesVPCArray);
            CollectionAssert.AreEqual(resultnrVPCArray, testResult);

        }

        //GetNrVPC
        [TestMethod]
        [Description("Test if funcion properly finds VPC nr by R_peak index - equality test")]
        public void GetNrVPC_EQTest_3()
        {
            HRT_Params testParams = new HRT_Params("Test");

            // Init test here

            double[] testrrTimesArray = { 198,  460,  709,  966,  1223, 1480, 1742,
                                          2016, 2288, 2551, 2804, 3053, 3304, 3564,
                                          3836, 4103, 4372, 4636, 4902, 5155, 5408,
                                          5562, 5924, 6196, 6469, 6737, 6997, 7247,
                                          7507, 7768, 7923, 8291, 8565, 8830, 9094,
                                          9351, 9601, 9846, 10106,10381,10646,10909,
                                          11169,11425,11677,11932,12188,12450,12626 };
            int[] testrrTimesVPCArray = { 202, 2060, 3798, 5560, 7556, 9400, 11169 };

            // Process test here

            List<int> resultnrVPCArray = new List<int>();
            resultnrVPCArray.Add(0);
            resultnrVPCArray.Add(7);
            resultnrVPCArray.Add(14);
            resultnrVPCArray.Add(21);
            resultnrVPCArray.Add(28);
            resultnrVPCArray.Add(35);
            resultnrVPCArray.Add(42);

            // Assert results

            HRT_Alg testAlg = new HRT_Alg();
            List<int> testResult = testAlg.GetNrVPC(testrrTimesArray, testrrTimesVPCArray);
            CollectionAssert.AreEqual(resultnrVPCArray, testResult);

        }

        // GetNrVPC
        [TestMethod]
        [Description("Test if GetNrVPC throws null if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void GetNrVPC_NullTest_1()
        {
            HRT_Params testParams = new HRT_Params("Test");

            // Init test here

            double[] testrrTimesArray = null;
            int[] testrrTimesVPCArray = { 30, 60, 155 };

            // Process test here

            HRT_Alg testAlg = new HRT_Alg();
            List<int> testResult = testAlg.GetNrVPC(testrrTimesArray, testrrTimesVPCArray);
            
        }

        // GetNrVPC
        [TestMethod]
        [Description("Test if GetNrVPC throws null if argument is not initialized")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Array must not be empty")]
        public void GetNrVPC_EmptyArrayTest_2()
        {
            HRT_Params testParams = new HRT_Params("Test");

            // Init test here

            double[] testrrTimesArray = { 0, 15, 30, 45, 60, 75, 90, 105 };
            int[] testrrTimesVPCArray = { };

            // Process test here

            HRT_Alg testAlg = new HRT_Alg();
            List<int> testResult = testAlg.GetNrVPC(testrrTimesArray, testrrTimesVPCArray);

        }

        [TestMethod]
        [Description("Test if GetNrVPC throws null if argument is out of ranged")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Parameter out of range")]
        public void GetNrVPC_OutOfRangeTest_1()
        {
            HRT_Params testParams = new HRT_Params("Test");

            // Init test here

            double[] testrrTimesArray = { };
            int[] testrrTimesVPCArray = { 30, 60, 155 };
            
            // Process test here

            HRT_Alg testAlg = new HRT_Alg();
            List<int> testResult = testAlg.GetNrVPC(testrrTimesArray, testrrTimesVPCArray);

        }

        [TestMethod]
        [Description("Test if GetNrVPC throws null if argument is out of ranged")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "rrTimesVPC array is larger than rrTimes array")]
        public void GetNrVPC_OutOfRangeTest_2()
        {
            HRT_Params testParams = new HRT_Params("Test");

            // Init test here

            double[] testrrTimesArray = { 30, 60, 155 };
            int[] testrrTimesVPCArray = { 0, 15, 30, 45, 60, 75, 90, 105 };
            
            // Process test here

            HRT_Alg testAlg = new HRT_Alg();
            List<int> testResult = testAlg.GetNrVPC(testrrTimesArray, testrrTimesVPCArray);

        }

        //removeRedundant
        [TestMethod]
        [Description("Test if funcion properly remove redundant elements - equality test")]
        public void removeRedundant_EQTest_1()
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
        public void removeRedundant_NullTest()
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
        public void removeRedundant_OutOfRangeTest()
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
        public void rrTimesShift_EQTest_1()
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

        //SearchPrematureTurbulences
        [TestMethod]
        [Description("Test function if properly searches premature turbulences but for the hardest set of parameters - equality test")]
        public void SearchPrematureTurbulences_EQTest_3()
        {
            HRT_Params testParams = new HRT_Params("Test");

            // Init test here

            double[] testTachogram1 = { 725, 736, 722, 711, 589, 1000, 717, 739, 767, 744, 711, 692, 700, 700, 722, 725, 747, 725, 744, 714, 692 };
            double[] testTachogram2 = { 753, 764, 722, 717, 433, 986,  703, 725, 742, 728, 728, 703, 694, 675, 711, 739, 739, 717, 733, 714, 689 };
            double[] testTachogram3 = { 689, 706, 719, 758, 587, 1022, 700, 694, 678, 711, 736, 736, 725, 739, 728, 689, 675, 708, 736, 753, 725 };
            double[] testTachogram4 = { 708, 708, 694, 711, 601, 820, 744, 711, 689, 697, 708, 736, 744, 717, 742, 719, 700, 678, 708, 714, 750 };
            double[] testTachogram5 = { 753, 742, 739, 706, 394, 989,  742, 739, 750, 739, 728, 706, 714, 711, 708, 722, 753, 764, 736, 700, 692 };
            List<double[]> testTachogram = new List<double[]>();
            testTachogram.Add(testTachogram1);
            testTachogram.Add(testTachogram2);
            testTachogram.Add(testTachogram3);
            testTachogram.Add(testTachogram4);
            testTachogram.Add(testTachogram5);

            List<int> testPikVC = new List<int>();
            testPikVC.Add(74300); //and should be rejected - test check this
            testPikVC.Add(102115);
            testPikVC.Add(108027); //and should be rejected - test check this
            testPikVC.Add(118344); //and should be rejected - test check this
            testPikVC.Add(126112);

            List<int> expectedPikVC = new List<int>();
            expectedPikVC.Add(102115);                   
            expectedPikVC.Add(126112);

            // Process test here

            HRT_Alg testAlg = new HRT_Alg();
            List<int> resultPikVC = testAlg.SearchPrematureTurbulences(testTachogram, testPikVC);

            // Assert results

            CollectionAssert.AreEqual(resultPikVC, expectedPikVC);

        }

        //SearchPrematureTurbulences
        [TestMethod]
        [Description("Test function if properly handle exception of missmach size of the lists")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Both list must have the same amound of the elements")]
        public void SearchPrematureTurbulences_OutOfRangeTest()
        {
            HRT_Params testParams = new HRT_Params("Test");

            // Init test here

            double[] testTachogram1 = { 725, 736, 722, 711, 589, 1000, 717, 739, 767, 744, 711, 692, 700, 700, 722, 725, 747, 725, 744, 714, 692 };
            double[] testTachogram2 = { 753, 764, 722, 717, 433, 986, 703, 725, 742, 728, 728, 703, 694, 675, 711, 739, 739, 717, 733, 714, 689 };
            double[] testTachogram3 = { 689, 706, 719, 758, 587, 1022, 700, 694, 678, 711, 736, 736, 725, 739, 728, 689, 675, 708, 736, 753, 725 };
            double[] testTachogram4 = { 708, 708, 694, 711, 601, 820, 744, 711, 689, 697, 708, 736, 744, 717, 742, 719, 700, 678, 708, 714, 750 };
            double[] testTachogram5 = { 753, 742, 739, 706, 394, 989, 742, 739, 750, 739, 728, 706, 714, 711, 708, 722, 753, 764, 736, 700, 692 };
            List<double[]> testTachogram = new List<double[]>();
            testTachogram.Add(testTachogram1);
            testTachogram.Add(testTachogram2);
            testTachogram.Add(testTachogram3);
            testTachogram.Add(testTachogram4);
            testTachogram.Add(testTachogram5);

            List<int> testPikVC = new List<int>();
            testPikVC.Add(74300); 
            testPikVC.Add(102115);
            testPikVC.Add(108027);
            testPikVC.Add(118344); //one PikVC is missing 
                   
            // Process test here

            HRT_Alg testAlg = new HRT_Alg();
            List<int> resultPikVC = testAlg.SearchPrematureTurbulences(testTachogram, testPikVC);
                      
        }

        //TakeNonAtrialComplexes
        [TestMethod]
        [Description("Test function if properly takes non atrial complexes - equality test")]
        public void TakeNonAtrialComplexes_EQTest_1()
        {
            HRT_Params testParams = new HRT_Params("Test");

            // Init test here
                      
            List<Tuple<int, int>> testClass = new TupleList<int, int>
            {
                {  198, 1 },
                {  460, 1 },
                {  709, 1 },
                {  966, 1 },
                { 1223, 1 },
                { 1480, 1 },
                { 1742, 1 },
                { 2016, 1 },
                { 2288, 1 },
                { 2551, 1 },
                { 2804, 1 },
                { 3053, 1 },
                { 3304, 1 },
                { 3564, 1 },
                { 3836, 1 },
                { 4103, 1 },
                { 4372, 1 },
                { 4636, 1 },
                { 4902, 1 },
                { 5155, 1 }
            };                  

            List<int> expectedResult = new List<int>();
            expectedResult.Add(198);
            expectedResult.Add(460);
            expectedResult.Add(709);
            expectedResult.Add(966);
            expectedResult.Add(1223);
            expectedResult.Add(1480);
            expectedResult.Add(1742);
            expectedResult.Add(2016);
            expectedResult.Add(2288);
            expectedResult.Add(2551);
            expectedResult.Add(2804);
            expectedResult.Add(3053);
            expectedResult.Add(3304);
            expectedResult.Add(3564);
            expectedResult.Add(3836);
            expectedResult.Add(4103);
            expectedResult.Add(4372);
            expectedResult.Add(4636);
            expectedResult.Add(4902);
            expectedResult.Add(5155);

            // Process test here

            HRT_Alg testAlg = new HRT_Alg();
            List<int> testResult = testAlg.TakeNonAtrialComplexes(testClass);

            // Assert results

            CollectionAssert.AreEqual(testResult, expectedResult);

        }

        //TakeNonAtrialComplexes
        [TestMethod]
        [Description("Test function if properly takes non atrial complexes - diffrent set of parameters - equality test")]
        public void TakeNonAtrialComplexes_EQTest_2()
        {
            HRT_Params testParams = new HRT_Params("Test");

            // Init test here

            List<Tuple<int, int>> testClass = new TupleList<int, int>
            {
                {  198, 1 },
                {  460, 0 },
                {  709, 1 },
                {  966, 0 },
                { 1223, 0 },
                { 1480, 1 },
                { 1742, 2 },
                { 2016, 2 },
                { 2288, 1 },
                { 2551, 1 },
                { 2804, 0 },
                { 3053, 1 },
                { 3304, 1 },
                { 3564, 0 },
                { 3836, 1 },
                { 4103, 1 },
                { 4372, 3 },
                { 4636, 1 },
                { 4902, 2 },
                { 5155, 1 }
            };

            List<int> expectedResult = new List<int>();
            expectedResult.Add(198);            
            expectedResult.Add(709);
            expectedResult.Add(1480);  
            expectedResult.Add(2288);
            expectedResult.Add(2551);            
            expectedResult.Add(3053);
            expectedResult.Add(3304);            
            expectedResult.Add(3836);
            expectedResult.Add(4103);            
            expectedResult.Add(4636);            
            expectedResult.Add(5155);

            // Process test here

            HRT_Alg testAlg = new HRT_Alg();
            List<int> testResult = testAlg.TakeNonAtrialComplexes(testClass);

            // Assert results

            CollectionAssert.AreEqual(testResult, expectedResult);

        }

        //TakeNonAtrialComplexes
        [TestMethod]
        [Description("Test function if properly takes non atrial complexes - extreme set of parameters - equality test")]
        public void TakeNonAtrialComplexes_EQTest_3()
        {
            HRT_Params testParams = new HRT_Params("Test");

            // Init test here

            List<Tuple<int, int>> testClass = new TupleList<int, int>
            {
                {  198, 0 },
                {  460, 0 },
                {  709, 2 },
                {  966, 0 },
                { 1223, 0 },
                { 1480, 0 },
                { 1742, 2 },
                { 2016, 2 },
                { 2288, 3 },
                { 2551, 0 },
                { 2804, 0 },
                { 3053, 0 },
                { 3304, 2 },
                { 3564, 0 },
                { 3836, 0 },
                { 4103, 2 },
                { 4372, 3 },
                { 4636, 0 },
                { 4902, 2 },
                { 5155, 0 }
            };

            List<int> expectedResult = new List<int>();
         
            // Process test here

            HRT_Alg testAlg = new HRT_Alg();
            List<int> testResult = testAlg.TakeNonAtrialComplexes(testClass);

            // Assert results

            CollectionAssert.AreEqual(testResult, expectedResult);

        }

    }
}
