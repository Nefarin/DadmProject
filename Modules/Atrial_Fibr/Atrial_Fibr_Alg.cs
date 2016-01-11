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
    public partial class Atrial_Fibr : IModule
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

        public Atrial_Fibr()
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
            //Atrial_Fibr af = new Atrial_Fibr();
            //Tuple<bool, Vector<double>, double> Wynik = new Tuple<bool, Vector<double>, double>(false, af.pointsDetected, 0);
            ////TempInput.setInputFilePath(@"C:\Users\Anna\Desktop\int3.txt");
            //TempInput.setInputFilePath(@"E:\Studia\ROK 5\DADM\projekt\int3.txt");
            //af.Fs = 250;
            //af.Param.Method = Detect_Method.POINCARE;
            //af.RR_intervals = TempInput.getSignal();
            ////TempInput.setInputFilePath(@"C:\Users\Anna\Desktop\qrs3.txt");
            //TempInput.setInputFilePath(@"E:\Studia\ROK 5\DADM\projekt\qrs3.txt");
            //af.R_Peaks = TempInput.getSignal();

            //Wynik = af.detectAF(af.RR_intervals, af.R_Peaks, af.Fs, af.Param);

            //if (Wynik.Item1)
            //{
            //    Console.WriteLine("MIGOTANIE PRZEDSIONKOW");
            //    Console.ReadLine();
            //}
            //else
            //{
            //    Console.WriteLine("BRAK MIGOTANIA");
            //    Console.ReadLine();
            //}
            //Console.WriteLine(Wynik.Item2.ToString());
            //Console.ReadLine();
            //Console.WriteLine(Wynik.Item2.Count());

            //Console.ReadLine();


        //}
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


        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="_rrIntervals" </param>
        /// <param name="_rPeaks" </param>
        /// <param name="fs" </param>
        /// <param name="_method" </param>
        /// <returns></returns>
        #endregion
        private Tuple<bool, Vector<double>, double> detectAF(Vector<double> _rrIntervals, Vector<double> _rPeaks, uint fs, Atrial_Fibr_Params _method)
        {
            double tmp;
            bool[] detectedIntervals;
            int dividingFactor, nrOfParts;
            double lengthOfDetection=0;

            if (_method.Method== Detect_Method.STATISTIC)
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
            detectedIntervals = new bool[nrOfParts*dividingFactor];
            bool detectedPart = false;
            for (int i = 0; i < nrOfParts; i++)
            {
                _rrIntervals.CopySubVectorTo(partOfRrIntervals, i * dividingFactor, 0, dividingFactor);
                if (_method.Method == Detect_Method.STATISTIC)
                {
                    detectedPart = detectAFStat(partOfRrIntervals, fs);
                }
                else
                {
                    detectedPart = detectAFPoin(partOfRrIntervals, fs);
                }
                
                for (int j = 0; j < dividingFactor; j++)
                {
                    detectedIntervals[i*dividingFactor + j] = detectedPart;
                }
            }
            double lengthOfDetectedIntervals = 0.0;
            bool afDetected = false;
            for (int i=0; i < detectedIntervals.Length; i++)
            {
                if (detectedIntervals[i])
                {
                    lengthOfDetectedIntervals += _rrIntervals.At(i);
                    afDetected = true;
                }
            }
            //string afDetectedS;
            //string afDetectionDescription="";
            if (afDetected)
            {
                pointsDetected= Vector<Double>.Build.Dense(Convert.ToInt32(lengthOfDetectedIntervals));
                int lastIndex = 0;
                for (int i = 0; i < detectedIntervals.Length; i++)
                {
                    if (detectedIntervals[i])
                    {
                        int j;
                        for (j = 0; j <  _rrIntervals.At(i); j++)
                        {
                            pointsDetected.At(j + lastIndex, _rPeaks.At(i) + j);
                        }
                        lastIndex += j;
                    }
                }
                //afDetectedS = "Wykryto migotanie przedsionków.";
                double lengthOfSignal = (_rPeaks.At(_rPeaks.Count-1) - _rPeaks.At(0)) / fs;
                //lengthOfDetection = lengthOfDetectedIntervals / fs;
                //percentOfDetection = (lengthOfDetection / lengthOfSignal) * 100;
                //afDetectionDescription += "Wykryto migotanie trwające ";
                //afDetectionDescription += lengthOfDetection.ToString("F1", CultureInfo.InvariantCulture);
                //afDetectionDescription += "s. Stanowi to ";
                //afDetectionDescription += percentOfDetection.ToString("F1", CultureInfo.InvariantCulture);
                //afDetectionDescription += "% trwania sygnału.";
            }
            else
            {
                pointsDetected=Vector<Double>.Build.Dense(1);
                //afDetectedS="Nie wykryto migotania przedsionków";
            }
            Tuple<bool, Vector<double>, double> result = Tuple.Create(afDetected, pointsDetected, lengthOfDetection);
            return result;
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="_RR"></param>
        /// <returns></returns>
        #endregion
        double TPR(Vector<double> _RR)
        {
            int turningPoints = 0;
            for (int i = 1; i < (_RR.Count - 1); i++)
            {
                if (((_RR.At(i - 1) < _RR.At(i)) && (_RR.At(i + 1) < _RR.At(i))) || ((_RR.At(i - 1) > _RR.At(i)) && (_RR.At(i + 1) > _RR.At(i))))
                {
                    turningPoints++;
                }
            }
            return tpr = turningPoints / ((2 * 30 - 4) / 3);
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="_RR"></param>
        /// <returns></returns>
        #endregion
        double SE(Vector<double> _RR)
        {
            double[] histogram = new double[8];
            double dzielnik = 0.0;
            double maxRr = _RR.Maximum();
            double minRr = _RR.Minimum();
            double width = (maxRr - minRr) / 8;
            double tmp = minRr;
            Vector<double> _RR1;
            _RR1 = _RR;
            List<Tuple<int, double>> listOfElements = new List<Tuple<int, double>>();
            for (int i = 0; i < 8; i++)
            {
                if (i < 7)
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
                }
                else
                {
                    dzielnik = (32 - (histogram.Sum() * 24.0)) / 24.0;
                    histogram[i] = dzielnik;
                }
                tmp += width;
            }
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
        /// TODO
        /// </summary>
        /// <param name="_RR"></param>
        /// <returns></returns>
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

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="_RR"></param>
        /// <param name="fs"></param>
        /// <returns></returns>
        #endregion
        bool detectAFStat(Vector<double> _RR, uint fs )
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

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="_RR"></param>
        /// <param name="fs"></param>
        /// <returns></returns>
        #endregion
        bool detectAFPoin(Vector<double> _RR, uint fs)
        {
            bool AF = false;
            double[] Ii;
            double[] Ii1;
            int length = _RR.Count;
            if (length % 2 == 0)
            {
                int tmp = length / 2;
                Ii = new double[tmp];
                Ii1 = new double[tmp];
                int j=0;

                for (int i = 0; i < 2* tmp; i = i + 2)
                {
                    Ii[j] = _RR.At(i);
                    Ii1[j] = _RR.At(i+1);
                    j++;    
                }
            }
            else
            {
                double tmp = length / 2;
                int size = Convert.ToInt32(Math.Ceiling(tmp));
                Ii = new double[size];
                Ii1 = new double[size];
                int j = 0;

                for (int i = 0; i < size; i = i + 2)
                {
                    Ii[j] = _RR.At(i);
                    j++;
                }

                j = 0;

                for (int i = 1; i < (size-1); i = i + 2)
                {
                    Ii1[j] = _RR.At(i);
                    j++;
                }
            }
            double[] A1 = new double[Ii.Length];
            for (int i = 0; i < Ii.Length;i++)
            {
                A1[i] = Ii[i] - Ii1[i];
            }
            double[] A2 = new double[A1.Length];

            for(int i=0;i<A1.Length;i++)
            {
                A2[i] = Math.Pow(A1[i],2);
            }

            double suma = 0;
            for(int i = 0; i < A2.Length; i++)
            {
                suma += A2[i];
            }

            double[] A3 = new double [A1.Length];
            for (int i = 0; i < A1.Length; i++)
            {
                A3[i] = Math.Abs(A1[i]);
            }

            double suma_modul = 0;
            for (int i = 0; i < A3.Length; i++)
            {
                suma_modul += A3[i];
            }

            double C,D,E,F,G,d;
            C = suma / (2 * length - 2);
            D = suma_modul / ((length - 1) * Math.Sqrt(2));
            E = Math.Pow(D, 2);
            F = Math.Sqrt(C - E);

            double sum = 0;
            for(int i = 0; i < Ii.Length; i++)
            {
                sum += Ii[i];
            }

            G = (-_RR.At(1) - _RR.At(_RR.Count - 1) + 2 * sum) / (2 * length - 2);
            d = F / G;

            if (d <= 0.06)
                AF = false;
            else
            {
                double[] sil = new double[5];
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

   
        public class DataPoint
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
            { }
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="Vector1"></param>
        /// <param name="Vector2"></param>
        /// <returns></returns>
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

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="_rawDataToCluster"></param>
        /// <returns></returns>
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

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="_rawDataToCluster"></param>
        /// <param name="_normalizedDataToCluster"></param>
        /// <returns></returns>
        #endregion
        private Tuple<List<DataPoint>,List<DataPoint>> InitializeCentroids(List<DataPoint> _rawDataToCluster, List<DataPoint> _normalizedDataToCluster)
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
            Tuple<List<DataPoint>, List<DataPoint>> result = new Tuple<List<DataPoint>, List<DataPoint>> (_normalizedDataToCluster, _rawDataToCluster);
            return result;
        }

        List<DataPoint> _clusters = new List<DataPoint>();

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="_normalizedDataToCluster"></param>
        /// <param name="_clusters"></param>
        /// <returns></returns>
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

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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


        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="dataPoint"></param>
        /// <param name="mean"></param>
        /// <returns></returns>
        #endregion
        private double ElucidanDistance(DataPoint dataPoint, DataPoint mean)
        {
            double _diffs = 0.0;
            _diffs = Math.Pow(dataPoint.A - mean.A, 2);
            _diffs += Math.Pow(dataPoint.B - mean.B, 2);
            return Math.Sqrt(_diffs);
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="_rawDataToCluster"></param>
        /// <param name="_normalizedDataToCluster"></param>
        /// <param name="_clusters"></param>
        /// <returns></returns>
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

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="distances"></param>
        /// <returns></returns>
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

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="_rawDataToCluster"></param>
        /// <param name="_normalizedDataToCluster"></param>
        /// <param name="_clusters"></param>
        /// <returns></returns>
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

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="_rawDataToCluster"></param>
        /// <returns></returns>
        #endregion
        private double SilhouetteCoefficient(List<DataPoint> _rawDataToCluster)
        {
            double SilhouetteCoeff = new double();
            List<double> distanceIn_a = new List<double>();
            List<double> distanceIn_b = new List<double>();
            List<double> distanceOut_a = new List<double>();
            List<double> distanceOut_b = new List<double>();
            List<double> a_mean_In = new List<double>();
            List<double> b_mean_In = new List<double>();
            List<double> a_mean_Out = new List<double>();
            List<double> b_mean_Out = new List<double>();
            List<double> dist_In = new List<double>();
            List<double> dist_Out = new List<double>();
            List<double> Silh = new List<double>();

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
                    a_mean_In.Add(0.0);
                    b_mean_In.Add(0.0);
                    a_mean_Out.Add(distanceOut_a.Average());
                    b_mean_Out.Add(distanceOut_b.Average());
                }
                else if (distanceOut_a.Count == 0)
                {
                    a_mean_Out.Add(0.0);
                    b_mean_Out.Add(0.0);
                    a_mean_In.Add(distanceIn_a.Average());
                    b_mean_In.Add(distanceIn_b.Average());
                    //dist_Out.Add(0.0);
                }
                else
                {

                    a_mean_In.Add(distanceIn_a.Average());
                    b_mean_In.Add(distanceIn_b.Average());
                    a_mean_Out.Add(distanceOut_a.Average());
                    b_mean_Out.Add(distanceOut_b.Average());

                }
                if (a_mean_In.Count == 1 && a_mean_In[0] == 0.0)
                {
                    dist_In.Add(0.0);
                    dist_Out.Add(Math.Sqrt(Math.Pow(a_mean_Out[0], 2) + Math.Pow(b_mean_Out[0], 2)));

                }
                else if (a_mean_Out.Count == 1 && a_mean_Out[0] == 0.0)
                {
                    dist_Out.Add(0.0);
                    dist_In.Add(Math.Sqrt(Math.Pow(a_mean_In[0], 2) + Math.Pow(b_mean_In[0], 2)));

                }
                else
                {
                    dist_In.Add(Math.Sqrt(Math.Pow(a_mean_In[0], 2) + Math.Pow(b_mean_In[0], 2)));
                    dist_Out.Add(Math.Sqrt(Math.Pow(a_mean_Out[0], 2) + Math.Pow(b_mean_Out[0], 2)));
                }
                Silh.Add(Math.Abs(dist_Out[0] - dist_In[0]) / Math.Max(dist_Out[0], dist_In[0]));
                distanceIn_a.Clear();
                distanceIn_b.Clear();
                distanceOut_a.Clear();
                distanceOut_b.Clear();
                a_mean_In.Clear();
                b_mean_In.Clear();
                a_mean_Out.Clear();
                b_mean_Out.Clear();
                dist_In.Clear();
                dist_Out.Clear();
            }
            SilhouetteCoeff = Silh.Average();

            Silh.Clear();
            return SilhouetteCoeff;
        }

    }
}
