using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using EKG_Project.Modules.Flutter;


namespace EKG_Unit.Modules.Flutter
{
    [TestClass]
    public class Flutter_Alg_Test
    {
        [TestMethod]
        [Description("Should return correct ecg parts using qrsonsets and tends")]
        public void GetEcgPart_test1()
        {
            // Init 
            double[] samplestmp = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4 };
            Vector<double> samples = Vector<double>.Build.DenseOfArray(samplestmp);
            List<int> tends = new List<int>() { 1, 10, 19 };
            List<int> qrsonsets = new List<int>() { 4, 13, 29 };

            List<double[]> result = new List<double[]>()
            {
                new double[] {1, 2, 3, 4},
                new double[] {0, 1, 2, 3},
                new double[] {9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9}
            };

            Flutter_Alg flutter = new Flutter_Alg(tends, qrsonsets, samples, 360);
            PrivateObject obj = new PrivateObject(flutter);

            // Act
            List<double[]> t2qrsEkgParts = (List<double[]>)obj.Invoke("GetEcgPart");

            // Assert results
            CollectionAssert.AreEqual(result[0], t2qrsEkgParts[0]);
            CollectionAssert.AreEqual(result[1], t2qrsEkgParts[1]);
            CollectionAssert.AreEqual(result[2], t2qrsEkgParts[2]);
        }

        [TestMethod]
        [Description("Should return empty list if qrs onset and tends are empty")]
        public void GetEcgPart_test2()
        {
            // Init 
            double[] samplestmp = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4 };
            Vector<double> samples = Vector<double>.Build.DenseOfArray(samplestmp);
            List<int> tends = new List<int>();
            List<int> qrsonsets = new List<int>();

            Flutter_Alg flutter = new Flutter_Alg(tends, qrsonsets, samples, 360);
            PrivateObject obj = new PrivateObject(flutter);

            // Act
            List<double[]> t2qrsEkgParts = (List<double[]>)obj.Invoke("GetEcgPart");

