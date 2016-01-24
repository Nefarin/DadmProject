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
        [TestMethod]
        [Description("Test if DataToCalculate constructs proper data")]
        public void test_DataToCalculate()
        {
            double[] samp1 = { 2.09, 3.94, 4.66, 9.78, 10.62 };
            double[] RR = { 12, 13 };
            Vector<double> samp = Vector<double>.Build.DenseOfArray(samp1);

            DataToCalculate data = new DataToCalculate(22, 37, 42, samp, QT_Calc_Method.FRAMIGHAMA, T_End_Method.PARABOLA, 360, RR);

            PrivateObject obj = new PrivateObject(data);
            Assert.AreEqual(22, obj.GetField("QRS_onset"));
            Assert.AreEqual(37, obj.GetField("QRS_End"));
            Assert.AreEqual(42, obj.GetField("T_End_Global"));
            Assert.AreEqual(samp, obj.GetField("samples"));
            Assert.AreEqual(QT_Calc_Method.FRAMIGHAMA, obj.GetField("QT_Calc_method"));
            Assert.AreEqual(T_End_Method.PARABOLA, obj.GetField("T_End_method"));
            Assert.AreEqual((uint)360, obj.GetField("Fs"));
            Assert.AreEqual(RR, obj.GetField("R_Peak"));
        }

        [TestMethod]
        [Description("Test if default constructor sets proper data")]
        public void test_DataToCalculate2()
        {
            double[] RR = new double[2];
            Vector<double> samp = Vector<double>.Build.Dense(1);

            DataToCalculate data = new DataToCalculate();

            PrivateObject obj = new PrivateObject(data);
            Assert.AreEqual(-1, obj.GetField("QRS_onset"));
            Assert.AreEqual(-1, obj.GetField("QRS_End"));
            Assert.AreEqual(-1, obj.GetField("T_End_Global"));
            //Assert.AreEqual(samp, obj.GetField("samples"));
            Assert.AreEqual(QT_Calc_Method.BAZETTA, obj.GetField("QT_Calc_method"));
            Assert.AreEqual(T_End_Method.TANGENT, obj.GetField("T_End_method"));
            Assert.AreEqual((uint)360, obj.GetField("Fs"));
           // Assert.AreEqual(RR, obj.GetField("R_Peak"));
        }
    }
}
