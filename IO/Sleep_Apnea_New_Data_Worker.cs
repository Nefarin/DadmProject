using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.IO
{
    public class Sleep_Apnea_New_Data_Worker
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
        /// Default constructor
        /// </summary>
        #endregion
        public Sleep_Apnea_New_Data_Worker() 
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
        }

        #region Documentation
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        #endregion
        public Sleep_Apnea_New_Data_Worker(String analysisName)  : this()
        {
            this.analysisName = analysisName;
        }

        #region Documentation
        /// <summary>
        /// Saves Detected_Apnea in txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="result">detected apnea result</param>
        #endregion
        public void SaveDetectedApnea(string lead, bool mode, List<Tuple<int, int>> results)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName1 = analysisName + "_" + moduleName + "_" + lead + "_Detected_Apnea" + "_Item1" +  ".txt";
            string pathOut1 = Path.Combine(directory, fileName1);

            StreamWriter sw1 = new StreamWriter(pathOut1, mode);
            foreach (var result in results)
            {
                sw1.WriteLine(result.Item1);
            }

            sw1.Close();

            string fileName2 = analysisName + "_" + moduleName + "_" + lead + "_Detected_Apnea" + "_Item2" + ".txt";
            string pathOut2 = Path.Combine(directory, fileName2);

            StreamWriter sw2 = new StreamWriter(pathOut2, mode);
            foreach (var result in results)
            {
                sw2.WriteLine(result.Item2);
            }

            sw2.Close();
        }

        #region Documentation
        /// <summary>
        /// Loads Detected_Apnea from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="startIndex">start index</param>
        /// <param name="length">length</param>
        /// <returns>detected apnea result list</returns>
        #endregion
        public List<Tuple<int, int>> LoadDetectedApnea(string lead, int startIndex, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName1 = analysisName + "_" + moduleName + "_" + lead + "_Detected_Apnea" + "_Item1" + ".txt";
            string pathIn1 = Path.Combine(directory, fileName1);

            StreamReader sr1 = new StreamReader(pathIn1);

            string fileName2 = analysisName + "_" + moduleName + "_" + lead + "_Detected_Apnea" + "_Item2" + ".txt";
            string pathIn2 = Path.Combine(directory, fileName2);

            StreamReader sr2 = new StreamReader(pathIn2);

            uint sampleNumber = getNumberOfSamples(lead);
            //pomijane linie ...
            int iterator = 0;
            while (iterator < startIndex && iterator < sampleNumber && !sr1.EndOfStream)
            {
                sr1.ReadLine();
                sr2.ReadLine();
                iterator++;
            }

            iterator = 0;
            List<Tuple<int, int>> list = new List<Tuple<int, int>>();
            while (iterator < length && iterator < sampleNumber)
            {
                if (sr1.EndOfStream)
                {
                    throw new IndexOutOfRangeException();
                }
                string item1 = sr1.ReadLine();
                int convertedItem1 = Convert.ToInt32(item1);

                string item2 = sr2.ReadLine();
                int convertedItem2 = Convert.ToInt32(item2);
                

                Tuple<int, int> tuple = Tuple.Create(convertedItem1, convertedItem2);
                list.Add(tuple);
                iterator++;
            }
            sr1.Close();
            sr2.Close();

            return list;
        }

        #region Documentation
        /// <summary>
        /// Gets number of Detected_Apnea samples
        /// </summary>
        /// <param name="lead">lead</param>
        /// <returns>number of samples</returns>
        #endregion
        public uint getNumberOfSamples(string lead)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_Detected_Apnea" + "_Item1" + ".txt";
            string path = Path.Combine(directory, fileName);

            uint count = 0;
            using (StreamReader r = new StreamReader(path))
            {
                while (r.ReadLine() != null)
                {
                    count++;
                }
            }
            return count;
        }

        #region Documentation
        /// <summary>
        /// Saves h_amp list in txt files
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="results">h_amp</param>
        #endregion
        public void SaveHAmp(string lead, bool mode, List<List<double>> results)
        {
            int iterator = 0;
            foreach (var result in results)
            {
                string moduleName = this.GetType().Name;
                moduleName = moduleName.Replace("_Data_Worker", "");
                string fileName = analysisName + "_" + moduleName + "_" + lead + "_" + "h_amp" + "_" + iterator + ".txt";
                string pathOut = Path.Combine(directory, fileName);

                StreamWriter sw = new StreamWriter(pathOut, mode);
                foreach (var value in result)
                {
                    sw.WriteLine(value);
                }
                sw.Close();
                iterator++;
            }
        }

        /// <summary>
        /// Loads part of h_amp from txt files
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="startIndex">start index</param>
        /// <param name="length">length</param>
        /// <returns>h_amp</returns>
        public List<List<double>> LoadHAmp(string lead, int[] startIndex, int[] length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileNamePattern = analysisName + "_" + moduleName + "_" + lead + "_" + "h_amp" + "*";
            string[] HAmpFiles = Directory.GetFiles(directory, fileNamePattern);

            int filesIndex = 0;
            uint[] samplesNumber = getHAmpNumberOfSamples(lead);
            List<List<double>> exList = new List<List<double>>();
            foreach(var file in HAmpFiles)
            {
                StreamReader sr = new StreamReader(file);
                //pomijane linie ...
                int iterator = 0;
                while (iterator < startIndex[filesIndex] && iterator < samplesNumber[filesIndex] && !sr.EndOfStream)
                {
                    string readLine = sr.ReadLine();
                    iterator++;
                }

                iterator = 0;
                List<double> inList = new List<double>();
                while (iterator < length[filesIndex] && iterator < samplesNumber[filesIndex])
                {
                    if (sr.EndOfStream)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    string readLine = sr.ReadLine();
                    double value = Convert.ToDouble(readLine);

                    inList.Add(value);
                    iterator++;
                }
                sr.Close();
                exList.Add(inList);
                filesIndex++;
            }

            return exList;
        }

        
        /// <summary>
        /// Gets h_amp numebr of samples
        /// </summary>
        /// <param name="lead">lead</param>
        /// <returns>array of number of samples</returns>
        public uint[] getHAmpNumberOfSamples(string lead)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileNamePattern = analysisName + "_" + moduleName + "_" + lead + "_" + "h_amp" + "*";
            string[] HAmpFiles = Directory.GetFiles(directory, fileNamePattern);

            int filesIndex = 0;
            uint[] count = new uint[HAmpFiles.Count()];
            foreach(var file in HAmpFiles)
            {
                using (StreamReader r = new StreamReader(file))
                {
                    while (r.ReadLine() != null)
                    {
                        count[filesIndex]++;
                    }
                }
                filesIndex++;
            }
            
            return count;
        }

        #region Documentation
        /// <summary>
        /// Saves il_Apnea in txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="value">value</param>
        #endregion
        public void SaveIlApnea(string lead, double value)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_il_Apnea" + ".txt";
            string pathOut = System.IO.Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut);
            sw.WriteLine(value);
            sw.Close();
        }

        #region Documentation
        /// <summary>
        /// Loads il_Apnea from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <returns>il_Apnea value</returns>
        #endregion
        public double LoadIlApnea(string lead)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_il_Apnea" + ".txt";
            string pathIn = System.IO.Path.Combine(directory, fileName);

            StreamReader sr = new StreamReader(pathIn);
            string readLine = sr.ReadLine();
            sr.Close();

            double readValue = Convert.ToDouble(readLine);
            return readValue;
        }

        #region Documentation
        /// <summary>
        /// Deletes all analysis files with Sleep_Apnea_Data
        /// </summary> 
        #endregion
        public void DeleteFiles()
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileNamePattern = analysisName + "_" + moduleName + "*";
            string[] analysisFiles = Directory.GetFiles(directory, fileNamePattern);

            foreach (string file in analysisFiles)
            {
                File.Delete(file);
            }

        }
    }
}
