﻿using MathNet.Numerics.LinearAlgebra;
using System;
using MathNet.Numerics;
using MathNet.Numerics.LinearRegression;
using MathNet.Numerics.IntegralTransforms;
using System.Numerics;

namespace EKG_Project.Modules.ECG_Baseline
{
    public class ECG_Baseline_Alg
    {
        public class Filter
        {
            //========================================================================================================
            #region
            /// <summary>
            /// Butterworth filter. Method that filters the input ECG signal by Butterworth filter.
            /// This function allows to select type of filter: lowpass or highpass.
            /// <param name="signal">The raw ECG signal that is filtered</param>
            /// <param name="fs">Sampling frequency of signal</param>
            /// <param name="fc">Cut-off frequency of signal</param>
            /// <param name="order">Filter order</param>
            /// <param name="type">Filter type: lowpass or highpass</param>
            /// <returns>The result of this method of filtration is the filtered signal by low- or highpass butterworth filter</returns>
            #endregion
            public Vector<double> butterworth(Vector<double> signal, double fs, double fc, int order, Filtr_Type type)
            {
                try
                {

                    int signal_length = signal.Count;
                    int DCGain = 1;

                    double[] DoubleArray = new double[signal_length];
                    DoubleArray = signal.ToArray();

                    Complex[] ComplexArray = new Complex[signal_length];

                    for (int i = 0; i < signal_length; i++)
                    {
                        ComplexArray[i] = new Complex(DoubleArray[i], 0);
                    }

                    Fourier.Forward(ComplexArray, FourierOptions.Matlab);

                    double signallength_double = signal_length, binWidth, binFreq, gain;

                    if (fc > 0)
                    {
                        binWidth = fs / signal_length;

                        for (int i = 0; i < (signallength_double / 2); i++)
                        {
                            binFreq = binWidth * (i + 1);
                            gain = DCGain / Math.Sqrt(1 + Math.Pow(binFreq / fc, 2.0 * order));
                            if (type == Filtr_Type.HIGHPASS)
                            {
                                gain = 1 - gain;
                            }
                            ComplexArray[i] *= gain;
                            ComplexArray[signal_length - 1 - i] *= gain;
                        }
                    }

                    Fourier.Inverse(ComplexArray, FourierOptions.Matlab);

                    for (int i = 0; i < signal_length; i++)
                    {
                        DoubleArray[i] = ComplexArray[i].Real;
                    }

                    Vector<double> output_signal = Vector<double>.Build.DenseOfArray(DoubleArray);
                    return output_signal;
                }
                catch
                {
                    return signal;
                }

            }

            #region
            /// <summary>
            /// Overloaded functions 'butterworth()' that allows to do bandpass filter by butterworth filter.
            /// </summary>
            /// <param name="signal">The raw ECG signal that is filtered</param>
            /// <param name="fs">Sampling frequency of signal</param>
            /// <param name="fc_low">Lower cut-off frequency of fignal</param>
            /// <param name="order_low">Lower filter order</param>
            /// <param name="fc_high">Upper cut-off frequency of signal</param>
            /// <param name="order_high">Upper filter order</param>
            /// <param name="type">Filter type</param>
            /// <returns>The result of this method of filtration is the filtered signal by bandpass butterworth filter</returns>
            #endregion
            public Vector<double> butterworth(Vector<double> signal, double fs, double fc_low, int order_low, double fc_high, int order_high, Filtr_Type type = Filtr_Type.BANDPASS)
            {
                try
                {
                    if (type == Filtr_Type.BANDPASS)
                    {
                        int signal_size = signal.Count;
                        Vector<double> lp_filtered_signal = Vector<double>.Build.Dense(signal_size, 0);
                        lp_filtered_signal = butterworth(signal, fs, fc_low, order_low, Filtr_Type.LOWPASS);
                        Vector<double> hp_filtered_signal = Vector<double>.Build.Dense(signal_size, 0);
                        hp_filtered_signal = butterworth(lp_filtered_signal, fs, fc_high, order_high, Filtr_Type.HIGHPASS);

                        return hp_filtered_signal;
                    }

                    return signal;
                }
                catch
                {
                    return signal;
                }
            }

