using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.IO;

namespace EKG_Project.Modules.Heart_Class
{
    public partial class Heart_Class : IModule
    {
        //private Vector<double> _qrsOnset;        // inicjalizacja przez wczytanie Vector z pliku
        //private Vector<double> _qrsEnd;          // inicjalizacja przez wczytanie Vector z pliku
        //private int _currentQRSComplex = 0;

        //private Vector<double> _currentRVector;
        //private Vector<double> _currentECGBaselineVector;


        //private Vector<double> _qrssignal;
        //private double malinowskaCoefficient;
        //private double pnRatio;
        //private double speedAmpltudeRatio;
        //private double fastSample;
        //private double qRSTime;
        private uint _fs;
        //private int qrsLength;
        //private Heart_Class_Data HeartClassData;
        //private List<Vector<double>> coefficients; //lista współczynników kształtu dla zbioru treningowego
        //List<Tuple<int, int>> classificationResult; // pierwszy int - nr zespołu (nr R), drugi int - klasa zespołu

        public Heart_Class()
        {
            //_qrsOnset = Vector<double>.Build.Dense(1);
            QrsOnset = new List<int>();
            //_qrsEnd = Vector<double>.Build.Dense(1);
            QrsEnd = new List<int>();
            QrsNumber = new int();
            QrsR = Vector<double>.Build.Dense(1);
            QrsComplex = new List<Tuple<int, Vector<double>>>();
            QrsCoefficients = new List<Tuple<int, Vector<double>>>();

            //_qrssignal = Vector<double>.Build.Dense(1);
            //malinowskaCoefficient = new double();
            //pnRatio = new double();
            //speedAmpltudeRatio = new double();
            //fastSample = new double();
            //qRSTime = new double();

            //qrsLength = _qrssignal.Count();
            //_qrsCoefficients = new List<Tuple<int, Vector<double>>> ();
            //HeartClassData = new Heart_Class_Data();
            // nie wiem czemu ale poniższe wywołanie obiektu nie działa, musi byc w metodzie loadFile ;/
            //List<Vector<double>> coefficients = new List<Vector<double>>();
            //classificationResult = new List<Tuple<int, int>>();
        }


        /// <summary>
        /// TODO 
        /// </summary>
        /// <param name="loadedSignal"></param>
        /// <param name="R"></param>
        /// <param name="qrsOnset"></param>
        /// <param name="qrsEnd"></param>
        /// <returns></returns>
        Tuple<int, int> ClassificationOneQrs(Vector<double> loadedSignal, int qrsOnset, int qrsEnd, double R)
        {
            var qrsComplexOne = OneQrsComplex(loadedSignal, qrsOnset, qrsEnd, R);
            var qrsCoeffOne = CountCoeffOne(qrsComplexOne, _fs);
            //WCZYTANIE ZBIORU TRENINGOWEGO
            DebugECGPath loader = new DebugECGPath();
            List<Vector<double>> trainDataList = loadFile(Path.Combine(loader.getTempPath(), "train_d.txt"));


            //WCZYTANIE ETYKIET ZBIORU TRENINGOWEGO: 0-V, 1-NV
            List<Vector<double>> trainClassList = loadFile(Path.Combine(loader.getTempPath(), "train_d_label.txt"));
            // konwersja na listę intów, bo tak napisałam metodę do klasyfikacji:
            var trainClass = new List<int>();
            foreach (var item in trainClassList)
            {
                foreach (var element in item)
                {
                    var oneClassElement = (int)element;
                    trainClass.Add(oneClassElement);
                }
            }

            return ClassificationResultOne = TestKnnCaseOne(trainDataList, qrsCoeffOne, trainClass, 3);

        }

        private Tuple<int, Vector<double>> OneQrsComplex(Vector<double> loadedSignal, double singleQrsOnset, double signleQrsEnd, double singleQrsR)
        {
            if ((int)singleQrsOnset == -1)
                throw new ArgumentException("singleQrsOnset is -1 in OneQrsComplex should be filterd earlier");

            int qrsLength = (int)(signleQrsEnd - singleQrsOnset + 1);

            var subVector = loadedSignal.SubVector((int)singleQrsOnset, qrsLength);

            return new Tuple<int, Vector<double>>((int)singleQrsR, subVector);
        }

