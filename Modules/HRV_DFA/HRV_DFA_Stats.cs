using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;


namespace EKG_Project.Modules.HRV_DFA
{
    public class HRV_DFA_Stats : IModuleStats
    {
        private enum State { START_CHANNEL, CALCULATE, NEXT_CHANNEL, END };
        private bool _aborted;
        private bool _ended;
        private Dictionary<string, Object> _strToObj;
        private Dictionary<string, string> _strToStr;
        private string _analysisName;
        private HRV_DFA_Data _data;
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

            HRV_DFA_Data_Worker worker = new HRV_DFA_Data_Worker(_analysisName);
            worker.Load();
            _data = worker.Data;
            _currentState = State.START_CHANNEL;
            _currentChannelIndex = 0;
        }

        public void ProcessStats()
        {
            switch (_currentState)
            {
                case (State.START_CHANNEL):
                    _currentName = _data.DfaValueFn[_currentChannelIndex].Item1;
                    _currentState = State.CALCULATE;
                    break;

                case (State.CALCULATE):
                    Vector<double> currentFn = _data.Fluctuations[_currentChannelIndex].Item2;
                    double meanF = currentFn.Sum() / currentFn.Count;
                    _strToStr.Add(_currentName + " mean value: ", meanF.ToString());
                    _strToObj.Add(_currentName + " mean value: ", meanF);
                    double std = currentFn.StandardDeviation();
                    _strToStr.Add(_currentName + " std value: ", std.ToString());
                    _strToObj.Add(_currentName + " std value: ", std);

                    double currentAlpha = _data.ParamAlpha[_currentChannelIndex].Item2[1];
                    _strToStr.Add(_currentName + " alpha value: ", currentAlpha.ToString());
                    _strToObj.Add(_currentName + " alpha value: ", currentAlpha);

                    _currentState = State.NEXT_CHANNEL;
                    break;

                case (State.NEXT_CHANNEL):
                    _currentChannelIndex++;
                    if (_currentChannelIndex >= _data.ParamAlpha.Count)
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
            HRV_DFA_Stats stats = new HRV_DFA_Stats();
            stats.Init("234");


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
