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

        //int qrsLength;


        List<Tuple<int, int>> Classification(Vector<double> loadedSignal, uint fs, Vector<double> R, List<int> qrsOnset, List<int> qrsEnd)
        {
            Signal = loadedSignal;



            SetQrsComplex();

            //WCZYTANIE ZBIORU TRENINGOWEGO19
            //List<Vector<double>> trainDataList = loadFile(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\train_d.txt");

            //WCZYTANIE ETYKIET ZBIORU TRENINGOWEGO: 0-V, 1-NV
            //List<Vector<double>> trainClassList = loadFile(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\train_d_label.txt");

            List<Vector<double>> trainClassList;
            List<Vector<double>> trainDataList;
            List<Tuple<int, Vector<double>>> qrsCoefficients = CountCoefficients(GetQrsComplex(), fs);

            //KLASYFIKACJA
            //return TestKnnCase(trainDataList, qrsCoefficients, trainClass, 1); // klasyfikacja sygnału signal
            throw new NotImplementedException();
            }


        #region Documentation
        /// <summary>
        /// This method uses data from WAVES module (Qrs_onset and Qrs_end) and extracts single QRS complexes, creating list of Tuple. Each tuple contains int value - number of R peaks corresponding to the QRS complex, and vector - containing following signal samples. 
        /// </summary>
        #endregion
        void SetQrsComplex()
        {
            for (int i = 0; i < QrsNumber; i++)
            {
                double singleQrsOnset = QrsOnset[i];
                double signleQrsEnd = QrsEnd[i];
                int qrsLength = (int)(signleQrsEnd - singleQrsOnset + 1);
                SingleQrs = Vector<double>.Build.Dense(qrsLength);
                int singleQrsR = (int)QrsR.At(i);

                if ((int)singleQrsOnset != -1)
                {
                    Signal.CopySubVectorTo(SingleQrs, sourceIndex: (int)singleQrsOnset, targetIndex: 0, count: qrsLength);
                    Tuple<int, Vector<double>> a = new Tuple<int, Vector<double>>(singleQrsR, SingleQrs);
                    QrsComplex.Add(a);
                }
            }
        }

        List<Tuple<int, Vector<double>>> CountCoefficients(List<Tuple<int, Vector<double>>> qrsComplex, uint fs)
        {
            //bedzie wektorem cech dla 1 zespołu
            Vector<double> singleCoeffVect = Vector<double>.Build.Dense(4); // (5) jeśli dodamy czas trwania zespołu

            List<Tuple<int, Vector<double>>> result = new List<Tuple<int, Vector<double>>>();

            foreach (Tuple<int, Vector<double>> data in qrsComplex)
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





        ////private Vector<double> _qrsOnset;        // inicjalizacja przez wczytanie Vector z pliku
        ////private Vector<double> _qrsEnd;          // inicjalizacja przez wczytanie Vector z pliku

        //private Vector<double> _qrssignal;
        //double malinowskaCoefficient;
        //double pnRatio;
        //double speedAmpltudeRatio;
        //double fastSample;
        //double qRSTime;
        //uint fs;

        //private Heart_Class_Data HeartClassData;
        //List<Vector<double>> coefficients; //lista współczynników kształtu dla zbioru treningowego
        ////List<Tuple<int, int>> classificationResult; // pierwszy int - nr zespołu (nr R), drugi int - klasa zespołu


        //public Heart_Class()
        //{

        //    Signal = Vector<double>.Build.Dense(1);
        //    //_qrsOnset = Vector<double>.Build.Dense(1);
        //    QrsOnset = new List<int>();
        //    //_qrsEnd = Vector<double>.Build.Dense(1);
        //    QrsEnd = new List<int>();
        //    QrsNumber = new int();
        //    QrsR = Vector<double>.Build.Dense(1);
        //    SingleQrs = Vector<double>.Build.Dense(1);
        //    QrsComplex = new List<Tuple<int, Vector<double>>>();
        //    QrsCoefficients = new List<Tuple<int, Vector<double>>>();


        //    _qrssignal = Vector<double>.Build.Dense(1);
        //    malinowskaCoefficient = new double();
        //    pnRatio = new double();
        //    speedAmpltudeRatio = new double();
        //    fastSample = new double();
        //    qRSTime = new double();
        //    fs = new uint();
        //    qrsLength = _qrssignal.Count();
        //    //_qrsCoefficients = new List<Tuple<int, Vector<double>>> ();
        //    HeartClassData = new Heart_Class_Data();
        //    // nie wiem czemu ale poniższe wywołanie obiektu nie działa, musi byc w metodzie loadFile ;/
        //    List<Vector<double>> coefficients = new List<Vector<double>>();
        //    //classificationResult = new List<Tuple<int, int>>();
        //}


        //#region Documentation
        ///// <summary>
        ///// This method returns QRS complexes, which were set in SetQrsComplex()
        ///// </summary>
        ///// <returns></returns>
        //#endregion
        //public List<Tuple<int, Vector<double>>> GetQrsComplex()
        //{
        //    return QrsComplex;
        //}



        //#region Documentation
        ///// <summary>
        ///// Metody wczytujące zbior treningowy i testowy od Ani Metz:
        ///// </summary>
        ///// <param name="path"></param>
        ///// <returns></returns>
        //#endregion
        //List<Vector<double>> loadFile(string path)
        //{
        //    List<Vector<double>> coefficients = new List<Vector<double>>(); //inicjalizacja listy wektorów z jednego pliku
        //    using (StreamReader sr = new StreamReader(path))
        //    {
        //        string fileData = sr.ReadToEnd(); //wczytanie całego pliku
        //        string[] fileLines = fileData.Split('\n'); //rozdzielenie linii
        //        for (int i = 0; i < fileLines.Length - 1; i++) //fileLines.Length-1 -> ostatnia linia pliku jest pusta dlatego "- 1"
        //        {
        //            string readCoefficients = fileLines[i]; // dane z jednego wiersza w postaci string
        //            Vector<double> vectorCoefficients = stringToVector(readCoefficients); // zmiana na wektor
        //            coefficients.Add(vectorCoefficients); // wpisanie do listy
        //        }
        //    }

        //    return coefficients;

        //}

        //public static Vector<double> stringToVector(string input)
        //{
        //    double[] digits = input //string z jednego wiersza
        //                      .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) // wczytanie liczb rozdzielonych przecinkiem
        //                      .Select(digit => Convert.ToDouble(digit, new System.Globalization.NumberFormatInfo())) // konwersja na double
        //                      .ToArray(); // zamiana stringa w tablicę
        //    Vector<double> vector = Vector<double>.Build.Dense(digits.Length); // inicjalizacja wektora
        //    vector.SetValues(digits); // zapisanie do wektora tablicy typu double
        //    return vector;

        //}

        ///*
        //#region Documentation
        ///// <summary>
        ///// TODO
        ///// </summary>
        //#endregion
        //public Vector<double> QrsOnset
        //{
        //    get { return _qrsOnset; }
        //    set
        //    {
        //        //powinien byc typ int! ale to pozniej, bo klasa TempInut nie wczytuje int
        //        _qrsOnset = value;
        //        QrsNumber = _qrsOnset.Count();
        //    }
        //}

        //#region Documentation
        ///// <summary>
        ///// TODO
        ///// </summary>
        //#endregion
        //public Vector<double> QrsEnd
        //{
        //    get { return _qrsEnd; }
        //    set { _qrsEnd = value; }
        //}
        //*/

        public Vector<double> Signal { get; set; }

        //public Vector<double> QrsR { get; set; }

        //public int QrsNumber { get; set; }

        //public Vector<double> SingleQrs { get; set; }

        //public List<Tuple<int, Vector<double>>> QrsComplex { get; set; }

        //public List<Tuple<int, Vector<double>>> QrsCoefficients { get; set; }

        //public List<int> QrsOnset { get; set; }

        //public List<int> QrsEnd { get; set; }

        #region Documentation
        /// <summary>
        /// Test method of Heart_Class module
        /// </summary>
        #endregion
        public static void Main()
        {

            ////Do tesowania:
            //List<Vector<double>> testDataList = loadFile(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\test_d.txt");
            //// Tworzenie listy tupli zbioru testowego - w celach testowych (zbior treningowy i testowy wczytywany jest z pliku). 
            ////w ostatecznej  wresji testDataList będzie obliczane w programie w formie:  List<Tuple<int, Vector<double>>>: 
            //List<Tuple<int, Vector<double>>> testSamples;
            //testSamples = new List<Tuple<int, Vector<double>>>();
            //Tuple<int, Vector<double>> oneElement;
            //int Rpeak = 1;
            //foreach (var item in testDataList)
            //{
            //    oneElement = new Tuple<int, Vector<double>>(Rpeak, item.Clone());
            //    testSamples.Add(oneElement);
            //}

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


            Heart_Class HeartClass = new Heart_Class();
            TempInput.setInputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\signal.txt");
            uint fs = TempInput.getFrequency();
            HeartClass.Signal = TempInput.getSignal();
            TempInput.setInputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\qrsOnset.txt");
            //ponizej chce wczytac jako wektor a to jest juz lista
            //HeartClass.QrsOnset = TempInput.getSignal();
            TempInput.setInputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\qrsEnd.txt");
            //HeartClass.QrsEnd = TempInput.getSignal();

            // uwaga tu mam pozniej wrzucic plik qrsR.txt !!!!
            TempInput.setInputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\qrsEnd.txt");
            HeartClass.QrsR = TempInput.getSignal();

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


            List<Vector<double>> testDataList = HeartClass.loadFile(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\test_d.txt");
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