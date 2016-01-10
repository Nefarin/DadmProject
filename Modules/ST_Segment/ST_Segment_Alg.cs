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
    public partial class ST_Segment : IModule
    {
        public static void Main(string[] args)
        {
            // wczytywanie pliku
            TempInput.setInputFilePath(@"C:\Users\Ania\Desktop\DADM_Projekt\R_100.txt");
            uint fs = TempInput.getFrequency();
            Vector<double> sig = TempInput.getSignal();

            ST_Segment st = new ST_Segment();

            // zamiana próbek na czas
            Vector<double> tacho_rr = st.TimeConvert(fs, sig.ToArray());


            //....
            Console.WriteLine(fs);
            Console.ReadKey();
        }
        // Metody
        // zamiana próbek na czas
        public Vector<double> TimeConvert(uint samplFreq, double[] rRawSample)
        {
            int signal_size = rRawSample.Count();
            Vector<double> tachos_r = Vector<double>.Build.Dense(signal_size);

            for (int i = 0; i < signal_size; i++)
            {
                tachos_r[i] = rRawSamples[i] * 1000 / samplFreq;  //[ms]
            }
            return tachos_r;
        }
        //dalej

        public StAnalysisResult Method(Vector<double> signal, Vector<uint> tQRS_onset, Vector<uint> tQRS_ends)
        {
            StAnalysisResult result = new StAnalysisResult();
            int[] finalShapes = new int[signal.Count()];
            for (int i = 0; i < signal.Count(); ++i)
            {
                double tJ = tQRS_ends[i] + 20;
                double tST = tQRS_ends[i] + 35;
                result.tJs.Add(tJ);
                result.tSTs.Add(tST);
                int tADD = 0;
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
                long tJX = tQRS_onset[i] + tADD;
                int offset = war_tJX[i] - war_tQRS_onset[i];
                double tTE = tST;
                double a = (war_tTE[i] - war_tJ[i]) / (tTE - tJ);
                double b = (war_tJ[i] * tTE - war_tTE[i] * tJ) / (tTE - tJ);
                int shape = 0;
                for (double time = tJ; time < tTE; time += 1.0)
                {
                    double distance = (Math.Abs(a * time + war_time + b)) / (Math.Sqrt(1 + a * a));
                    if (distance > 0.15)
                    {
                        ++shape;
                    }
                }
                if (shape == 0)
                {
                    if (a > 0.15)
                    {
                        finalShapes[i] = 3;
                    }
                    else if (a < -0.15)
                    {
                        finalShapes[i] = 5;
                    }
                    else
                    {
                        finalShapes[i] = 4;
                    }
                }
                else
                {
                    int pointsBeneath = 0;
                    int pointsAbove = 0;
                    for (double time = tJ; time < tTE; time += 1.0)
                    {
                        if (war_time > a * time + b)
                        {
                            ++pointsAbove;
                        }
                        else
                        {
                            ++pointsBeneath;
                        }
                    }
                    if (pointsAbove / allPoints > 0.7)
                    {
                        finalShapes[i] = 2;
                    }
                    else if (pointsBeneath / allPoints > 0.7)
                    {
                        finalShapes[i] = 1;
                    }
                }
            }
            for (int i = 0; i < signal.Count(); ++i)
            {
                switch (finalShapes[i])
                {
                    case 1:
                        ++result.ConcaveCurves;
                        break;

                    case 2:
                        ++result.ConvexCurves;
                        break;

                    case 3:
                        ++result.IncreasingLines;
                        break;

                    case 4:
                        ++result.HorizontalLines;
                        break;

                    case 5:
                        ++result.DecreasingLines;
                        break;
                }
            }
            return result;
        }



    }
}
}