            //========================================================================================================
            #region
            /// <summary>
            /// Least mean squares filter. Method that filters the input ECG signal by LMS filter.
            /// </summary>
            /// <param name="signal">The raw ECG signal that is filtered</param>
            /// <param name="filtered_signal">Filtered reference signal by butterworth filter with certain parameters that are dependent on the type of filtration</param>
            /// <param name="window_size">Number of the weighting coefficients of adaptation</param>
            /// <param name="mi">Speed of adaptation coefficient which value is between 0 and 1</param>
            /// <returns>The result of this method of filtration is the filtered signal by LMS filter</returns>
            #endregion
            public Vector<double> lms(Vector<double> signal, Vector<double> filtered_signal, int window_size, double mi = 0.07)
            {
                try
                {
                    int signal_size = signal.Count;

                    //double mi = 0.07; //Współczynnik szybkości adaptacji

                    Vector<double> coeff = Vector<double>.Build.Dense(window_size, 0); //Wektor z wagami filtru
                    Vector<double> bufor = Vector<double>.Build.Dense(window_size, 0); //Inicjalizacja bufora sygnału wejściowego
                    Vector<double> out_signal = Vector<double>.Build.Dense(signal_size, 0);

                    double dest;
                    double err;

                    for (int i = 0; i < signal_size; i++)
                    {

                        bufor.CopySubVectorTo(bufor, 0, 1, window_size - 1);
                        bufor[0] = signal[i];

                        dest = coeff * bufor;
                        err = filtered_signal[i] - dest;

                        coeff.Add(2 * mi * err * bufor, coeff);

                        //coeff = coeff + (2 * mi * err * bufor);

                        out_signal[i] = dest;

                    }

                    return out_signal;
                }
                catch
                {
                    return signal;
                }
            }

            #region
            /// <summary>
            /// Overloaded functions 'lms()' that allows to filter the input signal by LMS filter.
            /// This function allows to select type of filter: lowpass, highpass or bandpass.
            /// </summary>
            /// <param name="signal">The raw ECG signal that is filtered</param>
            /// <param name="fs">Sampling frequency of signal</param>
            /// <param name="window_size">Number of the weighting coefficients of adaptation</param>
            /// <param name="type">Filter type</param>
            /// <param name="mi">Speed of adaptation coefficient which value is between 0 and 1</param>
            /// <returns>>The result of this method of filtration is the filtered signal by low-, high- or bandpass LMS filter</returns>
            #endregion
            public Vector<double> lms(Vector<double> signal, double fs, int window_size, Filtr_Type type, double mi = 0.07)
            {
                try
                {
                    if (type == Filtr_Type.LOWPASS)
                    {
                        int signal_size = signal.Count;
                        Vector<double> lp_filtered_signal = Vector<double>.Build.Dense(signal_size, 0);
                        lp_filtered_signal = butterworth(signal, fs, 50, 30, Filtr_Type.LOWPASS);
                        return lms(signal, lp_filtered_signal, window_size, mi);
                    }
                    if (type == Filtr_Type.HIGHPASS)
                    {
                        int signal_size = signal.Count;
                        Vector<double> hp_filtered_signal = Vector<double>.Build.Dense(signal_size, 0);
                        hp_filtered_signal = butterworth(signal, fs, 2, 30, Filtr_Type.HIGHPASS);
                        return lms(signal, hp_filtered_signal, window_size, mi);
                    }
                    if (type == Filtr_Type.BANDPASS)
                    {
                        int signal_size = signal.Count;
                        Vector<double> lp_filtered_signal = Vector<double>.Build.Dense(signal_size, 0);
                        lp_filtered_signal = butterworth(signal, fs, 50, 30, Filtr_Type.LOWPASS);
                        Vector<double> hp_filtered_signal = Vector<double>.Build.Dense(signal_size, 0);
                        hp_filtered_signal = butterworth(lp_filtered_signal, fs, 2, 30, Filtr_Type.HIGHPASS);

                        return lms(signal, hp_filtered_signal, window_size, mi);
                    }

                    return signal;
                }
                catch
                {
                    return signal;
                }
            }

            //========================================================================================================

