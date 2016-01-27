using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Ink;
using EKG_Project.IO;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;


namespace EKG_Project.Modules.Atrial_Fibr
{
    #region Documentation
    /// <summary>
    /// Class which detect Atrial Fibrillation in ECG signal.
    /// </summary>
    #endregion
    public class Atrial_Fibr_Alg
    {
        private uint fs;
        private Vector<double> _rr_intervals;
        private Vector<double> _r_Peaks;
        private Vector <double> pointsDetected;
        bool migotanie;
        double tpr, se, rmssd;
        int amountOfCluster;
        List<DataPoint> _RawData;
        List<DataPoint> _NormalizedData;
        List<DataPoint> _ClusteredData;
        Tuple<bool, Vector<double>, double> _wynik;
        Atrial_Fibr_Params param;

        #region Documentation
        /// <summary>
        /// Declaration of fields of class Atrial_Fibr
        /// </summary>
        #endregion
        public Atrial_Fibr_Alg()
        {
            Param = new Atrial_Fibr_Params();
            Fs = new uint();
            pointsDetected= Vector<double>.Build.Dense(1);
            _rr_intervals = Vector<double>.Build.Dense(1);
            _r_Peaks= Vector<double>.Build.Dense(1);
            migotanie = new bool();
            tpr = new double();
            se = new double();
            rmssd = new double();
            amountOfCluster = new int();
            _RawData = new List<DataPoint>();
            _NormalizedData = new List<DataPoint>();
            _ClusteredData = new List<DataPoint>();
        }


        //public static void Main()
        //{
        //    Atrial_Fibr af = new Atrial_Fibr();
        //    Tuple<bool, Vector<double>, double> Wynik = new Tuple<bool, Vector<double>, double>(false, af.pointsDetected, 0);
        //    //TempInput.setInputFilePath(@"C:\Users\Anna\Desktop\int3.txt");
        //    TempInput.setInputFilePath(@"E:\Studia\ROK 5\DADM\projekt\int3.txt");
        //    af.Fs = 250;
        //    af.Param.Method = Detect_Method.POINCARE;
        //    af.RR_intervals = TempInput.getSignal();
        //    //TempInput.setInputFilePath(@"C:\Users\Anna\Desktop\qrs3.txt");
        //    TempInput.setInputFilePath(@"E:\Studia\ROK 5\DADM\projekt\qrs3.txt");
        //    af.R_Peaks = TempInput.getSignal();

        //    Wynik = af.detectAF(af.RR_intervals, af.R_Peaks, af.Fs, af.Param);

        //    if (Wynik.Item1)
        //    {
        //        Console.WriteLine("MIGOTANIE PRZEDSIONKOW");
        //        Console.ReadLine();
        //    }
        //    else
        //    {
        //        Console.WriteLine("BRAK MIGOTANIA");
        //        Console.ReadLine();
        //    }
        //    Console.WriteLine(Wynik.Item2.ToString());
        //    Console.ReadLine();
        //    Console.WriteLine(Wynik.Item2.Count());

        //    Console.ReadLine();


        //}

        //Getery i setery////////////////////////////////////////////////////////////////////////////////////////
        public Atrial_Fibr_Params Param
        {
            get { return param; }
            set { param = value; }
        }
        public uint Fs
        {
            get { return fs; }
            set { fs = value; }
        }
        public Vector<double> RR_intervals
        {
            get { return _rr_intervals; }
            set { _rr_intervals = value; }
        }
        public Tuple<bool, Vector<double>, double> Wynik
        {
            get { return _wynik; }
            set { _wynik = value; }
        }
        public Vector<double> R_Peaks
        {
            get { return _r_Peaks; }
            set { _r_Peaks = value; }
        }

        public Vector<double> Points_Detected
        {
            get { return pointsDetected; }
            set { pointsDetected = value; }
        }

        public List<DataPoint> RawData
        {
            get { return _RawData; }
            set { _RawData = value; }
        }

        public List<DataPoint> NormalizedData
        {
            get { return _NormalizedData; }
            set { _NormalizedData = value; }
        }

        public List<DataPoint> ClusteredData
        {
            get { return _ClusteredData; }
            set { _ClusteredData = value; }
        }

