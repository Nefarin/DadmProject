using System;
using System.Collections.Generic;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.HRV2
{
    public class HRV2_Stats : IModuleStats
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
        private R_Peaks_New_Data_Worker _R_PeaksWorker;
        private HRV2_New_Data_Worker _worker;
        private string[] _leads;
        private Vector<double> _currentVector;

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
            _worker = new HRV2_New_Data_Worker(_analysisName);
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

                    // Our statisics include only Tinn, Tiangle Index, SD1 and SD2
                    Vector<double> currentData = _basicWorker.LoadSignal(_currentName, 0, (int)_basicWorker.getNumberOfSamples(_currentName));
                    _currentVector = _R_PeaksWorker.LoadSignal(R_Peaks_Attributes.RRInterval, _currentName, 0, (int)_basicWorker.getNumberOfSamples(_currentName));
                    HRV2_Alg _alg = new HRV2_Alg(_currentVector);

                    double Tinn = _alg.Tinn;
                    double TriangleIndex = _alg.TriangleIndex;
                    double SD1 = _alg.SD1();
                    double SD2 = _alg.SD2();
                    _strToStr.Add(_currentName + "Tinn: ", Tinn.ToString());
                    _strToObj.Add(_currentName + "Tinn: ", Tinn);

                    _strToStr.Add(_currentName + "Triangle index: ", TriangleIndex.ToString());
                    _strToObj.Add(_currentName + "Triangle index: ", TriangleIndex);

                    _strToStr.Add(_currentName + "SD1: ", SD1.ToString());
                    _strToObj.Add(_currentName + "SD1: ", SD1);

                    _strToStr.Add(_currentName + "SD2: ", SD2.ToString());
                    _strToObj.Add(_currentName + "SD2: ", SD2);

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
            HRV2_Stats stats = new HRV2_Stats();
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