            /// <summary>
            /// Savitzky-Golay's filter. Method that filters the input ECG signal by Savitzky-Golay's filter.
            /// This function allows to select type of filter: lowpass or highpass.
            /// </summary>
            /// <param name="signal">The raw ECG signal that is filtered</param>
            /// <param name="window_size">Size of filtration window</param>
            /// <param name="type">Filter type</param>
            /// <returns>The result of this method of filtration is the filtered signal by low- or highpass Savitzky-Golay's filter</returns>
            public Vector<double> savitzky_golay(Vector<double> signal, int window_size, Filtr_Type type)
            {
                try
                {

                    int signal_size = signal.Count;
                    if ((window_size % 2) == 0)
                    {
                        window_size = window_size + 1;
                    }

                    Vector<double> signal_extension_front = Vector<double>.Build.Dense((window_size / 2), signal[0]);
                    Vector<double> signal_extension_back = Vector<double>.Build.Dense((window_size / 2), signal[signal_size - 1]);

                    int signal_extension_size = signal_extension_front.Count;

                    Vector<double> signal_extended = Vector<double>.Build.Dense(2 * signal_extension_size + signal_size, 0); //przygotowanie zmiennej, w której znajdzie się sygnał przygotowany do filtracji
                    Vector<double> signal_filtered = Vector<double>.Build.Dense(signal_size, 0); //zmienna na sygnał przefiltrowany

                    for (int i = 0; i < signal_extension_size; i++)
                    {
                        signal_extended[i] = signal_extension_front[i]; //powielenie piewszej próbki sygnału wejściowego
                    }
                    for (int i = 0; i < signal_size; i++)
                    {
                        signal_extended[i + signal_extension_size] = signal[i]; //powielenie piewszej próbki sygnału wejściowego
                    }
                    for (int i = 0; i < signal_extension_size; i++)
                    {
                        signal_extended[i + signal_extension_size + signal_size] = signal_extension_back[i]; //powielenie piewszej próbki sygnału wejściowego
                    }
                    Vector<double> samples = Vector<double>.Build.Dense(signal_extended.Count, 0);

                    for (int i = 0; i < samples.Count; i++)
                    {
                        samples[i] = i + 1;
                    }

                    double[] coeff = new double[3];
                    Vector<double> samples_destination = Vector<double>.Build.Dense(window_size, 0);
                    Vector<double> signal_extended_destination = Vector<double>.Build.Dense(window_size, 0);
                    double[] output_signal_table = new double[signal_size];

                    for (int i = 0; i < signal_size; i++)
                    {
                        samples.CopySubVectorTo(samples_destination, i, 0, window_size);
                        signal_extended.CopySubVectorTo(signal_extended_destination, i, 0, window_size);

                        coeff = Fit.Polynomial(samples_destination.ToArray(), signal_extended_destination.ToArray(), 3, DirectRegressionMethod.QR);
                        output_signal_table[i] = coeff[3] * Math.Pow(samples[i + (window_size / 2)], 3) + coeff[2] * Math.Pow(samples[i + (window_size / 2)], 2) + coeff[1] * samples[i + (window_size / 2)] + coeff[0];
                    }

                    Vector<double> output_signal = Vector<double>.Build.DenseOfArray(output_signal_table);

                    if (type == Filtr_Type.HIGHPASS)
                    {
                        return signal - output_signal;
                    }

                    return output_signal;
                }
                catch
                {
                    return signal;
                }
              }

            /// <summary>
            /// Overloaded function 'savitzky-golay()' that allows to do bandpass filter by Saviztky-Golay's filter
            /// </summary>
            /// <param name="signal">The raw ECG signal that is filtered<</param>
            /// <param name="window_size_low">Size of lower filtration window</param>
            /// <param name="window_size_high">Size of upper filtration window</param>
            /// <param name="type">Filter type</param>
            /// <returns>The result of this method of filtration is the filtered signal by bandpass Savitzky-Golay's filter</returns>
            public Vector<double> savitzky_golay(Vector<double> signal, int window_size_low, int window_size_high, Filtr_Type type = Filtr_Type.BANDPASS)
            {
                try
                {
                    if (type == Filtr_Type.BANDPASS)
                    {
                        int signal_size = signal.Count;
                        Vector<double> lp_filtered_signal = Vector<double>.Build.Dense(signal_size, 0);
                        lp_filtered_signal = savitzky_golay(signal, window_size_low, Filtr_Type.LOWPASS);
                        Vector<double> hp_filtered_signal = Vector<double>.Build.Dense(signal_size, 0);
                        hp_filtered_signal = savitzky_golay(lp_filtered_signal, window_size_high, Filtr_Type.HIGHPASS);

                        return hp_filtered_signal;
                    }

                    return signal;
                }
                catch
                {
                    return signal;
                }
            }

