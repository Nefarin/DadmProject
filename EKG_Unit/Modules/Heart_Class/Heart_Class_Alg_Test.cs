using System;
using EKG_Project.Modules.Heart_Class;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;


namespace EKG_Unit.Modules.Heart_Class
{
    [TestClass]
    public class Heart_Class_Alg_Test
    {
        [TestMethod]
        [Description("Test if method counts an integral in vector properly")]
        public void IntegrateTest1()
        {
            double[] testArray = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            double expectedResult = 32.75;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
      
            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            double testResult = testAlgs.Integrate(testVector);

            Assert.AreEqual(testResult, expectedResult);

        }

        [TestMethod]
        [Description("Test if method counts an integral in vector properly - not equality test")]
        public void IntegrateTest2()
        {
            double[] testArray = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            double expectedResult = 31.75;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            double testResult = testAlgs.Integrate(testVector);
            
            Assert.AreNotEqual(testResult, expectedResult);

        }

        [TestMethod]
        [Description("Test if method counts a perimeter in vector properly")]
        public void PerimeterTest1()
        {
            double[] testArray = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            uint fs = 360;
            double expectedResult = 47.25;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            double testResult = testAlgs.Perimeter(testVector,fs);

            Assert.AreEqual(testResult, expectedResult);

        }

        [TestMethod]
        [Description("Test if method counts a perimeter in vector properly - - not equality test")]
        public void PerimeterTest2()
        {
            double[] testArray = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            uint fs = 360;
            double expectedResult = 47.00;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            double testResult = testAlgs.Perimeter(testVector, fs);

            Assert.AreNotEqual(testResult, expectedResult);

        }

        [TestMethod]
        [Description("Test if method counts a Malinowska's factor from vector properly - equality test")]
        public void CountMalinowskaFactor2()
        {
            double[] testArray = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            uint fs = 360;
            double expectedResult = 0.6231;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            double testResult = testAlgs.CountMalinowskaFactor(testVector, fs);

            expectedResult = System.Math.Round(expectedResult, 3);
            testResult = System.Math.Round(testResult, 3);

            Assert.AreNotEqual(testResult, expectedResult);

        }

        [TestMethod]
        [Description("Test if method counts a Malinowska's factor from vector properly - not equality test")]
        public void CountMalinowskaFactor1()
        {
            double[] testArray = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            uint fs = 360;
            double expectedResult = 0.6931;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            double testResult = testAlgs.CountMalinowskaFactor(testVector, fs);

            expectedResult = System.Math.Round(expectedResult, 3);
            testResult = System.Math.Round(testResult, 3);

            Assert.AreEqual(testResult, expectedResult);

        }

        [TestMethod]
        [Description("Test if method counts the ratio of positive part to negative part of signal's amplitude properly - equality test")]
        public void PnRatio1()
        {
            double[] testArray = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            double expectedResult = 2.4474;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            double testResult = testAlgs.PnRatio(testVector);

            expectedResult = System.Math.Round(expectedResult, 3);
            testResult = System.Math.Round(testResult, 3);

            Assert.AreEqual(testResult, expectedResult);

        }

        [TestMethod]
        [Description("Test if method counts the ratio of positive part to negative part of signal's amplitude properly - not equality test")]
        public void PnRatio2()
        {
            double[] testArray = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            double expectedResult = 3.4474;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            double testResult = testAlgs.PnRatio(testVector);

            expectedResult = System.Math.Round(expectedResult, 3);
            testResult = System.Math.Round(testResult, 3);

            Assert.AreNotEqual(expectedResult, testResult);

        }


        [TestMethod]
        [Description("Test if method counts the ratio of maximum speed in signal to maximum signal's amplitude properly - equality test")]
        public void SpeedAmpRatio1()
        {
            double[] testArray = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            double expectedResult = 1.6667;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            double testResult = testAlgs.SpeedAmpRatio(testVector);

            expectedResult = System.Math.Round(expectedResult, 3);
            testResult = System.Math.Round(testResult, 3);

            Assert.AreEqual(expectedResult, testResult);

        }

        [TestMethod]
        [Description("Test if method counts the ratio of maximum speed in signal to maximum signal's amplitude properly - not equality test")]
        public void SpeedAmpRatio2()
        {
            double[] testArray = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            double expectedResult = 1.8667;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            double testResult = testAlgs.SpeedAmpRatio(testVector);

            expectedResult = System.Math.Round(expectedResult, 3);
            testResult = System.Math.Round(testResult, 3);

            Assert.AreNotEqual(expectedResult, testResult);

        }

        [TestMethod]
        [Description("Test if method counts the percentage of samples in which the speed exceeds the 0.4 of maximum speed properly - equality test")]
        public void FastSampleCount1()
        {
            double[] testArray = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            double expectedResult = 0.7500;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            double testResult = testAlgs.FastSampleCount(testVector);

            expectedResult = System.Math.Round(expectedResult, 3);
            testResult = System.Math.Round(testResult, 3);

            Assert.AreEqual(expectedResult, testResult);

        }

        [TestMethod]
        [Description("Test if method counts the percentage of samples in which the speed exceeds the 0.4 of maximum speed properly - not equality test")]
        public void FastSampleCount2()
        {
            double[] testArray = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            double expectedResult = 0.2355;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            double testResult = testAlgs.FastSampleCount(testVector);

            expectedResult = System.Math.Round(expectedResult, 3);
            testResult = System.Math.Round(testResult, 3);

            Assert.AreNotEqual(expectedResult, testResult);

        }

        [TestMethod]
        [Description("Test if method calculates the time of QRS complex properly - equality test")]
        public void QrsDuration1()
        {
            double[] testArray = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            uint fs = 360;
            double expectedResult = 0.0250;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            double testResult = testAlgs.QrsDuration(testVector, fs);

            expectedResult = System.Math.Round(expectedResult, 3);
            testResult = System.Math.Round(testResult, 3);

            Assert.AreEqual(expectedResult, testResult);

        }

        [TestMethod]
        [Description("Test if method calculates the time of QRS complex properly - not equality test")]
        public void QrsDuration2()
        {
            double[] testArray = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            uint fs = 360;
            double expectedResult = 0.1250;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            double testResult = testAlgs.QrsDuration(testVector, fs);

            expectedResult = System.Math.Round(expectedResult, 3);
            testResult = System.Math.Round(testResult, 3);

            Assert.AreNotEqual(expectedResult, testResult);

        }




    }
}
