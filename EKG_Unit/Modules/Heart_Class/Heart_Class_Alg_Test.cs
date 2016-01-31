using System;
using EKG_Project.Modules.Heart_Class;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.IO;
using System.Collections.Generic;


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

        [TestMethod]
        [Description("Test if method calculates the time of QRS complex properly - not equality test")]
        public void CountCoeff1()
        {
            double[] qRSComplex =
            {
                -0.13126, -0.13644, -0.16032, -0.20561, -0.26753, -0.33335, -0.38369, -0.39605,
                -0.35046, -0.236, -0.055888, 0.17117, 0.41375, 0.63385, 0.79486, 0.86883, 0.84255, 0.72056, 0.52455,
                0.28928, 0.055008, -0.14166, -0.27553, -0.33743, -0.33399, -0.28368, -0.21097, -0.1402, -0.090148,
                -0.070429, -0.080307, -0.11024, -0.1458
            };
            Vector<double> testQrsComplex = Vector<double>.Build.DenseOfArray(qRSComplex);
            uint fs = 360;
            int R = 133;

            Tuple<int, Vector<double>> testTuple;
            testTuple = new Tuple<int, Vector<double>>(R, testQrsComplex);

            Vector<double> expectedCoefficients;
            expectedCoefficients = Vector<double>.Build.Dense(4);
            expectedCoefficients[0] = 3.3063;
            expectedCoefficients[1] = 1.0897;
            expectedCoefficients[2] = 0.0793;
            expectedCoefficients[3] = 0.3750;

            Tuple<int, Vector<double>> expectedResult = new Tuple<int, Vector<double>>(R, expectedCoefficients );

            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            Tuple<int, Vector<double>> testResult = testAlgs.CountCoeff(testTuple, fs);


            for (int i = 0; i < expectedResult.Item2.Count; i++)
            {
                expectedResult.Item2[i] = System.Math.Round(expectedResult.Item2[i], 3);
            }

            for (int i = 0; i < testResult.Item2.Count; i++)
            {
                testResult.Item2[i] = System.Math.Round(testResult.Item2[i], 3);
            }
            
            Assert.AreNotEqual(expectedResult, testResult);

        }


        [TestMethod]
        [Description("Test if method calculates the distances Q-R and R-S properly - equality test")]
        public void DistancesFromR1()
        {
            
            uint fs = 360;
            int expectedNumberOfSamplesQR = 23;
            int expectedNumberOfSamplesRS = 34;

            Tuple<int, int> expectedResult = new Tuple<int, int>(expectedNumberOfSamplesQR, expectedNumberOfSamplesRS);
           
            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            Tuple<int, int> testResult = testAlgs.DistancesFromR(fs);


            Assert.AreEqual(expectedResult, testResult);

        }

        [TestMethod]
        [Description("Test if method set correct QRS complex  - equality test")]
        public void OneQrsComplex1()
        {
            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);

            int qrsOnset = 5;
            int qrsEnd = 29;
            double R = 13;
            uint fs = 360;
            double[] testArray =
            {
                -0.13126, -0.13644, -0.16032, -0.20561, -0.26753, -0.33335, -0.38369, -0.39605,
                -0.35046, -0.236, -0.055888, 0.17117, 0.41375, 0.63385, 0.79486, 0.86883, 0.84255, 0.72056, 0.52455,
                0.28928, 0.055008, -0.14166, -0.27553, -0.33743, -0.33399, -0.28368, -0.21097, -0.1402, -0.090148,
                -0.070429, -0.080307, -0.11024, -0.1458
            };
            Vector<double> exampleSignal = Vector<double>.Build.DenseOfArray(testArray);
            testAlgs.Signal = exampleSignal;

            double[] expectedArray =
            {
                -0.33335, -0.38369, -0.39605, -0.35046, -0.236, -0.055888, 0.17117, 0.41375, 0.63385, 0.79486, 0.86883,
                0.84255, 0.72056, 0.52455, 0.28928, 0.055008, -0.14166, -0.27553, -0.33743, -0.33399, -0.28368, -0.21097,
                -0.1402, -0.090148, -0.070429
            };

            Vector<double> expectedVector = Vector<double>.Build.DenseOfArray(expectedArray);
            Tuple < int, Vector < double >> expectedResult = new Tuple<int, Vector<double>>((int)R, expectedVector);

            object[] args = { qrsOnset, qrsEnd, R, fs };
            obj.Invoke("OneQrsComplex", args);

            Assert.AreEqual(expectedResult, testAlgs.QrsComplexOne);

        }

        [TestMethod]
        [Description("Test if method set arbitrary values, when qrsOnset or qrsEnd is -1")]
        public void OneQrsComplex2()
        {
            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);

            int qrsOnset = 15;
            int qrsEnd = -1;
            double R = 30;
            uint fs = 360;
            double[] testArray =
            {
                -0.086264, -0.096522,   -0.099909,  -0.096139,   -0.086465,   -0.073789,   -0.062517,   -0.057074,   -0.060127,   -0.071301,   -0.086807,   -0.10118,    -0.10969,    -0.10975,    -0.10166,    -0.088353,
                -0.074281,   -0.064497,   -0.063391,   -0.072903,   -0.091486,   -0.1141, -0.13362, -0.14414,    -0.14401,    -0.13699,    -0.13126,    -0.13644,    -0.16032,    -0.20561,    -0.26753,    -0.33335,
                -0.38369,    -0.39605,    -0.35046,    -0.236,  -0.055888,   0.17117, 0.41375, 0.63385, 0.79486, 0.86883, 0.84255, 0.72056, 0.52455, 0.28928, 0.055008,    -0.14166,    -0.27553,    -0.33743,    -0.33399,
                -0.28368,    -0.21097,    -0.1402, -0.090148,   -0.070429,   -0.080307,   -0.11024,    -0.1458, -0.17298,    -0.18275,    -0.17326,    -0.14924,    -0.11914,    -0.091773,   -0.073821,   -0.068456,
                -0.074897,   -0.088698,   -0.10303,    -0.1111
            };

            Vector<double> exampleSignal = Vector<double>.Build.DenseOfArray(testArray);
            testAlgs.Signal = exampleSignal;
            
            double[] expectedArray =
            {   -0.057074,
                -0.060127, -0.071301, -0.086807, -0.10118, -0.10969, -0.10975, -0.10166, -0.088353,
                -0.074281, -0.064497, -0.063391, -0.072903, -0.091486, -0.1141, -0.13362, -0.14414,
                -0.14401, -0.13699, -0.13126, -0.13644, -0.16032, -0.20561, -0.26753, -0.33335,
                -0.38369, -0.39605, -0.35046, -0.236, -0.055888, 0.17117, 0.41375, 0.63385, 0.79486,
                0.86883, 0.84255, 0.72056, 0.52455, 0.28928, 0.055008, -0.14166, -0.27553, -0.33743, -0.33399,
                -0.28368, -0.21097, -0.1402, -0.090148, -0.070429, -0.080307, -0.11024, -0.1458,
                -0.17298, -0.18275, -0.17326, -0.14924, -0.11914, -0.091773
            };

            Vector<double> expectedVector = Vector<double>.Build.DenseOfArray(expectedArray);
            Tuple<int, Vector<double>> expectedResult = new Tuple<int, Vector<double>>((int)R, expectedVector);

            object[] args = { qrsOnset, qrsEnd, R, fs };
            obj.Invoke("OneQrsComplex", args);

            Assert.AreEqual(expectedResult, testAlgs.QrsComplexOne);

        }

        [TestMethod]
        [Description("Test if method set arbitrary values, when qrsOnser and qrsEnd is -1")]
        public void OneQrsComplex3()
        {
            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);

            int qrsOnset = -1;
            int qrsEnd = -1;
            double R = 30;
            uint fs = 360;
            double[] testArray =
            {
                -0.086264, -0.096522,   -0.099909,  -0.096139,   -0.086465,   -0.073789,   -0.062517,   -0.057074,   -0.060127,   -0.071301,   -0.086807,   -0.10118,    -0.10969,    -0.10975,    -0.10166,    -0.088353,
                -0.074281,   -0.064497,   -0.063391,   -0.072903,   -0.091486,   -0.1141, -0.13362, -0.14414,    -0.14401,    -0.13699,    -0.13126,    -0.13644,    -0.16032,    -0.20561,    -0.26753,    -0.33335,
                -0.38369,    -0.39605,    -0.35046,    -0.236,  -0.055888,   0.17117, 0.41375, 0.63385, 0.79486, 0.86883, 0.84255, 0.72056, 0.52455, 0.28928, 0.055008,    -0.14166,    -0.27553,    -0.33743,    -0.33399,
                -0.28368,    -0.21097,    -0.1402, -0.090148,   -0.070429,   -0.080307,   -0.11024,    -0.1458, -0.17298,    -0.18275,    -0.17326,    -0.14924,    -0.11914,    -0.091773,   -0.073821,   -0.068456,
                -0.074897,   -0.088698,   -0.10303,    -0.1111
            };

            Vector<double> exampleSignal = Vector<double>.Build.DenseOfArray(testArray);
            testAlgs.Signal = exampleSignal;

            double[] expectedArray =
            {   -0.057074,
                -0.060127, -0.071301, -0.086807, -0.10118, -0.10969, -0.10975, -0.10166, -0.088353,
                -0.074281, -0.064497, -0.063391, -0.072903, -0.091486, -0.1141, -0.13362, -0.14414,
                -0.14401, -0.13699, -0.13126, -0.13644, -0.16032, -0.20561, -0.26753, -0.33335,
                -0.38369, -0.39605, -0.35046, -0.236, -0.055888, 0.17117, 0.41375, 0.63385, 0.79486,
                0.86883, 0.84255, 0.72056, 0.52455, 0.28928, 0.055008, -0.14166, -0.27553, -0.33743, -0.33399,
                -0.28368, -0.21097, -0.1402, -0.090148, -0.070429, -0.080307, -0.11024, -0.1458,
                -0.17298, -0.18275, -0.17326, -0.14924, -0.11914, -0.091773
            };

            Vector<double> expectedVector = Vector<double>.Build.DenseOfArray(expectedArray);
            Tuple<int, Vector<double>> expectedResult = new Tuple<int, Vector<double>>((int)R, expectedVector);

            object[] args = { qrsOnset, qrsEnd, R, fs };
            obj.Invoke("OneQrsComplex", args);

            Assert.AreEqual(expectedResult, testAlgs.QrsComplexOne);

        }

        [TestMethod]
        [Description("Test if method set arbitrary values, when both qrsOnset or qrsEnd are larger than arbitrary. So when something is not okay in Waves ")]
        public void OneQrsComplex4()
        {
            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);

            int qrsOnset = 5;
            int qrsEnd = 70;
            double R = 30;
            uint fs = 360;
            double[] testArray =
            {
                -0.086264, -0.096522,   -0.099909,  -0.096139,   -0.086465,   -0.073789,   -0.062517,   -0.057074,   -0.060127,
                -0.071301,   -0.086807,   -0.10118,    -0.10969,    -0.10975,    -0.10166,    -0.088353,
                -0.074281,   -0.064497,   -0.063391,   -0.072903,   -0.091486,   -0.1141, -0.13362, -0.14414,
                -0.14401,    -0.13699,    -0.13126,    -0.13644,    -0.16032,    -0.20561,    -0.26753,    -0.33335,
                -0.38369,    -0.39605,    -0.35046,    -0.236,  -0.055888,   0.17117, 0.41375, 0.63385, 0.79486, 0.86883,
                0.84255, 0.72056, 0.52455, 0.28928, 0.055008,    -0.14166,    -0.27553,    -0.33743,    -0.33399,
                -0.28368,    -0.21097,    -0.1402, -0.090148,   -0.070429,   -0.080307,   -0.11024,    -0.1458,
                -0.17298,    -0.18275,    -0.17326,    -0.14924,    -0.11914,    -0.091773,   -0.073821,   -0.068456,
                -0.074897,   -0.088698,   -0.10303,    -0.1111
            };

            Vector<double> exampleSignal = Vector<double>.Build.DenseOfArray(testArray);
            testAlgs.Signal = exampleSignal;

            double[] expectedArray =
            {   -0.057074,
                -0.060127, -0.071301, -0.086807, -0.10118, -0.10969, -0.10975, -0.10166, -0.088353,
                -0.074281, -0.064497, -0.063391, -0.072903, -0.091486, -0.1141, -0.13362, -0.14414,
                -0.14401, -0.13699, -0.13126, -0.13644, -0.16032, -0.20561, -0.26753, -0.33335,
                -0.38369, -0.39605, -0.35046, -0.236, -0.055888, 0.17117, 0.41375, 0.63385, 0.79486,
                0.86883, 0.84255, 0.72056, 0.52455, 0.28928, 0.055008, -0.14166, -0.27553, -0.33743, -0.33399,
                -0.28368, -0.21097, -0.1402, -0.090148, -0.070429, -0.080307, -0.11024, -0.1458,
                -0.17298, -0.18275, -0.17326, -0.14924, -0.11914, -0.091773
            };

            Vector<double> expectedVector = Vector<double>.Build.DenseOfArray(expectedArray);
            Tuple<int, Vector<double>> expectedResult = new Tuple<int, Vector<double>>((int)R, expectedVector);

            object[] args = { qrsOnset, qrsEnd, R, fs };
            obj.Invoke("OneQrsComplex", args);

            Assert.AreEqual(expectedResult, testAlgs.QrsComplexOne);

        }

        [TestMethod]
        [Description("Test if method set arbitrary value for qrsEnd  if it's larger than arbitrary. So when something is not okay in Waves ")]
        public void OneQrsComplex5()
        {
            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);

            int qrsOnset = 10;
            int qrsEnd = 70;
            double R = 30;
            uint fs = 360;
            double[] testArray =
            {
                -0.086264, -0.096522,   -0.099909,  -0.096139,   -0.086465,   -0.073789,   -0.062517,   -0.057074,   -0.060127,
                -0.071301,   -0.086807,   -0.10118,    -0.10969,    -0.10975,    -0.10166,    -0.088353,
                -0.074281,   -0.064497,   -0.063391,   -0.072903,   -0.091486,   -0.1141, -0.13362, -0.14414,
                -0.14401,    -0.13699,    -0.13126,    -0.13644,    -0.16032,    -0.20561,    -0.26753,    -0.33335,
                -0.38369,    -0.39605,    -0.35046,    -0.236,  -0.055888,   0.17117, 0.41375, 0.63385, 0.79486, 0.86883,
                0.84255, 0.72056, 0.52455, 0.28928, 0.055008,    -0.14166,    -0.27553,    -0.33743,    -0.33399,
                -0.28368,    -0.21097,    -0.1402, -0.090148,   -0.070429,   -0.080307,   -0.11024,    -0.1458,
                -0.17298,    -0.18275,    -0.17326,    -0.14924,    -0.11914,    -0.091773,   -0.073821,   -0.068456,
                -0.074897,   -0.088698,   -0.10303,    -0.1111
            };

            Vector<double> exampleSignal = Vector<double>.Build.DenseOfArray(testArray);
            testAlgs.Signal = exampleSignal;

            double[] expectedArray =
            {   -0.086807,   -0.10118,    -0.10969,    -0.10975,    -0.10166,    -0.088353,
                -0.074281,   -0.064497,   -0.063391,   -0.072903,   -0.091486,   -0.1141, -0.13362, -0.14414,
                -0.14401,    -0.13699,    -0.13126,    -0.13644,    -0.16032,    -0.20561,    -0.26753, -0.33335,
                -0.38369, -0.39605, -0.35046, -0.236, -0.055888, 0.17117, 0.41375, 0.63385, 0.79486,
                0.86883, 0.84255, 0.72056, 0.52455, 0.28928, 0.055008, -0.14166, -0.27553, -0.33743, -0.33399,
                -0.28368, -0.21097, -0.1402, -0.090148, -0.070429, -0.080307, -0.11024, -0.1458,
                -0.17298, -0.18275, -0.17326, -0.14924, -0.11914, -0.091773
            };

            Vector<double> expectedVector = Vector<double>.Build.DenseOfArray(expectedArray);
            Tuple<int, Vector<double>> expectedResult = new Tuple<int, Vector<double>>((int)R, expectedVector);

            object[] args = { qrsOnset, qrsEnd, R, fs };
            obj.Invoke("OneQrsComplex", args);

            Assert.AreEqual(expectedResult, testAlgs.QrsComplexOne);

        }

        [TestMethod]
        [Description("Test if method set arbitrary values, when both qrsOnset or qrsEnd are larger than arbitrary. So when something is not okay in Waves ")]
        public void OneQrsComplex6()
        {
            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);

            int qrsOnset = 5;
            int qrsEnd = 50;
            double R = 30;
            uint fs = 360;
            double[] testArray =
            {
                -0.086264, -0.096522,   -0.099909,  -0.096139,   -0.086465,   -0.073789,   -0.062517,   -0.057074,   -0.060127,
                -0.071301,   -0.086807,   -0.10118,    -0.10969,    -0.10975,    -0.10166,    -0.088353,
                -0.074281,   -0.064497,   -0.063391,   -0.072903,   -0.091486,   -0.1141, -0.13362, -0.14414,
                -0.14401,    -0.13699,    -0.13126,    -0.13644,    -0.16032,    -0.20561,    -0.26753,    -0.33335,
                -0.38369,    -0.39605,    -0.35046,    -0.236,  -0.055888,   0.17117, 0.41375, 0.63385, 0.79486, 0.86883,
                0.84255, 0.72056, 0.52455, 0.28928, 0.055008,    -0.14166,    -0.27553,    -0.33743,    -0.33399,
                -0.28368,    -0.21097,    -0.1402, -0.090148,   -0.070429,   -0.080307,   -0.11024,    -0.1458,
                -0.17298,    -0.18275,    -0.17326,    -0.14924,    -0.11914,    -0.091773,   -0.073821,   -0.068456,
                -0.074897,   -0.088698,   -0.10303,    -0.1111
            };

            Vector<double> exampleSignal = Vector<double>.Build.DenseOfArray(testArray);
            testAlgs.Signal = exampleSignal;

            double[] expectedArray =
            {   -0.057074,
                -0.060127, -0.071301, -0.086807, -0.10118, -0.10969, -0.10975, -0.10166, -0.088353,
                -0.074281, -0.064497, -0.063391, -0.072903, -0.091486, -0.1141, -0.13362, -0.14414,
                -0.14401, -0.13699, -0.13126, -0.13644, -0.16032, -0.20561, -0.26753, -0.33335,
                -0.38369, -0.39605, -0.35046, -0.236, -0.055888, 0.17117, 0.41375, 0.63385, 0.79486,
                0.86883, 0.84255, 0.72056, 0.52455, 0.28928, 0.055008, -0.14166, -0.27553, -0.33743, -0.33399
 
            };

            Vector<double> expectedVector = Vector<double>.Build.DenseOfArray(expectedArray);
            Tuple<int, Vector<double>> expectedResult = new Tuple<int, Vector<double>>((int)R, expectedVector);

            object[] args = { qrsOnset, qrsEnd, R, fs };
            obj.Invoke("OneQrsComplex", args);

            Assert.AreEqual(expectedResult, testAlgs.QrsComplexOne);

        }

        [TestMethod]
        [Description("Test if method counts the distance between two vectors properly - equality test")]
        public void GetDistance1()
        {
            double[] testArray1 = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            double[] testArray2 = {2, 8, 2.5, -2, 4, 12, 3, -5, 9.3};
            double expectedResult = 16.8850;

            Vector<double> testVector1 = Vector<double>.Build.DenseOfArray(testArray1);
            Vector<double> testVector2 = Vector<double>.Build.DenseOfArray(testArray2);

            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            double testResult = testAlgs.GetDistance(testVector1, testVector2);

            expectedResult = System.Math.Round(expectedResult, 3);
            testResult = System.Math.Round(testResult, 3);

            Assert.AreEqual(expectedResult, testResult);

        }

        [TestMethod]
        [Description("Test if method throw exception (index out of range), when vectors are not the same length")]
        [ExpectedException(typeof(IndexOutOfRangeException), "Index goes beyond the array")]
        public void GetDistance2()
        {
            double[] testArray1 = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            double[] testArray2 = { 2, 8, 2.5, -2, 4, 12, 3, -5 };
            double expectedResult = 16.8850;

            Vector<double> testVector1 = Vector<double>.Build.DenseOfArray(testArray1);
            Vector<double> testVector2 = Vector<double>.Build.DenseOfArray(testArray2);

            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            double testResult = testAlgs.GetDistance(testVector1, testVector2);

            expectedResult = System.Math.Round(expectedResult, 3);
            testResult = System.Math.Round(testResult, 3);

            Assert.AreEqual(expectedResult, testResult);

        }

        [TestMethod]
        [Description("Test if method counts classes properly  - equality test")]
        public void KnnTest1()
        {
            int R = 13;
            int K = 3;
            Vector<double> testCoefficients;
            testCoefficients = Vector<double>.Build.Dense(4);
            testCoefficients[0] = 3.3063;
            testCoefficients[1] = 1.0897;
            testCoefficients[2] = 0.0793;
            testCoefficients[3] = 0.3750;

            Tuple<int, Vector<double>> testDataTuple = new Tuple<int, Vector<double>>(R, testCoefficients);
            int expectedClass = 1;
            Tuple<int, int> expectedResult = new Tuple<int, int>(R, expectedClass);

            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            
            DebugECGPath loader = new DebugECGPath();
            List<Vector<double>> trainDataList = testAlgs.loadFile(System.IO.Path.Combine(loader.getTempPath(), "train_d.txt"));
            List<Vector<double>> trainClassList = testAlgs.loadFile(System.IO.Path.Combine(loader.getTempPath(), "train_d_label.txt"));

                int oneClassElement;
                List<int> trainClass;
                trainClass = new List<int>();
                foreach (var item in trainClassList)
                {
                    foreach (var element in item)
                    {
                        oneClassElement = (int)element;
                        trainClass.Add(oneClassElement);
                    }

                }

            Tuple<int, int> testResult = testAlgs.TestKnn(trainDataList, testDataTuple, trainClass, K);

            Assert.AreEqual(expectedResult, testResult);

        }

        [TestMethod]
        [Description("Test if method counts classes properly  - not equality test. Malinowska factor for V - beat")]
        public void KnnTest2()
        {
            int R = 13;
            int K = 3;
            Vector<double> testCoefficients;
            testCoefficients = Vector<double>.Build.Dense(4);
            testCoefficients[0] = 9.3063;
            testCoefficients[1] = 1.0897;
            testCoefficients[2] = 0.0793;
            testCoefficients[3] = 0.3750;

            Tuple<int, Vector<double>> testDataTuple = new Tuple<int, Vector<double>>(R, testCoefficients);
            int expectedClass = 1;
            Tuple<int, int> expectedResult = new Tuple<int, int>(R, expectedClass);

            Heart_Class_Alg testAlgs = new Heart_Class_Alg();

            DebugECGPath loader = new DebugECGPath();
            List<Vector<double>> trainDataList = testAlgs.loadFile(System.IO.Path.Combine(loader.getTempPath(), "train_d.txt"));
            List<Vector<double>> trainClassList = testAlgs.loadFile(System.IO.Path.Combine(loader.getTempPath(), "train_d_label.txt"));

            int oneClassElement;
            List<int> trainClass;
            trainClass = new List<int>();
            foreach (var item in trainClassList)
            {
                foreach (var element in item)
                {
                    oneClassElement = (int)element;
                    trainClass.Add(oneClassElement);
                }

            }

            Tuple<int, int> testResult = testAlgs.TestKnn(trainDataList, testDataTuple, trainClass, K);

            Assert.AreNotEqual(expectedResult, testResult);

        }

        [TestMethod]
        [Description("Test if method performs whole classification correctly  - equality test")]
        public void Classification1()
        {
            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);

            int qrsOnset = 5;
            int qrsEnd = 29;
            double R = 13;
            int expectedClass = 1;
            uint fs = 360;
            
            double[] testArray =
            {
                -0.13126, -0.13644, -0.16032, -0.20561, -0.26753, -0.33335, -0.38369, -0.39605,
                -0.35046, -0.236, -0.055888, 0.17117, 0.41375, 0.63385, 0.79486, 0.86883, 0.84255, 0.72056, 0.52455,
                0.28928, 0.055008, -0.14166, -0.27553, -0.33743, -0.33399, -0.28368, -0.21097, -0.1402, -0.090148,
                -0.070429, -0.080307, -0.11024, -0.1458
            };
            Vector<double> exampleSignal = Vector<double>.Build.DenseOfArray(testArray);
            Tuple<int, int> expectedResult = new Tuple<int, int>((int)R, expectedClass);
            
            object[] args = { exampleSignal, qrsOnset, qrsEnd, R, fs };
            obj.Invoke("Classification", args);

            Assert.AreEqual(expectedResult, testAlgs.ClassificationResultOne);

        }
    }
}
