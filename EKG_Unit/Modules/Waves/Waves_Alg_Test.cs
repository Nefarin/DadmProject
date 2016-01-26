using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EKG_Project.Modules.Waves;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;

namespace EKG_Unit.Modules.Waves
{
    [TestClass]
    public class Waves_Alg_Test
    {
        [TestMethod]
        [Description("Test if method returns exception when decomposition level is too low")]
        [ExpectedException(typeof(InvalidOperationException), "Decompositionlevel is too low")]
        public void ListHaarDWTTest()
        {
            double [] signalArray = { 8, 7, 6, 5, 5, 9, 6, 7, 8, 10, 9 };
            Vector<double> signalVector = Vector<double>.Build.DenseOfArray(signalArray);
            Waves_Params testParams = new Waves_Params(Wavelet_Type.haar, 3, "Analysis1", 500);

            Waves_Alg testAlgs = new Waves_Alg(testParams);
            PrivateObject obj = new PrivateObject(testAlgs);

            object[] args = { signalVector, 0 };

            obj.Invoke("ListHaarDWT", args);
        }

        [TestMethod]
        [Description("Test if method return exception decomposition level is to high (not enough samples)")]
        [ExpectedException(typeof(InvalidOperationException), "Not long enough signal for such decomposition")]
        public void ListHaarDWTTest2()
        {
            double[] signalArray = { 8, 7, 6, 5, 5, 9, 6, 7, 8, 10, 9 };
            Vector<double> signalVector = Vector<double>.Build.DenseOfArray(signalArray);
            Waves_Params testParams = new Waves_Params(Wavelet_Type.haar, 4, "Analysis1", 500);

            Waves_Alg testAlgs = new Waves_Alg(testParams);
            PrivateObject obj = new PrivateObject(testAlgs);

            object[] args = { signalVector, 0 };

            obj.Invoke("ListHaarDWT", args);
        }

        [TestMethod]
        [Description("Test if method returns right result of Haar decomposition")]
        public void HaarDecompositionResult()
        {
            Waves_Params param = new Waves_Params(Wavelet_Type.haar, 2, "TestAnalysis100", 500);
            Waves_Alg testAlgs = new Waves_Alg(param);

            double[] signalArray = { 1, 2, 3, 4, 5, 9, 7, 7, 8, 24, 9 };
            Vector<double> signalVector = Vector<double>.Build.DenseOfArray(signalArray);

            double[] trueOut = { -2, 0};
            Vector<double> trueOutVector = Vector<double>.Build.DenseOfArray(trueOut);

            PrivateObject obj = new PrivateObject(testAlgs);

            object[] args = { signalVector, 2 };
            var dwt  = obj.Invoke("ListHaarDWT", args);
            List<Vector<double>> dwtL = (List<Vector<double>>)dwt;
            Vector<double> dwtLevel2 = dwtL[1];
            Assert.AreEqual(dwtLevel2 , trueOutVector);
        }

        [TestMethod]
        [Description("Test if method returns right result of db2 decomposition")]
        public void db2DecompositionResult()
        {
            Waves_Params param = new Waves_Params(Wavelet_Type.db2, 2, "TestAnalysis100", 500);
            Waves_Alg testAlgs = new Waves_Alg(param);

            double[] signalArray = { 1, 2, 3, 4, 5, 9, 7, 7, 8, 24, 9 };
            Vector<double> signalVector = Vector<double>.Build.DenseOfArray(signalArray);

            double[] trueOut = { 3.6818, 18.5651 };
            Vector<double> trueOutVector = Vector<double>.Build.DenseOfArray(trueOut);

            PrivateObject obj = new PrivateObject(testAlgs);
            
            object[] args = { signalVector, 2 , Wavelet_Type.db2};
            var dwt = obj.Invoke("ListDWT", args);
            List<Vector<double>> dwtL = (List<Vector<double>>)dwt;
            Vector<double> dwtLevel2 = dwtL[1];
            for( int i = 0; i< dwtLevel2.Count; i++)
            {
                dwtLevel2[i] = Math.Round(dwtLevel2[i], 4);
            }
            Assert.AreEqual(dwtLevel2, trueOutVector);
        }

        [TestMethod]
        [Description("Test if method returns right result of db2 decomposition")]
        public void db3DecompositionResult()
        {
            Waves_Params param = new Waves_Params(Wavelet_Type.db3, 2, "TestAnalysis100", 500);
            Waves_Alg testAlgs = new Waves_Alg(param);

            double[] signalArray = { 1, 2, 3, 4, 5, 9, 7, 7, 8, 24, 9 };
            Vector<double> signalVector = Vector<double>.Build.DenseOfArray(signalArray);

            double[] trueOut = { 16.5803, -2.4791 };
            Vector<double> trueOutVector = Vector<double>.Build.DenseOfArray(trueOut);

            PrivateObject obj = new PrivateObject(testAlgs);

            object[] args = { signalVector, 2, Wavelet_Type.db3 };
            var dwt = obj.Invoke("ListDWT", args);
            List<Vector<double>> dwtL = (List<Vector<double>>)dwt;
            Vector<double> dwtLevel2 = dwtL[1];
            for (int i = 0; i < dwtLevel2.Count; i++)
            {
                dwtLevel2[i] = Math.Round(dwtLevel2[i], 4);
            }
            Assert.AreEqual(dwtLevel2, trueOutVector);
        }

        [TestMethod]
        [Description("Test if derivSquare return right value")]
        public void derivSquareValueTest()
        {
            Waves_Params param = new Waves_Params(Wavelet_Type.db3, 2, "TestAnalysis100", 500);
            Waves_Alg testAlgs = new Waves_Alg(param);
            double[] signalArray = { 1, 2, 3, 4, 5, 9, 7, 7, 8, 24, 9 };
            Vector<double> signalVector = Vector<double>.Build.DenseOfArray(signalArray);

            double trueResult = 3.0625;
            PrivateObject obj = new PrivateObject(testAlgs);

            object[] args = { 4, signalVector };
            double result2check = (double)obj.Invoke("derivSquare", args);

            Assert.AreEqual(result2check, trueResult, 0.0001);
        }

        [TestMethod]
        [Description("Test if method finds maximum value in vector")]
        public void MaxValueTest1()
        {
            Waves_Params testParams = new Waves_Params(Wavelet_Type.haar,3,"Analysis1",500);

            double[] testArray = { 8, 7, 6, 5, 5, 9, 6, 7, 8 , 10 , 9 };
            double result = 9;
            int begin_loc = 0;
            int end_loc = 6;
            double max_val = 0;
            int max_loc = 0;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            Waves_Alg testAlgs = new Waves_Alg(testParams);
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { begin_loc , end_loc ,  max_loc ,  max_val , testVector };

            obj.Invoke("FindMaxValue", args);

            Assert.AreEqual( args[3] , result);


        }

        [TestMethod]
        [Description("Test if method finds location of maximum in vector")]
        public void MaxValueTest2()
        {
            Waves_Params testParams = new Waves_Params(Wavelet_Type.haar, 3, "Analysis1", 500);

            double[] testArray = { 8, 7, 6, 5, 5, 9, 6, 7, 8, 10, 9 };
            int result = 5;
            int begin_loc = 0;
            int end_loc = 6;
            double max_val = 0;
            int max_loc = 0;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            Waves_Alg testAlgs = new Waves_Alg(testParams);
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { begin_loc, end_loc, max_loc, max_val, testVector };

            obj.Invoke("FindMaxValue", args);

            Assert.AreEqual( args[2], result);


        }
    }
}