        //Główna metoda/////////////////////////////////////////////////////////////////////////////////////////////////
        #region Documentation
        /// <summary>
        /// Detection of Atrial Fibrillation.
        /// </summary>
        /// <param name="_rrIntervals"> Intervals between R peaks</param>
        /// <param name="_rPeaks"> R peaks found in ECG signal </param>
        /// <param name="fs"> Frequency of sampling for ECG siganl</param>
        /// <param name="_method"> Paramethers of detection </param>
        /// <returns>Boolean inforamtion if AF was detected, vector of points with AF detected, length of detected AF in s.</returns>
        #endregion
        private Tuple<bool, Vector<double>, double> detectAF(Vector<double> _rrIntervals, Vector<double> _rPeaks, uint fs, Atrial_Fibr_Params _method)
        {
            if (_rrIntervals.Exists(x => x <= 0))
            {
                pointsDetected = Vector<Double>.Build.Dense(1);
                Tuple<bool, Vector<double>, double> result = Tuple.Create(false, pointsDetected, 0.0);
                return result;
            }
            else
            {
                double tmp;
                bool[] detectedIntervals;
                int dividingFactor, nrOfParts;
                double lengthOfDetection = 0;

                if (_method.Method == Detect_Method.STATISTIC)
                {
                    dividingFactor = 32;
                }
                else
                {
                    dividingFactor = 30;
                }
                tmp = _rrIntervals.Count / dividingFactor;
                nrOfParts = Convert.ToInt32(Math.Floor(tmp));
                Vector<double> partOfRrIntervals = Vector<double>.Build.Dense(dividingFactor);
                detectedIntervals = new bool[nrOfParts * dividingFactor];
                bool detectedPart = false;
                for (int i = 0; i < nrOfParts; i++)
                {
                    _rrIntervals.CopySubVectorTo(partOfRrIntervals, i * dividingFactor, 0, dividingFactor);
                    if (_method.Method == Detect_Method.STATISTIC)
                    {
                        detectedPart = detectAFStat(partOfRrIntervals);
                    }
                    else
                    {
                        detectedPart = detectAFPoin(partOfRrIntervals);
                    }

                    for (int j = 0; j < dividingFactor; j++)
                    {
                        detectedIntervals[i * dividingFactor + j] = detectedPart;
                    }
                }
                double lengthOfDetectedIntervals = 0.0;
                bool afDetected = false;
                for (int i = 0; i < detectedIntervals.Length; i++)
                {
                    if (detectedIntervals[i])
                    {
                        lengthOfDetectedIntervals += _rrIntervals.At(i);
                        afDetected = true;
                    }
                }
                if (afDetected)
                {
                    pointsDetected = Vector<Double>.Build.Dense(Convert.ToInt32(lengthOfDetectedIntervals));
                    int lastIndex = 0;
                    for (int i = 0; i < detectedIntervals.Length; i++)
                    {
                        if (detectedIntervals[i])
                        {
                            int j;
                            for (j = 0; j < _rrIntervals.At(i); j++)
                            {
                                pointsDetected.At(j + lastIndex, _rPeaks.At(i) + j);
                            }
                            lastIndex += j;
                        }
                    }
                    double lengthOfSignal = (_rPeaks.At(_rPeaks.Count - 1) - _rPeaks.At(0)) / fs;
                }
                else
                {
                    pointsDetected = Vector<Double>.Build.Dense(1);
                }
                Tuple<bool, Vector<double>, double> result = Tuple.Create(afDetected, pointsDetected, lengthOfDetectedIntervals / fs);
                return result;
            }
        }
        //Funkcje pomocnicze do metody statystycznej///////////////////////////////////////////////////////////////////
        #region Documentation
        /// <summary>
        /// Calculation of turning point ratio.
        /// </summary>
        /// <param name="_RR"> Intervals between R peaks</param>
        /// <returns>Turning point ratio</returns>
        #endregion
        double TPR(Vector<double> _RR)
        {
            double turningPoints = 0;
            for (int i = 1; i < (_RR.Count - 1); i++)
            {
                if (((_RR.At(i - 1) < _RR.At(i)) && (_RR.At(i + 1) < _RR.At(i))) || ((_RR.At(i - 1) > _RR.At(i)) && (_RR.At(i + 1) > _RR.At(i))))
                {
                    turningPoints++;
                }
            }
            return tpr = turningPoints / ((2 * 32 - 4) / 3);
        }

