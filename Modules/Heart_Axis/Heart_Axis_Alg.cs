﻿using System;
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

        //todo: poprawne q i s - w pliku głównym?


        private double[] PseudoModule(uint Q, uint S, double[] signal)
        {

            uint j = 0;
            double[] pseudo_tab;
            pseudo_tab = new double[S - Q];
            for (uint i = Q; i < S; i++,j++)
            {
                pseudo_tab[j] = Math.Sqrt(Math.Pow(signal[i], 2) + Math.Pow(signal[i+1], 2));
            }
            return pseudo_tab;
        }

        /* Finding Max */
        private uint MaxOfPseudoModule(uint Q, double[] pseudo_tab)
        {

            double maxValue = pseudo_tab.Max();
            int maxIndex = Array.IndexOf(pseudo_tab, maxValue);
            uint uMaxIndex = (uint)maxIndex;
            uMaxIndex = uMaxIndex + Q;
            return uMaxIndex;
        }

        /*Least-Squares method*/
       
        private double[] LeastSquaresMethod(double []signal, uint Q, double[] pseudo_tab, int frequency) // todo: skąd wziąć tablicę samples
        {
            uint uMaxIndex = MaxOfPseudoModule(Q, pseudo_tab);
            int timePeriod = 40; // czy 20?
            int milisecondsDivider = 1000;
            double T = (timePeriod * frequency) / milisecondsDivider; // ilość próbek na 1 ms -  todo: sprawdzić czy zwraca poprawną wartość, zabezpieczyć się przed dzieleniem przez zero
            int roundedT = (int)Math.Round(T, MidpointRounding.AwayFromZero); // zaokrąglenie - zmienić na floor

            //Trzeba się zabezpieczyć przed wyjściem poza zakres tablicy!

            int signalSize = signal.Length;
            double[] indexes = null;
            int partArrayLength = 0;
            if ((uMaxIndex - roundedT >= 0) && (uMaxIndex + roundedT < signalSize))
            {
                partArrayLength = 2 * roundedT + 1;
                indexes = new double[partArrayLength];
            }
            else
            {
                throw new Exception(); //jak wyrzucać wyjątki, jeśli dane są niepoprawne?
            }
            

            for (int i = 0; i < indexes.Length; i++) // inicjalizacja indexes/samples numerami próbek
            {
                indexes[i] = uMaxIndex - roundedT + i; //zrobić, żeby było od 1???
            }

            // inicjalizacja tablicy z częścią sygnału
            double[] partSignal = new double[partArrayLength];

            int startCopyIndex = (int)uMaxIndex - roundedT;

            // przekopiuj z tablicy signal do tablicy partSignal n elementów (gdzie n = indexes.Length)
            Array.Copy(signal, startCopyIndex, partSignal, 0, indexes.Length);

            int order = 2;
            double[] bestFitCoefficients = Fit.Polynomial(indexes, partSignal, order);
            return bestFitCoefficients;
        }
        


        /*Max of Polynomial*/
        private double MaxOfPolynomial(uint Q, double [] fitting_parameters)
        {
            double maxOfPoly =(-fitting_parameters[1])/(2*fitting_parameters[0]);       // x = -b/2a
            uint uMaxOfPoly = (uint)maxOfPoly;
            //uMaxOfPoly = uMaxOfPoly; //ew. dodać Q, jeśli zmienię wyżej
            return uMaxOfPoly;
        }
  

        /*Reading Amplitudes*/
        private double[] ReadingAmplitudes(double[] FirstLead, double[] SecondLead, uint uMaxOfPoly)
            {
            double[] amplitudes = new double[2];
            amplitudes[0] = FirstLead[uMaxOfPoly];
            amplitudes[1] = SecondLead[uMaxOfPoly];
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

