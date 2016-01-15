using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Solvers;

namespace EKG_Project.Modules.Heart_Class
{
    public class Heart_Class_Params : ModuleParams
    {
        private string _analysisName;
        //HEART_CLASS Module input parameters - to podobno idzie kaj indziej
        //private Vector<double> _ecg;
        //private Vector<uint> _QRSonsets;
        //private Vector<uint> _QRSends;

        public Heart_Class_Params() : base()
        {
        }

        public Heart_Class_Params(string analysisName)
        {
            this.AnalysisName = analysisName;
        }
    }
}
