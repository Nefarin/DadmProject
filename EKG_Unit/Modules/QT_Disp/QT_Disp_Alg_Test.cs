using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.QT_Disp;
using System.Collections.Generic;
using EKG_Project.IO;

namespace EKG_Unit.Modules.QT_Disp
{
    [TestClass]
    public class QT_Disp_Alg_Test
    {
       
        [TestMethod]
        [Description("Test if diff returns proper values")]
        public void test_diff()
        {
            DataToCalculate data = new DataToCalculate();

            double[] testArray = { 1, 2, 5, 9, 11};
            double[] expectedArray = { 1, 3, 4, 2 };

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> expectedVector = Vector<double>.Build.DenseOfArray(expectedArray);

            Vector<double> result = data.diff(testVector);

            Assert.AreEqual(expectedVector, result);

        }
        [TestMethod]
        [Description("Test if diff argument is correct")]
        [ExpectedException(typeof(InvalidOperationException), "Wrong input vector")]
        public void test_diff2()
        {
            DataToCalculate data = new DataToCalculate();

            double[] testArray = { 1};

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);    
            Vector<double> result = data.diff(testVector);
        }
        [TestMethod]
        [Description("Test if DataToCalculate constructs proper data")]
        public void test_DataToCalculate()
        {
            double[] samp1 = { 2.09, 3.94, 4.66, 9.78, 10.62 };
            double[] RR = { 12, 13 };
            Vector<double> samp = Vector<double>.Build.DenseOfArray(samp1);

            DataToCalculate data = new DataToCalculate(22, 37, 42, samp, QT_Calc_Method.FRAMIGHAMA, T_End_Method.PARABOLA, 360, RR);

            PrivateObject obj = new PrivateObject(data);
            Assert.AreEqual(22, obj.GetField("QRS_onset"));
            Assert.AreEqual(37, obj.GetField("QRS_End"));
            Assert.AreEqual(42, obj.GetField("T_End_Global"));
            Assert.AreEqual(samp, obj.GetField("samples"));
            Assert.AreEqual(QT_Calc_Method.FRAMIGHAMA, obj.GetField("QT_Calc_method"));
            Assert.AreEqual(T_End_Method.PARABOLA, obj.GetField("T_End_method"));
            Assert.AreEqual((uint)360, obj.GetField("Fs"));
            Assert.AreEqual(RR, obj.GetField("R_Peak"));
        }

