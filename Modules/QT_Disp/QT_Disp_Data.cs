using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.QT_Disp
{
    public class QT_Disp_Data : ECG_Data
    {
        private List<Tuple<String, double>> _qt_disp_local;
        private List<Tuple<String, double>> _qt_mean;
        private List<Tuple<String, double>> _qt_std;
        private double _qt_disp_global;
        private List<Tuple<String,List<int>>> _t_end_local;
        private List<Tuple<String, List<double>>> _qt_intervals;
 
        public QT_Disp_Data(List<Tuple<String, double>>QT_Disp_local, List<Tuple<String, double>> QT_Mean, List<Tuple<String, double>> QT_Std, double QT_Disp_Global, List<Tuple<String,List<int>>> T_End_local, List<Tuple<String,List<double>>> QT_Intervals)
        {
            this.QT_disp_local = QT_Disp_local;
            this.QT_mean = QT_Mean;
            this.QT_std = QT_Std;
            this.QT_disp_global = QT_Disp_Global;
            this.T_End_Local = T_End_local;
            this.QT_Intervals = QT_Intervals;
        }
        public QT_Disp_Data()
        {
            this.QT_disp_local = new List<Tuple<string, double>>();
            this.QT_mean = new List<Tuple<string, double>>();
            this.QT_std = new List<Tuple<string, double>>();
            this.QT_disp_global = 0;
            this.T_End_Local = new List<Tuple<string, List<int>>>();
            this.QT_Intervals = new List<Tuple<string, List<double>>>();
            
        }

        public List<Tuple<String, double>> QT_disp_local
        {
            get
            {
                return _qt_disp_local;
            }
            set
            {
                _qt_disp_local = value;
            }
        }

        public List<Tuple<String, double>> QT_mean
        {
            get
            {
                return _qt_mean;
            }
            set
            {
                _qt_mean = value;
            }

        }

        public List<Tuple<String, double>> QT_std
        {
            get
            {
                return _qt_std;
            }
            set
            {
                _qt_std = value;
            }
        }

        public double QT_disp_global
        {
            get
            {
                return _qt_disp_global;
            }
            set
            {
                _qt_disp_global = value;
            }
        }
        public List<Tuple<String,List<int>>> T_End_Local
        {
            get
            {
                return _t_end_local;
            }
            set
            {
                _t_end_local = value;
            }
        }
        public List<Tuple<String, List<double>>> QT_Intervals
        {
            get
            {
                return _qt_intervals;
            }
            set
            {
                _qt_intervals = value;
            }
        }

    }
}
