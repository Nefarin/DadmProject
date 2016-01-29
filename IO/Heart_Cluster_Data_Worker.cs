using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EKG_Project.IO
{
    #region Documentation
    /// <summary>
    /// Stores Heart_Cluster_Attributes_I
    /// </summary>
    #endregion

    public enum Heart_Cluster_Attributes_I { TotalQrsComplex, NumberofClass };

    #region Documentation
    /// <summary>
    /// Stores Heart_Cluster_Attributes_II
    /// </summary>
    #endregion

    public enum Heart_Cluster_Attributes_II { indexOfClass, QrsComplexNo, indexOfRepresent };

    #region Documentation
    /// <summary>
    /// Class that saves and loads Heart_Cluster_Data from txt files
    /// </summary> 
    #endregion
    public class Heart_Cluster_Data_Worker
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
        public Heart_Cluster_Data_Worker() 
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
        }

        #region Documentation
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        #endregion

        public Heart_Cluster_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        #region Documentation
        /// <summary>
        /// Saves Heart_Cluster_Attributes_I
        /// </summary>
        /// <param name="atr">attribute</param>
        /// <param name="lead">lead</param>
        /// <param name="value">value</param> 
        #endregion
        public void SaveAttributeI(Heart_Cluster_Attributes_I atr, string lead,  uint value)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_"  + atr + ".txt";
            string pathOut = System.IO.Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut);
            sw.WriteLine(value);
            sw.Close();
        }

        #region Documentation
        /// <summary>
        /// Loads Heart_Cluster_Attributes_I
        /// </summary>
        /// <param name="atr">atribute</param>
        /// <param name="lead">lead</param>
        /// <returns>value</returns> 
        #endregion
        public uint LoadAttributeI(Heart_Cluster_Attributes_I atr, string lead)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_" + atr + ".txt";
            string pathIn = System.IO.Path.Combine(directory, fileName);

            StreamReader sr = new StreamReader(pathIn);
            string readLine = sr.ReadLine();
            sr.Close();

            uint readValue = Convert.ToUInt32(readLine);
            return readValue;
        }

        #region Documentation
        /// <summary>
        /// Saves Heart_Cluster_Attributes_II
        /// </summary>
        /// <param name="atr">attribute</param>
        /// <param name="lead">lead</param>
        /// <param name="value">value</param> 
        #endregion
        public void SaveAttributeII(Heart_Cluster_Attributes_II atr, string lead, int value)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_" + atr + ".txt";
            string pathOut = System.IO.Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut);
            sw.WriteLine(value);
            sw.Close();
        }

        #region Documentation
        /// <summary>
        /// Loads Heart_Cluster_Attributes_II
        /// </summary>
        /// <param name="atr">atribute</param>
        /// <param name="lead">lead</param>
        /// <returns>value</returns> 
        #endregion
        public int LoadAttributeII(Heart_Cluster_Attributes_II atr, string lead)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_" + atr + ".txt";
            string pathIn = System.IO.Path.Combine(directory, fileName);

            StreamReader sr = new StreamReader(pathIn);
            string readLine = sr.ReadLine();
            sr.Close();

            int readValue = Convert.ToInt32(readLine);
            return readValue;
        }

        #region Documentation
        /// <summary>
        /// Saves ChannelMliiDetected in txt file
        /// </summary>
        /// <param name="value">ChannelMliiDetected value</param> 
        #endregion
        public void SaveChannelMliiDetected(bool value)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_ChannelMliiDetected" + ".txt";
            string pathOut = System.IO.Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut);
            sw.WriteLine(value);
            sw.Close();
        }

        #region Documentation
        /// <summary>
        /// Loads ChannelMliiDetected from txt file
        /// </summary>
        /// <returns>ChannelMliiDetected bool</returns> 
        #endregion
        public bool LoadChannelMliiDetected()
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_ChannelMliiDetected" + ".txt";
            string pathIn = System.IO.Path.Combine(directory, fileName);

            StreamReader sr = new StreamReader(pathIn);
            string readLine = sr.ReadLine();
            sr.Close();

            bool readValue = Convert.ToBoolean(readLine);
            return readValue;
        }

        #region Documentation
        /// <summary>
        /// Saves NormalComplexPerc in txt file
        /// </summary>
        /// <param name="lead">lead</param> 
        /// <param name="value">NormalComplexPerc value</param> 
        #endregion
        public void SaveNormalComplexPerc(string lead, double value)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_NormalComplexPerc" + ".txt";
            string pathOut = System.IO.Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut);
            sw.WriteLine(value);
            sw.Close();
        }

        #region Documentation
        /// <summary>
        /// Loads NormalComplexPerc from txt file
        /// </summary>
        /// <param name="lead">lead</param> 
        /// <returns>NormalComplexPerc double</returns> 
        #endregion
        public double LoadNormalComplexPerc(string lead)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_NormalComplexPerc" + ".txt";
            string pathIn = System.IO.Path.Combine(directory, fileName);

            StreamReader sr = new StreamReader(pathIn);
            string readLine = sr.ReadLine();
            sr.Close();

            double readValue = Convert.ToDouble(readLine);
            return readValue;
        }

        #region Documentation
        /// <summary>
        /// Deletes all analysis files with Heart_Cluster_Data
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
