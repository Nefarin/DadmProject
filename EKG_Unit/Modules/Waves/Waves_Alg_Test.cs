using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EKG_Project.Modules.Waves;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;

namespace EKG_Unit.Modules.Waves
{
    [TestClass]
    public class Waves_Alg_Test
    {
        [TestMethod]
        [Description("Test if method returns exception when decomposition level is too low")]
        [ExpectedException(typeof(InvalidOperationException), "Decompositionlevel is too low")]
        public void ListHaarDWTTest()
        {
            double [] signalArray = { 8, 7, 6, 5, 5, 9, 6, 7, 8, 10, 9 };
            Vector<double> signalVector = Vector<double>.Build.DenseOfArray(signalArray);
            Waves_Params testParams = new Waves_Params(Wavelet_Type.haar, 3, "Analysis1", 500);

            Waves_Alg testAlgs = new Waves_Alg(testParams);
            PrivateObject obj = new PrivateObject(testAlgs);

            object[] args = { signalVector, 0 };

            obj.Invoke("ListHaarDWT", args);
        }

        [TestMethod]
        [Description("Test if method return exception decomposition level is to high (not enough samples)")]
        [ExpectedException(typeof(InvalidOperationException), "Not long enough signal for such decomposition")]
        public void ListHaarDWTTest2()
        {
            double[] signalArray = { 8, 7, 6, 5, 5, 9, 6, 7, 8, 10, 9 };
            Vector<double> signalVector = Vector<double>.Build.DenseOfArray(signalArray);
            Waves_Params testParams = new Waves_Params(Wavelet_Type.haar, 4, "Analysis1", 500);

            Waves_Alg testAlgs = new Waves_Alg(testParams);
            PrivateObject obj = new PrivateObject(testAlgs);

            object[] args = { signalVector, 0 };

            obj.Invoke("ListHaarDWT", args);
        }

        [TestMethod]
        [Description("Test if method returns right result of Haar decomposition")]
        public void HaarDecompositionResult()
        {
            Waves_Params param = new Waves_Params(Wavelet_Type.haar, 2, "TestAnalysis100", 500);
            Waves_Alg testAlgs = new Waves_Alg(param);

            double[] signalArray = { 1, 2, 3, 4, 5, 9, 7, 7, 8, 24, 9 };
            Vector<double> signalVector = Vector<double>.Build.DenseOfArray(signalArray);

            double[] trueOut = { -2, 0};
            Vector<double> trueOutVector = Vector<double>.Build.DenseOfArray(trueOut);

            PrivateObject obj = new PrivateObject(testAlgs);

            object[] args = { signalVector, 2 };
            var dwt  = obj.Invoke("ListHaarDWT", args);
            List<Vector<double>> dwtL = (List<Vector<double>>)dwt;
            Vector<double> dwtLevel2 = dwtL[1];
            Assert.AreEqual(dwtLevel2 , trueOutVector);
        }

        [TestMethod]
        [Description("Test if method returns right result of db2 decomposition")]
        public void db2DecompositionResult()
        {
            Waves_Params param = new Waves_Params(Wavelet_Type.db2, 2, "TestAnalysis100", 500);
            Waves_Alg testAlgs = new Waves_Alg(param);

            double[] signalArray = { 1, 2, 3, 4, 5, 9, 7, 7, 8, 24, 9 };
            Vector<double> signalVector = Vector<double>.Build.DenseOfArray(signalArray);

            double[] trueOut = { 3.6818, 18.5651 };
            Vector<double> trueOutVector = Vector<double>.Build.DenseOfArray(trueOut);

            PrivateObject obj = new PrivateObject(testAlgs);
            
            object[] args = { signalVector, 2 , Wavelet_Type.db2};
            var dwt = obj.Invoke("ListDWT", args);
            List<Vector<double>> dwtL = (List<Vector<double>>)dwt;
            Vector<double> dwtLevel2 = dwtL[1];
            for( int i = 0; i< dwtLevel2.Count; i++)
            {
                dwtLevel2[i] = Math.Round(dwtLevel2[i], 4);
            }
            Assert.AreEqual(dwtLevel2, trueOutVector);
        }

        [TestMethod]
        [Description("Test if method returns right result of db2 decomposition")]
        public void db3DecompositionResult()
        {
            Waves_Params param = new Waves_Params(Wavelet_Type.db3, 2, "TestAnalysis100", 500);
            Waves_Alg testAlgs = new Waves_Alg(param);

            double[] signalArray = { 1, 2, 3, 4, 5, 9, 7, 7, 8, 24, 9 };
            Vector<double> signalVector = Vector<double>.Build.DenseOfArray(signalArray);

            double[] trueOut = { 16.5803, -2.4791 };
            Vector<double> trueOutVector = Vector<double>.Build.DenseOfArray(trueOut);

            PrivateObject obj = new PrivateObject(testAlgs);

            object[] args = { signalVector, 2, Wavelet_Type.db3 };
            var dwt = obj.Invoke("ListDWT", args);
            List<Vector<double>> dwtL = (List<Vector<double>>)dwt;
            Vector<double> dwtLevel2 = dwtL[1];
            for (int i = 0; i < dwtLevel2.Count; i++)
            {
                dwtLevel2[i] = Math.Round(dwtLevel2[i], 4);
            }
            Assert.AreEqual(dwtLevel2, trueOutVector);
        }

        [TestMethod]
        [Description("Test if derivSquare return right value")]
        public void derivSquareValueTest()
        {
            Waves_Params param = new Waves_Params(Wavelet_Type.db3, 2, "TestAnalysis100", 500);
            Waves_Alg testAlgs = new Waves_Alg(param);
            double[] signalArray = { 1, 2, 3, 4, 5, 9, 7, 7, 8, 24, 9 };
            Vector<double> signalVector = Vector<double>.Build.DenseOfArray(signalArray);

            double trueResult = 3.0625;
            PrivateObject obj = new PrivateObject(testAlgs);

            object[] args = { 4, signalVector };
            double result2check = (double)obj.Invoke("derivSquare", args);

            Assert.AreEqual(result2check, trueResult, 0.0001);
        }

        [TestMethod]
        [Description("Test if lastNderivSquares return right value")]
        public void lastNderivSquaresTest()
        {
            Waves_Params param = new Waves_Params(Wavelet_Type.db3, 2, "TestAnalysis100", 500);
            Waves_Alg testAlgs = new Waves_Alg(param);
            double[] signalArray = { 1, 2, 3, 4, 5, 9, 7, 7, 8, 24, 9 };
            Vector<double> signalVector = Vector<double>.Build.DenseOfArray(signalArray);

            double trueResult = 5.71875;
            PrivateObject obj = new PrivateObject(testAlgs);

            object[] args = { 4, 5, signalVector };
            double result2check = (double)obj.Invoke("lastNderivSquares", args);

            Assert.AreEqual(result2check, trueResult, 0.0001);
        }

        [TestMethod]
        [Description("Test if nextNderivSquares return right value")]
        public void nextNderivSquaresTest()
        {
            Waves_Params param = new Waves_Params(Wavelet_Type.db3, 2, "TestAnalysis100", 500);
            Waves_Alg testAlgs = new Waves_Alg(param);
            double[] signalArray = { 1, 2, 3, 4, 5, 9, 7, 7, 8, 24, 9 };
            Vector<double> signalVector = Vector<double>.Build.DenseOfArray(signalArray);

            double trueResult = 25.546875;
            PrivateObject obj = new PrivateObject(testAlgs);

            object[] args = { 4, 5, signalVector };
            double result2check = (double)obj.Invoke("nextNderivSquares", args);

            Assert.AreEqual(result2check, trueResult, 0.0001);
        }

