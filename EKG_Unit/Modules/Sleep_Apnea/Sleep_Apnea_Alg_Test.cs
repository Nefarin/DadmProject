using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.Sleep_Apnea;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using MathNet.Filtering.Median;
using MathNet.Numerics.IntegralTransforms;
using System.Linq;
using System.Numerics;
using EKG_Project.IO;

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
            DebugECGPath debugECGPath = new DebugECGPath();
            Sleep_Apnea_Params testParams = new Sleep_Apnea_Params("Test");
            Sleep_Apnea_Alg testAlgs = new Sleep_Apnea_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);

            // input
            string rpeaksPath = Path.Combine(debugECGPath.getDataPath(), "rpeaks.csv");
            List<string> lines = ReadFile(rpeaksPath);
            List<uint> R_detected = new List<uint>();
            foreach (string line in lines)
            {
                R_detected.Add((uint)decimal.Parse(line, NumberStyles.Float));
            }
            int fs = 100;

            // ouput
            string timeInSecPath = Path.Combine(debugECGPath.getDataPath(), "time_in_sec.csv");
            lines = ReadFile(timeInSecPath);
            List<double> timeInSecMatlab = new List<double>();
            foreach (string line in lines)
            {
                timeInSecMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            string rrDist = Path.Combine(debugECGPath.getDataPath(), "rr_dist.csv");
            lines = ReadFile(rrDist);
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
                Assert.AreEqual(timeInSecMatlab[i], result[0][i], 0.0000001);
                Assert.AreEqual(rrDistMatlab[i], result[1][i], 0.0000001);
            }
        }

        [TestMethod]
        [Description("Test average filter")]
        public void Sleep_Apnea_average_filter_Test1()
        {
            // Init test here
            DebugECGPath debugECGPath = new DebugECGPath();
            Sleep_Apnea_Params testParams = new Sleep_Apnea_Params("Test");
            Sleep_Apnea_Alg testAlgs = new Sleep_Apnea_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);

            // input
            string timeInSec = Path.Combine(debugECGPath.getDataPath(), "time_in_sec.csv");
            List<string> lines = ReadFile(timeInSec);
            List<double> timeInSecMatlab = new List<double>();
            foreach (string line in lines)
            {
                timeInSecMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            string rrDist = Path.Combine(debugECGPath.getDataPath(), "rr_dist.csv");
            lines = ReadFile(rrDist);
            List<double> rrDistMatlab = new List<double>();
            foreach (string line in lines)
            {
                rrDistMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            List<List<double>> RR = new List<List<double>>();
            RR.Add(timeInSecMatlab);
            RR.Add(rrDistMatlab);

            // output
            string timeInSecFiltered = Path.Combine(debugECGPath.getDataPath(), "time_in_sec_filtered.csv");
            lines = ReadFile(timeInSecFiltered);
            List<double> timeInSecFilteredMatlab = new List<double>();
            foreach (string line in lines)
            {
                timeInSecFilteredMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            string rrDistFiltered = Path.Combine(debugECGPath.getDataPath(), "rr_dist_filtered.csv");
            lines = ReadFile(rrDistFiltered);
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
                Assert.AreEqual(timeInSecFilteredMatlab[i], result[0][i], 0.000001);
                Assert.AreEqual(rrDistFilteredMatlab[i], result[1][i], 0.000001);
            }
        }

        [TestMethod]
        [Description("Test resampling")]
        public void Sleep_Apnea_resampling_Test1()
        {
            // Init test here
            DebugECGPath debugECGPath = new DebugECGPath();
            Sleep_Apnea_Params testParams = new Sleep_Apnea_Params("Test");
            Sleep_Apnea_Alg testAlgs = new Sleep_Apnea_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);

            // input
            string timeInSecFiltered = Path.Combine(debugECGPath.getDataPath(), "time_in_sec_filtered.csv");
            List<string> lines = ReadFile(timeInSecFiltered);
            List<double> timeInSecFilteredMatlab = new List<double>();
            foreach (string line in lines)
            {
                timeInSecFilteredMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            string rrDistFiltered = Path.Combine(debugECGPath.getDataPath(), "rr_dist_filtered.csv");
            lines = ReadFile(rrDistFiltered);
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
            string timeInSecResampled = Path.Combine(debugECGPath.getDataPath(), "time_in_sec_resampled.csv");
            lines = ReadFile(timeInSecResampled);
            List<double> timeInSecResampledMatlab = new List<double>();
            foreach (string line in lines)
            {
                timeInSecResampledMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            string rrDistResampled = Path.Combine(debugECGPath.getDataPath(), "rr_dist_resampled.csv");
            lines = ReadFile(rrDistResampled);
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
                Assert.AreEqual(timeInSecResampledMatlab[i], result[0][i], 0.00001);
                Assert.AreEqual(rrDistResampledMatlab[i], result[1][i], 0.00001);
            }
        }

        [TestMethod]
        [Description("Test highpass")]
        public void Sleep_Apnea_HP_Test1()
        {
            // Init test here
            DebugECGPath debugECGPath = new DebugECGPath();
            Sleep_Apnea_Params testParams = new Sleep_Apnea_Params("Test");
            Sleep_Apnea_Alg testAlgs = new Sleep_Apnea_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);

            // input
            string timeInSecResampled = Path.Combine(debugECGPath.getDataPath(), "time_in_sec_resampled.csv");
            List<string> lines = ReadFile(timeInSecResampled);
            List<double> timeInSecResampledMatlab = new List<double>();
            foreach (string line in lines)
            {
                timeInSecResampledMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            string rrDistResampled = Path.Combine(debugECGPath.getDataPath(), "rr_dist_resampled.csv");
            lines = ReadFile(rrDistResampled);
            List<double> rrDistResampledMatlab = new List<double>();
            foreach (string line in lines)
            {
                rrDistResampledMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }

            List<List<double>> RRResampled = new List<List<double>>();
            RRResampled.Add(timeInSecResampledMatlab);
            RRResampled.Add(rrDistResampledMatlab);

            // ouput
            string timeInSecHP = Path.Combine(debugECGPath.getDataPath(), "time_in_sec_HP.csv");
            lines = ReadFile(timeInSecHP);
            List<double> timeInSecHPMatlab = new List<double>();
            foreach (string line in lines)
            {
                timeInSecHPMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            string rrDistHP = Path.Combine(debugECGPath.getDataPath(), "rr_dist_HP.csv");
            lines = ReadFile(rrDistHP);
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
                Assert.AreEqual(timeInSecHPMatlab[i], result[0][i], 0.0001);
                Assert.AreEqual(rrDistHPMatlab[i], result[1][i], 0.0001);
            }
        }

        [TestMethod]
        [Description("Test lowpass")]
        public void Sleep_Apnea_LP_Test1()
        {
            // Init test here
            DebugECGPath debugECGPath = new DebugECGPath();
            Sleep_Apnea_Params testParams = new Sleep_Apnea_Params("Test");
            Sleep_Apnea_Alg testAlgs = new Sleep_Apnea_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);

            // input
            string timeInSecHP = Path.Combine(debugECGPath.getDataPath(), "time_in_sec_HP.csv");
            List<string> lines = ReadFile(timeInSecHP);
            List<double> timeInSecHPMatlab = new List<double>();
            foreach (string line in lines)
            {
                timeInSecHPMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            string rrDistHP = Path.Combine(debugECGPath.getDataPath(), "rr_dist_HP.csv");
            lines = ReadFile(rrDistHP);
            List<double> rrDistHPMatlab = new List<double>();
            foreach (string line in lines)
            {
                rrDistHPMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }

            List<List<double>> RRHP = new List<List<double>>();
            RRHP.Add(timeInSecHPMatlab);
            RRHP.Add(rrDistHPMatlab);

            // ouput
            string timeInSecLP = Path.Combine(debugECGPath.getDataPath(), "time_in_sec_LP.csv");
            lines = ReadFile(timeInSecLP);
            List<double> timeInSecLPMatlab = new List<double>();
            foreach (string line in lines)
            {
                timeInSecLPMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            string rrDistLP = Path.Combine(debugECGPath.getDataPath(), "rr_dist_LP.csv");
            lines = ReadFile(rrDistLP);
            List<double> rrDistLPMatlab = new List<double>();
            foreach (string line in lines)
            {
                rrDistLPMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }

            // Process test here
            List<List<double>> result = (List<List<double>>)obj.Invoke("LP", RRHP);

            // Assert results
            Assert.AreEqual(timeInSecLPMatlab.Count, result[0].Count);
            Assert.AreEqual(rrDistLPMatlab.Count, result[1].Count);
            for (int i = 0; i < result[0].Count; i++)
            {
                Assert.AreEqual(timeInSecLPMatlab[i], result[0][i], 0.0001);
                Assert.AreEqual(rrDistLPMatlab[i], result[1][i], 0.0001);
            }
        }

        [TestMethod]
        [Description("Test ampNormalization")]
        public void Sleep_Apnea_amp_normalization_Test1()
        {
            // Init test here
            DebugECGPath debugECGPath = new DebugECGPath();
            Sleep_Apnea_Params testParams = new Sleep_Apnea_Params("Test");
            Sleep_Apnea_Alg testAlgs = new Sleep_Apnea_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);

            // input
            string timeInSec = Path.Combine(debugECGPath.getDataPath(), "h_time_in_sec.csv");
            List<string> lines = ReadFile(timeInSec);
            List<double> hTimeInSec = new List<double>();
            foreach (string line in lines)
            {
                hTimeInSec.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            string hAmpFilt = Path.Combine(debugECGPath.getDataPath(), "h_amp_filtered.csv");
            lines = ReadFile(hAmpFilt);
            List <double> hAmpFiltered = new List<double>();
            foreach (string line in lines)
            {
                hAmpFiltered.Add((double)decimal.Parse(line, NumberStyles.Float));
            }

            List<List<double>> hAmp = new List<List<double>>();
            hAmp.Add(hTimeInSec);
            hAmp.Add(hAmpFiltered);

            // ouput
            string hAmpNorm = Path.Combine(debugECGPath.getDataPath(), "h_amp_normalised.csv");
            lines = ReadFile(hAmpNorm);
            List<double> hAmpNormalisedMatlab = new List<double>();
            foreach (string line in lines)
            {
                hAmpNormalisedMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }

            // Process test here
            obj.Invoke("ampNormalization", hAmp);

            // Assert results
            Assert.AreEqual(hAmpNormalisedMatlab.Count, hAmp[1].Count);
            Assert.AreEqual(hTimeInSec.Count, hAmp[0].Count);
            for (int i = 0; i < hAmp[1].Count; i++)
            {
                Assert.AreEqual(hAmpNormalisedMatlab[i], hAmp[1][i], 0.00001);
                Assert.AreEqual(hTimeInSec[i], hAmp[0][i], 0.00001);
            }
        }

        [TestMethod]
        [Description("Test detectApnea")]
        public void Sleep_Apnea_detect_apnea_Test1()
        {
            // Init test here
            DebugECGPath debugECGPath = new DebugECGPath();
            Sleep_Apnea_Params testParams = new Sleep_Apnea_Params("Test");
            Sleep_Apnea_Alg testAlgs = new Sleep_Apnea_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);

            // input
            string timeInSec = Path.Combine(debugECGPath.getDataPath(), "h_time_in_sec.csv");
            List<string> lines = ReadFile(timeInSec);
            List<double> hTimeInSec = new List<double>();
            foreach (string line in lines)
            {
                hTimeInSec.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            string hAmpNorm = Path.Combine(debugECGPath.getDataPath(), "h_amp_normalised.csv");
            lines = ReadFile(hAmpNorm);
            List<double> hAmpNormalised = new List<double>();
            foreach (string line in lines)
            {
                hAmpNormalised.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            string hFreqFilt = Path.Combine(debugECGPath.getDataPath(), "h_freq_filtered.csv");
            lines = ReadFile(hFreqFilt);
            List<double> hFreqFiltered = new List<double>();
            foreach (string line in lines)
            {
                hFreqFiltered.Add((double)decimal.Parse(line, NumberStyles.Float));
            }

            List<List<double>> hAmp = new List<List<double>>();
            hAmp.Add(hTimeInSec);
            hAmp.Add(hAmpNormalised);
            List<List<double>> hFreq = new List<List<double>>();
            hFreq.Add(hTimeInSec);
            hFreq.Add(hFreqFiltered);

            // ouput
            string det = Path.Combine(debugECGPath.getDataPath(), "detected.csv");
            lines = ReadFile(det);
            List<bool> detectedMatlab = new List<bool>();
            foreach (string line in lines)
            {
                detectedMatlab.Add(int.Parse(line) == 0 ? false : true);
            }
            string algAnn = Path.Combine(debugECGPath.getDataPath(), "alg_ann.csv");
            lines = ReadFile(algAnn);
            List<double> algAnnMatlab = new List<double>();
            foreach (string line in lines)
            {
                algAnnMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            List<bool> detected = new List<bool>();
            List<double> time = new List<double>(); //Ticking away the moments that make up a dull day 

            // Process test here
            obj.Invoke("detectApnea", hAmp, hFreq, detected, time);

            // Assert results
            Assert.AreEqual(detectedMatlab.Count, detected.Count);
            Assert.AreEqual(algAnnMatlab.Count, time.Count);
            for (int i = 0; i < detected.Count; i++)
            {
                Assert.AreEqual(detectedMatlab[i], detected[i]);
                Assert.AreEqual(algAnnMatlab[i], time[i], 1.0);
            }
        }

        [TestMethod]
        [Description("Test hilbert")]
        public void Sleep_Apnea_hilbert_Test1()
        {
            // Init test here
            DebugECGPath debugECGPath = new DebugECGPath();
            Sleep_Apnea_Params testParams = new Sleep_Apnea_Params("Test");
            Sleep_Apnea_Alg testAlgs = new Sleep_Apnea_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);

            // input
            string timeInSecLP = Path.Combine(debugECGPath.getDataPath(), "time_in_sec_LP.csv");
            List<string> lines = ReadFile(timeInSecLP);
            List<double> timeInSecLPMatlab = new List<double>();
            foreach (string line in lines)
            {
                timeInSecLPMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            string rrDistLP = Path.Combine(debugECGPath.getDataPath(), "rr_dist_LP.csv");
            lines = ReadFile(rrDistLP);
            List<double> rrDistLPMatlab = new List<double>();
            foreach (string line in lines)
            {
                rrDistLPMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }

            List<List<double>> RRLP = new List<List<double>>();
            RRLP.Add(timeInSecLPMatlab);
            RRLP.Add(rrDistLPMatlab);

            // ouput
            string amp = Path.Combine(debugECGPath.getDataPath(), "h_amp.csv");
            lines = ReadFile(amp);
            List<double> hAmpMatlab = new List<double>();
            foreach (string line in lines)
            {
                hAmpMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            string freq = Path.Combine(debugECGPath.getDataPath(), "h_freq.csv");
            lines = ReadFile(freq);
            List<double> hFreqMatlab = new List<double>();
            foreach (string line in lines)
            {
                hFreqMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }

            List<List<double>> hAmp = new List<List<double>>();
            List<List<double>> hFreq = new List<List<double>>();

            // Process test here
            obj.Invoke("hilbert", RRLP, hAmp, hFreq);

            // Assert results
            Assert.AreEqual(hAmpMatlab.Count, hAmp[1].Count);
            Assert.AreEqual(timeInSecLPMatlab.Count - 1, hAmp[0].Count);

            for (int i = 0; i < hAmp[1].Count; i++)
            {
                Assert.AreEqual(hAmpMatlab[i], hAmp[1][i], 0.00001);
                Assert.AreEqual(timeInSecLPMatlab[i], hAmp[0][i], 1);
                Assert.AreEqual(hFreqMatlab[i], hFreq[1][i], 0.00001);
                Assert.AreEqual(timeInSecLPMatlab[i], hFreq[0][i], 1);
            }
        }


        [TestMethod]
        [Description("Test hilbertMatlab")]
        public void Sleep_Apnea_hilbertMatlab_Test1()
        {
            // Init test here
            DebugECGPath debugECGPath = new DebugECGPath();
            Sleep_Apnea_Params testParams = new Sleep_Apnea_Params("Test");
            Sleep_Apnea_Alg testAlgs = new Sleep_Apnea_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);

            // input
            string rrBeforeHilbert = Path.Combine(debugECGPath.getDataPath(), "rr_before_hilbert.csv");
            List<string> lines = ReadFile(rrBeforeHilbert);
            List<double> rrs = new List<double>();
            foreach (string line in lines)
            {
                rrs.Add((double)decimal.Parse(line, NumberStyles.Float));
            }

            // ouput
            string hilbResReal = Path.Combine(debugECGPath.getDataPath(), "hilb_result_real.csv");
            lines = ReadFile(hilbResReal);
            List<double> realMatlab = new List<double>();
            foreach (string line in lines)
            {
                realMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            string hilbResImag = Path.Combine(debugECGPath.getDataPath(), "hilb_result_imag.csv");
            lines = ReadFile(hilbResImag);
            List<double> imagMatlab = new List<double>();
            foreach (string line in lines)
            {
                imagMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }

            // Process test here
            Complex[] result = (Complex[])obj.Invoke("MatlabHilbert", rrs.ToArray());

            for (int i = 0; i < result.Length; i++)
            {
                Assert.AreEqual(realMatlab[i], result[i].Real, 0.0001);
                Assert.AreEqual(imagMatlab[i], result[i].Imaginary, 0.0001);
            }
        }

        [TestMethod]
        [Description("only for checking sensitivity and pp")]
        public void Alg_Test()
        {
            // Init test here
            DebugECGPath debugECGPath = new DebugECGPath();
            Sleep_Apnea_Params testParams = new Sleep_Apnea_Params("Test");
            Sleep_Apnea_Alg testAlgs = new Sleep_Apnea_Alg();
            PrivateObject obj = new PrivateObject(testAlgs);

            // input
            string rpeaksa03 = Path.Combine(debugECGPath.getDataPath(), "rpeaks_a03.csv");
            List<string> lines = ReadFile(rpeaksa03);
            List<uint> rpeaks = new List<uint>();
            foreach (string line in lines)
            {
                rpeaks.Add((uint)decimal.Parse(line, NumberStyles.Float));
            }
            int fs = 100;
            int resampFreq = 1;
            List<List<double>> hAmp = new List<List<double>>();
            List<List<double>> hFreq = new List<List<double>>();
            List<bool> detected = new List<bool>();
            List<double> time = new List<double>();

            // ouput
            string algAnn = Path.Combine(debugECGPath.getDataPath(), "alg_ann_a03.csv");
            lines = ReadFile(algAnn);
            List<double> timeMatlab = new List<double>();
            foreach (string line in lines)
            {
                timeMatlab.Add((double)decimal.Parse(line, NumberStyles.Float));
            }
            string det = Path.Combine(debugECGPath.getDataPath(), "detected_a03.csv");
            lines = ReadFile(det);
            List<bool> detectedMatlab = new List<bool>();
            foreach (string line in lines)
            {
                detectedMatlab.Add(int.Parse(line) == 0 ? false : true);
            }

            // Process test here
            List<List<double>> RR = (List<List<double>>)obj.Invoke("findIntervals", rpeaks, fs);
            List<List<double>> RRFiltered = (List<List<double>>)obj.Invoke("averageFilter", RR);
            List<List<double>> RRResampled = (List<List<double>>)obj.Invoke("resampling", RRFiltered, resampFreq);
            List<List<double>> RRHP = (List<List<double>>)obj.Invoke("HP", RRResampled);
            List<List<double>> RRLP = (List<List<double>>)obj.Invoke("LP", RRHP);
            obj.Invoke("hilbert", RRLP, hAmp, hFreq);
            obj.Invoke("medianFilter", hFreq, hAmp);
            obj.Invoke("ampNormalization", hAmp);
            obj.Invoke("detectApnea", hAmp, hFreq, detected, time);

            int TP = 0;
            int FN = 0;
            int FP = 0;

            for (int i = 0; i < detected.Count && i < detectedMatlab.Count; i++)
            {
                if (detected[i] && detectedMatlab[i])
                {
                    TP++;
                }
                else if (detected[i] && !detectedMatlab[i])
                {
                    FP++;
                }
                else if (!detected[i] && detectedMatlab[i])
                {
                    FN++;
                }
            }

            double Se = ((double)TP) / (TP + FN);
            double PP = ((double)TP) / (TP + FP);

            Assert.IsTrue(true);
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
