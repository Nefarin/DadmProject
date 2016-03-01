
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.Modules.ECG_Baseline;
using EKG_Project.Modules.Waves;
using EKG_Project.Modules.R_Peaks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using EKG_Project.IO;


namespace EKG_Project.Modules.ST_Segment
{
    public class ST_Segment_Alg
    {
        static void Main(string[] args)
        {
            //read data from dat file
            TempInput.setInputFilePath(@"pliki_wejsciowe_wyjsciowe/sig.txt");
            uint fs = TempInput.getFrequency();
            Vector<double> signal = TempInput.getSignal();
            TempInput.setInputFilePath(@"pliki_wejsciowe_wyjsciowe/rr.txt");
            Vector<double> rr = TempInput.getSignal();
            TempInput.setInputFilePath(@"pliki_wejsciowe_wyjsciowe/QRS_Onset.txt");
            Vector<double> QRSOnset = TempInput.getSignal();
            TempInput.setInputFilePath(@"pliki_wejsciowe_wyjsciowe/QRS_End.txt");
            Vector<double> QRSEnd = TempInput.getSignal();
            TempInput.setInputFilePath(@"pliki_wejsciowe_wyjsciowe/db43.txt");
            Vector<double> db43 = TempInput.getSignal();
            TempInput.setInputFilePath(@"pliki_wejsciowe_wyjsciowe/db44.txt");
            Vector<double> db44 = TempInput.getSignal();

            ST_Segment_Alg alg = new ST_Segment_Alg();
            Vector<double> STOnset = alg.STOnsetDetect(QRSEnd, signal, rr, db43);
            Vector<double> STEnd = alg.STEndDetect(QRSEnd, signal, rr, db44);
            Tuple<Vector<double>, Vector<double>> stResults = alg.STAnalysis(fs, QRSEnd, QRSOnset, signal, STOnset, STEnd, rr);
            Vector<double> Offset = stResults.Item1;
            Vector<double> STShapes = stResults.Item2;
            double[] ShapesNum = alg.ShapesCounter(STShapes);
            Vector<double> shapess = Vector<double>.Build.DenseOfArray(ShapesNum);

            Tuple<Vector<double>, Vector<double>, Vector<double>> results = new Tuple<Vector<double>, Vector<double>, Vector<double>>(STOnset, STEnd, shapess);

            TempInput.setOutputFilePath(@"pliki_wejsciowe_wyjsciowe/result.txt");
            TempInput.writeFile(results);
        }

        public Vector<double> STOnsetDetect(Vector<double> QRSEnd, Vector<double> sig, Vector<double> rr, Vector<double> db43)
        {
            int q = QRSEnd.Count;
            int range = sig.Count / q;
            int rangeSc = (range * db43.Count) / sig.Count;
            Vector<double> y = db43.SubVector(0, rangeSc-1);
            int maxind = y.MaximumIndex();
            int st0 = maxind * sig.Count / db43.Count;

            Vector<double> r = RAdder(rr);
          
            Vector<double> STOnset = Vector<double>.Build.Dense(rr.Count);
            STOnset = r + (double)st0;

            return STOnset;
        }
        public Vector<double> STEndDetect(Vector<double> QRSEnd, Vector<double> sig, Vector<double> rr, Vector<double> db44)
        {
            int q = QRSEnd.Count;
            int range = sig.Count / q;
            int rangeSc = (range * db44.Count) / sig.Count;
            Vector<double> y = db44.SubVector(0, rangeSc - 1);
            int maxind = y.MaximumIndex();
            int st0 = maxind * sig.Count / db44.Count;

            Vector<double> r = RAdder(rr);
            
            Vector<double> STEnd = Vector<double>.Build.Dense(rr.Count);
            STEnd = r + (double)st0;

            return STEnd;
        }
        public Vector<double> RAdder(Vector<double> rr )
        {
            Vector<double> r = Vector<double>.Build.Dense(rr.Count);
            for (int i = 0; i < rr.Count-1; i++)
            {
                r[i] = rr[i + 1] + rr[i];
            }
            return r;
        }
        public int HeartRateX(uint fs, Vector<double> rr)
        {
            double rravg = rr.Average();
            double hr = (60.0 * (double)fs) / rravg;
            int x;

            if(hr < 100)
            {
                x = 80;
            }
            else if(hr >100 && hr < 110)
            {
                x = 72;
            }
            else if(hr >110 && hr < 120)
            {
                x = 64;
            }
            else { x = 60; }

            return x;
        }

