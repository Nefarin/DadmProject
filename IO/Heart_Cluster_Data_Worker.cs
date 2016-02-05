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

    public enum Heart_Cluster_Attributes_II { indexOfClass, QrsComplexNo }; //

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
        /// Saves Clusterization Result in txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="result">clusterization result</param>

        #endregion
        public void SaveClusterizationResult(string lead, bool mode, List<Tuple<int, int, int, int>> results)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName1 = analysisName + "_" + moduleName + "_" + lead + "_ClusterizationResult" + "_Item1" + ".txt";
            string pathOut1 = Path.Combine(directory, fileName1);

            StreamWriter sw1 = new StreamWriter(pathOut1, mode);
            foreach (var result in results)
            {
                sw1.WriteLine(result.Item1);
            }

            sw1.Close();

            string fileName2 = analysisName + "_" + moduleName + "_" + lead + "_ClusterizationResult" + "_Item2" + ".txt";
            string pathOut2 = Path.Combine(directory, fileName2);

            StreamWriter sw2 = new StreamWriter(pathOut2, mode);
            foreach (var result in results)
            {
                sw2.WriteLine(result.Item2);
            }

            sw2.Close();

            string fileName3 = analysisName + "_" + moduleName + "_" + lead + "_ClusterizationResult" + "_Item3" + ".txt";
            string pathOut3 = Path.Combine(directory, fileName3);

            StreamWriter sw3 = new StreamWriter(pathOut3, mode);
            foreach (var result in results)
            {
                sw3.WriteLine(result.Item3);
            }

            sw3.Close();


            string fileName4 = analysisName + "_" + moduleName + "_" + lead + "_ClusterizationResult" + "_Item4" + ".txt";
            string pathOut4 = Path.Combine(directory, fileName4);

            StreamWriter sw4 = new StreamWriter(pathOut4, mode);
            foreach (var result in results)
            {
                sw4.WriteLine(result.Item4);
            }

            sw4.Close();
        }

        #region Documentation
        /// <summary>
        /// Loads ClassificationResult from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="startIndex">start index</param>
        /// <param name="length">length</param>
        /// <returns>classification result list</returns>
        #endregion
        public List<Tuple<int, int, int, int>> LoadClusterizationResult(string lead, int startIndex, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName1 = analysisName + "_" + moduleName + "_" + lead + "_ClusterizationResult" + "_Item1" + ".txt";
            string pathIn1 = Path.Combine(directory, fileName1);

            StreamReader sr1 = new StreamReader(pathIn1);

            string fileName2 = analysisName + "_" + moduleName + "_" + lead + "_ClusterizationResult" + "_Item2" + ".txt";
            string pathIn2 = Path.Combine(directory, fileName2);

            StreamReader sr2 = new StreamReader(pathIn2);

            string fileName3 = analysisName + "_" + moduleName + "_" + lead + "_ClusterizationResult" + "_Item3" + ".txt";
            string pathIn3 = Path.Combine(directory, fileName3);

            StreamReader sr3 = new StreamReader(pathIn3);

            string fileName4 = analysisName + "_" + moduleName + "_" + lead + "_ClusterizationResult" + "_Item4" + ".txt";
            string pathIn4 = Path.Combine(directory, fileName4);

            StreamReader sr4 = new StreamReader(pathIn4);

            //pomijane linie ...
            int iterator = 0;
            while (iterator < startIndex && !sr1.EndOfStream)
            {
                sr1.ReadLine();
                sr2.ReadLine();
                sr3.ReadLine();
                sr4.ReadLine();
                iterator++;
            }

            iterator = 0;
            List<Tuple<int, int, int, int>> list = new List<Tuple<int, int, int, int>>();
            while (iterator < length)
            {
                if (sr1.EndOfStream)
                {
                    throw new IndexOutOfRangeException();
                }
                string item1 = sr1.ReadLine();
                int convertedItem1 = Convert.ToInt32(item1);

                string item2 = sr2.ReadLine();
                int convertedItem2 = Convert.ToInt32(item2);

                string item3 = sr3.ReadLine();
                int convertedItem3 = Convert.ToInt32(item3);

                string item4 = sr4.ReadLine();
                int convertedItem4 = Convert.ToInt32(item4);


                Tuple<int, int, int, int> tuple = Tuple.Create(convertedItem1, convertedItem2, convertedItem3, convertedItem4);
                list.Add(tuple);
                iterator++;
            }
            sr1.Close();
            sr2.Close();
            sr3.Close();
            sr4.Close();

            return list;
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
