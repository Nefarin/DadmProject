using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.QT_Disp;

namespace EKG_Unit.Modules.QT_Disp
{
    [TestClass]
    public class QT_Disp_Alg_Test
    {
       
        [TestMethod]
        [Description("Test if diff returns proper values")]
        public void test_diff()
        {
            DataToCalculate data = new DataToCalculate();

            double[] testArray = { 1, 2, 5, 9, 11};
            double[] expectedArray = { 1, 3, 4, 2 };

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> expectedVector = Vector<double>.Build.DenseOfArray(expectedArray);

            Vector<double> result = data.diff(testVector);

            Assert.AreEqual(expectedVector, result);

        }
        [TestMethod]
        [Description("Test if diff argument is correct")]
        [ExpectedException(typeof(InvalidOperationException), "Wrong input vector")]
        public void test_diff2()
        {
            DataToCalculate data = new DataToCalculate();

            double[] testArray = { 1};

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);    
            Vector<double> result = data.diff(testVector);
        }

    }
}
