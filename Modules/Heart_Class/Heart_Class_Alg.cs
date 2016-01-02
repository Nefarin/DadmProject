using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography.X509Certificates;
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
        private Vector<double> _qrssignal;
        double malinowskaCoefficient;
        double pnRatio;
        double speedAmpltudeRatio;
        double fastSample;
        double qRSTime;
        uint fs;
        int qrsLength; 
        private Heart_Class_Data HeartClassData;
        List<Vector<double>> coefficients; //lista współczynników kształtu dla zbioru treningowego
        List<Tuple<int, int>> classificationResult; // pierwszy int - nr zespołu (nr R), drugi int - klasa zespołu


        public Heart_Class()
        {
            _qrssignal = Vector<double>.Build.Dense(1);
            malinowskaCoefficient = new double();
            pnRatio = new double();
            speedAmpltudeRatio = new double();
            fastSample = new double();
            qRSTime = new double();
            fs = new uint();
            qrsLength = _qrssignal.Count();
            //_qrsCoefficients = new List<Tuple<int, Vector<double>>> ();
            HeartClassData = new Heart_Class_Data();
            // nie wiem czemu ale poniższe wywołanie obiektu nie działa, musi byc w metodzie loadFile ;/
            List<Vector<double>> coefficients = new List<Vector<double>>();
            classificationResult = new List<Tuple<int, int>>();
        }


        #region Documentation
        /// <summary>
        /// Test method of Heart_Class module
        /// </summary>
        #endregion
        public static void Main()
        {
            Heart_Class HeartClass = new Heart_Class();
            TempInput.setInputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\signal.txt");
            uint fs = TempInput.getFrequency();
            HeartClass.HeartClassData.Signal = TempInput.getSignal();
            TempInput.setInputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\qrsOnset.txt");
            HeartClass.HeartClassData.QrsOnset = TempInput.getSignal();
            TempInput.setInputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\qrsEnd.txt");
            HeartClass.HeartClassData.QrsEnd = TempInput.getSignal();

            // uwaga tu mam pozniej wrzucic plik qrsR.txt !!!!
            TempInput.setInputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\qrsEnd.txt");
            HeartClass.HeartClassData.QrsR = TempInput.getSignal();

            //WCZYTANIE ZESPOŁÓW QRS NA PODSTAWIE QRSonsets i QRSends
            HeartClass.SetQrsComplex(); 

            //LICZENIE WSPÓŁCZYNNIKÓW KSZTAŁTU
            HeartClass.HeartClassData.QrsCoefficients = HeartClass.CountCoeff(HeartClass.GetQrsComplex(), fs);

            
            TempInput.setOutputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\out_sig.txt");
            TempInput.writeFile(fs, HeartClass.HeartClassData.Signal);
            TempInput.setOutputFilePath(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\out_on.txt");
            TempInput.writeFile(fs, HeartClass.HeartClassData.QrsOnset);

            //WCZYTANIE ZBIORU TRENINGOWEGO
            List<Vector<double>> trainDataList = HeartClass.loadFile(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\train_d.txt");
            //foreach (var item in trainDataList)
            //    Console.WriteLine(item);
            // Console.Read();

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
            HeartClass.classificationResult = HeartClass.TestKnnCase(trainDataList, testSamples, trainClass, 1);


        }

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        #endregion
        private void SetQrsComplex()
        {
            for (int i = 0; i < HeartClassData.QrsNumber; i++)
            {
                double singleQrsOnset = HeartClassData.QrsOnset.At(i);
                double signleQrsEnd = HeartClassData.QrsEnd.At(i);
                int qrsLength = (int)(signleQrsEnd - singleQrsOnset+1);
                HeartClassData.SingleQrs = Vector<double>.Build.Dense(qrsLength);
                int singleQrsR = (int)HeartClassData.QrsR.At(i);
                HeartClassData.Signal.CopySubVectorTo(HeartClassData.SingleQrs, sourceIndex: (int)singleQrsOnset, targetIndex: 0, count: qrsLength);
                Tuple<int, Vector<double>> a = new Tuple<int, Vector<double>>(singleQrsR, HeartClassData.SingleQrs);
                HeartClassData.QrsComplex.Add(a);
            }
        }

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        #endregion
        public List<Tuple<int, Vector<double>>> GetQrsComplex()
        {
            return HeartClassData.QrsComplex;
        }

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_qrssignal"></param>
        /// <param name="fs"></param>
        /// <returns></returns>
        #endregion
        double CountMalinowskaFactor(Vector<double> _qrssignal, uint fs)
        {
            double surface = Integrate(_qrssignal);
            double perimeter = Perimeter(_qrssignal, fs);

            if (perimeter != 0)
            {
                malinowskaCoefficient = surface / perimeter;
            }
            else malinowskaCoefficient = 0;
            return malinowskaCoefficient;
        }

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_qrssignal"></param>
        /// <returns></returns>
        #endregion
        double Integrate(Vector<double> _qrssignal)
        {

            double result = 0;
            foreach (double value in _qrssignal)
            {
                if (value < 0)
                    result = result - value;
                else
                    result = result + value;
            }
            return result;
        }

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_qrssignal"></param>
        /// <param name="fs"></param>
        /// <returns></returns>
        #endregion
        double Perimeter(Vector<double> _qrssignal, uint fs)
        {
            qrsLength = _qrssignal.Count();
            double timeBtw2points = 1 / fs;
            double result = 0;
            double a, b;

            for (int i = 0; i < (qrsLength - 1); i++)
            {
                a = _qrssignal.At(i);
                b = _qrssignal.At(i + 1);
                result = result + Math.Sqrt(timeBtw2points * timeBtw2points + (b - a) * (b - a));
            }
            return result;
        }

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_qrssignal"></param>
        /// <returns></returns>
        #endregion
        double PnRatio(Vector<double> _qrssignal)
        {
            double result = 0;
            double positiveAmplitude = 0;
            double negativeAmplitude = 0;

            foreach (double value in _qrssignal)
            {
                if (value < 0)
                    negativeAmplitude = negativeAmplitude + (-value);
                else
                    positiveAmplitude = positiveAmplitude + value;

            }
            if (negativeAmplitude == 0)
                result = positiveAmplitude;
            else
                result = positiveAmplitude / negativeAmplitude;
            return result;
        }

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_qrssignal"></param>
        /// <returns></returns>
        #endregion
        double SpeedAmpRatio(Vector<double> _qrssignal)
        {
            qrsLength = _qrssignal.Count();
            double[] speed = new double[qrsLength-2];
            double maxSpeed;
            double maxAmp;

            for (int i = 0; i < (qrsLength - 2); i++)
            {
                speed[i] = Math.Abs(_qrssignal.At(i + 2) + _qrssignal.At(i) - 2 * _qrssignal.At(i + 1));
            }
            maxSpeed = speed.Max();
            maxAmp = Math.Abs(_qrssignal.Maximum() - _qrssignal.Minimum());
            return maxSpeed / maxAmp;
        }
        
        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_qrssignal"></param>
        /// <returns></returns>
        #endregion
        double FastSampleCount(Vector<double> _qrssignal)
        {
            // qrsLength = _qrssignal.Count();
            // double[] speed = new double[qrsLength - 1];
            int qrsLength = _qrssignal.Count();
            double[] speed = new double[qrsLength - 1];
            double threshold;
            int counter = 0;
            int speedLength;
            double constant = 0.4;

            for (int i = 0; i < (qrsLength - 1); i++)
            {
                speed[i] = Math.Abs(_qrssignal.At(i + 1) - _qrssignal.At(i));
            }
            threshold = constant * speed.Max();

            foreach (double value in speed)
            {
                if (value >= threshold)
                    counter = counter + 1;
            }

            speedLength = speed.Length;
            return (double)counter / (double)speedLength;
        }

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_qrssignal"></param>
        /// <param name="fs"></param>
        /// <returns></returns>
        #endregion
        double QrsDuration(Vector<double> _qrssignal, uint fs)
        {
            qrsLength = _qrssignal.Count();
            double samplingInterval = 1 / (double)fs;
            return (double)qrsLength * samplingInterval;
        }


        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_QrsComplex"></param>
        /// <param name="fs"></param>
        /// <returns></returns>
        #endregion
        List<Tuple<int, Vector<double>>> CountCoeff(List<Tuple<int, Vector<double>>> _QrsComplex, uint fs)
        {
            Vector<double> singleCoeffVect; //bedzie wektorem cech dla 1 zespołu
            singleCoeffVect = Vector<double>.Build.Dense(4); // (5) jeśli dodamy czas trwania zespołu
            int singleQrsR;
            Tuple<int, Vector<double>> coeffTuple;
            List<Tuple<int, Vector<double>>> result;
            result = new List<Tuple<int, Vector<double>>>();
 

            foreach (Tuple<int, Vector<double>> data in _QrsComplex)
            {
                singleQrsR = data.Item1;
                singleCoeffVect[0] = CountMalinowskaFactor(data.Item2, fs);
                singleCoeffVect[1] = PnRatio(data.Item2);
                singleCoeffVect[2] = SpeedAmpRatio(data.Item2);
                singleCoeffVect[3] = FastSampleCount(data.Item2);
                //singleCoeffVect[4] = QrsDuration(data.Item2, fs);
                
                coeffTuple = new Tuple<int, Vector<double>>(singleQrsR, singleCoeffVect.Clone());
                
                result.Add(coeffTuple);
                
            }
            return result;
        }

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trainSamples"></param>
        /// <param name="testSamples"></param>
        /// <param name="trainClasses"></param>
        /// <param name="K"></param>
        /// <returns></returns>
        #endregion
        List<Tuple<int, int>> TestKnnCase(List<Vector<double>> trainSamples, List<Tuple<int, Vector<double>>> testSamples,
            List<int> trainClasses, int K)
        {
            var testResults = new List<Tuple<int, int>>();
            int classResult;
            var testNumber = testSamples.Count();
            var trainNumber = trainSamples.Count();
            Tuple<int, int> resultTuple;
            int singleQrsR;

           var distances = new double[trainNumber][];
            for (var i = 0; i < trainNumber; i++)
            {
                distances[i] = new double[2]; // Will store both distance and index in here
            }


            // Performing KNN 
            for (var tst = 0; tst < testNumber; tst++)
            {
                // For every test sample, calculate distance from every training sample

                for (var trn = 0; trn < trainNumber; trn++)
                {
                    var dist = GetDistance(testSamples[tst].Item2, trainSamples[trn]);
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

                singleQrsR = testSamples[tst].Item1;
                resultTuple = new Tuple<int, int>(singleQrsR, classResult);

                testResults.Add(resultTuple);
            }


            return testResults;

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
        double GetDistance(Vector<double> sample1, Vector<double> sample2)
        {
            var distance = 0.0;
            // assume sample1 and sample2 are valid i.e. same length 

            for (var i = 0; i < sample1.Count; i++)
            {
                var temp = sample1.At(i) - sample2.At(i);
                distance += temp * temp;
            }
            //return distance; ??? bez pierwiastka? - zapytać mądrych ludzi
            return Math.Sqrt(distance);
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


    }

}