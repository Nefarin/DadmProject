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

        /* Input Data */

        private double[] EKGSignalValues;
        private double[] EKGSignalTimestamps; //potrzebne to?
        private int samplingRate;
        private int QSampleNumber; //wektor? i string?
        private int SSampleNumber; //wektor? i string?


        // todo: zastanowić się, czy trzeba dopisać settery i gettery do tych pól

        // PARAMETRY I TESTY

        // todo: dowiedzieć się, skąd dostaje się listę parametrów, i jakie są wartości w stringach

        // todo: ta funkcja powinna być konstruktorem - jak tu działają konstruktory?

        //public void InitializeFields

        /*public Heart_Axis((List<Tuple<String, Vector<double>>> parameters) //Heart_Axis? bledy - namespace tupple i namespace vector<double>
        {
            foreach (Tupple<String, Vector<double>> parameter in parameters)
            {
                String parameterName = parameter.Item1;
                Vector<double> parameterData = parameter.Item2;
                // todo: dowiedzieć się jak rozpoznawać co jest czym w liście; na podstawie nazw wykonać odpowiednie przypisania
            }
        }*/


        //private check // eee?

        /*Pseudo Module*/

        //todo: ujednoznacznić nazwy zmiennych - czemu?
        //todo: dodać zwracanie wartości

        private void PseudoModule(uint Q, uint S, double[] signal) // sygnal? spr
        {
            if (Q > S)
            {
                // todo: wyrzucić wyjątek z błędnymi parametrami - gdy S mniejsze od Q
            }
            uint j = 0;
            double[] pseudo_tab;
            pseudo_tab = new double[S - Q];
            for (uint i = Q; i < S; i++,j++)
            {
                pseudo_tab[j] = Math.Sqrt(Math.Pow(signal[i], 2) + Math.Pow(signal[i+1], 2));
            }
            //return pseudo_tab[]; //error: value expected
        }

        /* Finding Max */
        private int MaxOfPseudoModule(int Q, uint S, double[] pseudo_tab, double[] signal)
        {

            if (Q > S)
            {
                // todo: wyrzucić wyjątek z błędnymi parametrami
            }

            double maxValue = pseudo_tab.Max();
            int maxIndex = Array.IndexOf(pseudo_tab, maxValue); //Array? todo: zmienić na uint, potrzebny cast?
            maxIndex = maxIndex + Q;


            return maxIndex;
        }

            

            /*int QSLength = S - Q + 1;
            double[] QSArray = new double[QSLength];
            Array.Copy(signal, Q, QSArray, 0, QSLength);

            int maxValue = QSArray.Max();
            uint maxIndex = Array.IndexOf(QSArray, maxValue);

            return maxIndex;    //???*/



        /*Least-Squares method*/
        // Double[] Polynomial(Double[] x, Double[] y, int order, DirectRegressionMethod method)
        /*double[] Fitting(double[] xdata, double[] ydata, int order)
        {
            double[] p = Fit.Polynomial(xdata, ydata, 3);
            return p;
        }*/

        private double[] LeastSquaresMethod(double[] signal, double[] samples) // todo: skąd wziąć tablicę samples
        {
            int order = 2;
            double[] bestFitCoefficients = Fit.Polynomial(signal, samples, order);
            return bestFitCoefficients;
        }


        /*Max of Polynomial*/
        private double MaxOfPolynomial(double [] fitting_parameters) //todo: przekonwertować na uint
        {
            double I = 0;
            I =(-fitting_parameters[1])/(2*fitting_parameters[0]);
                return I; //todo: dodać Q?
        }
        // x = -b/2a

        //odczytanie polozen
        /*Reading Amplitudes*/
        private double[] ReadingAmplitudes(double[] FirstLead, double[] SecondLead, uint maximum)
            {
            double[] amplitudes = new double[1]; //1? 2 elementy
            amplitudes[0] = FirstLead[maximum];
            amplitudes[1] = SecondLead[maximum];
            return amplitudes;
            }


        /* Trigonometrical formula - between I and II */ //todo: zamiana na tablicę dwuelementową?
        private double IandII(double I, double II)
        {
            double angle = Math.Atan((2 * (II - I)) / (Math.Sqrt(3) * I));
            return angle;   // an angle in radians
        }

    }
}

