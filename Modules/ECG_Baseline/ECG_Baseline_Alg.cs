﻿using MathNet.Numerics.LinearAlgebra;
using System;
using MathNet.Numerics;
using MathNet.Numerics.LinearRegression;

namespace EKG_Project.Modules.ECG_Baseline
{
    public partial class ECG_Baseline : IModule
    {


        public class Filter
        {

            public Vector<double> savitzky_golay(Vector<double> signal, int window_size, int type)
            {

                //TODO: Comments

                int signal_size = signal.Count;
                if ((window_size % 2) == 0)
                {
                    window_size = window_size + 1;
                }

                Vector<double> signal_extension_front = Vector<double>.Build.Dense((window_size / 2), signal[0]);
                Vector<double> signal_extension_back = Vector<double>.Build.Dense((window_size / 2), signal[signal_size - 1]);

                int signal_extension_size = signal_extension_front.Count;

                Vector<double> signal_extended = Vector<double>.Build.Dense(2*signal_extension_size + signal_size, 0); //przygotowanie zmiennej, w której znajdzie się sygnał przygotowany do filtracji
                Vector<double> signal_filtered = Vector<double>.Build.Dense(signal_size, 0); //zmienna na sygnał przefiltrowany

                for (int i = 0; i < signal_extension_size; i++)
                {
                    signal_extended[i] = signal_extension_front[i];                   //powielenie piewszej próbki sygnału wejściowego
                }
                for (int i = 0; i < signal_size; i++)
                {
                    signal_extended[i + signal_extension_size] = signal[i];                   //powielenie piewszej próbki sygnału wejściowego
                }
                for (int i = 0; i < signal_extension_size; i++)
                {
                    signal_extended[i + signal_extension_size + signal_size] = signal_extension_back[i];                   //powielenie piewszej próbki sygnału wejściowego
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

                    coeff = Fit.Polynomial(samples_destination.ToArray(), signal_extended_destination.ToArray(), 3, DirectRegressionMethod.NormalEquations);
                    output_signal_table[i] = coeff[3] * Math.Pow(samples[i + (window_size / 2)], 3) + coeff[2] * Math.Pow(samples[i + (window_size / 2)], 2) + coeff[1] * samples[i + (window_size / 2)] + coeff[0];
                }

                Vector<double> output_signal = Vector<double>.Build.DenseOfArray(output_signal_table);

                return output_signal;
            }

            public Vector<double> moving_average(Vector<double> signal, int window_size)
            {
                int signal_size = signal.Count; //rozmiar sygału wejściowego
                Vector<double> signal_extension = Vector<double>.Build.Dense(window_size - 1, signal[0]); //uwzględnienie warunków początkowych filtracji
                Vector<double> signal_extended = Vector<double>.Build.Dense(signal_extension.Count + signal_size, 0); //przygotowanie zmiennej, w której znajdzie się sygnał przygotowany do filtracji
                Vector<double> signal_filtered = Vector<double>.Build.Dense(signal_size, 0); //zmienna na sygnał przefiltrowany

                for (int i = 0; i < signal_extension.Count; i++)
                {
                    signal_extended[i] = signal_extension[i];                   //powielenie piewszej próbki sygnału wejściowego
                }

                for (int i = 0; i < signal_size; i++)
                {
                    signal_extended[i + window_size - 1] = signal[i];           //przygotowanie sygnału do filtracji
                }

                double sum = 0;                                                 //zmienna pomocnicza
                for (int i = 0; i < signal_size; i++)
                {
                    sum = 0;
                    for (int j = 0; j < window_size; j++)
                    {
                        sum = sum + signal_extended[i + j];                     //sumowanie kolejnych próbek sygnału 
                    }
                    signal_filtered[i] = sum / window_size;                     //obliczenie średniej 
                }
                return signal_filtered;                                         //sygnał przefiltrowany

            }
        }


        static void Main()
        {
            double[] input_signal = {10,22,24,42,37,77,89,22,63,9};
         
            Vector<double> signal = Vector<double>.Build.DenseOfArray(input_signal);
            int signal_size = signal.Count;
            int window_size = 4;
            Vector<double> signal_filtered = Vector<double>.Build.Dense(signal_size, 0);

            Filter newFilter = new Filter();
            signal_filtered = newFilter.savitzky_golay(signal, window_size, 1);

            System.Console.WriteLine(signal_filtered.ToString());
            System.Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }

    }

}
