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

            List<Tuple<string, Vector<double>>> signals = getSignals();

            string readStartTime = columns[0][2];
            string cleanedStartTime = System.Text.RegularExpressions.Regex.Replace(readStartTime, @"\s+", "");
            DateTime startTime = DateTime.ParseExact(cleanedStartTime, "m:ss.fff", CultureInfo.InvariantCulture);

            string readStopTime = columns[0][lines.Length - 1].ToString();
            string cleanedStopTime = System.Text.RegularExpressions.Regex.Replace(readStopTime, @"\s+", "");
            DateTime stopTime = DateTime.ParseExact(cleanedStopTime, "m:ss.fff", CultureInfo.InvariantCulture);

            TimeSpan totalTime = stopTime - startTime;
            uint totalTimeValue = Convert.ToUInt32(totalTime.TotalSeconds);
            frequency = sampleAmount / totalTimeValue;

            return frequency;
        }


       public List<Tuple<string, Vector<double>>> getSignals()
       {
           List<Tuple<string, Vector<double>>> Signals = new List<Tuple<string, Vector<double>>>();
           
           Vector<double> signal = Vector<double>.Build.Dense(lines.Length - 2);
           for (int i = 1; i < columns.Count(); i++)
           {
               string leadName = columns[i][0];
               string cleanedLeadName = System.Text.RegularExpressions.Regex.Replace(leadName, @"\s+", "");

               for (int j = 2; j < lines.Count()-2; j++)
               {
                   double value = Convert.ToDouble(columns[i][j], new System.Globalization.NumberFormatInfo());
                   int index = j - 2;
                   signal.At(index, value);
               }

               getSampleAmount(signal);
               Tuple<string, Vector<double>> readedSignal = Tuple.Create(cleanedLeadName, signal);
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

    }
}