        [TestMethod]
        [Description("Test if default constructor sets proper data")]
        public void test_DataToCalculate2()
        {
            double[] RR = new double[2];
            Vector<double> samp = Vector<double>.Build.Dense(1);

            DataToCalculate data = new DataToCalculate();

            PrivateObject obj = new PrivateObject(data);
            Assert.AreEqual(-1, obj.GetField("QRS_onset"));
            Assert.AreEqual(-1, obj.GetField("QRS_End"));
            Assert.AreEqual(-1, obj.GetField("T_End_Global"));
            //Assert.AreEqual(samp, obj.GetField("samples"));
            Assert.AreEqual(QT_Calc_Method.BAZETTA, obj.GetField("QT_Calc_method"));
            Assert.AreEqual(T_End_Method.TANGENT, obj.GetField("T_End_method"));
            Assert.AreEqual((uint)360, obj.GetField("Fs"));
           // Assert.AreEqual(RR, obj.GetField("R_Peak"));
        }
        [TestMethod]
        [Description("Test constructor QT_Disp_Alg if proper assertion")]
        public void testQT_Disp_Alg()
        {
            QT_Disp_Alg test = new QT_Disp_Alg();
            PrivateObject obj = new PrivateObject(test);
            List<int> onset = new List<int>();
            onset.Add(191);
            List<int> end = new List<int>();
            end.Add(225);
            List<int> tend = new List<int>();
            tend.Add(352);
            double[] rpeak = { 204,606};
            Vector<double> Rpeak = Vector<double>.Build.DenseOfArray(rpeak);

            test.TODoInInit(onset, tend, end,Rpeak, T_End_Method.TANGENT, QT_Calc_Method.FRIDERICA, (uint)360);

            Assert.AreEqual(onset, obj.GetField("QRS_onset"));
            Assert.AreEqual(end, obj.GetField("QRS_End"));
            Assert.AreEqual(tend, obj.GetField("T_End_Global"));
            Assert.AreEqual(Rpeak, obj.GetField("R_Peaks"));
            Assert.AreEqual(T_End_Method.TANGENT, obj.GetField("T_End_method"));
            Assert.AreEqual(QT_Calc_Method.FRIDERICA, obj.GetField("QT_Calc_method"));
            Assert.AreEqual((uint)360, obj.GetField("Fs"));
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [Description("Test if method FindT_End returns -1 when contructor parameters = -1")]
        public void testQT_Disp_Alg2()
        {//check if this is correct...
            QT_Disp_Alg test = new QT_Disp_Alg();

           PrivateObject obj = new PrivateObject(test);
            double[] signalTab = {-0.158,-0.16091,-0.16404,-0.16543,-0.168,-0.16571,-0.16356,-0.15949,-0.15549,-0.1505,-0.1495,-0.14559,-0.14673,-0.15086,-0.15696,-0.15613,
                -0.15531,-0.15256,-0.15079,-0.15099,-0.1521,-0.15509,-0.15601,-0.156,-0.155,-0.15597,-0.15299,-0.15097,-0.14699,-0.14114,-0.1394,-0.14079,-0.13847,
                -0.13744,-0.13674,-0.13353,-0.13476,-0.13917,-0.14149,-0.14329,-0.14111,-0.13551,-0.12983,-0.12651,-0.11926,-0.11,-0.095014,-0.076143,-0.057529,
                -0.043529,-0.033486,-0.023414,-0.016671,-0.0112,-0.0084143,-0.0084429,-0.0124,-0.015757,-0.018086,-0.020214,-0.020086,-0.022729,-0.026186,-0.028571,
                -0.026943,-0.028314,-0.030629,-0.034886,-0.0381,-0.040257,-0.042371,-0.041557,-0.040814,-0.044057,-0.048286,-0.049471,-0.048643,-0.047771,-0.046857,
                 -0.048843,-0.048814,-0.047743,-0.044729,-0.037871,-0.029229,-0.024643,-0.016171,-0.0058143,0.0083571,0.022314,0.038014,0.046529,0.056857,0.065043,
                0.074086,0.080986,0.086814,0.087629,0.087471,0.087371,0.090229,0.094971,0.095714,0.094471,0.092257,0.090114,0.087986,0.091743,0.090486,0.086257,
                0.085057,0.082929,0.080843,0.083657,0.084414,0.079186,0.077957,0.075771,0.071643,0.073429,0.074114,0.071786,0.069443,0.072043,0.0736,0.074114,0.073557,
                 0.069986,0.064443,0.0609,0.057386,0.054843,0.054229,0.049614,0.045057,0.042486,0.040871,0.037214,0.034514};

            //Vector<double> sampl = TempInput.getSignal(); //to nie dziala bo prywatne...
            //PrivateObject sig = new PrivateObject(sampl); // a w ten sposob sie nie da...
            Vector<double> sampl = Vector<double>.Build.DenseOfArray(signalTab);
            List<int> onset = new List<int>();
            onset.Add(1);
            List<int> end = new List<int>();
            end.Add(5);
            List<int> tend = new List<int>();
            tend.Add(90);
            double[] rpeak = { 4, 120 };
            Vector<double> Rpeak = Vector<double>.Build.DenseOfArray(rpeak);
           
            test.TODoInInit(onset, tend, end, Rpeak, T_End_Method.TANGENT, QT_Calc_Method.FRIDERICA, (uint)360);
            test.ToDoInProccessData(sampl, 0);
            DataToCalculate data = new DataToCalculate(); // check if this is correct way of thinking...
                                                          // because I get adverse results than I expected...
            int result = data.FindT_End();
            Tuple<int, double> result2 = data.Calc_QT_Interval();

            Assert.AreEqual(-1, result);
            Assert.AreEqual(0, result2.Item2);
        }

    }
}
