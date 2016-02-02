using System;
using System.Collections.Generic;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;
using System.Linq;

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
        private State _currentState;
        private int _currentChannelIndex;
        private int _currentIndex;
        private string _currentName;
        private Basic_New_Data_Worker _basicWorker;
        private R_Peaks_New_Data_Worker _worker;
        private string[] _leads;


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
            _basicWorker = new Basic_New_Data_Worker(_analysisName);
            _worker = new R_Peaks_New_Data_Worker(_analysisName);
            _leads = _basicWorker.LoadLeads().ToArray();
            _currentState = State.START_CHANNEL;
            _currentChannelIndex = 0;

        }

        public void ProcessStats()
        {
            switch (_currentState)
            {
                case (State.START_CHANNEL):
                    _currentName = _leads[_currentChannelIndex];
                    _currentIndex = 0;
                    _currentState = State.CALCULATE;
                    break;

                case (State.CALCULATE):
                    Vector<double> currentDataR = _worker.LoadSignal(R_Peaks_Attributes.RPeaks, _currentName, 0, (int)_worker.getNumberOfSamples(R_Peaks_Attributes.RPeaks, _currentName));
                    Vector<double> currentDataRR = _worker.LoadSignal(R_Peaks_Attributes.RRInterval, _currentName, 0, (int)_worker.getNumberOfSamples(R_Peaks_Attributes.RRInterval, _currentName));

                    int noOfPeaks = currentDataR.Count;     //number of detected r_peaks
                    double meanRR = Math.Round(currentDataRR.Sum() / currentDataRR.Count, 3);   
                    //mean RR interval in ms//double minRR = currentDataRR.AbsoluteMinimum(); //min distance between Rs
                    //double maxRR = currentDataRR.AbsoluteMaximum(); //max distance between Rs
                    int noOfOverdetectedPeaks = 0;  //number of peaks to close to eachother
                    int noOfUnderdetectedPeaks = 0; //number of peaks to far to each other
                    foreach(var rr in currentDataRR)
                    {
                        if (rr < 0.8 * meanRR)
                        {
                            noOfOverdetectedPeaks++;
                        }
                        if (rr > 1.2 * meanRR)
                        {
                            noOfUnderdetectedPeaks++;
                        }
                    }
                    
                    //add to stats output
                    _strToStr.Add(_currentName + " Number of detected R-peaks: ", noOfPeaks.ToString());
                    _strToObj.Add(_currentName + " Number of detected R-peaks: ", noOfPeaks);
                    _strToStr.Add(_currentName + " Mean RR interval [ms]: ", meanRR.ToString());
                    _strToObj.Add(_currentName + " Mean RR interval [ms]: ", meanRR);
                    _strToStr.Add(_currentName + " Number of possible overdetected R-peaks: ", noOfOverdetectedPeaks.ToString());
                    _strToObj.Add(_currentName + " Number of possible overdetected R-peaks: ", noOfOverdetectedPeaks);
                    _strToStr.Add(_currentName + " Number of possible missed R-peaks: ", noOfUnderdetectedPeaks.ToString());
                    _strToObj.Add(_currentName + " Number of possible missed R-peaks: ", noOfUnderdetectedPeaks);
                    /* _strToStr.Add(_currentName + " Minimum distance between R peaks [ms]: ", minRR.ToString());
                     _strToObj.Add(_currentName + " Minimum distance between R peaks [ms]: ", minRR);
                     _strToStr.Add(_currentName + " Maximum distance between R peaks [ms]: ", maxRR.ToString());
                     _strToObj.Add(_currentName + " Maximum distance between R peaks [ms]:", maxRR);*/

                    _currentState = State.NEXT_CHANNEL;
                    break;

                case (State.NEXT_CHANNEL):
                    _currentChannelIndex++;
                    if (_currentChannelIndex >= _leads.Length) _currentState = State.END;
                    else
                    {
                        _currentState = State.START_CHANNEL;
                    }
                    break;
                case (State.END):
                    _ended = true;
                    break;
            }
        }

        public static void Main(String[] args)
        {
            R_Peaks_Stats stats = new R_Peaks_Stats();
            stats.Init("AA");


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
