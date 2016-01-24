using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.HRV1;


namespace EKG_Unit.Modules.HRV1
{
    [TestClass]
    public class HRV1_Alg_Test
    {
        [TestMethod]
        [Description("Test of LS")]
        public void lombscargleTest()
        {
            // Init test here

            var hrv1Test = new HRV1_Alg();

            var testintervals = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            hrv1Test.rrIntervals = Vector<double>.Build.Dense(testintervals);
            hrv1Test.rInstants = Vector<double>.Build.Dense(testintervals);

            var expectedResultArr = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var expectedResultVec = Vector<double>.Build.Dense(expectedResultArr);



            // Since we want to test private method there is needed another overhead - public function does not need this
            // can be invoked in normal way

            PrivateObject obj = new PrivateObject(hrv1Test);
            PrivateObject obj2 = new PrivateObject(hrv1Test.rrIntervals);
            PrivateObject obj3 = new PrivateObject(hrv1Test.rInstants);
            // Process test here

            obj.Invoke("lombScargle");

            // Assert results

            Assert.AreEqual(algoOutput, expectedResultVec);


        }
    }



}
