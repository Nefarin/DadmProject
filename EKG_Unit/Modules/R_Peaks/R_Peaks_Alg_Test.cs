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
        //______________________________________________________________________________________________
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

        //EMD
        [TestMethod]
        [Description("Test if EMD throws null if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void EMDNullTest()
        {
            Vector<double> testVector = null;
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.EMD(testVector, 10);
        }

        [TestMethod]
        [Description("Test if EMD return input signal if input signa does not suit (could not be decomposed)")]
        public void EMDFailureTest()
        {
            double[] testArray = { 0, 0, 0, 0, 0 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.EMD(testVector, 10);
            Assert.IsNull(testResult);
        }

        [TestMethod]
        [Description("Test if EMD performs properly")]
        public void EMDTest()
        {
            double[] testArray = { -0.03, -0.015, -0.02, -0.005, -0.005, -0.01, -0.015, -0.025, -0.015, -0.015, -0.015, -0.025, -0.025, -0.025, -0.005, 0.005, -0.01, -0.01, -0.015, -0.035, -0.03, -0.02, -0.02, -0.03, -0.025, -0.03, -0.015, 0.005, 0.005, 0.01, 0, 0.005, 0.02, 0.03, 0.02, 0.025, 0.005, 0.005, 0.03, 0.025, 0.025, 0.02, 0.015, 0.015, 0.025, 0.025, 0.025, 0.01, -0.005, 0, -0.005, -0.005, -0.015, -0.015, -0.025, -0.015, -0.01, 0, -0.015, -0.025, -0.045, -0.045, -0.05, -0.025, -0.03, -0.035, -0.04, -0.035, -0.035, -0.02, -0.03, -0.04, -0.04, -0.045, -0.025, -0.025, -0.035, -0.05, -0.07, -0.08, -0.09, -0.1, -0.13, -0.16, -0.155, -0.105, -0.005, 0.12, 0.215, 0.265, 0.315, 0.39, 0.5, 0.61, 0.695, 0.685, 0.56, 0.305, 0.035, -0.15, -0.25, -0.27, -0.25, -0.195, -0.145, -0.11, -0.095, -0.085, -0.095, -0.095, -0.08, -0.085, -0.085, -0.085, -0.09, -0.075, -0.075, -0.075, -0.07, -0.085, -0.095, -0.09, -0.075, -0.065, -0.065, -0.075, -0.095, -0.09, -0.085, -0.07, -0.065, -0.075, -0.08, -0.08, -0.07, -0.065, -0.07, -0.07, -0.08, -0.07, -0.065, -0.065, -0.065, -0.07, -0.085, -0.085, -0.065, -0.065, -0.055, -0.075, -0.075, -0.08, -0.07, -0.065, -0.075, -0.085, -0.08, -0.08, -0.075, -0.075, -0.075, -0.085, -0.095, -0.095, -0.075, -0.075, -0.085, -0.09, -0.115, -0.115, -0.105, -0.09, -0.11, -0.12, -0.12, -0.135, -0.13, -0.125, -0.145, -0.15, -0.165, -0.17, -0.155, -0.16, -0.17, -0.19, -0.195, -0.2, -0.19, -0.185, -0.19, -0.2, -0.21, -0.22, -0.205, -0.21, -0.21, -0.2, -0.205, -0.19, -0.185, -0.165, -0.155, -0.145, -0.155, -0.13, -0.095, -0.075, -0.07, -0.06, -0.065, -0.065, -0.045, -0.025, -0.01, -0.015, -0.015, -0.005, 0.005, 0.015, 0.015, 0.005, 0.005, 0.005, 0.015, 0.03, 0.015, 0.005, 0.005, 0.005, 0.015, 0.025, 0.025, 0.015, 0.005, 0.005, 0.005, 0.02, 0.015, 0.01, 0.005, 0.005, 0.01, 0.015, 0.015, 0.01, 0, -0.015, 0.005, 0.015, 0.015, 0.015, 0, -0.005, 0.01, 0.01, 0, 0, 0.005, -0.02, -0.005, 0.01, 0, -0.02, -0.03, -0.01, -0.005, 0, 0.005, -0.02, -0.025, -0.025, -0.015, -0.005, -0.005, -0.015, -0.025, -0.02, -0.025, 0.005, -0.015, -0.02, -0.015, -0.025, -0.015, -0.01, -0.01, -0.025, -0.03, -0.02, -0.01, -0.005, 0, -0.005, -0.02, -0.03, -0.015, -0.005, 0, -0.03, -0.02, -0.015, -0.005, 0, -0.005, -0.02, -0.02, -0.025, -0.005, 0, 0, -0.01, -0.03, -0.015, 0, 0, -0.005, -0.01, -0.015, -0.02, -0.005, 0, 0.005, -0.005, -0.005, -0.005, 0.02, 0.03, 0.01, 0.01, 0, 0.015, 0.035, 0.055, 0.055, 0.03, 0.025, 0.03, 0.035, 0.05, 0.04, 0.025, 0.01, 0.01, 0.015, 0.015, 0.025, 0.005, -0.015, -0.005, 0, 0.005, -0.005, -0.01, -0.02, -0.015, -0.01, -0.005, 0, -0.01, -0.02, -0.02, -0.02, 0, -0.005, -0.015, -0.025, -0.02, -0.015, -0.005, -0.02, -0.025, -0.04, -0.06, -0.08, -0.1, -0.145, -0.175, -0.145, -0.06, 0.075, 0.225, 0.345, 0.46, 0.575, 0.705, 0.835, 0.905, 0.81, 0.52, 0.155, -0.05, -0.07, -0.04, -0.04, -0.05, -0.055, -0.045, -0.045, -0.045, -0.055, -0.06, -0.06, -0.07, -0.045, -0.045, -0.05, -0.065, -0.07, -0.07, -0.065, -0.06, -0.06, -0.07, -0.08, -0.08, -0.07, -0.055, -0.065, -0.08, -0.09, -0.075, -0.065, -0.055, -0.06, -0.06, -0.08, -0.08, -0.065, -0.06, -0.055, -0.06, -0.075, -0.07, -0.075, -0.065, -0.065, -0.07, -0.08, -0.085, -0.08, -0.055, -0.07, -0.08, -0.09, -0.09, -0.07, -0.07, -0.085, -0.1, -0.1, -0.1, -0.095, -0.075, -0.095, -0.1, -0.11, -0.11, -0.115, -0.11, -0.11, -0.135, -0.145, -0.155, -0.15, -0.145, -0.16, -0.17, -0.175, -0.185, -0.17, -0.17, -0.185, -0.2, -0.215, -0.205, -0.21, -0.19, -0.205, -0.21, -0.225, -0.22, -0.21, -0.205, -0.2, -0.2, -0.205, -0.185, -0.165, -0.145, -0.135, -0.125, -0.11, -0.095, -0.075, -0.055, -0.05, -0.04, -0.045, -0.03, -0.005, 0, 0, -0.005, -0.01, -0.02, 0.005, 0.01, 0, 0, -0.01, 0, 0.01, 0.025, 0, -0.005, -0.01, 0, 0, 0.02, 0.01, 0.01, -0.005, -0.005, 0.005, 0.015, 0.01, 0.01, -0.01, -0.01, -0.005, -0.005, 0, -0.015, -0.025, -0.01, -0.01, -0.01, -0.015, -0.025, -0.025, -0.03, -0.01, -0.01, -0.03, -0.05, -0.05, -0.045, -0.03, -0.015, -0.03, -0.04, -0.045, -0.045, -0.04, -0.015, -0.025, -0.04, -0.04, -0.045, -0.04, -0.03, -0.035, -0.04, -0.03, -0.05, -0.025, -0.015};
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            double[] resultArray = { 97, 382 };
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.EMD(testVector, 360);

            Assert.AreEqual(resultVector, testResult);
        }

        [TestMethod]
        [Description("Test if EMD throws exception if fs equals zero")]
        [ExpectedException(typeof(ArgumentException), "Non-positive frequency value given as parameter")]
        public void EMDZeroTest()
        {
            uint fs = 0;
            double[] testArray = { 1, 1, 2, 3, 5, 8, 13, 21, 34, 55 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.EMD(testVector, fs);
        }

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

        [TestMethod]
        [Description("Test if HilbertTransform works properly if array is empty - not null")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Array must not be empty")]
        public void HilbertTransformEmptyTest()
        {
            double[] testArray = new double[] { };
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.HilbertTransform(testArray);
        }

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

        [TestMethod]
        [Description("Test if Filtering works properly if array is empty - not null")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Array must not be empty")]
        public void FilteringEmptyTest()
        {
            double[] testArray = new double[] { };
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Filtering(25, 5, 12, testArray);
        }

        // Integration for Hilbert method
        [TestMethod]
        [Description("Test if Integration works properly")]
        public void IntegrationTest()
        {
            double[] testArray = { 1, 2, 3, 4};
            double[] resultArray = { 0.3333, 0.6667, 1.0000, 0.7778};

            for (int i = 0; i < resultArray.Length; i++)
            {
                resultArray[i] = Math.Round(resultArray[i], 4);
            }
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Integration(testArray, 6);
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

        [TestMethod]
        [Description("Test if Integration works properly if array is empty - not null")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Array must not be empty")]
        public void IntegrationEmptyTest()
        {
            double[] testArray = new double[] { };
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Integration(testArray, 10);
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

        [TestMethod]
        [Description("Test if FindPeak works properly if array is empty - not null")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Array must not be empty")]
        public void FindPeakEmptyTest()
        {
            double[] testArray = new double[] { };
            double[] testArray2 = new double[] { };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray2);
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.FindPeak(testArray, testVector);
        }

        [TestMethod]
        [Description("Test if FindPeak works properly if array is empty - not null")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Array must not be empty")]
        public void FindPeakEmptyTest1()
        {
            double[] testArray = new double[] { 1, 2, 3 };
            double[] testArray2 = new double[] { };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray2);
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.FindPeak(testArray, testVector);
        }

        [TestMethod]
        [Description("Test if FindPeak works properly if array is empty - not null")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Array must not be empty")]
        public void FindPeakEmptyTest2()
        {
            double[] testArray = new double[] { };
            double[] testArray2 = new double[] { 1, 2, 3 };
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

        [TestMethod]
        [Description("Test if Derivative works properly if array is empty - not null")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Array must not be empty")]
        public void DerivativeEmptyTest()
        {
            double[] testArray = new double[] { };
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

        [TestMethod]
        [Description("Test if Squaring works properly if array is empty - not null")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Array must not be empty")]
        public void SquaringEmptyTest()
        {
            double[] testArray = new double[] { };
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Squaring(testArray);
        }

        // Integrating for PT method
        [TestMethod]
        [Description("Test if Integrating works properly")]
        public void IntegratingTest()
        {
            double[] testArray = { 1, 0, 2, -1, 5, 3, 0};
            double[] resultArray = { 0, 0, 1, 0, 2, 4, 1.5};

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
        [Description("Test if Integrating throws exception if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void IntegratingNullTest()
        {
            double[] testArray = null;
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Integrating(testArray, 15);
        }

        [TestMethod]
        [Description("Test if Integrating throws exception if fs equals zero")]
        [ExpectedException(typeof(ArgumentException), "Non-positive frequency value given as parameter")]
        public void IntegratingZeroTest()
        {
            uint fs = 0;
            double[] testArray = { 1, 1, 2, 3, 5, 8, 13, 21, 34, 55 };
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Integrating(testArray, fs);
        }

        [TestMethod]
        [Description("Test if Integrating works properly if array is empty - not null")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Array must not be empty")]
        public void IntegratingEmptyTest()
        {
            double[] testArray = new double[] { };
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Integrating(testArray, 15);
        }

        // Finding Peaks for PT method
        [TestMethod]
        [Description("Test if FindPeaks works properly")]
        public void FindPeaksTest()
        {
            uint fs = 10;
            double dist = 0.5;
            double[] testArray = { 2, 0, 1, -2, 9, -2, 0, 1, 0, 1 };
            double[] resultArray = { 4 };

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

        [TestMethod]
        [Description("Test if FindPeaks works properly if array is empty - not null")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Array must not be empty")]
        public void FindPeaksEmptyTest()
        {
            double[] testArray = new double[] { };
            R_Peaks_Alg test = new R_Peaks_Alg();
            List<double> testResult = test.FindPeaks(testArray, 10, 0.5);
        }

        // FindRs for PT method
        [TestMethod]
        [Description("Test if FindRs works properly")]
        public void FindRsTest()
        {
            double[] testArray = { 1, 0, 2, -1, 5, 3, 0, -1 };
            double[] testArray1 = { 0.5, 0.5, 1, 0.5, 2, 4, 1.5, 0 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray1);
            double[] resultArray = { 4};
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.findRs(testArray, testVector, 10);

            Assert.AreEqual(testResult, resultVector);
        }

        [TestMethod]
        [Description("Test if FindRs throws exception if there are no peaks in signal")]
        [ExpectedException(typeof(Exception), "No Rs found")]
        public void FindRsTest2()
        {
            double[] testArray = { 0, 0, 0, 0, 0, 0, 0 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.findRs(testArray, testVector, 10 );
        }

        [TestMethod]
        [Description("Test if FindRs throws exception if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void FindRsNullTest()
        {
            double[] testArray = { 1, 2, 3 };
            Vector<double> testVector = null;
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.findRs(testArray, testVector, 10);
        }

        [TestMethod]
        [Description("Test if FindRs throws exception if fs equals zero")]
        [ExpectedException(typeof(ArgumentException), "Non-positive frequency value given as parameter")]
        public void FindRsZeroTest()
        {
            uint fs = 0;
            double[] testArray = { 1, 1, 2, 3, 5, 2, 0, -1, 0, 1 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.findRs(testArray, testVector, fs);
        }

        [TestMethod]
        [Description("Test if FindPeaks works properly if array is empty - not null")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Array must not be empty")]
        public void FindRsEmptyTest()
        {
            double[] testArray = new double[] { };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.findRs(testArray, testVector, 10);
        }

        // Tests for other functions
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

        [TestMethod]
        [Description("Test if CutSignal works properly if array is empty - not null")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Array must not be empty")]
        public void CutSignalEmptyTest()
        {
            int begin = 2;
            int end = 5;
            double[] testArray = new double[] { };
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

        [TestMethod]
        [Description("Test if Diff works properly if array is empty - not null")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Array must not be empty")]
        public void DiffEmptyTest()
        {
            double[] testArray = new double[] { };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.Diff(testVector);
        }

        // RRinMS function
        [TestMethod]
        [Description("Test if RRinMS works properly")]
        public void RRinMSTest()
        {
            uint fs = 10;
            double[] testArray = { 0, 11, 22, 32, 43, 53 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            double[] resultArray = { 1100, 1100, 1000, 1100, 1000 };
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.RRinMS(testVector, fs);
            Assert.AreEqual(testResult, resultVector);
        }

        [TestMethod]
        [Description("Test if RRinMS works properly - not equality test")]
        public void RRinMSTest2()
        {
            uint fs = 10;
            double[] testArray = { 0, 11, 22, 32, 43, 53 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            double[] resultArray = { 11, 11, 10, 11, 10 };
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.RRinMS(testVector, fs);
            Assert.AreNotEqual(testResult, resultVector);
        }

        [TestMethod]
        [Description("Test if RRinMS throws exception if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void RRinMSNullTest()
        {
            Vector<double> testVector = null;
            uint fs = 10;
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.RRinMS(testVector, fs);
        }

        [TestMethod]
        [Description("Test if RRinMS throws exception if fs equals zero")]
        [ExpectedException(typeof(ArgumentException), "Non-positive frequency value given as parameter")]
        public void RRinMSZeroTest()
        {
            uint fs = 0;
            double[] testArray = { 0, 11, 22, 32, 43, 53 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.RRinMS(testVector, fs);
        }

        [TestMethod]
        [Description("Test if RRinMS works properly if array is empty - not null")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Array must not be empty")]
        public void RRinMSEmptyTest()
        {
            uint fs = 5;
            double[] testArray = new double[] { };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.RRinMS(testVector, fs);
        }

        //Tests for Hilbert algorithm 
        [TestMethod]
        [Description("Test if Hilbert works properly")]
        public void HilbertTest()
        {
            uint fs = 10;
            double[] testArray = { 1, 1, 1, 0, 0, 1, 1, 0, 0, -3, 12, -3, 0, 0, 0, 0, 1, 1, 1, 0, 0, 1, 2, 1, 0, -2, 11, -3, -1, 0, 0, 1, 1, 0, 0, 1, 1, 0, -1, 12, -4, -1, 0, 0, 0, 1, 1, 0, 0, 1 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            double[] resultArray = { 10, 26, 39 };
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.Hilbert(testVector, fs);
            Assert.AreEqual(testResult, resultVector);
        }

        [TestMethod]
        [Description("Test if Hilbert works properly - not equality test")]
        public void HilbertTest2()
        {
            uint fs = 10;
            double[] testArray = { 1, 1, 1, 0, 0, 1, 1, 0, 0, -3, 12, -3, 0, 0, 0, 0, 1, 1, 1, 0, 0, 1, 2, 1, 0, -2, 11, -3, -1, 0, 0, 1, 1, 0, 0, 1, 1, 0, -1, 12, -4, -1, 0, 0, 0, 1, 1, 0, 0, 1 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            double[] resultArray = { 12, 28, 41 };
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.Hilbert(testVector, fs);
            Assert.AreNotEqual(testResult, resultVector);
        }

        [TestMethod]
        [Description("Test if Hilbert throws exception if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void HilbertNullTest()
        {
            Vector<double> testVector = null;
            uint fs = 31;
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.Hilbert(testVector, fs);
        }

        [TestMethod]
        [Description("Test if Hilbert throws exception if fs equals zero")]
        [ExpectedException(typeof(ArgumentException), "Non-positive frequency value given as parameter")]
        public void HilbertZeroTest()
        {
            uint fs = 0;
            double[] testArray = { 2, 2, 0, 0, 1, 1, 0, 0, -3, 12, -4, 0, 0, 0, 2, 5, 3, 2, 0, 0 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.Hilbert(testVector, fs);
        }

        [TestMethod]
        [Description("Test if Hilbert works properly if array is empty - not null")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Array must not be empty")]
        public void HilbertEmptyTest()
        {
            uint fs = 31;
            double[] testArray = new double[] { };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.Hilbert(testVector, fs);
        }

        //Tests for Pan-Tompkins algorithm 
        
        [TestMethod]
        [Description("Test if PanTompkins works properly")]
        public void PanTompkinsTest()
        {
            uint fs = 360;
            double[] testArray = { -0.00038, -0.0031, -0.012, -0.029, -0.051, -0.071, -0.081, -0.081, -0.074, -0.067, -0.065, -0.068, -0.075, -0.082, -0.084, -0.081, -0.073, -0.063, -0.055, -0.047, -0.041, -0.036, -0.031, -0.029, -0.033, -0.043, -0.056, -0.066, -0.07, -0.069, -0.066, -0.065, -0.067, -0.073, -0.079, -0.086, -0.095, -0.1, -0.11, -0.1, -0.092, -0.079, -0.075, -0.081, -0.09, -0.096, -0.095, -0.089, -0.084, -0.085, -0.09, -0.094, -0.096, -0.095, -0.095, -0.096, -0.096, -0.092, -0.083, -0.075, -0.075, -0.087, -0.11, -0.14, -0.16, -0.18, -0.19, -0.21, -0.21, -0.19, -0.13, -0.029, 0.09, 0.21, 0.31, 0.41, 0.5, 0.59, 0.65, 0.62, 0.48, 0.24, -0.03, -0.24, -0.32, -0.3, -0.21, -0.13, -0.083, -0.085, -0.11, -0.14, -0.15, -0.15, -0.13, -0.12, -0.12, -0.13, -0.13, -0.13, -0.12, -0.11, -0.11, -0.11, -0.12, -0.12, -0.12, -0.12, -0.11, -0.11, -0.11, -0.1, -0.096, -0.09, -0.088, -0.091, -0.096, -0.098, -0.096, -0.093, -0.091, -0.094, -0.098, -0.097, -0.091, -0.082, -0.076, -0.076, -0.08, -0.085, -0.087, -0.085, -0.082, -0.08, -0.079, -0.079, -0.078, -0.08, -0.085, -0.09, -0.093, -0.09, -0.083, -0.076, -0.074, -0.075, -0.076, -0.074, -0.069, -0.067, -0.07, -0.078, -0.085, -0.086, -0.081, -0.076, -0.077, -0.085, -0.097, -0.11, -0.11, -0.11, -0.1, -0.1, -0.11, -0.12, -0.12, -0.13, -0.13, -0.14, -0.14, -0.14, -0.14, -0.14, -0.14, -0.15, -0.15, -0.15, -0.15, -0.14, -0.14, -0.14, -0.14, -0.14, -0.13, -0.12, -0.11, -0.094, -0.083, -0.071, -0.058, -0.046, -0.038, -0.032, -0.028, -0.019, -0.0039, 0.013, 0.027, 0.036, 0.04, 0.046, 0.054, 0.062, 0.066, 0.065, 0.062, 0.06, 0.06, 0.06, 0.057, 0.05, 0.042, 0.038, 0.039, 0.044, 0.049, 0.05, 0.047, 0.043, 0.041, 0.041, 0.04, 0.037, 0.033, 0.03, 0.029, 0.029, 0.027, 0.023, 0.019, 0.019, 0.025, 0.032, 0.034, 0.026, 0.012, -0.0021, -0.0092, -0.0093, -0.0064, -0.0048, -0.0052, -0.0058, -0.0064, -0.009, -0.015, -0.024, -0.029, -0.028, -0.02, -0.011, -0.0064, -0.0076, -0.013, -0.016, -0.015, -0.0096, -0.0064, -0.0092, -0.017, -0.025, -0.031, -0.032, -0.03, -0.027, -0.023, -0.019, -0.017, -0.02, -0.026, -0.032, -0.035, -0.033, -0.028, -0.022, -0.018, -0.019, -0.021, -0.022, -0.018, -0.013, -0.0094, -0.0097, -0.014, -0.019, -0.021, -0.022, -0.021, -0.021, -0.02, -0.015, -0.0087, -0.0031, -0.0024, -0.008, -0.018, -0.026, -0.03, -0.028, -0.025, -0.021, -0.019, -0.014, -0.0062, 0.0043, 0.013, 0.018, 0.02, 0.021, 0.025, 0.029, 0.03, 0.027, 0.023, 0.021, 0.023, 0.027, 0.029, 0.028, 0.025, 0.023, 0.022, 0.022, 0.019, 0.012, 0.0021, -0.0064, -0.012, -0.015, -0.017, -0.019, -0.019, -0.017, -0.013, -0.012, -0.018, -0.029, -0.041, -0.049, -0.048, -0.041, -0.033, -0.029, -0.029, -0.03, -0.03, -0.029, -0.029, -0.031, -0.033, -0.032, -0.03, -0.029, -0.034, -0.047, -0.066, -0.084, -0.1, -0.12, -0.13, -0.14, -0.11, -0.046, 0.056, 0.18, 0.29, 0.38, 0.46, 0.52, 0.6, 0.68, 0.71, 0.66, 0.49, 0.24, -0.048, -0.28, -0.41, -0.43, -0.35, -0.25, -0.17, -0.12, -0.11, -0.12, -0.13, -0.13, -0.13, -0.12, -0.11, -0.1, -0.1, -0.1, -0.098, -0.095, -0.094, -0.097, -0.099, -0.097, -0.09, -0.081, -0.076, -0.079, -0.086, -0.09, -0.088, -0.079, -0.07, -0.066, -0.066, -0.067, -0.067, -0.064, -0.061, -0.059, -0.059, -0.057, -0.055, -0.052, -0.053, -0.056, -0.059, -0.059, -0.054, -0.047, -0.043, -0.044, -0.048, -0.051, -0.052, -0.051, -0.05, -0.051, -0.052, -0.052, -0.05, -0.048, -0.049, -0.053, -0.057, -0.057, -0.053, -0.05, -0.051, -0.059, -0.07, -0.076, -0.076 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            double[] resultArray = { 78, 372 };
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.PanTompkins(testVector, fs);
            Assert.AreEqual(testResult, resultVector);
        }

        [TestMethod]
        [Description("Test if PanTompkins works properly - not equality test")]
        public void PanTompkinsTest2()
        {
            uint fs = 31;
            double[] testArray = { 1, 1, 1, 0, 0, 1, 1, 0, 0, -3, 12, -3, 0, 0, 0, 0, 1, 1, 1, 0, 0, 1, 2, 1, 0, -2, 11, -3, -1, 0, 0, 1, 1, 0, 0, 1, 1, 0, -1, 12, -4, -1, 0, 0, 0, 1, 1, 0, 0, 1 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            double[] resultArray = { 12, 28, 41 };
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.PanTompkins(testVector, fs);
            Assert.AreNotEqual(testResult, resultVector);
        }

        [TestMethod]
        [Description("Test if PanTompkins throws exception if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void PanTompkinsNullTest()
        {
            Vector<double> testVector = null;
            uint fs = 31;
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.PanTompkins(testVector, fs);
        }

        [TestMethod]
        [Description("Test if PanTompkins throws exception if fs equals zero")]
        [ExpectedException(typeof(ArgumentException), "Non-positive frequency value given as parameter")]
        public void PanTompkinsZeroTest()
        {
            uint fs = 0;
            double[] testArray = { 2, 2, 0, 0, 1, 1, 0, 0, -3, 12, -4, 0, 0, 0, 2, 5, 3, 2, 0, 0 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.PanTompkins(testVector, fs);
        }

        [TestMethod]
        [Description("Test if PanTompkins works properly if array is empty - not null")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Array must not be empty")]
        public void PanTompkinsEmptyTest()
        {
            uint fs = 31;
            double[] testArray = new double[] { };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.PanTompkins(testVector, fs);
        }

    }
}
