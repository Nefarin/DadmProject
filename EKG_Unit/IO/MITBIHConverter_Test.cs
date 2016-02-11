using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EKG_Project.IO;

namespace EKG_Unit.IO
{
    [TestClass]
    public class MITBIHConverter_Test
    {
        [TestMethod]
        public void getFrequencyTest()
        {
            MITBIHConverter mitbih = new MITBIHConverter();
            mitbih.loadMITBIHFile(@"C:\temp\100.dat");

            uint expected = 360;

            uint actual = mitbih.getFrequency();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void getSignalTest()
        {
            MITBIHConverter mitbih = new MITBIHConverter();
            mitbih.loadMITBIHFile(@"C:\temp\100.dat");

            PrivateObject obj = new PrivateObject(mitbih);
            obj.SetField("leads", new List<string>() { "MLII", "V5" });

            double[] expectedArray = { -0.065, -0.065, -0.065, -0.065, -0.065, -0.065, -0.065, -0.065, -0.08, -0.08 };
            Vector<double> expectedVector = Vector<double>.Build.Dense(expectedArray.Length);
            expectedVector.SetValues(expectedArray);

            Vector<double> actualVector = (Vector<double>)obj.Invoke("getSignal", new object[] { "V5", 0, 10 });

            Assert.AreEqual(expectedVector, actualVector);
        }

        [TestMethod]
        public void getLeadsTest()
        {
            MITBIHConverter mitbih = new MITBIHConverter();
            mitbih.loadMITBIHFile(@"C:\temp\100.dat");

            List<string> expectedList = new List<string>() { "MLII", "V5" };

            List<string> actualList = mitbih.getLeads();

            CollectionAssert.AreEqual(expectedList, actualList);
        }

        [TestMethod]
        public void getNumberOfSamplesTest()
        {
            MITBIHConverter mitbih = new MITBIHConverter();
            mitbih.loadMITBIHFile(@"C:\temp\100.dat");

            PrivateObject obj = new PrivateObject(mitbih);
            obj.SetField("leads", new List<string>() { "MLII", "V5" });

            uint expectedValue = 650000;

            Assert.AreEqual(expectedValue, obj.Invoke("getNumberOfSamples", new object[] { "MLII" }));

        }
    }
}
