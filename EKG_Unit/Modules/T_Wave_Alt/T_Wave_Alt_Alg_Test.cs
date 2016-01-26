using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using EKG_Project.Modules.T_Wave_Alt;
using MathNet.Numerics.LinearAlgebra;


namespace EKG_Unit.Modules.T_Wave_Alt
{
    [TestClass]
    public class T_Wave_Alt_Alg_Test
    {
        [TestMethod]
        [Description("Tests if T-wave array is built properly")]
        public void buildTWavesArrayEqualTest()
        {
            // Init test here

            double[] testArray = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            double[] resultArray1 = { 2, 3, 4 };
            double[] resultArray2 = { 7, 8, 9 };


            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            List<int> testTEndList = new List<int>();
            testTEndList.Add(4);
            testTEndList.Add(9);
            Vector<double> resultVector1 = Vector<double>.Build.DenseOfArray(resultArray1);
            Vector<double> resultVector2 = Vector<double>.Build.DenseOfArray(resultArray2);
            List<Vector<double>> resultList = new List<Vector<double>>();
            resultList.Add(resultVector1);
            resultList.Add(resultVector2);

            T_Wave_Alt_Alg testAlgs = new T_Wave_Alt_Alg();

            // Process test here

            List<Vector<double>> desiredList = testAlgs.buildTWavesArray(testVector, testTEndList);

            // Assert results

            Assert.AreEqual(desiredList[0], resultVector1);
            Assert.AreEqual(desiredList[1], resultVector2);


        }

        [TestMethod]
        [Description("Tests if T-wave array is built wrong (when needed)")]
        public void buildTWavesArrayNotEqualTest()
        {
            // Init test here

            double[] testArray = { 1, 2, 1, 4, 5, 6, 7, 3, 9, 10 };
            double[] resultArray1 = { 2, 3, 4 };
            double[] resultArray2 = { 7, 8, 9 };


            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            List<int> testTEndList = new List<int>();
            testTEndList.Add(4);
            testTEndList.Add(9);
            Vector<double> resultVector1 = Vector<double>.Build.DenseOfArray(resultArray1);
            Vector<double> resultVector2 = Vector<double>.Build.DenseOfArray(resultArray2);
            List<Vector<double>> resultList = new List<Vector<double>>();
            resultList.Add(resultVector1);
            resultList.Add(resultVector2);

            T_Wave_Alt_Alg testAlgs = new T_Wave_Alt_Alg();

            // Process test here

            List<Vector<double>> desiredList = testAlgs.buildTWavesArray(testVector, testTEndList);

            // Assert results

            Assert.AreNotEqual(desiredList[0], resultVector1);
            Assert.AreNotEqual(desiredList[1], resultVector2);

        }

        [TestMethod]
        [Description("Tests if median is calculated properly (for even T-waves amount)")]
        public void calculateMedianTWaveEvenTest()
        {
            // Init test here
            
            double[] testArray1 = { 2, 5 };
            double[] testArray2 = { 3, 6 };
            double[] testArray3 = { 4, 7 };
            double[] testArray4 = { 5, 8 };

            double[] testMedian = { 3.5, 6.5 };

            Vector<double> testVector1 = Vector<double>.Build.DenseOfArray(testArray1);
            Vector<double> testVector2 = Vector<double>.Build.DenseOfArray(testArray2);
            Vector<double> testVector3 = Vector<double>.Build.DenseOfArray(testArray3);
            Vector<double> testVector4 = Vector<double>.Build.DenseOfArray(testArray4);
            Vector<double> testMedianVec = Vector<double>.Build.DenseOfArray(testMedian);
            List<Vector<double>> testList = new List<Vector<double>>();
            testList.Add(testVector1);
            testList.Add(testVector2);
            testList.Add(testVector3);
            testList.Add(testVector4);

            T_Wave_Alt_Alg testAlgs = new T_Wave_Alt_Alg();

            // Process test here

            Vector<double> desiredMedian = testAlgs.calculateMedianTWave(testList);

            // Assert results

            Assert.AreEqual(testMedianVec, desiredMedian);
        }

        [TestMethod]
        [Description("Tests if median is calculated properly (for odd T-waves amount)")]
        public void calculateMedianTWaveOddTest()
        {
            // Init test here

            double[] testArray1 = { 2, 5 };
            double[] testArray2 = { 3, 5.5 };
            double[] testArray3 = { 3, 7 };

            double[] testMedian = { 3, 5.5 };

            Vector<double> testVector1 = Vector<double>.Build.DenseOfArray(testArray1);
            Vector<double> testVector2 = Vector<double>.Build.DenseOfArray(testArray2);
            Vector<double> testVector3 = Vector<double>.Build.DenseOfArray(testArray3);
            Vector<double> testMedianVec = Vector<double>.Build.DenseOfArray(testMedian);
            List<Vector<double>> testList = new List<Vector<double>>();
            testList.Add(testVector1);
            testList.Add(testVector2);
            testList.Add(testVector3);

            T_Wave_Alt_Alg testAlgs = new T_Wave_Alt_Alg();

            // Process test here

            Vector<double> desiredMedian = testAlgs.calculateMedianTWave(testList);

            // Assert results

            Assert.AreEqual(testMedianVec, desiredMedian);
        }

