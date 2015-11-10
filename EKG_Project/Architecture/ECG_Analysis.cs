using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace EKG_Project.Architecture
{
    public class ECG_Analysis
    {
        private ECG_Communication communication;
        public ECG_Analysis(ECG_Communication communication)
        {
            this.communication = communication;
        }
        public void run()
        {
            while (true)
            {

            }
        }

    }
}
