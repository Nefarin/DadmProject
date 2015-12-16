using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;


namespace EKG_Project.Modules.HRV1
{
    public class HRV1_Params : ModuleParams
    {
        //HRV1 input parameters 
        private double _Fs;
        private double _dt;

        //HRV1 output parameters
        //Statistical parameters - for table1
        private double _SDNN;
        private double _RMSSD;
        private double _SDSD;
        private double _NN50;
        private double _pNN50;

        // Frequency parameters - needed for frequency chart and for table2
        private double _HF;
        private double _LF;
        private double _VLF;
        private double _LFHF;
        //vector needed for frequency chart
        private Vector<double> _f;           
        private Vector<double> _PSD;

        public HRV1_Params(double SDNN, double RMSSD, double SDSD, double NN50, double pNN50)
        {
            _SDNN = SDNN;
            _RMSSD = RMSSD;
            _SDSD = SDSD;
            _NN50 = NN50;
            _pNN50 = pNN50;
        }

        public HRV1_Params(double HF, double LF, double VLF, double LFHF)
        {
            _HF = HF;
            _LF = LF;
            _VLF = VLF;
            _LFHF = LFHF;
         }

       
        public double SDNN
        { get
            { return _SDNN;}
          set
            { _SDNN = value;}
        }

        public double RMSSD
        { get
            { return _RMSSD; }
          set
            { _RMSSD = value; }
        }

        public double SDSD
        { get
            { return _SDSD; }
          set
            { _SDSD = value; }
        }

        public double NN50
        { get
            { return _NN50; }
          set
            { _NN50 = value; }
        }

        public double pNN50
        { get
            { return _pNN50; }
          set
            { _pNN50 = value; }
        }

        public double HF
        { get
            { return _HF; }
           set
            { _HF = value; }
        }

        public double LF
        { get
            { return _LF; }
          set
            { _LF = value; }
        }

        public double VLF
        { get
            { return _VLF; }
          set
            { _VLF = value; }
        }

        public double LFHF
        { get
            { return _LFHF; }
          set
            { _LFHF = value; }
        }

        public Vector<double> f
        { get
            { return _f; }
          set
            { _f = value; }
        }

        public Vector<double> PSD
        { get
            { return _PSD; }
          set
            { _PSD = value; }
        }
    }
}
