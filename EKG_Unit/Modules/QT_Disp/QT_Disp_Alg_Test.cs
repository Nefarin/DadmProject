using EKG_Project.IO;
using EKG_Project.Modules.QT_Disp;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

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
            double[] RR = { 12, 17 };
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
        [Description("Test if method returns -1, 0 when contructor parameters QRSonset= -1 and Tend = -1")]
        public void testQT_Disp_Alg2()
        {//check if this is correct...
            QT_Disp_Alg test = new QT_Disp_Alg();

           PrivateObject obj = new PrivateObject(test);
            double[] signalTab = { 0.3062351, 0.28985391, 0.25624014, 0.20787747, 0.14873037, 0.083813827, 0.018645662,
                -0.041548435, -0.092222265, -0.13023595, -0.15420422, -0.16411052, -0.16140441, -0.14840957, -0.12827963,
                -0.10450146, -0.080351337, -0.058148759, -0.039751104, -0.025919515, -0.016778473, -0.011926399, -0.010313956,
                -0.010919177, -0.012712054, -0.014793107, -0.016475001, -0.01736423, -0.017499676, -0.01712471, -0.016458301,
                -0.015804308, -0.015254206, -0.01493347, -0.014772223, -0.014494161, -0.013845832, -0.012680176, -0.011095811,
                -0.0093350022, -0.0076336086, -0.0063625255, -0.0058248832, -0.0060277634, -0.0069381437, -0.0083222683,
                -0.009871097, -0.011638705, -0.013456991, -0.015290911, -0.016919485, -0.018478639, -0.020146118, -0.021771038,
                -0.023559189, -0.025393898, -0.027162822, -0.02864509, -0.0294567, -0.029497743, -0.028787499, -0.027397805,
                -0.025715287, -0.023843042, -0.021963216, -0.020124823, -0.018468096, -0.017050461, -0.015939246, -0.015098473,
                -0.014537426, -0.014246029, -0.013958824, -0.013623241, -0.013244793, -0.012791544, -0.012382573, -0.011996321,
                -0.011532241, -0.010772696, -0.00965002, -0.0081109232, -0.0061365837, -0.0037646959, -0.00082585992, 0.0025274093,
                0.0063037222, 0.010376793, 0.014151442, 0.017074803, 0.018835974, 0.019124504, 0.018238457, 0.016535054,
                0.014527155, 0.0127432, 0.011652207, 0.011652873, 0.012747657, 0.014910444, 0.017870797, 0.021439944, 0.025295492,
                0.029010161, 0.032278838, 0.034952317, 0.037091057, 0.038739972, 0.040116293, 0.041103928, 0.041884918, 0.042336271,
                0.042657929, 0.042985063, 0.043253282, 0.043624026, 0.044051642, 0.044204047, 0.044015901, 0.043603348, 0.04300649,
                0.042207177, 0.041021891, 0.039517611, 0.037813943, 0.036128531, 0.034486542, 0.033055213, 0.032023102, 0.031172369,
                0.030639618, 0.030203127, 0.029668372, 0.02894542, 0.027939905, 0.026413671, 0.024017528, 0.020599098, 0.0159994,
                0.010383949, 0.003587472, -0.0040662968, -0.012030876, -0.019889449, -0.027074109, -0.032830551, -0.036782809,
                -0.038675805, -0.038642805, -0.036918008, -0.034535059, -0.032225691, -0.030380022, -0.029391611, -0.0295182,
                -0.030990501, -0.033375217, -0.036336287, -0.039476656, -0.041979303, -0.043492442, -0.043709881, -0.042582583,
                -0.040295752, -0.037113453, -0.033487308, -0.029682156, -0.026170604, -0.023502165, -0.021948085, -0.021453395,
                -0.021521662, -0.021949302, -0.022330802, -0.022095781, -0.021240034, -0.019713105, -0.017719845, -0.015545089,
                -0.01355662, -0.011934521, -0.010838052, -0.01044113, -0.010784676, -0.011548912, -0.012130486, -0.012253704,
                -0.011792794, -0.010401301, -0.0081359891, -0.0053352786, -0.0024425786, 0.00026555661, 0.0025425543, 0.0042861874,
                0.0053450003, 0.0055963754, 0.0052132415, 0.0046158541, 0.0041751443, 0.0040401008, 0.0041259112, 0.0047105992,
                0.0058954025, 0.0075254048, 0.0096612161, 0.012069461, 0.014588485, 0.016881878, 0.018473386, 0.019094709,
                0.018626091, 0.017236055, 0.015294126, 0.013019855, 0.010521463, 0.0081321647, 0.0063116305, 0.0053764007,
                0.0054134216, 0.0065454698, 0.0086304479, 0.011522721, 0.01460685, 0.017159445, 0.019000566, 0.0198684,
                0.01950051, 0.017847493, 0.014923426, 0.011080969, 0.0069463999, 0.0030662257, -2.3213654E-05, -0.0018644523,
                -0.0024082762, -0.0017458518, -0.00026619925, 0.0015228982, 0.0030792953, 0.0040186372, 0.0041961635,
                0.0037165528, 0.0026535313, 0.0013221654, 0.0002192174, -0.00032846604, -0.00010321325, 0.00090239151,
                0.0023411555, 0.0036296381, 0.0044818118, 0.0044996605, 0.0036468074, 0.0019930763, -0.00042874323, -0.0033531083,
                -0.0061960897, -0.0084261702, -0.0096485082, -0.0098476224, -0.0089435018, -0.0071793893, -0.0046324012,
                -0.0016219031, 0.0015557259, 0.0045152284, 0.0066640567, 0.007755425, 0.0077014038, 0.0065220087, 0.0043895961,
                0.0017453973, -0.00092599941, -0.0032528892, -0.0048468661, -0.0054337969, -0.0050973299, -0.0041945541,
                -0.0030736377, -0.0020270903, -0.0013784525, -0.0016188142, -0.0027735131, -0.0046823297, -0.0071357514,
                -0.0097731733, -0.012151547, -0.014077012, -0.015513907, -0.016145267, -0.015913858, -0.014749651, -0.012900054,
                -0.010741601, -0.0086398535, -0.0068062595, -0.0053670118, -0.0043258329, -0.0037454808, -0.0033580034,
                -0.0028738843, -0.0022398868, -0.0012816727, 4.87632E-06, 0.001627827, 0.0034248426, 0.0054188217, 0.0074603287,
                0.0092633771, 0.010625849, 0.011434304, 0.011740276, 0.011557815, 0.010849388, 0.0098618391, 0.0086457077,
                0.0070738671, 0.0050505671, 0.0024875369, -0.00053442454, -0.0037835692, -0.0068704182, -0.0094709421,
                -0.011050779, -0.011194157, -0.0098489647, -0.0071307049, -0.0033118771, 0.0011175949, 0.0055676656,
                0.0095236919, 0.012522285, 0.013879613, 0.013776328, 0.012585669, 0.01073486, 0.0086835438, 0.0067673748,
                0.0053710151, 0.0048362112, 0.0051280299, 0.0062452619, 0.0078623689, 0.0097808089, 0.011372411, 0.012265315,
                0.012015602, 0.010417466, 0.0078592953, 0.0046080279, 0.0010299947, -0.002487385, -0.0056500737, -0.0083482775,
                -0.010428551, -0.011982315, -0.013121246, -0.014113229, -0.015054261, -0.016291866, -0.017871408, -0.019714899,
                -0.021138853, -0.021779277, -0.021752931, -0.020815887, -0.019251711, -0.01750337, -0.015897326, -0.014824351,
                -0.014493504, -0.015016009, -0.016399922, -0.018411052, -0.020621463, -0.022567556, -0.023777676, -0.023909156,
                -0.023216619, -0.022044473, -0.020858181, -0.020517895, -0.021650476, -0.024849127, -0.030649733, -0.039004329,
                -0.049133123, -0.059844382, -0.069181469, -0.074874116, -0.074642659, -0.066266233, -0.048231191, -0.019937097,
                0.018387776, 0.064885728, 0.11667983, 0.1699266, 0.21990291, 0.26194305, 0.29193246, };
           
            Vector<double> sampl = Vector<double>.Build.DenseOfArray(signalTab);
            int onset = -1;
            //onset 192
            int end = 225;
            int tend = -1;
            //352
            double[] rpeak = { 204, 606 };
            //Vector<double> Rpeak = Vector<double>.Build.DenseOfArray(rpeak);
           
            //test.TODoInInit(onset, tend, end, Rpeak, T_End_Method.TANGENT, QT_Calc_Method.FRIDERICA, (uint)360);
            //test.ToDoInProccessData(sampl, 0);
            DataToCalculate data = new DataToCalculate(onset, end, tend, sampl, QT_Calc_Method.FRAMIGHAMA, T_End_Method.PARABOLA, 360, rpeak);
                                                          
            int result = data.FindT_End();
            Tuple<int, double> result2 = data.Calc_QT_Interval();

            Assert.AreEqual(-1, result);
            Assert.AreEqual(0, result2.Item2);
        }

        [TestMethod]
        [Description("Test if method parameter QRSonset not null")]
        [ExpectedException(typeof(ArgumentNullException), "QRS_Onset null")]
        public void test_DataToCalculate_3()
        {
            QT_Disp_Alg test = new QT_Disp_Alg();

            PrivateObject obj = new PrivateObject(test);
            double[] signalTab = { 0.3062351, 0.28985391, 0.25624014, 0.20787747, 0.14873037, 0.083813827, 0.018645662,
                -0.041548435, -0.092222265, -0.13023595, -0.15420422, -0.16411052, -0.16140441, -0.14840957, -0.12827963,
                -0.10450146, -0.080351337, -0.058148759, -0.039751104, -0.025919515, -0.016778473, -0.011926399, -0.010313956,
                -0.010919177, -0.012712054, -0.014793107, -0.016475001, -0.01736423, -0.017499676, -0.01712471, -0.016458301,
                0.018387776, 0.064885728, 0.11667983, 0.1699266, 0.21990291, 0.26194305, 0.29193246, };

            Vector<double> sampl = Vector<double>.Build.DenseOfArray(signalTab);
            int onset = new int();
            int end = 225;
            int tend = -1;
            //352
            double[] rpeak = { 204, 606 };
            //Vector<double> Rpeak = Vector<double>.Build.DenseOfArray(rpeak);

            DataToCalculate data = new DataToCalculate(onset, end, tend, sampl, QT_Calc_Method.FRAMIGHAMA, T_End_Method.PARABOLA, 360, rpeak);
        }

        [TestMethod]
        [Description("Test if method parameter QRSend not null")]
        [ExpectedException(typeof(ArgumentNullException), "QRS_End null")]
        public void test_DataToCalculate_4()
        {
            QT_Disp_Alg test = new QT_Disp_Alg();

            PrivateObject obj = new PrivateObject(test);
            double[] signalTab = { 0.3062351, 0.28985391, 0.25624014, 0.20787747, 0.14873037, 0.083813827, 0.018645662,
                -0.041548435, -0.092222265, -0.13023595, -0.15420422, -0.16411052, -0.16140441, -0.14840957, -0.12827963,
                -0.10450146, -0.080351337, -0.058148759, -0.039751104, -0.025919515, -0.016778473, -0.011926399, -0.010313956,
                -0.010919177, -0.012712054, -0.014793107, -0.016475001, -0.01736423, -0.017499676, -0.01712471, -0.016458301,
                0.018387776, 0.064885728, 0.11667983, 0.1699266, 0.21990291, 0.26194305, 0.29193246, };

            Vector<double> sampl = Vector<double>.Build.DenseOfArray(signalTab);
            int onset = 192;
            int end = new int();
            int tend = -1;
            //352
            double[] rpeak = { 204, 606 };
            //Vector<double> Rpeak = Vector<double>.Build.DenseOfArray(rpeak);

            DataToCalculate data = new DataToCalculate(onset, end, tend, sampl, QT_Calc_Method.FRAMIGHAMA, T_End_Method.PARABOLA, 360, rpeak);

            //int result = data.FindT_End();
            //Tuple<int, double> result2 = data.Calc_QT_Interval();

            //Assert.AreEqual(-1, result);
            //Assert.AreEqual(0, result2.Item2);
        }

        [TestMethod]
        [Description("Test if method parameter Tend_global not null")]
        [ExpectedException(typeof(ArgumentNullException), "T_End_Global null")]
        public void test_DataToCalculate_5()
        {
            QT_Disp_Alg test = new QT_Disp_Alg();

            PrivateObject obj = new PrivateObject(test);
            double[] signalTab = { 0.3062351, 0.28985391, 0.25624014, 0.20787747, 0.14873037, 0.083813827, 0.018645662,
                -0.041548435, -0.092222265, -0.13023595, -0.15420422, -0.16411052, -0.16140441, -0.14840957, -0.12827963,
                -0.10450146, -0.080351337, -0.058148759, -0.039751104, -0.025919515, -0.016778473, -0.011926399, -0.010313956,
                -0.010919177, -0.012712054, -0.014793107, -0.016475001, -0.01736423, -0.017499676, -0.01712471, -0.016458301,
                0.018387776, 0.064885728, 0.11667983, 0.1699266, 0.21990291, 0.26194305, 0.29193246, };

            Vector<double> sampl = Vector<double>.Build.DenseOfArray(signalTab);
            int onset = 192;
            int end = 255;
            int tend = new int();
            double[] rpeak = { 204, 606 };
            
            DataToCalculate data = new DataToCalculate(onset, end, tend, sampl, QT_Calc_Method.FRAMIGHAMA, T_End_Method.PARABOLA, 360, rpeak);  
        }

        [TestMethod]
        [Description("Test if method parameter proper samples vector length")]
        [ExpectedException(typeof(ArgumentNullException), "Samples wrong length")]
        public void test_DataToCalculate_6()
        {
            QT_Disp_Alg test = new QT_Disp_Alg();

            PrivateObject obj = new PrivateObject(test);
            double[] signalTab = { 0.3062351, 0.28985391, 0.25624014, 0.20787747, 0.14873037, 0.083813827, 0.018645662, };

            Vector<double> sampl = Vector<double>.Build.DenseOfArray(signalTab);
            int onset = 2;
            int end = 5;
            int tend = 6;
            double[] rpeak = { 0, 6 };

            DataToCalculate data = new DataToCalculate(onset, end, tend, sampl, QT_Calc_Method.FRAMIGHAMA, T_End_Method.PARABOLA, 360, rpeak);
        }
    }
}
