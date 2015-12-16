using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Histogram.Data;
using Histogram.HelperClasses;
using System.Collections.ObjectModel;

namespace EKG_Project.Modules.HRV2
{
    public partial class HRV2 : IModule
    {
        class Program
        {
            static void Main(string[] args)
            {
                SampleDataSource data = new SampleDataSource(.1f, "nsr001.txt");
                ObservableCollection<Sample> samples = data.Samples;

                foreach (Sample s in samples)
                {
                    Console.WriteLine(s.ToString());
                }
                Console.ReadKey();
            }
        }
    }
}
