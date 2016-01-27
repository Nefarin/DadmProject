using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EKG_Project.Modules;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.IO
{
    public interface IECGConverter
    {
        Vector<Double> getSignal(string lead, int startIndex, int length);
        string[] getLeads();
        void SaveResult();
        uint getFrequency();
        uint getNumberOfSamples(string lead);
        void ConvertFile(string path);

        
    }
}
