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

            //Atrial_Fibr_Params testParams = new Atrial_Fibr_Params(Detect_Method.STATISTIC, "test");

            double[] testArray = { 137, 147, 156, 129, 130, 123, 129, 138, 138, 162, 138, 161, 123, 144, 157, 156, 142, 136, 118, 136, 158, 139, 178, 158, 164, 126, 135, 134, 173, 171, 170, 143 };
            double result = 0.95;


            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            
            Atrial_Fibr_Alg testAlg = new Atrial_Fibr_Alg();
            PrivateObject obj = new PrivateObject(testAlg);
            object[] args = { testVector };
            double realresult= Convert.ToDouble(obj.Invoke("TPR",args));
            double delta = 0.001;
            Assert.AreEqual(realresult, result, delta);
        }
    }
}
