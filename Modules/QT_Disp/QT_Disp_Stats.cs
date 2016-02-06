using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;

namespace EKG_Project.Modules.QT_Disp
{
    public class QT_Disp_Stats : IModuleStats
    {
        private enum State { START_CHANNEL, CALCULATE, NEXT_CHANNEL, END };
        private bool _aborted;
        private bool _ended;
        private Dictionary<string, Object> _strToObj;
        private Dictionary<string, string> _strToStr;
        private string _analysisName;
        private QT_Disp_Data _data;
        private State _currentState;
        private int _currentChannelIndex;
        private string _currentName;
        private string[] _leads;
        Qt_Disp_New_Data_Worker worker;
        int _length;
        int _numberOfChannels;

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

            Basic_New_Data_Worker basicworker = new Basic_New_Data_Worker(_analysisName);
            worker = new Qt_Disp_New_Data_Worker(_analysisName);
            _leads = basicworker.LoadLeads().ToArray();
            _numberOfChannels = _leads.Count();          
            
            _currentState = State.START_CHANNEL;
            _currentChannelIndex = 0;
        }

        public void ProcessStats()
        {
            switch (_currentState)
            {
                case (State.START_CHANNEL):
                    _currentName = _leads[_currentChannelIndex];                  
                    _currentState = State.CALCULATE;
                    _length = (int)worker.getNumberOfSamples(Qt_Disp_Signal.T_End_Local, _currentName);
                    break;
                case (State.CALCULATE):
                    List<int> currentData = worker.LoadTEndLocal(_currentName, 0, _length);
                    double allT_Ends = currentData.Count;
                    double badT_Ends = 0;
                    double effectiveness = 0;
                    double mean = worker.LoadAttribute(Qt_Disp_Attributes.QT_mean, _currentName);
                    double std = worker.LoadAttribute(Qt_Disp_Attributes.QT_std, _currentName);
                    
                    foreach (int T_End in currentData)
                    {
                        if(T_End == -1)
                        {
                            badT_Ends += 1;
                        }
                    }
                    effectiveness = ((allT_Ends - badT_Ends) / allT_Ends)*100;

                    _strToStr.Add(_currentName + "_mean value: ", mean.ToString("#.00")); //generalnie to jakie statystyki wasz moduł powinien wyznacać zależy przede wszystkim od was
                    _strToObj.Add(_currentName + "_mean value: ", mean);
                    _strToStr.Add(_currentName + "_standard deviation: ", std.ToString("#.00"));
                    _strToObj.Add(_currentName + "_standard deviation: ", std);
                    _strToStr.Add(_currentName + "_effectiveness: " , effectiveness.ToString("#.00"));
                    _strToObj.Add(_currentName + "_effectiveness: ", effectiveness);                                                          
                    
                    _currentState = State.NEXT_CHANNEL;
                    break;
                case (State.NEXT_CHANNEL):
                    _currentChannelIndex++;
                    if (_currentChannelIndex >= _numberOfChannels)
                    {
                        _currentState = State.END;
                    }
                    else _currentState = State.START_CHANNEL;
                    break;
                case (State.END):
                    _ended = true;
                    break;
            }
        }
        public static void Main(String[] args)
        {
            QT_Disp_Stats stats = new QT_Disp_Stats();
            stats.Init("Analysis QT");


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