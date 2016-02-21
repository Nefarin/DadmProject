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
        private State _currentState;
        private int _currentChannelIndex;
        private int _currentIndex;
        private string _currentName;
        private Basic_New_Data_Worker _basicWorker;
        private Waves_New_Data_Worker _wavesWorker;
        private T_Wave_Alt_New_Data_Worker _worker;
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

            _basicWorker = new Basic_New_Data_Worker(_analysisName);
            _wavesWorker = new Waves_New_Data_Worker(_analysisName);
            _worker = new T_Wave_Alt_New_Data_Worker(_analysisName);
            _leads = _basicWorker.LoadLeads().ToArray();
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
                    _currentName = _leads[_currentChannelIndex];
                    _currentIndex = 0;
                    _currentState = State.CALCULATE;
                    break;
                case (State.CALCULATE):

                    List<int> waves = _wavesWorker.LoadSignal(Waves_Signal.TEnds, _currentName, _currentIndex, (int)_wavesWorker.getNumberOfSamples(Waves_Signal.TEnds, _currentName));
                    List<int> t_alt = new List<int>();
                    foreach (Tuple<int, int> alt in _worker.LoadAlternansDetectedList(_currentName, _currentIndex, (int)_worker.getNumberOfSamples(_currentName)))
                    {
                        t_alt.Add(alt.Item1);
                    }

                    _strToStr.Add(_currentName + " T Wave Alternans recognized: ", CountPercentOfRecognized(t_alt, waves).ToString() + " [%]");
                    _strToObj.Add(_currentName + " T Wave Alternans recognized: ", CountPercentOfRecognized(t_alt, waves));

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
            T_Wave_Alt_Stats stats = new T_Wave_Alt_Stats();
            stats.Init("Analysis 1");


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