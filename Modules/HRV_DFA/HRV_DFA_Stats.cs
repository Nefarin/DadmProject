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
        private Basic_New_Data_Worker _basicWorker;
        private R_Peaks_New_Data_Worker _rworker;
        private HRV_DFA_New_Data_Worker _worker;
        private State _currentState;
        private int _currentChannelIndex;
        private string _currentName;
        private string[] _leads;

        public Basic_New_Data_Worker BasicWorker
        {
            get
            {
                return _basicWorker;
            }

            set
            {
                _basicWorker = value;
            }
        }

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
            _rworker = new R_Peaks_New_Data_Worker(_analysisName);
            _worker = new HRV_DFA_New_Data_Worker(_analysisName);
            _leads = _basicWorker.LoadLeads().ToArray();

            _currentState = State.START_CHANNEL;
            _currentChannelIndex = 0;
        }

        public void ProcessStats()
        {
            switch (_currentState)
            {
                case (State.START_CHANNEL):
                    _currentName = _leads[0];
                    _currentState = State.CALCULATE;
                    break;

                case (State.CALCULATE):
                    int sampl = (int)_worker.getNumberOfSamples(HRV_DFA_Signals.Fluctuations, _currentName);
                    Tuple<Vector<double>, Vector<double>> currentFn = _worker.LoadSignal(HRV_DFA_Signals.Fluctuations,_currentName,0, sampl);
                    double meanF = currentFn.Item2.Sum() / currentFn.Item2.Count;
                    _strToStr.Add(_currentName + " mean value: ", meanF.ToString());
                    _strToObj.Add(_currentName + " mean value: ", meanF);
                    double std = currentFn.Item2.StandardDeviation();
                    _strToStr.Add(_currentName + " std value: ", std.ToString());
                    _strToObj.Add(_currentName + " std value: ", std);

                    Tuple<Vector<double>, Vector<double>> currentAlpha = _worker.LoadSignal(HRV_DFA_Signals.ParamAlpha, _currentName, 0,(int)_worker.getNumberOfSamples(HRV_DFA_Signals.ParamAlpha, _currentName));
                    _strToStr.Add(_currentName + " alpha value: ", currentAlpha.Item1[0].ToString());
                    _strToStr.Add(_currentName + " alpha value: ", currentAlpha.Item2[0].ToString());

                    _currentState = State.END;
                    break;
                    

                case (State.END):
                    _ended = true;
                    break;
            }
        }

        
        public static void Main(String[] args)
        {
            HRV_DFA_Stats stats = new HRV_DFA_Stats();
            stats.Init("a100dat");


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
