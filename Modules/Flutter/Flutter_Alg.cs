using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.Flutter
{
    public class Flutter_Alg
    {
        private List<int> _QRSonsets; // początek zespołu qrs
        private List<int> _Tends; // lista numerów próbek gdzie kończy się fala T
        private Vector<double> _samples; // próbki sygnału
        private double _fs;

        List<int> _ecgPartStarts = new List<int>();
        List<int> _ecgPartEnds = new List<int>();

        private const double RI_LOWER_LIMIT_FOR_AFL = 0.3;
        private const double RI_UPPER_LIMIT_FOR_AFL = 1.1;

        public Flutter_Alg(List<int> tEnds, List<int> qrsOnsets,
            Vector<double> samples, double fs)
        {
            _Tends = tEnds;
            _QRSonsets = qrsOnsets;
            _samples = samples;
            _fs = fs;
        }

        public Flutter_Alg()
        {
        }

        public List<Tuple<int, int>> DetectAtrialFlutter()
        {
            List<double[]> t2qrsEkgParts = GetEcgPart();
            List<double[]> spectralDensityList = CalculateSpectralDensity(t2qrsEkgParts);
            List<double[]> frequenciesList = CalculateFrequenciesAxis(spectralDensityList);
            TrimToGivenFreq(spectralDensityList, frequenciesList, 70.0);
            InterpolateSpectralDensity(spectralDensityList, frequenciesList, 0.01);
            List<double> powerList = CalculateIntegralForEachSpectrum(frequenciesList, spectralDensityList);
            List<Tuple<int, int>> aflAnnotations = Detect(spectralDensityList, frequenciesList, powerList);

            return aflAnnotations;
        }

        #region
        /// <summary>
        /// Function that detects AFL based on regularity index RI which is defined as the bandwidth about max frequency, in which 50% of the total power is contained
        /// </summary>
        /// <param name="spectralDensityList"> List of trimmed spectral density </param>
        /// <param name="frequenciesList"> List of frequencies </param>
        /// <param name="powerList"> List of spectrum power </param>
        /// <returns name="aflAnnotations"> List of tuple containing the beginnings and ends of the segments (as the sample number), which represent atrial flutter </returns>
        #endregion

        public List<Tuple<int, int>> Detect(List<double[]> spectralDensityList, List<double[]> frequenciesList, List<double> powerList)
        {
            bool[] aflDetected = new bool[spectralDensityList.Count];

            for (int i = 0; i < spectralDensityList.Count; i++)
            {
                if (spectralDensityList[i].Length == 0)
                {
                    continue;
                }
                double maxValue = spectralDensityList[i].Max();
                int maxValueIndex = Array.IndexOf(spectralDensityList[i], maxValue);
                if (maxValueIndex == 0 || maxValueIndex == spectralDensityList[i].Length)
                {
                    continue;
                }
                double freqForMaxValue = frequenciesList[i][maxValueIndex];

                double RIPower = 0;
                int j = 0;

                int leftIndex = maxValueIndex - 1;
                int rightIndex = maxValueIndex + 1;
                while (RIPower < 0.5 * powerList[i])
                {
                    if (j % 2 == 0)
                    {
                        double a = spectralDensityList[i][leftIndex];
                        double b = spectralDensityList[i][leftIndex + 1];
                        double h = frequenciesList[i][leftIndex + 1] - frequenciesList[i][leftIndex];
                        RIPower += (a + b) * h * 0.5;
                        leftIndex--;
                    }
                    else
                    {
                        double a = spectralDensityList[i][rightIndex];
                        double b = spectralDensityList[i][rightIndex - 1];
                        double h = frequenciesList[i][rightIndex] - frequenciesList[i][rightIndex - 1];
                        RIPower += (a + b) * h * 0.5;
                        rightIndex++;
                    }

                    if (leftIndex <= 0 && rightIndex >= frequenciesList[i].Length - 1)
                    {
                        break;
                    }

                    if (leftIndex <= 0)
                    {
                        j = 1;
                    }
                    else if (rightIndex >= frequenciesList[i].Length - 1)
                    {
                        j = 2;
                    }
                    else
                    {
                        j++;
                    }

                }

                double RI = frequenciesList[i][rightIndex-1] - frequenciesList[i][leftIndex+1];

                if (RI > RI_LOWER_LIMIT_FOR_AFL && RI < RI_UPPER_LIMIT_FOR_AFL)
                {
                    aflDetected[i] = true;
                }
            }

            int start = -1;
            List<Tuple<int, int>> aflAnnotations = new List<Tuple<int, int>>();
            for (int i = 0; i < aflDetected.Length; i++)
            {
                if (aflDetected[i])
                {
                    if (start == -1)
                    {
                        start = i;
                    }
                    else if (start != -1 && i == aflDetected.Length - 1)
                    {
                        aflAnnotations.Add(new Tuple<int, int>(_ecgPartStarts[start], _ecgPartEnds[i]));
                    }
                }
                else
                {
                    if (start != -1)
                    {
                        aflAnnotations.Add(new Tuple<int, int>(_ecgPartStarts[start], _ecgPartEnds[i - 1]));
                        start = -1;
                    }
                }
            }
            return aflAnnotations;
        }

        #region
        /// <summary>
        /// Function that calculates integral (power) for each spectrum using trapezoidal rule to approximating the definite integral
        /// </summary>
        /// <param name="spectralDensityList"> List of trimmed spectral density </param>
        /// <param name="frequenciesList"> List of frequencies </param>
        /// <returns> List of power for each spectrum </returns>
        #endregion

        public List<double> CalculateIntegralForEachSpectrum(List<double[]> frequenciesList, List<double[]> spectralDensityList)
        {
            List<double> powerList = new List<double>(frequenciesList.Count);
            for (int i = 0; i < frequenciesList.Count; i++)
            {
                double sum = 0;
                for (int j = 0; j < frequenciesList[i].Length - 1; j++)
                {
                    double a = spectralDensityList[i][j];
                    double b = spectralDensityList[i][j + 1];
                    double h = frequenciesList[i][j + 1] - frequenciesList[i][j];
                    sum += (a + b) * h * 0.5;
                }
                powerList.Add(sum);
            }
            return powerList;
        }

        #region
        /// <summary>
        /// Function that interpolates spectral density and frequencies (calculates the value between the two known values) using linear function
        /// </summary>
        /// <param name="spectralDensityList"> List of trimmed spectral density </param>
        /// <param name="frequenciesList"> List of frequencies </param>
        /// <param name="step"> Value of interpolation step </param>
        /// <returns> Interpolated lists of spectral density and frequencies </returns>
        #endregion

        public void InterpolateSpectralDensity(List<double[]> spectralDensityList, List<double[]> frequenciesList, double step)
        {
            for (int i = 0; i < spectralDensityList.Count; i++)
            {
                List<double> interpolatedFreq = new List<double>((int)Math.Ceiling((frequenciesList[i].Last() - frequenciesList[i].First()) / step));
                List<double> interpolatedSpectralDensity = new List<double>((int)Math.Ceiling((frequenciesList[i].Last() - frequenciesList[i].First()) / step));
                double k = frequenciesList[i][0];
                for (int j = 0; j < spectralDensityList[i].Length - 1; j++)
                {
                    double y1 = spectralDensityList[i][j];
                    double y2 = spectralDensityList[i][j + 1];
                    double x1 = frequenciesList[i][j];
                    double x2 = frequenciesList[i][j + 1];

                    double a = (y2 - y1) / (x2 - x1);
                    double b = y1 - a * x1;

                    while (k <= x2)
                    {
                        interpolatedFreq.Add(k);
                        interpolatedSpectralDensity.Add(a * k + b);
                        k += step;
                    }
                }
                spectralDensityList[i] = interpolatedSpectralDensity.ToArray();
                frequenciesList[i] = interpolatedFreq.ToArray();
            }
        }

        #region
        /// <summary>
        /// Function that creates frequencies axis
        /// </summary>
        /// <param name="spectralDensityList"> List of trimmed spectral density </param>
        /// <returns> List of frequencies </returns>
        #endregion

        public List<double[]> CalculateFrequenciesAxis(List<double[]> spectralDensityList)
        {
            List<double[]> freqs = new List<double[]>();
            foreach (var spectralDensity in spectralDensityList)
            {
                double[] freq = new double[spectralDensity.Length];
                for (int i = 0; i < spectralDensity.Length; i++)
                {
                    freq[i] = (i * _fs) / spectralDensity.Length;
                }
                freqs.Add(freq);
            }
            return freqs;
        }

        #region
        /// <summary>
        /// Function that cuts spectral density and frequencies to the appropriate range
        /// </summary>
        /// <param name="spectralDensityList"> List of spectral density </param>
        /// <param name="frequenciesList"> List of frequencies </param>
        /// <param name="trimFreq"> Cut-off frequency (frequencies above the cut-off value ​​are insignificant for further analysis) </param>
        /// <returns> Trimmed spectral density and frequencies to further analysis </returns>
        #endregion

        public void TrimToGivenFreq(List<double[]> spectralDensityList, List<double[]> frequenciesList, double trimFreq)
        {
            for (int i = 0; i < spectralDensityList.Count; i++)
            {
                double firstGTtrimFreq = frequenciesList[i].First(x => x > trimFreq);
                int n = Array.IndexOf(frequenciesList[i], firstGTtrimFreq);
                double[] trimmed = new double[n];
                double[] freqTrimmed = new double[n];
                for (int j = 0; j < n; j++)
                {
                    trimmed[j] = spectralDensityList[i][j];
                    freqTrimmed[j] = frequenciesList[i][j];
                }
                spectralDensityList[i] = trimmed;
                frequenciesList[i] = freqTrimmed;
            }
        }

        public List<double[]> TrimSpectralDensity(List<double[]> spectralDensityList, List<double[]> frequenciesList, double trimFreq)
        {
            List<double[]> trimmedSpectralDensityList = new List<double[]>(spectralDensityList.Count);
            foreach (var spectralDensity in spectralDensityList)
            {
                int n = (int)(trimFreq * spectralDensity.Length / _fs);
                double[] trimmed = new double[n];
                double[] freqTrimmed = new double[n];
                for (int i = 0; i < n; i++)
                {
                    trimmed[i] = spectralDensity[i];
                }
                trimmedSpectralDensityList.Add(trimmed);
            }
            return trimmedSpectralDensityList;
        }

        #region
        /// <summary>
        /// Function that calculates spectral density using forward Fast Fourier Transform (FFT)
        /// </summary>
        /// <param name="t2qrsEcgParts"> List of segments between QRSOnsets and Tends </param>
        /// <returns> Spectral density of the analyzed parts of the ECG signal </returns>
        #endregion

        public List<double[]> CalculateSpectralDensity(List<double[]> t2qrsEcgParts)
        {
            List<double[]> spectralDensity = new List<double[]>(t2qrsEcgParts.Count);
            foreach (var ecgPart in t2qrsEcgParts)
            {
                Complex[] result = ecgPart.Select(x => new Complex(x, 0)).ToArray();
                Fourier.Forward(result);
                double[] power = result.Select(x => (x * (new Complex(x.Real, -1 * x.Imaginary))).Real).ToArray();
                spectralDensity.Add(power);
            }
            return spectralDensity;
        }

        #region
        /// <summary>
        /// Function that finds Tends to QRSonsets segments to analysis
        /// </summary>
        /// <param name="_Tends"> List of number of samples where T-waves end </param>
        /// <param name="_QRSonsets"> List of number of samples where QRS start </param>
        /// <returns> Segments to frequency analisis </returns>
        #endregion

        public List<double[]> GetEcgPart()
        {
            List<double[]> t2qrsEkgParts = new List<double[]>();
            _Tends = _Tends.Where(x => x > 0).ToList();
            _QRSonsets = _QRSonsets.Where(x => x > 0).ToList();

            for (int i = 0, j = 0; i < _QRSonsets.Count && j < _Tends.Count; j++)
            {
                List<double> tmpEkgPart = new List<double>(1000);
                int start = (int)_Tends[j];
                int end = (int)_QRSonsets[i];
                while (start >= end)
                {
                    i++;
                    if (i >= _QRSonsets.Count)
                    {
                        break;
                    }
                    end = (int)_QRSonsets[i];
                }
                while (true)
                {
                    if (j + 1 >= _Tends.Count)
                    {
                        break;
                    }
                    else
                    {
                        if (end - start > end - _Tends[j + 1] && end - _Tends[j + 1] > 0)
                        {
                            j++;
                            start = (int)_Tends[j];
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                _ecgPartStarts.Add(start);
                _ecgPartEnds.Add(end);
                while (i < _QRSonsets.Count && start < _samples.Count && start <= _QRSonsets[i])
                {
                    tmpEkgPart.Add(_samples[start]);
                    start++;
                }
                if (tmpEkgPart.Count > 2)
                {
                    t2qrsEkgParts.Add(tmpEkgPart.ToArray());
                }

            }

            return t2qrsEkgParts;
        }

        public static double[] ReadFromCSV(string path)
        {
            double[] samples = null;
            StreamReader reader = new StreamReader(File.OpenRead(path));
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] values = line.Split(',');

                samples = new double[values.Length];
                int i = 0;
                foreach (var value in values)
                {
                    samples[i] = double.Parse(value.Replace('.', ','), System.Globalization.NumberStyles.Float);
                    i++;
                }
            }
            return samples;
        }

    }
}
