using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.QT_Disp
{
    /// <summary>
    /// This class calulate QT dispersion and other Satistic
    /// </summary>
    class QTIntervalStatistics
    {
        
        //This parameters are statistics calculate according to Bazetta formula
        double QT_disp_Bazetta;
        double QT_min_Bazetta;
        double QT_max_Bazetta;
        double QT_mean_Bazetta;
        double QT_std_Bazetta;
        List<double> QT_Interval_Bazetta = new List<double>();
       
        //These parameters are statistics calculate according to Friderica formula
        double QT_disp_Friderica;
        double QT_min_Friderica;
        double QT_max_Friderica;
        double QT_mean_Friderica;
        double QT_std_Friderica;
        List<double> QT_Interval_Fridericia = new List<double>();
       
        //These parameters are statistics calculate accroding to Ftamigham formula
        double QT_disp_Framigham;
        double QT_min_Framigham;
        double QT_max_Framigham;
        double QT_mean_Framigham;
        double QT_std_Framigham;
        List<double> QT_Interval_Framigham = new List<double>();

        public void setTheList(List<UInt32> Q_onset, List<UInt32> T_end,float RR_interval, UInt16 Fs)
        {
            int Q_onsetSize = Q_onset.Count;
            int T_endSize = T_end.Count;
            QTCalculation QT_interval;
            if (Q_onset.ElementAt(0) < T_end.ElementAt(0))
            {
                for(int i = 0; i <= T_endSize; i++)
                {
                    QT_interval = new QTCalculation(Q_onset.ElementAt(i),T_end.ElementAt(i),RR_interval,Fs);
                    QT_Interval_Bazetta.Add(QT_interval.QT_Bazetta());
                    QT_Interval_Fridericia.Add(QT_interval.QT_Friderica());
                    QT_Interval_Framigham.Add(QT_interval.QT_Framigham());
                    
                }
            }
        }
        public void setBazettaParameters()
        {

        }
        public void setFridericiaParameters()
        {

        }
        public void setFramighamParameters()
        {

        }
    }
}
