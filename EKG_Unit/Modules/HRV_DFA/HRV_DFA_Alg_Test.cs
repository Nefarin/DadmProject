using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.HRV_DFA;
using MathNet.Numerics;

namespace EKG_Unit.Modules.HRV_DFA
{
    [TestClass]
    public class HRV_DFA_Alg_Test
    {
        //Testing main methods
        //ComputeDfaFluctuation
        [TestMethod]
        [Description("Test if not null is returned")]
        public void ComputeDfaFluctuation_NotNullTest()
        {
            double[] values = Generate.LinearRange(5, 1, 20);
            double[] samples = { 61, 14, 12, 35, 26, 33, 43, 23, 43, 22, 56, 32, 23 };
            Vector<double> samplesVec = Vector<double>.Build.DenseOfArray(samples);
            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> result = test.ComputeDfaFluctuation(samplesVec, values);
            Assert.IsNotNull(result);
        }
        [TestMethod]
        [Description("Test if input data is to short")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ComputeDfaFluctuations_NullInputTest()
        {
            double[] values = Generate.LinearRange(5, 1, 20);
            Vector<double> samplesVec = null;
            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> result = test.ComputeDfaFluctuation(samplesVec, values);
            Assert.IsNull(result);
        }

        //CommputeInBox
        [TestMethod]
        [Description("Test if not null is returned")]
        public void ComputeInBox_NotNullOutputTest()
        {
            double[] input1 = { 1, 3, 6, 5, 8, 11 };
            double[] input2 = { 2, 4, 6, 8, 10, 12 };
            Vector<double> input1v = Vector<double>.Build.DenseOfArray(input1);
            Vector<double> input2v = Vector<double>.Build.DenseOfArray(input2);
            int a = 7;
            HRV_DFA_Alg test = new HRV_DFA_Alg();
            double result = test.ComputeInBox(input1v, input2v, a);
            Assert.IsNotNull(result);

        }
        [TestMethod]
        [Description("Test if not equaly long input vectors are given")]
        [ExpectedException(typeof(ArgumentException))]
        public void ComputeInBox_InputsTest()
        {
            double[] input1 = { 1, 3, 6, 5, 8, 11 };
            double[] input2 = { 2, 4, 6};
            Vector<double> input1v = Vector<double>.Build.DenseOfArray(input1);
            Vector<double> input2v = Vector<double>.Build.DenseOfArray(input2);
            int a = 7;
            HRV_DFA_Alg test = new HRV_DFA_Alg();
            double result = test.ComputeInBox(input1v, input2v, a);
        }
        [TestMethod]
        [Description("Test if one null input is given")]
        [ExpectedException(typeof(NullReferenceException))]
        public void ComputeInBox_OneNullTest()
        {
            double[] input1 = { 1, 3, 6, 5, 8, 11 };
            Vector<double> input1v = Vector<double>.Build.DenseOfArray(input1);
            Vector<double> input2v = null;
            int a = 7;
            HRV_DFA_Alg test = new HRV_DFA_Alg();
            double result = test.ComputeInBox(input1v, input2v, a);
            Assert.IsNull(result);
        }

        //Testing subsidary methods
        //CumulativeSum
        [TestMethod]
        [Description("Test if CumulativeSum works properly")]
        public void CumulativeSum_Test()
        {
            double[] testArray = { 6, 10, 3, 1, 8 };
            double[] expectedResults = { 6, 16, 19, 20, 28 };

            HRV_DFA_Alg test = new HRV_DFA_Alg();
            double[] result = test.CumulativeSum(testArray);
            CollectionAssert.AreEqual(expectedResults, result);
        }
        [TestMethod]
        [Description("Test if output vector is equaly long as input vector")]
        public void CumulativeSum_VectorsEqualityTest()
        {
            double[] testArray = { 6, 10, 3, 1, 8 };
            HRV_DFA_Alg test = new HRV_DFA_Alg();
            double[] result = test.CumulativeSum(testArray);
            int testArrayLength = testArray.Length;
            int resultArrayLength = result.Length;
            Assert.AreEqual(testArrayLength, resultArrayLength);
        }

        //IntegrateDfa
        [TestMethod]
        [Description("Test if output vector is equaly long as input vector")]
        public void IntegrateDfa_VectorsEqualityTest()
        {
            double[] inputArray = { 6, 10, 3, 1, 8 };
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(inputArray);
            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> result = test.IntegrateDfa(testVector);
            int testVectorLength = testVector.Count;
            int resultVectorLength = result.Count;
            Assert.AreEqual(testVectorLength, resultVectorLength);
        }
        [TestMethod]
        [Description("Test if empty input is given")]
        [ExpectedException(typeof(NullReferenceException))]
        public void IntegrateDfa_NullArgumentTest()
        {
            Vector<double> testInput = null;
            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> result = test.IntegrateDfa(testInput);
        }

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
        [Description("Test if output vector is equaly long as input vector")]
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
