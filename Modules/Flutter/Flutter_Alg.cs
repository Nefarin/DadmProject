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

        public Flutter(List<int> tEnds, List<int> qrsOnsets, Vector<double> samples)
        {
            _Tends = tEnds;
            _QRSonsets = qrsOnsets;
            _samples = samples;
        }

        public List<Tuple<int, int>> DetectAtrialFlutter()
        {
            List<double[]> t2qrsEkgParts = GetEcgPart();
            List<double[]> spectralDensityList = CalculateSpectralDensity(t2qrsEkgParts);

            bool[] aflDetected = new bool[spectralDensityList.Count];

            foreach (double[] spectralDensity in spectralDensityList)
            {

            }

            throw new NotImplementedException();
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

            Flutter flutter = new Flutter(tEndsTmp, QRSOnsetsTmp, samplesTmp);
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
