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
    public partial class Flutter : IModule
    {
        private List<int> _QRSonsets; // początek zespołu qrs
        private List<int> _Tends; // lista numerów próbek gdzie kończy się fala T
        private Vector<double> _samples; // próbki sygnału
        private double _fs;

        public Flutter(List<int> tEnds, List<int> qrsOnsets, 
            Vector<double> samples, double fs)
        {
            _Tends = tEnds;
            _QRSonsets = qrsOnsets;
            _samples = samples;
            _fs = fs;
        }

        public List<Tuple<int, int>> DetectAtrialFlutter()
        {
            List<double[]> t2qrsEkgParts = GetEcgPart();
            List<double[]> spectralDensityList = CalculateSpectralDensity(t2qrsEkgParts);
            List<double[]> frequenciesList = CalculateFrequenciesAxis(spectralDensityList);
            TrimToGivenFreq(spectralDensityList, frequenciesList, 70.0);
            InterpolateSpectralDensity(spectralDensityList, frequenciesList, 0.01);
            List<double> powerList = CalculateIntegralForEachSpectrum(frequenciesList, spectralDensityList);

            bool[] aflDetected = new bool[spectralDensityList.Count];

            foreach (double[] spectralDensity in spectralDensityList)
            {
                if (spectralDensity.Length > 0)
                {                 
                    double maxValue = spectralDensity.Max();
                    int maxValueIndex = Array.IndexOf(spectralDensity, maxValue);
                    
                    
                }
            }

            throw new NotImplementedException();
        }

        private List<double> CalculateIntegralForEachSpectrum(List<double[]> frequenciesList, List<double[]> spectralDensityList)
        {
            List<double> powerList = new List<double>(frequenciesList.Count);
            for(int i = 0 ; i < frequenciesList.Count ; i++)
            {
                double sum = 0;
                for(int j = 0 ; j < frequenciesList[i].Length-1 ; j++)
                {
                    sum += (spectralDensityList[i][j] + spectralDensityList[i][j + 1])
                        * (frequenciesList[i][j + 1] - frequenciesList[i][j])
                        * 0.5;
                }
                powerList.Add(sum);
            }
            return powerList;
        }

        private void InterpolateSpectralDensity(List<double[]> spectralDensityList, List<double[]> frequenciesList, double step)
        {
            for (int i = 0 ; i < spectralDensityList.Count ; i++)
            {
                List<double> interpolatedFreq = new List<double>((int)Math.Ceiling((frequenciesList[i].Last() - frequenciesList[i].First()) / step));
                List<double> interpolatedSpectralDensity = new List<double>((int)Math.Ceiling((frequenciesList[i].Last() - frequenciesList[i].First()) / step));            
                for (int j = 0; j < spectralDensityList[i].Length-1; j++)
                {
                    double y1 = spectralDensityList[i][j];
                    double y2 = spectralDensityList[i][j + 1];
                    double x1 = frequenciesList[i][j];
                    double x2 = frequenciesList[i][j + 1];

                    double a = (y2 - y1) / (x2 - x1);
                    double b = y1 - a * x1;

                    double k = x1;
                    
                    while(k < x2)
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

        private List<double[]> CalculateFrequenciesAxis(List<double[]> spectralDensityList)
        {
            List<double[]> freqs = new List<double[]>();
            foreach(var spectralDensity in spectralDensityList)
            {
                double[] freq = new double[spectralDensity.Length];
                for(int i = 0 ; i < spectralDensity.Length ; i++)
                {
                    freq[i] = (i * _fs) / spectralDensity.Length;
                }
                freqs.Add(freq);
            }
            return freqs;
        }

        private void TrimToGivenFreq(List<double[]> spectralDensityList, List<double[]> frequenciesList, double trimFreq)
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

        private List<double[]> TrimSpectralDensity(List<double[]> spectralDensityList, List<double[]> frequenciesList, double trimFreq)
        {
            List<double[]> trimmedSpectralDensityList = new List<double[]>(spectralDensityList.Count);
            foreach(var spectralDensity in spectralDensityList)
            {
                int n = (int)(trimFreq * spectralDensity.Length / _fs);
                double[] trimmed = new double[n];
                double[] freqTrimmed = new double[n];
                for(int i = 0; i < n; i++)
                {
                    trimmed[i] = spectralDensity[i];
                }
                trimmedSpectralDensityList.Add(trimmed);
            }
            return trimmedSpectralDensityList;
        }

        private List<double[]> CalculateSpectralDensity(List<double[]> t2qrsEcgParts)
        {
            List<double[]> spectralDensity = new List<double[]>(t2qrsEcgParts.Count);
            foreach (var ecgPart in t2qrsEcgParts)
            {
                Complex[] result = ecgPart.Select(x => new Complex(x, 0)).ToArray();
                Fourier.Forward(result);
                double[] power = result.Select(x => (x * (new Complex(x.Real, -1 * x.Imaginary))).Real).ToArray();
                spectralDensity.Add(power);
                //Complex[] test = result.ToArray();
                //Fourier.Inverse(test);
            }
            return spectralDensity;
        }

        private List<double[]> GetEcgPart()
        {
            List<double[]> t2qrsEkgParts = new List<double[]>();

            for (int i = 1; i < _QRSonsets.Count || i < _Tends.Count; i++)
            {
                List<double> tmpEkgPart = new List<double>(1000);
                int start = (int)_Tends[i - 1];
                while (start <= _QRSonsets[i])
                {
                    tmpEkgPart.Add(_samples[start]);
                    start++;
                }
                t2qrsEkgParts.Add(tmpEkgPart.ToArray());
            }

            return t2qrsEkgParts;
        }

        static void Main()
        {
            string path = @"..\..\Modules\Flutter";

            string samplesFileName = "samples.csv";
            string QRSOnsetsFileName = "QRSOnsets.csv";
            string TEndsFileName = "TEnds.csv";


            double[] samples = ReadFromCSV(Path.Combine(path, samplesFileName));
            Vector<double> samplesTmp = Vector<double>.Build.DenseOfArray(samples);

            List<int> QRSOnsetsTmp = ReadFromCSV(Path.Combine(path, QRSOnsetsFileName)).Select(x => (int)x).ToList();
            List<int> tEndsTmp = ReadFromCSV(Path.Combine(path, TEndsFileName)).Select(x => (int)x).ToList();

            Flutter flutter = new Flutter(tEndsTmp, QRSOnsetsTmp, samplesTmp, 360);
            flutter.DetectAtrialFlutter();

            Console.ReadKey();
        }

        private static double[] ReadFromCSV(string path) 
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
