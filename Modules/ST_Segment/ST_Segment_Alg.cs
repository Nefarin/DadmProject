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
        
        public static void Main(string[] args)
        {
            //read data from file
            TempInput.setInputFilePath(@"C:\sciezka");
            uint fs = TempInput.getFrequency();
            Vector<double> sig = TempInput.getSignal();

            // zrobić metodę main z tej poniżej, wczytać




        //write result to dat file
        TempInput.setOutputFilePath(@"C:\Users\Paulina\Desktop\inervals\result.txt");
            TempInput.writeFile(fs, sig);
    }
        
        public ST_Segment_Data Method (Vector<double> signal, Vector<uint> tQRS_onset, Vector<uint> tQRS_ends, Vector<double> rInterval, int freq)
        {
            ST_Segment_Data result = new ST_Segment_Data();
            int[] finalShapes = new int[signal.Count()];
            long tJ;
            long tST;
            //int ConcaveCurves;

            for (int i = 0; i < signal.Count(); ++i)
            {
               // if (tQRS_ends[i] < 0 || tQRS_onset[i] < 0) continue;
                tJ = tQRS_ends[i] * 1 / 1000 * freq + 20;
                tST = tQRS_ends[i] * 1 / 1000 * freq + 35;
                result.tJs.Add(tJ);
                result.tSTs.Add(tST);
                int tADD = 0;
                int HR = (int)(60000 / rInterval[i]);
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
                long tJX = tQRS_onset[i] * 1 / 1000 * freq + tADD;
                int offset = (int)(signal[(int)tJX] - signal[(int)tQRS_onset[i]]); //
                double tTE = tST;
                double a = (signal[(int)tTE] - signal[(int)tJ]) / (tTE - tJ);
                double b = (signal[(int)tJ] * tTE - signal[(int)tTE] * tJ) / (tTE - tJ);
                int shape = 0;
                for (double time = tJ; time < tTE; time += 1.0)
                {
                    double distance = (Math.Abs(a * time + signal[i] + b)) / (Math.Sqrt(1 + a * a));
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
                    int allPoints = 0;
                    for (double time = tJ; time < tTE; time += 1.0)
                    {
                        ++allPoints;
                        if (signal[i] > a * time + b)
                        {
                            ++pointsAbove;
                        }
                        else
                        {
                            ++pointsBeneath;
                        }
                    }
                    if ((double)pointsAbove / (double)allPoints > 0.7)
                    {
                        finalShapes[i] = 2;
                    }
                    else if ((double)pointsBeneath / (double)allPoints > 0.7)
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

       /* public void ProcessData()
        {
            throw new NotImplementedException();
        }*/
    }

}
