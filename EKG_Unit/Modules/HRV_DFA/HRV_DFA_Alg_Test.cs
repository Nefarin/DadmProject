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
        //ObtainFluctuations
        [TestMethod]
        public void ObtainFluctuations_Test()
        {
            int step = 10;
            int start = 10;
            int stop = 100;
            double[] a = {600, 700, 650, 700, 750, 700, 600, 650, 800, 700, 650,600, 700, 650, 700, 750, 700, 600, 650, 800, 700, 650,
            600, 700, 650, 700, 750, 700, 600, 650, 800, 700, 650,600, 700, 650, 700, 750, 700, 600, 650, 800, 700, 650,
            600, 700, 650, 700, 750, 700, 600, 650, 800, 700, 650,600, 700, 650, 700, 750, 700, 600, 650, 800, 700, 650,
            600, 700, 650, 700, 750, 700, 600, 650, 800, 700, 650, 600, 700, 650, 700, 750, 700, 600, 650, 800, 700, 650,
            600, 700, 650, 700, 750, 700, 600, 650, 800, 700, 650, 600, 700, 650, 700, 750, 700, 600, 650, 800, 700, 650};
            Vector<double> v1 = Vector<double>.Build.DenseOfArray(a);
            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Tuple<Vector<double>, Vector<double>> result = test.ObtainFluctuations(step, start, stop,v1);
            Assert.IsNotNull(result.Item2);
        }
        [TestMethod]
        public void ObtainFluctuations_Test2()
        {
            int step = 10;
            int start = 50;
            int stop = 500;
            double[] a = {600, 700, 650, 700, 750, 700, 600, 650, 800, 700, 650,600, 700, 650, 700, 750, 700, 600, 650, 800, 700, 650,
            600, 700, 650, 700, 750, 700, 600, 650, 800, 700, 650,600, 700, 650, 700, 750, 700, 600, 650, 800, 700, 650,
            600, 700, 650, 700, 750, 700, 600, 650, 800, 700, 650,600, 700, 650, 700, 750, 700, 600, 650, 800, 700, 650,
            600, 700, 650, 700, 750, 700, 600, 650, 800, 700, 650, 600, 700, 650, 700, 750, 700, 600, 650, 800, 700, 650,
            600, 700, 650, 700, 750, 700, 600, 650, 800, 700, 650, 600, 700, 650, 700, 750, 700, 600, 650, 800, 700, 650};
            Vector<double> v1 = Vector<double>.Build.DenseOfArray(a);
            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Tuple<Vector<double>, Vector<double>> result = test.ObtainFluctuations(step, start, stop, v1);
            Assert.IsNotNull(result.Item2);
        }
        
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

        //CombineVectors
        [TestMethod]
        [Description("Test if CombineVectors works properly part 1")]
        public void CombineVectors_VectorsSizeTest()
        {
            double[] a1 = { 1, 2, 3, 4, 5 };
            double[] a2 = { 6, 7, 8 };
            double[] a3 = { 1, 2, 3, 4, 5, 6, 7, 8 };
            Vector<double> v1 = Vector<double>.Build.DenseOfArray(a1);
            Vector<double> v2 = Vector<double>.Build.DenseOfArray(a2);
            Vector<double> testv = Vector<double>.Build.DenseOfArray(a3);
            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> result = test.CombineVectors(v1, v2);
            Assert.AreEqual(testv, result);
        }
        [TestMethod]
        [Description("Test if CombineVectors works properly part 2")]
        public void CombineVectors_VectorsSizeTest2()
        {
            double[] a1 = { 1, 2, 3, 4, 5 };
            double[] a2 = { 6, 7, 8 };
            double a = 9;
            double[] a3 = { 1, 2, 3, 4, 5, 9, 6, 7, 8 };
            Vector<double> v1 = Vector<double>.Build.DenseOfArray(a1);
            Vector<double> v2 = Vector<double>.Build.DenseOfArray(a2);
            Vector<double> testv = Vector<double>.Build.DenseOfArray(a3);
            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> result = test.CombineVectors(v1, a, v2);
            Assert.AreEqual(testv, result);
        }
        [TestMethod]
        [Description("Test if CombineVectors works properly part 3")]
        public void CombineVectors_VectorsSizeTest3()
        {
            double[] a1 = { 1, 2, 3, 4, 5 };
            double[] a2 = { 6, 7, 8 };
            double[] a3 = { 1, 2, 3, 4, 6, 7, 8 };
            Vector<double> v01 = Vector<double>.Build.DenseOfArray(a1);
            Vector<double> v2 = Vector<double>.Build.DenseOfArray(a2);
            Vector<double> v1 = v01.SubVector(0, 4);
            Vector<double> testv = Vector<double>.Build.DenseOfArray(a3);
            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> result = test.CombineVectors(v1, v2);
            Assert.AreEqual(testv, result);
        }

        //Interpolate
        [TestMethod]
        [Description("Test if Interpolate does not cut correct values")]
        public void Interpolate_AccuracyTest()
        {
            double[] a1 = {650, 700, 600, 450, 800, 1300 };
            Vector<double> v1 = Vector<double>.Build.DenseOfArray(a1);
            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> result = test.Interpolate(v1);
            Assert.AreEqual(v1, result);
        }
        [TestMethod]
        [Description("Test if Interpolate interpolates too high values")]
        public void Interpolate_TooHighTest()
        {
            double[] a1 = { 650, 700, 1450, 450, 800, 1300 };
            Vector<double> v1 = Vector<double>.Build.DenseOfArray(a1);
            double[] a2 = { 650, 700, 575, 450, 800, 1300 };
            Vector<double> expected = Vector<double>.Build.DenseOfArray(a2);

            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> result = test.Interpolate(v1);
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        [Description("Test if Interpolate interpolates too high value when it is at the end")]
        public void Interpolate_TooHighEndTest()
        {
            double[] a1 = { 650, 700, 575, 450, 800, 1300 , 1450};
            Vector<double> v1 = Vector<double>.Build.DenseOfArray(a1);
            double[] a2 = { 650, 700, 575, 450, 800, 1300, 1300 };
            Vector<double> expected = Vector<double>.Build.DenseOfArray(a2);

            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> result = test.Interpolate(v1);
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        [Description("Test if Interpolate interpolates too high value when it is at the beginning")]
        public void Interpolate_TooHighBeginTest()
        {
            double[] a1 = { 1450, 650, 700, 575, 450, 800, 1300 };
            Vector<double> v1 = Vector<double>.Build.DenseOfArray(a1);
            double[] a2 = { 650, 650, 700, 575, 450, 800, 1300 };
            Vector<double> expected = Vector<double>.Build.DenseOfArray(a2);

            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> result = test.Interpolate(v1);
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        [Description("Test if Interpolate cuts too low values")]
        public void Interpolate_TooLowTest()
        {
            double[] a1 = { 650, 700, 575, 300, 800, 1300 };
            Vector<double> v1 = Vector<double>.Build.DenseOfArray(a1);
            double[] a2 = { 650, 700, 575, 800, 1300 };
            Vector<double> expected = Vector<double>.Build.DenseOfArray(a2);

            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> result = test.Interpolate(v1);
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        [Description("Test if Interpolate cuts too low value when it is at the end")]
        public void Interpolate_TooLowEndTest()
        {
            double[] a1 = { 650, 700, 575, 1300, 800, 300 };
            Vector<double> v1 = Vector<double>.Build.DenseOfArray(a1);
            double[] a2 = { 650, 700, 575, 1300, 800, 800 };
            Vector<double> expected = Vector<double>.Build.DenseOfArray(a2);

            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> result = test.Interpolate(v1);
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        [Description("Test if Interpolate cuts too low value when it is at the beginning")]
        public void Interpolate_TooLowBeginTest()
        {
            double[] a1 = { 300, 650, 700, 575, 1300, 800 };
            Vector<double> v1 = Vector<double>.Build.DenseOfArray(a1);
            double[] a2 = { 650, 700, 575, 1300, 800};
            Vector<double> expected = Vector<double>.Build.DenseOfArray(a2);

            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> result = test.Interpolate(v1);
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        [Description("Test if Interpolate properly when 2 high values appear")]
        public void Interpolate_TooHigh2Test()
        {
            double[] a1 = { 710, 650, 700, 1500, 550, 1300, 1450, 800 };
            Vector<double> v1 = Vector<double>.Build.DenseOfArray(a1);
            double[] a2 = { 710, 650, 700, 625, 550, 1300, 1050, 800 };
            Vector<double> expected = Vector<double>.Build.DenseOfArray(a2);

            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> result = test.Interpolate(v1);
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        [Description("Test if Interpolate properly when 2 low values appear")]
        public void Interpolate_TooLow2Test()
        {
            double[] a1 = { 656, 300, 650, 700, 575, 200, 1300, 800 };
            Vector<double> v1 = Vector<double>.Build.DenseOfArray(a1);
            double[] a2 = { 656, 650, 700, 575, 1300, 800 };
            Vector<double> expected = Vector<double>.Build.DenseOfArray(a2);

            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> result = test.Interpolate(v1);
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        [Description("Test if Interpolate properly when low and high values appear")]
        public void Interpolate_LowHighTest()
        {
            double[] a1 = { 656, 300, 650, 700, 550, 1500 , 1300, 800 };
            Vector<double> v1 = Vector<double>.Build.DenseOfArray(a1);
            double[] a2 = { 656, 650, 700, 550, 925, 1300, 800 };
            Vector<double> expected = Vector<double>.Build.DenseOfArray(a2);

            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> result = test.Interpolate(v1);
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        [Description("Test if Interpolate properly when high and low values appear")]
        public void Interpolate_HighLowTest()
        {
            double[] a1 = { 650, 1500, 650, 700, 550, 300, 1300, 800 };
            Vector<double> v1 = Vector<double>.Build.DenseOfArray(a1);
            double[] a2 = { 650, 650, 650, 700, 550, 1300, 800 };
            Vector<double> expected = Vector<double>.Build.DenseOfArray(a2);

            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> result = test.Interpolate(v1);
            Assert.AreEqual(expected, result);
        }
    }
}
