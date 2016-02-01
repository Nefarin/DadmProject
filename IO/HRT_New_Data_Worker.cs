using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EKG_Project.IO
{
    #region Documentation
    /// <summary>
    /// Class that saves and loads HRT_Data chunks from internal text file
    /// </summary>
    #endregion
    class HRT_New_Data_Worker
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
        public HRT_New_Data_Worker()
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
        }

        #region Documentation
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        #endregion
        public HRT_New_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        #region Documentation
        /// <summary>
        /// Saves HRT vectors in txt file
        /// </summary>
        /// <param name="signalName">HRT vector name enum</param>
        /// <param name="lead">Lead</param>
        /// <param name="mode">StreamWriter mode</param>
        /// <param name="Results">Data Vector</param>
        #endregion
        public void SaveHRTVector(HRT_Vectors signalName, string lead, bool mode, Vector<double> Results)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string FileName = analysisName + "_" + moduleName + "_" + lead + "_" + signalName.ToString() + ".txt";

            string PathOut = Path.Combine(directory, FileName);
            StreamWriter sw = new StreamWriter(PathOut, mode);
            foreach (var result in Results)
            {
                sw.WriteLine(result);
                sw.WriteLine("---");
            }
            sw.Close();
        }

        #region Documentation
        /// /// <summary>
        /// Loads HRT data vector from txt file
        /// </summary>
        /// <param name="signalName">HRT vector name enum</param>
        /// <param name="lead">Lead</param>
        /// <param name="startIndex"> Starting index</param>
        /// <param name="length">Length</param>
        /// <returns>Vector of specified HRT data</returns>
        #endregion
        public Vector<double> LoadHRTVector(HRT_Vectors signalName, string lead, int startIndex, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_" + signalName.ToString() + ".txt";
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
            List<double> list = new List<double>();
            while (iterator < length)
            {
                if (sr.EndOfStream)
                {
                    throw new IndexOutOfRangeException();
                }
                string item = sr.ReadLine();
                // Parsowanie typu string do double
                double sJsItem = double.Parse(item);
                list.Add(sJsItem);

                sr.ReadLine();
                iterator++;
            }
            sr.Close();
            Vector <double> HRTVector = Vector<double>.Build.Dense(list.ToArray<double>());
            return HRTVector;
        }

        #region Documentation
        /// <summary>
        /// Gets number of HRT vector samples
        /// </summary>
        /// <param name="signalName">HRT array name enum</param>
        /// <param name="lead"></param>
        /// <returns>Number of samples</returns>
        #endregion
        public uint getNumberOfVectorSamples(HRT_Vectors signalName, string lead)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_" + signalName.ToString() + ".txt";
            string path = Path.Combine(directory, fileName);

            uint count = 0;
            using (StreamReader r = new StreamReader(path))
            {
                while (r.ReadLine() != null)
                {
                    count++;
                }
            }

            count = count / 2;
            return count;
        }

        #region Documentation
        /// <summary>
        /// Saves HRT array in txt file
        /// </summary>
        /// <param name="signalName">HRT array name enum</param>
        /// <param name="lead">Lead</param>
        /// <param name="mode">StreamWriter mode</param>
        /// <param name="Results">Data array</param>
        #endregion
        public void SaveHRTArray(HRT_Arrays signalName, string lead, bool mode, double[] Results)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string FileName = analysisName + "_" + moduleName + "_" + lead + "_" + signalName.ToString() + ".txt";

            string PathOut = Path.Combine(directory, FileName);
            StreamWriter sw = new StreamWriter(PathOut, mode);
            foreach (var result in Results)
            {
                sw.WriteLine(result);
                sw.WriteLine("---");
            }
            sw.Close();
        }
        
        #region Documentation
        /// /// <summary>
        /// Loads HRT data array from txt file
        /// </summary>
        /// <param name="signalName">HRT array name enum</param>
        /// <param name="lead">Lead</param>
        /// <param name="startIndex"> Starting index</param>
        /// <param name="length">Length</param>
        /// <returns>Array of specified HRT data</returns>
        #endregion
        public double[] LoadHRTArray(HRT_Arrays signalName, string lead, int startIndex, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_" + signalName.ToString() + ".txt";
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
            List<double> list = new List<double>();
            while (iterator < length)
            {
                if (sr.EndOfStream)
                {
                    throw new IndexOutOfRangeException();
                }
                string item = sr.ReadLine();
                // Parsowanie typu string do double
                double sJsItem = double.Parse(item);
                list.Add(sJsItem);

                sr.ReadLine();
                iterator++;
            }
            sr.Close();
            return list.ToArray<double>();
        }

        #region Documentation
        /// <summary>
        /// Gets number of HRT array samples
        /// </summary>
        /// <param name="signalName">HRT array name enum</param>
        /// <param name="lead">Lead</param>
        /// <returns>Number of samples</returns>
        #endregion
        public uint getNumberOfArraySamples(HRT_Arrays signalName, string lead)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_" + signalName.ToString() + ".txt";
            string path = Path.Combine(directory, fileName);

            uint count = 0;
            using (StreamReader r = new StreamReader(path))
            {
                while (r.ReadLine() != null)
                {
                    count++;
                }
            }

            count = count / 2;
            return count;
        }
        
        #region Documentation
        /// <summary>
        /// Saves attribute in txt file
        /// </summary>
        /// <param name="mode">StreamWriter mode</param>
        /// <param name="lead">Lead</param>
        /// <param name="attribute">Attribute value</param>
        #endregion
        public void SaveAttribute(bool mode, string lead, int attributeValue)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string FileName = analysisName + "_" + moduleName + "_" + lead + "_VPCcount" + ".txt";

            string PathOut = Path.Combine(directory, FileName);
            StreamWriter sw = new StreamWriter(PathOut, mode);
            sw.WriteLine(attributeValue);
            sw.Close();
        }

        #region Documentation
        /// <summary>
        /// Loads attribute from txt file
        /// </summary>
        /// <param name="lead">Lead</param>
        /// <returns>Attribute value</returns>
        #endregion
        public int LoadAttribute(string lead)
        {
            int result = 0;
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string FileName = analysisName + "_" + moduleName + "_" + lead + "_VPCcount" + ".txt";
            string pathIn = Path.Combine(directory, FileName);

            StreamReader sr = new StreamReader(pathIn);
            string item = sr.ReadLine();
            result = int.Parse(item);
            sr.Close();

            return result;
        }
        
        #region Documentation
        /// <summary>
        /// Deletes all analysis files with HRT_New_Data_Worker
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

    #region Documentation
    /// <summary>
    /// HRT signals enum list
    /// </summary>
    #endregion
    public enum HRT_Vectors
    {
        TurbulenceOnset,
        TurbulenceSlope,
        VPCtachogram
    }

    #region Documentation
    /// <summary>
    /// HRT arrays enum list
    /// </summary>
    #endregion
    public enum HRT_Arrays
    {
        TurbulenceOnsetMean,
        TurbulenceSlopeMean
    }
}
