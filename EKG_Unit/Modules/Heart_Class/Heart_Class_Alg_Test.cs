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
        [Description("Test if method set correct QRS complex  - equality test")]
        public void OneQrsComplex1()
        {
            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);

            double qrsOnset = 5;
            double qrsEnd = 29;
            double R = 13;
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

            object[] args = { qrsOnset, qrsEnd, R };
            obj.Invoke("OneQrsComplex", args);

            Assert.AreEqual(expectedResult, testAlgs.QrsComplexOne);

        }

      
        [TestMethod]
        [Description("Test if method throws null, when qrsOnser or qrsEnd is -1")]
        [ExpectedException(typeof(NullReferenceException), "Object reference not set to an instance of an object")]
        public void OneQrsComplex2()
        {
            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);

            double qrsOnset = 5;
            double qrsEnd = -1;
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

            object[] args = { qrsOnset, qrsEnd, R };
            obj.Invoke("OneQrsComplex", args);

            Tuple<int, Vector<double>> testResult = testAlgs.CountCoeff(testAlgs.QrsComplexOne, fs);

        }

        [TestMethod]
        [Description("Test if method throws null, when qrsOnser and qrsEnd is -1")]
        [ExpectedException(typeof(NullReferenceException), "Object reference not set to an instance of an object")]
        public void OneQrsComplex3()
        {
            Heart_Class_Alg testAlgs = new Heart_Class_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);

            double qrsOnset = -1;
            double qrsEnd = -1;
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

            object[] args = { qrsOnset, qrsEnd, R };
            obj.Invoke("OneQrsComplex", args);

            Tuple<int, Vector<double>> testResult = testAlgs.CountCoeff(testAlgs.QrsComplexOne, fs);

        }
    }
}