        [TestMethod]
        [Description("Test if method finds maximum value in vector")]
        public void MaxValueTest1()
        {
            Waves_Params testParams = new Waves_Params(Wavelet_Type.haar,3,"Analysis1",500);

            double[] testArray = { 8, 7, 6, 5, 5, 9, 6, 7, 8 , 10 , 9 };
            double result = 9;
            int begin_loc = 0;
            int end_loc = 6;
            double max_val = 0;
            int max_loc = 0;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            Waves_Alg testAlgs = new Waves_Alg(testParams);
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { begin_loc , end_loc ,  max_loc ,  max_val , testVector };

            obj.Invoke("FindMaxValue", args);

            Assert.AreEqual( args[3] , result);


        }

        [TestMethod]
        [Description("Test if method finds location of maximum in vector")]
        public void MaxValueTest2()
        {
            Waves_Params testParams = new Waves_Params(Wavelet_Type.haar, 3, "Analysis1", 500);

            double[] testArray = { 8, 7, 6, 5, 5, 9, 6, 7, 8, 10, 9 };
            int result = 5;
            int begin_loc = 0;
            int end_loc = 6;
            double max_val = 0;
            int max_loc = 0;

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            Waves_Alg testAlgs = new Waves_Alg(testParams);
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { begin_loc, end_loc, max_loc, max_val, testVector };

            obj.Invoke("FindMaxValue", args);

            Assert.AreEqual( args[2], result);




        }

        [TestMethod]
        [Description("Test if QRSonset is found for wavelet type: haar, decomposition level: 3")]
        public void FindQRSTestQRSonsetHaar3()
        {
            Waves_Params testParams = new Waves_Params(Wavelet_Type.haar, 3, "Analysis1", 500);

            double[] testArray = { 0.030343,0.032414,0.031457,0.032414,0.029371,0.027314,0.023343,0.025343,0.0263,0.025214,0.021129,0.018971,0.015814,0.011671,0.011443,
                0.0082143,0.0030571,-0.004,-0.007,-0.011914,-0.014814,-0.0177,-0.018614,-0.019529,-0.017443,-0.0134,-0.010429,-0.0094571,-0.013357,-0.015143,-0.017786,
                -0.0203,-0.022743,-0.023143,-0.030371,-0.033486,-0.033471,-0.030371,-0.0302,-0.029986,-0.033643,-0.036157,-0.037514,-0.035771,-0.032014,-0.029229,-0.031371,
                -0.035386,-0.036357,-0.035271,-0.031229,-0.025286,-0.021414,-0.0176,-0.015829,-0.012071,-0.010329,-0.0076429,-0.0079429,-0.0072714,-0.0066429,-0.0020857,
                -0.00058571,-0.00014286,-0.0016714,-0.0041571,-0.0066429,-0.0051857,0.00014286,0.0093143,0.015371,0.019414,0.0234,0.029329,0.033171,0.036943,0.046529,
                0.056014,0.0644,0.073643,0.0827,0.083714,0.080729,0.076743,0.070843,0.065971,0.063071,0.062086,0.058114,0.054129,0.051143,0.051114,0.054029,0.058857,
                0.056743,0.045843,0.032186,0.013814,-0.0073,-0.024186,-0.032971,-0.040671,-0.043357,-0.045014,-0.041814,-0.040671,-0.043486,-0.049171,-0.0528,-0.057357,
                -0.0619,-0.062471,-0.064043,-0.061629,-0.060186,-0.055771,-0.050471,-0.048271,-0.053029,-0.053771,-0.053471,-0.054086,-0.0507,-0.049286,-0.053786,-0.059157,
                -0.069357,-0.087257,-0.1049,-0.12523,-0.15313,-0.18153,-0.19469,-0.18593,-0.1487,-0.077514,0.026143,0.1717,0.34774,0.544,0.74039,0.9181,1.037,1.0741,1.013,
                0.85803,0.63816,0.39679,0.16481,-0.023329,-0.1495,-0.21566,-0.2434,-0.24019,-0.22521,-0.20456,-0.18911,-0.1778,-0.17356,-0.16939,-0.16921,-0.16604,-0.1648,
                -0.16346,-0.16014,-0.15797,-0.16091,-0.16404,-0.16543,-0.168,-0.16571,-0.16356,-0.15949,-0.15549,-0.1505,-0.1495,-0.14559,-0.14673,-0.15086,-0.15696,
                -0.15613,-0.15531,-0.15256,-0.15079,-0.15099,-0.1521,-0.15509,-0.15601,-0.156,-0.155,-0.15597,-0.15299,-0.15097,-0.14699,-0.14114,-0.1394,-0.14079,-0.13847,
                -0.13744,-0.13674,-0.13353,-0.13476,-0.13917,-0.14149,-0.14329,-0.14111,-0.13551,-0.12983,-0.12651,-0.11926,-0.11,-0.095014,-0.076143,-0.057529,-0.043529,
                -0.033486,-0.023414,-0.016671,-0.0112,-0.0084143,-0.0084429,-0.0124,-0.015757,-0.018086,-0.020214,-0.020086,-0.022729,-0.026186,-0.028571,-0.026943,-0.028314,
                -0.030629,-0.034886,-0.0381,-0.040257,-0.042371,-0.041557,-0.040814,-0.044057,-0.048286,-0.049471,-0.048643,-0.047771,-0.046857,-0.048843,-0.048814,-0.047743,
                -0.044729,-0.037871,-0.029229,-0.024643,-0.016171,-0.0058143,0.0083571,0.022314,0.038014,0.046529,0.056857,0.065043,0.074086,0.080986,0.086814,0.087629,
                0.087471,0.087371,0.090229,0.094971,0.095714,0.094471,0.092257,0.090114,0.087986,0.091743,0.090486,0.086257,0.085057,0.082929,0.080843,0.083657,0.084414,
                0.079186,0.077957,0.075771,0.071643,0.073429,0.074114,0.071786,0.069443,0.072043,0.0736,0.074114,0.073557,0.069986,0.064443,0.0609,0.057386,0.054843,0.054229,
                0.049614,0.045057,0.042486,0.040871,0.037214,0.034514};

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            double[] rpeak = { 144 };

            Vector<double> rpeaks = Vector<double>.Build.DenseOfArray(rpeak);

            List<int> qrsOns = new List<int>();
            List<int> qrsEnds = new List<int>();
            List<int> POns = new List<int>();
            List<int> Pends = new List<int>();
            List<int> Tends = new List<int>();

            uint freq = 360;
            int offset = 0;

            Waves_Alg testAlgs = new Waves_Alg(testParams);
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { testVector, rpeaks, qrsOns, qrsEnds,
                            POns, Pends, Tends, offset, freq};

            obj.Invoke("analyzeSignalPart", args);

            List<int> qrsOnsOut = (List<int>)args[2];
            Assert.AreEqual( 119, qrsOnsOut[0], 10);

        }

