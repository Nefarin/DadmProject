//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;


//namespace EKG_Project.Modules.QT_Disp
//{
//    /// <summary>
//    /// This class has implemented three methods (formula) to calculate normalized QT_interval
//    /// </summary>
//    class QTCalculation
//    {
//        private UInt32 QRS_Onset;
//        private UInt32 T_End;
//        private float RR_Interval;
//        private UInt16 Fs;
//        public QTCalculation(UInt32 QRS_Onset, UInt32 T_End, float RR_Interval, UInt16 Fs)
//        {
//            this.QRS_Onset = QRS_Onset;
//            this.T_End = T_End;
//            this.RR_Interval = RR_Interval;
//            this.Fs = Fs;
//        }
//        /// <summary>
//        /// This function caluclate QT interval according to Bazetta's formula
//        /// </summary>
//        /// <returns>QT_Bazetta interval in seconds</returns>
//        public double QT_Bazetta()
//        {
//            return ((T_End - QRS_Onset) / Fs) / Math.Sqrt(RR_Interval);
//        }
//        /// <summary>
//        /// This function calculate QT interval according to Friderica's formula
//        /// </summary>
//        /// <returns>QT_Friderica interval in seconds</returns>
//        public double QT_Friderica()
//        {
//            return ((T_End - QRS_Onset) / Fs) / Math.Pow(RR_Interval, 1 / 3);

//        }
//        /// <summary>
//        /// This function calculate QT interval according to Framigham's formula
//        /// </summary>
//        /// <returns>QT_Framigham interval in seconds</returns>
//        public double QT_Framigham()
//        {
//            return ((T_End - QRS_Onset) / Fs) + 0.154 * (1 - RR_Interval);
//        }
//    }
//}
