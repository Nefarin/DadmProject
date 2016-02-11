using System;
using System.IO;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EKG_Project.IO;

namespace EKG_Unit.IO
{
    [TestClass]
    public class Basic_Data_Worker_Test
    {
        [TestMethod]
        public void SaveAttributeTest()
        {
            Basic_New_Data_Worker worker = new Basic_New_Data_Worker("Test");
            worker.SaveAttribute(Basic_Attributes.Frequency, 360);

            IECGPath pathBuilder = new DebugECGPath();
            string directory = pathBuilder.getTempPath();
            string fileName = "Test_Basic_New_Frequency.txt";
            string expectedPath = System.IO.Path.Combine(directory, fileName);

            Assert.IsTrue(File.Exists(expectedPath));
        }

        [TestMethod]
        public void LoadAttributeTest()
        {
            Basic_New_Data_Worker worker = new Basic_New_Data_Worker("Test");
            worker.SaveAttribute(Basic_Attributes.Frequency, 360);

            uint expected = 360;

            uint actual = worker.LoadAttribute(Basic_Attributes.Frequency);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SaveSignalTest()
        {
            Basic_New_Data_Worker worker = new Basic_New_Data_Worker("Test");

            double[] savedArray = { -0.065, -0.065, -0.065, -0.065, -0.065, -0.065, -0.065, -0.065, -0.08, -0.08 };
            Vector<double> savedVector = Vector<double>.Build.Dense(savedArray.Length);
            savedVector.SetValues(savedArray);

            worker.SaveSignal("V5", false, savedVector);

            IECGPath pathBuilder = new DebugECGPath();
            string directory = pathBuilder.getTempPath();
            string fileName = "Test_Basic_New_V5.txt";
            string expectedPath = System.IO.Path.Combine(directory, fileName);

            Assert.IsTrue(File.Exists(expectedPath));
        }

        [TestMethod]
        public void LoadSignalTest()
        {
            Basic_New_Data_Worker worker = new Basic_New_Data_Worker("Test");

            double[] savedArray = { -0.065, -0.065, -0.065, -0.065, -0.065, -0.065, -0.065, -0.065, -0.08, -0.08 };
            Vector<double> expectedVector = Vector<double>.Build.Dense(savedArray.Length);
            expectedVector.SetValues(savedArray);

            worker.SaveSignal("V5", false, expectedVector);

            Vector<double> actualVector = worker.LoadSignal("V5", 0, 10);

            Assert.AreEqual(expectedVector, actualVector);
        }

        [TestMethod]
        public void getNumberOfSamplesTest()
        {
            Basic_New_Data_Worker worker = new Basic_New_Data_Worker("Test");

            double[] savedArray = { -0.065, -0.065, -0.065, -0.065, -0.065, -0.065, -0.065, -0.065, -0.08, -0.08 };
            Vector<double> savedVector = Vector<double>.Build.Dense(savedArray.Length);
            savedVector.SetValues(savedArray);

            worker.SaveSignal("V5", false, savedVector);
            worker.SaveSignal("V5", false, savedVector);

            uint expected1 = 10;
            uint actual1 = worker.getNumberOfSamples("V5");

            Assert.AreEqual(expected1, actual1);

            worker.SaveSignal("V5", true, savedVector);
            worker.SaveSignal("V5", true, savedVector);

            uint expected2 = 30;
            uint actual2 = worker.getNumberOfSamples("V5");

            Assert.AreEqual(expected2, actual2);
        }

        [TestMethod]
        public void SaveLeadsTest()
        {
            Basic_New_Data_Worker worker = new Basic_New_Data_Worker("Test");
            worker.SaveLeads(new List<string>() { "MLII", "V5" });

            IECGPath pathBuilder = new DebugECGPath();
            string directory = pathBuilder.getTempPath();
            string fileName = "Test_Basic_New_Leads.txt";
            string expectedPath = System.IO.Path.Combine(directory, fileName);

            Assert.IsTrue(File.Exists(expectedPath));
        }

        [TestMethod]
        public void LoadLeadsTest()
        {
            Basic_New_Data_Worker worker = new Basic_New_Data_Worker("Test");
            worker.SaveLeads(new List<string>() { "MLII", "V5" });

            List<string> expected = new List<string>() { "MLII", "V5" };
            List<string> actual = worker.LoadLeads();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DeleteFilesTest()
        {
            Basic_New_Data_Worker worker = new Basic_New_Data_Worker("Test");
            worker.SaveLeads(new List<string>() { "MLII", "V5" });

            IECGPath pathBuilder = new DebugECGPath();
            string directory1 = pathBuilder.getTempPath();
            string fileName1 = "Test_Basic_New_Leads.txt";
            string expectedPath1 = System.IO.Path.Combine(directory1, fileName1);

            double[] savedArray = { -0.065, -0.065, -0.065, -0.065, -0.065, -0.065, -0.065, -0.065, -0.08, -0.08 };
            Vector<double> savedVector = Vector<double>.Build.Dense(savedArray.Length);
            savedVector.SetValues(savedArray);

            worker.SaveSignal("V5", false, savedVector);

            string directory2 = pathBuilder.getTempPath();
            string fileName2 = "Test_Basic_New_V5.txt";
            string expectedPath2 = System.IO.Path.Combine(directory2, fileName2);

            worker.SaveAttribute(Basic_Attributes.Frequency, 360);

            string directory3 = pathBuilder.getTempPath();
            string fileName3 = "Test_Basic_New_Frequency.txt";
            string expectedPath3 = System.IO.Path.Combine(directory3, fileName3);

            worker.DeleteFiles();

            Assert.IsFalse(File.Exists(expectedPath1));
            Assert.IsFalse(File.Exists(expectedPath2));
            Assert.IsFalse(File.Exists(expectedPath3));
        }
    }
}
