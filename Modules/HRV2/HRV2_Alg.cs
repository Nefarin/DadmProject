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
        private HRV2_Data HRV2Data;

        #region Documentation
        /// <summary>
        /// TODO - uzupełnić dokumentację metody Main w HRV2 (jako metody testowej dla modułu)
        /// </summary>
        /// <param name="args"></param>
        /// 
        #endregion
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

        #region Documentation
        /// <summary>
        /// TODO - uzupełnić dokumentację konstruktora HRV2
        /// </summary>
        ///
        #endregion
        public HRV2()
        {
            HRV2Data = new HRV2_Data();
        }
    }
}