        #region Documentation
        /// <summary>
        /// Calculation of Shannon entropy.
        /// </summary>
        /// <param name="_RR"> Intervals between R peaks</param>
        /// <returns>Shannon entropy</returns>
        #endregion
        double SE(Vector<double> _RR)
        {
            double[] histogram = { 0, 0, 0, 0, 0, 0, 0, 0 };
            double dzielnik = 0.0;
            double maxRr = _RR.Maximum();
            double minRr = _RR.Minimum();
            double width = (maxRr - minRr) / 8;
            double tmp = minRr;
            Vector<double> _RR1;
            _RR1 = _RR;
            List<Tuple<int, double>> listOfElements = new List<Tuple<int, double>>();
            int i;
            for (i = 0; i < 7; i++)
            {
                 for (int k = 0; k < _RR1.Count; k++)
                 {
                    Tuple<int, double> elements = _RR1.Find(element => (element >= tmp && element < (tmp + width)));
                    if (elements != null)
                    {
                        listOfElements.Add(elements);
                        _RR1.ClearSubVector(elements.Item1, 1);
                    }
                    else
                    {
                        break;
                    }
                }
                dzielnik = listOfElements.Count / 24.0;
                histogram[i] = dzielnik;
                listOfElements.Clear();
                tmp += width;
            }
            dzielnik = (32 - (histogram.Sum() * 24.0)) / 24.0;
            histogram[7] = dzielnik;
            tmp += width;
            double se = 0;
            foreach (double a in histogram)
            {
                if (a != 0)
                    se += (a * Math.Log10(a)) / (Math.Log10(0.125));
            }
            return se;
        }

        #region Documentation
        /// <summary>
        /// Calculation of root mean square of successive differences.
        /// </summary>
        /// <param name="_RR"> Intervals between R peaks</param>
        /// <returns>Root mean square of successive differences</returns>
        #endregion
        double RMSSD(Vector<double> _RR)
        {
            double[] rmssd_vec = new double[_RR.Count - 1];
            double rmssd = 0.0;
            for (int i = 0; i < (_RR.Count - 1); i++)
            {
                rmssd_vec[i] = Math.Pow(_RR.At(i + 1) - _RR.At(i), 2);
            }
            rmssd = rmssd_vec.Sum();
            rmssd = rmssd / (_RR.Count - 1);
            rmssd = Math.Sqrt(rmssd);
            return (rmssd = rmssd / _RR.Average());
        }
        //Metoda statystyczna///////////////////////////////////////////////////////////////////////////////////////
        #region Documentation
        /// <summary>
        /// Detection of Atrial Fibrilation using Statistic method.
        /// </summary>
        /// <param name="_RR"> Intervals between R peaks</param>
        /// <returns> Boolean information if AF was detected.</returns>
        #endregion
        bool detectAFStat(Vector<double> _RR)
        {
           bool AF, tprD, seD, rmssdD;

           //Turning Punct Ratio
           tpr = TPR(_RR);
           if (tpr < 0.77 && tpr > 0.54)
           {
              tprD = false;
           }
           else
           {
              tprD = true;
           }

           //RMSSD
           rmssd = RMSSD(_RR);
           if (rmssd > 0.1)
           {
               rmssdD = true;
           }
           else
           {
               rmssdD = false;
           }
           
           //Shannon Entrophy
           se = SE(_RR);
           if (se > 0.7)
           {
               seD = true;
           }
           else
           {
               seD = false;
           }
           if (tprD && seD && rmssdD)
           {
               AF = true;
           }
           else
           {
               AF = false;
           }
           return migotanie = AF;
        }
        //Metoda nieliniowa///////////////////////////////////////////////////////////////////////////////////////////////
        #region Documentation
        /// <summary>
        /// Detection of Atrial Fibrilation using Poincare method.
        /// </summary>
        /// <param name="_RR">Intervals between R peaks</param>
        /// <returns>Boolean information if AF was detected.</returns>
        #endregion
        bool detectAFPoin(Vector<double> _RR)
        {
            bool AF = false;
            double[] Ii;
            double[] Ii1;
            int length = _RR.Count;
            int tmp = length - 1 ;
            Ii = new double[tmp];
            Ii1 = new double[tmp];
            int j=0;

            for (int i = 0; i < tmp; i = i + 1)
            {
                 Ii[j] = _RR.At(i);
                 Ii1[j] = _RR.At(i+1);
                 j++;    
            }
            double d = dCoeff(Ii, Ii1, _RR);
            if (d <= 0.06)
                AF = false;
            else
            {
                double[] sil = new double[4];
                int counter = new int();
                counter = 0;
                RawData = InitilizeRawData(Ii, Ii1);
                NormalizedData = NormalizeData(RawData);
                for (amountOfCluster = 2; amountOfCluster < 6; amountOfCluster++)
                {
                    RawData = Cluster(RawData, NormalizedData, ClusteredData);
                    sil[counter] = SilhouetteCoefficient(RawData);
                    counter++;
                }

                if (sil.Max() < 0.92) AF = true;
                else AF = false;
            }
            return AF;
        }