        [TestMethod]
        [Description("Tests if ACI are calculated properly")]
        public void calculateACITest()
        {
            // Init test here
            // TODO: check for values other than 1

            //double[] testArray1 = { 2, 5, 6 };
            double[] testArray1 = { 3, 6, 4 };
            double[] testArray2 = { 3, 6, 4 };
            double[] testArray3 = { 3, 6, 4 };
            //double[] testArray3 = { 3, 7, 4 };

            double[] testMedian = { 3, 6, 4 };

            //double[] testACIArray = { 0.983, 1, 1,098 };
            double[] testACIArray = { 1, 1, 1 };

            Vector<double> testVector1 = Vector<double>.Build.DenseOfArray(testArray1);
            Vector<double> testVector2 = Vector<double>.Build.DenseOfArray(testArray2);
            Vector<double> testVector3 = Vector<double>.Build.DenseOfArray(testArray3);
            Vector<double> testMedianVec = Vector<double>.Build.DenseOfArray(testMedian);
            Vector<double> testACI = Vector<double>.Build.DenseOfArray(testACIArray);
            List<Vector<double>> testList = new List<Vector<double>>();
            testList.Add(testVector1);
            testList.Add(testVector2);
            testList.Add(testVector3);

            T_Wave_Alt_Alg testAlgs = new T_Wave_Alt_Alg();

            // Process test here

            Vector<double> receivedACI = testAlgs.calculateACI(testList, testMedianVec);

            // Assert results

            Assert.AreEqual(testACI, receivedACI);
        }

        [TestMethod]
        [Description("Tests if fluctuations are detected properly (with multiple 1s in a row)")]
        public void findFluctuationsTest1()
        {
            // Init test here

            double[] testACIArray1 = { 0.34, 0.59, 1.4, -0.56, 0, 0, 1, 1, 1.23, 1.85, 0.83, 1.2 };
            Vector<double> testACIVector1 = Vector<double>.Build.DenseOfArray(testACIArray1);
   
            List<int> testFlucList1 = new List<int>();
            testFlucList1.Add(0);
            testFlucList1.Add(0);
            testFlucList1.Add(1);
            testFlucList1.Add(1);
            testFlucList1.Add(0);
            testFlucList1.Add(0);
            testFlucList1.Add(0);
            testFlucList1.Add(0);
            testFlucList1.Add(1);
            testFlucList1.Add(0);
            testFlucList1.Add(1);
            testFlucList1.Add(1);


            T_Wave_Alt_Alg testAlgs = new T_Wave_Alt_Alg();

            // Process test here

            List<int> receivedFluc = testAlgs.findFluctuations(testACIVector1);

            // Assert results

            CollectionAssert.AreEqual(testFlucList1, receivedFluc);
        }

        [TestMethod]
        [Description("Tests if alternans logic criteria are executed properly")]
        public void findAlternansTest()
        {
            // Init test here

            List<int> testFlucList1 = new List<int>();
            testFlucList1.Add(0);
            testFlucList1.Add(0);
            testFlucList1.Add(1);
            testFlucList1.Add(1);
            testFlucList1.Add(0);
            testFlucList1.Add(0);
            testFlucList1.Add(0);
            testFlucList1.Add(0);
            testFlucList1.Add(1);
            testFlucList1.Add(1);
            testFlucList1.Add(1);
            testFlucList1.Add(0);

            double[] testAlternansArray = { 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1 };
            Vector<double> testAlternansVector = Vector<double>.Build.DenseOfArray(testAlternansArray);

            T_Wave_Alt_Alg testAlgs = new T_Wave_Alt_Alg();

            // Process test here

            Vector<double> receivedAlternans = testAlgs.findAlternans(testFlucList1);

            // Assert results

            Assert.AreEqual(testAlternansVector, receivedAlternans);
        }

        [TestMethod]
        [Description("Tests if output data is correctly formatted to be sent to visualisation module")]
        public void alternansDetectionFormatTest()
        {
            // Init test here

            double[] testAlternansArray = { 1, 0, 1, 1, 0, 0, 1, 0 };
            Vector<double> testAlternansVector = Vector<double>.Build.DenseOfArray(testAlternansArray);

            List<int> testTEndsList1 = new List<int>();
            testTEndsList1.Add(124);
            testTEndsList1.Add(234);
            testTEndsList1.Add(345);
            testTEndsList1.Add(456);
            testTEndsList1.Add(567);
            testTEndsList1.Add(678);
            testTEndsList1.Add(789);
            testTEndsList1.Add(890);

            Tuple<int, int> record1 = new Tuple<int, int>(124, 1);
            Tuple<int, int> record2 = new Tuple<int, int>(345, 1);
            Tuple<int, int> record3 = new Tuple<int, int>(456, 1);
            Tuple<int, int> record4 = new Tuple<int, int>(789, 1);

            List<Tuple<int, int>> testFinalList1 = new List<Tuple<int, int>>();
            testFinalList1.Add(record1);
            testFinalList1.Add(record2);
            testFinalList1.Add(record3);
            testFinalList1.Add(record4);

            T_Wave_Alt_Alg testAlgs = new T_Wave_Alt_Alg();

            // Process test here

           List<Tuple<int,int>> receivedList = testAlgs.alternansDetection(testAlternansVector,testTEndsList1);

            // Assert results

            CollectionAssert.AreEqual(testFinalList1, receivedList);
        }
    }
}
