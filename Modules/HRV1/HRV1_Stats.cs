using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;

namespace EKG_Project.Modules.HRV1
{
    public class HRV1_Stats : IModuleStats
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
        private IO.R_Peaks_New_Data_Worker peaksWorker;
        private IO.Basic_New_Data_Worker basicWorker;
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

            basicWorker = new Basic_New_Data_Worker(_analysisName);
            peaksWorker = new R_Peaks_New_Data_Worker(_analysisName);

            _leads = basicWorker.LoadLeads().ToArray();
            _currentChannelIndex = 0;
        }

        public void ProcessStats()
        {
            _currentName = _leads[_currentChannelIndex];
            _currentIndex = 0;
            var InputWorker = peaksWorker;

            var lead = _currentName;
            int startindex = 0;
            uint peaksLength = InputWorker.getNumberOfSamples(IO.R_Peaks_Attributes.RPeaks, lead);
            uint intervalsLength = InputWorker.getNumberOfSamples(IO.R_Peaks_Attributes.RRInterval, lead);

            var instants = InputWorker.LoadSignal(IO.R_Peaks_Attributes.RPeaks, lead, startindex, (int)peaksLength - 1);
            var intervals = InputWorker.LoadSignal(IO.R_Peaks_Attributes.RRInterval, lead, startindex, (int)intervalsLength - 1);

            var algo = new HRV1_Alg();

            algo.rInstants = instants;
            algo.rrIntervals = intervals;

            algo.CalculateTimeBased();
            algo.CalculateFreqBased();

            var tparams = algo.TimeParams;
            var fparams = algo.FreqParams;

            for (int i = 0; i < tparams.Count; ++i)
            {
                _strToStr.Add(_currentName + tparams[i].Item1 + ": ", tparams[i].Item2.ToString());
                _strToObj.Add(_currentName + tparams[i].Item1 + ": ", tparams[i].Item2);
            }

            for (int i = 0; i < fparams.Count; ++i)
            {
                _strToStr.Add(_currentName + fparams[i].Item1 + ": ", fparams[i].Item2.ToString());
                _strToObj.Add(_currentName + fparams[i].Item1 + ": ", fparams[i].Item2);
            }

            _strToStr.Add(_currentName + "Number of peaks: ", peaksLength.ToString());
            _strToObj.Add(_currentName + "Number of peaks: ", peaksLength);

            _strToStr.Add(_currentName + "Number of intervals", intervalsLength.ToString());
            _strToObj.Add(_currentName + "Number of intervals", intervalsLength);

            _ended = true;
        }

        public static void Main(String[] args)
        {
            HRV1_Stats stats = new HRV1_Stats();
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
