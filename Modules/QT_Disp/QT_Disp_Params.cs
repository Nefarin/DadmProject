using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EKG_Project.Modules.QT_Disp
{
    public enum QT_Calc_Method { BAZETTA, FRIDERICA, FRAMIGHAMA };
    public enum T_End_Method { TANGENT, PARABOLA };

    public class QT_Disp_Params : ModuleParams, INotifyPropertyChanged
    {


        //Input which can be modify in GUI
        private QT_Calc_Method _qt_method;
        private T_End_Method _t_end_method;
        private bool _alldrains; // determinate if we calculate QT_disp for all drains or only one
        public event PropertyChangedEventHandler PropertyChanged;

        public QT_Disp_Params()
        {
            this._alldrains = false;
            this._qt_method = QT_Calc_Method.FRAMIGHAMA;
            this._t_end_method = T_End_Method.TANGENT;
        }


        public QT_Disp_Params(string analysisName)
        {
            this._alldrains = false;
            this._qt_method = QT_Calc_Method.BAZETTA;
            this._t_end_method = T_End_Method.TANGENT;
            this.AnalysisName = analysisName;
        }

        public QT_Calc_Method QTMethod
        {
            get
            {
                return _qt_method;
            }
            set
            {
                _qt_method = value;
            }
        }
        public T_End_Method TEndMethod
        {
            get
            {
                return _t_end_method;
            }
            set
            {
                _t_end_method = value;
            }
        }
        public bool AllDrains
        {
            get
            {
                return _alldrains;
            }
            set
            {
                _alldrains = value;
                this.NotifyPropertyChanged("AllDrains");
                this.NotifyPropertyChanged("SingleDrain");
            }
        }

        public bool SingleDrain
        {
            get
            {
                return !_alldrains;
            }
            set
            {
                _alldrains = !value;
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
