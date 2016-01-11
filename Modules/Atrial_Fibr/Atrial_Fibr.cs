using System;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.R_Peaks;
using MathNet.Numerics;
using System.Collections.Generic;
using System.Globalization;

namespace EKG_Project.Modules.Atrial_Fibr
{
    public partial class Atrial_Fibr : IModule
    {
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _samplesProcessed;
        private int _numberOfChannels;

        //int qrsEndStep;// o tyle qrs się przesuwamy 
        //int i; //do inkrementacji co 10
        //int step; // ilość próbek o którą sie przesuwamy

        private R_Peaks_Data_Worker _inputRpeaksWorker;
        private Atrial_Fibr_Data_Worker _outputWorker;

        private R_Peaks_Data _inputRpeaksData;
        private Atrial_Fibr_Data _outputData;

        private Atrial_Fibr_Params _params;
        private Basic_Data _inputData_basic;
        private Basic_Data_Worker _inputWorker_basic;
        private Vector<Double> _currentVector;
        private Vector<Double> _vectorOfIntervals;
        private Tuple<bool, Vector<double>, double> _tempClassResult;
        private Tuple<bool, Vector<double>, double> _ClassResult; 

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
            Params = parameters as Atrial_Fibr_Params;
            Aborted = false;
            if (!Runnable()) _ended = true;
            else
            {
                _ended = false;
                InputWorker_basic = new Basic_Data_Worker(Params.AnalysisName);
                InputWorker_basic.Load();
                InputData_basic = InputWorker_basic.BasicData;

                InputRpeaksWorker = new R_Peaks_Data_Worker(Params.AnalysisName);
                InputRpeaksWorker.Load();
                InputRpeaksData = InputRpeaksWorker.Data;

                OutputWorker = new Atrial_Fibr_Data_Worker(Params.AnalysisName);
                OutputData = new Atrial_Fibr_Data();

                _currentChannelIndex = 0;
                _samplesProcessed = 0;
                NumberOfChannels = InputRpeaksData.RPeaks.Count;
                _currentChannelLength = InputRpeaksData.RRInterval[_currentChannelIndex].Item2.Count;
                _currentVector = Vector<Double>.Build.Dense(_currentChannelLength);
                _vectorOfIntervals = Vector<Double>.Build.Dense(_currentChannelLength);

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
            int startIndex = _samplesProcessed;
            int step=480;
            bool detected=false;
            Vector<double> pointsDetected=Vector<double>.Build.Dense(1);
            Vector<double> pointsDetected2;
            double lengthOfDetection = 0;
            _ClassResult = new Tuple<bool, Vector<double>, double>(detected, pointsDetected, lengthOfDetection);
            if (channel < NumberOfChannels)
            {
                
                if (startIndex + step >= _currentChannelLength)
                {
                    _currentVector = InputRpeaksData.RPeaks[_currentChannelIndex].Item2.SubVector(startIndex, _currentChannelLength - startIndex-1);
                    _vectorOfIntervals = InputRpeaksData.RRInterval[_currentChannelIndex].Item2.SubVector(startIndex, _currentChannelLength - startIndex-1);

                    _tempClassResult = detectAF(_vectorOfIntervals, _currentVector, Convert.ToUInt32(InputData_basic.Frequency), Params);

                    if (_tempClassResult.Item1 | _ClassResult.Item1)
                    {
                        detected = true;
                        pointsDetected.SetSubVector(pointsDetected.Count, _tempClassResult.Item2.Count, _tempClassResult.Item2);
                        lengthOfDetection += _tempClassResult.Item3;
                        _ClassResult = new Tuple<bool, Vector<double>, double>(detected, pointsDetected, lengthOfDetection);
                        pointsDetected2 = Vector<double>.Build.Dense(_ClassResult.Item2.Count - 1);
                    }
                    else
                    {
                        pointsDetected2 = Vector<double>.Build.Dense(1);
                    }
                    
                    if (_ClassResult.Item1)
                    {
                        double percentOfDetection = _ClassResult.Item3 / (InputRpeaksData.RPeaks[_currentChannelIndex].Item2.At(InputRpeaksData.RPeaks[_currentChannelIndex].Item2.Count) - InputRpeaksData.RPeaks[_currentChannelIndex].Item2.At(0))*100* Convert.ToUInt32(InputData_basic.Frequency);
                        OutputData.AfDetection.Add(new Tuple<bool, Vector<double>,string,string>(true,pointsDetected2, "Wykryto migotanie przedsionków.",
                            "Wykryto migotanie trwające "+ _ClassResult.Item3.ToString("F1", CultureInfo.InvariantCulture)+ "s. Stanowi to "+
                            percentOfDetection.ToString("F1", CultureInfo.InvariantCulture)+ "% trwania sygnału."));
                    }
                    else
                    {
                        OutputData.AfDetection.Add(new Tuple<bool, Vector<double>, string, string>(false, pointsDetected2, "Nie wykryto migotania przedsionków.",""));
                    }
                    _currentChannelIndex++;

                    if (_currentChannelIndex < NumberOfChannels)
                    {
                        _samplesProcessed = 0;
                        _currentChannelLength = InputRpeaksData.RPeaks[_currentChannelIndex].Item2.Count;
                        _currentVector = Vector<Double>.Build.Dense(_currentChannelLength);
                    }


                }
                else
                {

                    _currentVector = InputRpeaksData.RPeaks[_currentChannelIndex].Item2.SubVector(startIndex, step);
                    _vectorOfIntervals = InputRpeaksData.RRInterval[_currentChannelIndex].Item2.SubVector(startIndex, step);

                    _tempClassResult = detectAF(_vectorOfIntervals, _currentVector, Convert.ToUInt32(InputData_basic.Frequency), Params);

                    if (_tempClassResult.Item1 | _ClassResult.Item1)
                    {
                        detected = true;
                        pointsDetected.SetSubVector(pointsDetected.Count, _tempClassResult.Item2.Count, _tempClassResult.Item2);
                        lengthOfDetection += _tempClassResult.Item3;
                        _ClassResult = new Tuple<bool, Vector<double>, double>(detected, pointsDetected, lengthOfDetection);
                    }
                    
                    _samplesProcessed = startIndex + step;

                }
            }
            else
            {
                OutputWorker.Save(OutputData);
                _ended = true;
            }
        }


