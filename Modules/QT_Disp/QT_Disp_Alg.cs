using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Differentiation;
using EKG_Project.IO;
            

namespace EKG_Project.Modules.QT_Disp
{
    public partial class QT_Disp : IModule
    {
        static void Main()
        {

            TempInput.setInputFilePath("sdas");


            Console.WriteLine("Test");
            UInt32 number = uint.MaxValue;
            Console.WriteLine(number);
           
            double[] linia = new double[10];
            double[] wynik = new double[9];
          



            NumericalDerivative diff = new NumericalDerivative();

            for (int i = 0; i < 10; i++)
            {
                linia[i] = (double)(i*i );
            }
            wynik = pochodna(linia, 1, 10);
            Vector<double> aa = Vector<double>.Build.DenseOfArray(linia);
            foreach (double x in aa)
            {
                Console.WriteLine(x);
            }
           Console.WriteLine(wynik.Min());          
            //foreach (double x in aa)
            //{
            //    Console.WriteLine(x);

            //}


            // input argument`
            UInt16 Fs;

            List<float> ECG_Baseline = new List<float>();
            List<float> RR_Interval = new List<float>();
            List<UInt32> P_OnsetIndex = new List<UInt32>();
            List<UInt32> T_EndIndex = new List<UInt32>();
            List<UInt32> QRS_OnsetIndex = new List<UInt32>();

            float QT;




        }
        public static double[] pochodna(double[] tab, double stepSize, int length)
        {
            double[] wynik = new double[length - 1];
            for (int i = 0; i < length - 1; i++)
            {
                wynik[i] = tab[i + 1] - tab[i];
            }
            return wynik;
        }
    }
   
}