        [TestMethod]
        [Description("Test if QRSonset is found for wavelet type: db2, decomposition level: 1")]
        public void FindQRSTestQRSonsetDb2_1()
        {
            Waves_Params testParams = new Waves_Params(Wavelet_Type.db2, 1, "Analysis1", 500);

            double[] testArray = { 0.030343,0.032414,0.031457,0.032414,0.029371,0.027314,0.023343,0.025343,0.0263,0.025214,0.021129,0.018971,0.015814,0.011671,0.011443,
                0.0082143,0.0030571,-0.004,-0.007,-0.011914,-0.014814,-0.0177,-0.018614,-0.019529,-0.017443,-0.0134,-0.010429,-0.0094571,-0.013357,-0.015143,-0.017786,
                -0.0203,-0.022743,-0.023143,-0.030371,-0.033486,-0.033471,-0.030371,-0.0302,-0.029986,-0.033643,-0.036157,-0.037514,-0.035771,-0.032014,-0.029229,-0.031371,
                -0.035386,-0.036357,-0.035271,-0.031229,-0.025286,-0.021414,-0.0176,-0.015829,-0.012071,-0.010329,-0.0076429,-0.0079429,-0.0072714,-0.0066429,-0.0020857,
                -0.00058571,-0.00014286,-0.0016714,-0.0041571,-0.0066429,-0.0051857,0.00014286,0.0093143,0.015371,0.019414,0.0234,0.029329,0.033171,0.036943,0.046529,
                0.056014,0.0644,0.073643,0.0827,0.083714,0.080729,0.076743,0.070843,0.065971,0.063071,0.062086,0.058114,0.054129,0.051143,0.051114,0.054029,0.058857,
                0.056743,0.045843,0.032186,0.013814,-0.0073,-0.024186,-0.032971,-0.040671,-0.043357,-0.045014,-0.041814,-0.040671,-0.043486,-0.049171,-0.0528,-0.057357,
                -0.0619,-0.062471,-0.064043,-0.061629,-0.060186,-0.055771,-0.050471,-0.048271,-0.053029,-0.053771,-0.053471,-0.054086,-0.0507,-0.049286,-0.053786,-0.059157,
                -0.069357,-0.087257,-0.1049,-0.12523,-0.15313,-0.18153,-0.19469,-0.18593,-0.1487,-0.077514,0.026143,0.1717,0.34774,0.544,0.74039,0.9181,1.037,1.0741,1.013,
                0.85803,0.63816,0.39679,0.16481,-0.023329,-0.1495,-0.21566,-0.2434,-0.24019,-0.22521,-0.20456,-0.18911,-0.1778,-0.17356,-0.16939,-0.16921,-0.16604,-0.1648,
                -0.16346,-0.16014,-0.15797,-0.16091,-0.16404,-0.16543,-0.168,-0.16571,-0.16356,-0.15949,-0.15549,-0.1505,-0.1495,-0.14559,-0.14673,-0.15086,-0.15696,
                -0.15613,-0.15531,-0.15256,-0.15079,-0.15099,-0.1521,-0.15509,-0.15601,-0.156,-0.155,-0.15597,-0.15299,-0.15097,-0.14699,-0.14114,-0.1394,-0.14079,-0.13847,
                -0.13744,-0.13674,-0.13353,-0.13476,-0.13917,-0.14149,-0.14329,-0.14111,-0.13551,-0.12983,-0.12651,-0.11926,-0.11,-0.095014,-0.076143,-0.057529,-0.043529,
                -0.033486,-0.023414,-0.016671,-0.0112,-0.0084143,-0.0084429,-0.0124,-0.015757,-0.018086,-0.020214,-0.020086,-0.022729,-0.026186,-0.028571,-0.026943,-0.028314,
                -0.030629,-0.034886,-0.0381,-0.040257,-0.042371,-0.041557,-0.040814,-0.044057,-0.048286,-0.049471,-0.048643,-0.047771,-0.046857,-0.048843,-0.048814,-0.047743,
                -0.044729,-0.037871,-0.029229,-0.024643,-0.016171,-0.0058143,0.0083571,0.022314,0.038014,0.046529,0.056857,0.065043,0.074086,0.080986,0.086814,0.087629,
                0.087471,0.087371,0.090229,0.094971,0.095714,0.094471,0.092257,0.090114,0.087986,0.091743,0.090486,0.086257,0.085057,0.082929,0.080843,0.083657,0.084414,
                0.079186,0.077957,0.075771,0.071643,0.073429,0.074114,0.071786,0.069443,0.072043,0.0736,0.074114,0.073557,0.069986,0.064443,0.0609,0.057386,0.054843,0.054229,
                0.049614,0.045057,0.042486,0.040871,0.037214,0.034514};

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            double[] rpeak = { 144 };

            Vector<double> rpeaks = Vector<double>.Build.DenseOfArray(rpeak);

            List<int> qrsOns = new List<int>();
            List<int> qrsEnds = new List<int>();
            List<int> POns = new List<int>();
            List<int> Pends = new List<int>();
            List<int> Tends = new List<int>();

            uint freq = 360;
            int offset = 0;

            Waves_Alg testAlgs = new Waves_Alg(testParams);
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { testVector, rpeaks, qrsOns, qrsEnds,
                            POns, Pends, Tends, offset, freq};

            obj.Invoke("analyzeSignalPart", args);

            List<int> qrsOnsOut = (List<int>)args[2];
            Assert.AreEqual( 119, qrsOnsOut[0], 10);

        }

