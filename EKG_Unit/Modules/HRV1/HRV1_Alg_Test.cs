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
            var testfreq = Vector<double>.Build.Dense(10, i => (double)i / 10);

            var expectedArr = new double[] { 0, 52.3607, 14.4721, 7.6393, 5.5279, 2.5000, 5.5279, 7.6393, 14.4721, 52.3607 };
            var expectedResult = Vector<double>.Build.Dense(expectedArr);

            // access private fields externally

            PrivateObject obj = new PrivateObject(hrv1Test);
            obj.SetField("rrIntervals", testintervals);
            obj.SetField("rInstants", testinstants);
            obj.SetField("f", testfreq);

            // Process test here

            obj.Invoke("lombScargle");
            var actualResult = (Vector<double>)obj.GetField("PSD");

            // Assert results

            Assert.AreEqual(expectedResult.Count, actualResult.Count);

            for (int i = 0; i < expectedResult.Count; ++i)
            {
                Assert.IsTrue(Math.Abs(expectedResult[i] - actualResult[i]) < 100);
            }
        }

        [TestMethod]
        [Description("Test of instants To Intervals")]
        public void instantsToIntervalsTest()
        {
            // Init test here

            var hrv1Test = new HRV1_Alg();

            var testinstants = new double[]{ 2, 3, 5, 9, 11, 15, 20, 22, 28, 30};    

            var expectedArr = new double[] { 1, 2, 4, 2, 4, 5, 2, 6, 2 };
            var expectedResult = Vector<double>.Build.Dense(expectedArr);

            // access private fields externally

            PrivateObject obj = new PrivateObject(hrv1Test);
            obj.SetField("rInstants", testinstants);

            // Process test here

            obj.Invoke("instantsToIntervals");
            var actualResult = (Vector<double>)obj.GetField("rrIntervals");

            // Assert results

            Assert.AreEqual(expectedResult.Count, actualResult.Count);

        }
    }
}
