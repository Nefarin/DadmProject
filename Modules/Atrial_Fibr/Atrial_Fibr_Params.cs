﻿using System;
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
      
        public Detect_Method Method
        {
            get { return _method; }
            set { _method = value; }
        }

        public Atrial_Fibr_Params(Detect_Method method)
        {
            this.Method = method;
        }

        public Atrial_Fibr_Params()
        {
            this.Method = Detect_Method.STATISTIC;
        }

    }
}