        #region Documentation
        /// <summary>
        /// This method returns QRS complexes, which were set in SetQrsComplex()
        /// </summary>
        /// <returns></returns>
        #endregion
        public List<Tuple<int, Vector<double>>> GetQrsComplex()
        {
            return QrsComplex;
        }

        Tuple<int, Vector<double>> CountCoeffOne(Tuple<int, Vector<double>> qrsComplexOne, uint fs)
        {
            var singleCoeffVect = Vector<double>.Build.Dense(4);

            var singleQrsR = qrsComplexOne.Item1;
            singleCoeffVect[0] = Coefficients.MalinowskaFactor(qrsComplexOne.Item2, fs);
            singleCoeffVect[1] = Coefficients.PnRatio(qrsComplexOne.Item2);
            singleCoeffVect[2] = Coefficients.SpeedAmpRatio(qrsComplexOne.Item2);
            singleCoeffVect[3] = Coefficients.FastSampleCount(qrsComplexOne.Item2);
            //singleCoeffVect[4] = QrsDuration(data.Item2, fs);

            return new Tuple<int, Vector<double>>(singleQrsR, singleCoeffVect);
        }

        Tuple<int, int> TestKnnCaseOne(List<Vector<double>> trainSamples, Tuple<int, Vector<double>> testSamples, List<int> trainClasses, int K)
        {
            int classResult;
            var trainNumber = trainSamples.Count;

            var distances = new double[trainNumber][];
            for (var i = 0; i < trainNumber; i++)
            {
                distances[i] = new double[2]; // Will store both distance and index in here
            }


            // Performing KNN 

            // For every test sample, calculate distance from every training sample

            for (var trn = 0; trn < trainNumber; trn++)
            {
                var dist = GetDistance(testSamples.Item2, trainSamples[trn]);
                distances[trn][0] = dist;
                distances[trn][1] = trn;
            }

            // Sort distances and take top K 
            var votingDistances = distances.OrderBy(t => t[0]).Take(K);

            // Do a 'majority vote' to classify test sample
            var yes = 0.0;
            var no = 0.0;

            foreach (var voter in votingDistances)
            {
                if (trainClasses[(int)voter[1]] == 1)
                    yes++;
                else
                    no++;
            }
            if (yes > no)
                classResult = 1;
            else
                classResult = 0;

            var singleQrsR = testSamples.Item1;
            var resultTuple = new Tuple<int, int>(singleQrsR, classResult);

            return resultTuple;
        }

        // Calculates and returns square of Euclidean distance between two vectors:
        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sample1"></param>
        /// <param name="sample2"></param>
        /// <returns></returns>
        #endregion
        private double GetDistance(Vector<double> sample1, Vector<double> sample2)
        {
            // assume sample1 and sample2 are valid i.e. same length 

            return sample1.Subtract(sample2).L2Norm();

            //var distance = 0.0;

            //for (var i = 0; i < sample1.Count; i++)
            //{
            //    var temp = sample1[i] - sample2[i];
            //    distance += temp * temp;
            //}
            //return Math.Sqrt(distance);
        }

        // Metody wczytujące zbior treningowy i testowy od Ani Metz:
        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        #endregion
        List<Vector<double>> loadFile(string path)
        {
            List<Vector<double>> coefficients = new List<Vector<double>>(); //inicjalizacja listy wektorów z jednego pliku
            using (StreamReader sr = new StreamReader(path))
            {
                string fileData = sr.ReadToEnd(); //wczytanie całego pliku
                string[] fileLines = fileData.Split('\n'); //rozdzielenie linii
                for (int i = 0; i < fileLines.Length - 1; i++) //fileLines.Length-1 -> ostatnia linia pliku jest pusta dlatego "- 1"
                {
                    string readCoefficients = fileLines[i]; // dane z jednego wiersza w postaci string
                    Vector<double> vectorCoefficients = stringToVector(readCoefficients); // zmiana na wektor
                    coefficients.Add(vectorCoefficients); // wpisanie do listy
                }
            }

            return coefficients;

        }

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        #endregion
        public static Vector<double> stringToVector(string input)
        {
            double[] digits = input //string z jednego wiersza
                              .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) // wczytanie liczb rozdzielonych przecinkiem
                              .Select(digit => Convert.ToDouble(digit, new System.Globalization.NumberFormatInfo())) // konwersja na double
                              .ToArray(); // zamiana stringa w tablicę
            Vector<double> vector = Vector<double>.Build.Dense(digits.Length); // inicjalizacja wektora
            vector.SetValues(digits); // zapisanie do wektora tablicy typu double
            return vector;

        }




