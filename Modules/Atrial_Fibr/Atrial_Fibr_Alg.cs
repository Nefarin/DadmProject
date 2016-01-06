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
    //TODO: 
    //-cały alorytm poincare, k-średnie, wartość dopasowania
    //-testy
    //-komentarze zgodnie z dokumentacją
    public partial class Atrial_Fibr : IModule
    {
        private uint fs;
        //private int[] RR_intervals;
        private Vector<double> RR_intervals;
        private Vector<double> partOfRrIntervals;
        private Vector<double> pointsDetected;

        public static void Main()
        {
            var af = new Atrial_Fibr();

            TempInput.setInputFilePath(@"E:\Studia\ROK 5\DADM\projekt\RR.txt");
            uint fs = TempInput.getFrequency();
            
            af.RR_intervals = TempInput.getSignal();
            af.fs = TempInput.getFrequency();

            af.detectAFStat(af.RR_intervals,af.fs);
        }
        
        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
        private void detectAF (Vector<double> _rrIntervals, uint fs)
        {
            //int[] rrIntervals = new int[(RR.Count - 1)];
            //for (int i=0; i < (RR.Count - 2); i++)
            //{
            //    rrIntervals[i] = RR[i + 1] - RR[i];
            //}

            double tmp;
            bool[] detectedIntervals;
            int dividingFactor, nrOfParts;
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
            detectedIntervals = new bool[nrOfParts];
            for (int i = 0; i < nrOfParts; i++)
            {
                //int[] partOfRrIntervals = new int[dividingFactor];
                //Array.Copy(rrIntervals, i * dividingFactor, partOfRrIntervals, 0, dividingFactor);
                _rrIntervals.CopySubVectorTo(partOfRrIntervals, i * dividingFactor, 0, dividingFactor);
                if (_method.Method == Detect_Method.STATISTIC)
                {
                    detectedIntervals[i] = detectAFStat(partOfRrIntervals, fs);
                }
                else
                {
                    //detectedIntervals[i] = detectAFPoin(partOfRrIntervals, fs);
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
            //double[] pointsDetected;
            string afDetectedS; //nieuzywane
            string afDetectionDescription="";
            if (afDetected)
            {
                int lastIndex = 0;
                //pointsDetected = new double [lengthOfDetectedIntervals];
                for (int i = 0; i < detectedIntervals.Length; i++)
                {
                    if (detectedIntervals[i])
                    {
                        int j;
                        for (j = 0; j <  _rrIntervals.At(i); j++)
                        {
                            pointsDetected[j + lastIndex] = _rrIntervals.At(i) + j;
                        }
                        lastIndex = j;
                    }
                }
                afDetectedS = "Wykryto migotanie przedsionków.";
                double lengthOfSignal = (_rrIntervals.At(_rrIntervals.Count) - _rrIntervals.At(0)) / fs;
                double lengthOfDetection = lengthOfDetectedIntervals / fs;
                double percentOfDetection = (lengthOfDetection / lengthOfSignal) * 100;
                afDetectionDescription += "Wykryto migotanie trwające ";
                afDetectionDescription += lengthOfDetection.ToString("F1", CultureInfo.InvariantCulture);
                afDetectionDescription += "s. Stanowi to ";
                afDetectionDescription += percentOfDetection.ToString("F1", CultureInfo.InvariantCulture);
                afDetectionDescription += "% trwania sygnału.";
            }
            else
            {
                pointsDetected.Clear();
                afDetectedS="Nie wykryto migotania przedsionków";
            }

        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion

         bool detectAFStat(Vector<double> _RR, uint fs )
        {
            bool AF, tprD, seD, rmssdD;

            //Turning Punct Ratio
            int turningPoints = 0;
            for (int i = 1; i < (_RR.Count - 1); i++)
            {
                if( ((_RR.At(i - 1) < _RR.At(i)) && (_RR.At(i + 1) < _RR.At(i)))|| ((_RR.At(i - 1) > _RR.At(i)) && (_RR.At(i + 1) > _RR.At(i))))
                {
                    turningPoints++;
                }
            }
            double tpr = turningPoints / ((2 * 30- 4) / 3);
            if (tpr < 0.77 && tpr > 0.54)
            {
                tprD = false;
            }
            else
            {
                tprD = true;
            }

            //Shannon Entrophy
            int[] histogram = new int[8];
            double maxRr = _RR.Maximum();
            double minRr = _RR.Minimum();
            double width = (maxRr - minRr) / 8;
            double tmp = minRr;
            List<double> listOfElements = new List<double>();
            for (int i=0; i < 8; i++)
            {
                if (i < 7)
                {
                    for (int k = 0; k < _RR.Count; k++)
                    {
                        double elements = _RR.Find(element => (element >= tmp && element < (tmp + width))).Item2;
                        listOfElements.Add(elements);
                    }
                    histogram[i] = listOfElements.Count / (32 - 8);
                    listOfElements.Clear();
                }
                else
                {
                    histogram[i] = listOfElements.Count / (32 - 8);
                    listOfElements.Clear();
                }
                tmp += width;
            }
            double se = 0;
            foreach (int a in histogram)
            {
                se -= a * Math.Log10(a) / Math.Log10(0.125);
            }

            if (se > 0.7)
            {
                seD = true;
            }
            else
            {
                seD = false;
            }

            //RMSSD
            double rmssd=0;
            for (int i=0; i < (_RR.Count - 1); i++)
            {
                rmssd += Math.Pow((_RR.At(i + 1) - _RR.At(i)), 2);
            }
            rmssd /= (32 - 1);
            rmssd = Math.Sqrt(rmssd);
            rmssd /= _RR.Average();
            if (rmssd > 0.1)
            {
                rmssdD = true;
            }
            else
            {
                rmssdD = false;
            }
            if (tprD && seD && rmssdD)
            {
                AF = true;
            }
            else
            {
                AF = false;
            }
            return AF;  
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion
           
        bool detectAFPoin(int[] RR, double fs)
        {
            bool AF = false;
            int length = RR.Length;
            int[] Ii;
            int[] Ii1;
            if (length % 2 == 0)
            {
                int tmp = length / 2;
                Ii = new int[tmp];
                Ii1 = new int[tmp];
                int j = 0;

                for (int i = 0; i < tmp; i = i + 2)
                {
                    Ii[j] = RR[i];
                    Ii1[j] = RR[i+1];
                    j++;
                }
            }
            else
            {
                double tmp = length / 2;
                int size = Convert.ToInt32(Math.Ceiling(tmp));
                Ii = new int[size];
                Ii1 = new int[size];
                int j = 0;

                for (int i = 0; i < size; i = i + 2)
                {
                    Ii[j] = RR[i];
                    j++;
                }

                for (int i = 1; i < (size-1); i = i + 2)
                {
                    Ii1[j] = RR[i];
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

            int sum = 0;
            for(int i = 0; i < Ii.Length; i++)
            {
                sum += Ii[i];
            }

            G = (-RR[1] - RR[RR.Length - 1] + 2 * sum) / (2 * length - 2);
            d = F / G;

            if (d <= 0.06)
                AF = false;
            else
            {
                //..............................

            }
            return AF;
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion

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
            {
                //A = 0;
                //B = 0;
                //Cluster = 0;
            }
        }

        List<DataPoint> _rawDataToCluster = new List<DataPoint>();
        List<DataPoint> _normalizedDataToCluster = new List<DataPoint>();
        List<DataPoint> _clusters = new List<DataPoint>();
        private int _numberOfClusters = 0;

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion

        private void InitilizeRawData(double[] Vector1, double[] Vector2)
        {
            for (int i = 0; i < Vector1.Length; i++)
            {
                DataPoint dp = new DataPoint();
                dp.A = Vector1[i];
                dp.B = Vector2[i];
                _rawDataToCluster.Add(dp);
            }
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion

        private void NormalizeData()
        {
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
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion

        private void InitializeCentroids()
        {
            int _amount = 3;

            for (int i = 0; i < _amount; ++i)
            {
                _normalizedDataToCluster[i].Cluster = _rawDataToCluster[i].Cluster = i;
            }
            Random random = new Random();
            for (int i = _amount; i < _normalizedDataToCluster.Count; i++)
            {
                _normalizedDataToCluster[i].Cluster = _rawDataToCluster[i].Cluster = random.Next(0, _amount);
            }
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion

        private bool UpdateDataPointMeans()
        {
            if (EmptyCluster(_normalizedDataToCluster)) return false;

            var groupToComputeMeans = _normalizedDataToCluster.GroupBy(p => p.Cluster).OrderBy(p => p.Key);
            int clusterIndex = 0;
            double a = 0.0;
            double b = 0.0;
            foreach (var item in groupToComputeMeans)
            {
                foreach (var value in item)
                {
                    a += value.A;
                    b += value.B;
                }
                _clusters[clusterIndex].A = a / item.Count();
                _clusters[clusterIndex].B = b / item.Count();
                clusterIndex++;
                a = 0.0;
                b = 0.0;
            }
            return true;
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion

        private bool EmptyCluster(List<DataPoint> data)
        {
            var emptyCluster =
            data.GroupBy(s => s.Cluster).OrderBy(s => s.Key).Select(g => new { Cluster = g.Key, Count = g.Count() });

            foreach (var item in emptyCluster)
            {
                if (item.Count == 0)
                {
                    return true;
                }
            }
            return false;
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
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
        #endregion

        private bool UpdateClusterMembership()
        {
            bool changed = false;

            double[] distances = new double[_numberOfClusters];

            for (int i = 0; i < _normalizedDataToCluster.Count; ++i)
            {

                for (int k = 0; k < _numberOfClusters; ++k)
                    distances[k] = ElucidanDistance(_normalizedDataToCluster[i], _clusters[k]);

                int newClusterId = MinIndex(distances);
                if (newClusterId != _normalizedDataToCluster[i].Cluster)
                {
                    changed = true;
                    _normalizedDataToCluster[i].Cluster = _rawDataToCluster[i].Cluster = newClusterId;
                }
            }
            if (changed == false) return false;
            if (EmptyCluster(_normalizedDataToCluster)) return false;
            // w innym przypadku niż powyższe zwraca true
            return true;
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
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
        #endregion

        public void Cluster(List<DataPoint> data)
        {
            bool _changed = true;
            bool _success = true;
            InitializeCentroids();

            int maxIteration = data.Count * 10;
            int _threshold = 0;
            while (_success == true && _changed == true && _threshold < maxIteration)
            {
                ++_threshold;
                _success = UpdateDataPointMeans();
                _changed = UpdateClusterMembership();
            }
        }

        #region Documentation
        /// <summary>
        /// TODO
        /// </summary>
        #endregion

        private void ClusteringPoints(object sender, EventArgs e, double[] Vector1, double[] Vector2)
        {
            InitilizeRawData(Vector1, Vector2);
            NormalizeData();
            Cluster(_normalizedDataToCluster);
            var group = _rawDataToCluster.GroupBy(s => s.Cluster).OrderBy(s => s.Key);
        }

        //bool detectAFStat(int[] RR, double fs)
        //{
        //    bool AF;
        //    bool tprD;
        //    bool seD;
        //    bool rmssdD;

        //    //Turning Punct Ratio
        //    int turningPoints = 0;
        //    for (int i = 1; i < (RR.Length - 1); i++)
        //    {
        //        if (((RR[i - 1] < RR[i]) && (RR[i + 1] < RR[i])) || ((RR[i - 1] > RR[i]) && (RR[i + 1] > RR[i])))
        //        {
        //            turningPoints++;
        //        }
        //    }
        //    double tpr = turningPoints / ((2 * 30 - 4) / 3);
        //    if (tpr < 0.77 && tpr > 0.54)
        //    {
        //        tprD = false;
        //    }
        //    else
        //    {
        //        tprD = true;
        //    }

        //    //Shannon Entrophy
        //    int[] histogram = new int[8];
        //    int maxRr = RR.Max();
        //    int minRr = RR.Min();
        //    double width = (maxRr - minRr) / 8;
        //    double tmp = minRr;
        //    for (int i = 0; i < 8; i++)
        //    {
        //        if (i < 7)
        //        {
        //            int[] elements = Array.FindAll(RR, element => (element >= tmp && element < (tmp + width)));
        //            histogram[i] = elements.Length / (32 - 8);
        //        }
        //        else
        //        {
        //            int[] elements = Array.FindAll(RR, element => (element >= tmp && element <= (tmp + width)));
        //            histogram[i] = elements.Length / (32 - 8);
        //        }
        //        tmp += width;
        //    }
        //    double se = 0;
        //    foreach (int a in histogram)
        //    {
        //        se -= a * Math.Log10(a) / Math.Log10(0.125);
        //    }

        //    if (se > 0.7)
        //    {
        //        seD = true;
        //    }
        //    else
        //    {
        //        seD = false;
        //    }

        //    //RMSSD
        //    double rmssd = 0;
        //    for (int i = 0; i < (RR.Length - 1); i++)
        //    {
        //        rmssd += Math.Pow((RR[i + 1] - RR[i]), 2);
        //    }
        //    rmssd /= (32 - 1);
        //    rmssd = Math.Sqrt(rmssd);
        //    rmssd /= RR.Average();
        //    if (rmssd > 0.1)
        //    {
        //        rmssdD = true;
        //    }
        //    else
        //    {
        //        rmssdD = false;
        //    }
        //    if (tprD && seD && rmssdD)
        //    {
        //        AF = true;
        //    }
        //    else
        //    {
        //        AF = false;
        //    }
        //    return AF;

        //}

    }
}
