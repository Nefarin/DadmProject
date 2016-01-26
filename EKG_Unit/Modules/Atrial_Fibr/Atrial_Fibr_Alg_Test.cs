using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.Atrial_Fibr;

namespace EKG_Unit.Modules.Atrial_Fibr
{
    [TestClass]
    public class Atrial_Fibr_Alg_Test
    {
        [TestMethod]
        [Description("Test if turning points is correct for AF signal")]
        public void TPRTestAF()
        {
            //init
            double[] testArray = { 137, 147, 156, 129, 130, 123, 129, 138, 138, 162, 138, 161, 123, 144, 157, 156, 142, 136, 118, 136, 158, 139, 178, 158, 164, 126, 135, 134, 173, 171, 170, 143 };
            double result = 0.95;
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { testVector };
            double delta = 0.001;
            //test
            double realresult= Convert.ToDouble(obj.Invoke("TPR",args));
            //results
            Assert.AreEqual(realresult, result, delta);
        }

        [TestMethod]
        [Description("Test if turning points is correct for healthy signal")]
        public void TPRTestNAF()
        {
            //init
            double[] testArray = { 85, 84, 82, 83, 84, 89, 94, 101, 101, 99, 97, 96, 95, 95, 97, 98, 97, 94, 95, 98, 95, 97, 101, 103, 95, 95, 97, 98, 97, 97, 100, 100 };
            double result = 0.35;
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { testVector };
            double delta = 0.001;
            //test
            double realresult = Convert.ToDouble(obj.Invoke("TPR", args));
            //results
            Assert.AreEqual(realresult, result, delta);
        }
        [TestMethod]
        [Description("Test if SE is correct for AF signal")]
        public void SETestAF()
        {
            //init
            double[] testArray = { 137, 147, 156, 129, 130, 123, 129, 138, 138, 162, 138, 161, 123, 144, 157, 156, 142, 136, 118, 136, 158, 139, 178, 158, 164, 126, 135, 134, 173, 171, 170, 143 };
            double result = 0.9865;
            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { testVector };
            double delta = 0.001;
            //test
            double realresult = Convert.ToDouble(obj.Invoke("SE", args));
            //results
            Assert.AreEqual(result, realresult,  delta);
        }
    }
}
