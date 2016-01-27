using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.HRV_DFA;
using System.Collections.Generic;

namespace EKG_Unit.Modules.HRV_DFA
{
    [TestClass]
    public class HRV_DFA_Alg_Test
    {
        //Test for subsidary methods
        //ConvertToLog
        [TestMethod]
        [Description("Test if ConvertToLog works properly")]
        public void ConvertToLog_Test()
        {
            double[] valuesToTest = { 1, 10, 100 };
            double[] valuesToCompare = { 0, 1, 2 };
            Vector<double> vectorToTest = Vector<double>.Build.DenseOfArray(valuesToTest);
            Vector<double> vectorToCompare = Vector<double>.Build.DenseOfArray(valuesToCompare);
            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> result = test.ConvertToLog(vectorToTest);
            Assert.AreEqual(vectorToCompare, result);
        }
        [TestMethod]
        [Description("Test if empty input is given")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConvertToLog_NullArgumentTest()
        {
            Vector<double> testInput = null;
            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> result = test.ConvertToLog(testInput);
        }

        //LinearSquare
        [TestMethod]
        [Description("Test if LinearSquare inputs are not equal")]
        [ExpectedException(typeof(ArgumentException))]
        public void LinearSquare_ArgumetsEqualityTest()
        {
            double[] testX = { 1, 2, 3, 4, 5 };
            double[] testY = { 2, 5, 6 };
            Vector<double> testVectorX = Vector<double>.Build.DenseOfArray(testX);
            Vector<double> testVectorY = Vector<double>.Build.DenseOfArray(testY);
            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Tuple<double[], Vector<double>> results = test.LinearSquare(testVectorX, testVectorY);
        }
        [TestMethod]
        [Description("Test if output line is equaly long as input vector")]
        public void LinearSquare_OutputLineEqualTest()
        {
            double[] testX = { 1, 2, 3, 4, 5 };
            double[] testY = { 2, 5, 6 , 7, 4};
            Vector<double> testVectorX = Vector<double>.Build.DenseOfArray(testX);
            Vector<double> testVectorY = Vector<double>.Build.DenseOfArray(testY);
            int testVectorLength = testVectorY.Count;
            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Tuple<double[], Vector<double>> results = test.LinearSquare(testVectorX, testVectorY);
            int resultVecLength = results.Item2.Count;
            Assert.AreEqual(testVectorLength, resultVecLength);
        }
        [TestMethod]
        [Description("Test if null inputs appear")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LinearSquare_NullArgumentTest()
        {
            Vector<double> vectorX = null;
            Vector<double> vectorY = null;
            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Tuple<double[], Vector<double>> results = test.LinearSquare(vectorX, vectorY);
        }

        //RemoveZeros
        [TestMethod]
        [Description("Test if RemoveZeros works properly")]
        public void RemoveZeros_Test()
        {
            double[] testArray = {1, 3, 3, 2, 0, 0,0 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            double[] resultAr = { 1, 3, 3, 2 };
            Vector<double> resultVc = Vector<double>.Build.DenseOfArray(resultAr);

            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> testResult = test.RemoveZeros(testVector);
            Assert.AreEqual(resultVc, testResult);
        }
        [TestMethod]
        [Description("Test if Remove zeros works properly when no zeros inside vector")]
        public void RemoveZeros_WhenNoZerosTest()
        {
            double[] testArray1 = { 1, 2, 3};
            Vector<double> testVector1 = Vector<double>.Build.DenseOfArray(testArray1);
            double[] resultAr1 = { 1, 2, 3};
            Vector<double> resultVc1 = Vector<double>.Build.DenseOfArray(resultAr1);

            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> testResult = test.RemoveZeros(testVector1);
            Assert.AreEqual(resultVc1, testResult);
        }

        [TestMethod]
        [Description("Test if empty input for RemoveZeros - NullExpection")]
        [ExpectedException(typeof(ArgumentNullException), "Null given as parameter")]
        public void RemoveZeros_NullInputTest()
        {
            Vector<double> testVector2 = null;
            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> testResult = test.RemoveZeros(testVector2);
        }

        [TestMethod]
        [Description("Test if Remove zeros returns null when only zeros in Vector")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Null given as parameter")]
        public void RemoveZeros_WhenAllZerosTest()
        {
            double[] testArray = { 0, 0, 0, 0 };
            Vector<double> testVector3 = Vector<double>.Build.DenseOfArray(testArray);
            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> testResult = test.RemoveZeros(testVector3);
        }
    }
}
