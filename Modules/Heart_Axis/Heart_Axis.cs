using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using MathNet.Numerics;

namespace EKG_Project.Modules.Heart_Axis
{
    public partial class Heart_Axis : IModule
    {

        /* Zmienne do komunikacji z GUI - czy liczenie osi serca się skończyło/zostało przerwane */

        private bool _ended;
        private bool _aborted;

        /* Stałe */

        private const string LeadISymbol = "I";
        private const string LeadIISymbol = "II";

        /* Dane wejściowe */

        //private Basic_Data_Worker _inputBasicDataWorker; //Basic_Data_Worker could not be found

        /* Dane wyjściowe - Kąt osi serca  */

        //private Heart_Axis_Data_Worker _output;   //Heart_Axis_Data_Worker could not be found
        private Heart_Axis_Data _outputData; //Heart_Axis_Data could not be found

        public void Abort()
        {
            Aborted = true;
            _ended = true;
        }

        public bool Ended()
        {
            return _ended;
        }

        public void Init(ModuleParams parameters) //moduł nie przyjmuje parametrów, co tu napisać?
        {
            throw new NotImplementedException();
        }

        public void ProcessData(int numberOfSamples)
        {
            throw new NotImplementedException();
        }

        public double Progress()
        {
            throw new NotImplementedException();
        }

        public bool Runnable()
        {
            // czy w GUI zostały ustawione parametry do obliczeń - brak parametrów -  zawsze true?
            //return Params != null; //params does not exist
            throw new NotImplementedException();
        }

        public bool Aborted
        {
            get
            {
                return _aborted;
            }

            set
            {
                _aborted = value;
            }
        }

    }
     
}