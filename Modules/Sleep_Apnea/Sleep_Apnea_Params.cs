using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.Sleep_Apnea
{
    public class Sleep_Apnea_Params : ModuleParams
    {         //INPUT        
        protected int sampling_freqency { get; set; }

        public void sleep_apnea(int sampling_freqency_get)
        {
            sampling_freqency = sampling_freqency_get;
        }

        //Module set properties
        public int data_freq;  //sampling freqency of data
        public int window_average;  //window for average filter 
        public int filtr;  //for hilbert transform 
        public int window_median;

        //OUTPUT (this will be shown in GUI)
        private Vector<double> tab_R_peaks;
        private Matrix<double> tab_R_peaks2;


        public Matrix<double> sleep_apnea_plots
        {

            get
            {
                return tab_R_peaks2;
            }
            set
            {
                tab_R_peaks2 = value;
            }
        }
        //These matrix contatin Time and Hilbert amplitude/frequency to select it on graph
        //Vector[0]: Time in samples
        //Vector[1]: Hilbert amplitude
        //Vector[2]: Hilbert freqency

        public Matrix<double> sleep_apnea_output
        {

            get
            {
                return tab_R_peaks2;
            }
            set
            {
                tab_R_peaks2 = value;
            }
        }
        //Matrix of pairs: begin and end sample of apnea detection

        public Vector<double> gui_output
        {

            get
            {
                return tab_R_peaks;
            }
            set
            {
                tab_R_peaks = value;
            }
        }
        //Vector[0]: Treshold value of min_amplitude [s]
        //Vector[1]: Treshold value of max_frequency [Hz]
        //Vector[2]: Apnea assessment in the time domain [%]
        //Vector[3]: Apnea assessment in the frequency domain [%]

    }
}

