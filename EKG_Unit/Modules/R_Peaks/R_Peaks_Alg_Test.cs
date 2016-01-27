using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.R_Peaks;
using System.Collections.Generic;

namespace EKG_Unit.Modules.R_Peaks
{
    [TestClass]
    public class R_Peaks_Alg_Test
    {
        // Tests for EMD functions
        //FindIndexes
        [TestMethod]
        [Description("Test if find indexes performs properly")]
        public void FindIndexesTest()
        {
            double[] testArray = { 0, 2, 0, 0, 4, 7, 9, 0 };
            double[] resultArray = { 0, 2, 3, 7 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);
            Func<double, bool> testPredicate = item => item == 0;

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.FindIndexes(testVector, testPredicate);
            Assert.AreEqual(testResult, resultVector);
        }

        [TestMethod]
        [Description("Test if FIndIndexes throws null if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void FindIndexesNullTest()
        {
            double[] testArray = { 0, 2, 0, 0, 4, 7, 9, 0 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Func<double, bool> testPredicate = null;
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.FindIndexes(testVector, testPredicate);
        }

        [TestMethod]
        [Description("Test if find indexes return null if no elements find")]
        public void FindIndexesFailureTest()
        {
            double[] testArray = { 0, 2, 0, 0, 4, 7, 9, 0 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Func<double, bool> testPredicate = item => item == 1;

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.FindIndexes(testVector, testPredicate);
            Assert.IsNull(testResult);
        }

        //Extrema
        [TestMethod]
        [Description("Test if Extrema throws null if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void ExtremaNullTest()
        {
            Vector<double> testVector = null;
            R_Peaks_Alg test = new R_Peaks_Alg();
            Tuple<List<double>, List<double>> testResult = test.Extrema(testVector);
        }

        [TestMethod]
        [Description("Test if Extrema return empty lists if no extrema find")]
        public void ExtremaFailureTest()
        {
            double[] testArray = { 0, 0, 0, 0, 0 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            List<double> testMinList = new List<double>();
            List<double> testMaxList = new List<double>();

            R_Peaks_Alg test = new R_Peaks_Alg();
            Tuple<List<double>, List<double>> testResult = test.Extrema(testVector);
            CollectionAssert.AreEqual(testMinList, testResult.Item1);
            CollectionAssert.AreEqual(testMaxList, testResult.Item2);
        }

        [TestMethod]
        [Description("Test if Extrema performs properly")]
        public void ExtremaTest()
        {
            double[] testArray = { 0, 2, 0, 0, -4, 6, 1, 1, -1, 0, 8, -5 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            double[] testMinArr = { 4, 8 };
            double[] testMaxArr = { 1, 5, 10 };
            List<double> testMinList = new List<double>() { 4, 8 };
            List<double> testMaxList = new List<double>() { 1, 5, 10 };

            R_Peaks_Alg test = new R_Peaks_Alg();
            Tuple<List<double>, List<double>> testResult = test.Extrema(testVector);
            CollectionAssert.AreEqual(testResult.Item1, testMinList);
            CollectionAssert.AreEqual(testMaxList, testResult.Item2);
        }

        //CubicSplineInterpolation
        [TestMethod]
        [Description("Test if CubicSpline throws null if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void CubicSpllineInterpNullTest()
        {
            double[] testVector = null;
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.CubicSplineInterp(10, testVector, testVector);
        }

        [TestMethod]
        [Description("Test if CubicSpline throws null if argument is not initialized")]
        [ExpectedException(typeof(ArgumentException), "The given array is too small. It must be at least 2 long")]
        public void CubicSpllineInterpFailureTest()
        {
            double[] testX = { 1 };
            double[] testY = { 1 };
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.CubicSplineInterp(10, testX, testY);
        }

        [TestMethod]
        [Description("Test if CubicSpline performs properly")]
        public void CubicSplineTest()
        {
            double[] testX = { 1, 2, 3 };
            double[] testY = { 0, 1, 0 };
            int testLength = 5;
            double[] resultArray = { -1, 0, 1, 0, -1 };
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.CubicSplineInterp(testLength, testX, testY);
            Assert.AreEqual(testResult, resultVector);
        }

        //ExtractMode
        [TestMethod]
        [Description("Test if ExtractMode throws null if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void ExtractModeNullTest()
        {
            Vector<double> testVector = null;
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.ExtractModeFun(testVector);
        }
       
        [TestMethod]
        [Description("Test if ExtractMode return input signal if it does not consist of any modes")]
        public void ExtractModeFailureTest()
        {
            double[] testArray = { 0, 0, 0, 0, 0 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.ExtractModeFun(testVector);
            Assert.AreEqual(testVector, testResult);
        }

        [TestMethod]
        [Description("Test if ExtractMode performs properly")]
        public void ExtractModeTest()
        {
            double[] testArray = { 1, 5, 0, -5, 1, 6, -3, 1 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            double[] resultArray = { 2.1250, 5.66667, 0.2083, -5.2500, 0.2916667, 4.8333, -4.6250, -1.0833 };
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.ExtractModeFun(testVector);
            for (int i = 0; i < resultVector.Count; i++)
            {
                resultVector[i] = Math.Round(resultVector[i], 4);
                testResult[i] = Math.Round(testResult[i], 4);
            }

            Assert.AreEqual(resultVector, testResult);
        }

        //EmpiricalModeDecomposition
        [TestMethod]
        [Description("Test if EmpiricalModeDecomp throws null if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void EmpiricalModeDecompNullTest()
        {
            Vector<double> testVector = null;
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.ExtractModeFun(testVector);
        }

        [TestMethod]
        [Description("Test if EmpiricalModeDecomp return input signals if it does not consist of any modes")]
        public void EmpiricalModeDecompFailureTest()
        {
            double[] testArray = { 0, 0, 0, 0, 0 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double>[] resultVectors = { testVector, testVector };
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double>[] testResult = test.EmpiricalModeDecomposition(testVector);
            CollectionAssert.AreEqual(resultVectors, testResult);
        }
        //check
        [TestMethod] 
        [Description("Test if EmpiricalModeDecomposition performs properly")]          
        public void EmpiricalModeDecompTest()
        {
            double[] testArray = { 0, 2, -2, 4, -4, 1, -1, -2, 0, 4, 2, -2, 4, -4, 1, -1, -2, 0, 4, 1 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            double[] imf1 = { 2.30957, 1.71301, -3.40024, 3.17601, -2.96313, 2.79749, -0.26414, -2.71007, -1.14758, 2.94203, 0.84069, -3.23907, 3.27102, -3.15764, 2.90023, 0.40448, -2.11690, -1.79369, 1.24454, -1.13177 };
            double[] imf2 = { -9.47447, -3.27213, 0.27978, 1.15376, -0.06656, -0.81764, -0.19879, 0.53112, 0.18858, -0.40234, -0.22762, 0.36748, 0.57561, -0.29400, -0.85333, -0.24241, 0.83112, 1.31704, 0.16507, -3.67489 };
            Vector<double> vect1 = Vector<double>.Build.DenseOfArray(imf1);
            Vector<double> vect2 = Vector<double>.Build.DenseOfArray(imf2);
            Vector<double>[] resultVectors = { vect1, vect2 };

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double>[] testResult = test.EmpiricalModeDecomposition(testVector);
            for (int i = 0; i < testVector.Count; i++)
            {
                resultVectors[0][i] = Math.Round(resultVectors[0][i], 4);
                resultVectors[1][i] = Math.Round(resultVectors[1][i], 4);
                testResult[0][i] = Math.Round(testResult[0][i], 4);
                testResult[1][i] = Math.Round(testResult[1][i], 4);
            }

            CollectionAssert.AreEqual(resultVectors, testResult);
        }

        //Transform IMFs
        [TestMethod]
        [Description("Test if TranformImfs return input signals if it does not consist of any modes")]
        public void TranformImfsTest()
        {
            double[] testArray = { 1, 2, 1, 1, 2, 2 };
            double[] testArray1 = { 1, -1, 1, -1, 1, 1 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> testVector1 = Vector<double>.Build.DenseOfArray(testArray1);
            Vector<double>[] testVectors = { testVector, testVector1 };

            double[] resultArray = { 2, 2, 2, 4};
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.TransformImf(testVectors, 10);
            Assert.AreEqual(resultVector, testResult);
        }
        
        [TestMethod]
        [Description("Test if TranformImfs throws exception if lenght of vectors is not the same")]
        [ExpectedException (typeof(ArgumentOutOfRangeException), "Lenghts of vectors in array must be tha same")]
        public void TranformImfsFailureTest()
        {
            double[] testArray = { 1, 2, 1, 1, 2, 2 };
            double[] testArray1 = { 1, -1, 1, -1 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> testVector1 = Vector<double>.Build.DenseOfArray(testArray1);
            Vector<double>[] testVectors = { testVector, testVector1 };

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.TransformImf(testVectors, 10);
        }

        [TestMethod]
        [Description("Test if  TranformImfs throws exception if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void  TranformImfNullTest()
        {
            Vector<double>[] testArray = null;
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.TransformImf(testArray, 15);
        }

        [TestMethod]
        [Description("Test if TranformImf throws exception if fs equals zero")]
        [ExpectedException(typeof(ArgumentException), "Non-positive frequency value given as parameter")]
        public void TranformImfZeroTest()
        {
            uint fs = 0;
            Vector<double>[] testArray = { Vector<double>.Build.Random(5), Vector<double>.Build.Random(5) } ;
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.TransformImf(testArray, fs);
        }

        //LPFiltering
        [TestMethod]
        [Description("Test if LPFiltering works properly")]
        public void LPFilteringTest()
        {
            double[] testArray = { 2, 1, 0, -3, 12, -4, 0, 0, 2, 5 };
            double[] resultArray = { 0.034311, 0.084601, 0.098854, 0.043995, 0.19689, 0.32738, 0.24752, 0.23903, 0.26514, 0.37613};

            for (int i = 0; i < resultArray.Length; i++)
            {
                resultArray[i] = Math.Round(resultArray[i], 2);
            }
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.LPFiltering(testArray, 10);
            for (int i = 0; i < testResult.Length; i++)
            {
                testResult[i] = Math.Round(testResult[i], 2);
            }

           CollectionAssert.AreEqual(resultArray, testResult);
        }

        [TestMethod]
        [Description("Test if LPFiltering throws exception if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void LPFilteringNullTest()
        {
            double[] testArray = null;
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.LPFiltering(testArray, 10);
        }

        [TestMethod]
        [Description("Test if LPFiltering throws exception if any of input frequencies is below zero")]
        [ExpectedException(typeof(ArgumentException), "Non-positive frequency value given as parameter")]
        public void LPFilteringZeroTest()
        {
            uint fs = 0;
            double[] testArray = { 2, 1, 1, 2, 3, 2, 1, 1, 0, 1 };
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.LPFiltering(testArray, fs);
        }

        [TestMethod]
        [Description("Test if LPFiltering works properly if array is empty - not null")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Array must not be empty")]
        public void LPFilteringEmptyTest()
        {
            double[] testArray = new double[] { };
            double[] resultArray = {};

            Vector<double> res = Vector<double>.Build.DenseOfArray(resultArray);
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.LPFiltering(testArray, 10);
            Vector<double> testR = Vector<double>.Build.DenseOfArray(testResult);

            Assert.AreEqual(res, testR);
        }

        // FindPeaksTh
        [TestMethod]
        [Description("Test if FindPeaksTh works properly")]
        public void FindPeaksThTest()
        {
            double[] testArray = { 1,-5, 0, 1, 10, 0, -4, 4 };
            List<double> resultList = new List<double>(){ 4 };

            R_Peaks_Alg test = new R_Peaks_Alg();
            List<double> testResult = test.FindPeaksTh(testArray);

            CollectionAssert.AreEqual(testResult, resultList);
        }

        [TestMethod]
        [Description("Test if FindPeaks works properly if there are no peaks in signal")]
        public void FindPeaksThTest2()
        {
            double[] testArray = { 0, 0, 0, 0, 0, 0, 0 };
            List<double> resultList = new List<double>();

            R_Peaks_Alg test = new R_Peaks_Alg();
            List<double> testResult = test.FindPeaksTh(testArray);
            CollectionAssert.AreEqual(testResult, resultList);
        }

        [TestMethod]
        [Description("Test if FindPeaks throws exception if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void FindPeaksThNullTest()
        {
            double[] testArray = null;

            R_Peaks_Alg test = new R_Peaks_Alg();
            List<double> testResult = test.FindPeaksTh(testArray);
        }

        [TestMethod]
        [Description("Test if FindPeaksTh throws exception if array is empty - not null")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Array must not be empty")]
        public void FindPeaksThEmptyTest()
        {
            double[] testArray = new double[] { };

            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.LPFiltering(testArray, 10);
        }
        //-0.000608, -0.00506, -0.0199, -0.0502, -0.0916, -0.131, -0.154, -0.156, -0.142, -0.125, -0.117, -0.119, -0.128, -0.135, -0.136, -0.133, -0.131, -0.13, -0.13, -0.125, -0.117, -0.108, -0.107, -0.119, -0.142, -0.168, -0.19, -0.208, -0.223, -0.238, -0.242, -0.218, -0.156, -0.0558, 0.0632, 0.181, 0.286, 0.383, 0.479, 0.569, 0.623, 0.596, 0.455, 0.216, -0.0525, -0.259, -0.346, -0.319, -0.231, -0.146, -0.103, -0.105, -0.133, -0.161, -0.172, -0.164, -0.149, -0.137, -0.136, -0.142, -0.149, -0.148, -0.139, -0.127, -0.121, -0.124, -0.132, -0.139, -0.139, -0.134, -0.128, -0.124, -0.121, -0.116, -0.109, -0.103, -0.101, -0.104, -0.108, -0.11, -0.108, -0.104, -0.103, -0.105, -0.109, -0.108, -0.102, -0.0927, -0.0865, -0.0862, -0.0904, -0.095, -0.0965, -0.0943, -0.091, -0.089, -0.0881, -0.0873, -0.0868, -0.0886, -0.0933, -0.0986, -0.101, -0.0978, -0.0907, -0.0839, -0.081, -0.082, -0.0831, -0.0809, -0.0761, -0.0735, -0.0768, -0.0848, -0.0918, -0.0925, -0.0875, -0.0824, -0.0832, -0.0912, -0.103, -0.112, -0.114, -0.112, -0.11, -0.11, -0.115, -0.121, -0.127, -0.133, -0.138, -0.143, -0.147, -0.148, -0.148, -0.147, -0.148, -0.152, -0.154, -0.155, -0.152, -0.149, -0.147, -0.148, -0.149, -0.145, -0.137, -0.124, -0.11, -0.0977, -0.0861, -0.0743, -0.0616, -0.0495, -0.0408, -0.0357, -0.0308, -0.0217, -0.00695, 0.0101, 0.0242, 0.0327, 0.0377, 0.0434, 0.0516, 0.0597, 0.0636, 0.0625, 0.0592, 0.0574, 0.0577, 0.0578, 0.0547, 0.0479, 0.0401, 0.0358, 0.037, 0.0421, 0.0469, 0.0478, 0.0447, 0.0407, 0.0388, 0.0387, 0.038, 0.0349, 0.0307, 0.0279, 0.0274, 0.0276, 0.0257, 0.0214, 0.0173, 0.0178, 0.0237, 0.0307, 0.0321, 0.0243, 0.01, -0.00359, -0.0107, -0.0107, -0.00779, -0.00614, -0.00651, -0.00717, -0.00773, -0.0103, -0.0166, -0.0248, -0.0301, -0.0288, -0.0215, -0.0126, -0.00751, -0.00868, -0.0137, -0.0173, -0.0157, -0.0106, -0.00743, -0.0102, -0.0181, -0.0265, -0.0315, -0.0325, -0.031, -0.0281, -0.0242, -0.02, -0.018, -0.0205, -0.0266, -0.0329, -0.0359, -0.0341, -0.0285, -0.0224, -0.0191, -0.0196, -0.0218, -0.0223, -0.0192, -0.0139, -0.0101, -0.0104, -0.0144, -0.0192, -0.0218, -0.0221, -0.0217, -0.0215, -0.0201, -0.0159, -0.00927, -0.00368, -0.00295, -0.0085, -0.0181, -0.0268, -0.0305, -0.029, -0.025, -0.0217, -0.019, -0.0147, -0.00664, 0.0038, 0.0129, 0.0179, 0.0196, 0.0211, 0.0244, 0.0283, 0.0296, 0.0269, 0.0224, 0.0203, 0.0224, 0.0264, 0.0288, 0.0277, 0.0245, 0.0222, 0.0219, 0.0219, 0.0188, 0.0114, 0.00182, -0.00669, -0.0122, -0.0153, -0.0173, -0.0191, -0.0193, -0.0171, -0.0137, -0.0127, -0.0179, -0.029, -0.0416, -0.0494, -0.0486, -0.0414, -0.0334, -0.0293, -0.0293, -0.0305, -0.0304, -0.0293, -0.0293, -0.0311, -0.0329, -0.0324, -0.0299, -0.0291, -0.0346, -0.0477, -0.0658, -0.0847, -0.102, -0.119, -0.133, -0.136, -0.111, -0.0466, 0.0561, 0.178, 0.293, 0.385, 0.456, 0.524, 0.601, 0.675, 0.71, 0.658, 0.495, 0.238, -0.0479, -0.284, -0.413, -0.426, -0.355, -0.254, -0.168, -0.12, -0.109, -0.119, -0.131, -0.134, -0.128, -0.117, -0.109, -0.105, -0.103, -0.101, -0.0977, -0.0946, -0.0944, -0.0972, -0.0996, -0.0975, -0.09, -0.081, -0.0765, -0.0793, -0.0861, -0.0904, -0.0877, -0.0792, -0.0704, -0.066, -0.0661, -0.0674, -0.0667, -0.064, -0.0611, -0.0595, -0.0587, -0.0572, -0.0546, -0.0523, -0.0526, -0.0559, -0.0594, -0.0593, -0.0542, -0.047, -0.0427, -0.0436, -0.0479, -0.0515, -0.0521, -0.051, -0.0503, -0.0511, -0.0521, -0.0517, -0.0499, -0.0484, -0.0495, -0.0533, -0.0567, -0.0567, -0.053, -0.0497, -0.0514, -0.0594, -0.0696, -0.0759, -0.0757, -0.0723, -0.0712, -0.0752, -0.0829, -0.0905, -0.0956, -0.0991, -0.104, -0.11, -0.116, -0.12, -0.121, -0.121, -0.125, -0.133, -0.141, -0.143, -0.141, -0.135, -0.133, -0.135, -0.141, -0.146, -0.146, -0.141, -0.133, -0.123, -0.114, -0.103, -0.0894, -0.0746, -0.0605, -0.0476, -0.0338, -0.0156, 0.00672, 0.0289, 0.045, 0.0524, 0.0541, -0.00061, -0.0051, -0.02, -0.05, -0.092, -0.13, -0.15, -0.16, -0.14, -0.13, -0.12, -0.12, -0.13, -0.13, -0.14, -0.13, -0.13, -0.13, -0.13, -0.13, -0.12, -0.11, -0.11, -0.12, -0.14, -0.17, -0.19, -0.21, -0.22, -0.24, -0.24, -0.22, -0.16, -0.056, 0.063, 0.18, 0.29, 0.38, 0.48, 0.57, 0.62, 0.6, 0.46, 0.22, -0.052, -0.26, -0.35, -0.32, -0.23, -0.15, -0.1, -0.1, -0.13, -0.16, -0.17, -0.16, -0.15, -0.14, -0.14, -0.14, -0.15, -0.15, -0.14, -0.13, -0.12, -0.12, -0.13, -0.14, -0.14, -0.13, -0.13, -0.12, -0.12, -0.12, -0.11, -0.1, -0.1, -0.1, -0.11, -0.11, -0.11, -0.1, -0.1, -0.11, -0.11, -0.11, -0.1, -0.093, -0.087, -0.086, -0.09, -0.095, -0.096, -0.094, -0.091, -0.089, -0.088, -0.087, -0.087, -0.089, -0.093, -0.099, -0.1, -0.098, -0.091, -0.084, -0.081, -0.082, -0.083, -0.081, -0.076, -0.073, -0.077, -0.085, -0.092, -0.093, -0.088, -0.082, -0.083, -0.091, -0.1, -0.11, -0.11, -0.11, -0.11, -0.11, -0.11, -0.12, -0.13, -0.13, -0.14, -0.14, -0.15, -0.15, -0.15, -0.15, -0.15, -0.15, -0.15, -0.15, -0.15, -0.15, -0.15, -0.15, -0.15, -0.15, -0.14, -0.12, -0.11, -0.098, -0.086, -0.074, -0.062, -0.05, -0.041, -0.036, -0.031, -0.022, -0.0069, 0.01, 0.024, 0.033, 0.038, 0.043, 0.052, 0.06, 0.064, 0.063, 0.059, 0.057, 0.058, 0.058, 0.055, 0.048, 0.04, 0.036, 0.037, 0.042, 0.047, 0.048, 0.045, 0.041, 0.039, 0.039, 0.038, 0.035, 0.031, 0.028, 0.027, 0.028, 0.026, 0.021, 0.017, 0.018, 0.024, 0.031, 0.032, 0.024, 0.01, -0.0036, -0.011, -0.011, -0.0078, -0.0061, -0.0065, -0.0072, -0.0077, -0.01, -0.017, -0.025, -0.03, -0.029, -0.021, -0.013, -0.0075, -0.0087, -0.014, -0.017, -0.016, -0.011, -0.0074, -0.01, -0.018, -0.026, -0.031, -0.032, -0.031, -0.028, -0.024, -0.02, -0.018, -0.02, -0.027, -0.033, -0.036, -0.034, -0.028, -0.022, -0.019, -0.02, -0.022, -0.022, -0.019, -0.014, -0.01, -0.01, -0.014, -0.019, -0.022, -0.022, -0.022, -0.021, -0.02, -0.016, -0.0093, -0.0037, -0.0029, -0.0085, -0.018, -0.027, -0.031, -0.029, -0.025, -0.022, -0.019, -0.015, -0.0066, 0.0038, 0.013, 0.018, 0.02, 0.021, 0.024, 0.028, 0.03, 0.027, 0.022, 0.02, 0.022, 0.026, 0.029, 0.028, 0.025, 0.022, 0.022, 0.022, 0.019, 0.011, 0.0018, -0.0067, -0.012, -0.015, -0.017, -0.019, -0.019, -0.017, -0.014, -0.013, -0.018, -0.029, -0.042, -0.049, -0.049, -0.041, -0.033, -0.029, -0.029, -0.031, -0.03, -0.029, -0.029, -0.031, -0.033, -0.032, -0.03, -0.029, -0.035, -0.048, -0.066, -0.085, -0.1, -0.12, -0.13, -0.14, -0.11, -0.047, 0.056, 0.18, 0.29, 0.38, 0.46, 0.52, 0.6, 0.68, 0.71, 0.66, 0.49, 0.24, -0.048, -0.28, -0.41, -0.43, -0.35, -0.25, -0.17, -0.12, -0.11, -0.12, -0.13, -0.13, -0.13, -0.12, -0.11, -0.1, -0.1, -0.1, -0.098, -0.095, -0.094, -0.097, -0.1, -0.097, -0.09, -0.081, -0.077, -0.079, -0.086, -0.09, -0.088, -0.079, -0.07, -0.066, -0.066, -0.067, -0.067, -0.064, -0.061, -0.06, -0.059, -0.057, -0.055, -0.052, -0.053, -0.056, -0.059, -0.059, -0.054, -0.047, -0.043, -0.044, -0.048, -0.052, -0.052, -0.051, -0.05, -0.051, -0.052, -0.052, -0.05, -0.048, -0.05, -0.053, -0.057, -0.057, -0.053, -0.05, -0.051, -0.059, -0.07, -0.076, -0.076, -0.072, -0.071, -0.075, -0.083, -0.091, -0.096, -0.099, -0.1, -0.11, -0.12, -0.12, -0.12, -0.12, -0.13, -0.13, -0.14, -0.14, -0.14, -0.14, -0.13, -0.14, -0.14, -0.15, -0.15, -0.14, -0.13, -0.12, -0.11, -0.1, -0.089, -0.075, -0.06, -0.048, -0.034, -0.016, 0.0067, 0.029, 0.045, 0.052, 0.054
        // __________________________________________________________________________________________

        // Tests for HILBERT functions
        // Perform Hilbert Transform
        [TestMethod]
        [Description("Test if Hilbert Transform performs properly")]
        public void HilbertTransformTest()
        {
            double[] testArray = { 0, 1, 0, 0, -2, 3, 0, 1 };
            double[] resultArray = { 0, 1.0398, 0, 0, 2.0796, 3.1194, 0, 1.0398 };

            for (int i = 0; i < resultArray.Length; i++)
            {
                resultArray[i] = Math.Round(resultArray[i], 3);
            }
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.HilbertTransform(testArray);
            for (int i = 0; i < testResult.Length; i++)
            {
                testResult[i] = Math.Round(testResult[i], 3);
            }
            CollectionAssert.AreEqual(testResult, resultArray);
        }

        [TestMethod]
        [Description("Test if Hilbert Transform performs properly - not equality test")]
        public void HilbertTransformTest2()
        {
            double[] testArray = { 0, 1, 0, 0, -2, 3, 0, 1 };
            double[] resultArray = { 0, 1.1, 0, 0, 2, 3.2, 0, 1 };

            for (int i = 0; i < resultArray.Length; i++)
            {
                resultArray[i] = Math.Round(resultArray[i], 3);
            }
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.HilbertTransform(testArray);
            for (int i = 0; i < testResult.Length; i++)
            {
                testResult[i] = Math.Round(testResult[i], 3);
            }
            CollectionAssert.AreNotEqual(testResult, resultArray);
        }

        [TestMethod]
        [Description("Test if HilbertTransform throws null if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void HilbertTransformNullTest()
        {
            double[] testArray = null;
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.HilbertTransform(testArray);
        }
        // TEST - co jeśli pusta tablica na wejściu -> wyjątek dot pustej tablicy!

        // Perform Filtering
        [TestMethod]
        [Description("Test if Filtering works properly")]
        public void FilteringTest()
        {
            double[] testArray = { 2, 1, 0, -3, 12, -4, 0, 0, 2, 5 };
            double[] resultArray = { -0.060547, 0.101003, 0.045224, 0.097230, -0.725425, 0.927442, -0.297861, 0.641052, -1.412134, 0.005191 };

            for (int i = 0; i < resultArray.Length; i++)
            {
                resultArray[i] = Math.Round(resultArray[i], 4);
            }
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Filtering(25, 5, 12 , testArray);
            for (int i = 0; i < testResult.Length; i++)
            {
                testResult[i] = Math.Round(testResult[i], 4);
            }
            CollectionAssert.AreEqual(testResult, resultArray);
        }

        [TestMethod]
        [Description("Test if Filtering works properly - not equality test")]
        public void FilteringTest2()
        {
            double[] testArray = { 2, 1, 0, -3, 12, -4, 0, 0, 2, 5 };
            double[] resultArray = { -0.081234, 0.152345, 0.0465343, 0.0995345, 0.725425, -0.927442, -0.297861, 0.641052, -1.412134, 0.005191 };

            for (int i = 0; i < resultArray.Length; i++)
            {
                resultArray[i] = Math.Round(resultArray[i], 4);
            }
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Filtering(25, 5, 12, testArray);
            for (int i = 0; i < testResult.Length; i++)
            {
                testResult[i] = Math.Round(testResult[i], 4);
            }
            CollectionAssert.AreNotEqual(testResult, resultArray);
        }

        [TestMethod]
        [Description("Test if Filtering throws exception if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void FilteringNullTest()
        {
            double[] testArray = null;
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Filtering(25, 5, 12, testArray);
        }

        [TestMethod]
        [Description("Test if Filtering throws exception if any of input frequencies is below zero")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Non-positive frequency value given as parameter")]
        public void FilteringZeroTest()
        {
            double fs = 0;
            double fl = -5;
            double fh = 0;
            double[] testArray = { 2, 1, 1, 2, 3, 2, 1, 1, 0, 1 };
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Filtering(fs, fl, fh, testArray);
        }

        // Integration for Hilbert method
        [TestMethod]
        [Description("Test if Integration works properly")]
        public void IntegrationTest()
        {
            double[] testArray = { 2, 0, 1, -2, 8, -3, 0, 1, 0, 2 };
            double[] resultArray = { 0.3333, 0.1111, 1.0000, 0.6667, 0.4444, 0.5556, 0.4444, 0.8889 };

            for (int i = 0; i < resultArray.Length; i++)
            {
                resultArray[i] = Math.Round(resultArray[i], 4);
            }
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Integration(testArray, 15);
            for (int i = 0; i < testResult.Length; i++)
            {
                testResult[i] = Math.Round(testResult[i], 4);
            }
            CollectionAssert.AreEqual(testResult, resultArray);
        }
        
        [TestMethod]
        [Description("Test if Integration works properly - not equality test")]
        public void IntegrationTest2()
        {
            double[] testArray = { 2, 0, 1, -2, 8, -3, 0, 1, 0, 2 };
            double[] resultArray = { 0.2222, 0.2222, 1.0000, 0.6667, 0.4444, 0.5556, 0.4444, 0.8889 };

            for (int i = 0; i < resultArray.Length; i++)
            {
                resultArray[i] = Math.Round(resultArray[i], 4);
            }
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Integration(testArray, 15);
            for (int i = 0; i < testResult.Length; i++)
            {
                testResult[i] = Math.Round(testResult[i], 4);
            }
            CollectionAssert.AreNotEqual(testResult, resultArray);
        }

        [TestMethod]
        [Description("Test if Integration throws exception if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void IntegrationNullTest()
        {
            double[] testArray = null;
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Integration(testArray, 15);
        }

        [TestMethod]
        [Description("Test if Integration throws exception if fs equals zero")]
        [ExpectedException(typeof(ArgumentException), "Non-positive frequency value given as parameter")]
        public void IntegrationZeroTest()
        {
            uint fs = 0;
            double[] testArray = { 2, 1, 1, 2, 3, 2, 1, 1, 0, 1 };
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Integration(testArray, fs);
        }

        // Locating peaks for Hilbert method
        [TestMethod]
        [Description("Test if FindPeak works properly")]
        public void FindPeakTest()
        {
            double[] testArray1 = { 2, 0, 1, -2, 9, -2, 0, 1, 0, 1 };
            double[] testArray2 = { 0.1000, 1.0000, 0.8000, 0.6000, 0.7000, 0.6000, 0.9000, 0, 0.2000, 0.2000 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray2);
            double[] resultArray = { 4 };

            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.FindPeak(testArray1, testVector);
            CollectionAssert.AreEqual(testResult, resultArray);
        }

        [TestMethod]
        [Description("Test if FindPeak works properly - not equality test")]
        public void FindPeakTest2()
        {
            double[] testArray1 = { 2, 0, 1, -2, 9, -2, 0, 1, 0, 1 };
            double[] testArray2 = { 0.1000, 1.0000, 0.8000, 0.6000, 0.7000, 0.6000, 0.9000, 0, 0.2000, 0.2000 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray2);
            double[] resultArray = { 5 };

            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.FindPeak(testArray1, testVector);
            CollectionAssert.AreNotEqual(testResult, resultArray);
        }

        [TestMethod]
        [Description("Test if FindPeak throws exception if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter (array)")]
        public void FindPeakNullTest1()
        {
            double[] testArray = null;
            double[] testArray2 = { 1, 2, 3};
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray2);
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.FindPeak(testArray, testVector);
        }

        [TestMethod]
        [Description("Test if FindPeak throws exception if argument is not initialized")]
        [ExpectedException(typeof(NullReferenceException), "Null given as parameter (vector)")]
        public void FindPeakNullTest2()
        {
            double[] testArray = { 1, 2, 3 };
            double[] testArray2 = null;
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray2);
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.FindPeak(testArray, testVector);
        }

        // __________________________________________________________________________________________

        // Tests for PAN TOMPKINS functions
        // Derivative for PT method
        [TestMethod]
        [Description("Test if Derivative works properly")]
        public void DerivativeTest()
        {
            double[] testArray = { 2, 1, 1, 3, 5, 1, 1 };
            double[] resultArray = { -2, -5, -3, -1, -7, -8, 4 };

            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Derivative(testArray);
            CollectionAssert.AreEqual(testResult, resultArray);
        }

        [TestMethod]
        [Description("Test if Derivative works properly - not equality tes")]
        public void DerivativeTest2()
        {
            double[] testArray = { 2, 1, 1, 3, 5, 1, 1 };
            double[] resultArray = { -5, -3, -1, -7, -8, 4, -2};

            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Derivative(testArray);
            CollectionAssert.AreNotEqual(testResult, resultArray);
        }

        [TestMethod]
        [Description("Test if Derivative throws exception if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void DerivativeNullTest()
        {
            double[] testArray = null;
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Derivative(testArray);
        }

        // Squaring for PT method
        [TestMethod]
        [Description("Test if Squaring works properly")]
        public void SquaringTest()
        {
            double[] testArray = { 2, 5, -3, 0, 1 };
            double[] resultArray = { 4, 25, 9, 0, 1 };

            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Squaring(testArray);
            CollectionAssert.AreEqual(testResult, resultArray);
        }

        [TestMethod]
        [Description("Test if Squaring works properly - not equality tes")]
        public void SquaringTest2()
        {
            double[] testArray = { 2, 5, -3, 0, 1 };
            double[] resultArray = { 4, 25, -9, 0, 1 };

            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Squaring(testArray);
            CollectionAssert.AreNotEqual(testResult, resultArray);
        }

        [TestMethod]
        [Description("Test if Squaring throws exception if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void SquaringNullTest()
        {
            double[] testArray = null;
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Squaring(testArray);
        }

        // Integrating for PT method
        [TestMethod]
        [Description("Test if Integrating works properly")]
        public void IntegratingTest()
        {
            double[] testArray = { 1, 0, 2, -1, 5, 3, 0, -1};
            double[] resultArray = { 0.5, 0.5, 1, 0.5, 2, 4, 1.5, 0 };

            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Integrating(testArray, 10);
            CollectionAssert.AreEqual(testResult, resultArray);
        }

        [TestMethod]
        [Description("Test if Integrating works properly - not equality test")]
        public void IntegratingTest2()
        {
            double[] testArray = { 1, 0, 2, -1, 5, 3, 0, -1 };
            double[] resultArray = { 0.5, 0.5, 1, 0.5, 2, 4, 1.5, 0 };

            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Integrating(testArray, 30);
            CollectionAssert.AreNotEqual(testResult, resultArray);
        }

        [TestMethod]
        [Description("Test if Integration throws exception if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void IntegratingNullTest()
        {
            double[] testArray = null;
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Integrating(testArray, 15);
        }

        [TestMethod]
        [Description("Test if Integration throws exception if fs equals zero")]
        [ExpectedException(typeof(ArgumentException), "Non-positive frequency value given as parameter")]
        public void IntegratingZeroTest()
        {
            uint fs = 0;
            double[] testArray = { 1, 1, 2, 3, 5, 8, 13, 21, 34, 55 };
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Integrating(testArray, fs);
        }
        // ____________________________________ZERKNIJ NA TO_____________________________________________

        // Finding Peaks peaks for PT method
        [TestMethod]
        [Description("Test if FindPeaks works properly")]
        public void FindPeaksTest()
        {
            uint fs = 10;
            double dist = 0.5;
            double[] testArray = { 2, 0, 1, -2, 9, -2, 0, 1, 0, 1 };
            double[] resultArray = { 4 };
            //List<double> resultList = new List<double>(resultArray);

            R_Peaks_Alg test = new R_Peaks_Alg();
            List<double> testResult = test.FindPeaks(testArray, fs, dist);
            Vector<double> resultList = Vector<double>.Build.DenseOfArray(resultArray);
            Vector<double> testRes = Vector<double>.Build.DenseOfEnumerable(testResult);

            Assert.AreEqual(testRes, resultList);
        }

        [TestMethod]
        [Description("Test if FindPeaks works properly - not equality test")]
        public void FindPeaksTest2()
        {
            uint fs = 10;
            double dist = 1;
            double[] testArray = { 2, 0, 1, -2, 9, -2, 0, 1, 0, 1 };
            double[] resultArray = { 5 };
            List<double> resultList = new List<double>(resultArray);

            R_Peaks_Alg test = new R_Peaks_Alg();
            List<double> testResult = test.FindPeaks(testArray, fs, dist);
            CollectionAssert.AreNotEqual(testResult, resultList);
        }

        [TestMethod]
        [Description("Test if FindPeaks works properly if there are no peaks in signal")]
        public void FindPeaksTest3()
        {
            uint fs = 10;
            double dist = 1;
            double[] testArray = { 0, 0, 0, 0, 0, 0, 0 };
            List<double> resultList = new List<double>();

            R_Peaks_Alg test = new R_Peaks_Alg();
            List<double> testResult = test.FindPeaks(testArray, fs, dist);
            CollectionAssert.AreEqual(testResult, resultList);
        }

        [TestMethod]
        [Description("Test if FindPeaks throws exception if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void FindPeaksNullTest()
        {
            uint fs = 10;
            double dist = 1;
            double[] testArray = null;
            double[] resultArray = { 5 };

            R_Peaks_Alg test = new R_Peaks_Alg();
            List<double> testResult = test.FindPeaks(testArray, fs, dist);
        }

        [TestMethod]
        [Description("Test if FindPeaks throws exception if fs equals zero")]
        [ExpectedException(typeof(ArgumentException), "Non-positive frequency value given as parameter")]
        public void FindPeaksZeroTest()
        {
            uint fs = 0;
            double[] testArray = { 1, 1, 2, 3, 5, 2, 0, -1, 0, 1 };
            double dist = 1;
            R_Peaks_Alg test = new R_Peaks_Alg();
            List<double> testResult = test.FindPeaks(testArray, fs, dist);
        }

        // Test for other functions
        // CutSignal function
        [TestMethod]
        [Description("Test if CutSignal works properly")]
        public void CutSignalTest()
        {
            int begin = 2;
            int end = 5;
            double[] testArray = { 0, 1, 2, 3, 4, 5, 6, 7 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            double[] resultArray = { 2, 3, 4, 5 };
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.CutSignal(testVector, begin, end);
            Assert.AreEqual(testResult, resultVector);
        }

        [TestMethod]
        [Description("Test if CutSignal works properly - not equality test")]
        public void CutSignalTest2()
        {
            int begin = 2;
            int end = 5;
            double[] testArray = { 0, 1, 2, 3, 4, 5, 6, 7 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            double[] resultArray = { 1, 2, 3, 4 };
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.CutSignal(testVector, begin, end);
            Assert.AreNotEqual(testResult, resultVector);
        }

        [TestMethod]
        [Description("Test if CutSignal throws exception if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void CutSignalNullTest()
        {
            int begin = 2;
            int end = 5;
            Vector<double> testVector = null;
            double[] resultArray = { 1, 2, 3, 4 };
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.CutSignal(testVector, begin, end);
        }

        [TestMethod]
        [Description("Test if CutSignal throws exception if end is lower than begin")]
        [ExpectedException(typeof(ArgumentException), "Wrong begin or end value given as parameter")]
        public void CutSignalTimeTravelTest()
        {
            int begin = 5;
            int end = 2;
            double[] testArray = { 0, 1, 2, 3, 4, 5, 6, 7 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.CutSignal(testVector, begin, end);
        }

        // Diff function
        [TestMethod]
        [Description("Test if Diff works properly")]
        public void DiffTest()
        {
            double[] testArray = { 2, 4, 6, 3, 1, -2, -4, 5 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            double[] resultArray = { 2, 2, -3, -2, -3, -2, 9 };
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.Diff(testVector);
            Assert.AreEqual(testResult, resultVector);
        }

        [TestMethod]
        [Description("Test if Diff works properly - not equality test")]
        public void DiffTest2()
        {
            double[] testArray = { 0, 1, 2, 3, 4, 5, 6, 7 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            double[] resultArray = { 2, 2, 3, 2, -3, -2, 9 };
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.Diff(testVector);
            Assert.AreNotEqual(testResult, resultVector);
        }

        [TestMethod]
        [Description("Test if Diff throws exception if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void DiffNullTest()
        {
            Vector<double> testVector = null;
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.Diff(testVector);
        }

        [TestMethod]
        [Description("Test if Diff throws exception if input vector have only one element")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Vector must be at least 2 long")]
        public void DiffWrongVectorTest()
        {
            double[] testArray = { 5 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.Diff(testVector);
        }

        // ____________________________________DOTĄD DZIAŁAM!_____________________________________________
        //...

    }
}
