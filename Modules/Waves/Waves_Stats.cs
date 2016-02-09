using System;
using System.Collections.Generic;
using System.Linq;
using EKG_Project.IO;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.Waves
{
    public class Waves_Stats : IModuleStats
    {
        // declarations of variables
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
        private Waves_New_Data_Worker _worker;
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

            // make workers from data
            _basicWorker = new Basic_New_Data_Worker(_analysisName);
            _worker = new Waves_New_Data_Worker(_analysisName);
            _leads = _basicWorker.LoadLeads().ToArray();
            _currentState = State.START_CHANNEL;
            _currentChannelIndex = 0;
        }

        #region
        /// <summary>
        /// This method counts amount of unrecognized characteristic points
        /// </summary>
        /// <param name="listInp"> List of all elements from current characteristic points</param>
        /// <returns> Amount of unrecognized points</returns>
        #endregion
        int CountUnrecognized( List<int> listInp)
        {
            int unrecognized = 0;
            foreach( int element in listInp)
            {
                if (element == -1)
                    unrecognized++; // if element is not recognized increment variable
            }
            return unrecognized;
        }

        #region
        /// <summary>
        /// This method counts percentage of recognized characteristic points
        /// </summary>
        /// <param name="listInp"> List of all elements from current characteristic points</param>
        /// <returns> Percentage of recognized points</returns>
        #endregion
        double CountPercentOfRecognized( List<int> listInp)
        {
            int listSize = listInp.Count;
            int unrecognized = CountUnrecognized(listInp);
            double result = (double)(listSize - unrecognized) / (double)listSize; // calculate recognized to all elements ratio
            result = Math.Round(result, 2);
            return result*100; // multiplicate in order to get percnetage
        }

        #region
        /// <summary>
        /// This method calculates stats and add them to stats output
        /// </summary>
        #endregion
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
                    // load calculated samples of characteristic ECG points
                    List<int> qrsOns = _worker.LoadSignal(Waves_Signal.QRSOnsets,_currentName,_currentIndex,(int)_worker.getNumberOfSamples(Waves_Signal.QRSOnsets,_currentName));
                    List<int> qrsEnds = _worker.LoadSignal(Waves_Signal.QRSEnds, _currentName, _currentIndex, (int)_worker.getNumberOfSamples(Waves_Signal.QRSEnds, _currentName));
                    List<int> pOns = _worker.LoadSignal(Waves_Signal.POnsets, _currentName, _currentIndex, (int)_worker.getNumberOfSamples(Waves_Signal.POnsets, _currentName));
                    List<int> pEnds = _worker.LoadSignal(Waves_Signal.PEnds, _currentName, _currentIndex, (int)_worker.getNumberOfSamples(Waves_Signal.PEnds, _currentName));
                    List<int> tEnds = _worker.LoadSignal(Waves_Signal.TEnds, _currentName, _currentIndex, (int)_worker.getNumberOfSamples(Waves_Signal.TEnds, _currentName));

                    // add recognize precentage of characteristic ECG points to stats output
                    _strToStr.Add(_currentName + " QRS Onsets recognized: ", CountPercentOfRecognized(qrsOns).ToString() + " [%]");
                    _strToObj.Add(_currentName + " QRS Onsets recognized: ", CountPercentOfRecognized(qrsOns));
                    _strToStr.Add(_currentName + " QRS Ends recognized: ", CountPercentOfRecognized(qrsEnds).ToString() + " [%]");
                    _strToObj.Add(_currentName + " QRS Ends recognized: ", CountPercentOfRecognized(qrsEnds));
                    _strToStr.Add(_currentName + " P Onsets recognized: ", CountPercentOfRecognized(pOns).ToString() + " [%]");
                    _strToObj.Add(_currentName + " P Onsets recognized: ", CountPercentOfRecognized(pOns));
                    _strToStr.Add(_currentName + " P Ends recognized: ", CountPercentOfRecognized(pEnds).ToString() + " [%]");
                    _strToObj.Add(_currentName + " P Ends recognized: ", CountPercentOfRecognized(pEnds));
                    _strToStr.Add(_currentName + " T Ends recognized: ", CountPercentOfRecognized(tEnds).ToString() + " [%]");
                    _strToObj.Add(_currentName + " T Ends recognized: ", CountPercentOfRecognized(tEnds));

                    _currentState = State.NEXT_CHANNEL;
                    break;
                case (State.NEXT_CHANNEL):
                    _currentChannelIndex++;
                    if (_currentChannelIndex >= _leads.Length)
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
            Waves_Stats stats = new Waves_Stats();
            stats.Init("Analysis 1");


            while (true)
            {
                if (stats.Ended()) break; // if stats ended then exit 
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
