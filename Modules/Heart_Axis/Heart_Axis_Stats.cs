using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra; //?

namespace EKG_Project.Modules.Heart_Axis
{
    public class Heart_Axis_Stats : IModuleStats
    {

        private enum State { START_CHANNEL, CALCULATE, NEXT_CHANNEL, END };
        private bool _aborted;
        private bool _ended;
        private Dictionary<string, Object> _strToObj;
        private Dictionary<string, string> _strToStr;
        private string _analysisName;
        private Heart_Axis_Data _data;
        private State _currentState;
        //private string _currentName;


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

            Heart_Axis_Data_Worker worker = new Heart_Axis_Data_Worker(_analysisName);
            worker.Load();
            _data = worker.Data;
            _currentState = State.START_CHANNEL;

        }

        public void ProcessStats()
        {

            switch (_currentState)
            {
                case (State.START_CHANNEL):
                    //_currentName = _data.HeartAxis.Item1; //moduł nie zwraca żadnej nazwy
                    _currentState = State.CALCULATE;
                    break;
                case (State.CALCULATE):
                    double currentData = _data.HeartAxis; // czy może być double zamiast double<Vector>?

                    _currentState = State.NEXT_CHANNEL;
                    break;
                case (State.NEXT_CHANNEL):
                        _currentState = State.END; //moduł wykonuje obliczenia na jednym odprowadzeniu
                    break;
                case (State.END):
                    _ended = true;
                    break;
            }

        }


        public static void Main(String[] args)
        {
            Heart_Axis_Stats stats = new Heart_Axis_Stats();
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
