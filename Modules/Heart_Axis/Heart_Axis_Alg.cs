using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;

namespace EKG_Project.Modules.Heart_Axis
{
    public partial class Heart_Axis : IModule
    {

        /*Pseudo Module*/

        private double[] PseudoModule(int Q, int S, double[] signal)
        {

            int j = 0;
            double[] pseudo_tab;
            pseudo_tab = new double[S - Q];
            for (int i = Q; i < S; i++,j++)
            {
                pseudo_tab[j] = Math.Sqrt(Math.Pow(signal[i], 2) + Math.Pow(signal[i+1], 2));
            }
            return pseudo_tab;
        }

        /* Finding Max */

        private int MaxOfPseudoModule(int Q, double[] pseudo_tab)
        {

            double maxValue = pseudo_tab.Max();
            int maxIndex = Array.IndexOf(pseudo_tab, maxValue);
            return maxIndex;
        }

        /*Least-Squares method*/
       
        private double[] LeastSquaresMethod(double []signal, int Q, double[] pseudo_tab, int frequency) // todo: skąd wziąć tablicę samples
        {
            int MaxIndex = MaxOfPseudoModule(Q, pseudo_tab);
            int timePeriod = 40; // czy 20?
            int milisecondsDivider = 1000;
            double T = (timePeriod * frequency) / milisecondsDivider; // ilość próbek na 1 ms
            int roundedT = (int)Math.Round(T, MidpointRounding.AwayFromZero); // zaokrąglenie - zmienić na floor?

            int signalSize = signal.Length;
            double[] indexes = null;
            int partArrayLength = 0;
            if ((MaxIndex - roundedT >= 0) && (MaxIndex + roundedT < signalSize)) //zabezpieczenie przed wyjściem poza zakres tabilcy
            {
                partArrayLength = 2 * roundedT + 1;
                indexes = new double[partArrayLength];
            }
            else
            {
                //wyjątek
                Aborted = true;
                _ended = true;
            }
            

            for (int i = 0; i < indexes.Length; i++) // inicjalizacja indexes numerami próbek
            {
                indexes[i] = MaxIndex - roundedT + i;
            }

            // inicjalizacja tablicy z częścią sygnału
            double[] partSignal = new double[partArrayLength];

            int startCopyIndex = (int)MaxIndex - roundedT;

            // przekopiuj z tablicy signal do tablicy partSignal n elementów (gdzie n = indexes.Length)
            Array.Copy(signal, startCopyIndex, partSignal, 0, indexes.Length);

            int order = 2;
            double[] bestFitCoefficients = Fit.Polynomial(indexes, partSignal, order);
            return bestFitCoefficients;
        }       


        /*Max of Polynomial*/
        private int MaxOfPolynomial(int Q, double [] fitting_parameters)
        {
            double dMaxOfPoly =(-fitting_parameters[1])/(2*fitting_parameters[0]);       // x = -b/2a
            int maxOfPoly = (int)Math.Round(dMaxOfPoly, MidpointRounding.AwayFromZero);
            return maxOfPoly;
        }
  

        /*Reading Amplitudes*/
        private double[] ReadingAmplitudes(double[] FirstLead, double[] SecondLead, int MaxOfPoly)
            {
            double[] amplitudes = new double[2];
            amplitudes[0] = FirstLead[MaxOfPoly];
            amplitudes[1] = SecondLead[MaxOfPoly];
            return amplitudes;
            }


        /* Trigonometrical formula - between I and II */
        private double IandII(double[]amplitudes)
        {
            double angle = Math.Atan((2 * (amplitudes[1] - amplitudes[0])) / (Math.Sqrt(3) * amplitudes[0]));
            return angle;   // an angle in radians
        }

    }
}

