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

            double[] testArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            double[] resultArray1 = { 2, 3, 4 };
            double[] resultArray2 = { 7, 8, 9 };


            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            List<int> testTEndList = new List<int>();
            testTEndList.Add(4); // index!!! not the 4th element
            testTEndList.Add(9);
            Vector<double> resultVector1 = Vector<double>.Build.DenseOfArray(resultArray1);
            Vector<double> resultVector2 = Vector<double>.Build.DenseOfArray(resultArray2);
            List<Vector<double>> resultList = new List<Vector<double>>();
            resultList.Add(resultVector1);
            resultList.Add(resultVector2);

            T_Wave_Alt_Alg testAlgs = new T_Wave_Alt_Alg();

            // Process test here

            testAlgs.Fs = 20;

            List<Vector<double>> desiredList = testAlgs.buildTWavesArray(testVector, testTEndList, 0);

            // Assert results

            Assert.AreEqual(resultVector1, desiredList[0]);
            Assert.AreEqual(resultVector2, desiredList[1]);


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

            testAlgs.Fs = 20;

            List<Vector<double>> desiredList = testAlgs.buildTWavesArray(testVector, testTEndList, 0);

            // Assert results

            Assert.AreNotEqual(desiredList[0], resultVector1);
            Assert.AreNotEqual(desiredList[1], resultVector2);

        }

        [TestMethod]
        [Description("Tests if the buildTWavesArray function omits the waves it cannot build ('left' border effect)")]
        public void buildTWavesArrayLeftBorderTest()
        {
            // Init test here

            double[] testArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            double[] resultArray1 = { 3, 4, 5 };
            double[] resultArray2 = { 7, 8, 9 };


            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            List<int> testTEndList = new List<int>();
            testTEndList.Add(1); // additional t-end - t-wave can't be built
            testTEndList.Add(5);
            testTEndList.Add(9);
            Vector<double> resultVector1 = Vector<double>.Build.DenseOfArray(resultArray1);
            Vector<double> resultVector2 = Vector<double>.Build.DenseOfArray(resultArray2);
            List<Vector<double>> resultList = new List<Vector<double>>();
            resultList.Add(resultVector1);
            resultList.Add(resultVector2);

            T_Wave_Alt_Alg testAlgs = new T_Wave_Alt_Alg();

            // Process test here

            testAlgs.Fs = 20;

            List<Vector<double>> desiredList = testAlgs.buildTWavesArray(testVector, testTEndList, 0);

            // Assert results

            Assert.AreEqual(resultVector1, desiredList[0]);
            Assert.AreEqual(resultVector2, desiredList[1]);


        }

        [TestMethod]
        [Description("Tests if the buildTWavesArray function omits T-ends lying outside the range (both borders effect)")]
        public void buildTWavesArrayBordersTest()
        {
            // Init test here

            double[] testArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            double[] resultArray1 = { 3, 4, 5 };

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            List<int> testTEndList = new List<int>();
            testTEndList.Add(-3); 
            testTEndList.Add(5);
            testTEndList.Add(19);

            Vector<double> resultVector1 = Vector<double>.Build.DenseOfArray(resultArray1);
            
            List<Vector<double>> resultList = new List<Vector<double>>();
            resultList.Add(resultVector1);

            T_Wave_Alt_Alg testAlgs = new T_Wave_Alt_Alg();

            // Process test here

            testAlgs.Fs = 20;

            List<Vector<double>> desiredList = testAlgs.buildTWavesArray(testVector, testTEndList, 0);

            // Assert results

            Assert.AreEqual(resultVector1, desiredList[0]);

        }

        [TestMethod]
        [Description("Tests if T-wave array is built properly with currentIndex parameter")]
        public void buildTWavesArrayIndexShiftTest()
        {
            // Init test here

            double[] testArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            double[] resultArray1 = { 2, 3, 4 };
            double[] resultArray2 = { 7, 8, 9 };


            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            List<int> testTEndList = new List<int>();
            testTEndList.Add(24);
            testTEndList.Add(29);
            Vector<double> resultVector1 = Vector<double>.Build.DenseOfArray(resultArray1);
            Vector<double> resultVector2 = Vector<double>.Build.DenseOfArray(resultArray2);
            List<Vector<double>> resultList = new List<Vector<double>>();
            resultList.Add(resultVector1);
            resultList.Add(resultVector2);

            T_Wave_Alt_Alg testAlgs = new T_Wave_Alt_Alg();

            // Process test here

            testAlgs.Fs = 20;

            List<Vector<double>> desiredList = testAlgs.buildTWavesArray(testVector, testTEndList, 20);

            // Assert results

            Assert.AreEqual(resultVector1, desiredList[0]);
            Assert.AreEqual(resultVector2, desiredList[1]);


        }

        [TestMethod]
        [Description("Tests if buildTWavesArray properly counts not detected T-waves")]
        public void buildTWavesArrayNoDetectionTest()
        {
            // Init test here

            double[] testArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 4, 5, 6 };
            double[] resultArray1 = { 0, 1, 2 };
            double[] resultArray2 = { 6, 7, 8 };


            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            List<int> testTEndList = new List<int>();
            testTEndList.Add(2);
            testTEndList.Add(-1);
            testTEndList.Add(8);
            testTEndList.Add(-1);
            Vector<double> resultVector1 = Vector<double>.Build.DenseOfArray(resultArray1);
            Vector<double> resultVector2 = Vector<double>.Build.DenseOfArray(resultArray2);
            List<Vector<double>> resultList = new List<Vector<double>>();
            resultList.Add(resultVector1);
            resultList.Add(resultVector2);

            T_Wave_Alt_Alg testAlgs = new T_Wave_Alt_Alg();

            // Process test here

            testAlgs.Fs = 20;

            List<Vector<double>> desiredList = testAlgs.buildTWavesArray(testVector, testTEndList, 0);

            // Assert results

            Assert.AreEqual(resultVector1, desiredList[0]);
            Assert.AreEqual(resultVector2, desiredList[1]);
            Assert.AreEqual(2, testAlgs.NoDetectionCount);


        }

        [TestMethod]
        [Description("Tests if buildTWavesArray properly builds _newTEndsList")]
        public void buildTWavesArrayNewTEndsListTest()
        {
            // Init test here

            double[] testArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 4, 5, 6 };
            
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            List<int> testTEndList = new List<int>();
            testTEndList.Add(1);
            testTEndList.Add(2);
            testTEndList.Add(-1);
            testTEndList.Add(8);
            testTEndList.Add(-1);
            testTEndList.Add(14);
            
            List<int> expectedNewTEndsList = new List<int>();
            expectedNewTEndsList.Add(2);
            expectedNewTEndsList.Add(8);

            T_Wave_Alt_Alg testAlgs = new T_Wave_Alt_Alg();

            // Process test here

            testAlgs.Fs = 20;

            List<Vector<double>> auxTWavesArray = testAlgs.buildTWavesArray(testVector, testTEndList, 0);
            List<int> resultNewTEndsList = testAlgs.NewTEndsList;

            // Assert results

            CollectionAssert.AreEqual(expectedNewTEndsList, resultNewTEndsList);
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

            testAlgs.TLength = 2; // defined here for easy testing purposes
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

            testAlgs.TLength = 2; // defined here for easy testing purposes
            Vector<double> desiredMedian = testAlgs.calculateMedianTWave(testList);

            // Assert results

            Assert.AreEqual(testMedianVec, desiredMedian);
        }

        [TestMethod]
        [Description("Tests if ACI are calculated properly - general logic test, all ACI values should equal 1")]
        public void calculateACITest1()
        {
            // Init test here

            double[] testArray1 = { 3, 6, 4 };
            double[] testArray2 = { 3, 6, 4 };
            double[] testArray3 = { 3, 6, 4 };

            double[] testMedian = { 3, 6, 4 };

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
        [Description("Tests if ACI are calculated properly - plausible values")]
        public void calculateACITest2()
        {
            // Init test here

            double[] testArray1 = { 2, 5, 6 };
            double[] testArray2 = { 3, 6, 4 };
            double[] testArray3 = { 3, 7, 4 };

            double[] testMedian = { 3, 6, 4 };

            double[] testACIArray = { 0.983, 1, 1.098 };

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
            double epsylon = 0.001;
            int iter = 0;
            foreach (double element in testACI)
            {
                Assert.AreEqual(testACI[iter], receivedACI[iter], epsylon);
                iter++;
            }
        }

        [TestMethod]
        [Description("Tests if ACI are calculated properly - other values")]
        public void calculateACITest3()
        {
            // Init test here

            double[] testArray1 = { 0.1, 0.41, 0.6 };
            double[] testArray2 = { 0.12, 0.4, 0.61 };
            double[] testArray3 = { 3.1, 0.14, 0.42 };

            double[] testMedian = { 0.115, 0.385, 0.605 };

            double[] testACIArray = { 1.009, 1.018, 1.26 };

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
            double epsylon = 0.001;
            int iter = 0;
            foreach (double element in testACI)
            {
                Assert.AreEqual(testACI[iter], receivedACI[iter], epsylon);
                iter++;
            }
        }

        [TestMethod]
        [Description("Tests if calculateACI works even if all of the medians are 0")]
        public void calculateACITest4()
        {
            // Init test here

            double[] testArray1 = { 1.5, 0.5, 2 };
            double[] testArray2 = { 0, 0, 0 };
            double[] testArray3 = { -0.3, -1, -0.5 };
            double[] testMedian = { 0, 0, 0 };

            double[] testACIArray = { 1.333, 0, -0.6 };

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
            double epsylon = 0.001;
            int iter = 0;
            foreach (double element in testACI)
            {
                Assert.AreEqual(testACI[iter], receivedACI[iter], epsylon);
                iter++;
            }
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
            testFlucList1.Add(1);
            testFlucList1.Add(1);
            testFlucList1.Add(1);
            testFlucList1.Add(1);
            testFlucList1.Add(0);

            double[] testAlternansArray = { 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0 };
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

            testAlgs.NewTEndsList = testTEndsList1;
            List<Tuple<int, int>> receivedList = testAlgs.alternansDetection(testAlternansVector, testTEndsList1);

            // Assert results

            CollectionAssert.AreEqual(testFinalList1, receivedList);
        }

        [TestMethod]
        [Description("Whole algorithm test - mock values")]
        public void wholeSequenceTest()
        {
            // Init test here
            // Inputs
            double[] testArray = { 0, 0.5, 3.2, 0.1, 0.41, 0.6, 0,
                                   0, 0.5, 3, 0.12, 0.4, 0.61, 0,
                                   0, 0.5, 3.1, 0.09, 0.36, 0.58, 0,
                                   0, 0.5, 3.3, 0.11, 0.68, 0.62, 0,
                                   0, 0.5, 3.6, 0.09, 0.36, 0.58, 0,
                                   0, 0.5, 3.1, 0.22, 0.74, 0.6, 0,
                                   0, 0.5, 3, 0.08, 0.37, 0.61, 0,
                                   0, 0.5, 3.2, 0.16, 0.68, 0.43, 0,
                                   0, 0.5, 3.3, 0.07, 0.4, 0.64, 0,
                                   0, 0.5, 3.1, 0.09, 0.42, 0.67, 0 };

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            List<int> testTEndList = new List<int>();
            testTEndList.Add(5);
            testTEndList.Add(12);
            testTEndList.Add(19);
            testTEndList.Add(26);
            testTEndList.Add(33);
            testTEndList.Add(39);
            testTEndList.Add(47);
            testTEndList.Add(55);
            testTEndList.Add(61);
            testTEndList.Add(67);

            // Desired outputs:
            Tuple<int, int> record1 = new Tuple<int, int>(19, 1);
            Tuple<int, int> record2 = new Tuple<int, int>(26, 1);
            Tuple<int, int> record3 = new Tuple<int, int>(33, 1);
            Tuple<int, int> record4 = new Tuple<int, int>(39, 1);
            Tuple<int, int> record5 = new Tuple<int, int>(47, 1);

            List<Tuple<int, int>> testFinalList1 = new List<Tuple<int, int>>();
            testFinalList1.Add(record1);
            testFinalList1.Add(record2);
            testFinalList1.Add(record3);
            testFinalList1.Add(record4);
            testFinalList1.Add(record5);

            T_Wave_Alt_Alg testAlgs = new T_Wave_Alt_Alg();

            // Process test here

            testAlgs.Fs = 20;
            List<Vector<double>> T_WavesArray = testAlgs.buildTWavesArray(testVector, testTEndList, 0);
            Vector<double> medianT_Wave = testAlgs.calculateMedianTWave(T_WavesArray);
            Vector<double> ACI = testAlgs.calculateACI(T_WavesArray, medianT_Wave);
            List<int> Flucts = testAlgs.findFluctuations(ACI);
            Vector<double> Alternans1 = testAlgs.findAlternans(Flucts);
            List<Tuple<int, int>> finalDetection = testAlgs.alternansDetection(Alternans1, testTEndList);

            foreach(Tuple<int,int> el in finalDetection)
            {
                Console.WriteLine(el.Item1);
            }

            // Assert results

            CollectionAssert.AreEqual(testFinalList1, finalDetection);

        }
    }
}
