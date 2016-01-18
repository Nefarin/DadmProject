using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using EKG_Project.Modules.R_Peaks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;

namespace EKG_Project.Modules.HRV_DFA
{
    
    public partial class HRV_DFA : IModule
    {
        private bool _ended;
        private bool _aborted;

        private int _currentChannelIndex;
        private int _currentRpeaksLength;
        private int _rPeaksProcessed;
        private int _numberOfChannels;

        
        private R_Peaks_Data_Worker _inputWorker;
        private R_Peaks_Data _inputData;

        private HRV_DFA_Data_Worker _outputWorker;
        private HRV_DFA_Data _outputData;

        private HRV_DFA_Params _params;

       /* private List<Tuple<string, Vector<double>, Vector<double>>> numberN;
        private List<Tuple<string, Vector<double>, Vector<double>>> fnValue;
        private List<Tuple<string, Vector<double>, Vector<double>>> pAlpha;*/

        private Vector<double> _currentVector;

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
            Params = parameters as HRV_DFA_Params;
            Aborted = false;
            if (!Runnable()) _ended = true;
            else
            {
                _ended = false;

                InputWorker = new R_Peaks_Data_Worker(Params.AnalysisName);
                InputWorker.Load();
                InputData = InputWorker.Data;

                OutputWorker = new HRV_DFA_Data_Worker(Params.AnalysisName);
                OutputData = new HRV_DFA_Data();

                _currentChannelIndex = 0;
                _rPeaksProcessed = 0;
                NumberOfChannels = InputData.RRInterval.Count;
                _currentRpeaksLength = InputData.RRInterval[_currentChannelIndex].Item2.Count;

               // _currentdfaNumberN = Tuple<string, Vector<double>, Vector<double>>();
                //_currentdfaValueFn = Vector<double>.Build.Dense(_currentRpeaksLength);
               // _currentparamAlpha = Vector<double>.Build.Dense(_currentRpeaksLength);
            }
        }

        public void ProcessData()
        {
            if (Runnable()) processData();
            else _ended = true;
        }

        public double Progress()
        {
            return 100.0 * ((double)_currentChannelIndex / (double)NumberOfChannels + (1.0 / NumberOfChannels) * ((double)_rPeaksProcessed / (double)_currentRpeaksLength));
        }

        public bool Runnable()
        {
            return Params != null;
        }
        private void processData()
        {
            int channel = _currentChannelIndex;
            int startIndex = _rPeaksProcessed;
            int step = 10000;

            if (channel < NumberOfChannels)
            {
                Console.WriteLine("Len: " +_currentRpeaksLength);
                if (_currentRpeaksLength > 20 && _currentRpeaksLength < 1000)
                {
                    this.boxVal = 100;
                    this.startValue = 10;
                    this.stepVal = 10;
                    this.longCorrelations = false;
                }
                if (_currentRpeaksLength > 1000)
                {
                    this.boxVal = 1000;
                    this.startValue = 50;
                    this.stepVal = 100;
                    this.longCorrelations = true;
                }
                if (_currentRpeaksLength < 20)
                {
                    Console.WriteLine("Number of R - Peaks is too short");
                    _aborted = true;
                }
              
                if (startIndex + step > _currentRpeaksLength && _aborted != true)
                {

                        HRV_DFA_Analysis(InputData.RRInterval[_currentChannelIndex].Item2, stepVal, boxVal);
                        Tuple<string, Vector<double>, Vector<double>> numberN = new Tuple<string, Vector<double>, Vector<double>>(InputData.RRInterval[_currentChannelIndex].Item1, veclogn1, veclogn2);
                        Tuple<string, Vector<double>, Vector<double>> fnValue = new Tuple<string, Vector<double>, Vector<double>>(InputData.RRInterval[_currentChannelIndex].Item1, veclogFn1, veclogFn2);
                        Tuple<string, Vector<double>, Vector<double>> pAlpha = new Tuple<string, Vector<double>, Vector<double>>(InputData.RRInterval[_currentChannelIndex].Item1, vecparam1, vecparam2);
                        OutputData.DfaNumberN.Add(numberN);
                        OutputData.DfaValueFn.Add(fnValue);
                        OutputData.ParamAlpha.Add(pAlpha);

                   
                    _currentChannelIndex++;

                    if (_currentChannelIndex < NumberOfChannels)
                    {
                        _rPeaksProcessed = 0;

                        _currentRpeaksLength = InputData.RRInterval[_currentChannelIndex].Item2.Count;
                        _currentVector = Vector<double>.Build.Dense(_currentRpeaksLength);
                    }
                }
                else
                {
                    if (_aborted != true)
                    {
                        HRV_DFA_Analysis(InputData.RRInterval[_currentChannelIndex].Item2, stepVal, boxVal);
                        _currentVector = InputData.RRInterval[_currentChannelIndex].Item2.SubVector(0, stepVal);
                        _rPeaksProcessed = startIndex + stepVal;
                    }
                    else _ended = true;

                }
            }
            else
            {
                Console.WriteLine("Here");
                OutputWorker.Save(OutputData);
                _ended = true;
            }

        }
//
        public bool Aborted
        { get{ return _aborted; }set {_aborted = value;} }

        public int CurrentChannelIndex
        { get {return _currentChannelIndex;} set{ _currentChannelIndex = value;}}

        public int CurrentRpeaksLength
        { get{return _currentRpeaksLength;} set{ _currentRpeaksLength = value;}}

        public int RPeaksProcessed
        { get{return _rPeaksProcessed; } set { _rPeaksProcessed = value; }}

        public int NumberOfChannels
        {get{ return _numberOfChannels;}set {_numberOfChannels = value;} }

        public HRV_DFA_Data_Worker OutputWorker
        { get {return _outputWorker;}set { _outputWorker = value;}}

        public HRV_DFA_Data OutputData
        {get{ return _outputData;}set {_outputData = value;}}
        
        public HRV_DFA_Params Params
        {get {return _params;} set{_params = value;}}

        public R_Peaks_Data_Worker InputWorker
        {get {return _inputWorker; }set { _inputWorker = value;}}

        public R_Peaks_Data InputData
        { get { return _inputData; } set{ _inputData = value;} }

        public static void Main()
        {
            HRV_DFA_Params param = new HRV_DFA_Params("TestAnalysis100");
            HRV_DFA testModule = new HRV_DFA();

            testModule.Init(param);
            while (true)
            {
                //Console.WriteLine("Press key to continue.");
                //Console.Read();
                if (testModule.Ended()) break;
                Console.WriteLine(testModule.Progress());
                testModule.ProcessData();
            }
            //Console.Read();
        }
    }
     
}
