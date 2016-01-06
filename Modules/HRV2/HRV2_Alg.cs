using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using EKG_Project.Modules.R_Peaks;

namespace EKG_Project.Modules.HRV2
{
    public partial class HRV2 : IModule
    {
        #region

        #endregion
        private HRV2_Data Analyse(R_Peaks_Data RRIntervals)
        {
            HRV2_Data hrv2Data = new HRV2_Data();

            int most_often_RR = 0;


            hrv2Data.HistogramData = ;
            return hrv2Data;
        }

        #region Documentation
        /// <summary>
        /// metoda testowa dla modułu
        /// </summary>
        /// <param name="args"></param>
        /// 
        #endregion
        static void Main(string[] args)
        {
            DataSource data = new DataSource(0.0078125, RRInterval); //nie wiem jak pobrac z r_peaks :(
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
        }
    }
}
