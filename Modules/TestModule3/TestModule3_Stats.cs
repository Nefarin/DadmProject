using System;
using System.Collections.Generic;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.TestModule3
{
    public class TestModule3_Stats : IModuleStats
    {
        private enum State {START_CHANNEL, CALCULATE, NEXT_CHANNEL, END};
        private bool _aborted;
        private bool _ended;
        private Dictionary<string, Object> _strToObj;
        private Dictionary<string, string> _strToStr;
        private string _analysisName;
        private State _currentState;
        private int _currentChannelIndex;
        private int _currentIndex;
        private string _currentName;
        private Basic_New_Data_Worker _worker;
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

            _worker = new Basic_New_Data_Worker(_analysisName);
            _leads = _worker.LoadLeads().ToArray();
            _currentState = State.START_CHANNEL;
            _currentChannelIndex = 0;
    }

        public void ProcessStats()
        {
            switch(_currentState)
            {
                case (State.START_CHANNEL):
                    _currentName = _leads[_currentChannelIndex];
                    _currentIndex = 0;
                    _currentState = State.CALCULATE;
                    break;
                case (State.CALCULATE):
                    // Podstawowe załozenie - ECG_Baseline nie generuje statystyk (generuje puste) i zakladamy, ze sygnal EKG bedzie krotszy niz okolo 150 dni
                    // w takim przypadku statystyki dla pozostalych modulow bez problemu powinny sie zmiescic w pamieci przetwarzajac odprowadzenie po odprowadzeniu
                    // jezeli z jakiegos powodu sygnal mialby byc dluzszy niz te prawie pol roku - trzeba dorobic dodatkowe IO do statsow, co z racji na brak czasu - pomijamy
                    // Zalozenie jest o tyle sensowne, ze sygnal 150 dni przy obecnej predkosci dzialania modulow liczylby sie okolo 37.5 godziny..
                    Vector<double> currentData = _worker.LoadSignal(_currentName, 0, (int) _worker.getNumberOfSamples(_currentName));
                    double mean = currentData.Sum() / currentData.Count;
                    _strToStr.Add(_currentName + " mean value: ", mean.ToString());
                    _strToObj.Add(_currentName + " mean value: ", mean);
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
            TestModule3_Stats stats = new TestModule3_Stats();
            stats.Init("abc123");

            
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
