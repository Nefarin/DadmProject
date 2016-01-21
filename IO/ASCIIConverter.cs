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
    /// <summary>
    /// Class that converts ASCII files
    /// </summary>
    class ASCIIConverter : IECGConverter
    {
        //FIELDS
        /// <summary>
        /// Stores analysis name
        /// </summary>
        string analysisName;

        /// <summary>
        /// Stores file lines in array
        /// </summary>
        string[] lines;

        /// <summary>
        /// Stores file columns in list 
        /// </summary>
        List<string>[] columns;

        /// <summary>
        /// Stores number of samples in signal
        /// </summary>
        uint sampleAmount;
        Basic_Data _data;

        /// <summary>
        /// Gets or sets Basic Data
        /// </summary>
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

        // METHODS
        /// <summary>
        /// Saves Basic Data in internal XML file
        /// </summary>
        public void SaveResult()
        {
            foreach (var property in Data.GetType().GetProperties())
            {

                if (property.GetValue(Data, null) == null)
                {
                    //throw new Exception();

                }
                else
                {
                    Basic_Data_Worker dataWorker = new Basic_Data_Worker(analysisName);
                    dataWorker.Save(Data);
                }
            }
        }

        /// <summary>
        /// Calls method loadASCIIFile and sets Basic Data
        /// </summary>
        /// <param name="path">input file path</param>
        public void ConvertFile(string path)
        {
            loadASCIIFile(path);
            Data = new Basic_Data();
            Data.Frequency = getFrequency();
            Data.Signals = getSignals();
            Data.SampleAmount = sampleAmount;
        }

        /// <summary>
        /// Loads ASCII input file and gets its lines and columns 
        /// </summary>
        /// <param name="path">input file path</param>
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

        /// <summary>
        /// Gets sampling frequency from input file
        /// </summary>
        /// <returns>sampling frequency</returns>
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

        /// <summary>
        /// Gets signals from input file
        /// </summary>
        /// <returns>signals</returns>
       public List<Tuple<string, Vector<double>>> getSignals()
       {
           List<Tuple<string, Vector<double>>> Signals = new List<Tuple<string, Vector<double>>>();
           for (int i = 1; i < columns.Count(); i++)
           {
               string leadName = columns[i][0];
               string cleanedLeadName = System.Text.RegularExpressions.Regex.Replace(leadName, @"\s+", "");

               Vector<double> signal = Vector<double>.Build.Dense(lines.Length - 2);
               for (int j = 2; j < lines.Count(); j++)
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

        /// <summary>
        /// Gets number of samples in signal
        /// </summary>
        /// <param name="signal">signal</param>
        /// <returns>number of samples</returns>
       public uint getSampleAmount(Vector<double> signal)
       {
           if (signal != null)
               sampleAmount = (uint)signal.Count;
           return sampleAmount;
       }

    }
}