        public Tuple<Vector<double>, Vector<double>> STAnalysis(uint fs, Vector<double> QRSEnd, Vector<double> QRSOnset, Vector<double> sig, Vector<double> STOnset, Vector<double> STEnd, Vector<double> rr)
        {
            int b = QRSEnd.Count - 1;
            Vector<double> Offset = Vector<double>.Build.Dense(QRSEnd.Count);
            Vector<double> STShapes = Vector<double>.Build.Dense(b);

            for(int i = 0; i < QRSEnd.Count-2; i++)
            {
                int stOnset = (int)STOnset[i];
                int stEnd = (int)STEnd[i];
                double[] tt = Generate.LinearRange(0, 1, sig.Count);
                Vector<double> t = Vector<double>.Build.DenseOfArray(tt);
                Vector<double> n = t.SubVector(stOnset, Math.Abs(stEnd - stOnset));

                // KST = slope*n + K
                double slope = (sig[stEnd]-sig[stOnset]) / (stEnd-stOnset);
                double k = (sig[stOnset]*stEnd - sig[stEnd]*stOnset) / (stEnd - stOnset);
                Vector<double> kst = slope * n + k; //możliwy bug

                // Distance
                double Dis = Math.Abs(slope * n.Count + 1 * kst[kst.Count-1] + k) / Math.Sqrt(1 + slope * slope);

                int x = HeartRateX(fs, rr);
                int jx = stOnset + x;

                // Offset
                Offset[i] = sig[jx] - sig[(int)QRSOnset[i]];

                // Determine ST type
                Vector<double> yy = sig.SubVector(stOnset,Math.Abs(stEnd - stOnset));
                double mean = kst.Average();
                STShapes[i] = STtypeDetector(Dis, n, yy, mean);

            }

            Tuple<Vector<double>, Vector<double>> result = new Tuple<Vector<double>, Vector<double>>(Offset, STShapes);
            return result;
        }

        public double STtypeDetector(double dis, Vector<double> n, Vector<double> yy, double mean)
        {
            string type;
            double STtype;
            if(dis > 0.15)
            {
                type = "curve";
            }
            else
            {
                type = "straigth";
            }

            switch(type)
            {
                case "curve":
                    //Concave or convex Curve Type
                    double[] p = Fit.Polynomial(n.ToArray(), yy.ToArray(), 2);
                    double a = p[2];
                    double b = p[1];
                    double c = p[0];
                    // Parabole top coordinate yw
                    double yw = (-(b*b-4-a-c)) / (4*a);
                    if(yw < mean)
                    {
                        STtype = 1; // concave
                    }
                    else
                    {
                        STtype = 2; // convex
                    }

                    return STtype;
                case "straigth":
                    // Slope direction of straigh type
                    
                    Tuple<double, double> ps = Fit.Line(n.ToArray(), yy.ToArray());
                    if(ps.Item2 > 0.15)
                    {
                        STtype = 3; // upward
                    }
                    else if(ps.Item2 < -0.15)
                    {
                        STtype = 4; // downward
                    }
                    else
                    {
                        STtype = 5; // horizon
                    }

                    return STtype;
                default:
                    return STtype = 0;
            }
            
        }
        public double[] ShapesCounter(Vector<double> Shapes)
        {
            int conc = 0;
            int conx = 0;
            int upwd = 0;
            int downw = 0;
            int horz = 0;
            double[] results = new double[5];
            for(int i = 0; i<Shapes.Count; i++)
            {
                double aa = Shapes[i];
                switch ((int)aa)
                {
                    case 1:
                        conc += 1;
                        break;
                    case 2:
                        conx += 1;
                        break;
                    case 3:
                        upwd += 1;
                        break;
                    case 4:
                        downw += 1;
                        break;
                    default:
                        horz += 1;
                        break;
                }
            }
            results[0] = conc;
            results[1] = conx;
            results[2] = upwd;
            results[3] = downw;
            results[4] = horz;

            return results;
        }

    }
}



