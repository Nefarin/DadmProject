using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.IO;
using EKG_Project.Modules.R_Peaks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.HRV1
{
    public class HRV1 : IModule
    {
        private enum _algoStates{ INIT, TIMEPARAMS, FREQPARAMS, END };
        private _algoStates state;

        private bool _ended;
        private bool _aborted;
        public bool Aborted;

        public IO.R_Peaks_New_Data_Worker InputWorker;
        public IO.HRV1_New_Data_Worker OutputWorker;
        private IO.Basic_New_Data_Worker basicWorker;

        //public R_Peaks_Data InputData;
        public HRV1_Data OutputData;
        public HRV1_Params Params;
        private HRV1_Alg algo;

        private string[] leads;
        string lead;


        public void Abort()
        {
            Aborted = true;
            _ended = true;
        }

        public bool IsAborted()
        {
            return Aborted;
        }

        public bool Ended()
        {
            return _ended;
        }

        public void Init(ModuleParams parameters)
        {
            Params = parameters as HRV1_Params;
            OutputData = new HRV1_Data();
            Aborted = false;
            if (!Runnable()) _ended = true;
            else
            {
                _ended = false;
                InputWorker = new R_Peaks_New_Data_Worker(Params.AnalysisName);
                OutputWorker = new HRV1_New_Data_Worker(Params.AnalysisName);
                basicWorker = new IO.Basic_New_Data_Worker(Params.AnalysisName);
                OutputData = new HRV1_Data();
                state = _algoStates.INIT;
            }
        }

        public void ProcessData() {
            if (Runnable()) this.processData();
            else _ended = true;
        }

        private void processData()
        {
            switch (state)
            {
                case _algoStates.INIT:
                    {
                        // Init
                        leads = basicWorker.LoadLeads().ToArray();
                        lead = leads[0];
                        int startindex = 0;
                        uint peaksLength = InputWorker.getNumberOfSamples(IO.R_Peaks_Attributes.RPeaks, lead);
                        uint intervalsLength = InputWorker.getNumberOfSamples(IO.R_Peaks_Attributes.RRInterval, lead);

                        var instants = InputWorker.LoadSignal(IO.R_Peaks_Attributes.RPeaks, lead, startindex, (int)peaksLength - 1);
                        var intervals = InputWorker.LoadSignal(IO.R_Peaks_Attributes.RRInterval, lead, startindex, (int)intervalsLength - 1);

                        algo = new HRV1_Alg();

                        algo.rInstants = instants;
                        algo.rrIntervals = intervals;
                        state = _algoStates.TIMEPARAMS;
                        break;
                    }

                case _algoStates.TIMEPARAMS:
                    {
                        // timebased 
                        algo.CalculateTimeBased();
                        state = _algoStates.FREQPARAMS;
                        break;
                    }

                case _algoStates.FREQPARAMS:
                    {
                        // freqbased
                        algo.CalculateFreqBased();
                        state = _algoStates.END;
                        break;
                    }
                case _algoStates.END:
                    {
                        // finish
                        List<Tuple<string, double>> tparams = algo.TimeParams;
                        List<Tuple<string, double>> fparams = algo.FreqParams;
                        List<Tuple<string, Vector<double>>> psd = algo.PowerSpectrum;

                        OutputData.TimeBasedParams = tparams;
                        OutputData.FreqBasedParams = fparams;
                        OutputData.PowerSpectrum = psd;

                        OutputWorker.SaveSignal(HRV1_Signal.FreqVector, lead, false, psd[0].Item2);
                        OutputWorker.SaveSignal(HRV1_Signal.PSD, lead, false, psd[1].Item2);

                        OutputWorker.SaveAttribute(HRV1_Attributes.AVNN, lead, tparams[0].Item2);
                        OutputWorker.SaveAttribute(HRV1_Attributes.SDNN, lead, tparams[1].Item2);
                        OutputWorker.SaveAttribute(HRV1_Attributes.RMSSD, lead, tparams[2].Item2);
                        OutputWorker.SaveAttribute(HRV1_Attributes.pNN50, lead, tparams[3].Item2);

                        OutputWorker.SaveAttribute(HRV1_Attributes.TP, lead, fparams[0].Item2);
                        OutputWorker.SaveAttribute(HRV1_Attributes.HF, lead, fparams[1].Item2);
                        OutputWorker.SaveAttribute(HRV1_Attributes.LF, lead, fparams[2].Item2);
                        OutputWorker.SaveAttribute(HRV1_Attributes.VLF, lead, fparams[3].Item2);
                        OutputWorker.SaveAttribute(HRV1_Attributes.LFHF, lead, fparams[4].Item2);

                        _ended = true;
                        break;
                    }
                default: { break; }
            }
        }


        public double Progress()
        {
            return 100 * ((int)state) / 4;
        }

        public bool Runnable()
        {
            return Params != null;
        }

        public static void Main()
        {
            //HRV1.AlgoTest();


            var param = new HRV1_Params("Analysis3");
            //param = null;
            HRV1 Hrv1 = new HRV1();
            Hrv1.Init(param);
            while (true)
            {
                //Console.WriteLine("Press key to continue.");
                //Console.Read();
                if (Hrv1.Ended()) break;
                Console.WriteLine(Hrv1.Progress());
                Hrv1.ProcessData();
            }
        }
    }
}
