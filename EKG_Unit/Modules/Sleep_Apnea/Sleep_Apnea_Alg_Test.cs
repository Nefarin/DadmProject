using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.Sleep_Apnea;
using System.Collections.Generic;
using System.IO;
using System.Globalization;


namespace EKG_Unit.Modules.Sleep_Apnea
{
    [TestClass]
    public class Sleep_Apnea_Alg_Test
    {
        [TestMethod]
        [Description("Test if rr intervals are calculated correctly")]
        public void Sleep_Apnea_findIntervals_Test1()
        {
            // Init test here
            Sleep_Apnea_Params testParams = new Sleep_Apnea_Params("Test");
            Sleep_Apnea_Alg testAlgs = new Sleep_Apnea_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);

            // input
            List<string> lines = ReadFile("..\\..\\..\\IO\\data\\rpeaks.csv");
            List<uint> R_detected = new List<uint>();
            foreach (string line in lines)
            {
                R_detected.Add((uint)decimal.Parse(line, NumberStyles.Float));
            }
            int fs = 100;

            // ouput
            lines = ReadFile("..\\..\\..\\IO\\data\\time_in_sec.csv");
            List<double> timeInSecMatlab = new List<double>();
            foreach (string line in lines)
            {
                timeInSecMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            lines = ReadFile("..\\..\\..\\IO\\data\\rr_dist.csv");
            List<double> rrDistMatlab = new List<double>();
            foreach (string line in lines)
            {
                rrDistMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }

            // Process test here
            List<List<double>> result = (List<List<double>>)obj.Invoke("findIntervals", R_detected, fs);

            // Assert results
            Assert.AreEqual(timeInSecMatlab.Count, result[0].Count);
            Assert.AreEqual(rrDistMatlab.Count, result[1].Count);
            for (int i = 0; i < result[0].Count; i++)
            {
                Assert.AreEqual(timeInSecMatlab[i], result[0][i], 0.5);
                Assert.AreEqual(rrDistMatlab[i], result[1][i], 0.5);
            }
        }

        [TestMethod]
        [Description("Test average filter")]
        public void Sleep_Apnea_average_filter_Test1()
        {
            // Init test here
            Sleep_Apnea_Params testParams = new Sleep_Apnea_Params("Test");
            Sleep_Apnea_Alg testAlgs = new Sleep_Apnea_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);