        [TestMethod]
        [Description("Test if QRSonset is found for wavelet type: db3, decomposition level: 2")]
        public void FindQRSTestQRSonsetDb3_2()
        {
            Waves_Params testParams = new Waves_Params(Wavelet_Type.db3, 2, "Analysis1", 500);

            double[] testArray = { 0.030343,0.032414,0.031457,0.032414,0.029371,0.027314,0.023343,0.025343,0.0263,0.025214,0.021129,0.018971,0.015814,0.011671,0.011443,
                0.0082143,0.0030571,-0.004,-0.007,-0.011914,-0.014814,-0.0177,-0.018614,-0.019529,-0.017443,-0.0134,-0.010429,-0.0094571,-0.013357,-0.015143,-0.017786,
                -0.0203,-0.022743,-0.023143,-0.030371,-0.033486,-0.033471,-0.030371,-0.0302,-0.029986,-0.033643,-0.036157,-0.037514,-0.035771,-0.032014,-0.029229,-0.031371,
                -0.035386,-0.036357,-0.035271,-0.031229,-0.025286,-0.021414,-0.0176,-0.015829,-0.012071,-0.010329,-0.0076429,-0.0079429,-0.0072714,-0.0066429,-0.0020857,
                -0.00058571,-0.00014286,-0.0016714,-0.0041571,-0.0066429,-0.0051857,0.00014286,0.0093143,0.015371,0.019414,0.0234,0.029329,0.033171,0.036943,0.046529,
                0.056014,0.0644,0.073643,0.0827,0.083714,0.080729,0.076743,0.070843,0.065971,0.063071,0.062086,0.058114,0.054129,0.051143,0.051114,0.054029,0.058857,
                0.056743,0.045843,0.032186,0.013814,-0.0073,-0.024186,-0.032971,-0.040671,-0.043357,-0.045014,-0.041814,-0.040671,-0.043486,-0.049171,-0.0528,-0.057357,
                -0.0619,-0.062471,-0.064043,-0.061629,-0.060186,-0.055771,-0.050471,-0.048271,-0.053029,-0.053771,-0.053471,-0.054086,-0.0507,-0.049286,-0.053786,-0.059157,
                -0.069357,-0.087257,-0.1049,-0.12523,-0.15313,-0.18153,-0.19469,-0.18593,-0.1487,-0.077514,0.026143,0.1717,0.34774,0.544,0.74039,0.9181,1.037,1.0741,1.013,
                0.85803,0.63816,0.39679,0.16481,-0.023329,-0.1495,-0.21566,-0.2434,-0.24019,-0.22521,-0.20456,-0.18911,-0.1778,-0.17356,-0.16939,-0.16921,-0.16604,-0.1648,
                -0.16346,-0.16014,-0.15797,-0.16091,-0.16404,-0.16543,-0.168,-0.16571,-0.16356,-0.15949,-0.15549,-0.1505,-0.1495,-0.14559,-0.14673,-0.15086,-0.15696,
                -0.15613,-0.15531,-0.15256,-0.15079,-0.15099,-0.1521,-0.15509,-0.15601,-0.156,-0.155,-0.15597,-0.15299,-0.15097,-0.14699,-0.14114,-0.1394,-0.14079,-0.13847,
                -0.13744,-0.13674,-0.13353,-0.13476,-0.13917,-0.14149,-0.14329,-0.14111,-0.13551,-0.12983,-0.12651,-0.11926,-0.11,-0.095014,-0.076143,-0.057529,-0.043529,
                -0.033486,-0.023414,-0.016671,-0.0112,-0.0084143,-0.0084429,-0.0124,-0.015757,-0.018086,-0.020214,-0.020086,-0.022729,-0.026186,-0.028571,-0.026943,-0.028314,
                -0.030629,-0.034886,-0.0381,-0.040257,-0.042371,-0.041557,-0.040814,-0.044057,-0.048286,-0.049471,-0.048643,-0.047771,-0.046857,-0.048843,-0.048814,-0.047743,
                -0.044729,-0.037871,-0.029229,-0.024643,-0.016171,-0.0058143,0.0083571,0.022314,0.038014,0.046529,0.056857,0.065043,0.074086,0.080986,0.086814,0.087629,
                0.087471,0.087371,0.090229,0.094971,0.095714,0.094471,0.092257,0.090114,0.087986,0.091743,0.090486,0.086257,0.085057,0.082929,0.080843,0.083657,0.084414,
                0.079186,0.077957,0.075771,0.071643,0.073429,0.074114,0.071786,0.069443,0.072043,0.0736,0.074114,0.073557,0.069986,0.064443,0.0609,0.057386,0.054843,0.054229,
                0.049614,0.045057,0.042486,0.040871,0.037214,0.034514};

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            double[] rpeak = { 144 };

            Vector<double> rpeaks = Vector<double>.Build.DenseOfArray(rpeak);

            List<int> qrsOns = new List<int>();
            List<int> qrsEnds = new List<int>();
            List<int> POns = new List<int>();
            List<int> Pends = new List<int>();
            List<int> Tends = new List<int>();

            uint freq = 360;
            int offset = 0;

            Waves_Alg testAlgs = new Waves_Alg(testParams);
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { testVector, rpeaks, qrsOns, qrsEnds,
                            POns, Pends, Tends, offset, freq};

            obj.Invoke("analyzeSignalPart", args);

            List<int> qrsOnsOut = (List<int>)args[2];
            Assert.AreEqual(qrsOnsOut[0], 126, 10);

        }

        [TestMethod]
        [Description("Test if length of QRSonsets is the same as input Rpeaks")]
        public void FindQRSTestQRSonsetCount()
        {
            Waves_Params testParams = new Waves_Params(Wavelet_Type.db3, 2, "Analysis1", 500);

            double[] testArray = { 0.030343,0.032414,0.031457,0.032414,0.029371,0.027314,0.023343,0.025343,0.0263,0.025214,0.021129,0.018971,0.015814,0.011671,0.011443,
                0.0082143,0.0030571,-0.004,-0.007,-0.011914,-0.014814,-0.0177,-0.018614,-0.019529,-0.017443,-0.0134,-0.010429,-0.0094571,-0.013357,-0.015143,-0.017786,
                -0.0203,-0.022743,-0.023143,-0.030371,-0.033486,-0.033471,-0.030371,-0.0302,-0.029986,-0.033643,-0.036157,-0.037514,-0.035771,-0.032014,-0.029229,-0.031371,
                -0.035386,-0.036357,-0.035271,-0.031229,-0.025286,-0.021414,-0.0176,-0.015829,-0.012071,-0.010329,-0.0076429,-0.0079429,-0.0072714,-0.0066429,-0.0020857,
                -0.00058571,-0.00014286,-0.0016714,-0.0041571,-0.0066429,-0.0051857,0.00014286,0.0093143,0.015371,0.019414,0.0234,0.029329,0.033171,0.036943,0.046529,
                0.056014,0.0644,0.073643,0.0827,0.083714,0.080729,0.076743,0.070843,0.065971,0.063071,0.062086,0.058114,0.054129,0.051143,0.051114,0.054029,0.058857,
                0.056743,0.045843,0.032186,0.013814,-0.0073,-0.024186,-0.032971,-0.040671,-0.043357,-0.045014,-0.041814,-0.040671,-0.043486,-0.049171,-0.0528,-0.057357,
                -0.0619,-0.062471,-0.064043,-0.061629,-0.060186,-0.055771,-0.050471,-0.048271,-0.053029,-0.053771,-0.053471,-0.054086,-0.0507,-0.049286,-0.053786,-0.059157,
                -0.069357,-0.087257,-0.1049,-0.12523,-0.15313,-0.18153,-0.19469,-0.18593,-0.1487,-0.077514,0.026143,0.1717,0.34774,0.544,0.74039,0.9181,1.037,1.0741,1.013,
                0.85803,0.63816,0.39679,0.16481,-0.023329,-0.1495,-0.21566,-0.2434,-0.24019,-0.22521,-0.20456,-0.18911,-0.1778,-0.17356,-0.16939,-0.16921,-0.16604,-0.1648,
                -0.16346,-0.16014,-0.15797,-0.16091,-0.16404,-0.16543,-0.168,-0.16571,-0.16356,-0.15949,-0.15549,-0.1505,-0.1495,-0.14559,-0.14673,-0.15086,-0.15696,
                -0.15613,-0.15531,-0.15256,-0.15079,-0.15099,-0.1521,-0.15509,-0.15601,-0.156,-0.155,-0.15597,-0.15299,-0.15097,-0.14699,-0.14114,-0.1394,-0.14079,-0.13847,
                -0.13744,-0.13674,-0.13353,-0.13476,-0.13917,-0.14149,-0.14329,-0.14111,-0.13551,-0.12983,-0.12651,-0.11926,-0.11,-0.095014,-0.076143,-0.057529,-0.043529,
                -0.033486,-0.023414,-0.016671,-0.0112,-0.0084143,-0.0084429,-0.0124,-0.015757,-0.018086,-0.020214,-0.020086,-0.022729,-0.026186,-0.028571,-0.026943,-0.028314,
                -0.030629,-0.034886,-0.0381,-0.040257,-0.042371,-0.041557,-0.040814,-0.044057,-0.048286,-0.049471,-0.048643,-0.047771,-0.046857,-0.048843,-0.048814,-0.047743,
                -0.044729,-0.037871,-0.029229,-0.024643,-0.016171,-0.0058143,0.0083571,0.022314,0.038014,0.046529,0.056857,0.065043,0.074086,0.080986,0.086814,0.087629,
                0.087471,0.087371,0.090229,0.094971,0.095714,0.094471,0.092257,0.090114,0.087986,0.091743,0.090486,0.086257,0.085057,0.082929,0.080843,0.083657,0.084414,
                0.079186,0.077957,0.075771,0.071643,0.073429,0.074114,0.071786,0.069443,0.072043,0.0736,0.074114,0.073557,0.069986,0.064443,0.0609,0.057386,0.054843,0.054229,
                0.049614,0.045057,0.042486,0.040871,0.037214,0.034514};

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            double[] rpeak = { 144 , 148, 170, 200, 214};

            Vector<double> rpeaks = Vector<double>.Build.DenseOfArray(rpeak);

            List<int> qrsOns = new List<int>();
            List<int> qrsEnds = new List<int>();
            List<int> POns = new List<int>();
            List<int> Pends = new List<int>();
            List<int> Tends = new List<int>();

            uint freq = 360;
            int offset = 0;

            Waves_Alg testAlgs = new Waves_Alg(testParams);
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { testVector, rpeaks, qrsOns, qrsEnds,
                            POns, Pends, Tends, offset, freq};

            obj.Invoke("analyzeSignalPart", args);

            List<int> qrsOnsOut = (List<int>)args[2];
            Assert.AreEqual(qrsOnsOut.Count , rpeaks.Count);

        }

