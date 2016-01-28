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
            double[] resultArray = { 2.45744345557991, -1.68204293128406, -4.27469867695616, -4.24472161824094, -2.31069580502097, -0.0357445380053719, 1.37593994217652, 1.70025470062549, 1.50904914030757, 1.47256308073577, 1.67832105808639, 1.49708669351868, 0.199765670238396, -2.13600411222992, -4.26956456906150, -4.47305241737448, -1.91645913917522, 2.29841061457380, 5.55089765485847, 5.60325199381815 };

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            for (int i = 0; i < resultVector.Count; i++)
            {
                resultVector[i] = System.Math.Round(resultVector[i], 3);
            }

            ECG_Baseline_Alg.Filter target = new ECG_Baseline_Alg.Filter();
            Vector<double> actual = target.butterworth(testVector, 360, 50, 10, 50, 10, Filtr_Type.BANDPASS);

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

        [TestMethod]
        [Description("Test if savitzky-golay BP method returns proper values")]
        public void SavitzkyGolayBandPassTest()
        {
            double[] testArray = { 1, 2, 8, 5, 1, 3, 6, 12, 23, 5, 8, 19, 5, 8, 5, 4, 6, 5, 84, 85 };
            double[] resultArray = { -8.34395037998317, -9.03055382978838, -10.2557444378903, -11.9210396990331, -13.7850999651036, -12.0178409992440, -11.9190486944668, -12.2646994199750, -10.5147522289540, -11.8088198542433, -14.3817531239193, -18.4077968054546, -22.8231004282246, -24.1049390627322, -35.0812371301258, -34.1946376662769, -21.6193854386529, -5.20937889437704, 13.7052930432130, 30.6646713209614 };

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            for (int i = 0; i < resultVector.Count; i++)
            {
                resultVector[i] = System.Math.Round(resultVector[i], 3);
            }

            ECG_Baseline_Alg.Filter target = new ECG_Baseline_Alg.Filter();
            Vector<double> actual = target.savitzky_golay(testVector, 9, 87, Filtr_Type.BANDPASS);

            for (int i = 0; i < actual.Count; i++)
            {
                actual[i] = System.Math.Round(actual[i], 3);
            }

            System.Console.WriteLine(actual.ToString());
            Assert.AreEqual(resultVector, actual);
        }

        [TestMethod]
        [Description("Test if moving average LP method returns proper values")]
        public void MovingAverageLowPassTest()
        {
            double[] testArray = { 1, 2, 8, 5, 1, 3, 6, 12, 23, 5, 8, 19, 5, 8, 5, 4, 6, 5, 84, 85 };
            double[] resultArray = { 1, 1.11111111111111, 1.88888888888889, 2.33333333333333, 2.33333333333333, 2.55555555555556, 3.11111111111111, 4.33333333333333, 6.77777777777778, 7.22222222222222, 7.88888888888889, 9.11111111111111, 9.11111111111111, 9.88888888888889, 10.1111111111111, 9.88888888888889, 9.22222222222222, 7.22222222222222, 16, 24.5555555555556 };

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            for (int i = 0; i < resultVector.Count; i++)
            {
                resultVector[i] = System.Math.Round(resultVector[i], 3);
            }

            ECG_Baseline_Alg.Filter target = new ECG_Baseline_Alg.Filter();
            Vector<double> actual = target.moving_average(testVector, 9, Filtr_Type.LOWPASS);

            for (int i = 0; i < actual.Count; i++)
            {
                actual[i] = System.Math.Round(actual[i], 3);
            }

            System.Console.WriteLine(actual.ToString());
            Assert.AreEqual(resultVector, actual);
        }

        [TestMethod]
        [Description("Test if moving average HP method returns proper values")]
        public void MovingAverageHighPassTest()
        {
            double[] testArray = { 1, 2, 8, 5, 1, 3, 6, 12, 23, 5, 8, 19, 5, 8, 5, 4, 6, 5, 84, 85 };
            double[] resultArray = { 0, 0.988505747126437, 6.90804597701149, 3.86206896551724, -0.137931034482759, 1.83908045977012, 4.78160919540230, 10.6551724137931, 21.4022988505747, 3.35632183908046, 6.27586206896552, 17.0689655172414, 3.02298850574713, 5.94252873563218, 2.89655172413793, 1.86206896551724, 3.80459770114943, 2.75862068965517, 80.8045977011494, 80.8390804597701 };

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            for (int i = 0; i < resultVector.Count; i++)
            {
                resultVector[i] = System.Math.Round(resultVector[i], 3);
            }

            ECG_Baseline_Alg.Filter target = new ECG_Baseline_Alg.Filter();
            Vector<double> actual = target.moving_average(testVector, 87, Filtr_Type.HIGHPASS);

            for (int i = 0; i < actual.Count; i++)
            {
                actual[i] = System.Math.Round(actual[i], 3);
            }

            System.Console.WriteLine(actual.ToString());
            Assert.AreEqual(resultVector, actual);
        }

        [TestMethod]
        [Description("Test if moving average BP method returns proper values")]
        public void MovingAverageBandPassTest()
        {
            double[] testArray = { 1, 2, 8, 5, 1, 3, 6, 12, 23, 5, 8, 19, 5, 8, 5, 4, 6, 5, 84, 85 };
            double[] resultArray = { 0,0.109833971902938,0.877394636015326,1.30651340996169,1.29118773946360,1.49553001277139,2.02681992337165,3.21072796934866,5.58876117496807,5.96168582375479,6.54916985951469,7.67816091954023,7.58492975734355,8.26053639846743,8.37803320561941,8.05363984674330,7.29246487867177,5.22094508301405,13.8263090676884,22.1111111111111 };

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            for (int i = 0; i < resultVector.Count; i++)
            {
                resultVector[i] = System.Math.Round(resultVector[i], 3);
            }

            ECG_Baseline_Alg.Filter target = new ECG_Baseline_Alg.Filter();
            Vector<double> actual = target.moving_average(testVector, 9, 87, Filtr_Type.BANDPASS);

            for (int i = 0; i < actual.Count; i++)
            {
                actual[i] = System.Math.Round(actual[i], 3);
            }

            System.Console.WriteLine(actual.ToString());
            Assert.AreEqual(resultVector, actual);
        }

        [TestMethod]
        [Description("Test if LMS LowPass method returns proper values")]
        public void LMSLowPassTest()
        {
            double[] testArray = { 1, 2, 8, 5, 1, 3, 6, 12, 23, 5, 8, 19, 5, 8, 5, 4, 6, 5, 84, 85 };
            double[] resultArray = { 0, 0.000121574342594726, 0.00127067584450675, 0.00256762812086248, 0.00177364722960814, 0.00211926204064925, 0.00390863744536996, 0.00891253160718405, 0.0239684711975802, 0.0286993750454615, 0.0400923764285664, 0.0695486753066789, 0.0758021483151843, 0.0851889059364655, 0.0886454778064214, 0.0958970676536276, 0.119820616779919, 0.158564161731197, 0.433641107383302, 1.23752605246487 };

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            for (int i = 0; i < resultVector.Count; i++)
            {
                resultVector[i] = System.Math.Round(resultVector[i], 3);
            }

            ECG_Baseline_Alg.Filter target = new ECG_Baseline_Alg.Filter();
            Vector<double> actual = target.lms(testVector, 360, 50, Filtr_Type.BANDPASS, 0.000001);

            for (int i = 0; i < actual.Count; i++)
            {
                actual[i] = System.Math.Round(actual[i], 3);
            }

            System.Console.WriteLine(actual.ToString());
            Assert.AreEqual(resultVector, actual);
        }


        [TestMethod]
        [Description("Test if LMS LP method returns proper values")]
        public void LMSHighPassTest()
        {
            double[] testArray = { 1, 2, 8, 5, 1, 3, 6, 12, 23, 5, 8, 19, 5, 8, 5, 4, 6, 5, 84, 85 };
            double[] resultArray = { 0, 4.00000000000000e-06, 8.79998560000002e-05, 0.00100998964801670, 0.00107586810931184, 0.00107176965426736, 0.00215958579237459, 0.00558473914513914, 0.0201031242830191, 0.0394228003615180, 0.0410897860746374, 0.0714849767635253, 0.0925725836135533, 0.101191785750685, 0.110583350977116, 0.103585621833041, 0.114080429837007, 0.116692482610497, 0.344122620993915, 1.92946016022332 };

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            for (int i = 0; i < resultVector.Count; i++)
            {
                resultVector[i] = System.Math.Round(resultVector[i], 3);
            }

            ECG_Baseline_Alg.Filter target = new ECG_Baseline_Alg.Filter();
            Vector<double> actual = target.lms(testVector, 360, 50, Filtr_Type.HIGHPASS, 0.000001);

            for (int i = 0; i < actual.Count; i++)
            {
                actual[i] = System.Math.Round(actual[i], 3);
            }

            System.Console.WriteLine(actual.ToString());
            Assert.AreEqual(resultVector, actual);
        }


        [TestMethod]
        [Description("Test if LMS BandPass method returns proper values")]
        public void LMSBandPassTest()
        {
            double[] testArray = { 1, 2, 8, 5, 1, 3, 6, 12, 23, 5, 8, 19, 5, 8, 5, 4, 6, 5, 84, 85 };
            double[] resultArray = { 0, 0.000121574342594726, 0.00127067584450675, 0.00256762812086248, 0.00177364722960815, 0.00211926204064925, 0.00390863744536996, 0.00891253160718405, 0.0239684711975802, 0.0286993750454615, 0.0400923764285664, 0.0695486753066789, 0.0758021483151843, 0.0851889059364655, 0.0886454778064214, 0.0958970676536276, 0.119820616779919, 0.158564161731197, 0.433641107383302, 1.23752605246487 };

            Vector<double> testVector = Vector<double>.Build.DenseOfArray(testArray);
            Vector<double> resultVector = Vector<double>.Build.DenseOfArray(resultArray);

            for (int i = 0; i < resultVector.Count; i++)
            {
                resultVector[i] = System.Math.Round(resultVector[i], 3);
            }

            ECG_Baseline_Alg.Filter target = new ECG_Baseline_Alg.Filter();
            Vector<double> actual = target.lms(testVector, 360, 50, Filtr_Type.BANDPASS, 0.000001);

            for (int i = 0; i < actual.Count; i++)
            {
                actual[i] = System.Math.Round(actual[i], 3);
            }

            System.Console.WriteLine(actual.ToString());
            Assert.AreEqual(resultVector, actual);
        }

    }

}
