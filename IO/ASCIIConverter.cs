using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    class ASCIIConverter : IECGConverter
    {
        string analysisName;
        string[] lines;
        List<string>[] columns;
        uint sampleAmount;
        Basic_Data _data;

        public Basic_Data Data
        {
            get
            {
                return _data;
            }

            set
            {
                _data = value;
            }
        }

        public ASCIIConverter (string ASCIIAnalysisName) 
        {
            analysisName = ASCIIAnalysisName;
        }

        public void SaveResult()
        {
            foreach (var property in Data.GetType().GetProperties())
            {

                if (property.GetValue(Data, null) == null)
                {
                    throw new Exception(); // < - robić coś takiego?

                }
                else
                {
                    Basic_Data_Worker dataWorker = new Basic_Data_Worker(analysisName);
                    dataWorker.Save(Data);
                }
            }
        }

        public void ConvertFile(string path)
        {
            loadASCIIFile(path);
            Data = new Basic_Data();
            Data.Frequency = getFrequency();
            Data.Signals = getSignals();
            Data.SampleAmount = sampleAmount;
        }

        public void loadASCIIFile(string path)
        {
            lines = File.ReadAllLines(path);

            char[] columnDelimiter = new char[] { '\t' };
            string[] headerColumns = lines[0].Split(columnDelimiter, StringSplitOptions.RemoveEmptyEntries);

            columns = new List<string>[headerColumns.Count()];
            for (int i = 0; i < headerColumns.Count(); i++)
            {
                columns[i] = new List<string>();
            }

            for (int i = 0; i < lines.Count(); i++)
            {
                string[] lineColumns = lines[i].Split(columnDelimiter, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < lineColumns.Count(); j++)
                {
                    columns[j].Add(lineColumns[j]);
                }
            }

        }

       public uint getFrequency()
        {
            uint frequency = 0;
            //TimeSpan startTime = TimeSpan.Parse(columns[0][3]);
            //TimeSpan stopTime = TimeSpan.Parse(columns[0][columns.Length]);
            return frequency;
        }


        public List<Tuple<string, Vector<double>>> getSignals()
       {
           List<Tuple<string, Vector<double>>> Signals = new List<Tuple<string, Vector<double>>>();
           
           Vector<double> signal = Vector<double>.Build.Dense(lines.Length - 2);
           for (int i = 1; i < columns.Count(); i++)
           {
               string leadName = columns[i][0];

               for (int j = 2; j < lines.Count()-2; j++)
               {
                   double value = Convert.ToDouble(columns[i][j], new System.Globalization.NumberFormatInfo());
                   int index = j - 2;
                   signal.At(index, value);
               }

               getSampleAmount(signal);
               Tuple<string, Vector<double>> readedSignal = Tuple.Create(leadName, signal);
               Signals.Add(readedSignal);
           }

           return Signals;

       }


        public uint getSampleAmount(Vector<double> signal)
        {
            if (signal != null)
                sampleAmount = (uint)signal.Count;
            return sampleAmount;
        }

        static void Main()
        {
            
                ASCIIConverter ascii = new ASCIIConverter("Analysis1");
                ascii.ConvertFile(@"C:\temp\234.txt");

                ascii.loadASCIIFile(@"C:\temp\234.txt");
            /*
                Console.WriteLine("File name: 234.txt");
                Console.WriteLine();

                List<Tuple<string, Vector<double>>> signals = ascii.getSignals();

                uint f = ascii.getFrequency();
                Console.WriteLine("Frequency: " + f + " Hz");

                uint samples = ascii.sampleAmount;
                Console.WriteLine("Sample amount: " + samples.ToString());
                Console.WriteLine();

                foreach (var tuple in signals)
                {
                    Console.WriteLine("Lead name: " + tuple.Item1);
                    Console.WriteLine("Signal Vector in mV: " + tuple.Item2);
                    Console.WriteLine();

                }
             * */

                TimeSpan dateTime = TimeSpan.ParseExact("16:23:01", "HH:mm:ss",
                                        CultureInfo.InvariantCulture);
                Console.WriteLine("DateTime: " + dateTime);
            
            Console.Read();
        }
    }
}
