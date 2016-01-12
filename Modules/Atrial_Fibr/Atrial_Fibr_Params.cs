using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.Atrial_Fibr
{
    public enum Detect_Method { STATISTIC, POINCARE };
    public class Atrial_Fibr_Params : ModuleParams
    {
        private Detect_Method _method;
        private string _analysisName;

        public Detect_Method Method
        {
            get { return _method; }
            set { _method = value; }
        }
        public string AnalysisName
        {
            get { return _analysisName; }
            set { _analysisName = value; }
        }
        public Atrial_Fibr_Params(Detect_Method method, string analysisName)
        {
            this.Method = method;
            this.AnalysisName = analysisName;
        }

        public Atrial_Fibr_Params()
        {
            this.Method = Detect_Method.STATISTIC;
            this.AnalysisName = "";
        }

        public void CopyParametersFrom(Atrial_Fibr_Params parameters)
        {
            this.Method = parameters.Method;
            this.AnalysisName = parameters.AnalysisName;

        }

    }
}
