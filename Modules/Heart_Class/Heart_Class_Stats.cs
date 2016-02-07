using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.Heart_Class
{
    public class Heart_Class_Stats : IModuleStats
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
        private Heart_Class_New_Data_Worker _worker;
        private string[] _leads;
        private Basic_New_Data_Worker _workerBasic;
        private List<Tuple<int, int>> _classificationList;
        private List<int> _classResults;

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

            _worker = new Heart_Class_New_Data_Worker(_analysisName);
            _workerBasic = new Basic_New_Data_Worker(_analysisName);
            _leads = _workerBasic.LoadLeads().ToArray();
            
            _currentState = State.START_CHANNEL;
            _currentChannelIndex = 0;
            //_output = _data.ClassificationResult;
            _classResults = new List<int>();
            _classificationList = new List<Tuple<int, int>>();

        }

        double CountPercentOfV(List<int> listInp)
        {
            int listSize = listInp.Count;
            int numberOfV = 0;
            double result;
     
            foreach (int element in listInp)
            {
                if (element == 0)
                    numberOfV ++;
            }

            result = (double) numberOfV/(double) listSize*100;
            result = Math.Round(result, 2);
            return result;

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
                    _classificationList = _worker.LoadClassificationResult(_currentName, 0,
                        (int) _worker.getNumberOfSamples(_currentName));
                    foreach (var item in _classificationList)
                    {
                        _classResults.Add(item.Item2);
                    }

                    _strToStr.Add(_currentName + " Percent of ventricular stimulation: ", CountPercentOfV(_classResults).ToString()); //generalnie to jakie statystyki wasz moduł powinien wyznacać zależy przede wszystkim od was
            
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
            Heart_Class_Stats stats = new Heart_Class_Stats();
            stats.Init("Analysis233");


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
