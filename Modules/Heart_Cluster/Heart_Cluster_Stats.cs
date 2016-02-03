using System;
using System.Collections.Generic;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private string _currentName;

        private List<int> _classResults; //klasa.

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
            Heart_Cluster_Data_Worker worker = new Heart_Cluster_Data_Worker(_analysisName);
            //worker.Load();
            //_data = worker.Data;
            _currentState = State.START_CHANNEL;
            _currentChannelIndex = 0;
            _classResults = new List<int>();

        //private uint _totalNumberOfQrsComplex;
        //private uint _numberOfClass;
        //private double _percentOfNormalComplex;
        //private Qrs_Class _cluster;
    }

        // count percent of classes ?


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