        public bool Aborted
        {
            get { return _aborted; }
            set { _aborted = value; }
        }

        public Atrial_Fibr_Params Params
        {
            get { return _params; }
            set { _params = value; }
        }
        Atrial_Fibr_Data OutputData
        {
            get { return _outputData; }
            set { _outputData = value; }
        }
        Atrial_Fibr_Data_Worker OutputWorker
        {
            get { return _outputWorker; }
            set { _outputWorker = value; }
        }

        public int NumberOfChannels
        {
            get { return _numberOfChannels; }
            set { _numberOfChannels = value; }
        }

        public Basic_Data_Worker InputWorker_basic
        {
            get { return _inputWorker_basic; }
            set { _inputWorker_basic = value; }
        }

        public R_Peaks_Data_Worker InputRpeaksWorker
        {
            get { return _inputRpeaksWorker; }
            set { _inputRpeaksWorker = value; }
        }

        public R_Peaks_Data InputRpeaksData
        {
            get { return _inputRpeaksData; }
            set { _inputRpeaksData = value; }
        }
        public Basic_Data InputData_basic
        {
            get{ return _inputData_basic;}
            set {_inputData_basic = value;}
        }

        public static void Main()
        {
            Atrial_Fibr_Params param = new Atrial_Fibr_Params(Detect_Method.STATISTIC, "TestAnalysis2");

            Atrial_Fibr testModule = new Atrial_Fibr();
            testModule.Init(param);
            while (true)
            {
                //Console.WriteLine("Press key to continue.");
                Console.Read();
                if (testModule.Ended()) break;
                Console.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }

        }
    }
}