using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace EKG_Project.Modules.Atrial_Fibr
{
    //TODO: -ustalić czy zostawiamy 128 w statystycznym, czy robimy usuwanie wartości skrajnych
    //-cały algorym statystyczny
    //-cały alorytm poincare, k-średnie, wartość dopasowania
    //-testy
    //-komentarze
    public partial class Atrial_Fibr : IModule
    {
        //public class detectedAF
        //{
        //    private bool _detected;
        //    private int[] _detectedPoints;
        //    private string _detectedS;
        //    private string _timeofAF;

        //    public bool Detected
        //    {
        //        get
        //        {
        //            return _detected;
        //        }
        //        set
        //        {
        //            _detected = value;
        //        }
        //    }

        //    public int[] DetectedPoint
        //    {
        //        get
        //        {
        //            return _detectedPoints;
        //        }
        //        set
        //        {
        //            _detectedPoints = value;
        //        }
        //    }

        //    public string DetectedS
        //    {
        //        get
        //        {
        //            return _detectedS;
        //        }
        //        set
        //        {
        //            _detectedS = value;
        //        }
        //    }

        //    public string TimeofAF
        //    {
        //        get
        //        {
        //            return _timeofAF;
        //        }
        //        set
        //        {
        //            _timeofAF = value;
        //        }
        //    }

        //    public detectedAF(bool detected, int[] detectedPoints, string detectedS, string timeofAF)
        //    {
        //        this.Detected = detected;
        //        this.DetectedPoint = detectedPoints;
        //        this.DetectedS = detectedS;
        //        this.TimeofAF = timeofAF;
        //    }
        //    public detectedAF()
        //    {
        //        this.Detected = false;
        //        int[] tmp = { 0 };
        //        this.DetectedPoint =tmp;
        //        this.DetectedS = "Nie wykryto migotania przedsionków";
        //        this.TimeofAF = "";
        //    }

        //}
        private void detectAF (int[] RR, double fs)
        {
            int[] rrIntervals = new int[(RR.Length - 1)];
            for (int i=0; i < (RR.Length - 2); i++)
            {
                rrIntervals[i] = RR[i + 1] - RR[i];
            }
            int nrOfParts;
            double tmp;
            bool[] detectedIntervals;
            int dividingFactor;
            if (_method.Method== Detect_Method.STATISTIC)
            {
                dividingFactor = 128;
            }
            else
            {
                dividingFactor = 30;
            }
            tmp = rrIntervals.Length / dividingFactor;
            nrOfParts = Convert.ToInt32(Math.Floor(tmp));
            detectedIntervals = new bool[nrOfParts];
            for (int i = 0; i < nrOfParts; i++)
            {
                int[] partOfRrIntervals = new int[dividingFactor];
                Array.Copy(rrIntervals, i * dividingFactor, partOfRrIntervals, 0, dividingFactor);
                if (_method.Method == Detect_Method.STATISTIC)
                {
                    detectedIntervals[i] = detectAFStat(partOfRrIntervals, fs);
                }
                else
                {
                    detectedIntervals[i] = detectAFPoin(partOfRrIntervals, fs);
                }
            }
            int lengthOfDetectedIntervals = 0;
            bool afDetected = false;
            for (int i=0; i < detectedIntervals.Length; i++)
            {
                if (detectedIntervals[i])
                {
                    lengthOfDetectedIntervals += rrIntervals[i];
                    afDetected = true;
                }
            }
            int[] pointsDetected;
            string afDetectedS;
            string afDetectionDescription="";
            if (afDetected)
            {
                int lastIndex = 0;
                pointsDetected = new int[lengthOfDetectedIntervals];
                for (int i = 0; i < detectedIntervals.Length; i++)
                {
                    if (detectedIntervals[i])
                    {
                        int j;
                        for (j = 0; j <  rrIntervals[i]; j++)
                        {
                            pointsDetected[j + lastIndex] = RR[i] + j;
                        }
                        lastIndex = j;
                    }
                }
                afDetectedS = "Wykryto migotanie przedsionków.";
                double lengthOfSignal = (RR[RR.Length] - RR[0]) / fs;
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
                pointsDetected=new int [0];
                afDetectedS="Nie wykryto migotania przedsionków";
            }

        }
        bool detectAFStat(int[] RR, double fs )
        {
            bool AF = false;
            return AF;
            
        }
        bool detectAFPoin(int[] RR, double fs)
        {
            bool AF = false;
            return AF;
        }

    }
}
