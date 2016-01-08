using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using EKG_Project.Modules.ECG_Baseline;
using EKG_Project.Modules.Waves;
using EKG_Project.Modules.R_Peaks;

namespace EKG_Project.Modules.QT_Disp
{
    public partial class QT_Disp : IModule
    {
        private bool _ended;
        private bool _aborted;

        //input workers
        private ECG_Baseline_Data_Worker _inputECGBaselineWorker;
        private R_Peaks_Data_Worker _inputRPeaksWorker;
        private Waves_Data_Worker _inputWavesWorker;
        private Basic_Data_Worker _inputBasicWorker;

        //output worker
        private QT_Disp_Data_Worker _outputWorker;

        //input data
        private ECG_Baseline_Data _inputECGBaselineData;
        private R_Peaks_Data _inputRPeaksData;
        private Waves_Data _inputWavesData;
        private Basic_Data _inputBasicData;

        //output data
        private QT_Disp_Data _outputData;
       
        private QT_Disp_Params _params;

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
            Params = parameters as QT_Disp_Params;
            Aborted = false;
            if (!Runnable()) _ended = true;
            else
            {
                //input workers
                InputECGBaselineWorker = new ECG_Baseline_Data_Worker(Params.AnalysisName);
                InputECGBaselineWorker.Load();
                InputECGBaselineData = InputECGBaselineWorker.Data;

                InputRPeaksWorker = new R_Peaks_Data_Worker(Params.AnalysisName);
                InputRPeaksWorker.Load();
                InputRPeaksData = InputRPeaksWorker.Data;

                InputWavesWorker = new Waves_Data_Worker(Params.AnalysisName);
                InputWavesWorker.Load();
                InputWavesData = InputWavesWorker.Data;

                InputBasicWorker = new Basic_Data_Worker(Params.AnalysisName);
                InputBasicWorker.Load();
                InputBasicData = InputBasicWorker.BasicData;
                //output workers

                OutputWorker = new QT_Disp_Data_Worker(Params.AnalysisName);
                OutputWorker.Load();
                OutputData = new QT_Disp_Data();



            }
            
        }

        public void ProcessData()
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
        //Getters and Setters
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
        //input workers
        public ECG_Baseline_Data_Worker InputECGBaselineWorker
        {
            get
            {
                return _inputECGBaselineWorker;
            }
            set
            {
                _inputECGBaselineWorker = value;
            }
        }
        public Waves_Data_Worker InputWavesWorker
        {
            get
            {
                return _inputWavesWorker;
            }
            set
            {
                _inputWavesWorker = value;
            }
        }
        public R_Peaks_Data_Worker InputRPeaksWorker
        {
            get
            {
                return _inputRPeaksWorker;
            }
            set
            {
                _inputRPeaksWorker = value;
            }
        }
        public Basic_Data_Worker InputBasicWorker
        {
            get
            {
                return _inputBasicWorker;
            }
            set
            {
                _inputBasicWorker = value;
            }
        }
        
        //output worker
        private QT_Disp_Data_Worker OutputWorker
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
        //input Data
        public ECG_Baseline_Data InputECGBaselineData
        {
            get
            {
                return _inputECGBaselineData;
            }
            set
            {
                _inputECGBaselineData = value;
            }
        }
        public Waves_Data InputWavesData
        {
            get
            {
                return _inputWavesData;
            }
            set
            {
                _inputWavesData = value;
            }
        }
        public R_Peaks_Data InputRPeaksData
        {
            get
            {
                return _inputRPeaksData;
            }
            set
            {
                _inputRPeaksData = value;
            }
        }
        public Basic_Data InputBasicData
        {
            get
            {
                return _inputBasicData;
            }
            set
            {
                _inputBasicData = value;
            }
        }
        //output Data
        public QT_Disp_Data OutputData
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
            
        public QT_Disp_Params Params
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
        
    }
}
