using System;
using MathNet.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;


namespace EKG_Unit.Modules.Heart_Cluster
{
    [TestClass]
    public class Heart_Cluster_Alg_Test
    {
        // Testy do wszystkich czterech Coefficients.
        [TestMethod]
        [Description("Test if Malinowska's factor is counted from vector properly - EQUALITY")]
        public void CountMalinowskaFactor1()
        {
            double[] testArray = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            uint fs = 360;
            double expectedResult = 0.6931;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            var testResult = EKG_Project.Modules.Heart_Cluster.Heart_Cluster.Coefficients.MalinowskaFactor(testVector, fs);

            expectedResult = System.Math.Round(expectedResult, 3);
            testResult = System.Math.Round(testResult, 3);

            Assert.AreEqual(testResult, expectedResult);

        }
        
        [TestMethod]
        [Description("Test if Malinowska's factor is counted from vector properly - NOT EQUALITY")]
        public void CountMalinowskaFactor2()
        {
            double[] testArray = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            uint fs = 360;
            double expectedResult = 0.6231;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            //var testAlgs = new EKG_Project.Modules.Heart_Cluster.Heart_Cluster();
            

            var testResult = EKG_Project.Modules.Heart_Cluster.Heart_Cluster.Coefficients.MalinowskaFactor(testVector, fs);


            expectedResult = System.Math.Round(expectedResult, 3);
            testResult = System.Math.Round(testResult, 3);

            Assert.AreNotEqual(testResult, expectedResult);

        }

        [TestMethod]
        [Description("Test if positive / negative ratio of signal's amplitude is counted properly - EQUALITY")]
        public void PnRatio1()
        {
            double[] testArray = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            double expectedResult = 2.4474;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            var testResult = EKG_Project.Modules.Heart_Cluster.Heart_Cluster.Coefficients.PnRatio(testVector);

            expectedResult = System.Math.Round(expectedResult, 3);
            testResult = System.Math.Round(testResult, 3);

            Assert.AreEqual(testResult, expectedResult);

        }

        [TestMethod]
        [Description("Test if positive / negative ratio of signal's amplitude is counted properly - NOT EQUALITY")]
        public void PnRatio2()
        {
            double[] testArray = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            double expectedResult = 3.4474;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            var testResult = EKG_Project.Modules.Heart_Cluster.Heart_Cluster.Coefficients.PnRatio(testVector);

            expectedResult = System.Math.Round(expectedResult, 3);
            testResult = System.Math.Round(testResult, 3);

            Assert.AreNotEqual(expectedResult, testResult);

        }

        [TestMethod]
        [Description("Test if maximum speed in signal to maximum amplitude is counted properly - EQUALITY")]
        public void SpeedAmpRatio1()
        {
            double[] testArray = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            double expectedResult = 1.6667;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            var testResult = EKG_Project.Modules.Heart_Cluster.Heart_Cluster.Coefficients.SpeedAmpRatio(testVector);

            expectedResult = System.Math.Round(expectedResult, 3);
            testResult = System.Math.Round(testResult, 3);

            Assert.AreEqual(expectedResult, testResult);

        }

        [TestMethod]
        [Description("Test if maximum speed in signal to maximum amplitude is counted properly - NOT EQUALITY")]
        public void SpeedAmpRatio2()
        {
            double[] testArray = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            double expectedResult = 1.8667;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            var testResult = EKG_Project.Modules.Heart_Cluster.Heart_Cluster.Coefficients.SpeedAmpRatio(testVector);

            expectedResult = System.Math.Round(expectedResult, 3);
            testResult = System.Math.Round(testResult, 3);

            Assert.AreNotEqual(expectedResult, testResult);

        }

        [TestMethod]
        [Description("Test if % of samples in which the speed exceeds 0.4 of max speed is counted properly - EQUALITY")]
        public void FastSampleCount1()
        {
            double[] testArray = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            double expectedResult = 0.7500;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            var testResult = EKG_Project.Modules.Heart_Cluster.Heart_Cluster.Coefficients.FastSampleCount(testVector);

            expectedResult = System.Math.Round(expectedResult, 3);
            testResult = System.Math.Round(testResult, 3);

            Assert.AreEqual(expectedResult, testResult);

        }

        [TestMethod]
        [Description("Test if % of samples in which the speed exceeds 0.4 of max speed is counted properly - NOT EQUALITY")]
        public void FastSampleCount2()
        {
            double[] testArray = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            double expectedResult = 0.2355;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            var testResult = EKG_Project.Modules.Heart_Cluster.Heart_Cluster.Coefficients.FastSampleCount(testVector);

            expectedResult = System.Math.Round(expectedResult, 3);
            testResult = System.Math.Round(testResult, 3);

            Assert.AreNotEqual(expectedResult, testResult);

        }


        [TestMethod]
        [Description("Test if integral is counted properly - equality")]
        public void IntegrateTest1()
        {
            double[] testArray = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            double expectedResult = 32.75;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            var testResult = EKG_Project.Modules.Heart_Cluster.Heart_Cluster.Coefficients.Integrate(testVector);

            Assert.AreEqual(testResult, expectedResult);

        }

        [TestMethod]
        [Description("Test if integral is counted properly - not equality")]
        public void IntegrateTest2()
        {
            double[] testArray = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            double expectedResult = 31.75;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            var testResult = EKG_Project.Modules.Heart_Cluster.Heart_Cluster.Coefficients.Integrate(testVector);

            Assert.AreNotEqual(testResult, expectedResult);

        }

        [TestMethod]
        [Description("Test if perimeted is counted properly - equality")]
        public void PerimeterTest1()
        {
            double[] testArray = { 1, 2, -3, 4, -5, 6, 7, -1.5, 3.25 };
            uint fs = 360;
            double expectedResult = 47.25;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            var testResult = EKG_Project.Modules.Heart_Cluster.Heart_Cluster.Coefficients.Perimeter(testVector, fs);

            expectedResult = System.Math.Round(expectedResult, 2);
            testResult = System.Math.Round(testResult, 2);


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

            var testResult = EKG_Project.Modules.Heart_Cluster.Heart_Cluster.Coefficients.Perimeter(testVector, fs);

            Assert.AreNotEqual(testResult, expectedResult);

        }

        //testowanie QRS komplexów.
        //testowanie metody klusteryzacji.

    }
}
