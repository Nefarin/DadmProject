using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Histogram.Data;
using System.Collections.ObjectModel;
using EKG_Project.Modules.R_Peaks;

namespace EKG_Project.Modules.HRV2
{
    public partial class HRV2 : IModule
    {
        private HRV2_Data HRV2Data;

        #region Documentation
        /// <summary>
        /// metoda testowa dla modułu
        /// </summary>
        /// <param name="args"></param>
        /// 
        #endregion
        static void Main(string[] args)
        {
            DataSource data = null;//new DataSource(0.0078125, RRInterval); //nie wiem jak pobrac z r_peaks :(
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