        //Funkcje i klasy pomocnicze do metody nieliniowej////////////////////////////////////////////////////////////////
        private double dCoeff(double[] Ii,double[] Ii1, Vector<double> _RR)
        {
            int length = _RR.Count;
            double[] A1 = new double[Ii.Length];
            for (int i = 0; i < Ii.Length; i++)
            {
                A1[i] = Ii[i] - Ii1[i];
            }
            double[] A2 = new double[A1.Length];

            for (int i = 0; i < A1.Length; i++)
            {
                A2[i] = Math.Pow(A1[i], 2);
            }

            double suma = 0;
            for (int i = 0; i < A2.Length; i++)
            {
                suma += A2[i];
            }

            double[] A3 = new double[A1.Length];
            for (int i = 0; i < A1.Length; i++)
            {
                A3[i] = Math.Abs(A1[i]);
            }

            double suma_modul = 0;
            for (int i = 0; i < A3.Length; i++)
            {
                suma_modul += A3[i];
            }

            double C, D, E, F, G, d;
            C = suma / (2 * length - 2);
            D = suma_modul / ((length - 1) * Math.Sqrt(2));
            E = Math.Pow(D, 2);
            F = Math.Sqrt(C - E);

            double sum = 0;
            for (int i = 0; i < Ii.Length; i++)
            {
                sum += Ii[i];
            }

            G = (-_RR.At(0) - _RR.At(_RR.Count - 1) + 2 * sum) / (2 * length - 2);
            d = F / G;
            return d;
        }



        #region Documentation
        /// <summary>
        /// Data point in clustering. Contain information about coordinates and number of cluster.
        /// </summary>
        #endregion
        public class DataPoint : System.Object
        {
            public double A { get; set; }
            public double B { get; set; }
            public int Cluster { get; set; }
            public DataPoint(double a, double b)
            {
                A = a;
                B = b;
                Cluster = 0;
            }

            public DataPoint()
            {
                A = 0.0;
                B = 0.0;
                Cluster = 0;
            }

            public DataPoint(double a, double b, int c)
            {
                A = a;
                B = b;
                Cluster = c;
            }
            public bool Equals(DataPoint point)
            {
                return (Math.Abs(A - point.A) < 0.0001) && (Math.Abs(B - point.B) < 0.0001) && (Cluster == point.Cluster);
            }

            public override bool Equals(System.Object obj)
            {
                // If parameter is null return false.
                if (obj == null)
                {
                    return false;
                }

                // If parameter cannot be cast to Point return false.
                DataPoint point = obj as DataPoint;
                if ((System.Object)point == null)
                {
                    return false;
                }

                // Return true if the fields match:
                return (Math.Abs(A - point.A)<0.0001) && (Math.Abs(B - point.B)<0.0001) && (Cluster == point.Cluster);
            }
            public override int GetHashCode()
            {
                int tmp = Convert.ToInt32(A) + Convert.ToInt32(B) + Cluster;
                return tmp;
            }

            public static bool operator ==(DataPoint point1, DataPoint point2)
            {
                return (point1.A == point2.A) && (point1.B == point2.B) && (point1.Cluster == point2.Cluster);
            }

            public static bool operator !=(DataPoint point1, DataPoint point2)
            {
                return (point1.A != point2.A) || (point1.B != point2.B) ||(point1.Cluster != point2.Cluster);
            }
        }

        //Inicjalizacja////////////////////////////////////////////////////////////////
        #region Documentation
        /// <summary>
        /// Create list of data points for clusterization.
        /// </summary>
        /// <param name="Vector1">First coordinate</param>
        /// <param name="Vector2">Second coordinate</param>
        /// <returns>List of data points for clusterization</returns>
        #endregion

