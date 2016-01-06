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
        private void PseudoModule(uint Q, uint S, double[] signal) // sygnal? spr
        {
            uint j = 1;
            double[] pseudo_tab;
            pseudo_tab = new double[Q - S + 1];
            for (uint i = Q; i < S; i++)
            {
                pseudo_tab[j] = Math.Sqrt(Math.Pow(i, 2) + Math.Pow(i, 2));
                j++;
                // return pseudo_tab;
            }
        }

        /* Finding Max */
        private uint Max(uint Q, uint S, double[] pseudo_tab, double[] signal)
        {
            uint max = Q;
            for (uint i = Q; i < S; i++)
            {
                if (signal[Q] < signal[i])
                {
                    max = i;
                }
                //znalezc funkcje max
            }
            return max;
        }

        /*Least-Squares method*/
        // Double[] Polynomial(Double[] x, Double[] y, int order, DirectRegressionMethod method)
        /*double[] Fitting(double[] xdata, double[] ydata, int order)
        {
            double[] p = Fit.Polynomial(xdata, ydata, 3);
            return p;
        }*/

        private double[] LeastSquaresMethod(double[] signal, double[] pseudo_tab)
        {
            int order = 2;
            double[] bestFitCoefficients = Fit.Polynomial(signal, pseudo_tab, order);
            return bestFitCoefficients;
        }


        /*Max of Polynomial*/
        /*private double First(double [] fitting_parameters)
        {
            double I = 0;
            I =(-fitting_parameters[1])/(2*fitting_parameters[0]);
                return I;
        }*/
        // x = -b/2a

        //odczytanie polozen


        /* Trigonometrical formula - between I and II */
        private double IandII(double I, double II)
        {
            double angle = Math.Atan((2 * (II - I)) / (Math.Sqrt(3) * I));
            return angle;   // an angle in radians
        }

    }
}

