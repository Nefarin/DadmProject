using System;
using System.Collections.Generic;
using System.Linq;
using EKG_Project.IO;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.Waves
{
    public class Waves_Stats : IModuleStats
    {
        private enum State { START_CHANNEL, CALCULATE, NEXT_CHANNEL, END };
        private bool _aborted;
        private bool _ended;
        private Dictionary<string, Object> _strToObj;
        private Dictionary<string, string> _strToStr;
        private string _analysisName;
        private Waves_Data _data;
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

            Waves_Data_Worker worker = new Waves_Data_Worker(_analysisName);
            worker.Load();
            _data = worker.Data;
            _currentState = State.START_CHANNEL;
            _currentChannelIndex = 0;
        }
        int CountUnrecognized( List<int> listInp)
        {
            int unrecognized = 0;
            foreach( int element in listInp)
            {
                if (element == -1)
                    unrecognized++;
            }
            return unrecognized;
        }
        double CountPercentOfRecognized( List<int> listInp)
        {
            int listSize = listInp.Count;
            int unrecognized = CountUnrecognized(listInp);
            double result = (double)(listSize - unrecognized) / (double)listSize;
            result = Math.Round(result, 2);
            return result*100;
        }
        public void ProcessStats()
        {
            switch (_currentState)
            {
                case (State.START_CHANNEL):
                    _currentName = _data.QRSEnds[_currentChannelIndex].Item1;
                    _currentState = State.CALCULATE;
                    break;
                case (State.CALCULATE):
                    List<int> qrsOns = _data.QRSOnsets[_currentChannelIndex].Item2;
                    List<int> qrsEnds = _data.QRSEnds[_currentChannelIndex].Item2;

                    List<int> pOns = _data.POnsets[_currentChannelIndex].Item2;
                    List<int> pEnds = _data.PEnds[_currentChannelIndex].Item2;

                    List<int> tEnds = _data.TEnds[_currentChannelIndex].Item2;


                    _strToStr.Add(_currentName + " qrs onsets recognize pertent: ", CountPercentOfRecognized(qrsOns).ToString());
                    _strToStr.Add(_currentName + " qrs ends recognize pertent: ", CountPercentOfRecognized(qrsEnds).ToString());

                    _strToStr.Add(_currentName + " p onsets recognize pertent: ", CountPercentOfRecognized(pOns).ToString());
                    _strToStr.Add(_currentName + " p ends recognize pertent: ", CountPercentOfRecognized(pEnds).ToString());

                    _strToStr.Add(_currentName + " t ends recognize pertent: ", CountPercentOfRecognized(tEnds).ToString());
                    _currentState = State.NEXT_CHANNEL;
                    break;
                case (State.NEXT_CHANNEL):
                    _currentChannelIndex++;
                    if (_currentChannelIndex >= _data.QRSEnds.Count)
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