        private List<DataPoint> InitilizeRawData(double[] Vector1, double[] Vector2)
        {
            List<DataPoint> _rawDataToCluster = new List<DataPoint>();
            for (int i = 0; i < Vector1.Length; i++)
            {
                DataPoint dp = new DataPoint();
                dp.A = Vector1[i];
                dp.B = Vector2[i];
                _rawDataToCluster.Add(dp);
            }
            return _rawDataToCluster;
        }

        //Normalizacja danych////////////////////////////////////////////////////////////////
        #region Documentation
        /// <summary>
        /// Normalize list of data point
        /// </summary>
        /// <param name="_rawDataToCluster">List of data points</param>
        /// <returns>Normalized list of data points</returns>
        #endregion
        private List<DataPoint> NormalizeData(List <DataPoint> _rawDataToCluster)
        {
            List<DataPoint> _normalizedDataToCluster = new List<DataPoint>();
            double aSum = 0.0;
            double bSum = 0.0;
            foreach (DataPoint dataPoint in _rawDataToCluster)
            {
                aSum += dataPoint.A;
                bSum += dataPoint.B;
            }
            double aMean = aSum / _rawDataToCluster.Count;
            double bMean = bSum / _rawDataToCluster.Count;
            double sumA = 0.0;
            double sumB = 0.0;
            foreach (DataPoint dataPoint in _rawDataToCluster)
            {
                sumA += Math.Pow(dataPoint.A - aMean, 2);
                sumB += Math.Pow(dataPoint.B - bMean, 2);
            }
            double aSD = sumA / _rawDataToCluster.Count;
            double bSD = sumB / _rawDataToCluster.Count;
            foreach (DataPoint dataPoint in _rawDataToCluster)
            {
                _normalizedDataToCluster.Add(new DataPoint()
                {
                    A = (dataPoint.A - aMean) / aSD,
                    B = (dataPoint.B - bMean) / bSD
                }
                );
            }
            return _normalizedDataToCluster;
        }

        //Przypisanie punktów do klastrów////////////////////////////////////////////////////////////////
        #region Documentation
        /// <summary>
        /// Initialize centroids for clusterization
        /// </summary>
        /// <param name="_rawDataToCluster">List of data points</param>
        /// <param name="_normalizedDataToCluster">List of normalized data points</param>
        /// <returns></returns>
        #endregion
        private void InitializeCentroids(List<DataPoint> _rawDataToCluster, List<DataPoint> _normalizedDataToCluster)
        {
            for (int i = 0; i < amountOfCluster; ++i)
            {
                _normalizedDataToCluster[i].Cluster = _rawDataToCluster[i].Cluster = i;
            }
            Random random = new Random();
            for (int i = amountOfCluster; i < _normalizedDataToCluster.Count; i++)
            {
                _normalizedDataToCluster[i].Cluster = _rawDataToCluster[i].Cluster = random.Next(0, amountOfCluster);
            }
         }

        //Wyznaczenie środków ciężkości poszczególnych klastrów////////////////////////////////////////////////////////////////
        #region Documentation
        /// <summary>
        /// Update means of data points in each cluster
        /// </summary>
        /// <param name="_normalizedDataToCluster">List of normalized data points</param>
        /// <param name="_clusters">List of means of data points in each cluster</param>
        /// <returns>result testing the clusters if each cluster has at least one member, list of new means of data points </returns>
        #endregion
        private Tuple<bool, List<DataPoint>> UpdateDataPointMeans(List<DataPoint> _normalizedDataToCluster,List<DataPoint> _clusters)
        {
            bool result_bool = new bool();
            if (EmptyCluster(_normalizedDataToCluster))
            {
                result_bool = false;
            }
            else
            {
                _clusters.Clear();
                var groupToComputeMeans = _normalizedDataToCluster.GroupBy(p => p.Cluster).OrderBy(p => p.Key);
                double a = 0.0;
                double b = 0.0;
                foreach (var item in groupToComputeMeans)
                {
                    double size = item.Count();
                    foreach (var value in item)
                    {
                        a += value.A;
                        b += value.B;
                    }
                    _clusters.Add(new DataPoint()
                    {
                        A = a / size,
                        B = b / size
                    }
                    );
                    a = 0.0;
                    b = 0.0;
                }
                result_bool = true;
            }
            Tuple<bool, List<DataPoint>> result = new Tuple<bool, List<DataPoint>>(result_bool, _clusters);
            return result;
        }

