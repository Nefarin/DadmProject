using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.Modules.ECG_Baseline
{
    public partial class ECG_Baseline : IModule
    {


        public class Filter
        {
            public Vector<double> moving_average(Vector<double> signal, int window_size)
            {
                int signal_size = signal.Count; //rozmiar sygału wejściowego
                Vector<double> signal_extension = Vector<double>.Build.Dense(window_size - 1, signal[0]); //uwzględnienie warunków początkowych filtracji
                Vector<double> signal_extended = Vector<double>.Build.Dense(signal_extension.Count + signal_size, 0); //przygotowanie zmiennej, w której znajdzie się sygnał przygotowany do filtracji
                Vector<double> signal_filtered = Vector<double>.Build.Dense(signal_size, 0); //zmienna na sygnał przefiltrowany

                for (int i = 0; i < signal_extension.Count; i++)
                {
                    signal_extended[i] = signal_extension[i];                   // powielenie piewszej próbki sygnału wejściowego
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
            int window_size = 3;
            Vector<double> signal_filtered = Vector<double>.Build.Dense(signal_size, 0);

            Filter newFilter = new Filter();
            signal_filtered = newFilter.moving_average(signal, window_size);


            System.Console.WriteLine("===========================================================================");
            System.Console.WriteLine(signal_filtered.ToString());
            System.Console.WriteLine("===========================================================================");
            System.Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }

    }

}
