using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EKG_Project.IO;
using System.Threading.Tasks;
using EKG_Project.Modules.Waves;
using EKG_Project.Modules.T_Wave_Alt;

namespace EKG_Project.Modules.T_Wave_Alt
{
    public class T_Wave_Alt_Stats : IModuleStats
    {

        private enum State { START_CHANNEL, CALCULATE, NEXT_CHANNEL, END };
        private bool _aborted;
        private bool _ended;
        private Dictionary<string, Object> _strToObj;
        private Dictionary<string, string> _strToStr;
        private string _analysisName;
        private T_Wave_Alt_Data _data;
        private Waves_Data _dataWaves;
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

            T_Wave_Alt_Data_Worker worker = new T_Wave_Alt_Data_Worker(_analysisName);
            Waves_Data_Worker workerWaves = new Waves_Data_Worker(_analysisName);
            worker.Load();
            workerWaves.Load();
            _data = worker.Data;
            _dataWaves = workerWaves.Data;
            _currentState = State.START_CHANNEL;
            _currentChannelIndex = 0;
        }

        double CountPercentOfRecognized(List<int> listInp, List<int> listWaves)
        {
            int listSize = listInp.Count;
            int listSizeWaves = listWaves.Count;
            double result = (double)(listSize) / (double)listSizeWaves;
            result = Math.Round(result, 2);
            return result * 100;
        }

        public void ProcessStats()
        {
            switch (_currentState)
            {
                case (State.START_CHANNEL):
                    // _currentName = _data.AlternansDetectedList[_currentChannelIndex].Item1;
                    _currentState = State.CALCULATE;
                    break;
                case (State.CALCULATE):

                    List<int> waves = new List<int>();
                    List<int> t_alt = new List<int>();
                    foreach (Tuple<int, int> alt in _data.AlternansDetectedList)
                    {
                        t_alt.Add(alt.Item1);
                    }

                    _strToStr.Add(_currentName + " t wave alternans recognize percentage: ", CountPercentOfRecognized(t_alt, waves).ToString());

                    _currentState = State.NEXT_CHANNEL;
                    break;
                case (State.NEXT_CHANNEL):
                    _currentChannelIndex++;
                    if (_currentChannelIndex >= _data.AlternansDetectedList.Count)
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
    }
}