        //Sprawdzenie, czy każdy klaster ma co najmniej jeden element////////////////////////////////////////////////////////////////
        #region Documentation
        /// <summary>
        /// Tests the clusters if each cluster has at least one member
        /// </summary>
        /// <param name="data">list of data points</param>
        /// <returns>true if at least one cluster is empty</returns>
        #endregion
        private bool EmptyCluster(List<DataPoint> data)
        {
            bool result = new bool();
            result = false;

            for (int i = 0; i < amountOfCluster; i++)
            {
                if (!data.Exists(x => x.Cluster == i))
                {
                    result = true;
                    break;
                }
            }            
            
            return result;
        }

        //Obliczenie odległości pomiędzy punktem a środkiem ciężkości klastra ////////////////////////////////////////////////////////////////
        #region Documentation
        /// <summary>
        /// Counting difference between an item and the mean of the current members of a cluster
        /// </summary>
        /// <param name="dataPoint">Data point</param>
        /// <param name="mean">Mean of data points in cluster</param>
        /// <returns>Difference between an item and the mean of the current members of a cluster</returns>
        #endregion
        private double ElucidanDistance(DataPoint dataPoint, DataPoint mean)
        {
            double _diffs = 0.0;
            _diffs = Math.Pow(dataPoint.A - mean.A, 2);
            _diffs += Math.Pow(dataPoint.B - mean.B, 2);
            return Math.Sqrt(_diffs);
        }

        //Zmiana przynależności punktu do klastra, jeżeli jego odległość do środka ciężkości innego klastra jest mniejsza////////////////////////////////////////////////////////////////
        #region Documentation
        /// <summary>
        /// Choose the cluster with the minimum difference and move the data point to that cluster
        /// </summary>
        /// <param name="_rawDataToCluster">List of data points</param>
        /// <param name="_normalizedDataToCluster">List of normalized data points</param>
        /// <param name="_clusters">List of means of data points in each cluster</param>
        /// <returns>true if ID of cluster is changed, list of normalized data points, list of data points</returns>
        #endregion
        private Tuple<bool, List<DataPoint>,List<DataPoint>> UpdateClusterMembership(List<DataPoint> _rawDataToCluster, List<DataPoint> _normalizedDataToCluster, List<DataPoint> _clusters)
        {
            bool changed = false;
            double[] distances = new double[amountOfCluster];

            for (int i = 0; i < _normalizedDataToCluster.Count; ++i)
            {

                for (int k = 0; k < amountOfCluster; ++k)
                    distances[k] = ElucidanDistance(_normalizedDataToCluster[i], _clusters[k]);

                int newClusterId = MinIndex(distances);
                if (newClusterId != _normalizedDataToCluster[i].Cluster)
                {
                    changed = true;
                    _normalizedDataToCluster[i].Cluster = _rawDataToCluster[i].Cluster = newClusterId;
                }
            }
            if (EmptyCluster(_normalizedDataToCluster)) changed = false;
            Tuple<bool, List<DataPoint>,List<DataPoint>> result = new Tuple<bool, List<DataPoint>,List<DataPoint>>(changed, _normalizedDataToCluster,_rawDataToCluster);
            _clusters.Clear();
            return result;
        }

        //Szuka minimum w tablicy i zwraca indeks////////////////////////////////////////////////////////////////
        #region Documentation
        /// <summary>
        /// Find the cluster with the minimum difference
        /// </summary>
        /// <param name="distances">Array of distances between point and means of clusters</param>
        /// <returns>Index of cluster with the minimum difference </returns>
        #endregion
        private int MinIndex(double[] distances)
        {
            int _indexOfMin = 0;
            double _smallDist = distances[0];
            for (int k = 0; k < distances.Length; ++k)
            {
                if (distances[k] < _smallDist)
                {
                    _smallDist = distances[k];
                    _indexOfMin = k;
                }
            }
            return _indexOfMin;
        }

