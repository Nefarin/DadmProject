using System;
using System.IO;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.IO
{
    #region Documentation
    /// <summary>
    /// Class that saves and loads Heart_Class_Data chunks from internal text file
    /// </summary>
    #endregion
    public class Heart_Class_New_Data_Worker
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
        public Heart_Class_New_Data_Worker()
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
        }

        #region Documentation
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        #endregion
        public Heart_Class_New_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        #region Documentation
        /// <summary>
        /// Saves ClassificationResult in txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="result">classification result</param>
        #endregion
        public void SaveClassificationResult(string lead, bool mode, List<Tuple<int, int>> results)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_ClassificationResult" + ".txt";
            string pathOut = Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut, mode);
            foreach (var result in results)
            {
                sw.WriteLine(result.Item1);
                sw.WriteLine(result.Item2);
                sw.WriteLine("---");
            }
            
            sw.Close();
        }

        #region Documentation
        /// <summary>
        /// Loads ClassificationResult from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <returns>classification result list</returns>
        #endregion
        public List<Tuple<int, int>> LoadClassificationResult(string lead, int startIndex, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_ClassificationResult" + ".txt";
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
            List<Tuple<int, int>> list = new List<Tuple<int, int>>();
            while (iterator < length)
            {
                if (sr.EndOfStream)
                {
                    throw new IndexOutOfRangeException();
                }
                string item1 = sr.ReadLine();
                int convertedItem1 = Convert.ToInt32(item1);

                string item2 = sr.ReadLine();
                int convertedItem2 = Convert.ToInt32(item2);

                Tuple<int, int> tuple = Tuple.Create(convertedItem1, convertedItem2);
                list.Add(tuple);

                sr.ReadLine();
                iterator++;
            }
            sr.Close();

            
            return list;
        }

        /// <summary>
        /// Gets number of ClassificationResult samples
        /// </summary>
        /// <param name="lead">lead</param>
        /// <returns>number of samples</returns>
        public uint getNumberOfSamples(string lead)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_ClassificationResult" + ".txt";
            string path = Path.Combine(directory, fileName);

            uint count = 0;
            using (StreamReader r = new StreamReader(path))
            {
                while (r.ReadLine() != null)
                {
                    count++;
                }
            }

            count = count / 3;
            return count;
        }

        /// <summary>
        /// Saves ChannelMliiDetected in txt file
        /// </summary>
        /// <param name="value">ChannelMliiDetected value</param>
        public void SaveChannelMliiDetected(bool value)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + ".txt";
            string pathOut = System.IO.Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut);
            sw.WriteLine(value);
            sw.Close();
        }

        /// <summary>
        /// Loads ChannelMliiDetected from txt file
        /// </summary>
        /// <returns>ChannelMliiDetected bool</returns>
        public bool LoadChannelMliiDetected()
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + ".txt";
            string pathIn = System.IO.Path.Combine(directory, fileName);

            StreamReader sr = new StreamReader(pathIn);
            string readLine = sr.ReadLine();
            sr.Close();

            bool readValue = Convert.ToBoolean(readLine);
            return readValue;
        }

        /// <summary>
        /// Saves CoefficientsForOneComplex in txt file
        /// </summary>
        /// <param name="value">CoefficientsForOneComplex</param>
        public void SaveCoefficientsForOneComplex(string lead, Tuple<int, Vector<double>> value)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_CoefficientsForOneComplex" + ".txt";
            string pathOut = System.IO.Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut);
            sw.WriteLine(value.Item1);
            sw.WriteLine("---");
            foreach(var sample in value.Item2)
            {
                sw.WriteLine(sample);
            }
            sw.Close();
        }
        
        /// <summary>
        /// Loads CoefficientsForOneComplex from txt file
        /// </summary>
        /// <returns>CoefficientsForOneComplex tuple</returns>
        public Tuple<int, Vector<double>> LoadCoefficientsForOneComplex(string lead)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_CoefficientsForOneComplex" + ".txt";
            string pathIn = System.IO.Path.Combine(directory, fileName);

            StreamReader sr = new StreamReader(pathIn);
            string readLine = sr.ReadLine();
            int readValue = Convert.ToInt32(readLine);

            sr.ReadLine();

            List<double> list = new List<double>();
            while(!sr.EndOfStream)
            {
                string rl = sr.ReadLine();
                double rv = Convert.ToDouble(rl);
                list.Add(rv);
            }
            sr.Close();

            Vector<double> vector = Vector<double>.Build.Dense(list.Count);
            vector.SetValues(list.ToArray());

            Tuple<int, Vector<double>> tuple = Tuple.Create(readValue, vector);
            return tuple;
        }

        /// <summary>
        /// Deletes all analysis files with Heart_Class_Data
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
