using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.Modules.ECG_Baseline;
using EKG_Project.Modules.Waves;
using EKG_Project.Modules.R_Peaks;
using MathNet;
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
        public int Metoda(int tST, int tJ,) // tu potrzebuje metode która bedzie wyświetlać lub poprostu zapamiętywać  to co wyjdzie?//

        {
            int signal_size = rRawSample.Count();
            for (int i = 0; i < signal_size; i++)
            {
                int tJ = list < int > tachos_r[i] + 45;
                int tST = tachos_r[i] + 60;

                if (HR < 100)  //puls [uderzeń na minute]
                {
                    int tADD = 80; // ms 
                }
                else if (HR < 110)
                {
                    int tADD = 72;
                }
                else if (HR < 120)
                {
                    int tADD = 64;
                }
                else
                {
                    int tADD = 60;
                }
                int tJX = tQRS_onset[i] + tADD;

                //odległość od izolinii
                const double K1 = -0.1;
                const double K2 = 0.1;

                int offset = war_tJX[i] - war_tQRS_onset[i];

                if (offset < K1)
                {
                    int odl = -1; //obniżony
                }
                else if (offset > K2)
                {
                    int odl = 1; // podwyższony
                }
                else
                {
                    int odl = 0; // normalny
                }

                // kształt : krzywa: wypukła/ wklesła, prosta rosnaca/malejaca/pozioma

                double tTE = tST;
                double a = (war_tTE[i] - war_tJ[i]) / (tTE - tJ);
                double b = (war_tJ[i] * tTE - war_tTE[i] * tJ) / (tTE - tJ);
                const double limit_prostej = 0.15;
                int ksztalt_koncowy = 0;
                for (double czas = tJ; double czas = tTE, ksztalt++)
                {
                    decimal[] decimals = { Decimal.MaxValue, 12.45M, 0M, -19.69M,  ///nie wiem czy to tak
                             Decimal.MinValue };
                    foreach (decimal value in decimals) ;

                    double distance = (Abs(a * czas + war_czas + b)) / Sqr(1 + a ^ 2);

                    if (double distance > limit_prostej)
            {
                int ksztalt = int ksztalt + 1;
            }
        }
        if (int ksztalt = 0)
            {
            if (double a> 0.15)
                {
            int ksztalt_koncowy = 3;
    }
             else if (double a< - 0.15);
                {
                int ksztalt_koncowy = 5;
    }
            else 
                {
               int ksztalt_koncowy = 4;
} 
                }
           else
            {   
            krzywa
            }

            int punkty_pod = 0;
int punkty_nad = 0;
        for ( double czas = tJ; double czas = tTE;)
            {
            if( war_czas > (int a* double czas + int b)
                {
                int punkty_nad ++;
                }
            else
                {
                int punkty_pod ++;   
                }
            }
          if (punkty_nad/wszystkie_punkty > 0.5)
            {
            int ksztalt_koncowy = 2;  // wypulkła
            }
            else if (punkty_pod / wszystkie punkty < 0.5)
                {
                int ksztalt_koncowy = 1; //wklesla
}
                

            } // nawias do dużego fora
 int ksztalt[i] = int ksztalt_koncowy;
   
    int licznik_krzywa_wklesla = 0;
int licznik_krzywa_wypukla = 0;
int licznik_prosta_rosnaca = 0;
int licznik_prosta_pozioma = 0;
int licznik_prosta_malejaca = 0;

for (int i = 0; i<signal_size; )
    {
    if (int ksztalt[i]==1)
{
int licznik_krzywa_wklesla ++; 
}
else if (int ksztalt[i] == 2)
{
int licznik_krzywa_wypukla ++;
}
else if (int ksztalt[i] == 3)
{
int licznik_prosta_rosnaca ++;
}
else if (int ksztalt[i] == 4)
{
int licznik_prosta_pozioma ++;
}
else if (int ksztalt[i] == 5)
{
int licznik_prosta_malejaca ++;
}

    }



        }
    }
}
