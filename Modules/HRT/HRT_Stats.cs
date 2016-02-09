using System;
using System.Collections.Generic;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.HRT
{
    public class HRT_Stats : IModuleStats
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
        private Basic_New_Data_Worker _basicWorker;
        private R_Peaks_New_Data_Worker _rPeaksWorker;
        private Heart_Class_New_Data_Worker _HeartClassWorker;
        private HRT_New_Data_Worker _worker;
        private string[] _leads;
        private HRT_Alg _alg;



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
            _basicWorker = new Basic_New_Data_Worker(_analysisName);
            _rPeaksWorker = new R_Peaks_New_Data_Worker(_analysisName);
            _HeartClassWorker = new Heart_Class_New_Data_Worker(_analysisName);

            _worker = new HRT_New_Data_Worker(_analysisName);
            _leads = _basicWorker.LoadLeads().ToArray();
            _currentState = State.START_CHANNEL;
            _currentChannelIndex = 0;

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
                    HRT_New_Data_Worker hWD = new HRT_New_Data_Worker(_analysisName);

                    if (hWD.LoadVPC(_currentName) != Modules.HRT.HRT.VPC.LETS_PLOT)
                    {
                        _currentState = State.NEXT_CHANNEL;
                        break;
                    }

                    List<double> currentTurbulenceOnset = _worker.LoadTurbulenceOnsetPDF(_currentName);
                    List<double> currentTurbulenceSlope = _worker.LoadTurbulenceSlopePDF(_currentName);
                    double meanTO = Mean(currentTurbulenceOnset);
                    double meanTS = Mean(currentTurbulenceSlope);

                    int[] statistics = _worker.LoadStatisticsClassNumbersPDF(_currentName);
                    double VPCvsAllQRS = 100 * ((double)statistics[2] / (double)statistics[0]);
                    double VPCvsVentricular = 100 * ((double)statistics[2] / (double)statistics[1]);

                    VPCvsAllQRS = Math.Round(VPCvsAllQRS, 3);
                    VPCvsVentricular = Math.Round(VPCvsVentricular, 3);

                    //add to stats output
                    _strToStr.Add(_currentName + " Mean Turbulence Onset: ", meanTO.ToString());
                    _strToObj.Add(_currentName + " Mean Turbulence Onset: ", meanTO);
                    _strToStr.Add(_currentName + " Mean Turbulence Slope: ", meanTS.ToString());
                    _strToObj.Add(_currentName + " Mean Turbulence Slope ", meanTS);
                    _strToStr.Add(_currentName + " Ratio between VPC and all QRS complexes detected [%]: ", VPCvsAllQRS.ToString());
                    _strToObj.Add(_currentName + " Ratio between VPC and all QRS complexes detected [%]:  ", VPCvsAllQRS);
                    _strToStr.Add(_currentName + " Ratio between VPC and all Ventricular complexes detected [%]: ", VPCvsAllQRS.ToString());
                    _strToObj.Add(_currentName + " Ratio between VPC and all Ventricular complexes detected [%]:  ", VPCvsAllQRS);
                    //_strToStr.Add(_currentName + " Turbulence Onset ", currentTurbulenceOnset.ToString());
                    _strToObj.Add(_currentName + " Turbulence Onset ", currentTurbulenceOnset);
                    //_strToStr.Add(_currentName + " Turbulence Slope ", currentTurbulenceSlope.ToString());
                    _strToObj.Add(_currentName + " Turbulence Slope ", currentTurbulenceSlope);
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
            HRT_Stats stats = new HRT_Stats();
            stats.Init("AA");


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

        double Mean(List<double> wektor)
        {

            double sumTO = 0;
            foreach (double licznik in wektor)
            {
                sumTO += licznik;
            }
            double meanTO = 0;
            return meanTO = Math.Round(sumTO / wektor.Count, 2);
        }
    }
}