﻿using System;
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

        public QT_Disp_Data(List<Tuple<String, double>>QT_Disp_local, List<Tuple<String, double>> QT_Mean, List<Tuple<String, double>> QT_Std, double QT_Disp_Global)
        {
            this.QT_disp_local = QT_Disp_local;
            this.QT_mean = QT_Mean;
            this.QT_std = QT_Std;
            this.QT_disp_global = QT_Disp_Global;
        }
        public QT_Disp_Data()
        {
            this.QT_disp_local = new List<Tuple<string, double>>();
            this.QT_mean = new List<Tuple<string, double>>();
            this.QT_std = new List<Tuple<string, double>>();
            this.QT_disp_global = 0;
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
    }
}
