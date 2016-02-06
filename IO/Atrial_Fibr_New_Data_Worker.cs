using System;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules;
using System.Diagnostics;

namespace EKG_Project.IO
{
    public class Atrial_Fibr_New_Data_Worker
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
        public Atrial_Fibr_New_Data_Worker() 
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
        }

        #region Documentation
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        #endregion
        public Atrial_Fibr_New_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        #region Documentation
        /// <summary>
        /// Saves part of AfDetection signal in txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="signal">AfDetection</param>
        #endregion
        public void SaveAfDetection(string lead, bool mode, bool saveWholeTuple, Tuple<bool, Vector<double>, string, string> results)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + ".txt";
            string pathOut = Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut, mode);
            foreach (var sample in results.Item2)
            {
                sw.WriteLine(sample.ToString());
            }
            sw.Close();
            if (saveWholeTuple)
            {
                string fileNameAtr = analysisName + "_" + moduleName + "_" + lead + "_Atr" + ".txt";
                string pathOutAtr = Path.Combine(directory, fileNameAtr);
                StreamWriter swAtr = new StreamWriter(pathOutAtr, false);

                swAtr.WriteLine(results.Item1.ToString());
                swAtr.WriteLine(results.Item3);
                swAtr.WriteLine(results.Item4);
                swAtr.Close();
            }
        }

        #region Documentation
        /// <summary>
        /// Loads part of AfDetection signal from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="startIndex">vector start index</param>
        /// <param name="length">vector length</param>
        /// <returns>AfDetection tuple</returns>
        #endregion
        public Tuple<bool, Vector<double>, string, string> LoadAfDetection(string lead, int startIndex, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + ".txt";
            string pathIn = Path.Combine(directory, fileName);

            StreamReader sr = new StreamReader(pathIn);

            //pomijane linie ...
            int iterator = 0;
            while (iterator < startIndex && !sr.EndOfStream)
            {
                string readLine = sr.ReadLine();
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
                readSamples[iterator] = Convert.ToDouble(readLine);
                iterator++;
            }

            sr.Close();

            Vector<double> vector = Vector<double>.Build.Dense(readSamples.Length);
            vector.SetValues(readSamples);

            string fileNameAtr = analysisName + "_" + moduleName + "_" + lead + "_Atr" + ".txt";
            string pathInAtr = Path.Combine(directory, fileNameAtr);
            StreamReader srAtr = new StreamReader(pathInAtr);

            string readLine1 = srAtr.ReadLine();
            bool value1 = Convert.ToBoolean(readLine1);

            string readLine3 = srAtr.ReadLine();

            string readLine4 = srAtr.ReadLine();

            Tuple<bool, Vector<double>, string, string> tuple = Tuple.Create(value1, vector, readLine3, readLine4);

            return tuple;
        }

        /// <summary>
        /// Gets number of AfDetection vector samples 
        /// </summary>
        /// <param name="lead">lead</param>
        /// <returns>number of samples</returns>
        public uint getNumberOfSamples(string lead)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + ".txt";
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

        /// <summary>
        /// Deletes all analysis files with Atrial_Fibr_Data
        /// </summary>
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
