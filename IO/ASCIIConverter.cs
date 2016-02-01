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
    #region Documentation
    /// <summary>
    /// Class that converts ASCII files
    /// </summary> 
    #endregion

    class ASCIIConverter : IECGConverter
    {
        //FIELDS
        #region Documentation
        /// <summary>
        /// Stores txt files directory
        /// </summary> 
        #endregion
        private string directory;

        #region Documentation
        /// <summary>
        /// Stores analysis name
        /// </summary> 
        #endregion
        private string analysisName;

        #region Documentation
        /// <summary>
        /// Stores file path
        /// </summary> 
        #endregion
        private string path;

        #region Documentation
        /// <summary>
        /// Stores sampling frequency
        /// </summary> 
        #endregion
        uint frequency;

        #region Documentation
        /// <summary>
        /// Stores number of samples
        /// </summary> 
        #endregion
        private uint numberOfSamples;

        #region Documentation
        /// <summary>
        /// Stores list of leads
        /// </summary> 
        #endregion
        private List<string> leads;

        #region Documentation
        /// <summary>
        /// Default constructor
        /// </summary>
        #endregion
        public ASCIIConverter()
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
        }

        #region Documentation
        /// <summary>
        /// Parametrized constructor
        /// </summary>
        /// <param name="ASCIIAnalysisName">analysis name</param>
        #endregion
        public ASCIIConverter (string ASCIIAnalysisName) : this()
        {
            analysisName = ASCIIAnalysisName;
        }

        // METHODS
        #region Documentation
        /// <summary>
        /// Saves freauency and lead names in txt files
        /// </summary> 
        #endregion
        public void SaveResult()
        {
            Basic_New_Data_Worker dataWorker = new Basic_New_Data_Worker(analysisName);
            dataWorker.SaveAttribute(Basic_Attributes.Frequency, frequency);
            dataWorker.SaveLeads(leads);
            
        }

        #region Documentation
        /// <summary>
        /// Sets input file path and Basic Data
        /// </summary>
        /// <param name="path">input file path</param> 
        #endregion
        public void ConvertFile(string path)
        {
            this.path = path;
            leads = getLeads();
            numberOfSamples = getNumberOfSamples(leads[0]);
            frequency = getFrequency();
        }

        #region Documentation
        /// <summary>
        /// Gets sampling frequency from input file
        /// </summary>
        /// <returns>sampling frequency</returns> 
        #endregion
       public uint getFrequency()
        {
            uint frequency = 0;

            StreamReader sr = new StreamReader(path);
            sr.ReadLine(); // header line 1
            sr.ReadLine(); // header line 2
            string readLine = sr.ReadLine();
            sr.Close();

            char[] columnDelimiter = new char[] { '\t' };
            string[] columns = readLine.Split(columnDelimiter, StringSplitOptions.RemoveEmptyEntries);

            string readStartTime = columns[0];
            string cleanedStartTime = System.Text.RegularExpressions.Regex.Replace(readStartTime, @"\s+", "");
            string startPattern = getTimeFormat(cleanedStartTime);
            DateTime startTime = DateTime.ParseExact(cleanedStartTime, startPattern, CultureInfo.InvariantCulture);

            string lastReadLine = File.ReadLines(path).Last();
            string[] lastColumns = lastReadLine.Split(columnDelimiter, StringSplitOptions.RemoveEmptyEntries);

            string readStopTime = lastColumns[0];
            string cleanedStopTime = System.Text.RegularExpressions.Regex.Replace(readStopTime, @"\s+", "");
            string stopPattern = getTimeFormat(cleanedStopTime);
            DateTime stopTime = DateTime.ParseExact(cleanedStopTime, stopPattern, CultureInfo.InvariantCulture);

            TimeSpan totalTime = stopTime - startTime;
            uint totalTimeValue = Convert.ToUInt32(totalTime.TotalSeconds);
            frequency = numberOfSamples / totalTimeValue;

            return frequency;
        }

       #region Documentation
       /// <summary>
       /// Gets time format of time column
       /// </summary>
       /// <param name="input">input form time column</param>
       /// <returns>time format</returns>
       #endregion
       public string getTimeFormat(string input)
       {
           string timeFormat = null;
           if (input.Count() == 8)
           {
               timeFormat = "m:ss.fff";
           }
           else if (input.Count() == 9)
           {
               timeFormat = "mm:ss.fff";
           }
           else if (input.Count() == 11)
           {
               timeFormat = "h:mm:ss.fff";
           }
           else if (input.Count() == 12)
           {
               timeFormat = "hh:mm:ss.fff";
           }
           return timeFormat;

       }
       #region Documentation
       /// <summary>
       /// Gets part of signal from input file
       /// </summary>
       /// <param name="lead">lead name</param>
       /// <param name="startIndex">start index</param>
       /// <param name="length">length</param>
       /// <returns>vector of samples</returns>
       #endregion
        public Vector<double> getSignal(string lead, int startIndex, int length)
        {
            Vector<double> vector = null;
            int signalColumn = leads.FindIndex(element => element == lead);

            StreamReader sr = new StreamReader(path);
            sr.ReadLine(); // header line 1
            sr.ReadLine(); // header line 2
            //pomijane linie ...
            int iterator = 0;
            while (iterator < startIndex && !sr.EndOfStream)
            {
                sr.ReadLine();
                iterator++;
            }

            iterator = 0;
            double[] readSamples = new double[length];
            while (iterator < length)
            {
                if (sr.EndOfStream)
                {
                    throw new IndexOutOfRangeException();
                }

                string readLine = sr.ReadLine();
                char[] columnDelimiter = new char[] { '\t' };
                string[] columns = readLine.Split(columnDelimiter, StringSplitOptions.RemoveEmptyEntries);
                readSamples[iterator] = Convert.ToDouble(columns[signalColumn + 1], new System.Globalization.NumberFormatInfo());
                iterator++;
            }

            sr.Close();

            try
            {
                // obsługa length == 0
                vector = Vector<double>.Build.Dense(readSamples.Length);
                vector.SetValues(readSamples);
            }
            catch (System.ArgumentOutOfRangeException e)
            {}
            

            return vector;
        }

        #region Documentation
        /// <summary>
        /// Gets lead names from input file
        /// </summary>
        /// <returns>lead names</returns>
        #endregion
        public List<string> getLeads()
        {
            StreamReader sr = new StreamReader(path);
            string headerLine = sr.ReadLine();
            sr.Close();

            char[] columnDelimiter = new char[] { '\t' };
            string[] headerColumns = headerLine.Split(columnDelimiter, StringSplitOptions.RemoveEmptyEntries);
            IEnumerable<string> cleanedHeaderColumns = headerColumns.Select(column => System.Text.RegularExpressions.Regex.Replace(column, @"\s+", ""));
            cleanedHeaderColumns = cleanedHeaderColumns.Skip(1);

            leads = new List<string>(cleanedHeaderColumns);
            return leads;
        }

        #region Documentation
        /// <summary>
        /// Gets number of signal samples 
        /// </summary>
        /// <param name="lead">lead</param>
        /// <returns>number of samples</returns>
        #endregion
        public uint getNumberOfSamples(string lead)
        {
            uint count = 0;
            foreach (var l in leads)
            {
                if (l == lead)
                {
                    StreamReader sr = new StreamReader(path);
                    sr.ReadLine(); // header line 1
                    sr.ReadLine(); // header line 2
                    while (!sr.EndOfStream)
                    {
                        sr.ReadLine();
                        count++;
                    }
                }
            }
            numberOfSamples = count;
            return count;
        }

        #region Documentation
        /// <summary>
        /// Deletes all analysis files
        /// </summary> 
        #endregion
        public void DeleteFiles()
        {
            string fileNamePattern = analysisName + "*";
            string[] analysisFiles = Directory.GetFiles(directory, fileNamePattern);

            foreach (string file in analysisFiles)
            {
                File.Delete(file);
            }
        } 
    }
}
