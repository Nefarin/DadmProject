using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.Modules.ECG_Baseline;
using EKG_Project.Modules.Waves;
using EKG_Project.Modules.R_Peaks;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.IO;

namespace EKG_Project.Modules.ST_Segment
{
    public class ST_Segment_Alg
    {
        private int[] _st_shapes;
        private Vector<int> _tJ;
        private Vector<int> _tST;

        public int[] St_shapes
        {
            get
            {return _st_shapes;}
            set
            {_st_shapes = value;}
        }

        public Vector<int> TJ
        {
            get
            {
                return _tJ;
            }

            set
            {
                _tJ = value;
            }
        }

        public Vector<int> TST
        {
            get
            {
                return _tST;
            }

            set
            {
                _tST = value;
            }
        }

        public ST_Segment_Alg(Vector<double> signalECGBaseline, List<int> tQRS_onset, List<int> tQRS_ends, Vector<double> rInterval, int frequency)
        {
            Vector<int> ttQRS_onset = Vector<int>.Build.DenseOfEnumerable(tQRS_onset);
            Vector<int> ttQRS_ends = Vector<int>.Build.DenseOfEnumerable(tQRS_ends);

            Tuple<int[], Vector<int>, Vector<int>> st_epizodes = ST_Analysis(ttQRS_ends, ttQRS_onset, signalECGBaseline, rInterval, frequency);
            St_shapes = st_epizodes.Item1;
            TJ = st_epizodes.Item2;
            TST = st_epizodes.Item3;
        }

        public Tuple<int[], Vector<int>, Vector<int>> ST_Analysis(Vector<int> tQRS_ends, Vector<int> tQRS_onset, Vector<double> signalECGBaseline, Vector<double> rInterval, int frequency)
        {

            Vector<int> tJ = Vector<int>.Build.Dense(tQRS_ends.Count);
            Vector<int> tST = Vector<int>.Build.Dense(tQRS_ends.Count);
            Vector<int> tJX = Vector<int>.Build.Dense(tQRS_ends.Count);
            int shapeType = 0;

            for (int i = 0; i < tQRS_ends.Count; ++i)
            {
                // if (tQRS_ends[i] < 0 || tQRS_onset[i] < 0) continue;
                tJ[i] = tQRS_ends[i] * 1 / 1000 * frequency + 20;
                tST[i] = tQRS_ends[i] * 1 / 1000 * frequency + 35;
                //result.tJs.Add(tJ);
                //result.tSTs.Add(tST);
                int tADD;
                //Częstość skurczów serca
                int HR = (int)(60000 / rInterval[i]);
                // Z literatury
                if (HR < 100)
                {
                    tADD = 80;
                }
                else if (HR < 110)
                {
                    tADD = 72;
                }
                else if (HR < 120)
                {
                    tADD = 64;
                }
                else
                {
                    tADD = 60;
                }
                // nic
                tJX[i] = tQRS_onset[i] * 1 / 1000 * frequency + tADD;
                int offset = (int)(signalECGBaseline[tJX[i]] - signalECGBaseline[tQRS_onset[i]]); //
                int tTE = tST[i];
                // wyznaczanie epizody
                double a = (signalECGBaseline[tTE] - signalECGBaseline[tJ[i]]) / (tTE - tJ[i]);
                double b = (signalECGBaseline[tJ[i]] * tTE - signalECGBaseline[tTE] * tJ[i]) / (tTE - tJ[i]);
                shapeType = ShapesDetector(a, b, tJ[i], tTE, signalECGBaseline);
            }
            // zliczanie

            int concaveCount = 0;
            int convexCount = 0;
            int increasingCount = 0;
            int horizontalCount = 0;
            int decreasingCount = 0;
            for (int i = 0; i < signalECGBaseline.Count(); ++i)
            {
                switch (shapeType)
                {
                    case 1:
                        ++concaveCount;
                        break;

                    case 2:
                        ++convexCount;
                        break;

                    case 3:
                        ++increasingCount;
                        break;

                    case 4:
                        ++horizontalCount;
                        break;

                    case 5:
                        ++decreasingCount;
                        break;
                }
            }
            int[] shapes = new int[5];
            shapes[0] = concaveCount;
            shapes[1] = convexCount;
            shapes[2] = increasingCount;
            shapes[3] = horizontalCount;
            shapes[4] = decreasingCount;

            Tuple <int[], Vector<int>, Vector<int>> result = new Tuple<int[], Vector<int>, Vector <int>>(shapes, tJ, tST);
  
            return result;
        }

        public int ShapesDetector(double a, double b, int tJ, int tTE, Vector<double> signal)
        {
            int shape = 0;
            for (int point = tJ; point < tTE; point++)
            {
                double distance = (Math.Abs(a * point + signal[point] + b)) / (Math.Sqrt(1 + a * a));

                if (distance > 0.15)
                {
                    int pointsBeneath = 0;
                    int pointsAbove = 0;
                    int allPoints = 0;

                    for (int i = tJ; i < tTE; i++)
                    {
                        ++allPoints;
                        if (signal[i] > a * i + b)
                        {
                            ++pointsAbove;
                        }
                        else
                        {
                            ++pointsBeneath;
                        }
                    }
                    double convex = pointsAbove / allPoints; //2
                    double concave = pointsBeneath / allPoints; //1
                    if (convex > 0.7)
                    {
                        shape = 2;
                    }
                    if (concave > 0.7)
                    {
                        shape = 1;
                    }
                }

                else
                {
                    if (a > 0.15)
                    {
                        shape = 3;
                    }
                    else if (a < -0.15)
                    {
                        shape = 5;
                    }
                    else
                    {
                        shape = 4;
                    }
                }
            }
            int output = shape;
            return output;
        }
    }
}

