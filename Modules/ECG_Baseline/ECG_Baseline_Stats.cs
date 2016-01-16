using System;
using System.Collections.Generic;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.ECG_Baseline
{
    public class ECG_Baseline_Stats : IModuleStats
    {

        private enum State { START_CHANNEL, CALCULATE, NEXT_CHANNEL, END };
        private bool _aborted;
        private bool _ended;
        private Dictionary<string, Object> _strToObj;
        private Dictionary<string, string> _strToStr;
        private string _analysisName;
        private ECG_Baseline_Data _data;
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

            ECG_Baseline_Data_Worker worker = new ECG_Baseline_Data_Worker(_analysisName);
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
                    _currentName = _data.SignalsFiltered[_currentChannelIndex].Item1;
                    _currentState = State.CALCULATE;
                    break;
                case (State.CALCULATE):
                    Vector<double> currentData = _data.SignalsFiltered[_currentChannelIndex].Item2;
                    //double mean = currentData.Sum() / currentData.Count;
                    //_strToStr.Add(_currentName + " mean value: ", mean.ToString()); 
                    //_strToObj.Add(_currentName + " mean value: ", mean);      
                         
                    //Tak jak napisał Marek - żadne statystyki nam nie przychodzą do głowy                                                                
                                                                                    
                                                                                    
                                                                                    
                                                                                    
                    _currentState = State.NEXT_CHANNEL;
                    break;
                case (State.NEXT_CHANNEL):
                    _currentChannelIndex++;
                    if (_currentChannelIndex >= _data.SignalsFiltered.Count)
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
            ECG_Baseline_Stats stats = new ECG_Baseline_Stats();
            stats.Init("Analysis6");


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
