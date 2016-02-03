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
        private HRV1_Data _outputData;
        private R_Peaks_Data _inputData;
        public HRV1_Data OutputData;
        public R_Peaks_Data InputData;

        public HRV1_Params Params;

        private bool _ended;
        private bool _aborted;
        public bool Aborted;

        public IO.R_Peaks_New_Data_Worker InputWorker;
        public IO.HRV1_Data_Worker OutputWorker;

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

                OutputWorker = new HRV1_Data_Worker(Params.AnalysisName);
                OutputData = new HRV1_Data();
            }
        }

        public void ProcessData() { this.processData(); }

        private void processData()
        {
            if (Runnable())
            {
                // Init
                var basicWorker = new IO.Basic_New_Data_Worker(Params.AnalysisName);
                var leads = basicWorker.LoadLeads().ToArray();

                string lead = leads[0];
                int startindex = 0;
                uint peaksLength = InputWorker.getNumberOfSamples(IO.R_Peaks_Attributes.RPeaks, lead);
                uint intervalsLength = InputWorker.getNumberOfSamples(IO.R_Peaks_Attributes.RRInterval, lead);

                // pytanie co jezeli wektory peaks i intervals nie koresponduja?
                //tu babol jest - IndexOutOfRange 
                var instants = InputWorker.LoadSignal(IO.R_Peaks_Attributes.RPeaks, lead, startindex, (int)peaksLength-1);
                var intervals = InputWorker.LoadSignal(IO.R_Peaks_Attributes.RRInterval, lead, startindex, (int)intervalsLength-1);
                

                //heja

                var algo = new HRV1_Alg();

                algo.rInstants = instants;
                algo.rrIntervals = intervals;

                algo.CalculateTimeBased();
                algo.CalculateFreqBased();

                var tparams = algo.TimeParams;
                var fparams = algo.FreqParams;
                var psd = algo.PowerSpectrum;

                _ended = true;


                //OutputData.TimeBasedParams = tparams;

                //var tparams = Vector<double>.Build.Dense(new double[] { HF, LF, VLF, LFHF });
                //var fparams = Vector<double>.Build.Dense(new double[] { SDNN, RMSSD, SDSD, NN50, pNN50 });

                //OutputData.TimeBasedParams.Add(new Tuple<string, Vector<double>>(" ", tparams));
                //OutputData.FreqBasedParams.Add(new Tuple<string, Vector<double>>(" ", fparams));

                //OutputData.RInstants.Add(new Tuple<string, Vector<double>>(" ", instants));
                //OutputData.RRIntervals.Add(new Tuple<string, Vector<double>>(" ", intervals));
            }
            else _ended = true;
        }

        public double Progress()
        {
            return 50;
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
