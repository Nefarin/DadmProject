using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.ST_Segment
{
    public partial class ST_Segment : IModule
    {

        public partial class ST_Segment : IModule
        {
           

            private Basic_Data_Worker _inputWorker;
            private R_Peaks_Data_Worker _inputRpeaksWorker;
            private ST_Segment_Data_Worker _outputWorker;

            private ST_Segment_Data _outputData;
            private Basic_Data _inputData;
            private R_Peaks_Data _inputRpeaksData;

            private ST_Segment_Params _params;

            private List<double> _tJs;
            private List<double> _tSTs;
            private int _ConcaveCurves;
            private int _ConvexCurves;
            private int _IncreasingLines;
            private int _HorizontalLines;
            private int _DecreasingLines;



        {
            


           // throw new NotImplementedException();
        }

        public bool Ended()
        {
            throw new NotImplementedException();
        }

        public void Init(ModuleParams parameters)
        {
            InputWorkerRpeaks = new R_Peaks_Data_Worker(Params.AnalysisName); //


            InputWorkerRpeaks.Load(); //
            InputDataRpeaks = InputWorkerRpeaks.Data; //
            //throw new NotImplementedException();
        }

        public void ProcessData(int numberOfSamples)
        {
            throw new NotImplementedException();
        }

        public double Progress()
        {
            throw new NotImplementedException();
        }

        public bool Runnable()
        {
            throw new NotImplementedException();


        }
    }
}