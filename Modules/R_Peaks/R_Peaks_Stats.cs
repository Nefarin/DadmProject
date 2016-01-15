using System;
using System.Collections.Generic;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.R_Peaks
{
    public class R_Peaks_Stats : IModuleStats
    {
        private enum State { START_CHANNEL, CALCULATE, NEXT_CHANNEL, END };
        private bool _aborted;
        private bool _ended;
        private Dictionary<string, Object> _strToObj;
        private Dictionary<string, string> _strToStr;
        private string _analysisName;
        private R_Peaks_Data _data;
        private State _currentState;
        private int _currentChannelIndex;
        private string _currentName;


        public void Abort()
        {
            _aborted = true;
        }

        public bool Aborted()
        {
            return _aborted;
        }

        public bool Ended()
        {
            return _ended;
        }

        public Dictionary<string, object> GetStats()
        {
            if (_strToObj == null) throw new NullReferenceException();

            return _strToObj;
        }

        public Dictionary<string, string> GetStatsAsString()
        {
            if (_strToStr == null) throw new NullReferenceException();

            return _strToStr;
        }

        public void Init(string analysisName)
        {
            _analysisName = analysisName;
            _ended = false;
            _aborted = false;

            _strToObj = new Dictionary<string, object>();
            _strToStr = new Dictionary<string, string>();

            //reading output data from module
            R_Peaks_Data_Worker worker = new R_Peaks_Data_Worker(_analysisName);
            worker.Load();
            _data = worker.Data;
            _currentState = State.START_CHANNEL;
            _currentChannelIndex = 0;

            //reading annotations 
            //!!!! TO DO --> Tymczasowy sposób --> zmienic jesli ma to byc wczytywane automatycznie z IO  !!!!!
            TempInput.setInputFilePath(@"D:\biomed\DADM\C#\100a.txt");
        }

        public void ProcessStats()
        {
            switch (_currentState)
            {
                case (State.START_CHANNEL):
                    _currentName = _data.RPeaks[_currentChannelIndex].Item1;
                    _currentState = State.CALCULATE;
                    break;

                case (State.CALCULATE):
                    Vector<double> currentData = _data.RPeaks[_currentChannelIndex].Item2;
                    Vector<double> ann = TempInput.getSignal();
                    int tp = 0;     //true positives
                    int fp = 0;     //false positives
                    int fn = 0;     //false negatives
                    int total = 0;     //all proper peaks in signal
                    int indAnn = 0;
                    int indData = 0;

                    //validation
                    while (indAnn<ann.Count && indData < currentData.Count)
                    {
                        double d = Math.Abs(currentData[indData] - ann[indAnn]);
                        if (d > 10)     //approved mistake = 10 samples
                        {
                            if (ann[indAnn] > currentData[indData])
                            {
                                fp++;
                                indData++;
                                continue;
                            }
                            else
                            {
                                fn++;
                                indAnn++;
                                continue;
                            }
                        }
                        else
                        {
                            tp++;
                            indAnn++;
                            indData++;
                        }
                    }
                    total = tp + fn;

                    double sensitivity = Math.Round(tp*100.00 / (tp + fn),2);
                    double positivePredictive = Math.Round(tp *100.00/ (tp + fp),2);

                    //add to stats output
                    _strToStr.Add(_currentName + " Total: ", total.ToString());
                    _strToObj.Add(_currentName + " Total: ", total);
                    _strToStr.Add(_currentName + " True positives: ", tp.ToString());
                    _strToObj.Add(_currentName + " True positives: ", tp);
                    _strToStr.Add(_currentName + " False positives: ", fp.ToString());
                    _strToObj.Add(_currentName + " False positives: ", fp);
                    _strToStr.Add(_currentName + " False negatives: ", fn.ToString());
                    _strToObj.Add(_currentName + " False negatives: ", fn);
                    _strToStr.Add(_currentName + " Sensitivity: ", sensitivity.ToString());
                    _strToObj.Add(_currentName + " Sensitivity: ", sensitivity);
                    _strToStr.Add(_currentName + " Positive predictive: ", positivePredictive.ToString());
                    _strToObj.Add(_currentName + " Positive predictive: ", positivePredictive);

                    _currentState = State.NEXT_CHANNEL;
                    break;

                case (State.NEXT_CHANNEL):
                    _currentChannelIndex++;
                    if (_currentChannelIndex >= _data.RPeaks.Count)
                    {
                        _currentState = State.END;
                    }
                    else _currentState = State.START_CHANNEL;
                    break;
                case (State.END):
                    _ended = true;
                    break;
            }
        }

        public static void Main(String[] args)
        {
            R_Peaks_Stats stats = new R_Peaks_Stats();
            stats.Init("TestAnalysis100");


            while (true)
            {
                if (stats.Ended()) break;
                stats.ProcessStats();
            }

            foreach (var key in stats.GetStatsAsString().Keys)
            {
                Console.WriteLine(key + stats.GetStatsAsString()[key]);
            }
            Console.Read();

        }
    }
}
