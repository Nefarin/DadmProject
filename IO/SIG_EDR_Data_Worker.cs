using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.SIG_EDR;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    public class SIG_EDR_Data_Worker
    {
        string directory;
        string analysisName;
        private SIG_EDR_Data _data;

        public SIG_EDR_Data Data
        {
            get
            {
                return _data;
            }

            set
            {
                _data = value;
            }
        }

        public SIG_EDR_Data_Worker()
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
            Data = null;
        }

        public SIG_EDR_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }
    }
}
