using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;

namespace EKG_Project.Modules.Heart_Axis
{
    public class Heart_Axis_Alg
    {

        #region Heart Axis Class doc
        /// <summary>
        /// Class that calculates Heart Axis
        /// </summary>
        #endregion


        #region Documentation
        /// <summary>
        /// Method calculates pseudo module in first lead
        /// </summary>
        /// <param name="Q">Q as int</param>
        /// <param name="S">S as int</param>
        /// <param name="signal">array with signal</param>
        /// <returns>array of doubles with pseudo modules</returns>
        #endregion


        /*Pseudo Module*/

        public double[] PseudoModule(int Q, int S, double[] signal)
        {

            int j = 0;
            double[] pseudo_tab;
            pseudo_tab = new double[S - Q];
            for (int i = Q; i < S; i++, j++)
            {
                pseudo_tab[j] = Math.Sqrt(Math.Pow(signal[i], 2) + Math.Pow(signal[i + 1], 2));
            }
            return pseudo_tab;
        }

        #region Documentation
        /// <summary>
        /// Method finds maximum in pseudo module
        /// </summary>
        /// <param name="Q">Q as int</param>
        /// <param name="pseudo_tab">array with pseudo modules</param>
        /// <returns>maximum of array with pseudo modules as int</returns>
        #endregion


        /* Finding Max */

        public int MaxOfPseudoModule(int Q, double[] pseudo_tab)
        {

            double maxValue = pseudo_tab.Max();
            int maxIndex = Array.IndexOf(pseudo_tab, maxValue);
            return maxIndex;
        }

        #region Documentation
        /// <summary>
        /// Method implaments Least-Squeres method
        /// </summary>
        /// <param name="signal">array with signal</param>
        /// <param name="Q">Q as int</param>
        /// <param name="pseudo_tab">array with pseudo modules</param>
        /// <param name="frequency">frequency as int</param>
        /// <returns>fitting parameters as array of doubles</returns>
        #endregion


        /*Least-Squares method*/

        public double[] LeastSquaresMethod(double[] signal, int Q, double[] pseudo_tab, int frequency)
        {
            int MaxIndex = MaxOfPseudoModule(Q, pseudo_tab);
            int timePeriod = 20;
            int milisecondsDivider = 1000;
            double T = (timePeriod * frequency) / milisecondsDivider; // ilość próbek na 1 ms
            int roundedT = (int)Math.Round(T, MidpointRounding.AwayFromZero);

            int signalSize = signal.Length;
            double[] indexes = null;
            int partArrayLength = 0;
            if ((MaxIndex - roundedT >= 0) && (MaxIndex + roundedT <= signalSize))
            {
                partArrayLength = 2 * roundedT + 1;
                indexes = new double[partArrayLength];
            }
            else
            {
                double[] zeros = { 0, 0, 0 };
                return zeros;
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

        #region Documentation
        /// <summary>
        /// Method finds maximum from polynomial equation
        /// </summary>
        /// <param name="Q">Q</param>
        /// <param name="fitting_parameters">double array with fitting parameters</param>
        /// <returns> maximum of polynomial as int</returns>
        #endregion


        /*Max of Polynomial*/
        public int MaxOfPolynomial(int Q, double[] fitting_parameters)
        {
            if (fitting_parameters[2] == 0) return 0;
            double dMaxOfPoly = (-fitting_parameters[1]) / (2 * fitting_parameters[2]);       // x = -b/2a
            int maxOfPoly = (int)Math.Round(dMaxOfPoly, MidpointRounding.AwayFromZero);
            return maxOfPoly;
        }

        #region Documentation
        /// <summary>
        /// Method reads the amplitudes from both leads
        /// </summary>
        /// <param name="FirstLead">array with signal from first lead</param>
        /// <param name="SecondLead">array with signal from second lead</param>
        /// <param name="MaxOfPoly">maximum of polynomial</param>
        /// <returns>double array with two read amplitudes</returns>
        #endregion

        /*Reading Amplitudes*/
        public double[] ReadingAmplitudes(double[] FirstLead, double[] SecondLead, int MaxOfPoly)
        {
            if (MaxOfPoly == 0)
            {
                double[] zeros = { 0, 0 };
                return zeros;
            }
            double[] amplitudes = new double[2];
            amplitudes[0] = FirstLead[MaxOfPoly];
            amplitudes[1] = SecondLead[MaxOfPoly];
            return amplitudes;
        }

        #region Documentation
        /// <summary>
        /// Method calculates trigonometrical formula - between lead I and II
        /// </summary>
        /// <param name="amplitudes">read amplitudes</param>
        /// <returns>heart axis in radians as double</returns>
        #endregion

        /* Trigonometrical formula - between I and II */
        public double IandII(double[] amplitudes)
        {
            if (amplitudes[0] == 0)
            {
                return 0;
            }
            double angle = Math.Atan((2 * (amplitudes[1] - amplitudes[0])) / (Math.Sqrt(3) * amplitudes[0]));
            return angle;   // an angle in radians
        }


    }
}

