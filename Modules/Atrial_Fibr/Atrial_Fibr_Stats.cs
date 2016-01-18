using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using EKG_Project.Modules.R_Peaks;

namespace EKG_Project.Modules.Atrial_Fibr
{
    public class Atrial_Fibr_Stats : IModuleStats
    {
        private enum State { START_CHANNEL, CALCULATE, NEXT_CHANNEL, END };
        private bool _aborted;
        private bool _ended;
        private Dictionary<string, Object> _strToObj;
        private Dictionary<string, string> _strToStr;
        private string _analysisName;
        private Atrial_Fibr_Data _data;
        private R_Peaks_Data _datarr;
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

            R_Peaks_Data_Worker workerrr = new R_Peaks_Data_Worker(_analysisName);
            workerrr.Load();
            _datarr = workerrr.Data;
            Atrial_Fibr_Data_Worker worker = new Atrial_Fibr_Data_Worker(_analysisName);
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
                    _currentName = _datarr.RPeaks[_currentChannelIndex].Item1;
                    _currentState = State.CALCULATE;
                    break;
                case (State.CALCULATE):
                    string currentDetected= _data.AfDetection[_currentChannelIndex].Item3;
                    string currentDescription= _data.AfDetection[_currentChannelIndex].Item4;
                    _strToStr.Add(_currentName, " " + currentDetected + " " +currentDescription);
                    _strToObj.Add(_currentName, " " + currentDetected + " " + currentDescription);                                                         
                    _currentState = State.NEXT_CHANNEL;
                    break;
                case (State.NEXT_CHANNEL):
                    _currentChannelIndex++;
                    if (_currentChannelIndex >= _data.AfDetection.Count)
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
        //public static void Main(String[] args)
        //{
        //    Atrial_Fibr_Stats stats = new Atrial_Fibr_Stats();
        //    stats.Init("Test_analisys_111");


        //    while (true)
        //    {
        //        if (stats.Ended()) break;
        //        stats.ProcessStats();
        //    }

        //    foreach (var key in stats.GetStatsAsString().Keys)
        //    {
        //        Console.WriteLine(key + stats.GetStatsAsString()[key]);
        //    }
        //    Console.Read();

        //}
    }
}