            //========================================================================================================
            /// <summary>
            /// Moving average filter. Method that filters the input ECG signal by moving average filter.
            /// This function allows to select type of filter: lowpass or highpass.
            /// </summary>
            /// <param name="signal">The raw ECG signal that is filtered</param>
            /// <param name="window_size">Size of filtration window</param>
            /// <param name="type">Filter type</param>
            /// <returns>The result of this method of filtration is the filtered signal by low- or highpass moving average filter</returns>
            public Vector<double> moving_average(Vector<double> signal, int window_size, Filtr_Type type)
            {
                try
                {
                    int signal_size = signal.Count; //rozmiar sygału wejściowego
                    Vector<double> signal_extension = Vector<double>.Build.Dense(window_size - 1, signal[0]); //uwzględnienie warunków początkowych filtracji
                    Vector<double> signal_extended = Vector<double>.Build.Dense(signal_extension.Count + signal_size, 0); //przygotowanie zmiennej, w której znajdzie się sygnał przygotowany do filtracji
                    Vector<double> signal_filtered = Vector<double>.Build.Dense(signal_size, 0); //zmienna na sygnał przefiltrowany

                    for (int i = 0; i < signal_extension.Count; i++)
                    {
                        signal_extended[i] = signal_extension[i]; //powielenie piewszej próbki sygnału wejściowego
                    }

                    for (int i = 0; i < signal_size; i++)
                    {
                        signal_extended[i + window_size - 1] = signal[i]; //przygotowanie sygnału do filtracji
                    }

                    double sum = 0; //zmienna pomocnicza
                    for (int i = 0; i < signal_size; i++)
                    {
                        sum = 0;
                        for (int j = 0; j < window_size; j++)
                        {
                            sum = sum + signal_extended[i + j]; //sumowanie kolejnych próbek sygnału 
                        }
                        signal_filtered[i] = sum / window_size; //obliczenie średniej 
                    }

                    if (type == Filtr_Type.HIGHPASS)
                    {
                        return signal - signal_filtered;
                    }

                    return signal_filtered; //sygnał przefiltrowany
                }
                catch
                {
                    return signal;
                }

            }
            /// <summary>
            /// Overloaded function 'moving_average' that allows to do bandpass filter by moving average filter
            /// </summary>
            /// <param name="signal">The raw ECG signal that is filtered<</param>
            /// <param name="window_size_low">Size of lower filtration window</param>
            /// <param name="window_size_high">Size of upper filtration window</param>
            /// <param name="type">Filter type</param>
            /// <returns>The result of this method of filtration is the filtered signal by bandpass moving average filter</returns>
            public Vector<double> moving_average(Vector<double> signal, int window_size_low, int window_size_high, Filtr_Type type = Filtr_Type.BANDPASS)
            {
                try
                {
                    if (type == Filtr_Type.BANDPASS)
                    {
                        int signal_size = signal.Count;
                        Vector<double> lp_filtered_signal = Vector<double>.Build.Dense(signal_size, 0);
                        lp_filtered_signal = moving_average(signal, window_size_low, Filtr_Type.LOWPASS);
                        Vector<double> hp_filtered_signal = Vector<double>.Build.Dense(signal_size, 0);
                        hp_filtered_signal = moving_average(lp_filtered_signal, window_size_high, Filtr_Type.HIGHPASS);

                        return hp_filtered_signal;
                    }

                    return signal;
                }
                catch
                {
                    return signal;
                }
                }

        }

        /*
        static void Main()
        {
            double[] input_signal = {10,22,24,42,37,77,89,22,63,9,11};
         
            Vector<double> signal = Vector<double>.Build.DenseOfArray(input_signal);
            int signal_size = signal.Count;
            int window_size = 7;
            Vector<double> signal_filtered = Vector<double>.Build.Dense(signal_size, 0);
            Vector<double> signal_filtered2 = Vector<double>.Build.Dense(signal_size, 0);
            Vector<double> signal_filtered3 = Vector<double>.Build.Dense(signal_size, 0);

            Filter newFilter = new Filter();
            signal_filtered = newFilter.savitzky_golay(signal, 10, 10, Filtr_Type.BANDPASS);

            Filter newFilter2 = new Filter();
            signal_filtered2 = newFilter2.lms(signal, 10, 10, Filtr_Type.BANDPASS, 0.000001);

            Filter newFilter3 = new Filter();
            signal_filtered3 = newFilter3.butterworth(signal,100, 2,2,4,4, Filtr_Type.BANDPASS);

            System.Console.WriteLine(signal_filtered.ToString());
            System.Console.WriteLine(signal_filtered2.ToString());
            System.Console.WriteLine(signal_filtered3.ToString());
            System.Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }
        */

    }

}
