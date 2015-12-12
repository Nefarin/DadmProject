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
        public QT_Calc_Method _qt_method;
        public T_End_Method _t_end_method;
        public bool _alldrains; // determinate if we calculate QT_disp for all drains or only one


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
            
        /*
        //Output this will be shown in GUI

        //This parameters are statistics 
        public Vector<double> qt_disp;
        public Vector<double> qt_mean;
        public Vector<double> qt_std;
        public double qt_disp_global;    

        //These matrix contatin Q_Onset and T_End index to select it on graph
        Matrix<double> t_endindex;
        Matrix<double> qrs_onsetindex;
        //The next rows of this matrix is a T_end indexes of next drains
        //Ex
        // T_end index of V1
        // T_end index of V2       

        //Getters and setters - Output
        public Vector<double> QT_Disp
        {
            get
            {
                return qt_disp;
            }
            set
            {
                qt_disp = value;
            }
        }
        public Vector<double> QT_Mean
        {
            get
            {
                return qt_mean;
            }
            set
            {
                qt_mean = value;
            }
        }
        public Vector<double> QT_Std
        {
            get
            {
                return qt_std;
            }
            set
            {
                qt_std = value;
            }
        }
        public double QT_Disp_Global
        {
            get
            {
                return qt_disp_global;
            }
            set
            {
                qt_disp_global = value;
            }
        }
        public Matrix<double> T_EndIndex{
            get
            {
                return t_endindex;
            }
            set
            {
                t_endindex = value;
            }
        }
        public Matrix<double> QRS_OnsetIndex
        {
            get
            {
                return qrs_onsetindex;
            }
            set
            {
                qrs_onsetindex = value;
            }
        }*/


    }
}