        //Algorytm k-średnich////////////////////////////////////////////////////////////////
        #region Documentation
        /// <summary>
        /// K-Means Clustering Algorithm
        /// </summary>
        /// <param name="_rawDataToCluster">List of data points</param>
        /// <param name="_normalizedDataToCluster">List of normalized data points</param>
        /// <param name="_clusters">List of means of data points in each cluster</param>
        /// <returns>New list of data points</returns>
        #endregion
        public List<DataPoint> Cluster(List<DataPoint> _rawDataToCluster, List<DataPoint> _normalizedDataToCluster, List<DataPoint> _clusters)
        {
            bool _changed = true;
            bool _success = true;
            InitializeCentroids(_rawDataToCluster,_normalizedDataToCluster);

            int maxIteration = _rawDataToCluster.Count * 10;
            int _threshold = 0;
            while (_success == true && _changed == true && _threshold < maxIteration)
            {
                ++_threshold;
                _success = UpdateDataPointMeans(_normalizedDataToCluster,_clusters).Item1;
                _changed = UpdateClusterMembership(_rawDataToCluster, _normalizedDataToCluster, UpdateDataPointMeans(_normalizedDataToCluster, _clusters).Item2).Item1;
            }

            return _rawDataToCluster;
        }

        //Obliczenie współczynnika dokładności dopasowania klasteryzacji////////////////////////////////////////////////////////////////
        #region Documentation
        /// <summary>
        /// Counts Silhouette Coefficient
        /// </summary>
        /// <param name="_rawDataToCluster">List of data points</param>
        /// <returns>Silhouette Coefficient </returns>
        #endregion
        private double SilhouetteCoefficient(List<DataPoint> _rawDataToCluster)
        {
            double SilhouetteCoeff = new double();
            List<double> distanceIn_a = new List<double>();
            List<double> distanceIn_b = new List<double>();
            List<double> distanceOut_a = new List<double>();
            List<double> distanceOut_b = new List<double>();
            List<double> Silh = new List<double>();
            double a_mean_In = new double();
            double b_mean_In = new double();
            double a_mean_Out = new double();
            double b_mean_Out = new double();
            double dist_In = new double();
            double dist_Out = new double();

            for (int i = 0; i < _rawDataToCluster.Count; i++)
            {
                for (int j = 0; j < _rawDataToCluster.Count; j++)
                {
                    if (_rawDataToCluster[i].Cluster == _rawDataToCluster[j].Cluster)
                    {
                        if (i != j)
                        {
                            distanceIn_a.Add(Math.Abs(_rawDataToCluster[j].A - _rawDataToCluster[i].A));
                            distanceIn_b.Add(Math.Abs(_rawDataToCluster[j].B - _rawDataToCluster[i].B));
                        }
                    }
                    else
                    {
                        distanceOut_a.Add(Math.Abs(_rawDataToCluster[j].A - _rawDataToCluster[i].A));
                        distanceOut_b.Add(Math.Abs(_rawDataToCluster[j].B - _rawDataToCluster[i].B));
                    }
                }

                if (distanceIn_a.Count == 0)
                {
                    a_mean_In =0.0;
                    b_mean_In = 0.0;
                    a_mean_Out = distanceOut_a.Average();
                    b_mean_Out = distanceOut_b.Average();
                }

                else if (distanceOut_a.Count == 0)
                {
                    a_mean_Out = 0.0;
                    b_mean_Out = 0.0;
                    a_mean_In = distanceIn_a.Average();
                    b_mean_In = distanceIn_b.Average();
                }

                else
                {
                    a_mean_In = distanceIn_a.Average();
                    b_mean_In = distanceIn_b.Average();
                    a_mean_Out = distanceOut_a.Average();
                    b_mean_Out = distanceOut_b.Average();
                }

                if (a_mean_In == 0.0)
                {
                    dist_In = 0.0;
                    dist_Out = (Math.Sqrt(Math.Pow(a_mean_Out, 2) + Math.Pow(b_mean_Out, 2)));
                }

                else if (a_mean_Out == 0.0)
                {
                    dist_Out = 0.0;
                    dist_In = (Math.Sqrt(Math.Pow(a_mean_In, 2) + Math.Pow(b_mean_In, 2)));
                }

                else
                {
                    dist_In = (Math.Sqrt(Math.Pow(a_mean_In, 2) + Math.Pow(b_mean_In, 2)));
                    dist_Out = (Math.Sqrt(Math.Pow(a_mean_Out, 2) + Math.Pow(b_mean_Out, 2)));
                }

                Silh.Add(Math.Abs(dist_Out - dist_In) / Math.Max(dist_Out, dist_In));
                distanceIn_a.Clear();
                distanceIn_b.Clear();
                distanceOut_a.Clear();
                distanceOut_b.Clear();
            }
            SilhouetteCoeff = Silh.Average();

            Silh.Clear();
            return SilhouetteCoeff;
        }

    }
}
