using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EKG_Project.IO;

namespace EKG_Unit.IO
{
    [TestClass]
    public class ASCIIConverter_Test
    {
        [TestMethod]
        public void getFrequencyTest()
        {
            ASCIIConverter ascii = new ASCIIConverter();
            PrivateObject obj = new PrivateObject(ascii);
            obj.SetField("path", @"C:\temp\100.txt");
            obj.SetField("numberOfSamples", (uint) 3600);

            uint expected = 360;

            Assert.AreEqual(expected, obj.Invoke("getFrequency"));
        }

        [TestMethod]
        public void getTimeFormatTest()
        {
            ASCIIConverter ascii = new ASCIIConverter();

            string expected1 = "m:ss.fff";
            string expected2 = "mm:ss.fff";
            string expected3 = "h:mm:ss.fff";
            string expected4 = "hh:mm:ss.fff";

            Assert.AreEqual(expected1, ascii.getTimeFormat("0:00.001"));
            Assert.AreEqual(expected2, ascii.getTimeFormat("00:00.001"));
            Assert.AreEqual(expected3, ascii.getTimeFormat("0:00:00.001"));
            Assert.AreEqual(expected4, ascii.getTimeFormat("00:00:00.001"));
        }

        [TestMethod]
        public void getSignalTest()
        {
            ASCIIConverter ascii = new ASCIIConverter();
            PrivateObject obj = new PrivateObject(ascii);
            obj.SetField("path", @"C:\temp\100.txt");
            obj.SetField("leads", new List<string>() {"MLII", "V5"});

            double[] expectedArray = { -0.065, -0.065, -0.065, -0.065, -0.065, -0.065, -0.065, -0.065, -0.08, -0.08 };
            Vector<double> expectedVector = Vector<double>.Build.Dense(expectedArray.Length);
            expectedVector.SetValues(expectedArray);

            Vector<double> actualVector = (Vector<double>) obj.Invoke("getSignal", new object[]{"V5", 0, 10});

            Assert.AreEqual(expectedVector, actualVector);
        }

        [TestMethod]
        public void getLeadsTest()
        {
            ASCIIConverter ascii = new ASCIIConverter();
            PrivateObject obj = new PrivateObject(ascii);
            obj.SetField("path", @"C:\temp\100.txt");

            List<string> expectedList = new List<string>() { "MLII", "V5" };

            List<string> actualList = (List<string>) obj.Invoke("getLeads");

            CollectionAssert.AreEqual(expectedList, actualList);
        }

        [TestMethod]
        public void getNumberOfSamplesTest()
        {
            ASCIIConverter ascii = new ASCIIConverter();
            PrivateObject obj = new PrivateObject(ascii);
            obj.SetField("path", @"C:\temp\100.txt");
            obj.SetField("leads", new List<string>() { "MLII", "V5" });

            uint expectedValue = 3600;

            Assert.AreEqual(expectedValue, obj.Invoke("getNumberOfSamples", new object[] {"MLII"}));

        }

        public static void Main()
        {
        }
    }
}