            // input
            List<string> lines = ReadFile("..\\..\\..\\IO\\data\\time_in_sec.csv");
            List<double> timeInSecMatlab = new List<double>();
            foreach (string line in lines)
            {
                timeInSecMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            lines = ReadFile("..\\..\\..\\IO\\data\\rr_dist.csv");
            List<double> rrDistMatlab = new List<double>();
            foreach (string line in lines)
            {
                rrDistMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            List<List<double>> RR = new List<List<double>>();
            RR.Add(timeInSecMatlab);
            RR.Add(rrDistMatlab);

            // output
            lines = ReadFile("..\\..\\..\\IO\\data\\time_in_sec_filtered.csv");
            List<double> timeInSecFilteredMatlab = new List<double>();
            foreach (string line in lines)
            {
                timeInSecFilteredMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            lines = ReadFile("..\\..\\..\\IO\\data\\rr_dist_filtered.csv");
            List<double> rrDistFilteredMatlab = new List<double>();
            foreach (string line in lines)
            {
                rrDistFilteredMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }

            // Process test here
            List<List<double>> result = (List<List<double>>)obj.Invoke("averageFilter", RR);

            // Assert results
            Assert.AreEqual(timeInSecFilteredMatlab.Count, result[0].Count);
            Assert.AreEqual(rrDistFilteredMatlab.Count, result[1].Count);
            for (int i = 0; i < result[0].Count; i++)
            {
                Assert.AreEqual(timeInSecFilteredMatlab[i], result[0][i], 1);
                Assert.AreEqual(rrDistFilteredMatlab[i], result[1][i], 1);
            }
        }

        [TestMethod]
        [Description("Test resampling")]
        public void Sleep_Apnea_resampling_Test1()
        {
            // Init test here
            Sleep_Apnea_Params testParams = new Sleep_Apnea_Params("Test");
            Sleep_Apnea_Alg testAlgs = new Sleep_Apnea_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);

            // input
            List<string> lines = ReadFile("..\\..\\..\\IO\\data\\time_in_sec_filtered.csv");
            List<double> timeInSecFilteredMatlab = new List<double>();
            foreach (string line in lines)
            {
                timeInSecFilteredMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            lines = ReadFile("..\\..\\..\\IO\\data\\rr_dist_filtered.csv");
            List<double> rrDistFilteredMatlab = new List<double>();
            foreach (string line in lines)
            {
                rrDistFilteredMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }

            int resampFreq = 1;
            List<List<double>> RRFiltered = new List<List<double>>();
            RRFiltered.Add(timeInSecFilteredMatlab);
            RRFiltered.Add(rrDistFilteredMatlab);

            //output
            lines = ReadFile("..\\..\\..\\IO\\data\\time_in_sec_resampled.csv");
            List<double> timeInSecResampledMatlab = new List<double>();
            foreach (string line in lines)
            {
                timeInSecResampledMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            lines = ReadFile("..\\..\\..\\IO\\data\\rr_dist_resampled.csv");
            List<double> rrDistResampledMatlab = new List<double>();
            foreach (string line in lines)
            {
                rrDistResampledMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }

            // Process test here
            List<List<double>> result = (List<List<double>>)obj.Invoke("resampling", RRFiltered, resampFreq);

            // Assert results
            Assert.AreEqual(timeInSecResampledMatlab.Count, result[0].Count);
            Assert.AreEqual(rrDistResampledMatlab.Count, result[1].Count);
            for (int i = 0; i < result[0].Count; i++)
            {
                Assert.AreEqual(timeInSecResampledMatlab[i], result[0][i], 1);
                Assert.AreEqual(rrDistResampledMatlab[i], result[1][i], 1);
            }
        }

        [TestMethod]
        [Description("Test highpass")]
        public void Sleep_Apnea_HP_Test1()
        {
            // Init test here
            Sleep_Apnea_Params testParams = new Sleep_Apnea_Params("Test");
            Sleep_Apnea_Alg testAlgs = new Sleep_Apnea_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);

            // input
            List<string> lines = ReadFile("..\\..\\..\\IO\\data\\time_in_sec_resampled.csv");
            List<double> timeInSecResampledMatlab = new List<double>();
            foreach (string line in lines)
            {
                timeInSecResampledMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            lines = ReadFile("..\\..\\..\\IO\\data\\rr_dist_resampled.csv");
            List<double> rrDistResampledMatlab = new List<double>();
            foreach (string line in lines)
            {
                rrDistResampledMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }

            List<List<double>> RRResampled = new List<List<double>>();
            RRResampled.Add(timeInSecResampledMatlab);
            RRResampled.Add(rrDistResampledMatlab);

            // ouput
            lines = ReadFile("..\\..\\..\\IO\\data\\time_in_sec_HP.csv");
            List<double> timeInSecHPMatlab = new List<double>();
            foreach (string line in lines)
            {
                timeInSecHPMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            lines = ReadFile("..\\..\\..\\IO\\data\\rr_dist_HP.csv");
            List<double> rrDistHPMatlab = new List<double>();
            foreach (string line in lines)
            {
                rrDistHPMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }

            // Process test here
            List<List<double>> result = (List<List<double>>)obj.Invoke("HP", RRResampled);

            // Assert results
            Assert.AreEqual(timeInSecHPMatlab.Count, result[0].Count);
            Assert.AreEqual(rrDistHPMatlab.Count, result[1].Count);
            for (int i = 0; i < result[0].Count; i++)
            {
                Assert.AreEqual(timeInSecHPMatlab[i], result[0][i], 1);
                Assert.AreEqual(rrDistHPMatlab[i], result[1][i], 1);
            }
        }




        List<string> ReadFile(string name)
        {
            List<string> lines = new List<string>();
            var reader = new StreamReader(File.OpenRead(name));
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                lines.Add(line.Replace('.', ','));
            }
            return lines;
        }

    }
}
