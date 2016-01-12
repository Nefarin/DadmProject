using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using EKG_Project.Modules.R_Peaks;
using EKG_Project.Modules.Heart_Class;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using EKG_Project.Modules.Heart_Axis;

namespace EKG_Project.Modules.HRT
{
    public partial class HRT : IModule
    {
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _samplesProcessed;
        private int _numberOfChannels;

        //stworzenie obiektów poszczególnych klas workerów
        private R_Peaks_Data_Worker _inputRpeaksWorker;
        private Heart_Class_Data_Worker _inputHeartclassWorker;
        private HRT_Data_Worker _outputWorker;

        //stworzenie obiektów poszczególnych klas danych
        private R_Peaks_Data _inputRpeaksData;
        private Heart_Class_Data _inputHeartclassData;
        private HRT_Data _outputData;
        private HRT_Params _params;

        private Vector<Double> _currentVector;



        public void Abort()
        {
            Aborted = true;
            _ended = true;
        }

        public bool Ended()
        {
            return _ended;
        }

        public void Init(ModuleParams parameters)
        {
            Params = parameters as HRT_Params;
            Aborted = false;
            if (!Runnable()) _ended = true;
            else
            {
                _ended = false;

                InputRpeaksWorker = new R_Peaks_Data_Worker(Params.AnalysisName);
                InputRpeaksWorker.Load();
                InputRpeaksData = InputRpeaksWorker.Data;

                //??NIE WIEM CZY .DATA czy .OUTPUTDATA

                InputHeartclassWorker = new Heart_Class_Data_Worker(Params.AnalysisName);
                InputHeartclassWorker.Load();
                InputHeartclassData = InputHeartclassWorker.Data;

                OutputWorker = new HRT_Data_Worker(Params.AnalysisName);
                OutputData = new HRT_Data();
            }
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
            throw new NotImplementedException();
        }

        //gettery i settery
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

        public HRT_Params Params
        {
            get
            {
                return _params;
            }
            set
            {
                _params = value;
            }
        }

        public R_Peaks_Data_Worker InputRpeaksWorker
        {
            get
            {
                return _inputRpeaksWorker;
            }
            set
            {
                _inputRpeaksWorker = value;
            }
        }

        public Heart_Class_Data_Worker InputHeartclassWorker
        {
            get
            {
                return _inputHeartclassWorker;
            }
            set
            {
                _inputHeartclassWorker = value;
            }
        }

        public HRT_Data_Worker OutputWorker
        {
            get
            {
                return _outputWorker;
            }
            set
            {
                _outputWorker = value;
            }
        }

        public R_Peaks_Data InputRpeaksData
        {
            get
            {
                return _inputRpeaksData;
            }
            set
            {
                _inputRpeaksData = value;
            }
        }

        public Heart_Class_Data InputHeartclassData
        {
            get
            {
                return _inputHeartclassData;
            }
            set
            {
                _inputHeartclassData = value;
            }
        }

        public HRT_Data OutputData
        {
            get
            {
                return _outputData;
            }
            set
            {
                _outputData = value;
            }
        }
    }
}