        [TestMethod]
        [Description("Test if QRSend is found for wavelet type: haar, decomposition level: 3")]
        public void FindQRSTestQRSendHaar3()
        {
            Waves_Params testParams = new Waves_Params(Wavelet_Type.db2, 3, "Analysis1", 500);

            double[] testArray = { 0.030343,0.032414,0.031457,0.032414,0.029371,0.027314,0.023343,0.025343,0.0263,0.025214,0.021129,0.018971,0.015814,0.011671,0.011443,
                0.0082143,0.0030571,-0.004,-0.007,-0.011914,-0.014814,-0.0177,-0.018614,-0.019529,-0.017443,-0.0134,-0.010429,-0.0094571,-0.013357,-0.015143,-0.017786,
                -0.0203,-0.022743,-0.023143,-0.030371,-0.033486,-0.033471,-0.030371,-0.0302,-0.029986,-0.033643,-0.036157,-0.037514,-0.035771,-0.032014,-0.029229,-0.031371,
                -0.035386,-0.036357,-0.035271,-0.031229,-0.025286,-0.021414,-0.0176,-0.015829,-0.012071,-0.010329,-0.0076429,-0.0079429,-0.0072714,-0.0066429,-0.0020857,
                -0.00058571,-0.00014286,-0.0016714,-0.0041571,-0.0066429,-0.0051857,0.00014286,0.0093143,0.015371,0.019414,0.0234,0.029329,0.033171,0.036943,0.046529,
                0.056014,0.0644,0.073643,0.0827,0.083714,0.080729,0.076743,0.070843,0.065971,0.063071,0.062086,0.058114,0.054129,0.051143,0.051114,0.054029,0.058857,
                0.056743,0.045843,0.032186,0.013814,-0.0073,-0.024186,-0.032971,-0.040671,-0.043357,-0.045014,-0.041814,-0.040671,-0.043486,-0.049171,-0.0528,-0.057357,
                -0.0619,-0.062471,-0.064043,-0.061629,-0.060186,-0.055771,-0.050471,-0.048271,-0.053029,-0.053771,-0.053471,-0.054086,-0.0507,-0.049286,-0.053786,-0.059157,
                -0.069357,-0.087257,-0.1049,-0.12523,-0.15313,-0.18153,-0.19469,-0.18593,-0.1487,-0.077514,0.026143,0.1717,0.34774,0.544,0.74039,0.9181,1.037,1.0741,1.013,
                0.85803,0.63816,0.39679,0.16481,-0.023329,-0.1495,-0.21566,-0.2434,-0.24019,-0.22521,-0.20456,-0.18911,-0.1778,-0.17356,-0.16939,-0.16921,-0.16604,-0.1648,
                -0.16346,-0.16014,-0.15797,-0.16091,-0.16404,-0.16543,-0.168,-0.16571,-0.16356,-0.15949,-0.15549,-0.1505,-0.1495,-0.14559,-0.14673,-0.15086,-0.15696,
                -0.15613,-0.15531,-0.15256,-0.15079,-0.15099,-0.1521,-0.15509,-0.15601,-0.156,-0.155,-0.15597,-0.15299,-0.15097,-0.14699,-0.14114,-0.1394,-0.14079,-0.13847,
                -0.13744,-0.13674,-0.13353,-0.13476,-0.13917,-0.14149,-0.14329,-0.14111,-0.13551,-0.12983,-0.12651,-0.11926,-0.11,-0.095014,-0.076143,-0.057529,-0.043529,
                -0.033486,-0.023414,-0.016671,-0.0112,-0.0084143,-0.0084429,-0.0124,-0.015757,-0.018086,-0.020214,-0.020086,-0.022729,-0.026186,-0.028571,-0.026943,-0.028314,
                -0.030629,-0.034886,-0.0381,-0.040257,-0.042371,-0.041557,-0.040814,-0.044057,-0.048286,-0.049471,-0.048643,-0.047771,-0.046857,-0.048843,-0.048814,-0.047743,
                -0.044729,-0.037871,-0.029229,-0.024643,-0.016171,-0.0058143,0.0083571,0.022314,0.038014,0.046529,0.056857,0.065043,0.074086,0.080986,0.086814,0.087629,
                0.087471,0.087371,0.090229,0.094971,0.095714,0.094471,0.092257,0.090114,0.087986,0.091743,0.090486,0.086257,0.085057,0.082929,0.080843,0.083657,0.084414,
                0.079186,0.077957,0.075771,0.071643,0.073429,0.074114,0.071786,0.069443,0.072043,0.0736,0.074114,0.073557,0.069986,0.064443,0.0609,0.057386,0.054843,0.054229,
                0.049614,0.045057,0.042486,0.040871,0.037214,0.034514};

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            double[] rpeak = { 144};

            Vector<double> rpeaks = Vector<double>.Build.DenseOfArray(rpeak);

            List<int> qrsOns = new List<int>();
            List<int> qrsEnds = new List<int>();
            List<int> POns = new List<int>();
            List<int> Pends = new List<int>();
            List<int> Tends = new List<int>();

            uint freq = 360;
            int offset = 0;

            Waves_Alg testAlgs = new Waves_Alg(testParams);
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { testVector, rpeaks, qrsOns, qrsEnds,
                            POns, Pends, Tends, offset, freq};

            obj.Invoke("analyzeSignalPart", args);

            List<int> qrsEndsOut = (List<int>)args[3];
            Assert.AreEqual(qrsEndsOut[0], 160, 10);

        }

