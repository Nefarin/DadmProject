using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.QT_Disp
{
    public enum QT_Calc_Method { BAZETTA, FRIDERICA, FRAMIGHAMA };
    public enum T_End_Method { TANGENT, PARABOLA };

    public class QT_Disp_Params : ModuleParams
    {


        //Input which can be modify in GUI
        private QT_Calc_Method _qt_method;
        private T_End_Method _t_end_method;
        private bool _alldrains; // determinate if we calculate QT_disp for all drains or only one
        private string _analysisName;
        public QT_Disp_Params()
        {
            this._alldrains = false;
            this._qt_method = QT_Calc_Method.BAZETTA;
            this._t_end_method = T_End_Method.TANGENT;
            this._analysisName = "TestAnalysis100";
        }


        public QT_Disp_Params(string analysisName)
        {
            this._alldrains = false;
            this._qt_method = QT_Calc_Method.BAZETTA;
            this._t_end_method = T_End_Method.TANGENT;
            this._analysisName = analysisName;
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
            }
        }
        public string AnalysisName
        {
            get
            {
                return _analysisName;
            }
            set
            {
                _analysisName = value;
            }

        }




    }
}
