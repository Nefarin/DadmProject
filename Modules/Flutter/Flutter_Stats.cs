using EKG_Project.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.Flutter
{
    public class Flutter_Stats : IModuleStats
    {
        private bool _aborted;
        private bool _ended;
        private Dictionary<string, Object> _strToObj;
        private Dictionary<string, string> _strToStr;
        private string _analysisName;
        private Flutter_Data _data;
        private Basic_Data _basicData;

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

            Flutter_Data_Worker worker = new Flutter_Data_Worker(_analysisName);
            worker.Load();
            _data = worker.Data;

            Basic_Data_Worker basicWorker = new Basic_Data_Worker(_analysisName);
            basicWorker.Load();
            _basicData = basicWorker.BasicData;
        }

        public void ProcessStats()
        {
            _strToStr.Add("Flutter: ", string.Empty);
            _strToObj.Add("Flutter: ", string.Empty);

            for(int i = 0 ; i < _data.FlutterAnnotations.Count; i++)
            {
                _strToStr.Add(string.Format("Wystąpienie {0}", i), string.Format(" od {0} do {1}", ((double)_data.FlutterAnnotations[i].Item1),  ((double)_data.FlutterAnnotations[i].Item2)));
                _strToObj.Add(string.Format("Wystąpienie {0}", i), _data.FlutterAnnotations[i]); 
            }

            _ended = true;
        }
    }
}
