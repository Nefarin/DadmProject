using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.Heart_Cluster
{
    public class Heart_Cluster_Stats : IModuleStats
    {
        private enum State { START_CHANNEL, CALCULATE, NEXT_CHANNEL, END };
        private bool _aborted;
        private bool _ended;
        private Dictionary<string, Object> _strToObj;
        private Dictionary<string, string> _strToStr;
        private string _analysisName;
        private Heart_Cluster_Data _data;
        private State _currentState;
        private int _currentChannelIndex;
        private int _currentIndex;
        private string _currentName;
        private Heart_Cluster_Data_Worker _worker;
        private string[] _leads;
        private Basic_New_Data_Worker _workerBasic;


        private List<Tuple<int, int, int, int>> _clusterizationList;
        private List<int> _clusterResults; //?

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

            //reading output data from module
            _worker = new Heart_Cluster_Data_Worker(_analysisName);
            _workerBasic = new Basic_New_Data_Worker(_analysisName);
            _leads = _workerBasic.LoadLeads().ToArray();

            _currentState = State.START_CHANNEL;
            _currentChannelIndex = 0;
            _clusterResults = new List<int>();
            _clusterizationList = new List<Tuple<int, int, int, int>>();
        }


        double CountClussPercent(List<int> classCounts)
        {
            int listSize = classCounts.Count;
            int numberOfC1 = 0;
            int numberOfC2 = 0;

            double result1;
            double result2;

            foreach (int element in classCounts)
            {
                if (element == 0) //klasa pierwsza
                    numberOfC1++;
                if (element == 1) //druga pierwsza
                    numberOfC2++;
            }
            result1 = (double) numberOfC1 / (double)listSize * 100;
            result1 = Math.Round(result1, 2);

            result2 = (double)numberOfC2 / (double)listSize * 100;
            result2 = Math.Round(result2, 2);
            return result1;
            return result2;

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
                    // Vector<double> currentData = _data.Output[_currentChannelIndex].Item2;
                    // foreach (var item in _output)
                    //{ classResults.Add(item.Item2); }

                    // procent klas ?

                    _currentState = State.END; //_currentState = State.NEXT_CHANNEL; ale ja dla jednego liczę. Zweryfikować!
                    break;

                case (State.END):
                    _ended = true;
                    break;
            }
        }
        



        public static void Main(String[] args)
        {
            Heart_Cluster_Stats stats = new Heart_Cluster_Stats();
            stats.Init("TestAnalysis1");

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
