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
            double[] testX= { 1,2,3};
            double[] testY = { 0,1,0};
            int testLength = 5;
            double[] resultArray = {-1,0,1,0,-1};
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
            double[] resultArray = {2.1250, 5.66667, 0.2083, -5.2500, 0.2916667, 4.8333, -4.6250, -1.0833};
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.ExtractModeFun(testVector);
            for (int i=0; i<resultVector.Count; i++)
            {
                resultVector[i] = Math.Round(resultVector[i],4);
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
            double[] testArray = { 0, 2, -2, 4, -4, 1, -1, -2, 0, 4, 2, -2, 4, -4, 1, -1, -2, 0, 4, 1};
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            double[] imf1 = { 2.30957, 1.71301, -3.40024, 3.17601, -2.96313, 2.79749, -0.26414, -2.71007, -1.14758, 2.94203, 0.84069, -3.23907, 3.27102, -3.15764, 2.90023, 0.40448, -2.11690, -1.79369, 1.24454, -1.13177 };
            double[] imf2 = { -9.47447, -3.27213, 0.27978, 1.15376, -0.06656, -0.81764, -0.19879, 0.53112, 0.18858, -0.40234, -0.22762, 0.36748, 0.57561, -0.29400, -0.85333, -0.24241, 0.83112, 1.31704, 0.16507, -3.67489 };
            Vector<double> vect1 = Vector<double>.Build.DenseOfArray(imf1);
            Vector<double> vect2 = Vector<double>.Build.DenseOfArray(imf2);
            Vector<double>[] resultVectors= { vect1, vect2 };

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

    }
}
