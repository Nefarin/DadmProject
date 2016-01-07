using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Ink;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.IO;
using System.Globalization;

namespace EKG_Project.Modules.Heart_Class
{
    public partial class Heart_Class : IModule
    {
        List<Tuple<int, int>> Classification(Vector<double> currentChannelFilteredSignal, uint fs, Vector<double> R, List<int> qrsOnset, List<int> qrsEnds)
        {
            //czym ma byc R?
            var qrsComplexes = SetQrsComplex(currentChannelFilteredSignal, qrsOnset, qrsEnds, R);
            
            List<Tuple<int, Vector<double>>> qrsCoefficients = CountCoefficients(qrsComplexes, fs);

            //KLASYFIKACJA
            return TestKnnCase(Params.TrainSamples, qrsCoefficients, Params.TrainClasses, Params.K); // klasyfikacja sygnału signal
            }

        #region Documentation
        /// <summary>
        /// This method uses data from WAVES module (Qrs_onset and Qrs_end) and extracts single QRS complexes, creating list of Tuple. Each tuple contains int value - number of R peaks corresponding to the QRS complex, and vector - containing following signal samples. 
        /// </summary>
        #endregion
        private List<Tuple<int, Vector<double>>> SetQrsComplex(Vector<double> filteredSignals, List<int> qrsOnsets, List<int> qrsEnds, Vector<double> qrsR)
        {
            var qrsComplexes = new List<Tuple<int, Vector<double>>>();

            for (int i = 0; i < qrsOnsets.Count; i++)
            {
                var singleQrsOnset = qrsOnsets[i];
                var signleQrsEnd = qrsEnds[i];
                var qrsLength = signleQrsEnd - singleQrsOnset + 1;
                var singleQrs = Vector<double>.Build.Dense(qrsLength);
                var singleQrsR = (int)qrsR[i];

                if (singleQrsOnset != -1)
                {
                    filteredSignals.CopySubVectorTo(singleQrs, singleQrsOnset, 0, qrsLength);
                    Tuple<int, Vector<double>> a = new Tuple<int, Vector<double>>(singleQrsR, singleQrs);
                    qrsComplexes.Add(a);
                }
            }
            return qrsComplexes;
        }

        List<Tuple<int, Vector<double>>> CountCoefficients(List<Tuple<int, Vector<double>>> qrsComplexes, uint fs)
        {
            //bedzie wektorem cech dla 1 zespołu
            Vector<double> singleCoeffVect = Vector<double>.Build.Dense(4); // (5) jeśli dodamy czas trwania zespołu

            List<Tuple<int, Vector<double>>> result = new List<Tuple<int, Vector<double>>>();

            foreach (Tuple<int, Vector<double>> data in qrsComplexes)
            {
                int singleQrsR = data.Item1;
                singleCoeffVect[0] = Coefficients.MalinowskaFactor(data.Item2, fs);
                singleCoeffVect[1] = Coefficients.PnRatio(data.Item2);
                singleCoeffVect[2] = Coefficients.SpeedAmpRatio(data.Item2);
                singleCoeffVect[3] = Coefficients.FastSampleCount(data.Item2);
                //singleCoeffVect[4] = QrsDuration(data.Item2, fs);

                Tuple<int, Vector<double>> coeffTuple = new Tuple<int, Vector<double>>(singleQrsR, singleCoeffVect.Clone());
                result.Add(coeffTuple);
            }
            return result;
        }

