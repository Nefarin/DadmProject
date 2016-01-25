using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.ECG_Baseline;

namespace EKG_Unit.Modules.ECG_Baseline
{
    [TestClass]
    public class ECG_Baseline_Alg_Test
    {
        [TestMethod]
        [Description("Test if butterworth LP method returns proper values")]
        public void ButterworthLowPassTest()
        {
            double[] testArray = { 1, 2, 8, 5, 1, 3, 6, 12, 23, 5, 8, 19, 5, 8, 5, 4, 6, 5, 84, 85 };
            double[] resultArray = { 32.3643282005727, 20.0758852247596, 8.91923822080578, 2.41187284324484, 1.15437187737426, 3.50427665689673, 7.12650149527923, 10.2652306405650, 12.1552193306381, 12.6835991096352, 11.8538265613178, 9.66629432623774, 6.61987509331262, 4.28500933452372, 5.07436177505664, 10.8990754278231, 21.3952095102152, 33.1182767621193, 40.8370664692867, 40.5904809431651 };

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            for(int i = 0; i < resultVector.Count; i++)
            {
                resultVector[i] = System.Math.Round(resultVector[i], 3);
            }

            ECG_Baseline_Alg.Filter target = new ECG_Baseline_Alg.Filter();
            Vector<double> actual = target.butterworth(testVector, 360, 50, 10, Filtr_Type.LOWPASS);

            for (int i = 0; i < actual.Count; i++)
            {
                actual[i] = System.Math.Round(actual[i], 3);
            }

            System.Console.WriteLine(actual.ToString());
            Assert.AreEqual(resultVector, actual);
        }

        [TestMethod]
        [Description("Test if butterworth HP method returns proper values")]
        public void ButterworthHighPassTest()
        {
            double[] testArray = { 1, 2, 8, 5, 1, 3, 6, 12, 23, 5, 8, 19, 5, 8, 5, 4, 6, 5, 84, 85 };
            double[] resultArray = { -31.3643282005727, -18.0758852247596, -0.919238220805767, 2.58812715675517, -0.154371877374255, -0.504276656896726, -1.12650149527924, 1.73476935943496, 10.8447806693619, -7.68359910963517, -3.85382656131779, 9.33370567376227, -1.61987509331262, 3.71499066547628, -0.0743617750566429, -6.89907542782311, -15.3952095102152, -28.1182767621193, 43.1629335307133, 44.4095190568349 };

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            for (int i = 0; i < resultVector.Count; i++)
            {
                resultVector[i] = System.Math.Round(resultVector[i], 3);
            }

            ECG_Baseline_Alg.Filter target = new ECG_Baseline_Alg.Filter();
            Vector<double> actual = target.butterworth(testVector, 360, 50, 10, Filtr_Type.HIGHPASS);

            for (int i = 0; i < actual.Count; i++)
            {
                actual[i] = System.Math.Round(actual[i], 3);
            }

            System.Console.WriteLine(actual.ToString());
            Assert.AreEqual(resultVector, actual);
        }

        [TestMethod]
        [Description("Test if butterworth BP method returns proper values")]
        public void ButterworthBandPassTest()
        {
            double[] testArray = { 1, 2, 8, 5, 1, 3, 6, 12, 23, 5, 8, 19, 5, 8, 5, 4, 6, 5, 84, 85 };
            double[] resultArray = { 32.3643282005727, 20.0758852247596, 8.91923822080578, 2.41187284324484, 1.15437187737426, 3.50427665689673, 7.12650149527923, 10.2652306405650, 12.1552193306381, 12.6835991096352, 11.8538265613178, 9.66629432623774, 6.61987509331262, 4.28500933452372, 5.07436177505664, 10.8990754278231, 21.3952095102152, 33.1182767621193, 40.8370664692867, 40.5904809431651 };

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            for (int i = 0; i < resultVector.Count; i++)
            {
                resultVector[i] = System.Math.Round(resultVector[i], 3);
            }

            ECG_Baseline_Alg.Filter target = new ECG_Baseline_Alg.Filter();
            Vector<double> actual = target.butterworth(testVector, 360, 50, 10, Filtr_Type.BANDPASS);

            for (int i = 0; i < actual.Count; i++)
            {
                actual[i] = System.Math.Round(actual[i], 3);
            }

            System.Console.WriteLine(actual.ToString());
            Assert.AreEqual(resultVector, actual);
        }

        [TestMethod]
        [Description("Test if savitzky-golay LP method returns proper values")]
        public void SavitzkyGolayLowPassTest()
        {
            double[] testArray = { 1, 2, 8, 5, 1, 3, 6, 12, 23, 5, 8, 19, 5, 8, 5, 4, 6, 5, 84, 85 };
            double[] resultArray = { 2.65800865800865, 3.38528138528139, 3.62337662337663, 3.46753246753248, 3.15584415584407, 6.51515151515139, 8.24242424242408, 9.55844155844155, 13.0000000000000, 13.4242424242422, 12.5930735930738, 10.3290043290043, 7.69264069264068, 8.20346320346346, -0.969696969700635, 1.72727272727161, 16.1168831168861, 34.3419913419930, 55.0692640692650, 73.8354978354992 };

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            for (int i = 0; i < resultVector.Count; i++)
            {
                resultVector[i] = System.Math.Round(resultVector[i], 3);
            }

            ECG_Baseline_Alg.Filter target = new ECG_Baseline_Alg.Filter();
            Vector<double> actual = target.savitzky_golay(testVector, 9, Filtr_Type.LOWPASS);

            for (int i = 0; i < actual.Count; i++)
            {
                actual[i] = System.Math.Round(actual[i], 3);
            }

            System.Console.WriteLine(actual.ToString());
            Assert.AreEqual(resultVector, actual);
        }

        [TestMethod]
        [Description("Test if savitzky-golay HP method returns proper values")]
        public void SavitzkyGolayHighPassTest()
        {
            double[] testArray = { 1, 2, 8, 5, 1, 3, 6, 12, 23, 5, 8, 19, 5, 8, 5, 4, 6, 5, 84, 85 };
            double[] resultArray = { -9.91396403582741, -10.5764523554482, -6.29724001185127, -11.0724981197438, -16.8983977938328, -16.7711101488251, -15.6868062994279, -11.6416573603483, -2.63183444629301, -22.6535086719694, -21.7028511520843, -12.7760330013446, -28.8692253344577, -27.9785992661304, -33.1003259110696, -36.2305763839825, -36.3655217995760, -39.5013332725574, 37.3658180823667, 36.2397611504889 };

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            for (int i = 0; i < resultVector.Count; i++)
            {
                resultVector[i] = System.Math.Round(resultVector[i], 3);
            }

            ECG_Baseline_Alg.Filter target = new ECG_Baseline_Alg.Filter();
            Vector<double> actual = target.savitzky_golay(testVector, 87, Filtr_Type.HIGHPASS);

            for (int i = 0; i < actual.Count; i++)
            {
                actual[i] = System.Math.Round(actual[i], 3);
            }

            System.Console.WriteLine(actual.ToString());
            Assert.AreEqual(resultVector, actual);
        }
    }
}
