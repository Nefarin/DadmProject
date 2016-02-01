using System;
using System.Collections.Generic;
using System.Linq;
using EKG_Project.IO;
using EKG_Project.Modules.R_Peaks;
using System.Text;
using System.Threading.Tasks;


namespace EKG_Project.Modules.HRV2
{
    public class HRV2_Stats : IModuleStats
    {
        private enum State { START_CHANNEL, CALCULATE, NEXT_CHANNEL, END };
        private bool _aborted;
        private bool _ended;
        private Dictionary<string, Object> _strToObj;
        private Dictionary<string, string> _strToStr;
        private string _analysisName;
        private HRV2_Data _data;
        private State _currentState;
        private int _currentChannelIndex;
        private string _currentName;


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

            HRV2_Data_Worker worker = new HRV2_Data_Worker(_analysisName);
            worker.Load();
            _data = worker.Data;
            _currentState = State.START_CHANNEL;
            _currentChannelIndex = 0;
        }

        //a se tym razem to jako maszyne stanów zrobimy i pominiemy step, bo to sie powinno liczyc duzo szybciej niż moduły,
        // jedynie odprowadzenia podzielmy - wasze powinny byc jednak zdecydowanie bardziej skomplikowane
        public void ProcessStats()
        {
            //switch (_currentState)
            //{
            //    case (State.START_CHANNEL):
            //        _currentName = _data.Output[_currentChannelIndex].Item1;
            //        _currentState = State.CALCULATE;
            //        break;
            //    case (State.CALCULATE):
            //        Vector<double> currentData = _data.Output[_currentChannelIndex].Item2;
            //        double mean = currentData.Sum() / currentData.Count;
            //        _strToStr.Add(_currentName + " mean value: ", mean.ToString()); //generalnie to jakie statystyki wasz moduł powinien wyznacać zależy przede wszystkim od was
            //        _strToObj.Add(_currentName + " mean value: ", mean);            //klucz nie jest tak ważny, bo i tak można je wszystkie później wyciągnąć automatycznie
            //                                                                        //jeżeli coś może się liczyć dłużej to musicie podzielić również sygnał na części
            //                                                                        // zależnie od modułu powinny sie tu znaleźć średnie, wariancje, odchylenia, ilości dobrze/źle
            //                                                                        // wykrytych rzeczy, jakieś porównanie klasyfikacji etc. - możliwe, że niektóre moduły zostawią
            //                                                                        // tą funkcje po prostu pustą (chociaż zdecydowana większość powinna ją uzupełnić - narazie jedynymi
            //                                                                        // wyjątkami, które widze są Heart_Axis i ECG_Baseline
            //        _currentState = State.NEXT_CHANNEL;
            //        break;
            //    case (State.NEXT_CHANNEL):
            //        _currentChannelIndex++;
            //        if (_currentChannelIndex >= _data.Output.Count)
            //        {
            //            _currentState = State.END;
            //        }
            //        else _currentState = State.START_CHANNEL;
            //        break;
            //    case (State.END):
            //        _ended = true;
            //        break;
            //}
        }

        public static void Main(String[] args)
        {
            HRV2_Stats stats = new HRV2_Stats();
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