        [TestMethod]
        [Description("Test if QRSend is found for wavelet type: haar, decomposition level: 3")]
        public void FindQRSTestQRSenddb2_2()
        {
            Waves_Params testParams = new Waves_Params(Wavelet_Type.db2, 2, "Analysis1", 500);

            double[] testArray = { 0.030343,0.032414,0.031457,0.032414,0.029371,0.027314,0.023343,0.025343,0.0263,0.025214,0.021129,0.018971,0.015814,0.011671,0.011443,
                0.0082143,0.0030571,-0.004,-0.007,-0.011914,-0.014814,-0.0177,-0.018614,-0.019529,-0.017443,-0.0134,-0.010429,-0.0094571,-0.013357,-0.015143,-0.017786,
                -0.0203,-0.022743,-0.023143,-0.030371,-0.033486,-0.033471,-0.030371,-0.0302,-0.029986,-0.033643,-0.036157,-0.037514,-0.035771,-0.032014,-0.029229,-0.031371,
                -0.035386,-0.036357,-0.035271,-0.031229,-0.025286,-0.021414,-0.0176,-0.015829,-0.012071,-0.010329,-0.0076429,-0.0079429,-0.0072714,-0.0066429,-0.0020857,
                -0.00058571,-0.00014286,-0.0016714,-0.0041571,-0.0066429,-0.0051857,0.00014286,0.0093143,0.015371,0.019414,0.0234,0.029329,0.033171,0.036943,0.046529,
                0.056014,0.0644,0.073643,0.0827,0.083714,0.080729,0.076743,0.070843,0.065971,0.063071,0.062086,0.058114,0.054129,0.051143,0.051114,0.054029,0.058857,
                0.056743,0.045843,0.032186,0.013814,-0.0073,-0.024186,-0.032971,-0.040671,-0.043357,-0.045014,-0.041814,-0.040671,-0.043486,-0.049171,-0.0528,-0.057357,
                -0.0619,-0.062471,-0.064043,-0.061629,-0.060186,-0.055771,-0.050471,-0.048271,-0.053029,-0.053771,-0.053471,-0.054086,-0.0507,-0.049286,-0.053786,-0.059157,
                -0.069357,-0.087257,-0.1049,-0.12523,-0.15313,-0.18153,-0.19469,-0.18593,-0.1487,-0.077514,0.026143,0.1717,0.34774,0.544,0.74039,0.9181,1.037,1.0741,1.013,
                0.85803,0.63816,0.39679,0.16481,-0.023329,-0.1495,-0.21566,-0.2434,-0.24019,-0.22521,-0.20456,-0.18911,-0.1778,-0.17356,-0.16939,-0.16921,-0.16604,-0.1648,
                -0.16346,-0.16014,-0.15797,-0.16091,-0.16404,-0.16543,-0.168,-0.16571,-0.16356,-0.15949,-0.15549,-0.1505,-0.1495,-0.14559,-0.14673,-0.15086,-0.15696,
                -0.15613,-0.15531,-0.15256,-0.15079,-0.15099,-0.1521,-0.15509,-0.15601,-0.156,-0.155,-0.15597,-0.15299,-0.15097,-0.14699,-0.14114,-0.1394,-0.14079,-0.13847,
                -0.13744,-0.13674,-0.13353,-0.13476,-0.13917,-0.14149,-0.14329,-0.14111,-0.13551,-0.12983,-0.12651,-0.11926,-0.11,-0.095014,-0.076143,-0.057529,-0.043529,
                -0.033486,-0.023414,-0.016671,-0.0112,-0.0084143,-0.0084429,-0.0124,-0.015757,-0.018086,-0.020214,-0.020086,-0.022729,-0.026186,-0.028571,-0.026943,-0.028314,
                -0.030629,-0.034886,-0.0381,-0.040257,-0.042371,-0.041557,-0.040814,-0.044057,-0.048286,-0.049471,-0.048643,-0.047771,-0.046857,-0.048843,-0.048814,-0.047743,
                -0.044729,-0.037871,-0.029229,-0.024643,-0.016171,-0.0058143,0.0083571,0.022314,0.038014,0.046529,0.056857,0.065043,0.074086,0.080986,0.086814,0.087629,
                0.087471,0.087371,0.090229,0.094971,0.095714,0.094471,0.092257,0.090114,0.087986,0.091743,0.090486,0.086257,0.085057,0.082929,0.080843,0.083657,0.084414,
                0.079186,0.077957,0.075771,0.071643,0.073429,0.074114,0.071786,0.069443,0.072043,0.0736,0.074114,0.073557,0.069986,0.064443,0.0609,0.057386,0.054843,0.054229,
                0.049614,0.045057,0.042486,0.040871,0.037214,0.034514};

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            double[] rpeak = { 144 };

            Vector<double> rpeaks = Vector<double>.Build.DenseOfArray(rpeak);

            List<int> qrsOns = new List<int>();
            List<int> qrsEnds = new List<int>();
            List<int> POns = new List<int>();
            List<int> Pends = new List<int>();
            List<int> Tends = new List<int>();

            uint freq = 360;
            int offset = 0;

            Waves_Alg testAlgs = new Waves_Alg(testParams);
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { testVector, rpeaks, qrsOns, qrsEnds,
                            POns, Pends, Tends, offset, freq};

            obj.Invoke("analyzeSignalPart", args);

            List<int> qrsEndsOut = (List<int>)args[3];
            Assert.AreEqual(qrsEndsOut[0], 160, 10);

        }

        [TestMethod]
        [Description("Test if QRSend is found for wavelet type: haar, decomposition level: 3")]
        public void FindQRSTestQRSendDb3_1()
        {
            Waves_Params testParams = new Waves_Params(Wavelet_Type.db3, 1, "Analysis1", 500);

            double[] testArray = { 0.030343,0.032414,0.031457,0.032414,0.029371,0.027314,0.023343,0.025343,0.0263,0.025214,0.021129,0.018971,0.015814,0.011671,0.011443,
                0.0082143,0.0030571,-0.004,-0.007,-0.011914,-0.014814,-0.0177,-0.018614,-0.019529,-0.017443,-0.0134,-0.010429,-0.0094571,-0.013357,-0.015143,-0.017786,
                -0.0203,-0.022743,-0.023143,-0.030371,-0.033486,-0.033471,-0.030371,-0.0302,-0.029986,-0.033643,-0.036157,-0.037514,-0.035771,-0.032014,-0.029229,-0.031371,
                -0.035386,-0.036357,-0.035271,-0.031229,-0.025286,-0.021414,-0.0176,-0.015829,-0.012071,-0.010329,-0.0076429,-0.0079429,-0.0072714,-0.0066429,-0.0020857,
                -0.00058571,-0.00014286,-0.0016714,-0.0041571,-0.0066429,-0.0051857,0.00014286,0.0093143,0.015371,0.019414,0.0234,0.029329,0.033171,0.036943,0.046529,
                0.056014,0.0644,0.073643,0.0827,0.083714,0.080729,0.076743,0.070843,0.065971,0.063071,0.062086,0.058114,0.054129,0.051143,0.051114,0.054029,0.058857,
                0.056743,0.045843,0.032186,0.013814,-0.0073,-0.024186,-0.032971,-0.040671,-0.043357,-0.045014,-0.041814,-0.040671,-0.043486,-0.049171,-0.0528,-0.057357,
                -0.0619,-0.062471,-0.064043,-0.061629,-0.060186,-0.055771,-0.050471,-0.048271,-0.053029,-0.053771,-0.053471,-0.054086,-0.0507,-0.049286,-0.053786,-0.059157,
                -0.069357,-0.087257,-0.1049,-0.12523,-0.15313,-0.18153,-0.19469,-0.18593,-0.1487,-0.077514,0.026143,0.1717,0.34774,0.544,0.74039,0.9181,1.037,1.0741,1.013,
                0.85803,0.63816,0.39679,0.16481,-0.023329,-0.1495,-0.21566,-0.2434,-0.24019,-0.22521,-0.20456,-0.18911,-0.1778,-0.17356,-0.16939,-0.16921,-0.16604,-0.1648,
                -0.16346,-0.16014,-0.15797,-0.16091,-0.16404,-0.16543,-0.168,-0.16571,-0.16356,-0.15949,-0.15549,-0.1505,-0.1495,-0.14559,-0.14673,-0.15086,-0.15696,
                -0.15613,-0.15531,-0.15256,-0.15079,-0.15099,-0.1521,-0.15509,-0.15601,-0.156,-0.155,-0.15597,-0.15299,-0.15097,-0.14699,-0.14114,-0.1394,-0.14079,-0.13847,
                -0.13744,-0.13674,-0.13353,-0.13476,-0.13917,-0.14149,-0.14329,-0.14111,-0.13551,-0.12983,-0.12651,-0.11926,-0.11,-0.095014,-0.076143,-0.057529,-0.043529,
                -0.033486,-0.023414,-0.016671,-0.0112,-0.0084143,-0.0084429,-0.0124,-0.015757,-0.018086,-0.020214,-0.020086,-0.022729,-0.026186,-0.028571,-0.026943,-0.028314,
                -0.030629,-0.034886,-0.0381,-0.040257,-0.042371,-0.041557,-0.040814,-0.044057,-0.048286,-0.049471,-0.048643,-0.047771,-0.046857,-0.048843,-0.048814,-0.047743,
                -0.044729,-0.037871,-0.029229,-0.024643,-0.016171,-0.0058143,0.0083571,0.022314,0.038014,0.046529,0.056857,0.065043,0.074086,0.080986,0.086814,0.087629,
                0.087471,0.087371,0.090229,0.094971,0.095714,0.094471,0.092257,0.090114,0.087986,0.091743,0.090486,0.086257,0.085057,0.082929,0.080843,0.083657,0.084414,
                0.079186,0.077957,0.075771,0.071643,0.073429,0.074114,0.071786,0.069443,0.072043,0.0736,0.074114,0.073557,0.069986,0.064443,0.0609,0.057386,0.054843,0.054229,
                0.049614,0.045057,0.042486,0.040871,0.037214,0.034514};

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            double[] rpeak = { 144 };

            Vector<double> rpeaks = Vector<double>.Build.DenseOfArray(rpeak);

            List<int> qrsOns = new List<int>();
            List<int> qrsEnds = new List<int>();
            List<int> POns = new List<int>();
            List<int> Pends = new List<int>();
            List<int> Tends = new List<int>();

            uint freq = 360;
            int offset = 0;

            Waves_Alg testAlgs = new Waves_Alg(testParams);
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { testVector, rpeaks, qrsOns, qrsEnds,
                            POns, Pends, Tends, offset, freq};

            obj.Invoke("analyzeSignalPart", args);

            List<int> qrsEndsOut = (List<int>)args[3];
            Assert.AreEqual(qrsEndsOut[0], 160, 10);

        }

