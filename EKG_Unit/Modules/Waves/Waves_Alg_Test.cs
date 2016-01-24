using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EKG_Project.Modules.Waves;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Unit.Modules.Waves
{
    [TestClass]
    public class Waves_Alg_Test
    {
        [TestMethod]
        [Description("Test if method finds maximum in vector")]
        public void MaxValueTest1()
        {
            Waves_Params testParams = new Waves_Params(Wavelet_Type.haar,3,"Analysis1",500);

            double[] testArray = { 8, 7, 6, 5, 5, 9, 6, 7, 8 , 10 , 9 };
            double result = 5;
            int begin_loc = 0;
            int end_loc = 6;
            double max_val = 0;
            int max_loc = 0;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            Waves_Alg testAlgs = new Waves_Alg(testParams);
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { begin_loc , end_loc , max_loc , max_val , testVector };

            obj.Invoke("FindMaxValue", args);

            Assert.AreEqual(max_val, result);


        }

        [TestMethod]
        [Description("Test if method finds location of maximum in vector")]
        public void MaxValueTest2()
        {
            Waves_Params testParams = new Waves_Params(Wavelet_Type.haar, 3, "Analysis1", 500);

            double[] testArray = { 8, 7, 6, 5, 5, 9, 6, 7, 8, 10, 9 };
            int result = 8;
            int begin_loc = 0;
            int end_loc = 6;
            double max_val = 0;
            int max_loc = 0;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            Waves_Alg testAlgs = new Waves_Alg(testParams);
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { begin_loc, end_loc, max_loc, max_val, testVector };

            obj.Invoke("FindMaxValue", args);

            Assert.AreEqual(max_loc, result);


        }
    }
}
