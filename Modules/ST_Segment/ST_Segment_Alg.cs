
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

        #region
        /// <summary>
        /// Metoda main służąca do wczytywania plików wejściowych oraz zapisywania rezultatów do pliku wyjściowego
        /// </summary>
        /// <param name="args"></param>
        #endregion
        static void Main(string[] args)
        {
            //read data from file
            TempInput.setInputFilePath(@"pliki_wejsciowe_wyjsciowe\sig.txt");
            Vector<double> sig = TempInput.getSignal();
            TempInput.setInputFilePath(@"pliki_wejsciowe_wyjsciowe\QRS_End.txt");
            Vector<double> QRS_End = TempInput.getSignal();
            TempInput.setInputFilePath(@"pliki_wejsciowe_wyjsciowe\QRS_Onset.txt");
            Vector<double> QRS_Onset = TempInput.getSignal();
            TempInput.setInputFilePath(@"pliki_wejsciowe_wyjsciowe\rr.txt");
            Vector<double> rr = TempInput.getSignal();

            uint fs = TempInput.getFrequency();

            ST_Segment_Alg Analise = new ST_Segment_Alg(sig, QRS_Onset, QRS_End, rr, 360);

            Tuple<Vector<double>, Vector<double>, Vector<double>> rezultat = Analise.ST_Analysis(QRS_End, QRS_Onset, sig, rr, 360);

            Console.WriteLine(sig.ToString());
            Console.WriteLine(QRS_End.ToString());
            Console.WriteLine(QRS_Onset.ToString());
            Console.WriteLine(rr.ToString());
            Console.Read();

            //write result to dat file              
            TempInput.setOutputFilePath(@"pliki_wejsciowe_wyjsciowe\result.txt");
            TempInput.writeFile(rezultat);

        }
        private Vector<double> _st_shapes;
        private Vector<double> _tJ;
        private Vector<double> _tST;

        public Vector<double> St_shapes
        {
            get
            { return _st_shapes; }
            set
            { _st_shapes = value; }
        }

        public Vector<double> TJ
        {
            get
            { return _tJ; }
            set
            { _tJ = value; }
        }

        public Vector<double> TST
        {
            get
            { return _tST; }
            set
            { _tST = value; }
        }

        #region
        /// <summary>
        /// Metoda służąca do otrzymania rezultatów modułu
        /// </summary>
        /// <param name="signalECGBaseline">sygnał z modułu ECGBaseline</param>
        /// <param name="tQRS_onset"> Wyznaczone punkty początkowe przez moduł Waves</param>
        /// <param name="tQRS_ends"> Wyznaczone punkty końcowe przez moduł Waves</param>
        /// <param name="rInterval">Wyznaczone punkty przez moduł R_Peaks</param>
        /// <param name="frequency">wartość częstotliwości</param>
        #endregion
        public ST_Segment_Alg(Vector<double> signalECGBaseline, Vector<double> tQRS_onset, Vector<double> tQRS_ends, Vector<double> rInterval, int frequency)
        {

            Tuple<Vector<double>, Vector<double>, Vector<double>> st_epizodes = ST_Analysis(tQRS_ends, tQRS_onset, signalECGBaseline, rInterval, frequency);
            St_shapes = st_epizodes.Item1;
            TJ = st_epizodes.Item2;
            TST = st_epizodes.Item3;
        }

        #region
        /// <summary>
        /// Wyznaczanie punktów granicznych odcinka ST, wyznaczenie i zliczenie epizodów
        /// </summary>
        /// <param name="tQRS_ends">Wyznaczone punkty końcowe przez moduł Waves</param>
        /// <param name="tQRS_onset"> Wyznaczone punkty początkowe przez moduł Waves</param>
        /// <param name="signalECGBaseline">Sygnał przygotowany przez moduł ECGBaseline</param>
        /// <param name="rInterval"></param>
        /// <param name="frequency">Częstotliwość</param>
        /// <returns>Tuple wektorów z shapesV, tJ (punkt początkowy odcinka ST), tST (punkt końcowy odcinka ST)</returns>
        #endregion
        public Tuple<Vector<double>, Vector<double>, Vector<double>> ST_Analysis(Vector<double> tQRS_ends, Vector<double> tQRS_onset, Vector<double> signalECGBaseline, Vector<double> rInterval, int frequency)
        {

            Vector<double> tJ = Vector<double>.Build.Dense(tQRS_ends.Count);
            Vector<double> tST = Vector<double>.Build.Dense(tQRS_ends.Count);
            Vector<double> tJX = Vector<double>.Build.Dense(tQRS_ends.Count);
            int shapeType = 0;

            for (int i = 0; i < tQRS_ends.Count; ++i)
            {
                if (tQRS_ends[i] < 0 || tQRS_onset[i] < 0) continue; //zabezpiecza przed nie wykrytymi 
                tJ[i] = tQRS_ends[i] + 20 / (0.001 * frequency);
                tST[i] = tQRS_ends[i] + 35 / (0.001 * frequency);

                int tADD;
                //Częstość skurczów serca
                int HR = (int)(60000 / rInterval[i]);

                if (HR < 100)
                {
                    tADD = (int)(80 / (0.001 * frequency));
                }
                else if (HR < 110)
                {
                    tADD = (int)(72 / (0.001 * frequency));
                }
                else if (HR < 120)
                {
                    tADD = (int)(64 / (0.001 * frequency));
                }
                else
                {
                    tADD = (int)(60 / (0.001 * frequency));
                }

                tJX[i] = (int)tQRS_onset[i] + tADD;
                int offset = (int)(signalECGBaseline[(int)tJX[i]] - signalECGBaseline[(int)tQRS_onset[i]]); //
                int tTE = (int)tST[i];
                // wyznaczanie epizody
                double a = (signalECGBaseline[tTE] - signalECGBaseline[(int)tJ[i]]) / (tTE - tJ[i]);
                double b = (signalECGBaseline[(int)tJ[i]] * tTE - signalECGBaseline[tTE] * tJ[i]) / (tTE - tJ[i]);
                shapeType = ShapesDetector(a, b, (int)tJ[i], tTE, signalECGBaseline);
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
            double[] shapes = new double[5];
            shapes[0] = concaveCount;
            shapes[1] = convexCount;
            shapes[2] = increasingCount;
            shapes[3] = horizontalCount;
            shapes[4] = decreasingCount;
            Vector<double> shapesV = Vector<double>.Build.DenseOfArray(shapes);

            Tuple<Vector<double>, Vector<double>, Vector<double>> result = new Tuple<Vector<double>, Vector<double>, Vector<double>>(shapesV, tJ, tST);

            return result;
        }

        #region
        /// <summary>
        /// Metoda pozwalająca na określenie epizodu( krzywa wklesła, krzywa wypukla, prosta rosnaca, prosta pozioma, prosta malejaca)
        /// </summary>
        /// <param name="a">Wspólczynnik prostej</param>
        /// <param name="b">Wspólczynnik prostej</param>
        /// <param name="tJ">Poczatek odcinka ST</param>
        /// <param name="tTE">Koniec odcinka ST</param>
        /// <param name="signal"></param>
        /// <returns>shape</returns>
        #endregion
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