        [TestMethod]
        [Description("Test if length of QRSends list is the same as input rpeaks ")]
        public void FindQRSTestQRSendCount()
        {
            Waves_Params testParams = new Waves_Params(Wavelet_Type.db2, 3, "Analysis1", 500);

            double[] testArray = { 0.030343,0.032414,0.031457,0.032414,0.029371,0.027314,0.023343,0.025343,0.0263,0.025214,0.021129,0.018971,0.015814,0.011671,0.011443,
                0.0082143,0.0030571,-0.004,-0.007,-0.011914,-0.014814,-0.0177,-0.018614,-0.019529,-0.017443,-0.0134,-0.010429,-0.0094571,-0.013357,-0.015143,-0.017786,
                -0.0203,-0.022743,-0.023143,-0.030371,-0.033486,-0.033471,-0.030371,-0.0302,-0.029986,-0.033643,-0.036157,-0.037514,-0.035771,-0.032014,-0.029229,-0.031371,
                -0.035386,-0.036357,-0.035271,-0.031229,-0.025286,-0.021414,-0.0176,-0.015829,-0.012071,-0.010329,-0.0076429,-0.0079429,-0.0072714,-0.0066429,-0.0020857,
                -0.00058571,-0.00014286,-0.0016714,-0.0041571,-0.0066429,-0.0051857,0.00014286,0.0093143,0.015371,0.019414,0.0234,0.029329,0.033171,0.036943,0.046529,
                0.056014,0.0644,0.073643,0.0827,0.083714,0.080729,0.076743,0.070843,0.065971,0.063071,0.062086,0.058114,0.054129,0.051143,0.051114,0.054029,0.058857,
                0.056743,0.045843,0.032186,0.013814,-0.0073,-0.024186,-0.032971,-0.040671,-0.043357,-0.045014,-0.041814,-0.040671,-0.043486,-0.049171,-0.0528,-0.057357,
                -0.0619,-0.062471,-0.064043,-0.061629,-0.060186,-0.055771,-0.050471,-0.048271,-0.053029,-0.053771,-0.053471,-0.054086,-0.0507,-0.049286,-0.053786,-0.059157,
                -0.069357,-0.087257,-0.1049,-0.12523,-0.15313,-0.18153,-0.19469,-0.18593,-0.1487,-0.077514,0.026143,0.1717,0.34774,0.544,0.74039,0.9181,1.037,1.0741,1.013,
                0.85803,0.63816,0.39679,0.16481,-0.023329,-0.1495,-0.21566,-0.2434,-0.24019,-0.22521,-0.20456,-0.18911,-0.1778,-0.17356,-0.16939,-0.16921,-0.16604,-0.1648,
                -0.16346,-0.16014,-0.15797,-0.16091,-0.16404,-0.16543,-0.168,-0.16571,-0.16356,-0.15949,-0.15549,-0.1505,-0.1495,-0.14559,-0.14673,-0.15086,-0.15696,
                -0.15613,-0.15531,-0.15256,-0.15079,-0.15099,-0.1521,-0.15509,-0.15601,-0.156,-0.155,-0.15597,-0.15299,-0.15097,-0.14699,-0.14114,-0.1394,-0.14079,-0.13847,
                -0.13744,-0.13674,-0.13353,-0.13476,-0.13917,-0.14149,-0.14329,-0.14111,-0.13551,-0.12983,-0.12651,-0.11926,-0.11,-0.095014,-0.076143,-0.057529,-0.043529,
                -0.033486,-0.023414,-0.016671,-0.0112,-0.0084143,-0.0084429,-0.0124,-0.015757,-0.018086,-0.020214,-0.020086,-0.022729,-0.026186,-0.028571,-0.026943,-0.028314,
                -0.030629,-0.034886,-0.0381,-0.040257,-0.042371,-0.041557,-0.040814,-0.044057,-0.048286,-0.049471,-0.048643,-0.047771,-0.046857,-0.048843,-0.048814,-0.047743,
                -0.044729,-0.037871,-0.029229,-0.024643,-0.016171,-0.0058143,0.0083571,0.022314,0.038014,0.046529,0.056857,0.065043,0.074086,0.080986,0.086814,0.087629,
                0.087471,0.087371,0.090229,0.094971,0.095714,0.094471,0.092257,0.090114,0.087986,0.091743,0.090486,0.086257,0.085057,0.082929,0.080843,0.083657,0.084414,
                0.079186,0.077957,0.075771,0.071643,0.073429,0.074114,0.071786,0.069443,0.072043,0.0736,0.074114,0.073557,0.069986,0.064443,0.0609,0.057386,0.054843,0.054229,
                0.049614,0.045057,0.042486,0.040871,0.037214,0.034514};

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            double[] rpeak = { 144 , 150, 160, 180};

            Vector<double> rpeaks = Vector<double>.Build.DenseOfArray(rpeak);

            List<int> qrsOns = new List<int>();
            List<int> qrsEnds = new List<int>();
            List<int> POns = new List<int>();
            List<int> Pends = new List<int>();
            List<int> Tends = new List<int>();

            uint freq = 360;
            int offset = 0;

            Waves_Alg testAlgs = new Waves_Alg(testParams);
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { testVector, rpeaks, qrsOns, qrsEnds,
                            POns, Pends, Tends, offset, freq};

            obj.Invoke("analyzeSignalPart", args);

            List<int> qrsEndsOut = (List<int>)args[3];
            Assert.AreEqual(qrsEndsOut.Count , rpeaks.Count);

        }

