using System;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.R_Peaks;
using MathNet.Numerics;
using System.Collections.Generic;
using System.Globalization;

namespace EKG_Project.Modules.Atrial_Fibr
{
    public class Atrial_Fibr : IModule
    {
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentChannelLength;
        private int _samplesProcessed;
        private int _numberOfChannels;

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
        Vector<double> pointsDetectedA;

        public void Abort()
        {
            Aborted = true;
            _ended = true;
        }

        public bool IsAborted()
        {
            return Aborted;
        }

        public bool Ended()
        {
            return _ended;
        }

        public void Init(ModuleParams parameters)
        {
            //Params = parameters as Atrial_Fibr_Params;
            //Aborted = false;
            //if (!Runnable()) _ended = true;
            //else
            //{
            //    _ended = false;
            //    InputWorker_basic = new Basic_Data_Worker(Params.AnalysisName);
            //    InputWorker_basic.Load();
            //    InputData_basic = InputWorker_basic.BasicData;

            //    InputRpeaksWorker = new R_Peaks_Data_Worker(Params.AnalysisName);
            //    InputRpeaksWorker.Load();
            //    InputRpeaksData = InputRpeaksWorker.Data;

            //    OutputWorker = new Atrial_Fibr_Data_Worker(Params.AnalysisName);
            //    OutputData = new Atrial_Fibr_Data();

            //    _currentChannelIndex = 0;
            //    _samplesProcessed = 0;
            //    NumberOfChannels = InputRpeaksData.RPeaks.Count;
            //    _currentChannelLength = InputRpeaksData.RRInterval[_currentChannelIndex].Item2.Count;
            //    _currentVector = Vector<Double>.Build.Dense(_currentChannelLength);
            //    _vectorOfIntervals = Vector<Double>.Build.Dense(_currentChannelLength);
            //    _ClassResult = new Tuple<bool, Vector<double>, double>(false, pointsDetected, 0);
            //    pointsDetectedA = Vector<double>.Build.Dense(Convert.ToInt32(InputData_basic.SampleAmount));

            //}
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

            //int channel = _currentChannelIndex;
            //int startIndex = _samplesProcessed;
            //int step=480;
            //bool detected=false;
            //double lengthOfDetection = 0;
            
            //if (channel < NumberOfChannels)
            //{
            //    int currentSample=0;
                
            //    if (startIndex + step >= _currentChannelLength)
            //    {
            //        _currentVector = InputRpeaksData.RPeaks[_currentChannelIndex].Item2.SubVector(startIndex, _currentChannelLength - startIndex-1);
            //        _vectorOfIntervals = InputRpeaksData.RRInterval[_currentChannelIndex].Item2.SubVector(startIndex, _currentChannelLength - startIndex-1).Multiply(InputData_basic.Frequency/1000);
                    
            //        _tempClassResult = detectAF(_vectorOfIntervals, _currentVector, Convert.ToUInt32(InputData_basic.Frequency), Params);
            //        Vector<double> pointsDetected2;
            //        if (_tempClassResult.Item1 | _ClassResult.Item1)
            //        {
            //            detected = true;
            //            if (_tempClassResult.Item1)
            //            {
            //                pointsDetectedA.SetSubVector(Convert.ToInt32(_ClassResult.Item3 * InputData_basic.Frequency), _tempClassResult.Item2.Count, _tempClassResult.Item2);
            //                lengthOfDetection = _ClassResult.Item3+_tempClassResult.Item3;
            //                _ClassResult = new Tuple<bool, Vector<double>, double>(detected, pointsDetectedA, lengthOfDetection);
            //            }
            //            currentSample = Convert.ToInt32(_ClassResult.Item3 * InputData_basic.Frequency) - 1;
            //            //_ClassResult = new Tuple<bool, Vector<double>, double>(detected, pointsDetected, lengthOfDetection);
            //            //int tmp = Convert.ToInt32(lengthOfDetection * InputData_basic.Frequency);
            //            pointsDetected2 = Vector<double>.Build.Dense(currentSample - 1);
            //            pointsDetectedA.CopySubVectorTo(pointsDetected2, 0, 0, currentSample - 1);
                        

            //        }
            //        else
            //        {
            //            pointsDetected2 = Vector<double>.Build.Dense(1);
            //        }
                    
            //        if (detected)
            //        {
            //            double percentOfDetection = _ClassResult.Item3 / (InputRpeaksData.RPeaks[_currentChannelIndex].Item2.At(InputRpeaksData.RPeaks[_currentChannelIndex].Item2.Count-1) - InputRpeaksData.RPeaks[_currentChannelIndex].Item2.At(0))*100* Convert.ToUInt32(InputData_basic.Frequency);
            //            OutputData.AfDetection.Add(new Tuple<bool, Vector<double>,string,string>(true,pointsDetected2, "Wykryto migotanie przedsionków.",
            //                "Wykryto migotanie trwające "+ _ClassResult.Item3.ToString("F1", CultureInfo.InvariantCulture)+ "s. Stanowi to "+
            //                percentOfDetection.ToString("F1", CultureInfo.InvariantCulture)+ "% trwania sygnału."));
            //        }
            //        else
            //        {
            //            OutputData.AfDetection.Add(new Tuple<bool, Vector<double>, string, string>(false, pointsDetected2, "Nie wykryto migotania przedsionków.",""));
            //        }
            //        _currentChannelIndex++;

            //        if (_currentChannelIndex < NumberOfChannels)
            //        {
            //            _samplesProcessed = 0;
            //            _currentChannelLength = InputRpeaksData.RPeaks[_currentChannelIndex].Item2.Count;
            //            _currentVector = Vector<Double>.Build.Dense(_currentChannelLength);
            //        }


            //    }
            //    else
            //    {

            //        _currentVector = InputRpeaksData.RPeaks[_currentChannelIndex].Item2.SubVector(startIndex, step);
            //        _vectorOfIntervals = InputRpeaksData.RRInterval[_currentChannelIndex].Item2.SubVector(startIndex, step).Multiply(InputData_basic.Frequency/1000) ;
            //        _tempClassResult = detectAF(_vectorOfIntervals, _currentVector, Convert.ToUInt32(InputData_basic.Frequency), Params);

            //        if (_tempClassResult.Item1)
            //        {
            //            detected = true;
            //            Console.WriteLine("_tempClassResult.Item2.Count");
            //            Console.WriteLine(_tempClassResult.Item2.Count);
            //            pointsDetectedA.SetSubVector(Convert.ToInt32(_ClassResult.Item3*InputData_basic.Frequency), _tempClassResult.Item2.Count, _tempClassResult.Item2);
            //            lengthOfDetection = _ClassResult.Item3+_tempClassResult.Item3;
            //            currentSample += _tempClassResult.Item2.Count ;
            //            _ClassResult = new Tuple<bool, Vector<double>, double>(detected, pointsDetectedA, lengthOfDetection);
            //        }
                    
            //        _samplesProcessed = startIndex + step;

            //    }
            //}
            //else
            //{
            //    OutputWorker.Save(OutputData);
            //    _ended = true;
            //}
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

        //public static void Main()
        //{
        //    Atrial_Fibr_Params param = new Atrial_Fibr_Params(Detect_Method.STATISTIC, "AF1");

        //    Atrial_Fibr testModule = new Atrial_Fibr();
        //    testModule.Init(param);
        //    while (true)
        //    {
        //        //Console.WriteLine("Press key to continue.");
        //        //Console.Read();
        //        if (testModule.Ended()) break;
        //        Console.WriteLine(testModule.Progress());
        //        testModule.ProcessData();
        //    }
        //    Console.WriteLine("Analiza zakonczona. Press key to continue.");
        //    Console.Read();
        //}
    }
}