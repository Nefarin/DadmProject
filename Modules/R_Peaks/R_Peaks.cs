using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.R_Peaks
{
    public partial class R_Peaks : IModule
    {
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _samplesProcessed;
        private int _numberOfChannels;

        private Basic_Data_Worker _inputWorker;
        private R_Peaks_Data_Worker _outputWorker;

        private R_Peaks_Data _outputData;
        private Basic_Data _inputData;
        private R_Peaks_Params _params;

        private Vector<Double> _currentVector;
        /*
        public R_Peaks()
        {
            Delay = 0;
            LocsR = Vector<double>.Build.Dense(1);
            RRms = Vector<double>.Build.Dense(1);
        }*/

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
            Params = parameters as R_Peaks_Params;
            Aborted = false;
            if (!Runnable()) _ended = true;
            else
            {
                _ended = false;

                InputWorker = new Basic_Data_Worker(Params.AnalysisName);
                InputWorker.Load();
                InputData = InputWorker.BasicData;          

                OutputWorker = new R_Peaks_Data_Worker(Params.AnalysisName);
                OutputData = new R_Peaks_Data();

                _currentChannelIndex = 0;
                _samplesProcessed = 0;
                NumberOfChannels = InputData.Signals.Count;
                _currentChannelLength = InputData.Signals[_currentChannelIndex].Item2.Count;
                _currentVector = Vector<Double>.Build.Dense(_currentChannelLength);

            }
        }

        public void ProcessData()
        {
            if (Runnable()) processData();
            else _ended = true;
        }

        public double Progress()
        {
            return 100.0 * ((double)_currentChannelIndex / (double)NumberOfChannels + (1.0 / NumberOfChannels) * ((double)_samplesProcessed / (double)_currentChannelLength));
        }

        public bool Runnable()
        {
            return Params != null;
        }

        private void processData()
        {
            int channel = _currentChannelIndex;
            int startIndex = (_samplesProcessed == 0) ? _samplesProcessed : LastRPeak;
            //int startIndex = _samplesProcessed; ///TODO: ostatni Rpeak+30
            int step = 6000; //krok (porcja sygnału)

            if (channel < NumberOfChannels)
            {
                if (startIndex + step > _currentChannelLength)
                {
                    //scaleSamples(channel, startIndex, _currentChannelLength - startIndex); ///TODO wybór metody detekcji:

                    switch (Params.Method)
                    {
                        // no idea what I'm doing
                        // skąd _currentVector wie, że jest sygnałem?
                        case R_Peaks_Method.PANTOMPKINS:
                            _currentVector = Hilbert(_currentVector, InputData.Frequency);
                            break;
                        case R_Peaks_Method.HILBERT:
                            _currentVector = PanTompkins(_currentVector, InputData.Frequency);
                            break;
                        case R_Peaks_Method.EMD:
                            //_currentVector = EMD(_currentVector, InputData.Frequency);
                            break;
                    }

                    OutputData.RPeaks.Add(new Tuple<string, Vector<double>>(InputData.Signals[_currentChannelIndex].Item1, _currentVector)); // Czy to doda Rpeaki do Rpeaków
                    OutputData.RRInterval.Add(new Tuple<string, Vector<double>>(InputData.Signals[_currentChannelIndex].Item1, _currentVector)); // A to RRinterwały do RRinterwałów bez dodatkowej zabawy (tzn. nowych currentVectorów czy cos?
                    _currentChannelIndex++;
                    if (_currentChannelIndex < NumberOfChannels)
                    {
                        _samplesProcessed = 0;
                        _currentChannelLength = InputData.Signals[_currentChannelIndex].Item2.Count;
                        _currentVector = Vector<Double>.Build.Dense(_currentChannelLength);
                    }


                }
                else
                {
                    //scaleSamples(channel, startIndex, step); ///TODO przerobić (jak wyżej):
                    _samplesProcessed = startIndex + step;
                }
            }
            else
            {
                OutputWorker.Save(OutputData);
                _ended = true;
            }


        }

        public R_Peaks_Data OutputData
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

        public R_Peaks_Params Params
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

        public int NumberOfChannels
        {
            get
            {
                return _numberOfChannels;
            }

            set
            {
                _numberOfChannels = value;
            }
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

        public Basic_Data InputData
        {
            get
            {
                return _inputData;
            }

            set
            {
                _inputData = value;
            }
        }

        public Basic_Data_Worker InputWorker
        {
            get
            {
                return _inputWorker;
            }

            set
            {
                _inputWorker = value;
            }
        }

        public R_Peaks_Data_Worker OutputWorker
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

        public static void Main()
        {
            R_Peaks_Params param = new R_Peaks_Params(R_Peaks_Method.PANTOMPKINS, "Analysis6");
            //R_Peaks_Params param = null;
            R_Peaks testModule = new R_Peaks();
            testModule.Init(param);
            while (true)
            {
                //Console.WriteLine("Press key to continue.");
                //Console.Read();
                if (testModule.Ended()) break;
                Console.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }
        }
    }
}
