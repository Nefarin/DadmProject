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
        private Vector<double> _signal;          
        private List<int> _qrsOnset;
        private List<int> _qrsEnd;
        private int _qrsNumber;                  
        private Vector<double> _qrsR;            
        private Vector<double> _singleQrs;       
        //private List<Tuple<int, Vector<double>>> _QrsComplex; 
        private Tuple<int, Vector<double>> _QrsComplexOne;
        //private List<Tuple<int, Vector<double>>> _qrsCoefficients;
        //private int _currentQRSComplex = 0;
        private Tuple<int, Vector<double>> _qrsCoeffOne;

        private Vector<double> _currentRVector;
        private Vector<double> _currentECGBaselineVector;
          

        private Vector<double> _qrssignal;
        private double malinowskaCoefficient;
        private double pnRatio;
        private double speedAmpltudeRatio;
        private double fastSample;
        private double qRSTime;
        private uint fs;
        private int qrsLength; 
        private Heart_Class_Data HeartClassData;
        private List<Vector<double>> coefficients; //lista współczynników kształtu dla zbioru treningowego
        private Tuple<int, int> _classificationResultOne; // pierwszy int - nr zespołu (nr R), drugi int - klasa zespołu

        public Heart_Class()
        {

            _signal = Vector<double>.Build.Dense(1);
            _qrsOnset = new List<int>();
            _qrsEnd = new List<int>();
            _qrsNumber = new int();
            _qrsR = Vector<double>.Build.Dense(1);
            _singleQrs = Vector<double>.Build.Dense(1);
            //_QrsComplex = new List<Tuple<int, Vector<double>>>();
            //_qrsCoefficients = new List<Tuple<int, Vector<double>>>();


            _qrssignal = Vector<double>.Build.Dense(1);
            malinowskaCoefficient = new double();
            pnRatio = new double();
            speedAmpltudeRatio = new double();
            fastSample = new double();
            qRSTime = new double();
            fs = new uint();
            qrsLength = _qrssignal.Count();
            HeartClassData = new Heart_Class_Data();
            List<Vector<double>> coefficients = new List<Vector<double>>();
        }


        #region Documentation
        /// <summary>
        /// Test method of Heart_Class module
        /// </summary>
        #endregion

        /*
        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loadedSignal"></param>
        /// <param name="fs"></param>
        /// <param name="R"></param>
        /// <param name="qrsOnset"></param>
        /// <param name="qrsEnd"></param>
        /// <returns></returns>
        #endregion
        List<Tuple<int, int>> Classification(Vector<double> loadedSignal, uint fs, Vector<double> R, List<int> qrsOnset,
            List<int> qrsEnd)
        {
            Signal = loadedSignal;
            SetQrsComplex();
            QrsCoefficients = CountCoeff(GetQrsComplex(), fs);
            //WCZYTANIE ZBIORU TRENINGOWEGO
            DebugECGPath loader = new DebugECGPath();
            List<Vector<double>> trainDataList = loadFile(System.IO.Path.Combine(loader.getTempPath(), "train_d.txt"));


            //WCZYTANIE ETYKIET ZBIORU TRENINGOWEGO: 0-V, 1-NV
            List<Vector<double>> trainClassList = loadFile(System.IO.Path.Combine(loader.getTempPath(), "train_d_label.txt"));
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

            //Do tesowania:
            List<Vector<double>> testDataList = loadFile(System.IO.Path.Combine(loader.getTempPath(), "test_d.txt"));
            // Tworzenie listy tupli zbioru testowego - w celach testowych (zbior treningowy i testowy wczytywany jest z pliku). 
            //w ostatecznej  wresji testDataList będzie obliczane w programie w formie:  List<Tuple<int, Vector<double>>>: 
            List<Tuple<int, Vector<double>>> testSamples;
            testSamples = new List<Tuple<int, Vector<double>>>();
            Tuple<int, Vector<double>> oneElement;
            int Rpeak = 1;
            foreach (var item in testDataList)
            {
                oneElement = new Tuple<int, Vector<double>>(Rpeak, item.Clone());
                testSamples.Add(oneElement);
            }

            //KLASYFIKACJA
            return HeartClassData.ClassificationResult = TestKnnCase(trainDataList, QrsCoefficients, trainClass, 1); // klasyfikacja sygnału signal
        }

        */

        #region Documentation
        /// <summary>
        /// TODO 
        /// </summary>
        /// <param name="loadedSignal"></param>
        /// <param name="fs"></param>
        /// <param name="R"></param>
        /// <param name="qrsOnset"></param>
        /// <param name="qrsEnd"></param>
        /// <returns></returns>
        #endregion
        Tuple<int, int> ClassificationOneQrs(Vector<double> loadedSignal, int qrsOnset, int qrsEnd, double R)
        {
            Signal = loadedSignal;
            OneQrsComplex(qrsOnset, qrsEnd, R);
            CountCoeffOne(QrsComplexOne, fs);
            //WCZYTANIE ZBIORU TRENINGOWEGO
            DebugECGPath loader = new DebugECGPath();
            List<Vector<double>> trainDataList = loadFile(System.IO.Path.Combine(loader.getTempPath(), "train_d.txt"));


            //WCZYTANIE ETYKIET ZBIORU TRENINGOWEGO: 0-V, 1-NV
            List<Vector<double>> trainClassList = loadFile(System.IO.Path.Combine(loader.getTempPath(), "train_d_label.txt"));
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


            return ClassificationResultOne = TestKnnCaseOne(trainDataList, QrsCoeffOne, trainClass, 3);
   
        }

        /*
        #region Documentation
        /// <summary>
        /// This method uses data from WAVES module (Qrs_onset and Qrs_end) and extracts single QRS complexes, creating list of Tuple. Each tuple contains int value - number of R peaks corresponding to the QRS complex, and vector - containing following signal samples. 
        /// </summary>
        #endregion
        private void SetQrsComplex()
        {
            for (int i = 0; i < QrsNumber; i++)
            {
                //OneQrsComplex();
                _currentQRSComplex++;
            }
        }
        */
        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="singleQrsOnset"></param>
        /// <param name="signleQrsEnd"></param>
        /// <param name="singleQrsR"></param>
        #endregion
        private void OneQrsComplex(double singleQrsOnset, double signleQrsEnd, double singleQrsR)
        {
            int qrsLength = (int)(signleQrsEnd - singleQrsOnset + 1);
            SingleQrs = Vector<double>.Build.Dense(qrsLength);

            if ((int)singleQrsOnset != -1) //modul WAVES daje na wyjściu -1 jeśli zespół nie został wykryty
            {
                Signal.CopySubVectorTo(SingleQrs, sourceIndex: (int)singleQrsOnset, targetIndex: 0,
                    count: qrsLength);
                Tuple<int, Vector<double>> a = new Tuple<int, Vector<double>>((int)singleQrsR, SingleQrs);
                QrsComplexOne = a;
            }
        }

        /*
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
        */

        #region Documentation
        /// <summary>
        /// This method calculates Malinowska's factor as one of shape coefficients using a single QRS complex (_qrssignal) and sampling frequency (fs).
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
        /// This method is a sub method used in CountMalinowskaFactor(). It integrates the signal.
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
        /// This method is a sub method used in CountMalinowskaFactor(). It calculates the perimeter of the signal.
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


        /*
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
                //singleCoeffVect[4] = QrsDuration(data.Item2, fs); //brak uzycia w projekcie
                
                coeffTuple = new Tuple<int, Vector<double>>(singleQrsR, singleCoeffVect.Clone());
                
                result.Add(coeffTuple);
                
            }
            return result;
        }
        */

        #region Documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_QrsComplexOne"></param>
        /// <param name="fs"></param>
        /// <returns></returns>
        #endregion
        Tuple<int, Vector<double>> CountCoeffOne(Tuple<int, Vector<double>> _QrsComplexOne, uint fs)
        {
            Vector<double> singleCoeffVect;
            singleCoeffVect = Vector<double>.Build.Dense(4); // (5) jeśli dodamy czas trwania zespołu
            int singleQrsR;
            Tuple<int, Vector<double>> coeffTuple;

                singleQrsR = _QrsComplexOne.Item1;
                singleCoeffVect[0] = CountMalinowskaFactor(_QrsComplexOne.Item2, fs);
                singleCoeffVect[1] = PnRatio(_QrsComplexOne.Item2);
                singleCoeffVect[2] = SpeedAmpRatio(_QrsComplexOne.Item2);
                singleCoeffVect[3] = FastSampleCount(_QrsComplexOne.Item2);
                //singleCoeffVect[4] = QrsDuration(data.Item2, fs);

                coeffTuple = new Tuple<int, Vector<double>>(singleQrsR, singleCoeffVect.Clone());
                QrsCoeffOne = coeffTuple;
                return QrsCoeffOne;
        }
        /*
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
        */

        Tuple<int, int> TestKnnCaseOne(List<Vector<double>> trainSamples, Tuple<int, Vector<double>> testSamples,
           List<int> trainClasses, int K)
        {
            Tuple< int, int> testResults;// = new Tuple<int, int>();
            testResults = new Tuple<int, int>(0,0);
            int classResult;
            //var testNumber = testSamples.Count();
            var trainNumber = trainSamples.Count();
            Tuple<int, int> resultTuple;
            int singleQrsR;

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

                singleQrsR = testSamples.Item1;
                resultTuple = new Tuple<int, int>(singleQrsR, classResult);

                
                return resultTuple;
        }






        #region Documentation
        /// <summary>
        /// Calculates and returns square of Euclidean distance between two vectors
        /// </summary>
        /// <param name="sample1"></param>
        /// <param name="sample2"></param>
        /// <returns></returns>
        #endregion
        double GetDistance(Vector<double> sample1, Vector<double> sample2)
        {
            var distance = 0.0;
            // zakładamy że sample 1 i sample 2 są tej samej długości 

            for (var i = 0; i < sample1.Count; i++)
            {
                var temp = sample1.At(i) - sample2.At(i);
                distance += temp * temp;
            }
            
            return Math.Sqrt(distance);
        }

        #region Documentation
        /// <summary>
        /// Method that loads training set with training labels
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
        /// Method that changes strings from one line of .txt file to Vector
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


        

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        public Vector<double> Signal
        {
            get { return _signal; }
            set { _signal = value; }
        }
 
        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        public Vector<double> QrsR
        {
            get { return _qrsR; }
            set { _qrsR = value; }
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        public int QrsNumber
        {
            get { return _qrsNumber; }
            set { _qrsNumber = value; }
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        public Vector<double> SingleQrs
        {
            get { return _singleQrs; }
            set { _singleQrs = value; }
        }
        /*
        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        public List<Tuple<int, Vector<double>>> QrsComplex
        {
            get { return _QrsComplex; }
            set { _QrsComplex = value; }
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        public List<Tuple<int, Vector<double>>> QrsCoefficients
        {
            get { return _qrsCoefficients; }
            set { _qrsCoefficients = value; }
        }
        */
        public List<int> QrsOnset
        {
            get { return _qrsOnset; }
            set { _qrsOnset = value; }
        }

        public List<int> QrsEnd
        {
            get { return _qrsEnd; }
            set { _qrsEnd = value; }
        }

        public Tuple<int, Vector<double>> QrsComplexOne
        {
            get { return _QrsComplexOne; }
            set { _QrsComplexOne = value; }
        }

        public Tuple<int, Vector<double>> QrsCoeffOne
        {
            get { return _qrsCoeffOne; }
            set { _qrsCoeffOne = value; }
        }
        
        public Tuple<int, int> ClassificationResultOne
        {
            get { return _classificationResultOne; }
            set { _classificationResultOne = value; }
        }
    }

}