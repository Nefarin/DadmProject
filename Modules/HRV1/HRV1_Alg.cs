/*
    TODO:
        1 zaimplementowac metode samplesToInstants() - pomnożyć numery próbek razy dt / podzielić fs
        2 zaimplenentowac metode instantsToIntervals() - odejmować próbki od siebie
        3 zaimplenetowac cala reszte
        4 podlaczyc do interfejsow
        5 Testy, testy, testy...
        ???
        PROFIT
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;
using EKG_Project.IO;


namespace EKG_Project.Modules.HRV1
{
    #region HRV class doc
    /// <summary>
    /// Class for calculating frequency- and time-based prameters of RR tachogram
    /// </summary>
    #endregion
    public partial class HRV1 : IModule
    {
        // Declaration of time parameters
        private double SDNN;
        private double RMSSD;
        private double SDSD;
        private double NN50;
        private double pNN50;

        // Declaration od frequency parameters
        private double HF;
        private double LF;
        private double VLF;
        private double LFHF;

        // Declaration of required vectors
        private Vector<double> rSamples;    // holds numbers of samples in original ECG signal on which R waves have been detected
        private Vector<double> rInstants;   // holds time instants in which which R waves have been detected
        private Vector<double> rrIntervals; // holds time intervals between consecutive R waves
        private Vector<double> f;           // vector of frequiencies on which PSD estimate is calculated
        private Vector<double> PSD;         // power spectral density estimate values at points specified in f
        private Vector<double> psdFilter;   // samples of digital FIR filter used for smoothing PSD

        private double Fs;  // sampling frequency of original ECG signal
        private double dt;  // time interval between consecutive samples of original ECG signal

        #region
        /// <summary>
        /// This methods calculates vector rInstants based on values of Fs and rSamples
        /// </summary>
        #endregion
        private void samplesToInstants(Vector<double> rSamples, double Fs )
        {
          ;
        }

        

        #region
        /// <summary>
        /// This methods calculates vaecor rrIntervals based on values in rInstants
        /// </summary>
        #endregion
        private void instantsToIntervals()
        {
    ;
        }

        
        #region
        /// <summary>
        /// This method calculates time-based parameters
        /// </summary>
        #endregion
        private void calculateTimeBased()
        {
    

    SDNN = 0;
            RMSSD = 0;
            SDSD = 0;
            NN50 = 0;
            pNN50 = 0;
        }


       


        #region
        /// <summary>
        /// thsi method generates vector of uniformely spaced numbers between fMin and fMax with step 'step'
        /// the last element in vector is larger or equal 'fMax'
        /// </summary>
        /// <param name="fMin"></param>
        /// <param name="fMax"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        #endregion
        private Vector<double> gnerateFreqVector(double fMin, double fMax, double step)
        {
            var elementsCount = (int)Math.Ceiling((fMax - fMin) / step) + 1;
            var f = Vector<double>.Build.Dense(elementsCount, (i) => fMin + i*step);
            return f;
        }


        #region
        /// <summary>
        /// This method calculates Lomb-Scargle periodogram of signal
        /// http://www.mathworks.com/help/signal/ref/plomb.html#lomb
        /// </summary>
        /// <param name="f"></param>
        /// <param name="t"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        #endregion
        public static Vector<double> lombScargle(Vector<double> f, Vector<double> t, Vector<double> x)
        {
            // TODO: upewnic sie ze wektory maja sensowna dlugosc

            var lsLen = f.Count;  // periodogram length equal to the length of frequenies vector
            var pi = Math.PI;
            var W = Vector<double>.Build.Dense(f.Count, (j) => 2 * pi * f[j]); // convert frequencies to angular frequency
            var LS = Vector<double>.Build.Dense(lsLen); // prealocate vector for PSD

            var mean = Statistics.Mean(x);
            var std2 = Statistics.Variance(x);

            double tauNumerator, tauDenominator, tau;
            double w;
            double P, PN1, PD1, PN2, PD2;

            for (int i = 0; i < lsLen; ++i)
            {
                w = W[i];

                // calculate tau for current w
                tauNumerator = Vector<double>.Build.Dense(lsLen, (k) => Math.Sin(2 * w * t[k])).Sum();
                tauDenominator = Vector<double>.Build.Dense(lsLen, (k) => Math.Cos(2 * w * t[k])).Sum();
                tau = Math.Atan2(tauNumerator, tauDenominator) / (2 * w);

                // calculate partial expressions for periodogram
                PN1 = Vector<double>.Build.Dense(lsLen, (j) => (x[j] - mean) * Math.Cos(w * (t[j] - tau))).Sum();
                PN2 = Vector<double>.Build.Dense(lsLen, (j) => (x[j] - mean) * Math.Sin(w * (t[j] - tau))).Sum();
                PD1 = Vector<double>.Build.Dense(lsLen, (j) => Math.Pow(Math.Cos(w * (t[j] - tau)), 2)).Sum();
                PD2 = Vector<double>.Build.Dense(lsLen, (j) => Math.Pow(Math.Sin(w * (t[j] - tau)), 2)).Sum();

                // calculate power at given w from partial expressions
                P = (1 / (2 * std2)) * ((Math.Pow(PN1, 2) / PD1) + Math.Pow(PN2, 2) / PD2);
                LS[i] = P;
            }
            return LS;
        }


#region
/// <summary>
/// This function calculates frequency-based parameters
/// </summary>
#endregion
private void calculateFreqBased()
{
        var temp_vec = Vector<double>.Build.Dense(f.Count, 1);
    //Obliczenie mocy widma w zakresie wysokich częstotliwości (0,15-0,4Hz)
        for (int i = 0; i < f.Count; i++)
        {

        if (f[i] >= 0.15 && f[i] < 0.4)
        {
            HF = temp_vec[i] + HF;
        }
    }

    //Wyznaczenie mocy widma w zakresie niskich częstotliwości (0,04-0,15Hz)
        for (int i = 0; i < f.Count; i++)
    {
        if (f[i] > 0.04 && f[i] < 0.15)
        {
            LF = temp_vec[i] + LF;
        }
    }

    //Obliczenie mocy widma w zakresie bardzo niskich częstotliwości (0,003-0,04Hz)
       for (int i = 0; i < f.Count; i++)
    {
        if (f[i] > 0.003 && f[i] < 0.04)
        {
            VLF = temp_vec[i] + VLF;
        }
    }

            //Obliczenie stosunku mocy widm niskich częstotliwości do mocy widm wysokich częstotliwości
            LFHF = LF / HF;

}






#region Main method doc
/// <summary>
/// 'Main' mehod used for code debugging and testing
/// </summary>
#endregion
public static void AlgoTsest()
        {
            Console.WriteLine("Hello Matylda!");

            // do testowania nalezy odkomentowac odpowiednia linijke, a zakomentowac pozostale
            //string dataPath = "F:\\Dropbox\\Studia\\DADM\\Projekt\\CiSzarp\\TestData\\"; // scieazka do tesotwych danych na duzym komputerze Michala
           // string dataPath = "C:\\Dropbox\\Studia\\DADM\\Projekt\\CiSzarp\\TestData\\"; // scieazka do tesotwych danych na laptopie Michala
            string dataPath = "...\\Dropbox\\Studia\\DADM\\Projekt\\CiSzarp\\TestData\\"; // scieazka do tesotwych danych na komputerze Matyldy
            string filename = "NSR001.txt"; // plik zawiera 'ladny' wycinek z tachogramu RR sygnalu NSR001
            dataPath = dataPath + filename;

            var hrv = new HRV1();

            // wczytywanie danych - tutaj trzeba bedzie dostosowac do nowego modulu IO, kiedy sie pojawi
            TempInput.setInputFilePath(dataPath);
            hrv.rSamples = TempInput.getSignal();
            hrv.Fs = TempInput.getFrequency();
            hrv.dt = 1 / hrv.Fs;

            //hrv.samplesToInstants();
            //hrv.instantsToIntervals();
            hrv.f = hrv.gnerateFreqVector(0, 1, (double)1/1000);
            //Console.WriteLine(hrv.f);

            // testowanie lomba
            double[] fSrc = new double[] { 1, 2, 4, 5, 6, 7, 8, 9, 10 };
            var f = Vector<double>.Build.Dense(fSrc);
            var xSrc = new double[] { 1, 2, 4, 5, 6, 7, 8, 9, 10 };
            var x = Vector<double>.Build.Dense(xSrc);
            var ySrc = new double[] { 1, 2, 4, 5, 6, 7, 8, 9, 10 };
            var y = Vector<double>.Build.Dense(ySrc);
            hrv.PSD = lombScargle(f, x, y);
            Console.WriteLine(hrv.PSD);

            //dalej heja

        }
    }
}
