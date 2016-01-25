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
            Assert.AreEqual(testResult, null);
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
        [Description("Test if Filtering throws null if argument is not initialized")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void FilteringNullTest()
        {
            double[] testArray = null;
            R_Peaks_Alg test = new R_Peaks_Alg();
            double[] testResult = test.Filtering(25, 5, 12, testArray);
        }

        // ____________________________________DOTĄD DZIAŁAM!_____________________________________________

        /*
        [TestMethod]
        [Description("Test if find indexes return null if no elements find")]
        public void HilbertTransformFailureTest()
        {
            double[] testArray = { 0, 2, 0, 0, 4, 7, 9, 0 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            R_Peaks_Alg test = new R_Peaks_Alg();
            Vector<double> testResult = test.FindIndexes(testVector);
            Assert.AreEqual(testResult, null);
        }*/

    }
}
