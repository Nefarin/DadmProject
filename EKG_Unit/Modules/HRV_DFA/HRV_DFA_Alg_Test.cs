using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.HRV_DFA;
using System.Collections.Generic;

namespace EKG_Unit.Modules.HRV_DFA
{
    [TestClass]
    public class HRV_DFA_Alg_Test
    {
        //Test for subsidary methods
       
        //RemoveZeros
        [TestMethod]
        [Description("Test if RemoveZeros works properly")]
        public void RemoveZerosTest()
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
        public void RemoveZerosWhenNoZerosTest()
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
        public void RemoveZerosNullInputTest()
        {
            Vector<double> testVector2 = null;
            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> testResult = test.RemoveZeros(testVector2);
        }

        [TestMethod]
        [Description("Test if Remove zeros returns null when only zeros in Vector")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Null given as parameter")]
        public void RemoveZerosWhenAllZerosTest()
        {
            double[] testArray = { 0, 0, 0, 0 };
            Vector<double> testVector3 = Vector<double>.Build.DenseOfArray(testArray);
            HRV_DFA_Alg test = new HRV_DFA_Alg();
            Vector<double> testResult = test.RemoveZeros(testVector3);
        }
    }
}
