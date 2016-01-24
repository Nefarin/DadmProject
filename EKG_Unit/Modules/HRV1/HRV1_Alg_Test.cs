using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EKG_Project.Modules.HRV1;
using MathNet.Numerics.LinearAlgebra;


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

            var testintervals = Vector<double>.Build.Dense(10, i => i);
            var testinstants = Vector<double>.Build.Dense(10, i => i);
            var testfreq = Vector<double>.Build.Dense(10, i => (double)i/10);

            var expectedResult = Vector<double>.Build.Dense(10, i => i);

            // access private fields externally

            PrivateObject obj = new PrivateObject(hrv1Test);
            obj.SetField("rrIntervals", testintervals);
            obj.SetField("rInstants", testinstants);
            obj.SetField("f", testfreq);

            // Process test here

            obj.Invoke("lombScargle");
            var actualResult = obj.GetField("PSD");

            // Assert results

            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}