            // Assert results
            Assert.AreEqual(0, t2qrsEkgParts.Count);
        }

        [TestMethod]
        [Description("Should return correct ecg parts even if waves were not correctly detected")]
        public void GetEcgPart_test3()
        {
            // Init 
            double[] samplestmp = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4 };
            Vector<double> samples = Vector<double>.Build.DenseOfArray(samplestmp);
            List<int> tends = new List<int>() { 1, -1, 19, 21 };
            List<int> qrsonsets = new List<int>() { 4, 13, -1, 30 };

            List<double[]> result = new List<double[]>()
            {
                new double[] {1, 2, 3, 4},
                new double[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 0}
            };

            Flutter_Alg flutter = new Flutter_Alg(tends, qrsonsets, samples, 360);
            PrivateObject obj = new PrivateObject(flutter);

            // Act
            List<double[]> t2qrsEkgParts = (List<double[]>)obj.Invoke("GetEcgPart");

            // Assert results
            CollectionAssert.AreEqual(result[0], t2qrsEkgParts[0]);
            CollectionAssert.AreEqual(result[1], t2qrsEkgParts[1]);
        }

        [TestMethod]
        [Description("Should return correct ecg parts when qrsonset starts before tend")]
        public void GetEcgPart_test4()
        {
            // Init 
            double[] samplestmp = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4 };
            Vector<double> samples = Vector<double>.Build.DenseOfArray(samplestmp);
            List<int> tends = new List<int>() { 2, -1, 19, 21 };
            List<int> qrsonsets = new List<int>() { 1, 4, 13, -1, 30 };

            List<double[]> result = new List<double[]>()
            {
                new double[] {2, 3, 4},
                new double[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 0}
            };

            Flutter_Alg flutter = new Flutter_Alg(tends, qrsonsets, samples, 360);
            PrivateObject obj = new PrivateObject(flutter);

            // Act
            List<double[]> t2qrsEkgParts = (List<double[]>)obj.Invoke("GetEcgPart");

            // Assert results
            CollectionAssert.AreEqual(result[0], t2qrsEkgParts[0]);
            CollectionAssert.AreEqual(result[1], t2qrsEkgParts[1]);
        }

        [TestMethod]
        [Description("Should calculate spectral density")]
        public void CalculateSpectralDensity_test1()
        {
            // Init 
            List<double[]> ecgParts = new List<double[]>()
            {
                new double[] {1, 0, -1, 0},
            };

            List<double[]> spectralDensity = new List<double[]>()
            {
                new double[] {0, 1, 0, 1},
            };

            Flutter_Alg flutter = new Flutter_Alg(null, null, null, 100);
            PrivateObject obj = new PrivateObject(flutter);

            // Act
            List<double[]> result = (List<double[]>)obj.Invoke("CalculateSpectralDensity", ecgParts);

            // Assert results
            CollectionAssert.AreEqual(result[0], spectralDensity[0]);
        }

        [TestMethod]
        [Description("Should return empty list if ecg parts are empty")]
        public void CalculateSpectralDensity_test2()
        {
            // Init 
            List<double[]> ecgParts = new List<double[]>();

            Flutter_Alg flutter = new Flutter_Alg(null, null, null, 100);
            PrivateObject obj = new PrivateObject(flutter);

            // Act
            List<double[]> result = (List<double[]>)obj.Invoke("CalculateSpectralDensity", ecgParts);

            // Assert results
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        [Description("Should calculate appropriate frequencies for spectral density")]
        public void CalculateFrequenciesAxis_test1()
        {
            // Init 
            int fs = 100;
            List<double[]> spectralDensity = new List<double[]>()
            {
                new double[] {0, 1, 0, 1},
            };

            List<double[]> frequencies = new List<double[]>()
            {
                new double[] {0, 25, 50, 75},
            };

            Flutter_Alg flutter = new Flutter_Alg(null, null, null, fs);
            PrivateObject obj = new PrivateObject(flutter);

            // Act
            List<double[]> result = (List<double[]>)obj.Invoke("CalculateFrequenciesAxis", spectralDensity);

            // Assert results
            CollectionAssert.AreEqual(result[0], frequencies[0]);
        }

        [TestMethod]
        [Description("Should return empty list if spectralDensity is empty")]
        public void CalculateFrequenciesAxis_test2()
        {
            // Init 
            int fs = 100;
            List<double[]> spectralDensity = new List<double[]>();

            Flutter_Alg flutter = new Flutter_Alg(null, null, null, fs);
            PrivateObject obj = new PrivateObject(flutter);

            // Act
            List<double[]> result = (List<double[]>)obj.Invoke("CalculateFrequenciesAxis", spectralDensity);

            // Assert results
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        [Description("Should trim spectrum to last freq less than given one")]
        public void TrimToGivenFreq_test1()
        {
            // Init 
            int fs = 100;
            List<double[]> spectralDensity = new List<double[]>()
            {
                new double[] {0, 1, 0, 1},
            };
            List<double[]> frequencies = new List<double[]>()
            {
                new double[] {0, 25, 50, 75},
            };

            List<double[]> spectralDensityExpectedResult = new List<double[]>()
            {
                new double[] {0, 1},
            };
            List<double[]> frequenciesExpectedResult = new List<double[]>()
            {
                new double[] {0, 25},
            };

            Flutter_Alg flutter = new Flutter_Alg(null, null, null, fs);
            PrivateObject obj = new PrivateObject(flutter);

            // Act
            obj.Invoke("TrimToGivenFreq", spectralDensity, frequencies, 40.0);

            // Assert results
            CollectionAssert.AreEqual(spectralDensityExpectedResult[0], spectralDensity[0]);
            CollectionAssert.AreEqual(frequenciesExpectedResult[0], frequencies[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [Description("Should throw exception if given trim freq is greastes than the highest one in spectrum")]
        public void TrimToGivenFreq_test2()
        {
            // Init 
            int fs = 100;
            List<double[]> spectralDensity = new List<double[]>()
            {
                new double[] {0, 1, 0, 1},
            };
            List<double[]> frequencies = new List<double[]>()
            {
                new double[] {0, 25, 50, 75},
            };

            Flutter_Alg flutter = new Flutter_Alg(null, null, null, fs);
            PrivateObject obj = new PrivateObject(flutter);

            // Act
            obj.Invoke("TrimToGivenFreq", spectralDensity, frequencies, 140.0);
        }

        [TestMethod]
        [Description("Should interpolate spectral density")]
        public void InterpolateSpectralDensity_test1()
        {
            // Init 
            int fs = 100;
            List<double[]> spectralDensity = new List<double[]>()
            {
                new double[] {0, 1, 2, 3}
            };
            List<double[]> frequencies = new List<double[]>()
            {
                new double[] {0, 10, 20, 30}
            };

            List<double[]> spectralDensityExpectedResult = new List<double[]>()
            {
                new double[] {0,0.1,0.2,0.3,0.4,0.5,0.6,0.7,0.8,0.9,
                            1.0,1.1,1.2,1.3,1.4,1.5,1.6,1.7,1.8,1.9,
                            2.0,2.1,2.2,2.3,2.4,2.5,2.6,2.7,2.8,2.9,3.0}
            };
            List<double[]> frequenciesExpectedResult = new List<double[]>()
            {
                new double[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
                             10,11,12,13,14,15,16,17,18,19,
                             20,21,22,23,24,25,26,27,28,29,30}
            };

            Flutter_Alg flutter = new Flutter_Alg(null, null, null, fs);
            PrivateObject obj = new PrivateObject(flutter);

            // Act
            obj.Invoke("InterpolateSpectralDensity", spectralDensity, frequencies, 1);

            // Assert results
            for (int i = 0; i < spectralDensityExpectedResult[0].Length; i++)
            {
                Assert.AreEqual(spectralDensityExpectedResult[0][i], spectralDensity[0][i], 0.0001);
                Assert.AreEqual(frequenciesExpectedResult[0][i], frequencies[0][i], 0.0001);
            }
        }

        [TestMethod]
        [Description("Should calculate spectrum's power")]
        public void CalculateIntegralForEachSpectrum_test1()
        {
            // Init 
            int fs = 100;
            List<double[]> spectralDensity = new List<double[]>()
            {
                new double[] {0, 1, 2, 1, 0}
            };
            List<double[]> frequencies = new List<double[]>()
            {
                new double[] {0, 1, 2, 3}
            };

            List<double> power = new List<double>() { 3.5 };

            Flutter_Alg flutter = new Flutter_Alg(null, null, null, fs);
            PrivateObject obj = new PrivateObject(flutter);

            // Act
            List<double> result = (List<double>)obj.Invoke("CalculateIntegralForEachSpectrum", frequencies, spectralDensity);

            // Assert results
            Assert.AreEqual(power[0], result[0]);
        }

        [TestMethod]
        [Description("Should detect flutter")]
        public void Detect_test1()
        {
            // Init 
            int fs = 100;
            List<double[]> spectralDensity = new List<double[]>()
            {
                new double[] {0, 2, 10, 1, 0, 0, 0, 0, 0, 0},
                new double[] {1, 2, 3, 2, 1, 0, 0, 0, 0, 0},
                new double[] {0, 1, 2, 3, 4, 3, 2, 1, 1, 0},
            };
            List<double[]> frequencies = new List<double[]>()
            {
                new double[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9},
                new double[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9},
                new double[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}
            };
            List<double> power = new List<double>() { 13.0, 8.5, 17.0};

            List<int> _ecgPartStarts = new List<int>() { 1, 5, 9 };
            List<int> _ecgPartEnds = new List<int>() {3, 7, 11 };

            List<Tuple<int, int>> detected = new List<Tuple<int, int>>()
            {
                new Tuple<int, int>(_ecgPartStarts[0], _ecgPartEnds[1]),
            };

            Flutter_Alg flutter = new Flutter_Alg(null, null, null, fs);
            PrivateObject obj = new PrivateObject(flutter);
            obj.SetField("_ecgPartStarts", _ecgPartStarts);
            obj.SetField("_ecgPartEnds", _ecgPartEnds);

            // Act
            List<Tuple<int, int>> result = (List<Tuple<int, int>>)obj.Invoke("Detect", spectralDensity, frequencies, power);

            // Assert results
            Assert.AreEqual(detected[0].Item1, result[0].Item1);
            Assert.AreEqual(detected[0].Item2, result[0].Item2);
        }


    }
}