        // getery i settery danych przejściowych

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        public Vector<double> QrsR { get; set; }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        public int QrsNumber { get; set; }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        public List<Tuple<int, Vector<double>>> QrsComplex { get; set; }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        public List<Tuple<int, Vector<double>>> QrsCoefficients { get; set; }

        public List<int> QrsOnset { get; set; }

        public List<int> QrsEnd { get; set; }

        public Tuple<int, int> ClassificationResultOne { get; set; }


        public static class Coefficients
        {
            #region Documentation
            /// <summary>
            /// This method calculates Malinowska's factor as one of shape coefficients using a single QRS complex (qrsSignal) and sampling frequency (fs).
            /// </summary>
            /// <param name="qrsSignal"></param>
            /// <param name="fs"></param>
            /// <returns></returns>
            #endregion
            public static double MalinowskaFactor(Vector<double> qrsSignal, uint fs)
            {
                double surface = Integrate(qrsSignal);
                double perimeter = Perimeter(qrsSignal, fs);

                return !perimeter.Equals(0.0) ? surface / perimeter : 0;
            }

            public static double PnRatio(Vector<double> qrsSignal)
            {
                double positiveAmplitude = 0;
                double negativeAmplitude = 0;

                foreach (double value in qrsSignal)
                {
                    if (value < 0)
                        negativeAmplitude += Math.Abs(value);
                    else
                        positiveAmplitude += value;
                }

                return negativeAmplitude.Equals(0.0) ? positiveAmplitude : positiveAmplitude / negativeAmplitude;
            }

            public static double SpeedAmpRatio(Vector<double> qrsSignal)
            {
                double maxSpeed = 0.0;

                for (int i = 0; i < (qrsSignal.Count - 2); i++)
                {
                    var currentSpeed = Math.Abs(qrsSignal[i + 2] + qrsSignal[i] - 2 * qrsSignal[i + 1]);
                    if (currentSpeed > maxSpeed)
                        maxSpeed = currentSpeed;
                }

                var maxAmp = Math.Abs(qrsSignal.Maximum() - qrsSignal.Minimum());
                return maxSpeed / maxAmp;
            }

            public static double FastSampleCount(Vector<double> qrsSignal)
            {
                double maxSpeed = 0.0;

                int qrsLength = qrsSignal.Count;
                double[] speed = new double[qrsLength - 1];
                double constant = 0.4;

                for (int i = 0; i < (qrsLength - 1); i++)
                {
                    var currentSpeed = Math.Abs(qrsSignal[i + 1] - qrsSignal[i]);
                    speed[i] = currentSpeed;

                    if (currentSpeed > maxSpeed)
                        maxSpeed = currentSpeed;
                }
                var threshold = constant * maxSpeed;
                double count = speed.Count(value => value >= threshold);

                return count / speed.Length;
            }

            public static double QrsDuration(Vector<double> qrsSignal, uint fs)
            {
                double samplingInterval = (double)1 / fs;
                return qrsSignal.Count * samplingInterval;
            }

            #region Documentation
            /// <summary>
            /// This method is a sub method used in CountMalinowskaFactor(). It integrates the signal.
            /// </summary>
            /// <param name="qrsSignal"></param>
            /// <returns></returns>
            #endregion
            private static double Integrate(Vector<double> qrsSignal)
            {
                return qrsSignal.Sum(value => Math.Abs(value));
            }

            #region Documentation
            /// <summary>
            /// This method is a sub method used in CountMalinowskaFactor(). It calculates the perimeter of the signal.
            /// </summary>
            /// <param name="qrsSignal"></param>
            /// <param name="fs"></param>
            /// <returns></returns>
            #endregion
            private static double Perimeter(Vector<double> qrsSignal, uint fs)
            {
                double timeBtw2Points = (double)1 / fs;
                double result = 0;

                for (int i = 0; i < (qrsSignal.Count - 1); i++)
                {
                    var a = qrsSignal[i];
                    var b = qrsSignal[i + 1];
                    result = result + Math.Sqrt(timeBtw2Points * timeBtw2Points + (b - a) * (b - a));
                }
                return result;
            }
        }

    }

}