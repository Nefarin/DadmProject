using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.ST_Segment
{
    public partial class ST_Segment : IModule
    {
        public void Abort()
        {
            throw new NotImplementedException();
        }

        public bool Ended()
        {
            throw new NotImplementedException();
        }

        public void Init(ModuleParams parameters)
        {
            InputWorkerRpeaks = new R_Peaks_Data_Worker(Params.AnalysisName);


            InputWorkerRpeaks.Load();
            InputDataRpeaks = InputWorkerRpeaks.Data;
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