        List<Tuple<int, int>> TestKnnCase(List<Vector<double>> trainSamples, List<Tuple<int, Vector<double>>> testSamples, List<int> trainClasses, int K)
        {
            var testResults = new List<Tuple<int, int>>();
            var trainCount = trainSamples.Count;
            var distances = new double[trainCount][];

            for (var i = 0; i < trainCount; i++)
            {
                distances[i] = new double[2]; // Will store both distance and index in here
            }

            // Performing KNN 
            foreach (Tuple<int, Vector<double>> testSample in testSamples)
            {
                // For every test sample, calculate distance from every training sample

                for (var trn = 0; trn < trainCount; trn++)
                {
                    var dist = GetDistance(testSample.Item2, trainSamples[trn]);
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
                var classResult = yes > no ? 1 : 0;

                var singleQrsR = testSample.Item1;
                var resultTuple = new Tuple<int, int>(singleQrsR, classResult);

                testResults.Add(resultTuple);
            }

            return testResults;
        }

        #region Documentation
        /// <summary>
        /// Calculates and returns square of Euclidean distance between two vectors:
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

        /* skopiowane metody wczytujace dane
        #region Documentation
        /// <summary>
        /// Metody wczytujące zbior treningowy i testowy od Ani Metz:
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

        public static Vector<double> stringToVector(string input)
        {
            double[] digits = input //string z jednego wiersza
                              .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) // wczytanie liczb rozdzielonych przecinkiem
                              .Select(digit => Convert.ToDouble(digit, new System.Globalization.NumberFormatInfo())) // konwersja na double
                              .ToArray(); // zamiana stringa w tablicę
            Vector<double> vector = Vector<double>.Build.Dense(digits.Length); // inicjalizacja wektora
            vector.SetValues(digits); // zapisanie do wektora tablicy typu double
            return vector;

        }*/

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


        public static void Main()
        {


            //// konwersja na listę intów, bo tak napisałam metodę do klasyfikacji:
            //int oneClassElement;
            //List<int> trainClass;
            //trainClass = new List<int>();
            //foreach (var item in trainClassList)
            //{
            //    foreach (var element in item)
            //    {
            //        oneClassElement = (int)element;
            //        trainClass.Add(oneClassElement);
            //    }
            //}

            //HeartClass.classificationResult = HeartClass.TestKnnCase(trainDataList, testSamples, trainClass, 1); // jeśli chcemy prztestować zbiór testowy (z matlaba)


            //Heart_Class HeartClass = new Heart_Class();
            //TempInput.setInputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\signal.txt");
            //uint fs = TempInput.getFrequency();
            //HeartClass.Signal = TempInput.getSignal();
            //TempInput.setInputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\qrsOnset.txt");
            ////ponizej chce wczytac jako wektor a to jest juz lista
            ////HeartClass.QrsOnset = TempInput.getSignal();
            //TempInput.setInputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\qrsEnd.txt");
            ////HeartClass.QrsEnd = TempInput.getSignal();

            //// uwaga tu mam pozniej wrzucic plik qrsR.txt !!!!
            //TempInput.setInputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\qrsEnd.txt");
            //HeartClass.QrsRPeaks = TempInput.getSignal();

            /*
            //WCZYTANIE ZESPOŁÓW QRS NA PODSTAWIE QRSonsets i QRSends
            HeartClass.SetQrsComplex(); 

            //LICZENIE WSPÓŁCZYNNIKÓW KSZTAŁTU
            HeartClass.QrsCoefficients = HeartClass.CountCoeff(HeartClass.GetQrsComplex(), fs);


            TempInput.setOutputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\out_sig.txt");
            TempInput.writeFile(fs, HeartClass.Signal);
            TempInput.setOutputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\out_on.txt");
            TempInput.writeFile(fs, HeartClass.QrsOnset);

            //WCZYTANIE ZBIORU TRENINGOWEGO
            List<Vector<double>> trainDataList = HeartClass.loadFile(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\train_d.txt");


            //WCZYTANIE ETYKIET ZBIORU TRENINGOWEGO: 0-V, 1-NV
            List<Vector<double>> trainClassList = HeartClass.loadFile(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\train_d_label.txt");
            // konwersja na listę intów, bo tak napisałam metodę do klasyfikacji:
                    int oneClassElement;
                    List<int> trainClass;
                    trainClass = new List<int>();
                    foreach (var item in trainClassList)
                    {
                        foreach (var element in item)
                        {
                            oneClassElement = (int)element;
                            trainClass.Add(oneClassElement);
                        }

                    }


            ////Do tesowania:
            //List<Vector<double>> testDataList = HeartClass.loadFile(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\test_d.txt");
            // Tworzenie listy tupli zbioru testowego - w celach testowych (zbior treningowy i testowy wczytywany jest z pliku). 
            //w ostatecznej  wresji testDataList będzie obliczane w programie w formie:  List<Tuple<int, Vector<double>>>: 
                    List<Tuple<int, Vector<double>>> testSamples;
                    testSamples = new List<Tuple<int, Vector<double>>>();
                    Tuple<int, Vector<double>> oneElement;
                    int R = 1; 
                    foreach (var item in testDataList)
                    {
                        oneElement = new Tuple<int, Vector<double>>(R, item.Clone());
                        testSamples.Add(oneElement);
                    }


            //KLASYFIKACJA
            HeartClass.HeartClassData.ClassificationResult = HeartClass.TestKnnCase(trainDataList, HeartClass.QrsCoefficients, trainClass, 1); // klasyfikacja sygnału testowego signal
            //HeartClass.classificationResult = HeartClass.TestKnnCase(trainDataList, testSamples, trainClass, 1); // jeśli chcemy prztestować zbiór testowy (z matlaba)

        */

            // nie działa bo onset i end są już List<int>
            //HeartClass.HeartClassData.ClassificationResult = HeartClass.Classification(HeartClass.Signal, fs,
            //    HeartClass.QrsR, HeartClass.QrsOnset, HeartClass.QrsEnd);

        }


    }

}