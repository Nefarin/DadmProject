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
        private Heart_Class_Data _data;
        private State _currentState;
        private int _currentChannelIndex;
        private string _currentName;
        private List<Tuple<int, int>> _output;
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

            Heart_Class_Data_Worker worker = new Heart_Class_Data_Worker(_analysisName);
            worker.Load();
            _data = worker.Data;
            _currentState = State.START_CHANNEL;
            _currentChannelIndex = 0;
            _output = _data.ClassificationResult;
            _classResults = new List<int>();

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

            result = (double) numberOfV/(double) listSize;
            result = Math.Round(result, 2);
            return result;

        }

        public void ProcessStats()
        {
            switch (_currentState)
            {
                case (State.START_CHANNEL):
                    //_currentName = _data.Output[_currentChannelIndex].Item1;
                    _currentName = "MLII/II:";
                    _currentState = State.CALCULATE;
                    break;
                case (State.CALCULATE):
                    foreach (var item in _output)
                    {
                        _classResults.Add(item.Item2);
                    }

                    _strToStr.Add(_currentName + " Percent of ventricular stimulation: ", CountPercentOfV(_classResults).ToString()); //generalnie to jakie statystyki wasz moduł powinien wyznacać zależy przede wszystkim od was
            
                    _currentState = State.END;
                    break;
                       // BRAK NEXT CHANNEL STATE bo u mnie jest wynik tylko z jednego kanału
                case (State.END):
                    _ended = true;
                    break;
            }


        }


        public static void Main(String[] args)
        {
            Heart_Class_Stats stats = new Heart_Class_Stats();
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
