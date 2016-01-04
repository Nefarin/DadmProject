using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using BeginEndPair = System.Collections.Generic.List<int>;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }


    class sleep_apnea
    {
        //INPUT        
        protected int sampling_freqency { get; set; }

        public sleep_apnea(int sampling_freqency_get)
        {
            sampling_freqency = sampling_freqency_get;
        }

        //Module set properties
        public int data_freq;  //sampling freqency of data
        public int window_average;  //window for average filter 
        public int filtr;  //for hilbert transform 
        public int window_median;

        //OUTPUT (this will be shown in GUI)
        private Vector <double> tab_R_peaks { get; set; }
        private Matrix <double> tab_R_peaks2{ get; set; }
        private Matrix <double> tab_RR{ get; set; }
		private Matrix <double> tab_RR_new{ get; set; }
		private Matrix <double> tab_res{ get; set; }

		
        public Matrix <double> sleep_apnea_plots(Vector <double> tab_R_peaks);
        
        //These matrix contatin Time and Hilbert amplitude/frequency to select it on graph
        //Vector[0]: Time in samples
        //Vector[1]: Hilbert amplitude
        //Vector[2]: Hilbert freqency

        public Matrix <double> sleep_apnea_output(Matrix <double> tab_R_peaks);
       
        //Matrix of pairs: begin and end sample of apnea detection

        public Vector <double> gui_output(Vector <double> tab_R_peaks);
        //Vector[0]: Treshold value of min_amplitude [s]
        //Vector[1]: Treshold value of max_frequency [Hz]
        //Vector[2]: Apnea assessment in the time domain [%]
        //Vector[3]: Apnea assessment in the frequency domain [%]

		 private Matrix <double> RR_intervals(Matrix <double> tab_R_peaks);
		
		 private Matrix <double> averange_filter(Matrix <double> tab_RR);

		 private Matrix <double> resample(Matrix <double> tab_RR_new);	
		 private Matrix  <double> HP_LP_filter(Matrix <double> tab_res,Matrix <double> &h_amp,Matrix <double> &h_freq);
		 private Matrix  <double> hilbert(Matrix <double> &h_amp,Matrix <double> &h_freq);	
		 private Matrix  <double> freq_amp_filter (Matrix <double> &h_amp,Matrix <double> &h_freq);	
		 private Matrix  <double> median_filter (Matrix <double> &h_amp,Matrix <double> &h_freq);	 
		 private Matrix  <double> apnea_detection (Matrix <double> tab_amp,Matrix <double> tab_freq);		

    }
}
