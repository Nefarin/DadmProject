/*
    TODO:
        1 Poprawic plomba
        2 Refaktoryzacja 
        5 Testy, testy, testy...
        6 ???
        7 nie ma numeru 3 i 4
        7 PROFIT
        8 ps. numer 7 jest dwa razy
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    public class HRV1_Alg
    {
        // Declaration of time parameters
		private double AVNN;
        private double SDNN;
        private double RMSSD;
        private double SDSD;
        private double NN50;
        private double pNN50;

        // Declaration od frequency parameters
		private double TP;
        private double HF;
        private double LF;
        private double VLF;
        private double LFHF;

        // Declaration of required vectors
        private Vector<double> rSamples;    // holds numbers of samples in original ECG signal on which R waves have been detected
        public Vector<double> rInstants;   // holds time instants in which which R waves have been detected
        public Vector<double> rrIntervals; // holds time intervals between consecutive R waves
        private Vector<double> f;           // vector of frequiencies on which PSD estimate is calculated
        private Vector<double> PSD;         // power spectral density estimate values at points specified in f
        private Vector<double> psdFilter;   // samples of digital FIR filter used for smoothing PSD

        public List<Tuple<string, double>> FreqParams = new List<Tuple<string, double>>();
        public List<Tuple<string, double>> TimeParams = new List<Tuple<string, double>>();
        public List<Tuple<string, Vector<double>>> PowerSpectrum;

        private double Fs;  // sampling frequency of original ECG signal
        private double dt;  // time interval between consecutive samples of original ECG signal

        public void CalculateTimeBased() {
            this.intervalsCorection();
            this.calculateTimeBased();
            TimeParams.Clear();
            TimeParams.Add(new Tuple<string, double>("AVNN", this.AVNN));
            TimeParams.Add(new Tuple<string, double>("SDNN", this.SDNN));
            TimeParams.Add(new Tuple<string, double>("RMSSD", this.RMSSD));
            TimeParams.Add(new Tuple<string, double>("pNN50", this.pNN50));
        }

        public void CalculateFreqBased() {
            this.generateFreqVector(0.001, 0.151, this.rrIntervals.Count);
            //this.lombScargle2();
            this.lombScargle();
            this.calculateFreqBased();
            this.PowerSpectrum = new List<Tuple<string, Vector<double>>>();
            this.PowerSpectrum.Add(new Tuple<string, Vector<double>>("f", this.f));
            this.PowerSpectrum.Add(new Tuple<string, Vector<double>>("PSD", this.PSD));
            this.FreqParams.Clear();
            this.FreqParams.Add(new Tuple<string, double>("TP", this.TP));
            this.FreqParams.Add(new Tuple<string, double>("HF", this.HF));
            this.FreqParams.Add(new Tuple<string, double>("LF", this.LF));
            this.FreqParams.Add(new Tuple<string, double>("VLF", this.VLF));
            this.FreqParams.Add(new Tuple<string, double>("LFHF", this.LFHF));
        }

        /*
        #region
        /// <summary>
        /// This methods calculates vector rInstants based on values of Fs and rSamples
        /// </summary>
        #endregion
        private void samplesToInstants(Vector<double> rSamples, double Fs )
        {
			this.rInstants = Vector<double>.Build.Dense(this.rSamples.Count, (i) => this.rSamples[i] * dt);
        }
        */
        

        #region
        /// <summary>
        /// This methods calculates vaecor rrIntervals based on values in rInstants
        /// </summary>
        #endregion
        private void instantsToIntervals()
        {
			this.rrIntervals = Vector<double>.Build.Dense(this.rInstants.Count-1, (i) => this.rInstants[i+1] - this.rInstants[i]);
        }

        
		#region
        /// <summary>
        /// Function for correcting instants vector for vaulty beats
        /// </summary>
        #endregion
        private void intervalsCorection()
        {
            double ectoThreshHi = 1000;
            double ectoThreshLo = 5;
            List<double> rrIntervals_temp = new List<double>();

            for (int i = 0; i < this.rrIntervals.Count; ++i)
            {
                if (this.rrIntervals[i] > ectoThreshLo && this.rrIntervals[i] < ectoThreshHi)
                {
                    rrIntervals_temp.Add(rrIntervals[i]);
                }
            }
            rrIntervals = Vector<double>.Build.Dense(rrIntervals_temp.ToArray());
            rInstants = Vector<double>.Build.Dense(rrIntervals.Count + 1, (i) => 0);
            for (int i = 1; i < rInstants.Count; ++i)
            {
                rInstants[i] = rInstants[i - 1] + rrIntervals[i - 1];
            }
        }


        #region
        /// <summary>
        /// This method calculates time-based parameters
        /// </summary>
        #endregion
        private void calculateTimeBased()
        {
            AVNN = this.rrIntervals.Sum() / this.rrIntervals.Count;
            SDNN = Statistics.StandardDeviation(this.rrIntervals);

            var sdL = this.rrIntervals.SubVector(0, this.rrIntervals.Count - 1);
            var sdR = this.rrIntervals.SubVector(1, this.rrIntervals.Count - 1);
            var succesiveDiffs = sdR.Subtract(sdL);
            

            RMSSD = Statistics.RootMeanSquare(succesiveDiffs);
            succesiveDiffs.MapInplace(x => Math.Abs(x));

            //SDSD = Statistics.StandardDeviation(succesiveDiffs);

            NN50 = 0;
            foreach (double x in succesiveDiffs) { NN50 = (x > 50) ? NN50 + 1 : NN50; }
            pNN50 = 100 * NN50 / this.rrIntervals.Count;
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
        private void generateFreqVector(double fMin, double fMax, int elementsCount)
        {
            //var elementsCount = (int)Math.Ceiling((fMax - fMin) / step) + 1;
            //f = Vector<double>.Build.Dense(elementsCount, (i) => fMin + i*step);
            double step = (fMax - fMin) / elementsCount;
            f = Vector<double>.Build.Dense(elementsCount, (i) => fMin + i * step);
        }


		#region
        /// <summary>
        /// This method calculates Lomb-Scargle periodogram of signal
        /// http://www.mathworks.com/help/signal/ref/plomb.html#lomb
        /// </summary>
        #endregion
        public void lombScargle()
        {
            // TODO: problem z f = 0
            // TODO: dlugosc periodo = najmnieszja dlugosc f inst lub interval

            var t = rInstants;
            var x = rrIntervals;

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
                if (w == 0) { LS[i] = 0; }
                else
                {
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
            }
            this.PSD = LS;
        }


        /// <summary>
        /// Another implementation of Lomb-Scargle periodogram
        /// </summary>
        public void lombScargle2()
        {
            Vector<double> tau;
            Vector<double> psd;
            Vector<double> omega;

            double mean = Statistics.Mean(this.rrIntervals);
            double var = Statistics.Variance(this.rrIntervals);

            int psdlength = this.rrIntervals.Count;
            double timespan = this.rInstants.Last() - this.rInstants.First();
            double pi = Math.PI;

            omega = Vector<double>.Build.Dense(2 * psdlength, i => (i + 1) * 2 * (pi / timespan));
            psd = Vector<double>.Build.Dense(2 * psdlength);
            this.PSD = Vector<double>.Build.Dense(2 * psdlength);

            var sins = Vector<double>.Build.Dense(psdlength, j => Math.Sin(2 * omega[j] * this.rInstants[j]));
            var coss = Vector<double>.Build.Dense(psdlength, j => Math.Cos(2 * omega[j] * this.rInstants[j]));
            double sinsum = sins.Sum();
            double cossum = coss.Sum();
            tau = Vector<double>.Build.Dense(2 * psdlength, i => Math.Atan2(sinsum, cossum) / (2 * omega[i]));

            /*
            for (int i = 0; i<2*psdlength; i++)
            {
                double sinsum = 0;
                double cossum = 0;
                for (int j = 0; j<psdlength; i++)
                {
                    sinsum += Math.Sin(2 * omega[j] * this.rInstants[j]);
                    cossum += Math.Cos(2 * omega[j] * this.rInstants[j]);
                }
                tau[i] = (Math.Atan2(sinsum, cossum) / (2*omega[i]));
            }
            */

            double stdcos, stdsin, cos2, sin2;

            for (int i = 0; i < (2*psdlength); i++) 
            {
                stdcos = 0;
                cos2 = 0;
                stdsin = 0;
                sin2 = 0;
                for (int j = 0; j<psdlength; j++)
                {
                    stdcos += (this.rrIntervals[j] - mean) * Math.Cos(omega[i] * (this.rInstants[j] - tau[i]));
                    stdsin += (this.rrIntervals[j] - mean) * Math.Sin(omega[i] * (this.rInstants[j] - tau[i]));
                    //stdsin *= stdsin;
                    //stdcos *= stdcos;
                    stdsin += stdsin;
                    stdcos += stdcos;
                    cos2 += Math.Pow(Math.Cos(omega[i] * (this.rInstants[j] - tau[i])), 2);
                    sin2 += Math.Pow(Math.Sin(omega[i] * (this.rInstants[j] - tau[i])), 2);
                }
                this.PSD[i] = ((stdcos / cos2 + stdsin / sin2) / (2 * var));
            }
            //this.PSD = psd;
            this.f = Vector<double>.Build.Dense(omega.Count, i => omega[i] / (2 * pi));
        }


        #region
        /// <summary>
        /// This function calculates frequency-based parameters
        /// </summary>
        #endregion
        private void calculateFreqBased()
        {
            //generateFreqVector(0, 1, this.rrIntervals.Count);
            //lombScargle();
            //double df = (double)1000 / rrIntervals.Count;

            double df = (this.f.Max() - this.f.Min()) / (this.f.Count-1);

            var temp_vec = Vector<double>.Build.Dense(PSD.Count, (i) => PSD[i]*df);
            
            TP = VLF = LF = HF = 0;

            //Obliczenie całkowitej mocy widma
            TP = temp_vec.Sum();

            //Obliczenie mocy widma w zakresie wysokich częstotliwości (0,15-0,4Hz)
            for (int i = 0; i < f.Count; i++)
            {
                if (f[i] >= 0.15 && f[i] <= 0.4)
                {
                    HF = temp_vec[i] + HF;
                }
            }

            //Wyznaczenie mocy widma w zakresie niskich częstotliwości (0,04-0,15Hz)
            for (int i = 0; i < f.Count; i++)
            {
                if (f[i] >= 0.04 && f[i] <= 0.15)
                {
                    LF = temp_vec[i] + LF;
                }
            }

            //Obliczenie mocy widma w zakresie bardzo niskich częstotliwości (0,003-0,04Hz)
               for (int i = 0; i < f.Count; i++)
            {
                if (f[i] >= 0.003 && f[i] <= 0.04)
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
public static void Main()
        {
            Console.WriteLine("Hello Matylda!");
            var hrv1Test = new HRV1_Alg();

            var testintervals = new double[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            hrv1Test.rrIntervals = Vector<double>.Build.Dense(testintervals);
            hrv1Test.rInstants = Vector<double>.Build.Dense(testintervals);

            hrv1Test.generateFreqVector(0, 1, 10);
            Console.WriteLine(hrv1Test.f);

            hrv1Test.lombScargle();
            Console.WriteLine(hrv1Test.PSD);

            // do testowania nalezy odkomentowac odpowiednia linijke, a zakomentowac pozostale
            //string dataPath = "F:\\Dropbox\\Studia\\DADM\\Projekt\\CiSzarp\\TestData\\"; // scieazka do tesotwych danych na duzym komputerze Michala
            // string dataPath = "C:\\Dropbox\\Studia\\DADM\\Projekt\\CiSzarp\\TestData\\"; // scieazka do tesotwych danych na laptopie Michala
            //string dataPath = "...\\Dropbox\\Studia\\DADM\\Projekt\\CiSzarp\\TestData\\"; // scieazka do tesotwych danych na komputerze Matyldy
            //string filename = "NSR001.txt"; // plik zawiera 'ladny' wycinek z tachogramu RR sygnalu NSR001
            //dataPath = dataPath + filename;

            //var hrv = new HRV1();

            // wczytywanie danych - tutaj trzeba bedzie dostosowac do nowego modulu IO, kiedy sie pojawi
            //TempInput.setInputFilePath(dataPath);
            // hrv.rSamples = TempInput.getSignal();
            // hrv.Fs = TempInput.getFrequency();
            // hrv.dt = 1 / hrv.Fs;

            //hrv.samplesToInstants();
            //hrv.instantsToIntervals();
            //hrv.generateFreqVector(0, 1, (double)1/1000);
            //Console.WriteLine(hrv.f);

            // testowanie lomba
            //     double[] fSrc = new double[] { 1, 2, 4, 5, 6, 7, 8, 9, 10 };
            //      var f = Vector<double>.Build.Dense(fSrc);
            //      var xSrc = new double[] { 1, 2, 4, 5, 6, 7, 8, 9, 10 };
            //      var x = Vector<double>.Build.Dense(xSrc);
            //     var ySrc = new double[] { 1, 2, 4, 5, 6, 7, 8, 9, 10 };
            //     var y = Vector<double>.Build.Dense(ySrc);
            //hrv.PSD = lombScargle();
            // Console.WriteLine(hrv.PSD);

            //dalej heja
        }
    }
}
