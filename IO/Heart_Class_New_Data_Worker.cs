using System;
using System.IO;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.IO
{
    #region Documentation
    /// <summary>
    /// Class that saves and loads Heart_Class_Data chunks from internal text file
    /// </summary>
    #endregion
    class Heart_Class_New_Data_Worker
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
        public void SaveSignal(string lead, bool mode, Tuple<int, int> result)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + ".txt";
            string pathOut = Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut, mode);

            sw.WriteLine(result.Item1);
            sw.WriteLine(result.Item2);
            
            sw.Close();
        }

        #region Documentation
        /// <summary>
        /// Loads ClassificationResult from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <returns>classification result tuple</returns>
        #endregion
        public Tuple<int, int> LoadSignal(string lead)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + ".txt";
            string pathIn = Path.Combine(directory, fileName);

            StreamReader sr = new StreamReader(pathIn);

            string item1 = sr.ReadLine();
            int convertedItem1 = Convert.ToInt32(item1);

            string item2 = sr.ReadLine();
            int convertedItem2 = Convert.ToInt32(item2);

            sr.Close();

            Tuple<int, int> tuple = Tuple.Create(convertedItem1, convertedItem2);
            return tuple;
        }

        /// <summary>
        /// Saves ChannelMliiDetected in txt file
        /// </summary>
        /// <param name="value">ChannelMliiDetected value</param>
        public void SaveAttribute(bool value)
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
        public bool LoadAttribute()
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
    }
}