        [TestMethod]
        [Description("Test if method finds location of Ponset in vector")]
        public void FindPTest1()
        {
            Waves_Params testParams = new Waves_Params(Wavelet_Type.haar, 3, "Analysis1", 500);

            double[] testArray = {0.030343,0.032414,0.031457,0.032414,0.029371,0.027314,0.023343,0.025343,0.0263,0.025214,0.021129,0.018971,0.015814,0.011671,0.011443,
                0.0082143,0.0030571,-0.004,-0.007,-0.011914,-0.014814,-0.0177,-0.018614,-0.019529,-0.017443,-0.0134,-0.010429,-0.0094571,-0.013357,-0.015143,-0.017786,
                -0.0203,-0.022743,-0.023143,-0.030371,-0.033486,-0.033471,-0.030371,-0.0302,-0.029986,-0.033643,-0.036157,-0.037514,-0.035771,-0.032014,-0.029229,-0.031371,
                -0.035386,-0.036357,-0.035271,-0.031229,-0.025286,-0.021414,-0.0176,-0.015829,-0.012071,-0.010329,-0.0076429,-0.0079429,-0.0072714,-0.0066429,-0.0020857,
                -0.00058571,-0.00014286,-0.0016714,-0.0041571,-0.0066429,-0.0051857,0.00014286,0.0093143,0.015371,0.019414,0.0234,0.029329,0.033171,0.036943,0.046529,
                0.056014,0.0644,0.073643,0.0827,0.083714,0.080729,0.076743,0.070843,0.065971,0.063071,0.062086,0.058114,0.054129,0.051143,0.051114,0.054029,0.058857,
                0.056743,0.045843,0.032186,0.013814,-0.0073,-0.024186,-0.032971,-0.040671,-0.043357,-0.045014,-0.041814,-0.040671,-0.043486,-0.049171,-0.0528,-0.057357,
                -0.0619,-0.062471,-0.064043,-0.061629,-0.060186,-0.055771,-0.050471,-0.048271,-0.053029,-0.053771,-0.0535};
            List<int> result = new List<int>() { 66 };
            uint frequency = 360;
            int offset = 0;
            List<int> QRSonset = new List<int>() { 119 };
            List<int> Ponset = new List<int>();
            List<int> Pend = new List<int>();

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            Waves_Alg testAlgs = new Waves_Alg(testParams);
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { frequency, offset, QRSonset, testVector, Ponset, Pend };

            obj.Invoke("FindP", args);

            Assert.AreEqual(args[4], result);


        }

        [TestMethod]
        [Description("Test if method finds location of Pends in vector")]
        public void FindPTest2()
        {
            Waves_Params testParams = new Waves_Params(Wavelet_Type.haar, 3, "Analysis1", 500);

            double[] testArray = {0.030343,0.032414,0.031457,0.032414,0.029371,0.027314,0.023343,0.025343,0.0263,0.025214,0.021129,0.018971,0.015814,0.011671,0.011443,
                0.0082143,0.0030571,-0.004,-0.007,-0.011914,-0.014814,-0.0177,-0.018614,-0.019529,-0.017443,-0.0134,-0.010429,-0.0094571,-0.013357,-0.015143,-0.017786,
                -0.0203,-0.022743,-0.023143,-0.030371,-0.033486,-0.033471,-0.030371,-0.0302,-0.029986,-0.033643,-0.036157,-0.037514,-0.035771,-0.032014,-0.029229,-0.031371,
                -0.035386,-0.036357,-0.035271,-0.031229,-0.025286,-0.021414,-0.0176,-0.015829,-0.012071,-0.010329,-0.0076429,-0.0079429,-0.0072714,-0.0066429,-0.0020857,
                -0.00058571,-0.00014286,-0.0016714,-0.0041571,-0.0066429,-0.0051857,0.00014286,0.0093143,0.015371,0.019414,0.0234,0.029329,0.033171,0.036943,0.046529,
                0.056014,0.0644,0.073643,0.0827,0.083714,0.080729,0.076743,0.070843,0.065971,0.063071,0.062086,0.058114,0.054129,0.051143,0.051114,0.054029,0.058857,
                0.056743,0.045843,0.032186,0.013814,-0.0073,-0.024186,-0.032971,-0.040671,-0.043357,-0.045014,-0.041814,-0.040671,-0.043486,-0.049171,-0.0528,-0.057357,
                -0.0619,-0.062471,-0.064043,-0.061629,-0.060186,-0.055771,-0.050471,-0.048271,-0.053029,-0.053771,-0.0535};
            List<int> result = new List<int>() { 104 };
            uint frequency = 360;
            int offset = 0;
            List<int> QRSonset = new List<int>() { 119 };
            List<int> Ponset = new List<int>();
            List<int> Pend = new List<int>();

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            Waves_Alg testAlgs = new Waves_Alg(testParams);
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { frequency, offset, QRSonset, testVector, Ponset, Pend };

            obj.Invoke("FindP", args);

            Assert.AreEqual(args[5], result);


        }

        [TestMethod]
        [Description("Test if method finds location of Tends in vector")]
        public void FindTTest1()
        {
            Waves_Params testParams = new Waves_Params(Wavelet_Type.haar, 3, "Analysis1", 500);

            double[] testArray = {-0.158,-0.16091,-0.16404,-0.16543,-0.168,-0.16571,-0.16356,-0.15949,-0.15549,-0.1505,-0.1495,-0.14559,-0.14673,-0.15086,-0.15696,-0.15613,
                -0.15531,-0.15256,-0.15079,-0.15099,-0.1521,-0.15509,-0.15601,-0.156,-0.155,-0.15597,-0.15299,-0.15097,-0.14699,-0.14114,-0.1394,-0.14079,-0.13847,
                -0.13744,-0.13674,-0.13353,-0.13476,-0.13917,-0.14149,-0.14329,-0.14111,-0.13551,-0.12983,-0.12651,-0.11926,-0.11,-0.095014,-0.076143,-0.057529,
                -0.043529,-0.033486,-0.023414,-0.016671,-0.0112,-0.0084143,-0.0084429,-0.0124,-0.015757,-0.018086,-0.020214,-0.020086,-0.022729,-0.026186,-0.028571,
                -0.026943,-0.028314,-0.030629,-0.034886,-0.0381,-0.040257,-0.042371,-0.041557,-0.040814,-0.044057,-0.048286,-0.049471,-0.048643,-0.047771,-0.046857,
                -0.048843,-0.048814,-0.047743,-0.044729,-0.037871,-0.029229,-0.024643,-0.016171,-0.0058143,0.0083571,0.022314,0.038014,0.046529,0.056857,0.065043,
                0.074086,0.080986,0.086814,0.087629,0.087471,0.087371,0.090229,0.094971,0.095714,0.094471,0.092257,0.090114,0.087986,0.091743,0.090486,0.086257,
                0.085057,0.082929,0.080843,0.083657,0.084414,0.079186,0.077957,0.075771,0.071643,0.073429,0.074114,0.071786,0.069443,0.072043,0.0736,0.074114,0.073557,
                0.069986,0.064443,0.0609,0.057386,0.054843,0.054229,0.049614,0.045057,0.042486,0.040871,0.037214,0.034514};
            List<int> result = new List<int>() { 75 };
            uint frequency = 360;
            int offset = 0;
            List<int> QRSend = new List<int>() { 1 };
            List<int> Tend = new List<int>();

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);

            Waves_Alg testAlgs = new Waves_Alg(testParams);
            PrivateObject obj = new PrivateObject(testAlgs);
            object[] args = { frequency, QRSend, offset, testVector, Tend };

            obj.Invoke("FindT", args);

            Assert.AreEqual(args[4], result);


        }
    }
}
