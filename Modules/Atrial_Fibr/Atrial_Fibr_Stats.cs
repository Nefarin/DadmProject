using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using EKG_Project.Modules.R_Peaks;
using MathNet.Numerics.LinearAlgebra;

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
        private Atrial_Fibr_New_Data_Worker _worker;
        private Basic_New_Data_Worker _basicWorker;
        private State _currentState;
        private int _currentChannelIndex;
        private string _currentName;
        private string[] _leads;
        

        #region Documentation
        /// <summary>
        /// TO DO
        /// </summary>
        #endregion
        public void Abort()
        {
            _aborted = true;
        }

        #region Documentation
        /// <summary>
        /// TO DO
        /// </summary>
        #endregion
        public bool Aborted()
        {
            return _aborted;
        }

        #region Documentation
        /// <summary>
        /// TO DO
        /// </summary>
        #endregion
        public bool Ended()
        {
            return _ended;
        }

        #region Documentation
        /// <summary>
        /// TO DO
        /// </summary>
        #endregion
        public Dictionary<string, object> GetStats()
        {
            if (_strToObj == null) throw new NullReferenceException();

            return _strToObj;
        }

        #region Documentation
        /// <summary>
        /// TO DO
        /// </summary>
        #endregion
        public Dictionary<string, string> GetStatsAsString()
        {
            if (_strToStr == null) throw new NullReferenceException();

            return _strToStr;
        }

        #region Documentation
        /// <summary>
        /// TO DO
        /// </summary>
        #endregion
        public void Init(string analysisName)
        {
            _analysisName = analysisName;
            _ended = false;
            _aborted = false;

            _strToObj = new Dictionary<string, object>();
            _strToStr = new Dictionary<string, string>();

            _worker = new Atrial_Fibr_New_Data_Worker(_analysisName);
            _basicWorker = new Basic_New_Data_Worker(_analysisName);
            _leads = _basicWorker.LoadLeads().ToArray();
            _currentState = State.START_CHANNEL;
            _currentChannelIndex = 0;
        }

        #region Documentation
        /// <summary>
        /// TO DO
        /// </summary>
        #endregion
        public void ProcessStats()
        {
            switch (_currentState)
            {
                case (State.START_CHANNEL):
                    _currentName = _leads[_currentChannelIndex];
                    _currentState = State.CALCULATE;
                    break;
                case (State.CALCULATE):
                    Tuple<bool, Vector<double>, string, string> _data = _worker.LoadAfDetection(_currentName, 0, 1);
                    string currentDetected= _data.Item3;
                    string currentDescription= _data.Item4;
                    _strToStr.Add(_currentName, " " + currentDetected + " " +currentDescription);
                    _strToObj.Add(_currentName, " " + currentDetected + " " + currentDescription);                                                         
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
            Atrial_Fibr_Stats stats = new Atrial_Fibr_Stats();
            stats.Init("analiza2");


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
