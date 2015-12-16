using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                pseudo_tab[j] = Math.Sqrt(Math.Pow(i,2)+Math.Pow(i,2));
                j++;
               // return pseudo_tab;
            }
        }

        /* Finding Max */
        private uint Max(uint Q, uint S, double[] pseudo_tab, double[] signal)
        {
            uint max = Q;
            for(uint i = Q; i < S; i++)
            {
                if (signal[Q] < signal[i])
                {
                    max = i;
                }
            }
            return max;
        }

        //metoda najmniejszych kwadratow - ustalic

        // max z paraboli

        //odczytanie polozen

        //rownania trygonometryczne
        // atan((2*(ODPR2-ODPR1))/(sqrt(3)*ODPR1));

    }
}

