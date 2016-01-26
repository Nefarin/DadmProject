﻿using System;
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
        /*[TestMethod]
        [Description("Test if FindPeaks works properly")]
        public void FindPeaksTest()
        {
            uint fs = 10;
            double dist = 1;
            double[] testArray = { 2, 0, 1, -2, 9, -2, 0, 1, 0, 1 };
            double[] resultArray = { 4 };

            R_Peaks_Alg test = new R_Peaks_Alg();
            List<double> testResult = test.FindPeaks(testArray, fs, dist);
            CollectionAssert.AreEqual(testResult, resultArray);
        }

        [TestMethod]
        [Description("Test if FindPeaks works properly - not equality test")]
        public void FindPeaksTest2()
        {
            uint fs = 10;
            double dist = 1;
            double[] testArray = { 2, 0, 1, -2, 9, -2, 0, 1, 0, 1 };
            double[] resultArray = { 5 };

            R_Peaks_Alg test = new R_Peaks_Alg();
            List<double> testResult = test.FindPeaks(testArray, fs, dist);
            CollectionAssert.AreNotEqual(testResult, resultArray);
        }*/
        
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
        [Description("Test if CutSignal throws exception if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void DiffNullTest()
        {
            Vector<double> testVector = null;
            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.Diff(testVector);
        }

        // ____________________________________DOTĄD DZIAŁAM!_____________________________________________
        //...

    }
}
